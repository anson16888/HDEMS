<template>
  <div class="shift-management-page">
    <!-- Page Header -->
    <a-page-header
      title="班次信息管理"
      sub-title="维护值班班次基本信息,包括班次编码、名称、时间段等"
    />

    <!-- Search and Action Card -->
    <a-card v-if="!showForm" class="search-card" :bordered="false">
      <a-space>
        <a-button v-if="!showForm" type="primary" @click="toggleForm">
          <template #icon><PlusOutlined /></template>
          新增班次
        </a-button>
      </a-space>
    </a-card>

    <!-- Shift Table -->
    <a-card v-if="!showForm" :bordered="false" class="table-card">
      <Table
        :columns="columns"
        :data-source="shifts"
        :loading="loading"
        :pagination="pagination"
        :scroll="{ y: 'calc(100vh - 450px)' }"
        row-key="id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'timeRange'">
            <Tag color="blue">{{ record.timeRange || '未设置' }}</Tag>
          </template>
          <template v-else-if="column.key === 'action'">
            <Space>
              <Button type="link" size="small" @click="handleEdit(record)">
                编辑
              </Button>
              <Popconfirm
                title="确定要删除该班次吗?"
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

    <!-- Shift Form Card -->
    <a-card v-else :bordered="false" class="form-card">
      <div class="form-header">
        <a-button @click="handleCancel">
          <template #icon><ArrowLeftOutlined /></template>
          返回列表
        </a-button>
      </div>
      <ShiftForm
        :shift="editingShift"
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
import ShiftForm from '../components/forms/ShiftForm.vue'
import {
  getShifts,
  createShift,
  updateShift,
  deleteShift
} from '../api/basicData.api.js'

const showForm = ref(false)
const loading = ref(false)
const shifts = ref([])
const editingShift = ref(null)

const pagination = reactive({
  current: 1,
  pageSize: 20,
  showSizeChanger: true,
  showTotal: (total) => `共 ${total} 条记录`
})

const columns = [
  {
    title: '班次编码',
    dataIndex: 'shiftCode',
    key: 'shiftCode',
    width: 150
  },
  {
    title: '班次名称',
    dataIndex: 'shiftName',
    key: 'shiftName'
  },
  {
    title: '时间段',
    dataIndex: 'timeRange',
    key: 'timeRange',
    width: 200
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

async function loadShifts() {
  try {
    loading.value = true
    const response = await getShifts()
    if (response.success) {
      shifts.value = response.data || []
    }
  } catch (error) {
    message.error('加载班次列表失败: ' + error.message)
  } finally {
    loading.value = false
  }
}

function toggleForm() {
  showForm.value = !showForm.value
  if (!showForm.value) {
    editingShift.value = null
  }
}

function handleEdit(record) {
  editingShift.value = { ...record }
  showForm.value = true
}

async function handleDelete(id) {
  try {
    const response = await deleteShift(id)
    if (response.success) {
      message.success('删除成功')
      await loadShifts()
    }
  } catch (error) {
    message.error('删除失败: ' + error.message)
  }
}

async function handleSubmit(formData) {
  try {
    let response
    if (editingShift.value) {
      response = await updateShift(editingShift.value.id, formData)
    } else {
      response = await createShift(formData)
    }

    if (response.success) {
      message.success(editingShift.value ? '更新成功' : '创建成功')
      showForm.value = false
      editingShift.value = null
      await loadShifts()
    }
  } catch (error) {
    message.error('操作失败: ' + error.message)
  }
}

function handleCancel() {
  showForm.value = false
  editingShift.value = null
}

onMounted(() => {
  loadShifts()
})
</script>

<style scoped>
.shift-management-page {
  padding: 16px;
  padding-bottom: 0;
}

.shift-management-page :deep(.ant-page-header) {
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
