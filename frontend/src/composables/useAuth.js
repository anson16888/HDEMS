/**
 * useAuth - Composable for authentication state management
 * Convenience wrapper around AuthStore
 */

import { computed } from 'vue'
import { useAuthStore } from '../stores/auth.store.js'

export function useAuth() {
  const authStore = useAuthStore()

  // Reactive state
  const isAuthenticated = computed(() => authStore.isAuthenticated)
  const user = computed(() => authStore.user)
  const token = computed(() => authStore.token)
  const userDisplayName = computed(() => authStore.userDisplayName)
  const lastError = computed(() => authStore.lastLoginError)

  // Role checks
  const isSystemAdmin = computed(() => authStore.isSystemAdmin)
  const isDutyAdmin = computed(() => authStore.isDutyAdmin)
  const isMaterialAdmin = computed(() => authStore.isMaterialAdmin)
  const hasAnyRole = computed(() => authStore.hasAnyRole)

  // Organization type checks
  const isHospitalSystem = computed(() => authStore.isHospitalSystem)
  const isBureauSystem = computed(() => authStore.isBureauSystem)

  // Token info
  const tokenExpiresIn = computed(() => authStore.tokenExpiresIn)
  const isTokenExpiringSoon = computed(() => {
    const expiresIn = authStore.tokenExpiresIn
    return expiresIn > 0 && expiresIn < 3600 // Less than 1 hour
  })

  // Actions
  const login = async (username, password) => {
    return await authStore.login({ username, password })
  }

  const logout = async () => {
    return await authStore.logout()
  }

  const checkAuth = async () => {
    return await authStore.checkAuth()
  }

  const refreshToken = async () => {
    return await authStore.refreshToken()
  }

  const clearError = () => {
    authStore.clearError()
  }

  const hasRole = (role) => {
    return authStore.hasRole(role)
  }

  const hasAnyRoleOf = (roles) => {
    return authStore.hasAnyRoleOf(roles)
  }

  const canAccess = (requiredRoles = []) => {
    if (!authStore.isAuthenticated) return false
    if (requiredRoles.length === 0) return true
    return authStore.hasAnyRoleOf(requiredRoles)
  }

  return {
    // State
    isAuthenticated,
    user,
    token,
    userDisplayName,
    lastError,

    // Roles
    isSystemAdmin,
    isDutyAdmin,
    isMaterialAdmin,
    hasAnyRole,

    // Organization
    isHospitalSystem,
    isBureauSystem,

    // Token
    tokenExpiresIn,
    isTokenExpiringSoon,

    // Actions
    login,
    logout,
    checkAuth,
    refreshToken,
    clearError,

    // Helpers
    hasRole,
    hasAnyRoleOf,
    canAccess
  }
}
