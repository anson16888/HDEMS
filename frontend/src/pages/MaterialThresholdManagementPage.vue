<template>
  <div class="material-threshold-management-page">
    <a-page-header title="库存阈值管理" sub-title="设置各物资类型的库存预警阈值" />

    <a-card class="search-card" :bordered="false">
      <a-form layout="inline">
        <a-row :gutter="16" style="width: 100%">
          <a-col :span="8">
            <a-form-item label="关键字">
              <a-input
                v-model:value="searchKeyword"
                placeholder="搜索物资类型名称"
                allow-clear
                @pressEnter="handleSearch"
              >
                <template #prefix>
                  <SearchOutlined />
                </template>
              </a-input>
            </a-form-item>
          </a-col>
          <a-col :span="6">
            <a-form-item label="状态">
              <a-select
                v-model:value="statusFilter"
                placeholder="全部"
                allow-clear
                style="width: 100%"
                @change="handleSearch"
              >
                <a-select-option :value="true">启用</a-select-option>
                <a-select-option :value="false">停用</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :span="10">
            <a-form-item :wrapper-col="{ span: 24 }">
              <a-space>
                <a-button type="primary" @click="handleSearch">
                  <template #icon><SearchOutlined /></template>
                  查询
                </a-button>
                <a-button @click="handleReset">重置</a-button>
                <a-button type="primary" @click="openCreateModal">
                  <template #icon><PlusOutlined /></template>
                  新增阈值
                </a-button>
              </a-space>
            </a-form-item>
          </a-col>
        </a-row>
      </a-form>
    </a-card>

    <a-card :bordered="false" class="table-card">
      <Table
        :columns="columns"
        :data-source="thresholds"
        :loading="loading"
        :pagination="pagination"
        :scroll="{ y: 'calc(100vh - 380px)' }"
        row-key="id"
        @change="handleTableChange"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'materialTypeName'">
            <Tag :color="record.materialTypeColor || 'blue'">
              {{ record.materialTypeName }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'threshold'">
            <span :class="{ 'threshold-warning': record.threshold > 0 }">
              {{ record.threshold }}
            </span>
          </template>
          <template v-else-if="column.key === 'isEnabled'">
            <Switch
              :checked="record.isEnabled"
              @change="() => handleToggle(record)"
            />
          </template>
          <template v-else-if="column.key === 'action'">
            <Space>
              <Button type="link" size="small" @click="openEditModal(record)">
                编辑
              </Button>
              <Popconfirm
                title="确定要删除该阈值配置吗?"
                ok-text="确定"
                cancel-text="取消"
                @confirm="handleDelete(record.id)"
              >
                <Button type="link" size="small" danger>删除</Button>
              </Popconfirm>
            </Space>
          </template>
        </template>
      </Table>
    </a-card>

    <a-modal
      v-model:open="modalVisible"
      :title="editingThreshold ? '编辑库存阈值' : '新增库存阈值'"
      :width="640"
      :footer="null"
      @cancel="closeModal"
    >
      <MaterialThresholdForm
        :material-threshold="editingThreshold"
        @submit="handleSubmit"
        @cancel="closeModal"
      />
    </a-modal>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import { SearchOutlined, PlusOutlined } from '@ant-design/icons-vue'
import { Table, Button, Space, Popconfirm, Tag, Switch } from 'ant-design-vue'
import MaterialThresholdForm from '../components/forms/MaterialThresholdForm.vue'
import {
  getMaterialThresholds,
  createMaterialThreshold,
  updateMaterialThreshold,
  deleteMaterialThreshold,
  toggleMaterialThreshold
} from '../api/materialThreshold.api.js'

const loading = ref(false)
const thresholds = ref([])
const searchKeyword = ref('')
const statusFilter = ref(undefined)
const modalVisible = ref(false)
const editingThreshold = ref(null)

const pagination = reactive({
  current: 1,
  pageSize: 20,
  total: 0,
  showSizeChanger: true,
  showTotal: (total) => `共 ${total} 条记录`
})

const columns = [
  {
    title: '物资类型',
    dataIndex: 'materialTypeName',
    key: 'materialTypeName',
    width: 150
  },
  {
    title: '阈值数量',
    dataIndex: 'threshold',
    key: 'threshold',
    width: 120,
    align: 'center'
  },
  {
    title: '排序号',
    dataIndex: 'sortOrder',
    key: 'sortOrder',
    width: 100,
    align: 'center'
  },
  {
    title: '状态',
    dataIndex: 'isEnabled',
    key: 'isEnabled',
    width: 100,
    align: 'center'
  },
  {
    title: '备注',
    dataIndex: 'remark',
    key: 'remark',
    width: 250
  },
  {
    title: '创建时间',
    dataIndex: 'createdAt',
    key: 'createdAt',
    width: 170
  },
  {
    title: '操作',
    key: 'action',
    width: 150,
    align: 'center',
    fixed: 'right'
  }
]

async function loadThresholds() {
  try {
    loading.value = true
    const params = {
      page: pagination.current,
      pageSize: pagination.pageSize,
      keyword: searchKeyword.value || undefined,
      isEnabled: statusFilter.value
    }
    const response = await getMaterialThresholds(params)
    if (response.success) {
      thresholds.value = response.data.items || []
      pagination.total = response.data.total || 0
    }
  } catch (error) {
    message.error('加载库存阈值失败: ' + error.message)
  } finally {
    loading.value = false
  }
}

function handleSearch() {
  pagination.current = 1
  loadThresholds()
}

function handleReset() {
  searchKeyword.value = ''
  statusFilter.value = undefined
  pagination.current = 1
  loadThresholds()
}

function handleTableChange(paginationConfig) {
  pagination.current = paginationConfig.current
  pagination.pageSize = paginationConfig.pageSize
  loadThresholds()
}

function openCreateModal() {
  editingThreshold.value = null
  modalVisible.value = true
}

function openEditModal(record) {
  editingThreshold.value = { ...record }
  modalVisible.value = true
}

function closeModal() {
  modalVisible.value = false
  editingThreshold.value = null
}

async function handleDelete(id) {
  try {
    const response = await deleteMaterialThreshold(id)
    if (response.success) {
      message.success('删除成功')
      await loadThresholds()
    }
  } catch (error) {
    message.error('删除失败: ' + error.message)
  }
}

async function handleToggle(record) {
  const previous = record.isEnabled
  record.isEnabled = !previous
  try {
    const response = await toggleMaterialThreshold(record.id)
    if (response.success) {
      message.success('状态已更新')
    }
  } catch (error) {
    record.isEnabled = previous
    message.error('更新状态失败: ' + error.message)
  }
}

async function handleSubmit(formData) {
  try {
    let response
    if (editingThreshold.value) {
      response = await updateMaterialThreshold(editingThreshold.value.id, formData)
    } else {
      response = await createMaterialThreshold(formData)
    }

    if (response.success) {
      message.success(editingThreshold.value ? '更新成功' : '创建成功')
      closeModal()
      await loadThresholds()
    } else {
      message.error(response.message || '操作失败')
    }
  } catch (error) {
    message.error('操作失败: ' + error.message)
  }
}

onMounted(() => {
  loadThresholds()
})
</script>

<style scoped>
.material-threshold-management-page {
  padding: 0 16px;
}

.search-card {
  margin-bottom: 16px;
}

.table-card {
  margin-bottom: 0;
}

.threshold-warning {
  color: #ff4d4f;
  font-weight: 500;
}
</style>
