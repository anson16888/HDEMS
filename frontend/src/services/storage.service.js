/**
 * StorageService - localStorage wrapper with error handling
 *
 * Provides a safe interface to localStorage with:
 * - JSON serialization/deserialization
 * - Error handling for quota exceeded
 * - Prefix isolation
 */

const STORAGE_PREFIX = 'hdems_'

export class StorageService {
  /**
   * Get item from localStorage
   * @param {string} key - Storage key (without prefix)
   * @returns {any|null} Parsed value or null
   */
  get(key) {
    try {
      const fullKey = `${STORAGE_PREFIX}${key}`
      const value = localStorage.getItem(fullKey)

      if (value === null) {
        return null
      }

      // Try to parse as JSON, fallback to string
      try {
        return JSON.parse(value)
      } catch {
        // If not JSON, return as string
        return value
      }
    } catch (error) {
      console.error('[StorageService] Error getting item:', error)
      return null
    }
  }

  /**
   * Set item in localStorage
   * @param {string} key - Storage key (without prefix)
   * @param {any} value - Value to store
   * @returns {boolean} Success status
   */
  set(key, value) {
    try {
      const fullKey = `${STORAGE_PREFIX}${key}`

      // For simple types (string, number, boolean), store directly
      // For complex types (object, array), use JSON.stringify
      let serialized
      if (typeof value === 'string' || typeof value === 'number' || typeof value === 'boolean') {
        serialized = String(value)
      } else {
        serialized = JSON.stringify(value)
      }

      localStorage.setItem(fullKey, serialized)
      return true
    } catch (error) {
      if (error.name === 'QuotaExceededError') {
        console.error('[StorageService] localStorage quota exceeded')
        alert('存储空间不足，请清理浏览器数据')
      } else {
        console.error('[StorageService] Error setting item:', error)
      }
      return false
    }
  }

  /**
   * Remove item from localStorage
   * @param {string} key - Storage key (without prefix)
   * @returns {boolean} Success status
   */
  remove(key) {
    try {
      const fullKey = `${STORAGE_PREFIX}${key}`
      localStorage.removeItem(fullKey)
      return true
    } catch (error) {
      console.error('[StorageService] Error removing item:', error)
      return false
    }
  }

  /**
   * Clear all HDEMS items from localStorage
   * @returns {boolean} Success status
   */
  clear() {
    try {
      const keysToRemove = []
      for (let i = 0; i < localStorage.length; i++) {
        const key = localStorage.key(i)
        if (key.startsWith(STORAGE_PREFIX)) {
          keysToRemove.push(key)
        }
      }
      keysToRemove.forEach(key => localStorage.removeItem(key))
      return true
    } catch (error) {
      console.error('[StorageService] Error clearing items:', error)
      return false
    }
  }

  /**
   * Check if key exists in localStorage
   * @param {string} key - Storage key (without prefix)
   * @returns {boolean}
   */
  exists(key) {
    try {
      const fullKey = `${STORAGE_PREFIX}${key}`
      return localStorage.getItem(fullKey) !== null
    } catch (error) {
      console.error('[StorageService] Error checking existence:', error)
      return false
    }
  }
}

// Export singleton instance
export const storageService = new StorageService()
export default storageService
