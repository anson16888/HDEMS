/**
 * UserService - User management business logic
 * Handles CRUD operations for users using backend API
 */

import * as userApi from '../api/user.api.js'
import { USER_ROLES, USER_STATUS } from '../config/api.config.js'

export class UserService {
  /**
   * Initialize user service (no-op for API version)
   * @returns {Promise<void>}
   */
  async initialize() {
    // No initialization needed when using API
  }

  /**
   * Get all users with pagination and filters
   * @param {object} filters - Optional filters { role, status, keyword, page, pageSize }
   * @returns {Promise<object>} Paginated user list { items: [], total, page, pageSize, totalPages }
   */
  async getUsers(filters = {}) {
    // Convert role from string to number if needed
    const params = { ...filters }

    if (filters.role && typeof filters.role === 'string') {
      // Convert role name to role number if needed
      const roleMap = {
        'SYSTEM_ADMIN': USER_ROLES.SYSTEM_ADMIN,
        'SCHEDULE_ADMIN': USER_ROLES.SCHEDULE_ADMIN,
        'MATERIAL_ADMIN': USER_ROLES.MATERIAL_ADMIN
      }
      params.roles = [roleMap[filters.role]]
      delete params.role
    }

    if (filters.status && typeof filters.status === 'string') {
      // Convert status name to status number if needed
      const statusMap = {
        'ACTIVE': USER_STATUS.ACTIVE,
        'INACTIVE': USER_STATUS.INACTIVE,
        'LOCKED': USER_STATUS.LOCKED
      }
      params.status = statusMap[filters.status]
    }

    const response = await userApi.getUsers(params)
    return response.data
  }

  /**
   * Get user by ID
   * @param {string} id - User ID
   * @returns {Promise<object>} User object
   */
  async getUserById(id) {
    const response = await userApi.getUserById(id)
    return response.data
  }

  /**
   * Create new user
   * @param {object} userData - User data
   * @returns {Promise<object>} Created user
   */
  async createUser(userData) {
    // Convert roles and status if needed (in case they come as strings)
    const data = { ...userData }

    if (data.roles && Array.isArray(data.roles)) {
      // Convert role names to role numbers if they are strings
      data.roles = data.roles.map(role => {
        if (typeof role === 'string') {
          const roleMap = {
            'SYSTEM_ADMIN': USER_ROLES.SYSTEM_ADMIN,
            'SCHEDULE_ADMIN': USER_ROLES.SCHEDULE_ADMIN,
            'MATERIAL_ADMIN': USER_ROLES.MATERIAL_ADMIN
          }
          return roleMap[role] || role
        }
        return role
      })
    }

    // Validate required fields after conversion
    this.validateUserData(data)

    const response = await userApi.createUser(data)
    return response.data
  }

  /**
   * Update user
   * @param {string} id - User ID
   * @param {object} updates - Fields to update
   * @returns {Promise<object>} Updated user
   */
  async updateUser(id, updates) {
    // Convert status if needed
    const data = { ...updates }

    if (updates.status && typeof updates.status === 'string') {
      const statusMap = {
        'ACTIVE': USER_STATUS.ACTIVE,
        'INACTIVE': USER_STATUS.INACTIVE,
        'LOCKED': USER_STATUS.LOCKED
      }
      data.status = statusMap[updates.status]
    }

    // Convert roles if needed
    if (updates.roles && Array.isArray(updates.roles)) {
      data.roles = updates.roles.map(role => {
        if (typeof role === 'string') {
          const roleMap = {
            'SYSTEM_ADMIN': USER_ROLES.SYSTEM_ADMIN,
            'SCHEDULE_ADMIN': USER_ROLES.SCHEDULE_ADMIN,
            'MATERIAL_ADMIN': USER_ROLES.MATERIAL_ADMIN
          }
          return roleMap[role] || role
        }
        return role
      })
    }

    const response = await userApi.updateUser(id, data)
    return response.data
  }

  /**
   * Delete user
   * @param {string} id - User ID
   * @returns {Promise<void>}
   */
  async deleteUser(id) {
    await userApi.deleteUser(id)
  }

  /**
   * Reset user password
   * @param {string} id - User ID
   * @param {string} newPassword - New password to set
   * @returns {Promise<object>} Response object
   */
  async resetPassword(id, newPassword) {
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

    const response = await userApi.resetPassword(id, newPassword)
    return response
  }

  /**
   * Unlock user account (update status to ACTIVE)
   * @param {string} id - User ID
   * @param {object} user - Full user object to send
   * @returns {Promise<object>} Updated user
   */
  async unlockAccount(id, user) {
    // Send complete user data with status set to ACTIVE
    const updateData = {
      ...user,
      status: USER_STATUS.ACTIVE
    }

    const response = await userApi.updateUser(id, updateData)
    return response.data
  }

  /**
   * Get login logs for user - Not implemented in backend yet
   * @param {string} userId - User ID
   * @returns {Promise<Array>} Empty array (not implemented)
   */
  async getLoginLogs(userId) {
    // This feature is not implemented in the backend yet
    return []
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

    // Password (only for create)
    if (data.password !== undefined) {
      if (!data.password) {
        throw { code: 'VALIDATION_ERROR', message: '密码不能为空' }
      }
      if (data.password.length < 8) {
        throw { code: 'VALIDATION_ERROR', message: '密码长度至少8位' }
      }
      if (!/[a-zA-Z]/.test(data.password) || !/[0-9]/.test(data.password)) {
        throw { code: 'VALIDATION_ERROR', message: '密码必须包含字母和数字' }
      }
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

    // // Roles
    // if (data.roles && Array.isArray(data.roles)) {
    //   if (data.roles.length === 0) {
    //     throw { code: 'VALIDATION_ERROR', message: '请至少选择一个角色' }
    //   }

    //   const validRoles = [USER_ROLES.SYSTEM_ADMIN, USER_ROLES.SCHEDULE_ADMIN, USER_ROLES.MATERIAL_ADMIN]
    //   const invalidRoles = data.roles.filter(r => !validRoles.includes(r))
    //   console.log(invalidRoles)
    //   if (invalidRoles.length > 0) {
    //     throw { code: 'VALIDATION_ERROR', message: '包含无效的角色' }
    //   }
    // }

    // Status - only validate if explicitly provided
    if (data.status !== undefined && !Object.values(USER_STATUS).includes(data.status)) {
      throw { code: 'VALIDATION_ERROR', message: '无效的状态' }
    }
  }

  /**
   * Get role display name
   * @param {number} roleCode - Role code (number)
   * @returns {string} Display name
   */
  getRoleDisplayName(roleCode) {
    const roleNames = {
      [USER_ROLES.SYSTEM_ADMIN]: '系统管理员',
      [USER_ROLES.SCHEDULE_ADMIN]: '值班管理员',
      [USER_ROLES.MATERIAL_ADMIN]: '物资管理员'
    }
    return roleNames[roleCode] || roleCode
  }

  /**
   * Get status display name
   * @param {number} statusCode - Status code (number)
   * @returns {string} Display name
   */
  getStatusDisplayName(statusCode) {
    const statusNames = {
      [USER_STATUS.ACTIVE]: '正常',
      [USER_STATUS.INACTIVE]: '停用',
      [USER_STATUS.LOCKED]: '锁定'
    }
    return statusNames[statusCode] || statusCode
  }
}

// Export singleton instance
export const userService = new UserService()
export default userService
