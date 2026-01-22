/**
 * AuthStore - Pinia store for authentication state management
 */

import { defineStore } from 'pinia'
import { authService } from '../services/auth.service.js'
import { ref, computed } from 'vue'

export const useAuthStore = defineStore('auth', () => {
  // State
  const token = ref(null)
  const user = ref(null)
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

  // Initialize store
  async function initialize() {
    if (!isInitialized.value) {
      await checkAuth()
      isInitialized.value = true
    }
  }

  return {
    // State
    token,
    user,
    isAuthenticated,
    loginAttempts,
    lastLoginError,
    isInitialized,

    // Getters
    isSystemAdmin,
    isDutyAdmin,
    isMaterialAdmin,
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
