/**
 * User API
 * User management API calls
 */

import { request } from '../utils/http.js'
import { API_ENDPOINTS } from '../config/api.config.js'

/**
 * Get user list with pagination and filters
 * @param {object} params - Query parameters { page, pageSize, keyword }
 * @returns {Promise<object>} Paginated user list
 */
export function getUsers(params = {}) {
  return request.get(API_ENDPOINTS.USER.LIST, params)
}

/**
 * Get user by ID
 * @param {string} id - User ID
 * @returns {Promise<object>} User detail
 */
export function getUserById(id) {
  return request.get(API_ENDPOINTS.USER.DETAIL(id))
}

/**
 * Create new user
 * @param {object} data - User data { username, realName, phone, department, roles[], hospitalId, isCommissionUser }
 * @returns {Promise<object>} Created user
 */
export function createUser(data) {
  return request.post(API_ENDPOINTS.USER.CREATE, data)
}

/**
 * Update user
 * @param {string} id - User ID
 * @param {object} data - Update data { realName, phone, department, roles[], status, hospitalId, isCommissionUser }
 * @returns {Promise<object>} Updated user
 */
export function updateUser(id, data) {
  return request.put(API_ENDPOINTS.USER.UPDATE(id), data)
}

/**
 * Delete user
 * @param {string} id - User ID
 * @returns {Promise<object>}
 */
export function deleteUser(id) {
  return request.delete(API_ENDPOINTS.USER.DELETE(id))
}

/**
 * Reset user password
 * @param {string} id - User ID
 * @param {string} newPassword - New password
 * @returns {Promise<object>}
 */
export function resetPassword(id, newPassword) {
  return request.post(API_ENDPOINTS.USER.RESET_PASSWORD(id), newPassword)
}

export default {
  getUsers,
  getUserById,
  createUser,
  updateUser,
  deleteUser,
  resetPassword
}
