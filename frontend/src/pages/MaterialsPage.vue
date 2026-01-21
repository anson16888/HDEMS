<template>
  <div class="materials-page">
    <!-- Page Header -->
    <a-page-header title="物资管理" sub-title="管理应急物资库存、预警状态与出入库记录" />

    <!-- Search and Action Card -->
    <a-card class="search-card" :bordered="false">
      <a-form layout="inline">
        <a-row :gutter="16" style="width: 100%">
          <a-col :span="6">
            <a-form-item label="关键字">
              <a-input
                v-model:value="materialStore.filters.keyword"
                placeholder="搜索物资名称、编号、规格"
                allow-clear
                @change="handleSearch"
              >
                <template #prefix>
                  <SearchOutlined />
                </template>
              </a-input>
            </a-form-item>
          </a-col>
          <a-col :span="4">
            <a-form-item label="物资类型">
              <a-select
                v-model:value="materialStore.filters.type"
                placeholder="选择类型"
                allow-clear
                style="width: 100%"
                @change="handleSearch"
              >
                <a-select-option value="">全部</a-select-option>
                <a-select-option value="FOOD">食品类</a-select-option>
                <a-select-option value="MEDICAL">医疗用品</a-select-option>
                <a-select-option value="EQUIPMENT">救援设备</a-select-option>
                <a-select-option value="CLOTHING">衣物类</a-select-option>
                <a-select-option value="OTHER">其他</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :span="4">
            <a-form-item label="库存状态">
              <a-select
                v-model:value="materialStore.filters.status"
                placeholder="选择状态"
                allow-clear
                style="width: 100%"
                @change="handleSearch"
              >
                <a-select-option value="">全部</a-select-option>
                <a-select-option value="NORMAL">正常</a-select-option>
                <a-select-option value="LOW">库存偏低</a-select-option>
                <a-select-option value="OUT">已耗尽</a-select-option>
                <a-select-option value="EXPIRED">已过期</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :span="8">
            <a-form-item :wrapper-col="{ span: 24 }">
              <a-space>
                <a-button type="primary" @click="handleSearch">
                  <template #icon><SearchOutlined /></template>
                  查询
                </a-button>
                <a-button @click="handleResetFilters">重置</a-button>
                <a-button @click="handleImport">
                  <template #icon><UploadOutlined /></template>
                  导入Excel
                </a-button>
                <a-button @click="handleExport">
                  <template #icon><DownloadOutlined /></template>
                  导出Excel
                </a-button>
                <a-button type="primary" @click="handleCreate">
                  <template #icon><PlusOutlined /></template>
                  新增物资
                </a-button>
              </a-space>
            </a-form-item>
          </a-col>
        </a-row>
      </a-form>
    </a-card>

    <!-- Materials Table -->
    <a-card :bordered="false" class="table-card">
      <a-spin :spinning="materialStore.loading">
        <a-empty
          v-if="materialStore.filteredMaterials.length === 0 && !materialStore.loading"
          :description="hasActiveFilters ? '没有找到匹配的物资' : '暂无物资数据'"
        >
          <a-button type="primary" v-if="!hasActiveFilters" @click="handleCreate">
            创建第一个物资
          </a-button>
        </a-empty>

        <a-table
          v-else
          :columns="columns"
          :data-source="materialStore.filteredMaterials"
          :pagination="false"
          :row-key="record => record.id"
          :scroll="{ x: 1200 }"
        >
          <!-- 物资类型 -->
          <template #bodyCell="{ column, record }">
            <template v-if="column.key === 'material_type'">
              <a-tag :color="getTypeColor(record.material_type)">
                {{ getTypeLabel(record.material_type) }}
              </a-tag>
            </template>

            <!-- 库存状态 -->
            <template v-else-if="column.key === 'status'">
              <a-badge
                :status="getStatusBadgeStatus(record.status)"
                :text="getStatusLabel(record.status)"
              />
            </template>

            <!-- 过期日期 -->
            <template v-else-if="column.key === 'expiry_date'">
              <span v-if="record.expiry_date">
                {{ formatDate(record.expiry_date) }}
                <a-tag v-if="isExpiringSoon(record.expiry_date)" color="orange" style="margin-left: 8px">
                  即将过期
                </a-tag>
              </span>
              <span v-else>-</span>
            </template>

            <!-- 操作列 -->
            <template v-else-if="column.key === 'action'">
              <a-space>
                <a-tooltip title="查看详情">
                  <a-button type="text" size="small" @click="handleView(record)">
                    <template #icon><EyeOutlined /></template>
                  </a-button>
                </a-tooltip>

                <a-tooltip title="编辑">
                  <a-button type="text" size="small" @click="handleEdit(record)">
                    <template #icon><EditOutlined /></template>
                  </a-button>
                </a-tooltip>

                <a-popconfirm
                  title="确定要删除该物资吗？此操作不可撤销。"
                  ok-text="确认"
                  cancel-text="取消"
                  @confirm="handleDelete(record)"
                >
                  <a-tooltip title="删除">
                    <a-button type="text" size="small" danger>
                      <template #icon><DeleteOutlined /></template>
                    </a-button>
                  </a-tooltip>
                </a-popconfirm>
              </a-space>
            </template>
          </template>
        </a-table>

        <!-- Pagination -->
        <div v-if="materialStore.filteredMaterials.length > 0" class="pagination-wrapper">
          <a-pagination
            v-model:current="materialStore.pagination.current"
            v-model:page-size="materialStore.pagination.pageSize"
            :page-size-options="['10', '20', '50', '100']"
            :total="materialStore.pagination.total"
            :show-total="(total) => `共 ${total} 条`"
            show-size-changer
            show-quick-jumper
            @change="handleTableChange"
          />
        </div>
      </a-spin>
    </a-card>

    <!-- 物资表单弹窗 -->
    <MaterialFormModal
      v-model:visible="formModalVisible"
      :mode="formMode"
      :material="currentMaterial"
      @save="handleFormSave"
    />

    <!-- 物资详情弹窗 -->
    <MaterialDetailModal
      v-model:visible="detailModalVisible"
      :material="currentMaterial"
    />

    <!-- Excel导入弹窗 -->
    <MaterialImportModal
      v-model:visible="importModalVisible"
      @success="handleImportSuccess"
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import {
  PlusOutlined,
  SearchOutlined,
  EyeOutlined,
  EditOutlined,
  DeleteOutlined,
  UploadOutlined,
  DownloadOutlined
} from '@ant-design/icons-vue'
import { useMaterialStore } from '../stores/material.store'
import MaterialFormModal from '../components/modals/MaterialFormModal.vue'
import MaterialDetailModal from '../components/modals/MaterialDetailModal.vue'
import MaterialImportModal from '../components/modals/MaterialImportModal.vue'
import dayjs from 'dayjs'

// Store
const materialStore = useMaterialStore()

// 弹窗状态
const formModalVisible = ref(false)
const detailModalVisible = ref(false)
const importModalVisible = ref(false)
const formMode = ref('create') // 'create' | 'edit'
const currentMaterial = ref(null)

// 表格列配置
const columns = [
  {
    title: '物资编号',
    dataIndex: 'material_code',
    key: 'material_code',
    width: 150
  },
  {
    title: '物资名称',
    dataIndex: 'material_name',
    key: 'material_name',
    width: 200
  },
  {
    title: '类型',
    dataIndex: 'material_type',
    key: 'material_type',
    width: 120
  },
  {
    title: '库存',
    dataIndex: 'quantity',
    key: 'quantity',
    width: 100,
    align: 'right'
  },
  {
    title: '状态',
    dataIndex: 'status',
    key: 'status',
    width: 120
  },
  {
    title: '过期日期',
    dataIndex: 'expiry_date',
    key: 'expiry_date',
    width: 150
  },
  {
    title: '操作',
    key: 'action',
    fixed: 'right',
    width: 200,
    align: 'center'
  }
]

/**
 * 获取物资类型颜色
 */
function getTypeColor(type) {
  const colors = {
    FOOD: 'orange',
    MEDICAL: 'green',
    EQUIPMENT: 'blue',
    CLOTHING: 'purple',
    OTHER: 'default'
  }
  return colors[type] || 'default'
}

/**
 * 获取物资类型标签
 */
function getTypeLabel(type) {
  const labels = {
    FOOD: '食品类',
    MEDICAL: '医疗用品',
    EQUIPMENT: '救援设备',
    CLOTHING: '衣物类',
    OTHER: '其他'
  }
  return labels[type] || type
}

/**
 * 获取库存状态标签
 */
function getStatusLabel(status) {
  const labels = {
    NORMAL: '正常',
    LOW: '库存偏低',
    OUT: '已耗尽',
    EXPIRED: '已过期'
  }
  return labels[status] || status
}

/**
 * 获取Badge状态（用于a-badge组件）
 */
function getStatusBadgeStatus(status) {
  const badgeStatus = {
    NORMAL: 'success',
    LOW: 'warning',
    OUT: 'error',
    EXPIRED: 'default'
  }
  return badgeStatus[status] || 'default'
}

/**
 * 判断是否有激活的筛选条件
 */
const hasActiveFilters = computed(() => {
  return materialStore.filters.keyword ||
         materialStore.filters.type ||
         materialStore.filters.status
})

/**
 * 格式化日期
 */
function formatDate(date) {
  return dayjs(date).format('YYYY-MM-DD')
}

/**
 * 判断是否即将过期（30天内）
 */
function isExpiringSoon(date) {
  if (!date) return false
  const today = dayjs()
  const expiryDate = dayjs(date)
  const diffDays = expiryDate.diff(today, 'day')
  return diffDays <= 30 && diffDays >= 0
}

/**
 * 搜索
 */
function handleSearch() {
  materialStore.pagination.current = 1
}

/**
 * 重置筛选
 */
function handleResetFilters() {
  materialStore.resetFilters()
  message.success('筛选条件已重置')
}

/**
 * 新增物资
 */
function handleCreate() {
  formMode.value = 'create'
  currentMaterial.value = null
  formModalVisible.value = true
}

/**
 * 查看详情
 */
function handleView(record) {
  currentMaterial.value = record
  detailModalVisible.value = true
}

/**
 * 编辑物资
 */
function handleEdit(record) {
  formMode.value = 'edit'
  currentMaterial.value = record
  formModalVisible.value = true
}

/**
 * 删除物资
 */
async function handleDelete(record) {
  try {
    await materialStore.deleteMaterial(record.id)
    message.success('物资删除成功')
  } catch (error) {
    message.error('删除失败：' + (error.message || '未知错误'))
  }
}

/**
 * 表单保存
 */
async function handleFormSave(data) {
  try {
    if (formMode.value === 'create') {
      await materialStore.createMaterial(data)
      message.success('物资创建成功')
    } else {
      await materialStore.updateMaterial(currentMaterial.value.id, data)
      message.success('物资更新成功')
    }
    formModalVisible.value = false
  } catch (error) {
    message.error('保存失败：' + (error.message || '未知错误'))
  }
}

/**
 * 导入Excel
 */
function handleImport() {
  importModalVisible.value = true
}

/**
 * 导入成功回调
 */
function handleImportSuccess() {
  importModalVisible.value = false
  // 刷新列表
  materialStore.fetchMaterials()
}

/**
 * 导出Excel
 */
async function handleExport() {
  try {
    const blob = await materialStore.exportMaterials(materialStore.filters)

    // 创建下载链接
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `物资清单_${dayjs().format('YYYYMMDD_HHmmss')}.xlsx`
    link.click()
    window.URL.revokeObjectURL(url)

    message.success('导出成功')
  } catch (error) {
    message.error('导出失败：' + (error.message || '未知错误'))
  }
}

/**
 * 表格变化（分页、排序、筛选）
 */
function handleTableChange(pagination, filters, sorter) {
  materialStore.setPagination({
    current: pagination.current,
    pageSize: pagination.pageSize
  })

  // TODO: 处理排序
  if (sorter.field) {
    console.log('排序:', sorter.field, sorter.order)
  }
}

// 生命周期
onMounted(() => {
  materialStore.fetchMaterials()
})
</script>

<style scoped>
.materials-page {
  padding: 24px;
}

.search-card {
  margin-bottom: 16px;
}

.table-card {
  margin-bottom: 16px;
}

.pagination-wrapper {
  display: flex;
  justify-content: flex-end;
  margin-top: 16px;
}
</style>
