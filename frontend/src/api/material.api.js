/**
 * 物资管理 API
 * 对接后端真实接口
 * 参考: swagger.json
 */

import { request } from '../utils/http'
import { API_ENDPOINTS, MATERIAL_TYPE, MATERIAL_STATUS } from '../config/api.config'

/**
 * 物资类型映射 - 前端使用字符串，后端使用数字
 */
const MATERIAL_TYPE_MAP = {
  1: 'MEDICAL',     // 医疗设备
  2: 'MEDICINE',    // 药品
  3: 'EMERGENCY',   // 急救物资
  4: 'CONSUMABLE',  // 耗材
  5: 'EQUIPMENT'    // 设备
}

/**
 * 物资状态映射 - 前端使用字符串，后端使用数字
 */
const MATERIAL_STATUS_MAP = {
  0: 'NORMAL',           // 正常
  1: 'LOW',              // 库存偏低
  2: 'OUT',              // 已耗尽
  3: 'EXPIRED',          // 已过期
  4: 'EXPIRING_SOON'     // 即将过期
}

/**
 * 反向映射 - 前端字符串转后端数字
 */
const MATERIAL_TYPE_REVERSE_MAP = {
  'MEDICAL': 1,
  'MEDICINE': 2,
  'EMERGENCY': 3,
  'CONSUMABLE': 4,
  'EQUIPMENT': 5
}

const MATERIAL_STATUS_REVERSE_MAP = {
  'NORMAL': 0,
  'LOW': 1,
  'OUT': 2,
  'EXPIRED': 3,
  'EXPIRING_SOON': 4
}

/**
 * 转换物资数据 - 后端格式转前端格式
 */
function transformMaterialFromBackend(material) {
  if (!material) return null

  return {
    id: material.id,
    material_code: material.materialCode,
    material_name: material.materialName,
    material_type: MATERIAL_TYPE_MAP[material.materialType] || material.materialType,
    materialTypeName: material.materialTypeName,
    specification: material.specification,
    quantity: material.quantity,
    unit: material.unit,
    production_date: material.productionDate,
    shelf_life: material.shelfLife,
    expiry_date: material.expiryDate,
    location: material.location,
    hospital_id: material.hospitalId,
    hospital_name: material.hospitalName,
    remark: material.remark,
    status: MATERIAL_STATUS_MAP[material.status] || material.status,
    statusName: material.statusName,
    created_at: material.createdAt,
    updated_at: material.updatedAt
  }
}

/**
 * 转换物资数据 - 前端格式转后端格式
 */
function transformMaterialToFrontend(data) {
  const result = {
    materialCode: data.material_code,
    materialName: data.material_name,
    materialType: data.material_type ? MATERIAL_TYPE_REVERSE_MAP[data.material_type] : undefined,
    specification: data.specification,
    quantity: data.quantity,
    unit: data.unit,
    productionDate: data.production_date,
    shelfLife: data.shelf_life,
    location: data.location,
    hospitalId: data.hospital_id,
    remark: data.remark
  }

  // 移除 undefined 值
  Object.keys(result).forEach(key => {
    if (result[key] === undefined) {
      delete result[key]
    }
  })

  return result
}

/**
 * 转换分页数据 - 后端格式转前端格式
 */
function transformPagedDataFromBackend(response) {
  if (!response) return { list: [], total: 0, page: 1, pageSize: 20 }

  return {
    list: (response.items || []).map(transformMaterialFromBackend),
    total: response.total || 0,
    page: response.page || 1,
    pageSize: response.pageSize || 20,
    totalPages: response.totalPages,
    hasPrevious: response.hasPrevious,
    hasNext: response.hasNext
  }
}

/**
 * 物资管理 API 类
 */
class MaterialApi {
  /**
   * 获取物资列表
   * @param {Object} params - 查询参数
   * @param {string} params.keyword - 关键词搜索
   * @param {number} params.materialType - 物资类型 (1-5)
   * @param {number} params.status - 物资状态 (0-4)
   * @param {string} params.hospitalId - 医院ID
   * @param {number} params.page - 页码
   * @param {number} params.pageSize - 每页条数
   * @returns {Promise} 返回分页数据
   */
  async getMaterials(params = {}) {
    const queryParams = {
      Keyword: params.keyword,
      MaterialType: params.materialType,
      Status: params.status,
      HospitalId: params.hospitalId,
      Page: params.page || 1,
      PageSize: params.pageSize || 20
    }

    // 移除空值
    Object.keys(queryParams).forEach(key => {
      if (queryParams[key] === undefined || queryParams[key] === null || queryParams[key] === '') {
        delete queryParams[key]
      }
    })

    const response = await request.get(API_ENDPOINTS.MATERIAL.LIST, queryParams)
    return transformPagedDataFromBackend(response.data)
  }

  /**
   * 获取物资详情
   * @param {string} id - 物资ID
   * @returns {Promise} 返回物资详情
   */
  async getMaterialById(id) {
    const response = await request.get(API_ENDPOINTS.MATERIAL.DETAIL(id))
    return transformMaterialFromBackend(response.data)
  }

  /**
   * 新增物资
   * @param {Object} data - 物资数据
   * @returns {Promise} 返回创建的物资
   */
  async createMaterial(data) {
    const backendData = transformMaterialToFrontend(data)
    const response = await request.post(API_ENDPOINTS.MATERIAL.CREATE, backendData)
    return transformMaterialFromBackend(response.data)
  }

  /**
   * 更新物资
   * @param {string} id - 物资ID
   * @param {Object} data - 更新数据
   * @returns {Promise} 返回更新后的物资
   */
  async updateMaterial(id, data) {
    const backendData = transformMaterialToFrontend(data)
    const response = await request.put(API_ENDPOINTS.MATERIAL.UPDATE(id), backendData)
    return transformMaterialFromBackend(response.data)
  }

  /**
   * 删除物资
   * @param {string} id - 物资ID
   * @returns {Promise}
   */
  async deleteMaterial(id) {
    return request.delete(API_ENDPOINTS.MATERIAL.DELETE(id))
  }

  /**
   * 批量删除物资
   * @param {Array<string>} ids - 物资ID数组
   * @returns {Promise}
   */
  async batchDeleteMaterials(ids) {
    return request.post(API_ENDPOINTS.MATERIAL.BATCH_DELETE, ids)
  }

  /**
   * 导入物资
   * @param {File} file - Excel 文件
   * @returns {Promise} 返回导入结果
   */
  async importMaterials(file) {
    const formData = new FormData()
    formData.append('file', file)

    const response = await request.upload(API_ENDPOINTS.MATERIAL.IMPORT, formData)

    // 后端返回 MaterialImportResult
    return {
      success: response.data.successCount,
      failed: response.data.failedCount,
      total: response.data.totalCount,
      errors: response.data.errors || []
    }
  }

  /**
   * 导出物资
   * @param {Object} filters - 筛选条件
   * @param {string} filters.keyword - 关键词
   * @param {number} filters.materialType - 物资类型
   * @param {number} filters.status - 状态
   * @param {string} filters.hospitalId - 医院ID
   * @param {number} filters.page - 页码
   * @param {number} filters.pageSize - 每页条数
   * @returns {Promise} 返回文件 Blob
   */
  async exportMaterials(filters = {}) {
    const queryParams = {
      keyword: filters.keyword,
      materialType: filters.materialType,
      status: filters.status,
      hospitalId: filters.hospitalId,
      page: filters.page || 1,
      pageSize: filters.pageSize || 20
    }

    // 移除空值
    Object.keys(queryParams).forEach(key => {
      if (queryParams[key] === undefined || queryParams[key] === null || queryParams[key] === '') {
        delete queryParams[key]
      }
    })

    const response = await request.post(API_ENDPOINTS.MATERIAL.EXPORT, queryParams, {
      responseType: 'blob'
    })
    // 返回实际的 blob 数据
    return response.data
  }

  /**
   * 获取物资统计信息
   * @param {string} hospitalId - 医院ID（可选）
   * @returns {Promise} 返回统计数据
   */
  async getStatistics(hospitalId = null) {
    const params = hospitalId ? { hospitalId } : {}
    const response = await request.get(API_ENDPOINTS.MATERIAL.STATISTICS, params)
    return response.data
  }

  /**
   * 下载导入模板
   * @returns {Promise} 返回文件 Blob
   */
  async downloadTemplate() {
    const response = await request.get(API_ENDPOINTS.IMPORT_EXPORT.MATERIAL_TEMPLATE, {}, {
      responseType: 'blob'
    })
    // 返回实际的 blob 数据
    return response.data
  }

  /**
   * 获取物资类型显示名称
   * @param {number|string} type - 物资类型
   * @returns {string} 显示名称
   */
  getTypeDisplayName(type) {
    const typeMap = {
      1: '医疗设备',
      2: '药品',
      3: '急救物资',
      4: '耗材',
      5: '设备',
      'MEDICAL': '医疗设备',
      'MEDICINE': '药品',
      'EMERGENCY': '急救物资',
      'CONSUMABLE': '耗材',
      'EQUIPMENT': '设备'
    }
    return typeMap[type] || type
  }

  /**
   * 获取状态显示名称
   * @param {number|string} status - 状态
   * @returns {string} 显示名称
   */
  getStatusDisplayName(status) {
    const statusMap = {
      0: '正常',
      1: '库存偏低',
      2: '已耗尽',
      3: '已过期',
      4: '即将过期',
      'NORMAL': '正常',
      'LOW': '库存偏低',
      'OUT': '已耗尽',
      'EXPIRED': '已过期',
      'EXPIRING_SOON': '即将过期'
    }
    return statusMap[status] ?? '未知'
  }
}

// 导出单例实例
export const materialApi = new MaterialApi()

// 导出默认实例
export default materialApi
