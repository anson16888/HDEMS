import { describe, it, expect, beforeEach, vi } from 'vitest'
import { MaterialService, materialService } from '../material.service'
import { materialApi } from '../../api/material.api'

// Mock the material API
vi.mock('../../api/material.api', () => ({
  materialApi: {
    getMaterials: vi.fn(),
    getMaterialById: vi.fn(),
    createMaterial: vi.fn(),
    updateMaterial: vi.fn(),
    deleteMaterial: vi.fn(),
    batchDeleteMaterials: vi.fn(),
    importMaterials: vi.fn(),
    exportMaterials: vi.fn(),
    downloadTemplate: vi.fn(),
    getStatistics: vi.fn(),
    getTypeDisplayName: vi.fn(),
    getStatusDisplayName: vi.fn()
  }
}))

describe('Material Service', () => {
  let service

  beforeEach(() => {
    service = new MaterialService()
    vi.clearAllMocks()
  })

  describe('Initialization', () => {
    it('should initialize successfully', async () => {
      await service.initialize()
      expect(service.initialized).toBe(true)
    })

    it('should only initialize once', async () => {
      await service.initialize()
      await service.initialize()
      expect(service.initialized).toBe(true)
    })
  })

  describe('list()', () => {
    it('should call API with correct parameters', async () => {
      const mockResponse = { list: [], total: 0 }
      materialApi.getMaterials.mockResolvedValue(mockResponse)

      const params = {
        keyword: '口罩',
        type: 'type-001',
        status: 'NORMAL',
        page: 1,
        pageSize: 20
      }

      await service.list(params)

      expect(materialApi.getMaterials).toHaveBeenCalledWith({
        keyword: '口罩',
        materialTypeId: 'type-001',
        status: 0, // NORMAL mapped to 0
        page: 1,
        pageSize: 20,
        hospitalId: undefined
      })
    })

    it('should map status strings to numbers', async () => {
      materialApi.getMaterials.mockResolvedValue({ list: [], total: 0 })

      const statusMappings = [
        { input: 'NORMAL', expected: 0 },
        { input: 'LOW', expected: 1 },
        { input: 'OUT', expected: 2 },
        { input: 'EXPIRED', expected: 3 },
        { input: 'EXPIRING_SOON', expected: 4 }
      ]

      for (const { input, expected } of statusMappings) {
        await service.list({ status: input })
        expect(materialApi.getMaterials).toHaveBeenCalledWith(
          expect.objectContaining({ status: expected })
        )
      }
    })

    it('should handle empty parameters', async () => {
      materialApi.getMaterials.mockResolvedValue({ list: [], total: 0 })

      await service.list()

      expect(materialApi.getMaterials).toHaveBeenCalledWith({
        keyword: undefined,
        page: undefined,
        pageSize: undefined,
        hospitalId: undefined,
        materialTypeId: undefined
      })
    })
  })

  describe('getById()', () => {
    it('should fetch material by ID', async () => {
      const mockMaterial = { id: '123', material_name: '测试物资' }
      materialApi.getMaterialById.mockResolvedValue(mockMaterial)

      const result = await service.getById('123')

      expect(materialApi.getMaterialById).toHaveBeenCalledWith('123')
      expect(result).toEqual(mockMaterial)
    })
  })

  describe('create()', () => {
    it('should create material with valid data', async () => {
      const validData = {
        material_name: '医用口罩',
        material_type: 'type-001',
        unit: '盒',
        quantity: 100,
        location: '仓库A'
      }

      materialApi.createMaterial.mockResolvedValue({ success: true })

      await service.create(validData)

      expect(materialApi.createMaterial).toHaveBeenCalledWith(validData)
    })

    it('should throw error if material_name is missing', async () => {
      const invalidData = {
        material_type: 'type-001',
        unit: '盒',
        quantity: 100,
        location: '仓库A'
      }

      await expect(service.create(invalidData)).rejects.toMatchObject({
        code: 'VALIDATION_ERROR',
        message: '物资名称不能为空'
      })
    })

    it('should throw error if material_name exceeds 100 characters', async () => {
      const invalidData = {
        material_name: 'a'.repeat(101),
        material_type: 'type-001',
        unit: '盒',
        quantity: 100,
        location: '仓库A'
      }

      await expect(service.create(invalidData)).rejects.toMatchObject({
        code: 'VALIDATION_ERROR',
        message: '物资名称最多100个字符'
      })
    })

    it('should throw error if material_type is missing', async () => {
      const invalidData = {
        material_name: '医用口罩',
        unit: '盒',
        quantity: 100,
        location: '仓库A'
      }

      await expect(service.create(invalidData)).rejects.toMatchObject({
        code: 'VALIDATION_ERROR',
        message: '请选择物资类型'
      })
    })

    it('should throw error if unit is missing', async () => {
      const invalidData = {
        material_name: '医用口罩',
        material_type: 'type-001',
        quantity: 100,
        location: '仓库A'
      }

      await expect(service.create(invalidData)).rejects.toMatchObject({
        code: 'VALIDATION_ERROR',
        message: '单位不能为空'
      })
    })

    it('should throw error if quantity is missing', async () => {
      const invalidData = {
        material_name: '医用口罩',
        material_type: 'type-001',
        unit: '盒',
        location: '仓库A'
      }

      await expect(service.create(invalidData)).rejects.toMatchObject({
        code: 'VALIDATION_ERROR',
        message: '库存数量不能为空'
      })
    })

    it('should throw error if quantity is negative', async () => {
      const invalidData = {
        material_name: '医用口罩',
        material_type: 'type-001',
        unit: '盒',
        quantity: -10,
        location: '仓库A'
      }

      await expect(service.create(invalidData)).rejects.toMatchObject({
        code: 'VALIDATION_ERROR',
        message: '库存数量不能为负数'
      })
    })

    it('should throw error if location is missing', async () => {
      const invalidData = {
        material_name: '医用口罩',
        material_type: 'type-001',
        unit: '盒',
        quantity: 100
      }

      await expect(service.create(invalidData)).rejects.toMatchObject({
        code: 'VALIDATION_ERROR',
        message: '存放位置不能为空'
      })
    })

    it('should accept quantity of 0', async () => {
      const validData = {
        material_name: '医用口罩',
        material_type: 'type-001',
        unit: '盒',
        quantity: 0,
        location: '仓库A'
      }

      materialApi.createMaterial.mockResolvedValue({ success: true })

      await expect(service.create(validData)).resolves.toBeDefined()
    })
  })

  describe('update()', () => {
    it('should update material with valid data', async () => {
      const validData = {
        material_name: '医用口罩',
        material_type: 'type-001',
        unit: '盒',
        quantity: 200,
        location: '仓库B'
      }

      materialApi.updateMaterial.mockResolvedValue({ success: true })

      await service.update('123', validData)

      expect(materialApi.updateMaterial).toHaveBeenCalledWith('123', validData)
    })

    it('should validate data before updating', async () => {
      const invalidData = {
        material_name: '',
        material_type: 'type-001',
        unit: '盒',
        quantity: 100,
        location: '仓库A'
      }

      await expect(service.update('123', invalidData)).rejects.toMatchObject({
        code: 'VALIDATION_ERROR'
      })
    })
  })

  describe('delete()', () => {
    it('should delete material by ID', async () => {
      materialApi.deleteMaterial.mockResolvedValue({ success: true })

      await service.delete('123')

      expect(materialApi.deleteMaterial).toHaveBeenCalledWith('123')
    })
  })

  describe('batchDelete()', () => {
    it('should batch delete materials', async () => {
      const ids = ['123', '456', '789']
      materialApi.batchDeleteMaterials.mockResolvedValue({ success: true })

      await service.batchDelete(ids)

      expect(materialApi.batchDeleteMaterials).toHaveBeenCalledWith(ids)
    })
  })

  describe('import()', () => {
    it('should import Excel file', async () => {
      const file = new File([''], 'materials.xlsx')
      materialApi.importMaterials.mockResolvedValue({ success: true, imported: 10 })

      const result = await service.import(file)

      expect(materialApi.importMaterials).toHaveBeenCalledWith(file)
      expect(result.imported).toBe(10)
    })
  })

  describe('export()', () => {
    it('should export materials with filters', async () => {
      const blob = new Blob(['data'])
      materialApi.exportMaterials.mockResolvedValue(blob)

      const filters = {
        keyword: '口罩',
        type: 'type-001',
        status: 'LOW'
      }

      const result = await service.export(filters)

      expect(materialApi.exportMaterials).toHaveBeenCalledWith({
        keyword: '口罩',
        materialTypeId: 'type-001',
        status: 1, // LOW mapped to 1
        hospitalId: undefined
      })
      expect(result).toBe(blob)
    })

    it('should handle empty filters', async () => {
      const blob = new Blob(['data'])
      materialApi.exportMaterials.mockResolvedValue(blob)

      await service.export({})

      expect(materialApi.exportMaterials).toHaveBeenCalledWith({
        keyword: undefined,
        materialTypeId: undefined,
        hospitalId: undefined
      })
    })
  })

  describe('downloadTemplate()', () => {
    it('should download import template', async () => {
      const blob = new Blob(['template'])
      materialApi.downloadTemplate.mockResolvedValue(blob)

      const result = await service.downloadTemplate()

      expect(materialApi.downloadTemplate).toHaveBeenCalled()
      expect(result).toBe(blob)
    })
  })

  describe('getStatistics()', () => {
    it('should get statistics without hospital filter', async () => {
      const mockStats = { total: 100, low: 20 }
      materialApi.getStatistics.mockResolvedValue(mockStats)

      const result = await service.getStatistics()

      expect(materialApi.getStatistics).toHaveBeenCalledWith(null)
      expect(result).toEqual(mockStats)
    })

    it('should get statistics with hospital filter', async () => {
      const mockStats = { total: 50, low: 10 }
      materialApi.getStatistics.mockResolvedValue(mockStats)

      const result = await service.getStatistics('hospital-001')

      expect(materialApi.getStatistics).toHaveBeenCalledWith('hospital-001')
      expect(result).toEqual(mockStats)
    })
  })

  describe('validateMaterialData()', () => {
    it('should pass validation for complete valid data', () => {
      const validData = {
        material_name: '医用口罩',
        material_type: 'type-001',
        unit: '盒',
        quantity: 100,
        location: '仓库A'
      }

      expect(() => service.validateMaterialData(validData)).not.toThrow()
    })

    it('should validate all required fields', () => {
      const testCases = [
        { data: { material_type: 'type-001', unit: '盒', quantity: 100, location: '仓库A' }, error: '物资名称不能为空' },
        { data: { material_name: '口罩', unit: '盒', quantity: 100, location: '仓库A' }, error: '请选择物资类型' },
        { data: { material_name: '口罩', material_type: 'type-001', quantity: 100, location: '仓库A' }, error: '单位不能为空' },
        { data: { material_name: '口罩', material_type: 'type-001', unit: '盒', location: '仓库A' }, error: '库存数量不能为空' },
        { data: { material_name: '口罩', material_type: 'type-001', unit: '盒', quantity: 100 }, error: '存放位置不能为空' }
      ]

      testCases.forEach(({ data, error }) => {
        expect(() => service.validateMaterialData(data)).toThrow(error)
      })
    })
  })

  describe('Exported materialService object', () => {
    it('should have all required methods', () => {
      expect(typeof materialService.list).toBe('function')
      expect(typeof materialService.getById).toBe('function')
      expect(typeof materialService.create).toBe('function')
      expect(typeof materialService.update).toBe('function')
      expect(typeof materialService.delete).toBe('function')
      expect(typeof materialService.batchDelete).toBe('function')
      expect(typeof materialService.import).toBe('function')
      expect(typeof materialService.export).toBe('function')
      expect(typeof materialService.downloadTemplate).toBe('function')
      expect(typeof materialService.getStatistics).toBe('function')
    })
  })
})
