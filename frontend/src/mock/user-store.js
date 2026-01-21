/**
 * UserStore - localStorage-based user data management
 * Handles CRUD operations for user entities
 */

import { storageService } from '../services/storage.service.js'
import { generateSeedUsers, generateInitKey, displayInitKey, getMockConfig } from './seed-data.js'

const USERS_KEY = 'users'
const TOKENS_KEY = 'auth_tokens'
const INIT_KEY = 'init_key'
const LOGIN_LOGS_KEY = 'login_logs'

const mockConfig = getMockConfig()

export class UserStore {
  constructor() {
    this.initialized = false
  }

  /**
   * Initialize store with seed data if empty
   */
  async initialize() {
    if (this.initialized) return

    const users = storageService.get(USERS_KEY)

    if (!users || users.length === 0) {
      // Generate and store seed users
      const seedUsers = await generateSeedUsers()
      storageService.set(USERS_KEY, seedUsers)

      // Generate and store initialization key
      const initKey = generateInitKey()
      storageService.set(INIT_KEY, initKey)

      // Display key in console
      displayInitKey(initKey)
    }

    this.initialized = true
  }

  /**
   * Get all users
   * @returns {Array} All users
   */
  getUsers() {
    return storageService.get(USERS_KEY) || []
  }

  /**
   * Get user by ID
   * @param {string} id - User ID
   * @returns {object|null} User object or null
   */
  getUserById(id) {
    const users = this.getUsers()
    return users.find(u => u.id === id) || null
  }

  /**
   * Get user by username
   * @param {string} username - Username
   * @returns {object|null} User object or null
   */
  getUserByUsername(username) {
    const users = this.getUsers()
    return users.find(u => u.username === username) || null
  }

  /**
   * Create new user
   * @param {object} userData - User data
   * @returns {object} Created user
   */
  createUser(userData) {
    const users = this.getUsers()

    // Check username uniqueness
    if (users.find(u => u.username === userData.username)) {
      throw new Error('USERNAME_EXISTS')
    }

    const now = new Date().toISOString()
    const newUser = {
      id: `user-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`,
      ...userData,
      login_attempts: 0,
      locked_until: null,
      last_login_at: null,
      created_at: now,
      updated_at: now
    }

    users.push(newUser)
    storageService.set(USERS_KEY, users)

    return newUser
  }

  /**
   * Update user
   * @param {string} id - User ID
   * @param {object} updates - Fields to update
   * @returns {object} Updated user
   */
  updateUser(id, updates) {
    const users = this.getUsers()
    const index = users.findIndex(u => u.id === id)

    if (index === -1) {
      throw new Error('USER_NOT_FOUND')
    }

    const updatedUser = {
      ...users[index],
      ...updates,
      id: users[index].id, // Preserve ID
      username: users[index].username, // Preserve username
      updated_at: new Date().toISOString()
    }

    users[index] = updatedUser
    storageService.set(USERS_KEY, users)

    return updatedUser
  }

  /**
   * Delete user (soft delete)
   * @param {string} id - User ID
   */
  deleteUser(id) {
    const users = this.getUsers()
    const filtered = users.filter(u => u.id !== id)

    if (filtered.length === users.length) {
      throw new Error('USER_NOT_FOUND')
    }

    storageService.set(USERS_KEY, filtered)
  }

  /**
   * Increment login attempts
   * @param {string} username - Username
   * @returns {object} Updated user
   */
  incrementLoginAttempts(username) {
    const user = this.getUserByUsername(username)
    if (!user) return null

    const attempts = (user.login_attempts || 0) + 1

    if (attempts >= mockConfig.maxLoginAttempts) {
      // Lock account
      const lockedUntil = new Date(Date.now() + mockConfig.lockoutDuration).toISOString()
      return this.updateUser(user.id, {
        login_attempts: attempts,
        locked_until: lockedUntil
      })
    }

    return this.updateUser(user.id, {
      login_attempts: attempts
    })
  }

  /**
   * Reset login attempts (successful login)
   * @param {string} username - Username
   * @returns {object} Updated user
   */
  resetLoginAttempts(username) {
    const user = this.getUserByUsername(username)
    if (!user) return null

    return this.updateUser(user.id, {
      login_attempts: 0,
      locked_until: null,
      last_login_at: new Date().toISOString()
    })
  }

  /**
   * Unlock user account
   * @param {string} id - User ID
   * @returns {object} Updated user
   */
  unlockAccount(id) {
    return this.updateUser(id, {
      login_attempts: 0,
      locked_until: null
    })
  }

  /**
   * Reset user password
   * @param {string} id - User ID
   * @param {string} newPassword - New password (already hashed)
   * @returns {object} Updated user
   */
  resetPassword(id, newPassword) {
    return this.updateUser(id, {
      password: newPassword
    })
  }

  /**
   * Get initialization key
   * @returns {string} Initialization key
   */
  getInitKey() {
    return storageService.get(INIT_KEY)
  }

  /**
   * Validate initialization key
   * @param {string} key - Key to validate
   * @returns {boolean}
   */
  validateInitKey(key) {
    const storedKey = this.getInitKey()
    return storedKey === key
  }

  /**
   * Store auth token
   * @param {object} tokenData - Token data
   */
  storeToken(tokenData) {
    const tokens = storageService.get(TOKENS_KEY) || []
    tokens.push(tokenData)
    storageService.set(TOKENS_KEY, tokens)
  }

  /**
   * Get active tokens for user
   * @param {string} userId - User ID
   * @returns {Array} Active tokens
   */
  getTokens(userId) {
    const tokens = storageService.get(TOKENS_KEY) || []
    return tokens.filter(t => t.user_id === userId)
  }

  /**
   * Remove all tokens for user (logout)
   * @param {string} userId - User ID
   */
  removeTokens(userId) {
    const tokens = storageService.get(TOKENS_KEY) || []
    const filtered = tokens.filter(t => t.user_id !== userId)
    storageService.set(TOKENS_KEY, filtered)
  }

  /**
   * Log login attempt
   * @param {object} logData - Log data
   */
  logLoginAttempt(logData) {
    const logs = storageService.get(LOGIN_LOGS_KEY) || []
    logs.unshift({
      id: `log-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`,
      ...logData,
      timestamp: new Date().toISOString()
    })

    // Keep only last 100 logs
    if (logs.length > 100) {
      logs.splice(100)
    }

    storageService.set(LOGIN_LOGS_KEY, logs)
  }

  /**
   * Get login logs for user
   * @param {string} userId - User ID
   * @returns {Array} Login logs
   */
  getLoginLogs(userId) {
    const logs = storageService.get(LOGIN_LOGS_KEY) || []
    return logs.filter(l => l.username === userId)
  }
}

// Export singleton instance
export const userStore = new UserStore()
export default userStore
