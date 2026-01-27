import { describe, it, expect, beforeEach, vi } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useUserStore } from '../user.store'
import { userService } from '../../services/user.service'

// Mock User Service
vi.mock('../../services/user.service', () => ({
  userService: {
    getUsers: vi.fn(),
    getUserById: vi.fn(),
    createUser: vi.fn(),
    updateUser: vi.fn(),
    deleteUser: vi.fn(),
    resetPassword: vi.fn(),
    unlockAccount: vi.fn(),
    getLoginLogs: vi.fn(),
    getRoleDisplayName: vi.fn(),
    getStatusDisplayName: vi.fn()
  }
}))

describe('User Store', () => {
  let store

  beforeEach(() => {
    setActivePinia(createPinia())
    store = useUserStore()
    vi.clearAllMocks()

    // Default mock response for fetchUsers side effects
    userService.getUsers.mockResolvedValue({ items: [], total: 0, totalPages: 0 })
  })

  describe('User Operations (Actions)', () => {
    it('should create a user and refresh list', async () => {
      const userData = { username: 'test' }
      userService.createUser.mockResolvedValue({ id: 'new-id' })

      await store.createUser(userData)

      expect(userService.createUser).toHaveBeenCalledWith(userData)
      // fetchUsers is called inside createUser
      expect(userService.getUsers).toHaveBeenCalled()
      expect(store.loading).toBe(false)
    })

    it('should update a user and update local state', async () => {
      store.users = [{ id: '1', realName: 'Old' }]
      const updates = { realName: 'New' }
      userService.updateUser.mockResolvedValue({ id: '1', realName: 'New' })

      await store.updateUser('1', updates)

      expect(userService.updateUser).toHaveBeenCalledWith('1', updates)
      expect(store.users[0].realName).toBe('New')
    })

    it('should handle delete user and refresh list', async () => {
      userService.deleteUser.mockResolvedValue()

      await store.deleteUser('1')

      expect(userService.deleteUser).toHaveBeenCalledWith('1')
      expect(userService.getUsers).toHaveBeenCalled()
    })

    it('should handle password reset', async () => {
      userService.resetPassword.mockResolvedValue({ success: true })
      const result = await store.resetPassword('1', 'new-pass')
      expect(result.success).toBe(true)
    })

    it('should handle account unlocking', async () => {
      store.users = [{ id: '1', status: 3 }]
      userService.unlockAccount.mockResolvedValue({ id: '1', status: 1 })

      await store.unlockAccount('1', store.users[0])

      expect(userService.unlockAccount).toHaveBeenCalled()
      expect(store.users[0].status).toBe(1)
    })
  })

  describe('Filter & Pagination Management', () => {
    it('should update filters and reset page', () => {
      store.pagination.page = 3
      store.setFilters({ keyword: 'search' })

      expect(store.filters.keyword).toBe('search')
      expect(store.pagination.page).toBe(1)
    })

    it('should clear filters', () => {
      store.filters.keyword = 'abc'
      store.clearFilters()
      expect(store.filters.keyword).toBe('')
    })

    it('should update pagination', () => {
      store.setPagination({ page: 2, pageSize: 50 })
      expect(store.pagination.page).toBe(2)
      expect(store.pagination.pageSize).toBe(50)
    })
  })

  describe('Display Names', () => {
    it('should delegate display name calls to service', () => {
      store.getRoleDisplayName(1)
      expect(userService.getRoleDisplayName).toHaveBeenCalledWith(1)

      store.getStatusDisplayName(1)
      expect(userService.getStatusDisplayName).toHaveBeenCalledWith(1)
    })
  })

  describe('Error Handling', () => {
    it('should capture error message on failure', async () => {
      userService.createUser.mockRejectedValue(new Error('Creation failed'))

      try {
        await store.createUser({})
      } catch (e) {
        // Expected
      }

      expect(store.error).toBe('Creation failed')
      expect(store.loading).toBe(false)
    })
  })
})
