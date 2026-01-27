import { describe, it, expect, beforeEach, vi } from 'vitest'
import { UserService } from '../user.service'
import * as userApi from '../../api/user.api'
import { USER_ROLES, USER_STATUS } from '../../config/api.config'

// Mock User API
vi.mock('../../api/user.api', () => ({
  getUsers: vi.fn(),
  getUserById: vi.fn(),
  createUser: vi.fn(),
  updateUser: vi.fn(),
  deleteUser: vi.fn(),
  resetPassword: vi.fn()
}))

describe('User Service', () => {
  let userService

  beforeEach(() => {
    userService = new UserService()
    vi.clearAllMocks()
  })

  describe('Validation', () => {
    it('should validate username (required, format, length)', () => {
      expect(() => userService.validateUserData({})).toThrow('账号不能为空')
      expect(() => userService.validateUserData({ username: 'a'.repeat(21) })).toThrow('账号长度最多20个字符')
      expect(() => userService.validateUserData({ username: 'invalid!' })).toThrow('账号只能包含字母、数字和下划线')
    })

    it('should validate password for creation', () => {
      const baseData = { username: 'user1', realName: '姓名', phone: '13812345678' }
      expect(() => userService.validateUserData({ ...baseData, password: '' })).toThrow('密码不能为空')
      expect(() => userService.validateUserData({ ...baseData, password: '123' })).toThrow('密码长度至少8位')
      expect(() => userService.validateUserData({ ...baseData, password: 'password' })).toThrow('密码必须包含字母和数字')
    })

    it('should validate realName and phone', () => {
      const baseData = { username: 'user1' }
      expect(() => userService.validateUserData({ ...baseData })).toThrow('姓名不能为空')
      expect(() => userService.validateUserData({ ...baseData, realName: 'a' })).toThrow('姓名长度必须为2-20个字符')
      expect(() => userService.validateUserData({ ...baseData, realName: '姓名' })).toThrow('手机号不能为空')
      expect(() => userService.validateUserData({ ...baseData, realName: '姓名', phone: '123' })).toThrow('请输入正确的11位手机号')
    })
  })

  describe('CRUD Operations', () => {
    it('should map role and status strings to codes in getUsers', async () => {
      userApi.getUsers.mockResolvedValue({ data: { items: [], total: 0 } })

      await userService.getUsers({ role: 'SCHEDULE_ADMIN', status: 'LOCKED' })

      expect(userApi.getUsers).toHaveBeenCalledWith({
        roles: [USER_ROLES.SCHEDULE_ADMIN],
        status: USER_STATUS.LOCKED
      })
    })

    it('should handle user creation with role name conversion', async () => {
      const userData = {
        username: 'new_user',
        password: 'Password123',
        realName: '张三',
        phone: '13811112222',
        roles: ['MATERIAL_ADMIN']
      }
      userApi.createUser.mockResolvedValue({ data: { id: '123' } })

      const result = await userService.createUser(userData)

      expect(userApi.createUser).toHaveBeenCalledWith(expect.objectContaining({
        roles: [USER_ROLES.MATERIAL_ADMIN]
      }))
      expect(result.id).toBe('123')
    })

    it('should handle user updates', async () => {
      userApi.updateUser.mockResolvedValue({ data: { id: '1', status: 2 } })

      await userService.updateUser('1', { status: 'INACTIVE', roles: ['SYSTEM_ADMIN'] })

      expect(userApi.updateUser).toHaveBeenCalledWith('1', {
        status: USER_STATUS.INACTIVE,
        roles: [USER_ROLES.SYSTEM_ADMIN]
      })
    })

    it('should handle user deletion', async () => {
      userApi.deleteUser.mockResolvedValue({})
      await userService.deleteUser('1')
      expect(userApi.deleteUser).toHaveBeenCalledWith('1')
    })

    it('should unlock account by setting status to ACTIVE', async () => {
      const mockUser = { id: '1', username: 'locked_user', status: 3 }
      userApi.updateUser.mockResolvedValue({ data: { ...mockUser, status: 1 } })

      await userService.unlockAccount('1', mockUser)

      expect(userApi.updateUser).toHaveBeenCalledWith('1', expect.objectContaining({
        status: USER_STATUS.ACTIVE
      }))
    })
  })

  describe('Display Names', () => {
    it('should return correct role display names', () => {
      expect(userService.getRoleDisplayName(USER_ROLES.SYSTEM_ADMIN)).toBe('系统管理员')
      expect(userService.getRoleDisplayName(999)).toBe(999)
    })

    it('should return correct status display names', () => {
      expect(userService.getStatusDisplayName(USER_STATUS.ACTIVE)).toBe('正常')
      expect(userService.getStatusDisplayName(USER_STATUS.LOCKED)).toBe('锁定')
    })
  })
})
