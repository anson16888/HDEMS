import { createPinia, setActivePinia } from 'pinia'

/**
 * 创建测试用的 Pinia 实例
 */
export function createTestPinia() {
  const pinia = createPinia()
  setActivePinia(pinia)
  return pinia
}

/**
 * 创建模拟的物资数据
 */
export function mockMaterialData(overrides = {}) {
  return {
    id: '1234567890',
    material_code: 'MAT-001',
    material_name: '医用口罩',
    material_type: 'type-001',
    materialTypeName: '医疗用品',
    specification: 'N95标准',
    quantity: 100,
    unit: '盒',
    production_date: '2024-01-01',
    shelf_life: 24,
    expiry_date: '2026-01-01',
    location: '仓库A-01',
    hospital_id: 'hospital-001',
    status: 'NORMAL',
    remark: '测试备注',
    created_at: '2024-01-01T00:00:00Z',
    updated_at: '2024-01-01T00:00:00Z',
    ...overrides
  }
}

/**
 * 创建模拟的物资列表
 */
export function mockMaterialList(count = 5) {
  return Array.from({ length: count }, (_, index) =>
    mockMaterialData({
      id: `material-${index + 1}`,
      material_code: `MAT-${String(index + 1).padStart(3, '0')}`,
      material_name: `测试物资${index + 1}`,
      quantity: (index + 1) * 10
    })
  )
}

/**
 * 创建模拟的 API 响应
 */
export function mockApiResponse(data, success = true) {
  return {
    success,
    data,
    message: success ? '操作成功' : '操作失败',
    code: success ? 200 : 500
  }
}

/**
 * 创建模拟的分页响应
 */
export function mockPaginatedResponse(items, page = 1, pageSize = 20) {
  return {
    success: true,
    data: {
      list: items,
      total: items.length,
      page,
      pageSize
    }
  }
}

/**
 * 延迟函数 (用于模拟异步操作)
 */
export function delay(ms = 100) {
  return new Promise(resolve => setTimeout(resolve, ms))
}

/**
 * 创建模拟的物资类型数据
 */
export function mockMaterialType(overrides = {}) {
  return {
    id: 'type-001',
    typeName: '医疗用品',
    typeCode: 'MEDICAL',
    color: 'green',
    threshold: 10,
    ...overrides
  }
}

/**
 * 创建模拟的统计数据
 */
export function mockStatistics(overrides = {}) {
  return {
    total: 100,
    normal: 70,
    low: 20,
    out: 5,
    expired: 3,
    expiringSoon: 2,
    ...overrides
  }
}
