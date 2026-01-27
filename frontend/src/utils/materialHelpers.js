import { describe, it, expect } from 'vitest'
import dayjs from 'dayjs'

/**
 * 计算物资过期日期
 * @param {string} productionDate - 生产日期 (YYYY-MM-DD)
 * @param {number} shelfLife - 保质期(月)
 * @returns {string|null} 过期日期 (YYYY-MM-DD) 或 null
 */
export function calculateExpiryDate(productionDate, shelfLife) {
  if (!productionDate || !shelfLife) {
    return null
  }

  try {
    const production = dayjs(productionDate)
    if (!production.isValid()) {
      return null
    }

    return production.add(shelfLife, 'month').format('YYYY-MM-DD')
  } catch (error) {
    return null
  }
}

/**
 * 判断物资是否即将过期 (30天内)
 * @param {string} expiryDate - 过期日期 (YYYY-MM-DD)
 * @returns {boolean}
 */
export function isExpiringSoon(expiryDate) {
  if (!expiryDate) {
    return false
  }

  try {
    const today = dayjs()
    const expiry = dayjs(expiryDate)

    if (!expiry.isValid()) {
      return false
    }

    const diffDays = expiry.diff(today, 'day')
    return diffDays <= 30 && diffDays >= 0
  } catch (error) {
    return false
  }
}

/**
 * 判断物资是否已过期
 * @param {string} expiryDate - 过期日期 (YYYY-MM-DD)
 * @returns {boolean}
 */
export function isExpired(expiryDate) {
  if (!expiryDate) {
    return false
  }

  try {
    const today = dayjs()
    const expiry = dayjs(expiryDate)

    if (!expiry.isValid()) {
      return false
    }

    return today.isAfter(expiry, 'day')
  } catch (error) {
    return false
  }
}

/**
 * 计算物资库存状态
 * @param {number} quantity - 库存数量
 * @param {number} threshold - 预警阈值
 * @param {string} expiryDate - 过期日期
 * @returns {string} 状态: 'NORMAL' | 'LOW' | 'OUT' | 'EXPIRED' | 'EXPIRING_SOON'
 */
export function calculateMaterialStatus(quantity, threshold, expiryDate) {
  // 优先检查是否已过期
  if (isExpired(expiryDate)) {
    return 'EXPIRED'
  }

  // 检查库存数量
  if (quantity === 0) {
    return 'OUT'
  }

  if (quantity <= threshold) {
    return 'LOW'
  }

  // 检查是否即将过期
  if (isExpiringSoon(expiryDate)) {
    return 'EXPIRING_SOON'
  }

  return 'NORMAL'
}

/**
 * 格式化日期
 * @param {string} date - 日期字符串
 * @param {string} format - 格式 (默认: 'YYYY-MM-DD')
 * @returns {string}
 */
export function formatDate(date, format = 'YYYY-MM-DD') {
  if (!date) {
    return '-'
  }

  try {
    const d = dayjs(date)
    return d.isValid() ? d.format(format) : '-'
  } catch (error) {
    return '-'
  }
}

describe('Material Helper Functions', () => {
  describe('calculateExpiryDate()', () => {
    it('should calculate expiry date correctly', () => {
      const result = calculateExpiryDate('2024-01-01', 12)
      expect(result).toBe('2025-01-01')
    })

    it('should handle different shelf life values', () => {
      expect(calculateExpiryDate('2024-01-01', 6)).toBe('2024-07-01')
      expect(calculateExpiryDate('2024-01-01', 24)).toBe('2026-01-01')
      expect(calculateExpiryDate('2024-01-01', 1)).toBe('2024-02-01')
    })

    it('should return null for missing production date', () => {
      expect(calculateExpiryDate(null, 12)).toBeNull()
      expect(calculateExpiryDate('', 12)).toBeNull()
      expect(calculateExpiryDate(undefined, 12)).toBeNull()
    })

    it('should return null for missing shelf life', () => {
      expect(calculateExpiryDate('2024-01-01', null)).toBeNull()
      expect(calculateExpiryDate('2024-01-01', 0)).toBeNull()
      expect(calculateExpiryDate('2024-01-01', undefined)).toBeNull()
    })

    it('should return null for invalid date', () => {
      expect(calculateExpiryDate('invalid-date', 12)).toBeNull()
      expect(calculateExpiryDate('2024-13-01', 12)).toBeNull()
    })

    it('should handle edge cases for month boundaries', () => {
      expect(calculateExpiryDate('2024-01-31', 1)).toBe('2024-02-29') // Leap year
      expect(calculateExpiryDate('2023-01-31', 1)).toBe('2023-02-28') // Non-leap year
    })
  })

  describe('isExpiringSoon()', () => {
    it('should return true for dates within 30 days', () => {
      const date30DaysLater = dayjs().add(30, 'day').format('YYYY-MM-DD')
      const date15DaysLater = dayjs().add(15, 'day').format('YYYY-MM-DD')
      const date1DayLater = dayjs().add(1, 'day').format('YYYY-MM-DD')

      expect(isExpiringSoon(date30DaysLater)).toBe(true)
      expect(isExpiringSoon(date15DaysLater)).toBe(true)
      expect(isExpiringSoon(date1DayLater)).toBe(true)
    })

    it('should return true for today', () => {
      const today = dayjs().format('YYYY-MM-DD')
      expect(isExpiringSoon(today)).toBe(true)
    })

    it('should return false for dates more than 30 days away', () => {
      const date31DaysLater = dayjs().add(31, 'day').format('YYYY-MM-DD')
      const date60DaysLater = dayjs().add(60, 'day').format('YYYY-MM-DD')

      expect(isExpiringSoon(date31DaysLater)).toBe(false)
      expect(isExpiringSoon(date60DaysLater)).toBe(false)
    })

    it('should return false for past dates', () => {
      const yesterday = dayjs().subtract(1, 'day').format('YYYY-MM-DD')
      const lastMonth = dayjs().subtract(1, 'month').format('YYYY-MM-DD')

      expect(isExpiringSoon(yesterday)).toBe(false)
      expect(isExpiringSoon(lastMonth)).toBe(false)
    })

    it('should return false for null or invalid dates', () => {
      expect(isExpiringSoon(null)).toBe(false)
      expect(isExpiringSoon('')).toBe(false)
      expect(isExpiringSoon('invalid-date')).toBe(false)
    })
  })

  describe('isExpired()', () => {
    it('should return true for past dates', () => {
      const yesterday = dayjs().subtract(1, 'day').format('YYYY-MM-DD')
      const lastMonth = dayjs().subtract(1, 'month').format('YYYY-MM-DD')
      const lastYear = dayjs().subtract(1, 'year').format('YYYY-MM-DD')

      expect(isExpired(yesterday)).toBe(true)
      expect(isExpired(lastMonth)).toBe(true)
      expect(isExpired(lastYear)).toBe(true)
    })

    it('should return false for today', () => {
      const today = dayjs().format('YYYY-MM-DD')
      expect(isExpired(today)).toBe(false)
    })

    it('should return false for future dates', () => {
      const tomorrow = dayjs().add(1, 'day').format('YYYY-MM-DD')
      const nextMonth = dayjs().add(1, 'month').format('YYYY-MM-DD')

      expect(isExpired(tomorrow)).toBe(false)
      expect(isExpired(nextMonth)).toBe(false)
    })

    it('should return false for null or invalid dates', () => {
      expect(isExpired(null)).toBe(false)
      expect(isExpired('')).toBe(false)
      expect(isExpired('invalid-date')).toBe(false)
    })
  })

  describe('calculateMaterialStatus()', () => {
    const futureDate = dayjs().add(60, 'day').format('YYYY-MM-DD')
    const expiringSoonDate = dayjs().add(15, 'day').format('YYYY-MM-DD')
    const expiredDate = dayjs().subtract(1, 'day').format('YYYY-MM-DD')

    it('should return EXPIRED for expired materials', () => {
      expect(calculateMaterialStatus(100, 10, expiredDate)).toBe('EXPIRED')
      expect(calculateMaterialStatus(0, 10, expiredDate)).toBe('EXPIRED')
    })

    it('should return OUT when quantity is 0', () => {
      expect(calculateMaterialStatus(0, 10, futureDate)).toBe('OUT')
    })

    it('should return LOW when quantity is at or below threshold', () => {
      expect(calculateMaterialStatus(10, 10, futureDate)).toBe('LOW')
      expect(calculateMaterialStatus(5, 10, futureDate)).toBe('LOW')
      expect(calculateMaterialStatus(1, 10, futureDate)).toBe('LOW')
    })

    it('should return EXPIRING_SOON when expiring within 30 days', () => {
      expect(calculateMaterialStatus(100, 10, expiringSoonDate)).toBe('EXPIRING_SOON')
    })

    it('should return NORMAL for normal materials', () => {
      expect(calculateMaterialStatus(100, 10, futureDate)).toBe('NORMAL')
      expect(calculateMaterialStatus(50, 10, futureDate)).toBe('NORMAL')
    })

    it('should prioritize EXPIRED over other statuses', () => {
      expect(calculateMaterialStatus(0, 10, expiredDate)).toBe('EXPIRED')
      expect(calculateMaterialStatus(5, 10, expiredDate)).toBe('EXPIRED')
    })

    it('should prioritize OUT over LOW', () => {
      expect(calculateMaterialStatus(0, 10, futureDate)).toBe('OUT')
    })

    it('should handle materials without expiry date', () => {
      expect(calculateMaterialStatus(100, 10, null)).toBe('NORMAL')
      expect(calculateMaterialStatus(5, 10, null)).toBe('LOW')
      expect(calculateMaterialStatus(0, 10, null)).toBe('OUT')
    })
  })

  describe('formatDate()', () => {
    it('should format date with default format', () => {
      expect(formatDate('2024-01-15')).toBe('2024-01-15')
    })

    it('should format date with custom format', () => {
      expect(formatDate('2024-01-15', 'YYYY/MM/DD')).toBe('2024/01/15')
      expect(formatDate('2024-01-15', 'MM-DD-YYYY')).toBe('01-15-2024')
      expect(formatDate('2024-01-15', 'YYYY年MM月DD日')).toBe('2024年01月15日')
    })

    it('should return "-" for null or empty dates', () => {
      expect(formatDate(null)).toBe('-')
      expect(formatDate('')).toBe('-')
      expect(formatDate(undefined)).toBe('-')
    })

    it('should return "-" for invalid dates', () => {
      expect(formatDate('invalid-date')).toBe('-')
      expect(formatDate('2024-13-01')).toBe('-')
    })

    it('should handle ISO datetime strings', () => {
      expect(formatDate('2024-01-15T10:30:00Z')).toBe('2024-01-15')
      expect(formatDate('2024-01-15T10:30:00Z', 'YYYY-MM-DD HH:mm')).toBe('2024-01-15 10:30')
    })
  })
})
