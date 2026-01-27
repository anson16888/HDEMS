/**
 * AuthStore - Pinia store for authentication state management
 */

import { defineStore } from 'pinia'
import { authService } from '../services/auth.service.js'
import { getHospitals, getHospitalById } from '../api/basicData.api.js'
import { ref, computed } from 'vue'

export const useAuthStore = defineStore('auth', () => {
  // State
  const token = ref(null)
  const user = ref(null)
  const hospitalConfig = ref(null) // 新增：医院配置信息
  const isAuthenticated = ref(false)
  const loginAttempts = ref(0)
  const lastLoginError = ref(null)
  const isInitialized = ref(false)

  // Getters
  const isSystemAdmin = computed(() => {
    return user.value?.roles?.includes('SYSTEM_ADMIN') || false
  })

  const isDutyAdmin = computed(() => {
    return user.value?.roles?.includes('SCHEDULE_ADMIN') || false
  })

  const isMaterialAdmin = computed(() => {
    return user.value?.roles?.includes('MATERIAL_ADMIN') || false
  })

  // 组织类型判断：通过医院名称判断（包含"卫健"则为卫健委系统）
  const isHospitalSystem = computed(() => {
    const hospitalName = hospitalConfig.value?.hospitalName || ''
    // 如果医院名称包含"卫健"，则是卫健委系统
    // 否则是医院系统
    return !hospitalName.includes('卫健')
  })

  const isBureauSystem = computed(() => {
    const hospitalName = hospitalConfig.value?.hospitalName || ''
    // 如果医院名称包含"卫健"，则是卫健委系统
    return hospitalName.includes('卫健')
  })

  const hasAnyRole = computed(() => {
    return isAuthenticated.value && user.value?.roles?.length > 0
  })

  const tokenExpiresIn = computed(() => {
    return authService.getTokenExpiresIn()
  })

  const userDisplayName = computed(() => {
    return user.value?.realName || '未登录'
  })

  // Actions
  async function login(credentials) {
    try {
      lastLoginError.value = null
      const response = await authService.login(credentials.username, credentials.password)

      // Update state
      token.value = response.token
      user.value = response.user
      isAuthenticated.value = true
      loginAttempts.value = 0

      // 登录成功后获取医院配置
      await fetchHospitalConfig()

      return response
    } catch (error) {
      loginAttempts.value++
      lastLoginError.value = authService.getErrorMessage(error)
      throw error
    }
  }

  async function logout() {
    try {
      await authService.logout()
    } catch (error) {
      console.error('Logout error:', error)
    } finally {
      // Clear state regardless of API call result
      token.value = null
      user.value = null
      hospitalConfig.value = null
      isAuthenticated.value = false
      loginAttempts.value = 0
      lastLoginError.value = null
    }
  }

  async function checkAuth() {
    try {
      const hasSession = await authService.initializeSession()
      if (hasSession) {
        token.value = authService.getToken()
        user.value = authService.getCurrentUser()
        isAuthenticated.value = true
        return true
      } else {
        clearAuth()
        return false
      }
    } catch (error) {
      clearAuth()
      return false
    }
  }

  async function refreshToken() {
    try {
      const newToken = await authService.refreshToken()
      token.value = newToken
      return newToken
    } catch (error) {
      clearAuth()
      throw error
    }
  }

  function clearError() {
    lastLoginError.value = null
  }

  function clearAuth() {
    token.value = null
    user.value = null
    hospitalConfig.value = null
    isAuthenticated.value = false
    loginAttempts.value = 0
    lastLoginError.value = null
  }

  function hasRole(role) {
    return authService.hasRole(role)
  }

  function hasAnyRoleOf(roles) {
    return authService.hasAnyRole(roles)
  }

  // 获取医院配置
  async function fetchHospitalConfig() {
    try {
      let hospitalData
      if (user.value?.hospitalId) {
        // 通过用户的 hospitalId 获取医院信息
        const response = await getHospitalById(user.value.hospitalId)
        hospitalData = response.data
      } else if (isSystemAdmin.value) {
        // 管理员且没有 hospitalId，获取第一个医院作为默认
        const response = await getHospitals()
        hospitalData = response.data?.[0] || null
      }
      // 非管理员没有 hospitalId 时，不设置 hospitalConfig（保持为 null）
      if (hospitalData) {
        hospitalConfig.value = hospitalData
        console.log('医院配置已加载:', hospitalConfig.value?.hospitalName)
      }
    } catch (error) {
      console.error('获取医院配置失败:', error)
    }
  }

  // Initialize store
  async function initialize() {
    if (!isInitialized.value) {
      await checkAuth()
      // 如果已登录，获取医院配置
      if (isAuthenticated.value) {
        await fetchHospitalConfig()
      }
      isInitialized.value = true
    }
  }

  return {
    // State
    token,
    user,
    isAuthenticated,
    hospitalConfig,
    loginAttempts,
    lastLoginError,
    isInitialized,

    // Getters
    isSystemAdmin,
    isDutyAdmin,
    isMaterialAdmin,
    isHospitalSystem,
    isBureauSystem,
    hasAnyRole,
    tokenExpiresIn,
    userDisplayName,

    // Actions
    login,
    logout,
    checkAuth,
    refreshToken,
    clearError,
    clearAuth,
    hasRole,
    hasAnyRoleOf,
    initialize
  }
})
