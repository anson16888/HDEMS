<template>
  <section class="page">
    <PageHeader
      title="医院信息管理"
      description="维护医院基本信息,包括医院名称、编码、联系方式等。"
      :cta-text="showForm ? '返回列表' : '新增医院'"
      :show-cta="true"
      @cta-click="toggleForm"
    />

    <!-- 医院列表 -->
    <div v-if="!showForm" class="card">
      <Table
        :columns="columns"
        :data-source="hospitals"
        :loading="loading"
        :pagination="pagination"
        row-key="id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'status'">
            <Tag :color="record.status === 1 ? 'green' : 'red'">
              {{ record.status === 1 ? '启用' : '停用' }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'action'">
            <Space>
              <Button type="link" size="small" @click="handleEdit(record)">
                编辑
              </Button>
              <Popconfirm
                title="确定要删除该医院吗?"
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

    <!-- 医院表单 -->
    <div v-else class="card">
      <HospitalForm
        :hospital="editingHospital"
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
import HospitalForm from '../components/forms/HospitalForm.vue'
import {
  getHospitals,
  createHospital,
  updateHospital,
  deleteHospital
} from '../api/basicData.api.js'

const showForm = ref(false)
const loading = ref(false)
const hospitals = ref([])
const editingHospital = ref(null)

const pagination = reactive({
  current: 1,
  pageSize: 20,
  showSizeChanger: true,
  showTotal: (total) => `共 ${total} 条记录`
})

const columns = [
  {
    title: '医院编码',
    dataIndex: 'hospitalCode',
    key: 'hospitalCode',
    width: 150
  },
  {
    title: '医院名称',
    dataIndex: 'hospitalName',
    key: 'hospitalName'
  },
  {
    title: '简称',
    dataIndex: 'shortName',
    key: 'shortName',
    width: 150
  },
  {
    title: '地址',
    dataIndex: 'address',
    key: 'address',
    ellipsis: true
  },
  {
    title: '联系人',
    dataIndex: 'contactPerson',
    key: 'contactPerson',
    width: 120
  },
  {
    title: '联系电话',
    dataIndex: 'contactPhone',
    key: 'contactPhone',
    width: 150
  },
  {
    title: '状态',
    dataIndex: 'status',
    key: 'status',
    width: 100,
    align: 'center'
  },
  {
    title: '排序',
    dataIndex: 'sortOrder',
    key: 'sortOrder',
    width: 80,
    align: 'center'
  },
  {
    title: '操作',
    key: 'action',
    width: 150,
    align: 'center'
  }
]

async function loadHospitals() {
  try {
    loading.value = true
    const response = await getHospitals()
    if (response.success) {
      hospitals.value = response.data || []
    }
  } catch (error) {
    message.error('加载医院列表失败: ' + error.message)
  } finally {
    loading.value = false
  }
}

function toggleForm() {
  showForm.value = !showForm.value
  if (!showForm.value) {
    editingHospital.value = null
  }
}

function handleEdit(record) {
  editingHospital.value = { ...record }
  showForm.value = true
}

async function handleDelete(id) {
  try {
    const response = await deleteHospital(id)
    if (response.success) {
      message.success('删除成功')
      await loadHospitals()
    }
  } catch (error) {
    message.error('删除失败: ' + error.message)
  }
}

async function handleSubmit(formData) {
  try {
    let response
    if (editingHospital.value) {
      response = await updateHospital(editingHospital.value.id, formData)
    } else {
      response = await createHospital(formData)
    }

    if (response.success) {
      message.success(editingHospital.value ? '更新成功' : '创建成功')
      showForm.value = false
      editingHospital.value = null
      await loadHospitals()
    }
  } catch (error) {
    message.error('操作失败: ' + error.message)
  }
}

function handleCancel() {
  showForm.value = false
  editingHospital.value = null
}

onMounted(() => {
  loadHospitals()
})
</script>
