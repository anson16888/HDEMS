/**
 * AuthService - Authentication business logic
 * Handles login, logout, token management
 */

import { login as loginApi, logout as logoutApi, refreshToken as refreshTokenApi, getCurrentUser as getCurrentUserApi } from '../api/auth.api.js'
import { storageService } from './storage.service.js'

const TOKEN_KEY = 'auth_token'
const USER_KEY = 'auth_user'

/**
 * Parse JWT token (simple base64 decode)
 * @param {string} token - JWT token
 * @returns {object|null} Parsed payload
 */
function parseJWT(token) {
  try {
    const base64Url = token.split('.')[1]
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/')
    const jsonPayload = decodeURIComponent(atob(base64).split('').map((c) => {
      return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)
    }).join(''))
    return JSON.parse(jsonPayload)
  } catch (error) {
    return null
  }
}

/**
 * Check if token is expired
 * @param {object} payload - Parsed JWT payload
 * @returns {boolean}
 */
function isTokenExpired(payload) {
  if (!payload.exp) return false
  const now = Math.floor(Date.now() / 1000)
  return payload.exp < now
}

/**
 * Get token expiry time in seconds
 * @param {object} payload - Parsed JWT payload
 * @returns {number} Seconds until expiry
 */
function getTokenExpiresIn(payload) {
  if (!payload.exp) return 0
  const now = Math.floor(Date.now() / 1000)
  return Math.max(0, payload.exp - now)
}

export class AuthService {
  /**
   * User login
   * @param {string} username - Username
   * @param {string} password - Password (plain text)
   * @returns {Promise<object>} Login response
   */
  async login(username, password) {
    try {
      const response = await loginApi(username, password)

      // Store token and user info in localStorage
      // Backend returns { code, message, data: { token, userInfo }, timestamp }
      storageService.set(TOKEN_KEY, response.data.token)
      storageService.set(USER_KEY, response.data.userInfo)

      return {
        token: response.data.token,
        user: response.data.userInfo
      }
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
      await logoutApi()

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
    const payload = parseJWT(token)
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
      const response = await getCurrentUserApi()
      const user = response.data
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

      const response = await refreshTokenApi(currentToken)
      const newToken = response.data
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

    const payload = parseJWT(token)
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
    // Backend returns different error formats
    // HTTP interceptor already shows error message via antd message
    // But for login form, we need to return the message
    if (error.message) {
      return error.message
    }

    // Map backend error codes to user-friendly messages
    const errorMessages = {
      400: '账号或密码错误',
      401: '账号或密码错误',
      403: '账号已被禁用，请联系管理员',
      404: '用户不存在',
      429: '请求过于频繁，请稍后重试',
      500: '服务器错误，请稍后重试'
    }

    return errorMessages[error.code] || error.message || '登录失败，请重试'
  }
}

// Export singleton instance
export const authService = new AuthService()
export default authService
