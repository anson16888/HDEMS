<template>
  <section class="page">
    <PageHeader
      title="科室信息管理"
      description="维护医院科室基本信息,包括科室编码、名称、类型等。"
      :cta-text="showForm ? '返回列表' : '新增科室'"
      :show-cta="true"
      @cta-click="toggleForm"
    />

    <!-- 科室列表 -->
    <div v-if="!showForm" class="card">
      <Table
        :columns="columns"
        :data-source="departments"
        :loading="loading"
        :pagination="pagination"
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
    </div>

    <!-- 科室表单 -->
    <div v-else class="card">
      <DepartmentForm
        :department="editingDepartment"
        @submit="handleSubmit"
        @cancel="handleCancel"
      />
    </div>
  </section>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { message, Table, Button, Space, Popconfirm, Tag } from 'ant-design-vue'
import PageHeader from '../components/PageHeader.vue'
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
