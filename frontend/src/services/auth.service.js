/**
 * AuthService - Authentication business logic
 * Handles login, logout, token management
 */

import { mockAuthBackend } from '../mock/auth-backend.js'
import { storageService } from './storage.service.js'
import { parseSimulatedJWT, isTokenExpired, getTokenExpiresIn } from '../mock/crypto.js'

const TOKEN_KEY = 'auth_token'
const USER_KEY = 'auth_user'

export class AuthService {
  /**
   * User login
   * @param {string} username - Username
   * @param {string} password - Password (plain text)
   * @returns {Promise<object>} Login response
   */
  async login(username, password) {
    try {
      const response = await mockAuthBackend.login(username, password)

      // Store token and user in localStorage
      storageService.set(TOKEN_KEY, response.token)
      storageService.set(USER_KEY, response.user)

      return response
    } catch (error) {
      throw error
    }
  }

  /**
   * User logout
   * @returns {Promise<void>}
   */
  async logout() {
    try {
      const user = this.getCurrentUser()
      if (user) {
        await mockAuthBackend.logout(user.id)
      }

      // Clear stored data
      storageService.remove(TOKEN_KEY)
      storageService.remove(USER_KEY)
    } catch (error) {
      // Still clear local data even if backend call fails
      storageService.remove(TOKEN_KEY)
      storageService.remove(USER_KEY)
      throw error
    }
  }

  /**
   * Get current token
   * @returns {string|null} Token or null
   */
  getToken() {
    return storageService.get(TOKEN_KEY)
  }

  /**
   * Get current user
   * @returns {object|null} User object or null
   */
  getCurrentUser() {
    return storageService.get(USER_KEY)
  }

  /**
   * Check if user is authenticated
   * @returns {boolean}
   */
  isAuthenticated() {
    const token = this.getToken()
    const user = this.getCurrentUser()

    if (!token || !user) {
      return false
    }

    // Check if token is expired
    const payload = parseSimulatedJWT(token)
    if (!payload || isTokenExpired(payload)) {
      // Clear expired token
      this.clearSession()
      return false
    }

    return true
  }

  /**
   * Validate token
   * @param {string} token - Token to validate
   * @returns {Promise<object>} User object if valid
   */
  async validateToken(token) {
    try {
      const user = await mockAuthBackend.validateToken(token)
      storageService.set(USER_KEY, user)
      return user
    } catch (error) {
      this.clearSession()
      throw error
    }
  }

  /**
   * Refresh token
   * @returns {Promise<string>} New token
   */
  async refreshToken() {
    try {
      const currentToken = this.getToken()
      if (!currentToken) {
        throw new Error('No token to refresh')
      }

      const newToken = await mockAuthBackend.refreshToken(currentToken)
      storageService.set(TOKEN_KEY, newToken)

      return newToken
    } catch (error) {
      this.clearSession()
      throw error
    }
  }

  /**
   * Check if user has specific role
   * @param {string} role - Role to check
   * @returns {boolean}
   */
  hasRole(role) {
    const user = this.getCurrentUser()
    if (!user || !user.roles) {
      return false
    }
    return user.roles.includes(role)
  }

  /**
   * Check if user has any of the specified roles
   * @param {Array<string>} roles - Roles to check
   * @returns {boolean}
   */
  hasAnyRole(roles) {
    const user = this.getCurrentUser()
    if (!user || !user.roles) {
      return false
    }
    return roles.some(role => user.roles.includes(role))
  }

  /**
   * Get token expiry time in seconds
   * @returns {number} Seconds until expiry (0 if not authenticated)
   */
  getTokenExpiresIn() {
    const token = this.getToken()
    if (!token) return 0

    const payload = parseSimulatedJWT(token)
    if (!payload) return 0

    return getTokenExpiresIn(payload)
  }

  /**
   * Initialize auth session from stored token
   * @returns {Promise<boolean>} True if session restored
   */
  async initializeSession() {
    const token = this.getToken()
    if (!token) {
      return false
    }

    try {
      const user = await this.validateToken(token)
      storageService.set(USER_KEY, user)
      return true
    } catch (error) {
      this.clearSession()
      return false
    }
  }

  /**
   * Clear session data
   */
  clearSession() {
    storageService.remove(TOKEN_KEY)
    storageService.remove(USER_KEY)
  }

  /**
   * Get login error message from error code
   * @param {object} error - Error object
   * @returns {string} User-friendly error message
   */
  getErrorMessage(error) {
    const errorMessages = {
      'INVALID_CREDENTIALS': '账号或密码错误',
      'ACCOUNT_LOCKED': '账号已被锁定，请30分钟后再试',
      'ACCOUNT_DISABLED': '账号已被禁用，请联系管理员',
      'TOKEN_EXPIRED': '登录已过期，请重新登录',
      'NETWORK_ERROR': '网络错误，请稍后重试',
      'SERVER_ERROR': '服务器错误，请稍后重试'
    }

    return error.message || errorMessages[error.code] || '登录失败，请重试'
  }
}

// Export singleton instance
export const authService = new AuthService()
export default authService
