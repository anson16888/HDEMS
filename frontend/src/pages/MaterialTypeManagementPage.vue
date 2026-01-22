<template>
  <div class="material-type-management-page">
    <a-page-header title="物资类型管理" sub-title="维护物资类型编码、颜色与启用状态" />

    <a-card class="search-card" :bordered="false">
      <a-form layout="inline">
        <a-row :gutter="16" style="width: 100%">
          <a-col :span="8">
            <a-form-item label="关键字">
              <a-input
                v-model:value="searchKeyword"
                placeholder="搜索类型编码或名称"
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
                  新增类型
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
        :data-source="materialTypes"
        :loading="loading"
        :pagination="pagination"
        :scroll="{ y: 'calc(100vh - 380px)' }"
        row-key="id"
        @change="handleTableChange"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'color'">
            <Tag :color="record.color || 'blue'">
              {{ record.color || '默认' }}
            </Tag>
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
                title="确定要删除该类型吗?"
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
      :title="editingType ? '编辑物资类型' : '新增物资类型'"
      :width="640"
      :footer="null"
      @cancel="closeModal"
    >
      <MaterialTypeForm
        :material-type="editingType"
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
import MaterialTypeForm from '../components/forms/MaterialTypeForm.vue'
import {
  getMaterialTypes,
  createMaterialType,
  updateMaterialType,
  deleteMaterialType,
  toggleMaterialType
} from '../api/materialType.api.js'

const loading = ref(false)
const materialTypes = ref([])
const searchKeyword = ref('')
const statusFilter = ref(undefined)
const modalVisible = ref(false)
const editingType = ref(null)

const pagination = reactive({
  current: 1,
  pageSize: 20,
  total: 0,
  showSizeChanger: true,
  showTotal: (total) => `共 ${total} 条记录`
})

const columns = [
  {
    title: '类型编码',
    dataIndex: 'typeCode',
    key: 'typeCode',
    width: 140
  },
  {
    title: '类型名称',
    dataIndex: 'typeName',
    key: 'typeName'
  },
  {
    title: '颜色',
    dataIndex: 'color',
    key: 'color',
    width: 120
  },
  {
    title: '状态',
    dataIndex: 'isEnabled',
    key: 'isEnabled',
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
    title: '备注',
    dataIndex: 'remark',
    key: 'remark',
    width: 200
  },
  {
    title: '操作',
    key: 'action',
    width: 150,
    align: 'center',
    fixed: 'right'
  }
]

async function loadMaterialTypes() {
  try {
    loading.value = true
    const params = {
      page: pagination.current,
      pageSize: pagination.pageSize,
      keyword: searchKeyword.value || undefined,
      isEnabled: statusFilter.value
    }
    const response = await getMaterialTypes(params)
    if (response.success) {
      materialTypes.value = response.data.items || []
      pagination.total = response.data.total || 0
    }
  } catch (error) {
    message.error('加载物资类型失败: ' + error.message)
  } finally {
    loading.value = false
  }
}

function handleSearch() {
  pagination.current = 1
  loadMaterialTypes()
}

function handleReset() {
  searchKeyword.value = ''
  statusFilter.value = undefined
  pagination.current = 1
  loadMaterialTypes()
}

function handleTableChange(paginationConfig) {
  pagination.current = paginationConfig.current
  pagination.pageSize = paginationConfig.pageSize
  loadMaterialTypes()
}

function openCreateModal() {
  editingType.value = null
  modalVisible.value = true
}

function openEditModal(record) {
  editingType.value = { ...record }
  modalVisible.value = true
}

function closeModal() {
  modalVisible.value = false
  editingType.value = null
}

async function handleDelete(id) {
  try {
    const response = await deleteMaterialType(id)
    if (response.success) {
      message.success('删除成功')
      await loadMaterialTypes()
    }
  } catch (error) {
    message.error('删除失败: ' + error.message)
  }
}

async function handleToggle(record) {
  const previous = record.isEnabled
  record.isEnabled = !previous
  try {
    const response = await toggleMaterialType(record.id)
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
    if (editingType.value) {
      response = await updateMaterialType(editingType.value.id, formData)
    } else {
      response = await createMaterialType(formData)
    }

    if (response.success) {
      message.success(editingType.value ? '更新成功' : '创建成功')
      closeModal()
      await loadMaterialTypes()
    }
  } catch (error) {
    message.error('操作失败: ' + error.message)
  }
}

onMounted(() => {
  loadMaterialTypes()
})
</script>

<style scoped>
.material-type-management-page {
  padding: 0 16px;
}

.search-card {
  margin-bottom: 16px;
}

.table-card {
  margin-bottom: 0;
}
</style>
