/**
 * MaterialStore - localStorage-based material data management
 * Handles CRUD operations for material entities
 */

import { storageService } from '../services/storage.service.js'
import { generateSeedMaterials } from './seed-data.js'

const MATERIALS_KEY = 'materials'

export class MaterialStore {
  constructor() {
    this.initialized = false
  }

  /**
   * Initialize store with seed data if empty
   */
  async initialize() {
    if (this.initialized) return

    const materials = storageService.get(MATERIALS_KEY)

    if (!materials || materials.length === 0) {
      // Generate and store seed materials
      const seedMaterials = await generateSeedMaterials()
      storageService.set(MATERIALS_KEY, seedMaterials)

      console.log('📦 MaterialStore initialized with seed data:', seedMaterials.length, 'materials')
    }

    this.initialized = true
  }

  /**
   * Get all materials
   * @returns {Array} All materials
   */
  getMaterials() {
    return storageService.get(MATERIALS_KEY) || []
  }

  /**
   * Get material by ID
   * @param {string} id - Material ID
   * @returns {object|null} Material object or null
   */
  getMaterialById(id) {
    const materials = this.getMaterials()
    return materials.find(m => m.id === id) || null
  }

  /**
   * Get material by code
   * @param {string} code - Material code
   * @returns {object|null} Material object or null
   */
  getMaterialByCode(code) {
    const materials = this.getMaterials()
    return materials.find(m => m.material_code === code) || null
  }

  /**
   * Create new material
   * @param {object} materialData - Material data
   * @returns {object} Created material
   */
  createMaterial(materialData) {
    const materials = this.getMaterials()

    // Check code uniqueness
    if (materialData.material_code) {
      if (materials.find(m => m.material_code === materialData.material_code)) {
        throw new Error('MATERIAL_CODE_EXISTS')
      }
    }

    const now = new Date().toISOString()

    // Auto-generate code if not provided
    if (!materialData.material_code) {
      materialData.material_code = this.generateMaterialCode(materials)
    }

    // Calculate status and expiry date
    const status = this.calculateStatus(materialData)
    const expiry_date = this.calculateExpiryDate(materialData.production_date, materialData.shelf_life)

    const newMaterial = {
      id: `material-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`,
      ...materialData,
      status,
      expiry_date,
      created_at: now,
      updated_at: now
    }

    materials.push(newMaterial)
    storageService.set(MATERIALS_KEY, materials)

    return newMaterial
  }

  /**
   * Update material
   * @param {string} id - Material ID
   * @param {object} updates - Fields to update
   * @returns {object} Updated material
   */
  updateMaterial(id, updates) {
    const materials = this.getMaterials()
    const index = materials.findIndex(m => m.id === id)

    if (index === -1) {
      throw new Error('MATERIAL_NOT_FOUND')
    }

    // Check code uniqueness if updating code
    if (updates.material_code) {
      const existing = materials.find(m => m.material_code === updates.material_code && m.id !== id)
      if (existing) {
        throw new Error('MATERIAL_CODE_EXISTS')
      }
    }

    const currentMaterial = materials[index]

    // Recalculate status and expiry date if relevant fields changed
    let status = currentMaterial.status
    let expiry_date = currentMaterial.expiry_date

    if (updates.production_date !== undefined || updates.shelf_life !== undefined ||
        updates.quantity !== undefined || updates.min_stock !== undefined) {
      status = this.calculateStatus({
        ...currentMaterial,
        ...updates
      })
    }

    if (updates.production_date !== undefined || updates.shelf_life !== undefined) {
      const productionDate = updates.production_date !== undefined
        ? updates.production_date
        : currentMaterial.production_date
      const shelfLife = updates.shelf_life !== undefined
        ? updates.shelf_life
        : currentMaterial.shelf_life
      expiry_date = this.calculateExpiryDate(productionDate, shelfLife)
    }

    const updatedMaterial = {
      ...materials[index],
      ...updates,
      id: materials[index].id, // Preserve ID
      material_code: updates.material_code || materials[index].material_code, // Preserve code if not updating
      status,
      expiry_date,
      updated_at: new Date().toISOString()
    }

    materials[index] = updatedMaterial
    storageService.set(MATERIALS_KEY, materials)

    return updatedMaterial
  }

  /**
   * Delete material
   * @param {string} id - Material ID
   */
  deleteMaterial(id) {
    const materials = this.getMaterials()
    const filtered = materials.filter(m => m.id !== id)

    if (filtered.length === materials.length) {
      throw new Error('MATERIAL_NOT_FOUND')
    }

    storageService.set(MATERIALS_KEY, filtered)
  }

  /**
   * Batch import materials
   * @param {Array} materialsData - Array of material data
   * @returns {object} Import result { success, failed, total }
   */
  batchImport(materialsData) {
    const materials = this.getMaterials()
    let successCount = 0
    let failedCount = 0
    const failedRows = []

    materialsData.forEach((data, index) => {
      try {
        // Check for duplicate code
        if (data.material_code && materials.find(m => m.material_code === data.material_code)) {
          throw new Error('MATERIAL_CODE_EXISTS')
        }

        // Auto-generate code if not provided
        if (!data.material_code) {
          data.material_code = this.generateMaterialCode(materials)
        }

        const now = new Date().toISOString()
        const status = this.calculateStatus(data)
        const expiry_date = this.calculateExpiryDate(data.production_date, data.shelf_life)

        const newMaterial = {
          id: `material-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`,
          ...data,
          status,
          expiry_date,
          created_at: now,
          updated_at: now
        }

        materials.push(newMaterial)
        successCount++
      } catch (error) {
        failedCount++
        failedRows.push({ row: index + 1, data, error: error.message })
      }
    })

    storageService.set(MATERIALS_KEY, materials)

    return {
      success: successCount,
      failed: failedCount,
      total: materialsData.length,
      failedRows
    }
  }

  /**
   * Generate material code
   * @param {Array} existingMaterials - Existing materials array
   * @returns {string} Generated code
   */
  generateMaterialCode(existingMaterials = []) {
    const materials = existingMaterials.length > 0 ? existingMaterials : this.getMaterials()
    const prefix = 'MAT'
    const maxNum = materials.reduce((max, m) => {
      const match = m.material_code?.match(/^MAT(\d+)$/)
      if (match) {
        const num = parseInt(match[1], 10)
        return Math.max(max, num)
      }
      return max
    }, 0)

    return `${prefix}${String(maxNum + 1).padStart(6, '0')}`
  }

  /**
   * Calculate material status based on quantity and expiry
   * @param {object} material - Material data
   * @returns {string} Status code
   */
  calculateStatus(material) {
    const { quantity, min_stock, production_date, shelf_life } = material

    // Check if expired
    if (production_date && shelf_life) {
      const expiryDate = this.calculateExpiryDate(production_date, shelf_life)
      if (expiryDate && new Date(expiryDate) < new Date()) {
        return 'EXPIRED'
      }
    }

    // Check stock level
    const qty = parseFloat(quantity) || 0
    const min = parseFloat(min_stock) || 0

    if (qty <= 0) {
      return 'OUT'
    } else if (qty <= min) {
      return 'LOW'
    } else {
      return 'NORMAL'
    }
  }

  /**
   * Calculate expiry date
   * @param {string|Date} productionDate - Production date
   * @param {number} shelfLife - Shelf life in months
   * @returns {string|null} Expiry date in ISO format
   */
  calculateExpiryDate(productionDate, shelfLife) {
    if (!productionDate || !shelfLife) return null

    const prodDate = new Date(productionDate)
    if (isNaN(prodDate.getTime())) return null

    const expiryDate = new Date(prodDate)
    expiryDate.setMonth(expiryDate.getMonth() + parseInt(shelfLife))

    return expiryDate.toISOString().split('T')[0]
  }

  /**
   * Get statistics
   * @returns {object} Statistics object
   */
  getStatistics() {
    const materials = this.getMaterials()

    return {
      total: materials.length,
      normal: materials.filter(m => m.status === 'NORMAL').length,
      low: materials.filter(m => m.status === 'LOW').length,
      out: materials.filter(m => m.status === 'OUT').length,
      expired: materials.filter(m => m.status === 'EXPIRED').length
    }
  }
}

// Export singleton instance
export const materialStore = new MaterialStore()
export default materialStore
