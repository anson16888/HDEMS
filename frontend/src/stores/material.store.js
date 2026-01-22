import { defineStore } from 'pinia'
import { ref, reactive, computed } from 'vue'
import { materialService } from '../services/material.service'
import { useAuthStore } from './auth.store'

/**
 * 物资管理状态管理
 */
export const useMaterialStore = defineStore('material', () => {
  // State
  const materials = ref([])
  const loading = ref(false)
  const pagination = reactive({
    current: 1,
    pageSize: 20,
    total: 0
  })
  const filters = reactive({
    keyword: '',
    type: undefined,
    status: undefined
  })

  // Getters
  /**
   * 过滤后的物资列表
   */
  const filteredMaterials = computed(() => {
    let result = materials.value

    // 关键词搜索
    if (filters.keyword) {
      const keyword = filters.keyword.toLowerCase()
      result = result.filter(m =>
        m.material_name?.toLowerCase().includes(keyword) ||
        m.material_code?.toLowerCase().includes(keyword) ||
        m.specification?.toLowerCase().includes(keyword)
      )
    }

    // 类型筛选
    if (filters.type) {
      result = result.filter(m => m.material_type === filters.type)
    }

    // 状态筛选
    if (filters.status) {
      result = result.filter(m => m.status === filters.status)
    }

    return result
  })

  /**
   * 统计卡片数据
   */
  const statistics = computed(() => {
    const all = materials.value

    return {
      total: all.length,
      low: all.filter(m => m.status === 'LOW').length,
      out: all.filter(m => m.status === 'OUT').length,
      expired: all.filter(m => m.status === 'EXPIRED').length
    }
  })

  // Actions
  /**
   * 获取物资列表
   */
  async function fetchMaterials(params = {}) {
    loading.value = true
    try {
      const response = await materialService.list({
        page: pagination.current,
        pageSize: pagination.pageSize,
        ...params
      })

      materials.value = response.data.list || []
      pagination.total = response.data.total || 0

      return response
    } catch (error) {
      console.error('获取物资列表失败:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  /**
   * 获取物资详情
   */
  async function fetchMaterialById(id) {
    try {
      const response = await materialService.getById(id)
      return response.data
    } catch (error) {
      console.error('获取物资详情失败:', error)
      throw error
    }
  }

  /**
   * 新增物资
   */
  async function createMaterial(data) {
    loading.value = true
    try {
      const response = await materialService.create(data)

      // 刷新列表
      await fetchMaterials()

      return response
    } catch (error) {
      console.error('新增物资失败:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  /**
   * 更新物资
   */
  async function updateMaterial(id, data) {
    loading.value = true
    try {
      const response = await materialService.update(id, data)

      // 刷新列表
      await fetchMaterials()

      return response
    } catch (error) {
      console.error('更新物资失败:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  /**
   * 删除物资
   */
  async function deleteMaterial(id) {
    loading.value = true
    try {
      const response = await materialService.delete(id)

      // 从列表中移除
      const index = materials.value.findIndex(m => m.id === id)
      if (index !== -1) {
        materials.value.splice(index, 1)
      }

      // 更新总数
      pagination.total = materials.value.length

      return response
    } catch (error) {
      console.error('删除物资失败:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  /**
   * 导入Excel
   */
  async function importMaterials(file) {
    loading.value = true
    try {
      const response = await materialService.import(file)

      // 刷新列表
      await fetchMaterials()

      return response
    } catch (error) {
      console.error('导入物资失败:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  /**
   * 导出Excel
   */
  async function exportMaterials(filters = {}) {
    try {
      const response = await materialService.export(filters)
      return response
    } catch (error) {
      console.error('导出物资失败:', error)
      throw error
    }
  }

  /**
   * 设置筛选条件
   */
  function setFilters(newFilters) {
    Object.assign(filters, newFilters)
  }

  /**
   * 重置筛选条件
   */
  function resetFilters() {
    filters.keyword = ''
    filters.type = undefined
    filters.status = undefined
    pagination.current = 1
  }

  /**
   * 设置分页
   */
  function setPagination(newPagination) {
    Object.assign(pagination, newPagination)
  }

  return {
    // State
    materials,
    loading,
    pagination,
    filters,

    // Getters
    filteredMaterials,
    statistics,

    // Actions
    fetchMaterials,
    fetchMaterialById,
    createMaterial,
    updateMaterial,
    deleteMaterial,
    importMaterials,
    exportMaterials,
    setFilters,
    resetFilters,
    setPagination
  }
})
