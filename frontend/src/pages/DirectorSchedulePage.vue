<template>
  <div class="director-schedule-page">
    <!-- Page Header -->
    <a-page-header title="院内主任排班" sub-title="管理医院内主任级别的排班信息，支持增删改查及Excel导入导出" />

    <!-- Search and Action Card -->
    <a-card class="search-card" :bordered="false">
      <a-form layout="inline">
        <a-row :gutter="16" style="width: 100%">
          <a-col :span="8">
            <a-form-item label="日期">
              <a-range-picker
                v-model:value="dateRange"
                format="YYYY-MM-DD"
                :placeholder="['开始日期', '结束日期']"
                @change="handleDateRangeChange"
              />
            </a-form-item>
          </a-col>
          <a-col :span="4">
            <a-form-item label="科室">
              <a-select
                v-model:value="filters.departmentId"
                placeholder="选择科室"
                allow-clear
                style="width: 100%"
                @change="handleSearch"
              >
                <a-select-option
                  v-for="dept in departments"
                  :key="dept.id"
                  :value="dept.id"
                >
                  {{ dept.departmentName }}
                </a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :span="4">
            <a-form-item label="班次">
              <a-select
                v-model:value="filters.shiftId"
                placeholder="选择班次"
                allow-clear
                style="width: 100%"
                @change="handleSearch"
              >
                <a-select-option
                  v-for="shift in shifts"
                  :key="shift.id"
                  :value="shift.id"
                >
                  {{ shift.shiftName }}
                </a-select-option>
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
                  新增排班
                </a-button>
              </a-space>
            </a-form-item>
          </a-col>
        </a-row>
      </a-form>
    </a-card>

    <!-- Schedules Table -->
    <a-card :bordered="false" class="table-card">
      <a-spin :spinning="loading">
        <a-empty
          v-if="schedules.length === 0 && !loading"
          :description="hasActiveFilters ? '没有找到匹配的排班记录' : '暂无排班数据'"
        >
          <a-button type="primary" v-if="!hasActiveFilters" @click="handleCreate">
            创建第一个排班
          </a-button>
        </a-empty>

        <a-table
          v-else
          :columns="columns"
          :data-source="schedules"
          :pagination="false"
          :row-key="record => record.id"
          :scroll="{ x: 1200, y: 'calc(100vh - 350px)' }"
        >
          <template #bodyCell="{ column, record }">
            <!-- 日期 -->
            <template v-if="column.key === 'scheduleDate'">
              {{ formatDate(record.scheduleDate) }}
            </template>

            <!-- 班次 -->
            <template v-else-if="column.key === 'shiftName'">
              <a-tag color="blue">{{ record.shiftName }}</a-tag>
            </template>

            <!-- 人员 -->
            <template v-else-if="column.key === 'personName'">
              {{ record.personName || '-' }}
            </template>

            <!-- 电话 -->
            <template v-else-if="column.key === 'phone'">
              {{ record.phone || '-' }}
            </template>

            <!-- 职级 -->
            <template v-else-if="column.key === 'rankName'">
              <a-tag color="green">{{ record.rankName || '-' }}</a-tag>
            </template>

            <!-- 科室 -->
            <template v-else-if="column.key === 'departmentName'">
              {{ record.departmentName || '-' }}
            </template>

            <!-- 职称 -->
            <template v-else-if="column.key === 'titleName'">
              {{ record.titleName || '-' }}
            </template>

            <!-- 备注 -->
            <template v-else-if="column.key === 'remark'">
              <a-tooltip v-if="record.remark" :title="record.remark">
                <span class="text-truncate">{{ record.remark }}</span>
              </a-tooltip>
              <span v-else>-</span>
            </template>

            <!-- 操作列 -->
            <template v-else-if="column.key === 'action'">
              <a-space>
                <a-tooltip title="编辑">
                  <a-button type="text" size="small" @click="handleEdit(record)">
                    <template #icon><EditOutlined /></template>
                  </a-button>
                </a-tooltip>

                <a-popconfirm
                  title="确定要删除这条排班记录吗？此操作不可撤销。"
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
        <div v-if="schedules.length > 0" class="pagination-wrapper">
          <a-pagination
            v-model:current="pagination.current"
            v-model:page-size="pagination.pageSize"
            :page-size-options="['10', '20', '50', '100']"
            :total="pagination.total"
            :show-total="(total) => `共 ${total} 条记录`"
            show-size-changer
            show-quick-jumper
            @change="handleTableChange"
          />
        </div>
      </a-spin>
    </a-card>

    <!-- 排班表单弹窗 -->
    <a-modal
      v-model:open="formModalVisible"
      :title="formMode === 'create' ? '新增排班' : '编辑排班'"
      :width="800"
      :confirm-loading="formSubmitting"
      ok-text="保存"
      cancel-text="取消"
      @ok="handleFormSubmit"
      @cancel="handleFormCancel"
    >
      <a-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        :label-col="{ span: 6 }"
        :wrapper-col="{ span: 16 }"
      >
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="排班日期" name="scheduleDate">
              <a-date-picker
                v-model:value="formData.scheduleDate"
                format="YYYY-MM-DD"
                style="width: 100%"
                placeholder="请选择日期"
              />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="科室" name="departmentId">
              <a-select
                v-model:value="formData.departmentId"
                placeholder="请选择科室"
                allow-clear
              >
                <a-select-option
                  v-for="dept in departments"
                  :key="dept.id"
                  :value="dept.id"
                >
                  {{ dept.departmentName }}
                </a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
        </a-row>

        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="班次" name="shiftId">
              <a-select v-model:value="formData.shiftId" placeholder="请选择班次">
                <a-select-option
                  v-for="shift in shifts"
                  :key="shift.id"
                  :value="shift.id"
                >
                  {{ shift.shiftName }} ({{ shift.timeRange }})
                </a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
        </a-row>

        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="人员姓名" name="personName">
              <a-input
                v-model:value="formData.personName"
                placeholder="请输入人员姓名"
              />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="联系电话" name="phone">
              <a-input
                v-model:value="formData.phone"
                placeholder="请输入联系电话"
              />
            </a-form-item>
          </a-col>
        </a-row>

        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="职级" name="rankId">
              <a-select v-model:value="formData.rankId" placeholder="请选择职级">
                <a-select-option
                  v-for="rank in personRanks"
                  :key="rank.id"
                  :value="rank.id"
                >
                  {{ rank.rankName }}
                </a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="职称" name="titleId">
              <a-select v-model:value="formData.titleId" placeholder="请选择职称" allow-clear>
                <a-select-option
                  v-for="title in personTitles"
                  :key="title.id"
                  :value="title.id"
                >
                  {{ title.titleName }}
                </a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
        </a-row>

        <a-row :gutter="16">
          <a-col :span="24">
            <a-form-item label="备注" name="remark" :label-col="{ span: 3 }" :wrapper-col="{ span: 21 }">
              <a-textarea
                v-model:value="formData.remark"
                :rows="3"
                placeholder="请输入备注信息（选填）"
              />
            </a-form-item>
          </a-col>
        </a-row>
      </a-form>
    </a-modal>

    <!-- Excel导入弹窗 -->
    <ScheduleImportModal
      v-model:visible="importModalVisible"
      :schedule-type="SCHEDULE_TYPE.DIRECTOR"
      @success="handleImportSuccess"
    />
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import {
  SearchOutlined,
  ReloadOutlined,
  PlusOutlined,
  UploadOutlined,
  DownloadOutlined,
  EditOutlined,
  DeleteOutlined
} from '@ant-design/icons-vue'
import dayjs from 'dayjs'
import * as scheduleApi from '../api/schedule.api.js'
import * as basicDataApi from '../api/basicData.api.js'
import { SCHEDULE_TYPE } from '../config/api.config.js'
import ScheduleImportModal from '../components/modals/ScheduleImportModal.vue'

// ==================== Data ====================

const loading = ref(false)
const schedules = ref([])
const departments = ref([])
const shifts = ref([])
const personRanks = ref([])
const personTitles = ref([])
const dateRange = ref([])

const filters = reactive({
  departmentId: undefined,
  shiftId: undefined,
  startDate: undefined,
  endDate: undefined
})

const pagination = reactive({
  current: 1,
  pageSize: 20,
  total: 0
})

// Form
const formModalVisible = ref(false)
const formMode = ref('create')
const formSubmitting = ref(false)
const formRef = ref()
const formData = reactive({
  scheduleDate: undefined,
  departmentId: undefined,
  shiftId: undefined,
  rankId: undefined,
  personName: '',
  phone: '',
  titleId: undefined,
  remark: ''
})

const formRules = {
  scheduleDate: [{ required: true, message: '请选择排班日期', trigger: 'change' }],
  shiftId: [{ required: true, message: '请选择班次', trigger: 'change' }],
  personName: [{ required: true, message: '请输入人员姓名', trigger: 'blur' }],
  phone: [{ required: true, message: '请输入联系电话', trigger: 'blur' }]
}

// Import
const importModalVisible = ref(false)

// ==================== Computed ====================

const hasActiveFilters = computed(() => {
  return filters.departmentId ||
         filters.shiftId ||
         filters.startDate ||
         filters.endDate
})

const filteredDepartments = computed(() => {
  return departments.value
})

const columns = [
  { title: '日期', dataIndex: 'scheduleDate', key: 'scheduleDate', width: 120, fixed: 'left' },
  { title: '班次', dataIndex: 'shiftName', key: 'shiftName', width: 100 },
  { title: '人员', dataIndex: 'personName', key: 'personName', width: 120 },
  { title: '电话', dataIndex: 'phone', key: 'phone', width: 130 },
  { title: '职级', dataIndex: 'rankName', key: 'rankName', width: 120 },
  { title: '科室', dataIndex: 'departmentName', key: 'departmentName', width: 120 },
  { title: '职称', dataIndex: 'titleName', key: 'titleName', width: 100 },
  { title: '备注', dataIndex: 'remark', key: 'remark', width: 150 },
  { title: '操作', key: 'action', width: 120, fixed: 'right' }
]

// ==================== Methods ====================

const formatDate = (date) => {
  return dayjs(date).format('YYYY-MM-DD')
}

const loadSchedules = async () => {
  try {
    loading.value = true
    const params = {
      scheduleType: SCHEDULE_TYPE.DIRECTOR,
      page: pagination.current,
      pageSize: pagination.pageSize,
      ...filters
    }
    const response = await scheduleApi.getSchedules(params)
    if (response.success && response.data) {
      schedules.value = response.data.items || []
      pagination.total = response.data.total || 0
    }
  } catch (error) {
    message.error('加载排班数据失败')
    console.error(error)
  } finally {
    loading.value = false
  }
}

const loadBasicData = async () => {
  try {
    const [deptRes, shiftRes, rankRes, titleRes] = await Promise.all([
      basicDataApi.getDepartments(),
      basicDataApi.getShifts(),
      basicDataApi.getPersonRanks(), // 加载所有职级数据
      basicDataApi.getPersonTitles()
    ])

    console.log('API Response - Departments:', deptRes)
    console.log('API Response - Shifts:', shiftRes)
    console.log('API Response - PersonRanks:', rankRes)
    console.log('API Response - PersonTitles:', titleRes)

    if (deptRes.success) {
      departments.value = deptRes.data || []
      console.log('Loaded departments:', departments.value.length)
    }
    if (shiftRes.success) {
      shifts.value = shiftRes.data || []
      console.log('Loaded shifts:', shifts.value.length)
    }
    if (rankRes.success) {
      personRanks.value = rankRes.data || []
      console.log('Loaded personRanks:', personRanks.value.length)
      console.log('PersonRanks data:', personRanks.value)
    }
    if (titleRes.success) {
      personTitles.value = titleRes.data || []
      console.log('Loaded personTitles:', personTitles.value.length)
    }
  } catch (error) {
    message.error('加载基础数据失败')
    console.error('loadBasicData error:', error)
  }
}

const handleSearch = () => {
  pagination.current = 1
  loadSchedules()
}

const handleDateRangeChange = (dates) => {
  if (dates && dates.length === 2) {
    filters.startDate = dates[0].format('YYYY-MM-DD')
    filters.endDate = dates[1].format('YYYY-MM-DD')
  } else {
    filters.startDate = undefined
    filters.endDate = undefined
  }
  handleSearch()
}

const handleResetFilters = () => {
  filters.departmentId = undefined
  filters.shiftId = undefined
  filters.startDate = undefined
  filters.endDate = undefined
  dateRange.value = []
  handleSearch()
}

const handleTableChange = (page, pageSize) => {
  pagination.current = page
  pagination.pageSize = pageSize
  loadSchedules()
}

const handleCreate = () => {
  formMode.value = 'create'
  resetForm()
  formModalVisible.value = true
}

const handleEdit = (record) => {
  formMode.value = 'edit'
  Object.assign(formData, {
    id: record.id,
    scheduleDate: record.scheduleDate ? dayjs(record.scheduleDate) : undefined,
    departmentId: record.departmentId,
    shiftId: record.shiftId,
    rankId: record.rankId,
    personName: record.personName || '',
    phone: record.phone || '',
    titleId: record.titleId,
    remark: record.remark || ''
  })
  formModalVisible.value = true
}

const handleDelete = async (record) => {
  try {
    const response = await scheduleApi.deleteSchedule(record.id)
    if (response.success) {
      message.success('删除成功')
      loadSchedules()
    } else {
      message.error(response.message || '删除失败')
    }
  } catch (error) {
    message.error('删除失败')
    console.error(error)
  }
}

const handleFormSubmit = async () => {
  try {
    await formRef.value.validate()
    formSubmitting.value = true

    const data = {
      scheduleDate: formData.scheduleDate ? formData.scheduleDate.toISOString() : undefined,
      scheduleType: SCHEDULE_TYPE.DIRECTOR,
      departmentId: formData.departmentId,
      shiftId: formData.shiftId,
      rankId: formData.rankId,
      personName: formData.personName,
      phone: formData.phone,
      titleId: formData.titleId,
      remark: formData.remark
    }

    const response = formMode.value === 'create'
      ? await scheduleApi.createSchedule(data)
      : await scheduleApi.updateSchedule(formData.id, data)

    if (response.success) {
      message.success(formMode.value === 'create' ? '创建成功' : '更新成功')
      formModalVisible.value = false
      resetForm() // 重置表单
      loadSchedules()
    } else {
      message.error(response.message || '操作失败')
    }
  } catch (error) {
    if (error.errorFields) {
      message.warning('请检查表单填写')
    } else {
      message.error('操作失败')
      console.error(error)
    }
  } finally {
    formSubmitting.value = false
  }
}

const handleFormCancel = () => {
  formModalVisible.value = false
  resetForm()
}

const resetForm = () => {
  Object.assign(formData, {
    scheduleDate: undefined,
    departmentId: undefined,
    shiftId: undefined,
    rankId: undefined,
    personName: '',
    phone: '',
    titleId: undefined,
    remark: ''
  })
  formRef.value?.resetFields()
}

// Import/Export
const handleImport = () => {
  importModalVisible.value = true
}

/**
 * 导入成功回调
 */
const handleImportSuccess = () => {
  importModalVisible.value = false
  // 刷新列表
  loadSchedules()
}

const handleExport = async () => {
  try {
    const data = {
      scheduleType: SCHEDULE_TYPE.DIRECTOR,
      startDate: filters.startDate,
      endDate: filters.endDate,
      departmentId: filters.departmentId
    }

    const blob = await scheduleApi.exportSchedules(data)

    // Create download link
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `院内主任排班_${dayjs().format('YYYYMMDD_HHmmss')}.xlsx`
    link.click()
    window.URL.revokeObjectURL(url)

    message.success('导出成功')
  } catch (error) {
    message.error('导出失败')
    console.error(error)
  }
}

// ==================== Lifecycle ====================

onMounted(() => {
  loadSchedules()
  loadBasicData()
})
</script>

<style scoped>
.director-schedule-page {
  padding: 0 16px;
}

.search-card {
  margin-bottom: 16px;
}

.table-card {
  min-height: 400px;
}

.pagination-wrapper {
  margin-top: 16px;
  display: flex;
  justify-content: flex-end;
}

.text-muted {
  color: #8c8c8c;
  font-size: 12px;
}

.text-truncate {
  display: inline-block;
  max-width: 150px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
</style>
