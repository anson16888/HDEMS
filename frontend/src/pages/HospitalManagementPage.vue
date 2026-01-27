<template>
  <div class="hospital-management-page">
    <!-- 管理员视图：医院列表 -->
    <template v-if="isSystemAdmin">
      <!-- Page Header -->
      <a-page-header title="医院信息" sub-title="管理系统医院信息" />

      <!-- Search and Action Card -->
      <a-card class="search-card" :bordered="false">
        <a-form layout="inline">
          <a-row :gutter="16" style="width: 100%">
            <a-col :span="8">
              <a-form-item label="关键字">
                <a-input
                  v-model:value="searchKeyword"
                  placeholder="搜索医院名称"
                  allow-clear
                  @change="handleSearch"
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
                  placeholder="选择状态"
                  allow-clear
                  style="width: 100%"
                  @change="handleStatusFilter"
                >
                  <a-select-option :value="undefined">全部</a-select-option>
                  <a-select-option :value="1">启用</a-select-option>
                  <a-select-option :value="0">停用</a-select-option>
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
                  <a-button @click="resetFilters">重置</a-button>
                  <a-button type="primary" @click="openCreateModal">
                    <template #icon><PlusOutlined /></template>
                    新增医院
                  </a-button>
                </a-space>
              </a-form-item>
            </a-col>
          </a-row>
        </a-form>
      </a-card>

      <!-- Hospital Table -->
      <a-card :bordered="false" class="table-card">
        <a-spin :spinning="loading">
          <a-empty
            v-if="filteredHospitals.length === 0 && !loading"
            :description="hasActiveFilters ? '没有找到匹配的医院' : '暂无医院数据'"
          >
            <a-button type="primary" v-if="!hasActiveFilters" @click="openCreateModal">
              创建第一个医院
            </a-button>
          </a-empty>

          <a-table
            v-else
            :columns="columns"
            :data-source="filteredHospitals"
            :pagination="false"
            :row-key="(record) => record.id"
            :scroll="{ x: 1200, y: 'calc(100vh - 400px)' }"
          >
            <!-- 状态 -->
            <template #bodyCell="{ column, record }">
              <template v-if="column.key === 'status'">
                <a-badge
                  :status="record.status === 1 ? 'success' : 'error'"
                  :text="record.status === 1 ? '启用' : '停用'"
                />
              </template>

              <!-- 操作 -->
              <template v-else-if="column.key === 'actions'">
                <a-space>
                  <a-tooltip title="编辑">
                    <a-button type="text" size="small" @click="openEditModal(record)">
                      <template #icon><EditOutlined /></template>
                    </a-button>
                  </a-tooltip>

                  <a-popconfirm
                    title="确定要删除该医院吗？此操作不可撤销。"
                    ok-text="确认"
                    cancel-text="取消"
                    @confirm="handleDeleteHospital(record)"
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
        </a-spin>
      </a-card>

      <!-- Modal -->
      <HospitalFormModal
        v-if="modalState.showForm"
        :mode="modalState.mode"
        :hospital="modalState.hospital"
        @save="handleSaveHospital"
        @cancel="closeModals"
      />
    </template>

    <!-- 非管理员视图：当前医院编辑 -->
    <template v-else>
      <!-- Page Header -->
      <a-page-header title="医院信息" sub-title="编辑当前医院的基本信息" />

      <!-- 空状态：没有医院信息 -->
      <a-card v-if="!formData.id && !loading" :bordered="false" class="form-card">
        <a-empty description="未找到您的医院信息">
          <p class="empty-hint">请联系系统管理员为您分配医院</p>
        </a-empty>
      </a-card>

      <!-- Hospital Edit Form -->
      <a-card v-else :bordered="false" class="form-card">
        <div class="hospital-form">
          <a-spin :spinning="loading">
            <a-form
              :model="formData"
              :rules="rules"
              ref="formRef"
              :label-col="{ span: 4 }"
              :wrapper-col="{ span: 12 }"
            >
              <a-form-item label="医院编码" name="hospitalCode">
                <a-input
                  v-model:value="formData.hospitalCode"
                  placeholder="请输入医院编码"
                  disabled
                />
              </a-form-item>

              <a-form-item label="医院名称" name="hospitalName">
                <a-input
                  v-model:value="formData.hospitalName"
                  placeholder="请输入医院名称"
                />
              </a-form-item>

              <a-form-item label="医院简称" name="shortName">
                <a-input
                  v-model:value="formData.shortName"
                  placeholder="请输入医院简称"
                />
              </a-form-item>

              <a-form-item label="地址" name="address">
                <a-input
                  v-model:value="formData.address"
                  placeholder="请输入医院地址"
                />
              </a-form-item>

              <a-form-item label="联系电话" name="contactPhone">
                <a-input
                  v-model:value="formData.contactPhone"
                  placeholder="请输入联系电话"
                />
              </a-form-item>

              <a-form-item label="联系人" name="contactPerson">
                <a-input
                  v-model:value="formData.contactPerson"
                  placeholder="请输入联系人姓名"
                />
              </a-form-item>

              <a-form-item :wrapper-col="{ span: 12, offset: 4 }">
                <a-space>
                  <a-button type="primary" @click="handleSubmit" :loading="submitting">
                    保存
                  </a-button>
                </a-space>
              </a-form-item>
            </a-form>
          </a-spin>
        </div>
      </a-card>
    </template>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue'
import { message } from 'ant-design-vue'
import { useAuthStore } from '@/stores/auth.store'
import { storeToRefs } from 'pinia'
import {
  getHospitals,
  getHospitalById,
  updateHospital,
  deleteHospital
} from '@/api/basicData.api.js'
import {
  SearchOutlined,
  PlusOutlined,
  EditOutlined,
  DeleteOutlined
} from '@ant-design/icons-vue'

// Components
import HospitalFormModal from '@/components/modals/HospitalFormModal.vue'

const authStore = useAuthStore()
const { isSystemAdmin, user, hospitalConfig } = storeToRefs(authStore)

// State
const hospitals = ref([])
const loading = ref(false)
const submitting = ref(false)
const searchKeyword = ref('')
const statusFilter = ref(undefined)

// 非管理员表单数据
const formData = reactive({
  id: '',
  hospitalCode: '',
  hospitalName: '',
  shortName: '',
  address: '',
  contactPhone: '',
  contactPerson: ''
})

// 非管理员表单规则
const rules = {
  hospitalName: [
    { required: true, message: '请输入医院名称', trigger: 'blur' },
    { min: 2, message: '医院名称至少2个字符', trigger: 'blur' }
  ],
  shortName: [
    { required: true, message: '请输入医院简称', trigger: 'blur' }
  ],
  contactPhone: [
    { pattern: /^[0-9\-+()\s]{6,20}$/, message: '请输入正确的联系电话', trigger: 'blur' }
  ]
}

const formRef = ref()

// Modal state
const modalState = reactive({
  showForm: false,
  mode: 'create',
  hospital: null
})

// 管理员表格列
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
    key: 'hospitalName',
    width: 200
  },
  {
    title: '医院简称',
    dataIndex: 'shortName',
    key: 'shortName',
    width: 150
  },
  {
    title: '地址',
    dataIndex: 'address',
    key: 'address',
    width: 200,
    customRender: ({ text }) => text || '-'
  },
  {
    title: '联系电话',
    dataIndex: 'contactPhone',
    key: 'contactPhone',
    width: 150,
    customRender: ({ text }) => text || '-'
  },
  {
    title: '联系人',
    dataIndex: 'contactPerson',
    key: 'contactPerson',
    width: 120,
    customRender: ({ text }) => text || '-'
  },
  {
    title: '排序',
    dataIndex: 'sortOrder',
    key: 'sortOrder',
    width: 80
  },
  {
    title: '状态',
    key: 'status',
    width: 100
  },
  {
    title: '操作',
    key: 'actions',
    fixed: 'right',
    width: 150,
    align: 'center'
  }
]

// Computed - 过滤后的医院列表（管理员）
const filteredHospitals = computed(() => {
  let result = hospitals.value || []

  if (searchKeyword.value) {
    const keyword = searchKeyword.value.toLowerCase()
    result = result.filter(h =>
      h.hospitalName?.toLowerCase().includes(keyword) ||
      h.shortName?.toLowerCase().includes(keyword) ||
      h.hospitalCode?.toLowerCase().includes(keyword)
    )
  }

  if (statusFilter.value !== undefined) {
    result = result.filter(h => h.status === statusFilter.value)
  }

  return result
})

const hasActiveFilters = computed(() => {
  return searchKeyword.value || statusFilter.value !== undefined
})

// 管理员方法
const handleSearch = () => {
  // 搜索已在 computed 中处理
}

const handleStatusFilter = () => {
  // 过滤已在 computed 中处理
}

const resetFilters = () => {
  searchKeyword.value = ''
  statusFilter.value = undefined
}

const openCreateModal = () => {
  modalState.showForm = true
  modalState.mode = 'create'
  modalState.hospital = null
}

const openEditModal = (hospital) => {
  modalState.showForm = true
  modalState.mode = 'edit'
  modalState.hospital = { ...hospital }
}

const closeModals = () => {
  modalState.showForm = false
  modalState.hospital = null
}

const handleSaveHospital = async () => {
  closeModals()
  await loadHospitals()
}

const handleDeleteHospital = async (hospital) => {
  try {
    await deleteHospital(hospital.id)
    message.success('医院已删除')
    await loadHospitals()
  } catch (error) {
    message.error(error.message || '删除失败')
  }
}

// 非管理员方法
async function loadHospitalInfo() {
  // 优先使用 authStore 中已有的 hospitalConfig
  if (hospitalConfig.value) {
    Object.assign(formData, {
      id: hospitalConfig.value.id,
      hospitalCode: hospitalConfig.value.hospitalCode || '',
      hospitalName: hospitalConfig.value.hospitalName || '',
      shortName: hospitalConfig.value.shortName || '',
      address: hospitalConfig.value.address || '',
      contactPhone: hospitalConfig.value.contactPhone || '',
      contactPerson: hospitalConfig.value.contactPerson || ''
    })
    return
  }

  // 如果 hospitalConfig 不存在，尝试通过 user.hospitalId 获取
  if (!user.value?.hospitalId) {
    // 非管理员没有 hospitalId，保持表单为空（不显示警告）
    return
  }

  try {
    loading.value = true
    const response = await getHospitalById(user.value.hospitalId)
    const hospitalData = response.data
    if (hospitalData) {
      Object.assign(formData, {
        id: hospitalData.id,
        hospitalCode: hospitalData.hospitalCode || '',
        hospitalName: hospitalData.hospitalName || '',
        shortName: hospitalData.shortName || '',
        address: hospitalData.address || '',
        contactPhone: hospitalData.contactPhone || '',
        contactPerson: hospitalData.contactPerson || ''
      })
    }
  } catch (error) {
    message.error('加载医院信息失败: ' + error.message)
  } finally {
    loading.value = false
  }
}

async function handleSubmit() {
  try {
    await formRef.value.validate()
  } catch (error) {
    message.warning('请检查表单输入')
    return
  }

  try {
    submitting.value = true
    const { id, hospitalCode, ...updateData } = formData
    await updateHospital(id, updateData)
    message.success('保存成功')
    await loadHospitalInfo()
    // 重新加载 auth store 中的医院配置
    await authStore.fetchHospitalConfig()
  } catch (error) {
    message.error('保存失败: ' + error.message)
  } finally {
    submitting.value = false
  }
}

// 加载医院列表（管理员）
async function loadHospitals() {
  try {
    loading.value = true
    const response = await getHospitals()
    hospitals.value = response.data || []
  } catch (error) {
    message.error('加载医院列表失败: ' + error.message)
  } finally {
    loading.value = false
  }
}

// Lifecycle
onMounted(async () => {
  if (isSystemAdmin.value) {
    await loadHospitals()
  } else {
    await loadHospitalInfo()
  }
})
</script>

<style scoped>
.hospital-management-page {
  padding: 0 16px;
}

.search-card {
  margin-bottom: 16px;
}

.table-card {
  margin-bottom: 16px;
}

.form-card {
  margin-bottom: 16px;
}

.hospital-form {
  max-width: 800px;
  padding: 20px;
}

.empty-hint {
  color: #999;
  margin-top: 8px;
  font-size: 14px;
}
</style>
