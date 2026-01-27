import { describe, it, expect, beforeEach, vi } from 'vitest'
import { StorageService } from '../storage.service'

describe('Storage Service', () => {
  let storageService
  const PREFIX = 'hdems_'

  beforeEach(() => {
    storageService = new StorageService()
    localStorage.clear()
    vi.clearAllMocks()
  })

  describe('set & get', () => {
    it('should store and retrieve strings', () => {
      storageService.set('name', 'test-user')
      expect(localStorage.getItem(PREFIX + 'name')).toBe('test-user')
      expect(storageService.get('name')).toBe('test-user')
    })

    it('should store and retrieve numbers', () => {
      storageService.set('count', 123)
      expect(storageService.get('count')).toBe(123)
    })

    it('should store and retrieve boolean values', () => {
      storageService.set('flag', true)
      expect(storageService.get('flag')).toBe(true)
    })

    it('should store and retrieve objects with JSON serialization', () => {
      const user = { id: 1, name: 'admin' }
      storageService.set('user', user)

      const stored = localStorage.getItem(PREFIX + 'user')
      expect(typeof stored).toBe('string')
      expect(JSON.parse(stored)).toEqual(user)

      expect(storageService.get('user')).toEqual(user)
    })
  })

  describe('remove & exists', () => {
    it('should remove items correctly', () => {
      storageService.set('key1', 'val1')
      expect(storageService.exists('key1')).toBe(true)

      storageService.remove('key1')
      expect(storageService.exists('key1')).toBe(false)
      expect(localStorage.getItem(PREFIX + 'key1')).toBeNull()
    })
  })

  describe('clear', () => {
    it('should clear only items with the specific prefix', () => {
      storageService.set('app_key', 'val1')
      localStorage.setItem('other_app_key', 'should_remain')

      storageService.clear()

      expect(storageService.exists('app_key')).toBe(false)
      expect(localStorage.getItem('other_app_key')).toBe('should_remain')
    })
  })
})
