/**
 * UserService - User management business logic
 * Handles CRUD operations for users
 */

import { userStore } from '../mock/user-store.js'
import { sha256 } from '../mock/crypto.js'

export class UserService {
  /**
   * Initialize user store
   * @returns {Promise<void>}
   */
  async initialize() {
    await userStore.initialize()
  }

  /**
   * Get all users
   * @param {object} filters - Optional filters { role, status, keyword }
   * @returns {Promise<Array>} List of users
   */
  async getUsers(filters = {}) {
    await new Promise(resolve => setTimeout(resolve, 200)) // Simulate delay

    let users = userStore.getUsers()

    // Apply filters
    if (filters.role) {
      users = users.filter(u => u.roles.includes(filters.role))
    }

    if (filters.status) {
      users = users.filter(u => u.status === filters.status)
    }

    if (filters.keyword) {
      const keyword = filters.keyword.toLowerCase()
      users = users.filter(u =>
        u.realName.toLowerCase().includes(keyword) ||
        u.username.toLowerCase().includes(keyword)
      )
    }

    return users
  }

  /**
   * Get user by ID
   * @param {string} id - User ID
   * @returns {Promise<object>} User object
   */
  async getUserById(id) {
    await new Promise(resolve => setTimeout(resolve, 100))

    const user = userStore.getUserById(id)
    if (!user) {
      throw { code: 'USER_NOT_FOUND', message: '用户不存在' }
    }

    return user
  }

  /**
   * Create new user
   * @param {object} userData - User data
   * @returns {Promise<object>} Created user
   */
  async createUser(userData) {
    await new Promise(resolve => setTimeout(resolve, 300))

    // Validate required fields
    this.validateUserData(userData)

    // Hash password
    const hashedPassword = await sha256(userData.password)

    // Create user
    const newUser = userStore.createUser({
      ...userData,
      password: hashedPassword
    })

    // Remove password from response
    const { password, ...userResponse } = newUser
    return userResponse
  }

  /**
   * Update user
   * @param {string} id - User ID
   * @param {object} updates - Fields to update
   * @returns {Promise<object>} Updated user
   */
  async updateUser(id, updates) {
    await new Promise(resolve => setTimeout(resolve, 300))

    // If updating password, hash it
    if (updates.password) {
      updates.password = await sha256(updates.password)
    }

    // Validate updates
    if (updates.username) {
      throw { code: 'VALIDATION_ERROR', message: '不能修改用户名' }
    }

    const updatedUser = userStore.updateUser(id, updates)

    // Remove password from response
    const { password, ...userResponse } = updatedUser
    return userResponse
  }

  /**
   * Delete user
   * @param {string} id - User ID
   * @returns {Promise<void>}
   */
  async deleteUser(id) {
    await new Promise(resolve => setTimeout(resolve, 200))

    const user = userStore.getUserById(id)
    if (!user) {
      throw { code: 'USER_NOT_FOUND', message: '用户不存在' }
    }

    userStore.deleteUser(id)
  }

  /**
   * Reset user password
   * @param {string} id - User ID
   * @param {string} newPassword - New password to set
   * @returns {Promise<object>} Object containing password
   */
  async resetPassword(id, newPassword) {
    await new Promise(resolve => setTimeout(resolve, 300))

    const user = userStore.getUserById(id)
    if (!user) {
      throw { code: 'USER_NOT_FOUND', message: '用户不存在' }
    }

    // Validate password
    if (!newPassword) {
      throw { code: 'VALIDATION_ERROR', message: '新密码不能为空' }
    }
    if (newPassword.length < 8) {
      throw { code: 'VALIDATION_ERROR', message: '密码长度至少8位' }
    }
    if (!/[a-zA-Z]/.test(newPassword) || !/[0-9]/.test(newPassword)) {
      throw { code: 'VALIDATION_ERROR', message: '密码必须包含字母和数字' }
    }

    // Hash password
    const hashedPassword = await sha256(newPassword)

    userStore.resetPassword(id, hashedPassword)

    return { password: newPassword }
  }

  /**
   * Unlock user account
   * @param {string} id - User ID
   * @returns {Promise<void>}
   */
  async unlockAccount(id) {
    await new Promise(resolve => setTimeout(resolve, 200))

    const user = userStore.getUserById(id)
    if (!user) {
      throw { code: 'USER_NOT_FOUND', message: '用户不存在' }
    }

    userStore.unlockAccount(id)
  }

  /**
   * Get login logs for user
   * @param {string} userId - User ID
   * @returns {Promise<Array>} Login logs
   */
  async getLoginLogs(userId) {
    await new Promise(resolve => setTimeout(resolve, 200))

    return userStore.getLoginLogs(userId)
  }

  /**
   * Validate user data
   * @param {object} data - User data to validate
   * @throws {Error} Validation error
   */
  validateUserData(data) {
    // Username
    if (!data.username) {
      throw { code: 'VALIDATION_ERROR', message: '账号不能为空' }
    }
    if (data.username.length > 20) {
      throw { code: 'VALIDATION_ERROR', message: '账号长度最多20个字符' }
    }
    if (!/^[a-zA-Z0-9_]+$/.test(data.username)) {
      throw { code: 'VALIDATION_ERROR', message: '账号只能包含字母、数字和下划线' }
    }

    // Password
    if (!data.password) {
      throw { code: 'VALIDATION_ERROR', message: '密码不能为空' }
    }
    if (data.password.length < 8) {
      throw { code: 'VALIDATION_ERROR', message: '密码长度至少8位' }
    }
    if (!/[a-zA-Z]/.test(data.password) || !/[0-9]/.test(data.password)) {
      throw { code: 'VALIDATION_ERROR', message: '密码必须包含字母和数字' }
    }

    // Real name
    if (!data.realName) {
      throw { code: 'VALIDATION_ERROR', message: '姓名不能为空' }
    }
    if (data.realName.length < 2 || data.realName.length > 20) {
      throw { code: 'VALIDATION_ERROR', message: '姓名长度必须为2-20个字符' }
    }

    // Phone
    if (!data.phone) {
      throw { code: 'VALIDATION_ERROR', message: '手机号不能为空' }
    }
    if (!/^1\d{10}$/.test(data.phone)) {
      throw { code: 'VALIDATION_ERROR', message: '请输入正确的11位手机号' }
    }

    // Roles
    if (!data.roles || data.roles.length === 0) {
      throw { code: 'VALIDATION_ERROR', message: '请至少选择一个角色' }
    }

    const validRoles = ['SYSTEM_ADMIN', 'DUTY_ADMIN', 'MATERIAL_ADMIN']
    const invalidRoles = data.roles.filter(r => !validRoles.includes(r))
    if (invalidRoles.length > 0) {
      throw { code: 'VALIDATION_ERROR', message: '包含无效的角色' }
    }

    // Status
    if (data.status && !['ACTIVE', 'INACTIVE'].includes(data.status)) {
      throw { code: 'VALIDATION_ERROR', message: '无效的状态' }
    }
  }

  /**
   * Get role display name
   * @param {string} roleCode - Role code
   * @returns {string} Display name
   */
  getRoleDisplayName(roleCode) {
    const roleNames = {
      'SYSTEM_ADMIN': '系统管理员',
      'DUTY_ADMIN': '值班管理员',
      'MATERIAL_ADMIN': '物资管理员'
    }
    return roleNames[roleCode] || roleCode
  }

  /**
   * Get status display name
   * @param {string} statusCode - Status code
   * @returns {string} Display name
   */
  getStatusDisplayName(statusCode) {
    const statusNames = {
      'ACTIVE': '启用',
      'INACTIVE': '禁用'
    }
    return statusNames[statusCode] || statusCode
  }
}

// Export singleton instance
export const userService = new UserService()
export default userService
