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
    keyword: ''
  })
  const pagination = ref({
    page: 1,
    pageSize: 20,
    total: 0
  })

  // Getters
  const filteredUsers = computed(() => {
    let result = users.value

    if (filters.value.keyword) {
      const keyword = filters.value.keyword.toLowerCase()
      result = result.filter(u =>
        u.realName.toLowerCase().includes(keyword) ||
        u.username.toLowerCase().includes(keyword)
      )
    }

    if (filters.value.role) {
      result = result.filter(u => u.roles.includes(filters.value.role))
    }

    if (filters.value.status) {
      result = result.filter(u => u.status === filters.value.status)
    }

    return result
  })

  const paginatedUsers = computed(() => {
    const start = (pagination.value.page - 1) * pagination.value.pageSize
    const end = start + pagination.value.pageSize
    return filteredUsers.value.slice(start, end)
  })

  // Actions
  async function fetchUsers() {
    try {
      loading.value = true
      error.value = null

      // Ensure user store is initialized
      await userService.initialize()

      const data = await userService.getUsers(filters.value)
      users.value = data
      pagination.value.total = data.length
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
      users.value.push(newUser)
      pagination.value.total++

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

      // Remove from list
      users.value = users.value.filter(u => u.id !== id)
      pagination.value.total--
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

  async function unlockAccount(id) {
    try {
      loading.value = true
      error.value = null

      await userService.unlockAccount(id)

      // Update in list
      const index = users.value.findIndex(u => u.id === id)
      if (index !== -1) {
        users.value[index].login_attempts = 0
        users.value[index].locked_until = null
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
      keyword: ''
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
