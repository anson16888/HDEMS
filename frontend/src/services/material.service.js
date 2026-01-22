import axios from 'axios'

/**
 * 物资管理 API 服务
 */
const API_BASE = '/api/v1/materials'

/**
 * 获取认证 Token
 */
function getAuthToken() {
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
 * 获取请求头（包含认证 Token）
 */
function getHeaders() {
  const token = getAuthToken()
  return {
    'Content-Type': 'application/json',
    'Authorization': token ? `Bearer ${token}` : ''
  }
}

/**
 * 物资服务对象
 */
export const materialService = {
  /**
   * 获取物资列表
   * @param {Object} params - 查询参数 { page, pageSize, keyword, type, status }
   * @returns {Promise}
   */
  list(params = {}) {
    return axios.get(API_BASE, {
      headers: getHeaders(),
      params
    })
  },

  /**
   * 获取物资详情
   * @param {string} id - 物资ID
   * @returns {Promise}
   */
  getById(id) {
    return axios.get(`${API_BASE}/${id}`, {
      headers: getHeaders()
    })
  },

  /**
   * 新增物资
   * @param {Object} data - 物资数据
   * @returns {Promise}
   */
  create(data) {
    return axios.post(API_BASE, data, {
      headers: getHeaders()
    })
  },

  /**
   * 更新物资
   * @param {string} id - 物资ID
   * @param {Object} data - 更新数据
   * @returns {Promise}
   */
  update(id, data) {
    return axios.put(`${API_BASE}/${id}`, data, {
      headers: getHeaders()
    })
  },

  /**
   * 删除物资
   * @param {string} id - 物资ID
   * @returns {Promise}
   */
  delete(id) {
    return axios.delete(`${API_BASE}/${id}`, {
      headers: getHeaders()
    })
  },

  /**
   * 导入 Excel
   * @param {File} file - Excel 文件
   * @returns {Promise}
   */
  import(file) {
    const formData = new FormData()
    formData.append('file', file)

    return axios.post(`${API_BASE}/import`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
        'Authorization': getAuthToken() ? `Bearer ${getAuthToken()}` : ''
      }
    })
  },

  /**
   * 导出 Excel
   * @param {Object} filters - 筛选条件 { keyword, type, status }
   * @returns {Promise}
   */
  export(filters = {}) {
    return axios.get(`${API_BASE}/export`, {
      headers: getHeaders(),
      params: filters,
      responseType: 'blob'
    })
  },

  /**
   * 下载导入模板
   * @returns {Promise}
   */
  downloadTemplate() {
    return axios.get(`${API_BASE}/template`, {
      headers: getHeaders(),
      responseType: 'blob'
    })
  }
}

export default materialService
