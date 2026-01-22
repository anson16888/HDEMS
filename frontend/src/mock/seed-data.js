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
      realName: '系统管理员',
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
      realName: '值班管理员',
      phone: '13800138001',
      department: '医务科',
      roles: ['SCHEDULE_ADMIN'],
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
      realName: '物资管理员',
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

/**
 * Generate seed materials
 * @returns {Promise<Array>} Array of seed material objects
 */
export async function generateSeedMaterials() {
  const now = new Date().toISOString()

  // Helper to create date string (months from now)
  const createDate = (monthsAgo) => {
    const date = new Date()
    date.setMonth(date.getMonth() - monthsAgo)
    return date.toISOString().split('T')[0]
  }

  // Helper to calculate expiry date
  const calculateExpiryDate = (productionDate, shelfLife) => {
    if (!productionDate || !shelfLife) return null
    const prodDate = new Date(productionDate)
    const expiryDate = new Date(prodDate)
    expiryDate.setMonth(expiryDate.getMonth() + parseInt(shelfLife))
    return expiryDate.toISOString().split('T')[0]
  }

  // Helper to calculate status
  const calculateStatus = (quantity, minStock, productionDate, shelfLife) => {
    const qty = parseFloat(quantity) || 0
    const min = parseFloat(minStock) || 0

    // Check if expired
    if (productionDate && shelfLife) {
      const expiryDate = calculateExpiryDate(productionDate, shelfLife)
      if (expiryDate && new Date(expiryDate) < new Date()) {
        return 'EXPIRED'
      }
    }

    // Check stock level
    if (qty <= 0) {
      return 'OUT'
    } else if (qty <= min) {
      return 'LOW'
    } else {
      return 'NORMAL'
    }
  }

  const materials = [
    {
      id: 'material-001',
      material_code: 'MAT000001',
      material_name: '医用防护口罩',
      material_type: 'MEDICAL',
      specification: 'N95/50个装',
      quantity: 5000,
      unit: '个',
      production_date: createDate(2),
      shelf_life: 24,
      min_stock: 1000,
      location: 'A区-01货架',
      supplier: '医疗器械有限公司'
    },
    {
      id: 'material-002',
      material_code: 'MAT000002',
      material_name: '一次性医用手套',
      material_type: 'MEDICAL',
      specification: '橡胶/M码',
      quantity: 200,
      unit: '双',
      production_date: createDate(3),
      shelf_life: 36,
      min_stock: 500,
      location: 'A区-02货架',
      supplier: '医疗用品厂'
    },
    {
      id: 'material-003',
      material_code: 'MAT000003',
      material_name: '应急帐篷',
      material_type: 'EQUIPMENT',
      specification: '单人/防水',
      quantity: 50,
      unit: '顶',
      production_date: createDate(12),
      shelf_life: 60,
      min_stock: 20,
      location: 'B区-01仓库',
      supplier: '户外装备公司'
    },
    {
      id: 'material-004',
      material_code: 'MAT000004',
      material_name: '压缩饼干',
      material_type: 'FOOD',
      specification: '500g/袋',
      quantity: 0,
      unit: '袋',
      production_date: createDate(6),
      shelf_life: 18,
      min_stock: 100,
      location: 'C区-01货架',
      supplier: '食品加工厂'
    },
    {
      id: 'material-005',
      material_code: 'MAT000005',
      material_name: '急救包',
      material_type: 'MEDICAL',
      specification: '基础型',
      quantity: 30,
      unit: '套',
      production_date: createDate(15),
      shelf_life: 36,
      min_stock: 10,
      location: 'A区-03货架',
      supplier: '医疗器械有限公司'
    },
    {
      id: 'material-006',
      material_code: 'MAT000006',
      material_name: '强光手电筒',
      material_type: 'EQUIPMENT',
      specification: 'LED/可充电',
      quantity: 100,
      unit: '把',
      production_date: createDate(8),
      shelf_life: 48,
      min_stock: 50,
      location: 'B区-02货架',
      supplier: '电子设备厂'
    },
    {
      id: 'material-007',
      material_code: 'MAT000007',
      material_name: '棉被',
      material_type: 'CLOTHING',
      specification: '双人/加厚',
      quantity: 150,
      unit: '床',
      production_date: createDate(10),
      shelf_life: 72,
      min_stock: 50,
      location: 'C区-02货架',
      supplier: '家纺厂'
    },
    {
      id: 'material-008',
      material_code: 'MAT000008',
      material_name: '碘伏消毒液',
      material_type: 'MEDICAL',
      specification: '500ml/瓶',
      quantity: 80,
      unit: '瓶',
      production_date: createDate(30),
      shelf_life: 24,
      min_stock: 50,
      location: 'A区-04货架',
      supplier: '制药公司'
    },
    {
      id: 'material-009',
      material_code: 'MAT000009',
      material_name: '多功能工兵铲',
      material_type: 'EQUIPMENT',
      specification: '折叠式',
      quantity: 200,
      unit: '把',
      production_date: createDate(5),
      shelf_life: 60,
      min_stock: 100,
      location: 'B区-03货架',
      supplier: '户外装备公司'
    },
    {
      id: 'material-010',
      material_code: 'MAT000010',
      material_name: '矿泉水',
      material_type: 'FOOD',
      specification: '550ml/瓶',
      quantity: 2000,
      unit: '瓶',
      production_date: createDate(1),
      shelf_life: 12,
      min_stock: 1000,
      location: 'C区-03货架',
      supplier: '饮料公司'
    },
    {
      id: 'material-011',
      material_code: 'MAT000011',
      material_name: '救生衣',
      material_type: 'EQUIPMENT',
      specification: '成人/泡沫',
      quantity: 45,
      unit: '件',
      production_date: createDate(18),
      shelf_life: 60,
      min_stock: 50,
      location: 'B区-04货架',
      supplier: '船舶用品厂'
    },
    {
      id: 'material-012',
      material_code: 'MAT000012',
      material_name: '应急发电机',
      material_type: 'EQUIPMENT',
      specification: '5kW/汽油',
      quantity: 5,
      unit: '台',
      production_date: createDate(6),
      shelf_life: 120,
      min_stock: 2,
      location: 'B区-05仓库',
      supplier: '发电设备厂'
    }
  ]

  // Add calculated fields
  return materials.map(m => ({
    ...m,
    expiry_date: calculateExpiryDate(m.production_date, m.shelf_life),
    status: calculateStatus(m.quantity, m.min_stock, m.production_date, m.shelf_life),
    created_at: now,
    updated_at: now
  }))
}
