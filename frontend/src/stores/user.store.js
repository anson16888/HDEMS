/**
 * UserStore - Pinia store for user management state
 */

import { defineStore } from 'pinia'
import { userService } from '../services/user.service.js'
import { ref, computed } from 'vue'

export const useUserStore = defineStore('users', () => {
  // State
  const users = ref([])
  const loading = ref(false)
  const error = ref(null)
  const filters = ref({
    role: '',
    status: '',
    keyword: '',
    hospitalId: undefined
  })
  const pagination = ref({
    page: 1,
    pageSize: 20,
    total: 0,
    totalPages: 0
  })

  // Getters
  // Note: Filtering is now done on backend, so we just return users directly
  const filteredUsers = computed(() => {
    return users.value
  })

  const paginatedUsers = computed(() => {
    // Pagination is now handled by backend
    return users.value
  })

  // Actions
  async function fetchUsers() {
    try {
      loading.value = true
      error.value = null

      // Pass filters and pagination to backend
      const params = {
        ...filters.value,
        page: pagination.value.page,
        pageSize: pagination.value.pageSize
      }

      const result = await userService.getUsers(params)
      users.value = result.items || []
      pagination.value.total = result.total || 0
      pagination.value.totalPages = result.totalPages || 0
    } catch (err) {
      error.value = err.message || '获取用户列表失败'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function fetchUserById(id) {
    try {
      loading.value = true
      error.value = null

      return await userService.getUserById(id)
    } catch (err) {
      error.value = err.message || '获取用户信息失败'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function createUser(userData) {
    try {
      loading.value = true
      error.value = null

      const newUser = await userService.createUser(userData)
      // Refresh list to get updated data
      await fetchUsers()

      return newUser
    } catch (err) {
      error.value = err.message || '创建用户失败'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function updateUser(id, updates) {
    try {
      loading.value = true
      error.value = null

      const updatedUser = await userService.updateUser(id, updates)

      // Update in list
      const index = users.value.findIndex(u => u.id === id)
      if (index !== -1) {
        users.value[index] = updatedUser
      }

      return updatedUser
    } catch (err) {
      error.value = err.message || '更新用户失败'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function deleteUser(id) {
    try {
      loading.value = true
      error.value = null

      await userService.deleteUser(id)

      // Refresh list to get updated data
      await fetchUsers()
    } catch (err) {
      error.value = err.message || '删除用户失败'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function resetPassword(id, newPassword) {
    try {
      loading.value = true
      error.value = null

      const result = await userService.resetPassword(id, newPassword)
      return result
    } catch (err) {
      error.value = err.message || '重置密码失败'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function unlockAccount(id, user) {
    try {
      loading.value = true
      error.value = null

      const updatedUser = await userService.unlockAccount(id, user)

      // Update in list
      const index = users.value.findIndex(u => u.id === id)
      if (index !== -1) {
        users.value[index] = updatedUser
      }
    } catch (err) {
      error.value = err.message || '解锁账号失败'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function getLoginLogs(userId) {
    try {
      loading.value = true
      error.value = null

      return await userService.getLoginLogs(userId)
    } catch (err) {
      error.value = err.message || '获取登录日志失败'
      throw err
    } finally {
      loading.value = false
    }
  }

  function setFilters(newFilters) {
    filters.value = { ...filters.value, ...newFilters }
    pagination.value.page = 1 // Reset to first page
  }

  function clearFilters() {
    filters.value = {
      role: '',
      status: '',
      keyword: '',
      hospitalId: undefined
    }
    pagination.value.page = 1
  }

  function setPagination(newPagination) {
    pagination.value = { ...pagination.value, ...newPagination }
  }

  function getRoleDisplayName(roleCode) {
    return userService.getRoleDisplayName(roleCode)
  }

  function getStatusDisplayName(statusCode) {
    return userService.getStatusDisplayName(statusCode)
  }

  return {
    // State
    users,
    loading,
    error,
    filters,
    pagination,

    // Getters
    filteredUsers,
    paginatedUsers,

    // Actions
    fetchUsers,
    fetchUserById,
    createUser,
    updateUser,
    deleteUser,
    resetPassword,
    unlockAccount,
    getLoginLogs,
    setFilters,
    clearFilters,
    setPagination,
    getRoleDisplayName,
    getStatusDisplayName
  }
})
