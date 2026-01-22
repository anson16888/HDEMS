/**
 * Seed data for Mock authentication
 * Automatically initializes default users and system data
 */

import { sha256 } from './crypto.js'

/**
 * Generate seed users
 * @returns {Promise<Array>} Array of seed user objects
 */
export async function generateSeedUsers() {
  const now = new Date().toISOString()

  // Hash passwords for seed users
  const [adminHash, dutyHash, materialHash] = await Promise.all([
    sha256('admin123456'),
    sha256('duty123456'),
    sha256('material123456')
  ])

  return [
    {
      id: 'user-001',
      username: 'admin',
      password: adminHash,
      real_name: '系统管理员',
      phone: '13800138000',
      department: '信息科',
      roles: ['SYSTEM_ADMIN'],
      status: 'ACTIVE',
      login_attempts: 0,
      locked_until: null,
      last_login_at: null,
      created_at: now,
      updated_at: now
    },
    {
      id: 'user-002',
      username: 'duty_admin',
      password: dutyHash,
      real_name: '值班管理员',
      phone: '13800138001',
      department: '医务科',
      roles: ['DUTY_ADMIN'],
      status: 'ACTIVE',
      login_attempts: 0,
      locked_until: null,
      last_login_at: null,
      created_at: now,
      updated_at: now
    },
    {
      id: 'user-003',
      username: 'material_admin',
      password: materialHash,
      real_name: '物资管理员',
      phone: '13800138002',
      department: '设备科',
      roles: ['MATERIAL_ADMIN'],
      status: 'ACTIVE',
      login_attempts: 0,
      locked_until: null,
      last_login_at: null,
      created_at: now,
      updated_at: now
    }
  ]
}

/**
 * Generate initialization key
 * @returns {string} 32-character hex string
 */
export function generateInitKey() {
  const chars = '0123456789abcdef'
  let key = ''
  for (let i = 0; i < 32; i++) {
    key += chars[Math.floor(Math.random() * chars.length)]
  }
  return key
}

/**
 * Get mock configuration
 * @returns {object} Mock configuration object
 */
export function getMockConfig() {
  return {
    networkDelay: { min: 300, max: 800 },
    errorRate: 0.05,
    maxLoginAttempts: 5,
    lockoutDuration: 30 * 60 * 1000, // 30 minutes
    tokenExpiry: 8 * 60 * 60 * 1000, // 8 hours
    tokenRefreshWindow: 60 * 60 * 1000, // 1 hour
    seedOnEmpty: true,
    demoModeBanner: true
  }
}

/**
 * Display initialization key in console
 * @param {string} initKey - Initialization key
 */
export function displayInitKey(initKey) {
  console.log('')
  console.log('='.repeat(60))
  console.log('🔑 HDEMS Mock Authentication - Initialization Key')
  console.log('='.repeat(60))
  console.log(`Initialization Key: ${initKey}`)
  console.log('')
  console.log('⚠️  IMPORTANT: Save this key for password reset operations')
  console.log('⚠️  To retrieve later: localStorage.getItem("hdems_init_key")')
  console.log('='.repeat(60))
  console.log('')
}
