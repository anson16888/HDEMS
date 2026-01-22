/**
 * Hospital Config API
 * Manages current hospital basic info
 */

import { request } from '../utils/http.js'
import { API_ENDPOINTS } from '../config/api.config.js'

/**
 * Get hospital config
 * @returns {Promise<object>} Hospital config
 */
export function getHospitalConfig() {
  return request.get(API_ENDPOINTS.HOSPITAL_CONFIG.DETAIL)
}

/**
 * Update hospital config
 * @param {object} data - Update data { hospitalName, hospitalPhone }
 * @returns {Promise<object>} Updated hospital config
 */
export function updateHospitalConfig(data) {
  return request.put(API_ENDPOINTS.HOSPITAL_CONFIG.UPDATE, data)
}
