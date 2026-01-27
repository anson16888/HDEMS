import { describe, it, expect, beforeEach, vi } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useAuthStore } from '../auth.store'
import { authService } from '../../services/auth.service'
import { getHospitalConfig } from '../../api/hospitalConfig.api'

// Mock dependencies
vi.mock('../../services/auth.service', () => ({
  authService: {
    login: vi.fn(),
    logout: vi.fn(),
    initializeSession: vi.fn(),
    getToken: vi.fn(),
    getCurrentUser: vi.fn(),
    refreshToken: vi.fn(),
    getTokenExpiresIn: vi.fn(),
    getErrorMessage: vi.fn(err => err.message || 'Error'),
    hasRole: vi.fn(),
    hasAnyRole: vi.fn()
  }
}))

vi.mock('../../api/hospitalConfig.api', () => ({
  getHospitalConfig: vi.fn()
}))

describe('Auth Store', () => {
  let store

  beforeEach(() => {
    setActivePinia(createPinia())
    store = useAuthStore()
    vi.clearAllMocks()
  })

  describe('Initial State', () => {
    it('should have default values', () => {
      expect(store.token).toBeNull()
      expect(store.user).toBeNull()
      expect(store.hospitalConfig).toBeNull()
      expect(store.isAuthenticated).toBe(false)
      expect(store.isInitialized).toBe(false)
      expect(store.loginAttempts).toBe(0)
    })
  })

  describe('Getters - Roles', () => {
    it('should correctly identify roles', () => {
      store.user = { roles: ['SYSTEM_ADMIN', 'SCHEDULE_ADMIN'] }

      expect(store.isSystemAdmin).toBe(true)
      expect(store.isDutyAdmin).toBe(true)
      expect(store.isMaterialAdmin).toBe(false)
    })
  })

  describe('Getters - System Type', () => {
    it('should identify Bureau system when hospital name contains "卫健"', () => {
      store.hospitalConfig = { hospitalName: '宝安区卫健委' }
      expect(store.isBureauSystem).toBe(true)
      expect(store.isHospitalSystem).toBe(false)
    })

    it('should identify Hospital system when hospital name does not contain "卫健"', () => {
      store.hospitalConfig = { hospitalName: '宝安区人民医院' }
      expect(store.isBureauSystem).toBe(false)
      expect(store.isHospitalSystem).toBe(true)
    })
  })

  describe('Actions - login', () => {
    const credentials = { username: 'admin', password: 'password' }
    const mockUser = { id: 1, realName: '管理员', roles: ['SYSTEM_ADMIN'] }
    const mockResponse = { token: 'mock-token', user: mockUser }

    it('should handle successful login', async () => {
      authService.login.mockResolvedValue(mockResponse)
      getHospitalConfig.mockResolvedValue({ code: 200, data: { hospitalName: 'Test' } })

      await store.login(credentials)

      expect(store.token).toBe('mock-token')
      expect(store.user).toEqual(mockUser)
      expect(store.isAuthenticated).toBe(true)
      expect(getHospitalConfig).toHaveBeenCalled()
    })
  })

  describe('Actions - logout', () => {
    it('should clear authentication state', async () => {
      store.token = 'token'
      store.user = { name: 'User' }
      store.isAuthenticated = true

      authService.logout.mockResolvedValue()

      await store.logout()

      expect(store.token).toBeNull()
      expect(store.user).toBeNull()
      expect(store.isAuthenticated).toBe(false)
      expect(store.hospitalConfig).toBeNull()
    })
  })

  describe('Actions - checkAuth', () => {
    it('should restore session if valid', async () => {
      const mockUser = { id: 1, name: 'User' }
      authService.initializeSession.mockResolvedValue(true)
      authService.getToken.mockReturnValue('stored-token')
      authService.getCurrentUser.mockReturnValue(mockUser)

      const result = await store.checkAuth()

      expect(result).toBe(true)
      expect(store.token).toBe('stored-token')
      expect(store.isAuthenticated).toBe(true)
    })
  })

  describe('Actions - initialize', () => {
    it('should run initialization flow and set isInitialized', async () => {
      // Setup: ensure checkAuth (via initializeSession) is called
      authService.initializeSession.mockResolvedValue(true)
      authService.getToken.mockReturnValue('t')
      authService.getCurrentUser.mockReturnValue({ roles: [] })
      getHospitalConfig.mockResolvedValue({ code: 200, data: { hospitalName: 'H' } })

      await store.initialize()

      expect(authService.initializeSession).toHaveBeenCalled()
      expect(store.isInitialized).toBe(true)
      expect(getHospitalConfig).toHaveBeenCalled()
    })

    it('should only run initialize logic once', async () => {
      authService.initializeSession.mockResolvedValue(false)

      await store.initialize()
      await store.initialize()

      // initializeSession is called inside checkAuth
      expect(authService.initializeSession).toHaveBeenCalledTimes(1)
    })
  })
})
