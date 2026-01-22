/**
 * Auth API
 * Authentication-related API calls
 */

import { request } from '../utils/http.js'
import { API_ENDPOINTS } from '../config/api.config.js'

/**
 * Login
 * @param {string} username - Username
 * @param {string} password - Password
 * @returns {Promise<object>} Login response with token and user info
 */
export function login(username, password) {
  return request.post(API_ENDPOINTS.AUTH.LOGIN, {
    username,
    password
  })
}

/**
 * Logout
 * @returns {Promise<object>}
 */
export function logout() {
  return request.post(API_ENDPOINTS.AUTH.LOGOUT)
}

/**
 * Refresh token
 * @param {string} token - Current token
 * @returns {Promise<string>} New token
 */
export function refreshToken(token) {
  return request.post(API_ENDPOINTS.AUTH.REFRESH, token)
}

/**
 * Get current user info
 * @returns {Promise<object>} Current user info
 */
export function getCurrentUser() {
  return request.get(API_ENDPOINTS.AUTH.CURRENT_USER)
}

/**
 * Change password
 * @param {string} oldPassword - Old password
 * @param {string} newPassword - New password
 * @returns {Promise<object>}
 */
export function changePassword(oldPassword, newPassword) {
  return request.post(API_ENDPOINTS.AUTH.CHANGE_PASSWORD, {
    oldPassword,
    newPassword
  })
}

/**
 * Reset admin password
 * @param {string} newPassword - New password
 * @returns {Promise<object>}
 */
export function resetAdminPassword(newPassword) {
  return request.post(API_ENDPOINTS.AUTH.RESET_ADMIN_PASSWORD, newPassword)
}

export default {
  login,
  logout,
  refreshToken,
  getCurrentUser,
  changePassword,
  resetAdminPassword
}
