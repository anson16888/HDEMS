/**
 * Material Type API
 * Manage material type dictionary data
 */

import { request } from '../utils/http.js'
import { API_ENDPOINTS } from '../config/api.config.js'

/**
 * Get material types list with pagination
 * @param {object} params - Query params { page, pageSize, keyword, isEnabled }
 * @returns {Promise<object>} Paged material types
 */
export function getMaterialTypes(params = {}) {
  const queryParams = {
    Page: params.page,
    PageSize: params.pageSize,
    Keyword: params.keyword,
    IsEnabled: params.isEnabled
  }
  return request.get(API_ENDPOINTS.MATERIAL_TYPE.LIST, queryParams)
}

/**
 * Get material type by id
 * @param {number} id - Material type id
 * @returns {Promise<object>} Material type
 */
export function getMaterialTypeById(id) {
  return request.get(API_ENDPOINTS.MATERIAL_TYPE.DETAIL(id))
}

/**
 * Get material type by code
 * @param {string} code - Type code
 * @returns {Promise<object>} Material type
 */
export function getMaterialTypeByCode(code) {
  return request.get(API_ENDPOINTS.MATERIAL_TYPE.BY_CODE(code))
}

/**
 * Get enabled material type options
 * @returns {Promise<object>} Material type options
 */
export function getEnabledMaterialTypes() {
  return request.get(API_ENDPOINTS.MATERIAL_TYPE.ENABLED)
}

/**
 * Create material type
 * @param {object} data - Material type data
 * @returns {Promise<object>} Created material type
 */
export function createMaterialType(data) {
  return request.post(API_ENDPOINTS.MATERIAL_TYPE.LIST, data)
}

/**
 * Update material type
 * @param {number} id - Material type id
 * @param {object} data - Update data
 * @returns {Promise<object>} Updated material type
 */
export function updateMaterialType(id, data) {
  return request.put(API_ENDPOINTS.MATERIAL_TYPE.DETAIL(id), data)
}

/**
 * Delete material type
 * @param {number} id - Material type id
 * @returns {Promise<object>}
 */
export function deleteMaterialType(id) {
  return request.delete(API_ENDPOINTS.MATERIAL_TYPE.DETAIL(id))
}

/**
 * Batch delete material types
 * @param {number[]} ids - Material type ids
 * @returns {Promise<object>}
 */
export function batchDeleteMaterialTypes(ids) {
  return request.delete(API_ENDPOINTS.MATERIAL_TYPE.BATCH_DELETE, undefined, { data: ids })
}

/**
 * Toggle material type enabled status
 * @param {number} id - Material type id
 * @returns {Promise<object>}
 */
export function toggleMaterialType(id) {
  return request.patch(API_ENDPOINTS.MATERIAL_TYPE.TOGGLE(id))
}

export default {
  getMaterialTypes,
  getMaterialTypeById,
  getMaterialTypeByCode,
  getEnabledMaterialTypes,
  createMaterialType,
  updateMaterialType,
  deleteMaterialType,
  batchDeleteMaterialTypes,
  toggleMaterialType
}
