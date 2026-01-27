import { describe, it, expect, beforeEach, vi } from 'vitest'
import { AuthService } from '../auth.service'
import { storageService } from '../../services/storage.service'
import { login as loginApi, logout as logoutApi, getCurrentUser as getCurrentUserApi } from '../../api/auth.api'

// Mock dependencies
vi.mock('../../services/storage.service', () => ({
  storageService: {
    get: vi.fn(),
    set: vi.fn(),
    remove: vi.fn()
  }
}))

vi.mock('../../api/auth.api', () => ({
  login: vi.fn(),
  logout: vi.fn(),
  refreshToken: vi.fn(),
  getCurrentUser: vi.fn()
}))

describe('Auth Service', () => {
  let authService

  // Create a helper to generate a mock JWT
  const createMockToken = (payload) => {
    const header = btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' }))
    const body = btoa(JSON.stringify(payload)).replace(/=/g, '')
    return `${header}.${body}.signature`
  }

  beforeEach(() => {
    authService = new AuthService()
    vi.clearAllMocks()
  })

  describe('JWT Parsing & Expiry', () => {
    it('should correctly calculate expires in time', () => {
      const now = Math.floor(Date.now() / 1000)
      const expiry = now + 3600 // 1 hour later
      const token = createMockToken({ exp: expiry })

      storageService.get.mockReturnValue(token)

      const expiresIn = authService.getTokenExpiresIn()
      // Allow 1-2 seconds difference due to test execution time
      expect(expiresIn).toBeGreaterThan(3590)
      expect(expiresIn).toBeLessThanOrEqual(3600)
    })

    it('should return 0 for expired tokens', () => {
      const past = Math.floor(Date.now() / 1000) - 3600
      const token = createMockToken({ exp: past })

      storageService.get.mockReturnValue(token)
      expect(authService.getTokenExpiresIn()).toBe(0)
    })

    it('should return 0 if no token exists', () => {
      storageService.get.mockReturnValue(null)
      expect(authService.getTokenExpiresIn()).toBe(0)
    })
  })

  describe('Authentication Status', () => {
    it('should be authenticated if valid token and user exist', () => {
      const expiry = Math.floor(Date.now() / 1000) + 3600
      storageService.get.mockImplementation((key) => {
        if (key === 'auth_token') return createMockToken({ exp: expiry })
        if (key === 'auth_user') return { name: 'Admin' }
        return null
      })

      expect(authService.isAuthenticated()).toBe(true)
    })

    it('should not be authenticated if token is expired', () => {
      const past = Math.floor(Date.now() / 1000) - 100
      storageService.get.mockImplementation((key) => {
        if (key === 'auth_token') return createMockToken({ exp: past })
        if (key === 'auth_user') return { name: 'Admin' }
        return null
      })

      expect(authService.isAuthenticated()).toBe(false)
      expect(storageService.remove).toHaveBeenCalled() // Should clear session
    })
  })

  describe('Login & Logout', () => {
    it('should store credentials on successful login', async () => {
      const mockResult = {
        data: {
          token: 'new-token',
          userInfo: { id: 1, name: 'Admin' }
        }
      }
      loginApi.mockResolvedValue(mockResult)

      const result = await authService.login('admin', 'password')

      expect(storageService.set).toHaveBeenCalledWith('auth_token', 'new-token')
      expect(storageService.set).toHaveBeenCalledWith('auth_user', mockResult.data.userInfo)
      expect(result.token).toBe('new-token')
    })

    it('should clear storage on logout', async () => {
      logoutApi.mockResolvedValue()

      await authService.logout()

      expect(storageService.remove).toHaveBeenCalledWith('auth_token')
      expect(storageService.remove).toHaveBeenCalledWith('auth_user')
    })
  })

  describe('Permissions', () => {
    const mockUser = { roles: ['ADMIN', 'EDITOR'] }

    it('should correctly check for specified role', () => {
      storageService.get.mockReturnValue(mockUser)

      expect(authService.hasRole('ADMIN')).toBe(true)
      expect(authService.hasRole('VIEWER')).toBe(false)
    })

    it('should correctly check for multiple roles', () => {
      storageService.get.mockReturnValue(mockUser)

      expect(authService.hasAnyRole(['EDITOR', 'VIEWER'])).toBe(true)
      expect(authService.hasAnyRole(['VIEWER', 'GUEST'])).toBe(false)
    })
  })

  describe('Error Messages', () => {
    it('should return correct user-friendly messages for error codes', () => {
      expect(authService.getErrorMessage({ code: 401 })).toBe('账号或密码错误')
      expect(authService.getErrorMessage({ code: 403 })).toBe('账号已被禁用，请联系管理员')
      expect(authService.getErrorMessage({ message: '自定义错误' })).toBe('自定义错误')
    })
  })
})
