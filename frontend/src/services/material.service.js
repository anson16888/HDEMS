/**
 * 物资管理服务
 * 对接后端真实 API
 */

import { materialApi } from '../api/material.api'

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
    if (!this.initialized) {
      this.initialized = true
    }
  }

  /**
   * 获取物资列表
   * @param {Object} params - 查询参数
   * @param {string} params.keyword - 关键词
   * @param {string} params.type - 物资类型 (字符串: 'MEDICAL', 'MEDICINE' 等)
   * @param {string} params.status - 状态 (字符串: 'NORMAL', 'EXPIRED' 等)
   * @param {string} params.hospitalId - 医院ID
   * @param {number} params.page - 页码
   * @param {number} params.pageSize - 每页条数
   * @returns {Promise}
   */
  async list(params = {}) {
    await this.initialize()

    // 转换前端参数到后端格式
    const queryParams = {
      keyword: params.keyword,
      page: params.page,
      pageSize: params.pageSize,
      hospitalId: params.hospitalId
    }

    // 转换物资类型字符串为数字
    if (params.type) {
      const typeMap = {
        'MEDICAL': 1,
        'MEDICINE': 2,
        'EMERGENCY': 3,
        'CONSUMABLE': 4,
        'EQUIPMENT': 5
      }
      queryParams.materialType = typeMap[params.type]
    }

    // 转换状态字符串为数字
    if (params.status) {
      const statusMap = {
        'NORMAL': 0,
        'LOW': 1,
        'OUT': 2,
        'EXPIRED': 3,
        'EXPIRING_SOON': 4
      }
      queryParams.status = statusMap[params.status]
    }

    return materialApi.getMaterials(queryParams)
  }

  /**
   * 获取物资详情
   * @param {string} id - 物资ID
   * @returns {Promise}
   */
  async getById(id) {
    await this.initialize()
    return materialApi.getMaterialById(id)
  }

  /**
   * 新增物资
   * @param {Object} data - 物资数据
   * @returns {Promise}
   */
  async create(data) {
    await this.initialize()

    // 验证必填字段
    this.validateMaterialData(data)

    return materialApi.createMaterial(data)
  }

  /**
   * 更新物资
   * @param {string} id - 物资ID
   * @param {Object} data - 更新数据
   * @returns {Promise}
   */
  async update(id, data) {
    await this.initialize()

    // 验证必填字段
    this.validateMaterialData(data)

    return materialApi.updateMaterial(id, data)
  }

  /**
   * 删除物资
   * @param {string} id - 物资ID
   * @returns {Promise}
   */
  async delete(id) {
    await this.initialize()
    return materialApi.deleteMaterial(id)
  }

  /**
   * 批量删除物资
   * @param {Array<string>} ids - 物资ID数组
   * @returns {Promise}
   */
  async batchDelete(ids) {
    await this.initialize()
    return materialApi.batchDeleteMaterials(ids)
  }

  /**
   * 导入 Excel
   * @param {File} file - Excel 文件
   * @returns {Promise}
   */
  async import(file) {
    await this.initialize()
    return materialApi.importMaterials(file)
  }

  /**
   * 导出 Excel
   * @param {Object} filters - 筛选条件
   * @param {string} filters.keyword - 关键词
   * @param {string} filters.type - 物资类型
   * @param {string} filters.status - 状态
   * @param {string} filters.hospitalId - 医院ID
   * @returns {Promise}
   */
  async export(filters = {}) {
    await this.initialize()

    // 转换物资类型字符串为数字
    const exportFilters = { ...filters }
    if (filters.type) {
      const typeMap = {
        'MEDICAL': 1,
        'MEDICINE': 2,
        'EMERGENCY': 3,
        'CONSUMABLE': 4,
        'EQUIPMENT': 5
      }
      exportFilters.materialType = typeMap[filters.type]
    }

    // 转换状态字符串为数字
    if (filters.status) {
      const statusMap = {
        'NORMAL': 0,
        'LOW': 1,
        'OUT': 2,
        'EXPIRED': 3,
        'EXPIRING_SOON': 4
      }
      exportFilters.status = statusMap[filters.status]
    }

    return materialApi.exportMaterials(exportFilters)
  }

  /**
   * 下载导入模板
   * @returns {Promise}
   */
  async downloadTemplate() {
    await this.initialize()
    return materialApi.downloadTemplate()
  }

  /**
   * 获取物资统计信息
   * @param {string} hospitalId - 医院ID（可选）
   * @returns {Promise}
   */
  async getStatistics(hospitalId = null) {
    await this.initialize()
    return materialApi.getStatistics(hospitalId)
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
    const validTypes = ['MEDICAL', 'MEDICINE', 'EMERGENCY', 'CONSUMABLE', 'EQUIPMENT']
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

    // 存放位置
    if (!data.location) {
      throw { code: 'VALIDATION_ERROR', message: '存放位置不能为空' }
    }

    // // 医院ID（根据后端要求，这是必填字段）
    // if (!data.hospital_id) {
    //   throw { code: 'VALIDATION_ERROR', message: '请选择所属医院' }
    // }
  }

  /**
   * 获取物资类型显示名称
   * @param {string} typeCode - 类型代码
   * @returns {string} 显示名称
   */
  getTypeDisplayName(typeCode) {
    return materialApi.getTypeDisplayName(typeCode)
  }

  /**
   * 获取状态显示名称
   * @param {string} statusCode - 状态代码
   * @returns {string} 显示名称
   */
  getStatusDisplayName(statusCode) {
    return materialApi.getStatusDisplayName(statusCode)
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
  batchDelete: (ids) => materialServiceInstance.batchDelete(ids),
  import: (file) => materialServiceInstance.import(file),
  export: (filters) => materialServiceInstance.export(filters),
  downloadTemplate: () => materialServiceInstance.downloadTemplate(),
  getStatistics: (hospitalId) => materialServiceInstance.getStatistics(hospitalId)
}

export default materialService
