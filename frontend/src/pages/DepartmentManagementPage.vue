<template>
  <div class="department-management-page">
    <!-- Page Header -->
    <a-page-header
      title="科室信息管理"
      sub-title="维护医院科室基本信息,包括科室编码、名称、类型等"
    />

    <!-- Search and Action Card -->
    <a-card v-if="!showForm" class="search-card" :bordered="false">
      <a-space>
        <a-button v-if="!showForm" type="primary" @click="toggleForm">
          <template #icon><PlusOutlined /></template>
          新增科室
        </a-button>
      </a-space>
    </a-card>

    <!-- Department Table -->
    <a-card v-if="!showForm" :bordered="false" class="table-card">
      <Table
        :columns="columns"
        :data-source="departments"
        :loading="loading"
        :pagination="pagination"
        :scroll="{ y: 'calc(100vh - 450px)' }"
        row-key="id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'departmentType'">
            <Tag :color="getDepartmentTypeColor(record.departmentType)">
              {{ getDepartmentTypeName(record.departmentType) }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'action'">
            <Space>
              <Button type="link" size="small" @click="handleEdit(record)">
                编辑
              </Button>
              <Popconfirm
                title="确定要删除该科室吗?"
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

    <!-- Department Form Card -->
    <a-card v-else :bordered="false" class="form-card">
      <div class="form-header">
        <a-button @click="handleCancel">
          <template #icon><ArrowLeftOutlined /></template>
          返回列表
        </a-button>
      </div>
      <DepartmentForm
        :department="editingDepartment"
        @submit="handleSubmit"
        @cancel="handleCancel"
      />
    </a-card>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import { PlusOutlined, ArrowLeftOutlined } from '@ant-design/icons-vue'
import { Table, Button, Space, Popconfirm, Tag } from 'ant-design-vue'
import DepartmentForm from '../components/forms/DepartmentForm.vue'
import {
  getDepartments,
  createDepartment,
  updateDepartment,
  deleteDepartment
} from '../api/basicData.api.js'

const showForm = ref(false)
const loading = ref(false)
const departments = ref([])
const editingDepartment = ref(null)

const pagination = reactive({
  current: 1,
  pageSize: 20,
  showSizeChanger: true,
  showTotal: (total) => `共 ${total} 条记录`
})

// 科室类型枚举
const DEPARTMENT_TYPES = {
  CLINICAL: 'clinical',
  MEDICAL_TECH: 'medical_tech',
  ADMINISTRATIVE: 'administrative'
}

const DEPARTMENT_TYPE_NAMES = {
  clinical: '临床科室',
  medical_tech: '医技科室',
  administrative: '行政科室'
}

const columns = [
  {
    title: '科室编码',
    dataIndex: 'departmentCode',
    key: 'departmentCode',
    width: 150
  },
  {
    title: '科室名称',
    dataIndex: 'departmentName',
    key: 'departmentName'
  },
  {
    title: '科室类型',
    dataIndex: 'departmentType',
    key: 'departmentType',
    width: 150
  },
  {
    title: '排序号',
    dataIndex: 'sortOrder',
    key: 'sortOrder',
    width: 100,
    align: 'center'
  },
  {
    title: '操作',
    key: 'action',
    width: 150,
    align: 'center'
  }
]

async function loadDepartments() {
  try {
    loading.value = true
    const response = await getDepartments()
    if (response.success) {
      departments.value = response.data || []
    }
  } catch (error) {
    message.error('加载科室列表失败: ' + error.message)
  } finally {
    loading.value = false
  }
}

function getDepartmentTypeName(type) {
  return DEPARTMENT_TYPE_NAMES[type] || type
}

function getDepartmentTypeColor(type) {
  const colors = {
    clinical: 'blue',
    medical_tech: 'green',
    administrative: 'orange'
  }
  return colors[type] || 'default'
}

function toggleForm() {
  showForm.value = !showForm.value
  if (!showForm.value) {
    editingDepartment.value = null
  }
}

function handleEdit(record) {
  editingDepartment.value = { ...record }
  showForm.value = true
}

async function handleDelete(id) {
  try {
    const response = await deleteDepartment(id)
    if (response.success) {
      message.success('删除成功')
      await loadDepartments()
    }
  } catch (error) {
    message.error('删除失败: ' + error.message)
  }
}

async function handleSubmit(formData) {
  try {
    let response
    if (editingDepartment.value) {
      response = await updateDepartment(editingDepartment.value.id, formData)
    } else {
      response = await createDepartment(formData)
    }

    if (response.success) {
      message.success(editingDepartment.value ? '更新成功' : '创建成功')
      showForm.value = false
      editingDepartment.value = null
      await loadDepartments()
    }
  } catch (error) {
    message.error('操作失败: ' + error.message)
  }
}

function handleCancel() {
  showForm.value = false
  editingDepartment.value = null
}

onMounted(() => {
  loadDepartments()
})
</script>

<style scoped>
.department-management-page {
  padding: 16px;
  padding-bottom: 0;
}

.department-management-page :deep(.ant-page-header) {
  padding: 16px 24px;
  background: #fff;
  border-radius: 8px;
  margin-bottom: 16px;
}

.search-card {
  margin-bottom: 16px;
}

.table-card {
  margin-bottom: 0;
}

.form-card {
  margin-bottom: 16px;
}

.form-header {
  margin-bottom: 16px;
}
</style>
