/**
 * MockAuthBackend - Simulates API calls with delays and errors
 * Provides realistic mock behavior for authentication operations
 */

import { userStore } from './user-store.js'
import { sha256, generateSimulatedJWT, parseSimulatedJWT, isTokenExpired } from './crypto.js'
import { getMockConfig } from './seed-data.js'

const mockConfig = getMockConfig()

/**
 * Simulate network delay
 * @param {number} min - Minimum delay in ms
 * @param {number} max - Maximum delay in ms
 * @returns {Promise<void>}
 */
function simulateDelay(min = mockConfig.networkDelay.min, max = mockConfig.networkDelay.max) {
  const delay = Math.floor(Math.random() * (max - min + 1)) + min
  return new Promise(resolve => setTimeout(resolve, delay))
}

/**
 * Simulate random errors
 * @throws {Error} Random error (based on errorRate config)
 */
function simulateError() {
  if (Math.random() < mockConfig.errorRate) {
    const errors = [
      { code: 'NETWORK_ERROR', message: '网络错误，请稍后重试' },
      { code: 'SERVER_ERROR', message: '服务器错误，请稍后重试' }
    ]
    const error = errors[Math.floor(Math.random() * errors.length)]
    throw error
  }
}

export class MockAuthBackend {
  /**
   * User login
   * @param {string} username - Username
   * @param {string} password - Plain text password
   * @returns {Promise<object>} Login response with token and user
   */
  async login(username, password) {
    await simulateDelay()
    simulateError()

    // Ensure store is initialized
    await userStore.initialize()

    // Check if user exists
    const user = userStore.getUserByUsername(username)
    if (!user) {
      userStore.logLoginAttempt({
        username,
        success: false,
        failure_reason: 'User not found'
      })
      throw { code: 'INVALID_CREDENTIALS', message: '账号或密码错误' }
    }

    // Check if account is locked
    if (user.locked_until) {
      const lockedUntil = new Date(user.locked_until)
      if (lockedUntil > new Date()) {
        userStore.logLoginAttempt({
          username,
          success: false,
          failure_reason: 'Account locked'
        })
        throw { code: 'ACCOUNT_LOCKED', message: '账号已被锁定，请30分钟后再试' }
      } else {
        // Lock expired, clear it
        userStore.unlockAccount(user.id)
      }
    }

    // Check if account is disabled
    if (user.status === 'INACTIVE') {
      userStore.logLoginAttempt({
        username,
        success: false,
        failure_reason: 'Account disabled'
      })
      throw { code: 'ACCOUNT_DISABLED', message: '账号已被禁用，请联系管理员' }
    }

    // Verify password
    const passwordHash = await sha256(password)
    if (user.password !== passwordHash) {
      userStore.incrementLoginAttempts(username)
      userStore.logLoginAttempt({
        username,
        success: false,
        failure_reason: 'Invalid password'
      })
      throw { code: 'INVALID_CREDENTIALS', message: '账号或密码错误' }
    }

    // Successful login - reset attempts
    const updatedUser = userStore.resetLoginAttempts(username)
    userStore.logLoginAttempt({
      username,
      success: true
    })

    // Generate token
    const token = generateSimulatedJWT({
      user_id: user.id,
      username: user.username,
      roles: user.roles
    })

    // Store token
    userStore.storeToken({
      token,
      user_id: user.id,
      expires_at: new Date(Date.now() + mockConfig.tokenExpiry).toISOString()
    })

    return {
      token,
      user: updatedUser
    }
  }

  /**
   * Validate token
   * @param {string} token - JWT token
   * @returns {Promise<object>} User object if valid
   */
  async validateToken(token) {
    await simulateDelay(50, 200) // Shorter delay for validation

    const payload = parseSimulatedJWT(token)
    if (!payload || isTokenExpired(payload)) {
      throw { code: 'TOKEN_EXPIRED', message: '登录已过期，请重新登录' }
    }

    const user = userStore.getUserById(payload.user_id)
    if (!user) {
      throw { code: 'USER_NOT_FOUND', message: '用户不存在' }
    }

    if (user.status === 'INACTIVE') {
      throw { code: 'ACCOUNT_DISABLED', message: '账号已被禁用' }
    }

    return user
  }

  /**
   * Refresh token
   * @param {string} currentToken - Current token
   * @returns {Promise<string>} New token
   */
  async refreshToken(currentToken) {
    await simulateDelay(100, 300)

    const payload = parseSimulatedJWT(currentToken)
    if (!payload) {
      throw { code: 'TOKEN_INVALID', message: '无效的令牌' }
    }

    const user = userStore.getUserById(payload.user_id)
    if (!user) {
      throw { code: 'USER_NOT_FOUND', message: '用户不存在' }
    }

    // Generate new token
    const newToken = generateSimulatedJWT({
      user_id: user.id,
      username: user.username,
      roles: user.roles
    })

    // Store new token
    userStore.storeToken({
      token: newToken,
      user_id: user.id,
      expires_at: new Date(Date.now() + mockConfig.tokenExpiry).toISOString()
    })

    return newToken
  }

  /**
   * Logout user
   * @param {string} userId - User ID
   * @returns {Promise<void>}
   */
  async logout(userId) {
    await simulateDelay(100, 300)
    userStore.removeTokens(userId)
  }
}

// Export singleton instance
export const mockAuthBackend = new MockAuthBackend()
export default mockAuthBackend
