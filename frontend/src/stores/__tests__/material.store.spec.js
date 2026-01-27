import { describe, it, expect, beforeEach, vi } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useMaterialStore } from '../material.store'
import { materialService } from '../../services/material.service'
import {
  mockMaterialData,
  mockMaterialList,
  mockPaginatedResponse,
  mockStatistics
} from '../../test-utils'

// Mock the material service
vi.mock('../../services/material.service', () => ({
  materialService: {
    list: vi.fn(),
    getById: vi.fn(),
    create: vi.fn(),
    update: vi.fn(),
    delete: vi.fn(),
    batchDelete: vi.fn(),
    import: vi.fn(),
    export: vi.fn(),
    downloadTemplate: vi.fn(),
    getStatistics: vi.fn()
  }
}))

describe('Material Store', () => {
  let store

  beforeEach(() => {
    // Create a fresh pinia instance for each test
    setActivePinia(createPinia())
    store = useMaterialStore()

    // Clear all mocks
    vi.clearAllMocks()
  })

  describe('State Initialization', () => {
    it('should initialize with empty materials array', () => {
      expect(store.materials).toEqual([])
    })

    it('should initialize with loading as false', () => {
      expect(store.loading).toBe(false)
    })

    it('should initialize pagination with default values', () => {
      expect(store.pagination).toEqual({
        current: 1,
        pageSize: 20,
        total: 0
      })
    })

    it('should initialize filters with empty values', () => {
      expect(store.filters).toEqual({
        keyword: '',
        type: undefined,
        status: undefined,
        hospitalId: undefined
      })
    })
  })

  describe('Actions - fetchMaterials', () => {
    it('should fetch materials successfully', async () => {
      const mockData = mockMaterialList(3)
      const mockResponse = mockPaginatedResponse(mockData, 1, 20)

      materialService.list.mockResolvedValue(mockResponse.data)

      await store.fetchMaterials()

      expect(materialService.list).toHaveBeenCalledWith({
        page: 1,
        pageSize: 20,
        keyword: '',
        type: undefined,
        status: undefined,
        hospitalId: undefined
      })
      expect(store.materials).toEqual(mockData)
      expect(store.pagination.total).toBe(3)
      expect(store.loading).toBe(false)
    })

    it('should set loading to true during fetch', async () => {
      materialService.list.mockImplementation(() => {
        expect(store.loading).toBe(true)
        return Promise.resolve({ list: [], total: 0 })
      })

      await store.fetchMaterials()
    })

    it('should handle fetch errors', async () => {
      const error = new Error('Network error')
      materialService.list.mockRejectedValue(error)

      await expect(store.fetchMaterials()).rejects.toThrow('Network error')
      expect(store.loading).toBe(false)
    })

    it('should apply filters when fetching', async () => {
      store.filters.keyword = '口罩'
      store.filters.type = 'type-001'
      store.filters.status = 'LOW'

      materialService.list.mockResolvedValue({ list: [], total: 0 })

      await store.fetchMaterials()

      expect(materialService.list).toHaveBeenCalledWith(
        expect.objectContaining({
          keyword: '口罩',
          type: 'type-001',
          status: 'LOW'
        })
      )
    })
  })

  describe('Actions - createMaterial', () => {
    it('should create material successfully', async () => {
      const newMaterial = mockMaterialData({ id: 'new-001' })
      const mockResponse = { success: true, data: newMaterial }

      materialService.create.mockResolvedValue(mockResponse)
      materialService.list.mockResolvedValue({ list: [newMaterial], total: 1 })

      await store.createMaterial(newMaterial)

      expect(materialService.create).toHaveBeenCalledWith(newMaterial)
      expect(materialService.list).toHaveBeenCalled()
    })

    it('should handle create errors', async () => {
      const error = new Error('Validation failed')
      materialService.create.mockRejectedValue(error)

      await expect(store.createMaterial({})).rejects.toThrow('Validation failed')
      expect(store.loading).toBe(false)
    })
  })

  describe('Actions - updateMaterial', () => {
    it('should update material successfully', async () => {
      const updatedMaterial = mockMaterialData({ quantity: 200 })
      const mockResponse = { success: true, data: updatedMaterial }

      materialService.update.mockResolvedValue(mockResponse)
      materialService.list.mockResolvedValue({ list: [updatedMaterial], total: 1 })

      await store.updateMaterial('1234567890', updatedMaterial)

      expect(materialService.update).toHaveBeenCalledWith('1234567890', updatedMaterial)
      expect(materialService.list).toHaveBeenCalled()
    })

    it('should handle update errors', async () => {
      const error = new Error('Material not found')
      materialService.update.mockRejectedValue(error)

      await expect(store.updateMaterial('invalid-id', {})).rejects.toThrow('Material not found')
    })
  })

  describe('Actions - deleteMaterial', () => {
    it('should delete material successfully', async () => {
      const materials = mockMaterialList(3)
      store.materials = [...materials]
      store.pagination.total = 3

      materialService.delete.mockResolvedValue({ success: true })

      await store.deleteMaterial('material-2')

      expect(materialService.delete).toHaveBeenCalledWith('material-2')
      expect(store.materials).toHaveLength(2)
      expect(store.materials.find(m => m.id === 'material-2')).toBeUndefined()
      expect(store.pagination.total).toBe(2)
    })

    it('should handle delete errors', async () => {
      const error = new Error('Delete failed')
      materialService.delete.mockRejectedValue(error)

      await expect(store.deleteMaterial('invalid-id')).rejects.toThrow('Delete failed')
    })
  })

  describe('Actions - batchDeleteMaterials', () => {
    it('should batch delete materials successfully', async () => {
      const materials = mockMaterialList(5)
      store.materials = [...materials]
      store.pagination.total = 5

      materialService.batchDelete.mockResolvedValue({ success: true })

      await store.batchDeleteMaterials(['material-1', 'material-3'])

      expect(materialService.batchDelete).toHaveBeenCalledWith(['material-1', 'material-3'])
      expect(store.materials).toHaveLength(3)
      expect(store.pagination.total).toBe(3)
    })
  })

  describe('Actions - importMaterials', () => {
    it('should import materials successfully', async () => {
      const file = new File([''], 'materials.xlsx')
      const mockResponse = { success: true, imported: 10 }

      materialService.import.mockResolvedValue(mockResponse)
      materialService.list.mockResolvedValue({ list: mockMaterialList(10), total: 10 })

      const result = await store.importMaterials(file)

      expect(materialService.import).toHaveBeenCalledWith(file)
      expect(result).toEqual(mockResponse)
      expect(materialService.list).toHaveBeenCalled()
    })
  })

  describe('Actions - exportMaterials', () => {
    it('should export materials successfully', async () => {
      const blob = new Blob(['data'], { type: 'application/vnd.ms-excel' })
      materialService.export.mockResolvedValue(blob)

      const result = await store.exportMaterials({ keyword: '口罩' })

      expect(materialService.export).toHaveBeenCalledWith({
        keyword: '口罩',
        type: undefined,
        status: undefined,
        hospitalId: undefined
      })
      expect(result).toBe(blob)
    })
  })

  describe('Getters - filteredMaterials', () => {
    beforeEach(() => {
      store.materials = [
        mockMaterialData({ id: '1', material_name: '医用口罩', material_type: 'type-001', status: 'NORMAL' }),
        mockMaterialData({ id: '2', material_name: '防护服', material_type: 'type-002', status: 'LOW' }),
        mockMaterialData({ id: '3', material_name: 'N95口罩', material_type: 'type-001', status: 'OUT' })
      ]
    })

    it('should return all materials when no filters applied', () => {
      expect(store.filteredMaterials).toHaveLength(3)
    })

    it('should filter by keyword', () => {
      store.filters.keyword = '口罩'
      expect(store.filteredMaterials).toHaveLength(2)
      expect(store.filteredMaterials.every(m => m.material_name.includes('口罩'))).toBe(true)
    })

    it('should filter by type', () => {
      store.filters.type = 'type-001'
      expect(store.filteredMaterials).toHaveLength(2)
      expect(store.filteredMaterials.every(m => m.material_type === 'type-001')).toBe(true)
    })

    it('should filter by status', () => {
      store.filters.status = 'LOW'
      expect(store.filteredMaterials).toHaveLength(1)
      expect(store.filteredMaterials[0].status).toBe('LOW')
    })

    it('should apply multiple filters', () => {
      store.filters.keyword = '口罩'
      store.filters.type = 'type-001'
      expect(store.filteredMaterials).toHaveLength(2)
    })
  })

  describe('Getters - statistics', () => {
    it('should calculate statistics correctly', () => {
      store.materials = [
        mockMaterialData({ status: 'NORMAL' }),
        mockMaterialData({ status: 'NORMAL' }),
        mockMaterialData({ status: 'LOW' }),
        mockMaterialData({ status: 'OUT' }),
        mockMaterialData({ status: 'EXPIRED' }),
        mockMaterialData({ status: 'EXPIRING_SOON' })
      ]

      const stats = store.statistics

      expect(stats.total).toBe(6)
      expect(stats.normal).toBe(2)
      expect(stats.low).toBe(1)
      expect(stats.out).toBe(1)
      expect(stats.expired).toBe(1)
      expect(stats.expiringSoon).toBe(1)
    })

    it('should return zero statistics for empty materials', () => {
      const stats = store.statistics

      expect(stats.total).toBe(0)
      expect(stats.normal).toBe(0)
      expect(stats.low).toBe(0)
      expect(stats.out).toBe(0)
      expect(stats.expired).toBe(0)
      expect(stats.expiringSoon).toBe(0)
    })
  })

  describe('Filter Management', () => {
    it('should set filters', () => {
      store.setFilters({ keyword: '测试', type: 'type-001' })

      expect(store.filters.keyword).toBe('测试')
      expect(store.filters.type).toBe('type-001')
    })

    it('should reset filters', () => {
      store.filters.keyword = '测试'
      store.filters.type = 'type-001'
      store.filters.status = 'LOW'
      store.pagination.current = 5

      store.resetFilters()

      expect(store.filters.keyword).toBe('')
      expect(store.filters.type).toBeUndefined()
      expect(store.filters.status).toBeUndefined()
      expect(store.pagination.current).toBe(1)
    })
  })

  describe('Pagination Management', () => {
    it('should set pagination', () => {
      store.setPagination({ current: 3, pageSize: 50 })

      expect(store.pagination.current).toBe(3)
      expect(store.pagination.pageSize).toBe(50)
    })

    it('should partially update pagination', () => {
      store.setPagination({ current: 2 })

      expect(store.pagination.current).toBe(2)
      expect(store.pagination.pageSize).toBe(20) // unchanged
    })
  })
})
