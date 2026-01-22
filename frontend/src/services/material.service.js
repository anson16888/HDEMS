/**
 * 物资管理服务
 * 支持Mock模式和API模式切换
 */

import { materialStore } from '../mock/material-store.js'

// Mock模式开关 - 设置为true使用mock数据
const USE_MOCK = true

/**
 * MaterialService - 物资管理业务逻辑
 * 处理物资的CRUD操作
 */
export class MaterialService {
  constructor() {
    this.initialized = false
  }

  /**
   * 初始化物资服务
   * @returns {Promise<void>}
   */
  async initialize() {
    if (USE_MOCK && !this.initialized) {
      await materialStore.initialize()
      this.initialized = true
    }
  }

  /**
   * 获取物资列表
   * @param {Object} params - 查询参数 { page, pageSize, keyword, type, status }
   * @returns {Promise}
   */
  async list(params = {}) {
    await this.initialize()

    if (USE_MOCK) {
      await new Promise(resolve => setTimeout(resolve, 200)) // Simulate delay

      let materials = materialStore.getMaterials()

      // Apply filters
      if (params.keyword) {
        const keyword = params.keyword.toLowerCase()
        materials = materials.filter(m =>
          m.material_name?.toLowerCase().includes(keyword) ||
          m.material_code?.toLowerCase().includes(keyword) ||
          m.specification?.toLowerCase().includes(keyword)
        )
      }

      if (params.type) {
        materials = materials.filter(m => m.material_type === params.type)
      }

      if (params.status) {
        materials = materials.filter(m => m.status === params.status)
      }

      // Pagination
      const page = params.page || 1
      const pageSize = params.pageSize || 20
      const start = (page - 1) * pageSize
      const end = start + pageSize
      const paginatedMaterials = materials.slice(start, end)

      return {
        data: {
          list: paginatedMaterials,
          total: materials.length,
          page,
          pageSize
        }
      }
    } else {
      // Real API call
      const axios = (await import('axios')).default
      const API_BASE = '/api/v1/materials'

      const token = this.getAuthToken()
      return axios.get(API_BASE, {
        headers: {
          'Content-Type': 'application/json',
          'Authorization': token ? `Bearer ${token}` : ''
        },
        params
      })
    }
  }

  /**
   * 获取物资详情
   * @param {string} id - 物资ID
   * @returns {Promise}
   */
  async getById(id) {
    await this.initialize()

    if (USE_MOCK) {
      await new Promise(resolve => setTimeout(resolve, 100))

      const material = materialStore.getMaterialById(id)
      if (!material) {
        throw { code: 'MATERIAL_NOT_FOUND', message: '物资不存在' }
      }

      return { data: material }
    } else {
      const axios = (await import('axios')).default
      const API_BASE = '/api/v1/materials'
      const token = this.getAuthToken()

      return axios.get(`${API_BASE}/${id}`, {
        headers: {
          'Content-Type': 'application/json',
          'Authorization': token ? `Bearer ${token}` : ''
        }
      })
    }
  }

  /**
   * 新增物资
   * @param {Object} data - 物资数据
   * @returns {Promise}
   */
  async create(data) {
    await this.initialize()

    if (USE_MOCK) {
      await new Promise(resolve => setTimeout(resolve, 300))

      // Validate required fields
      this.validateMaterialData(data)

      const newMaterial = materialStore.createMaterial(data)

      return { data: newMaterial }
    } else {
      const axios = (await import('axios')).default
      const API_BASE = '/api/v1/materials'
      const token = this.getAuthToken()

      return axios.post(API_BASE, data, {
        headers: {
          'Content-Type': 'application/json',
          'Authorization': token ? `Bearer ${token}` : ''
        }
      })
    }
  }

  /**
   * 更新物资
   * @param {string} id - 物资ID
   * @param {Object} data - 更新数据
   * @returns {Promise}
   */
  async update(id, data) {
    await this.initialize()

    if (USE_MOCK) {
      await new Promise(resolve => setTimeout(resolve, 300))

      const updatedMaterial = materialStore.updateMaterial(id, data)

      return { data: updatedMaterial }
    } else {
      const axios = (await import('axios')).default
      const API_BASE = '/api/v1/materials'
      const token = this.getAuthToken()

      return axios.put(`${API_BASE}/${id}`, data, {
        headers: {
          'Content-Type': 'application/json',
          'Authorization': token ? `Bearer ${token}` : ''
        }
      })
    }
  }

  /**
   * 删除物资
   * @param {string} id - 物资ID
   * @returns {Promise}
   */
  async delete(id) {
    await this.initialize()

    if (USE_MOCK) {
      await new Promise(resolve => setTimeout(resolve, 200))

      const material = materialStore.getMaterialById(id)
      if (!material) {
        throw { code: 'MATERIAL_NOT_FOUND', message: '物资不存在' }
      }

      materialStore.deleteMaterial(id)

      return { data: { success: true } }
    } else {
      const axios = (await import('axios')).default
      const API_BASE = '/api/v1/materials'
      const token = this.getAuthToken()

      return axios.delete(`${API_BASE}/${id}`, {
        headers: {
          'Content-Type': 'application/json',
          'Authorization': token ? `Bearer ${token}` : ''
        }
      })
    }
  }

  /**
   * 导入 Excel
   * @param {File} file - Excel 文件
   * @returns {Promise}
   */
  async import(file) {
    await this.initialize()

    if (USE_MOCK) {
      await new Promise(resolve => setTimeout(resolve, 1000))

      // Mock import - simulate parsing Excel file
      // In real implementation, would parse the actual file
      const mockImportData = [
        {
          material_name: '导入示例物资1',
          material_type: 'MEDICAL',
          specification: '示例规格',
          quantity: 100,
          unit: '个',
          production_date: '2025-01-01',
          shelf_life: 12,
          min_stock: 50,
          location: '导入位置'
        }
      ]

      const result = materialStore.batchImport(mockImportData)

      return {
        data: {
          success: result.success,
          failed: result.failed,
          total: result.total,
          message: `导入完成：成功${result.success}条，失败${result.failed}条`
        }
      }
    } else {
      const axios = (await import('axios')).default
      const API_BASE = '/api/v1/materials'
      const token = this.getAuthToken()

      const formData = new FormData()
      formData.append('file', file)

      return axios.post(`${API_BASE}/import`, formData, {
        headers: {
          'Content-Type': 'multipart/form-data',
          'Authorization': token ? `Bearer ${token}` : ''
        }
      })
    }
  }

  /**
   * 导出 Excel
   * @param {Object} filters - 筛选条件 { keyword, type, status }
   * @returns {Promise}
   */
  async export(filters = {}) {
    await this.initialize()

    if (USE_MOCK) {
      await new Promise(resolve => setTimeout(resolve, 500))

      // Mock export - in real implementation would generate actual Excel file
      let materials = materialStore.getMaterials()

      // Apply filters
      if (filters.keyword) {
        const keyword = filters.keyword.toLowerCase()
        materials = materials.filter(m =>
          m.material_name?.toLowerCase().includes(keyword) ||
          m.material_code?.toLowerCase().includes(keyword)
        )
      }

      if (filters.type) {
        materials = materials.filter(m => m.material_type === filters.type)
      }

      if (filters.status) {
        materials = materials.filter(m => m.status === filters.status)
      }

      // Return mock blob
      const mockData = JSON.stringify(materials, null, 2)
      const blob = new Blob([mockData], { type: 'application/json' })

      return {
        data: blob,
        headers: {
          'content-type': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        }
      }
    } else {
      const axios = (await import('axios')).default
      const API_BASE = '/api/v1/materials'
      const token = this.getAuthToken()

      return axios.get(`${API_BASE}/export`, {
        headers: {
          'Content-Type': 'application/json',
          'Authorization': token ? `Bearer ${token}` : ''
        },
        params: filters,
        responseType: 'blob'
      })
    }
  }

  /**
   * 下载导入模板
   * @returns {Promise}
   */
  async downloadTemplate() {
    await this.initialize()

    if (USE_MOCK) {
      await new Promise(resolve => setTimeout(resolve, 300))

      // Mock template download
      const templateData = [
        {
          material_code: 'MAT000001',
          material_name: '物资名称示例',
          material_type: 'MEDICAL',
          specification: '规格说明',
          quantity: 100,
          unit: '个',
          production_date: '2025-01-01',
          shelf_life: 12,
          min_stock: 50,
          location: '存放位置',
          supplier: '供应商'
        }
      ]

      const mockData = JSON.stringify(templateData, null, 2)
      const blob = new Blob([mockData], { type: 'application/json' })

      return {
        data: blob,
        headers: {
          'content-type': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        }
      }
    } else {
      const axios = (await import('axios')).default
      const API_BASE = '/api/v1/materials'
      const token = this.getAuthToken()

      return axios.get(`${API_BASE}/template`, {
        headers: {
          'Content-Type': 'application/json',
          'Authorization': token ? `Bearer ${token}` : ''
        },
        responseType: 'blob'
      })
    }
  }

  /**
   * 获取认证 Token
   * @returns {string|null}
   */
  getAuthToken() {
    try {
      const userStr = localStorage.getItem('hdems_user')
      if (userStr) {
        const user = JSON.parse(userStr)
        return user.token || null
      }
      return null
    } catch (error) {
      console.error('Failed to get auth token:', error)
      return null
    }
  }

  /**
   * 验证物资数据
   * @param {Object} data - 物资数据
   * @throws {Error} 验证错误
   */
  validateMaterialData(data) {
    // 物资名称
    if (!data.material_name) {
      throw { code: 'VALIDATION_ERROR', message: '物资名称不能为空' }
    }
    if (data.material_name.length > 100) {
      throw { code: 'VALIDATION_ERROR', message: '物资名称最多100个字符' }
    }

    // 物资类型
    if (!data.material_type) {
      throw { code: 'VALIDATION_ERROR', message: '请选择物资类型' }
    }
    const validTypes = ['FOOD', 'MEDICAL', 'EQUIPMENT', 'CLOTHING', 'OTHER']
    if (!validTypes.includes(data.material_type)) {
      throw { code: 'VALIDATION_ERROR', message: '无效的物资类型' }
    }

    // 单位
    if (!data.unit) {
      throw { code: 'VALIDATION_ERROR', message: '单位不能为空' }
    }

    // 库存数量
    if (data.quantity === undefined || data.quantity === null) {
      throw { code: 'VALIDATION_ERROR', message: '库存数量不能为空' }
    }
    if (parseFloat(data.quantity) < 0) {
      throw { code: 'VALIDATION_ERROR', message: '库存数量不能为负数' }
    }

    // 最小库存
    if (data.min_stock !== undefined && data.min_stock !== null) {
      if (parseFloat(data.min_stock) < 0) {
        throw { code: 'VALIDATION_ERROR', message: '最小库存不能为负数' }
      }
    }

    // 质保期
    if (data.shelf_life !== undefined && data.shelf_life !== null) {
      if (parseInt(data.shelf_life) < 0) {
        throw { code: 'VALIDATION_ERROR', message: '质保期不能为负数' }
      }
    }

    // 存放位置
    if (!data.location) {
      throw { code: 'VALIDATION_ERROR', message: '存放位置不能为空' }
    }
  }

  /**
   * 获取物资类型显示名称
   * @param {string} typeCode - 类型代码
   * @returns {string} 显示名称
   */
  getTypeDisplayName(typeCode) {
    const typeNames = {
      'FOOD': '食品类',
      'MEDICAL': '医疗用品',
      'EQUIPMENT': '救援设备',
      'CLOTHING': '衣物类',
      'OTHER': '其他'
    }
    return typeNames[typeCode] || typeCode
  }

  /**
   * 获取状态显示名称
   * @param {string} statusCode - 状态代码
   * @returns {string} 显示名称
   */
  getStatusDisplayName(statusCode) {
    const statusNames = {
      'NORMAL': '正常',
      'LOW': '库存偏低',
      'OUT': '已耗尽',
      'EXPIRED': '已过期'
    }
    return statusNames[statusCode] || statusCode
  }
}

// 导出兼容的对象和类实例
const materialServiceInstance = new MaterialService()

// 兼容旧版API：导出对象形式的方法
export const materialService = {
  list: (params) => materialServiceInstance.list(params),
  getById: (id) => materialServiceInstance.getById(id),
  create: (data) => materialServiceInstance.create(data),
  update: (id, data) => materialServiceInstance.update(id, data),
  delete: (id) => materialServiceInstance.delete(id),
  import: (file) => materialServiceInstance.import(file),
  export: (filters) => materialServiceInstance.export(filters),
  downloadTemplate: () => materialServiceInstance.downloadTemplate()
}

export default materialService
