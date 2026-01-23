/**
 * Material Threshold API
 * Manage material threshold settings
 */

import { request } from '../utils/http.js'
import { API_ENDPOINTS } from '../config/api.config.js'

/**
 * Get material thresholds list with pagination
 * @param {object} params - Query params { page, pageSize, keyword, isEnabled }
 * @returns {Promise<object>} Paged material thresholds
 */
export function getMaterialThresholds(params = {}) {
  const queryParams = {
    Page: params.page,
    PageSize: params.pageSize,
    Keyword: params.keyword,
    IsEnabled: params.isEnabled
  }
  return request.get(API_ENDPOINTS.MATERIAL_THRESHOLD.LIST, queryParams)
}

/**
 * Get material threshold by id
 * @param {string} id - Material threshold id
 * @returns {Promise<object>} Material threshold
 */
export function getMaterialThresholdById(id) {
  return request.get(API_ENDPOINTS.MATERIAL_THRESHOLD.DETAIL(id))
}

/**
 * Create material threshold
 * @param {object} data - Material threshold data
 * @returns {Promise<object>} Created material threshold
 */
export function createMaterialThreshold(data) {
  return request.post(API_ENDPOINTS.MATERIAL_THRESHOLD.LIST, data)
}

/**
 * Update material threshold
 * @param {string} id - Material threshold id
 * @param {object} data - Update data
 * @returns {Promise<object>} Updated material threshold
 */
export function updateMaterialThreshold(id, data) {
  return request.put(API_ENDPOINTS.MATERIAL_THRESHOLD.DETAIL(id), data)
}

/**
 * Delete material threshold
 * @param {string} id - Material threshold id
 * @returns {Promise<object>}
 */
export function deleteMaterialThreshold(id) {
  return request.delete(API_ENDPOINTS.MATERIAL_THRESHOLD.DETAIL(id))
}

/**
 * Toggle material threshold enabled status
 * @param {string} id - Material threshold id
 * @returns {Promise<object>}
 */
export function toggleMaterialThreshold(id) {
  return request.patch(API_ENDPOINTS.MATERIAL_THRESHOLD.TOGGLE(id))
}

export default {
  getMaterialThresholds,
  getMaterialThresholdById,
  createMaterialThreshold,
  updateMaterialThreshold,
  deleteMaterialThreshold,
  toggleMaterialThreshold
}
