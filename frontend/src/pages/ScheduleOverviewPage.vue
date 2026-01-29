<template>
  <div class="schedule-overview-page">
    <!-- Page Header -->
    <a-page-header title="排班一览表" sub-title="汇总展示全区排班数据，支持按医院、类型、日期筛选" />

    <!-- Statistics Cards -->
    <a-row :gutter="16" class="stats-row">
      <a-col :span="6">
        <a-card class="stat-card">
          <a-statistic
            title="总排班人数"
            :value="statistics.totalCount || 0"
            :loading="statisticsLoading"
          >
            <template #suffix>
              <span class="stat-meta">总排班人数</span>
            </template>
          </a-statistic>
        </a-card>
      </a-col>
      <a-col :span="6">
        <a-card class="stat-card">
          <a-statistic
            title="局级行政排班"
            :value="statistics.bureauCount || 0"
            :value-style="{ color: '#1890ff' }"
            :loading="statisticsLoading"
          >
            <template #suffix>
              <span class="stat-meta">排班人数</span>
            </template>
          </a-statistic>
        </a-card>
      </a-col>
      <a-col :span="6">
        <a-card class="stat-card">
          <a-statistic
            title="院内行政排班"
            :value="statistics.hospitalCount || 0"
            :value-style="{ color: '#52c41a' }"
            :loading="statisticsLoading"
          >
            <template #suffix>
              <span class="stat-meta">排班人数</span>
            </template>
          </a-statistic>
        </a-card>
      </a-col>
      <a-col :span="6">
        <a-card class="stat-card">
          <a-statistic
            title="院内专家排班"
            :value="statistics.directorCount || 0"
            :value-style="{ color: '#faad14' }"
            :loading="statisticsLoading"
          >
            <template #suffix>
              <span class="stat-meta">排班人数</span>
            </template>
          </a-statistic>
        </a-card>
      </a-col>
    </a-row>

    <!-- Search and Action Card -->
    <a-card class="search-card" :bordered="false">
      <a-form layout="inline">
        <a-row :gutter="16" style="width: 100%">
          <a-col :span="7">
            <a-form-item label="日期范围">
              <a-range-picker
                v-model:value="dateRange"
                format="YYYY-MM-DD"
                :placeholder="['开始日期', '结束日期']"
                style="width: 100%"
                @change="handleDateRangeChange"
              />
            </a-form-item>
          </a-col>
          <a-col :span="4">
            <a-form-item label="排班类型">
              <a-select
                v-model:value="filters.scheduleType"
                placeholder="选择类型"
                allow-clear
                style="width: 100%"
                @change="handleSearch"
              >
                <a-select-option
                  v-for="type in scheduleTypeOptions"
                  :key="type.value"
                  :value="type.value"
                >
                  {{ type.label }}
                </a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :span="4" v-if="isSystemAdmin">
            <a-form-item label="医院">
              <a-select
                v-model:value="filters.hospitalId"
                placeholder="选择医院"
                allow-clear
                style="width: 100%"
                @change="handleSearch"
              >
                <a-select-option value="">全部</a-select-option>
                <a-select-option
                  v-for="hospital in hospitals"
                  :key="hospital.id"
                  :value="hospital.id"
                >
                  {{ hospital.hospitalName }}
                </a-select-option>
              </a-select>
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
          <a-col :span="5">
            <a-form-item :wrapper-col="{ span: 24 }">
              <a-space>
                <a-button type="primary" @click="handleSearch">
                  <template #icon><SearchOutlined /></template>
                  查询
                </a-button>
                <a-button @click="handleResetFilters">重置</a-button>
                <a-button @click="handleExport">
                  <template #icon><DownloadOutlined /></template>
                  导出报表
                </a-button>
              </a-space>
            </a-form-item>
          </a-col>
        </a-row>
      </a-form>
    </a-card>

    <!-- Schedule Overview Table -->
    <a-card :bordered="false" class="table-card">
      <a-spin :spinning="loading">
        <a-empty
          v-if="overviewData.length === 0 && !loading"
          :description="hasActiveFilters ? '没有找到匹配的排班记录' : '暂无排班数据'"
        />

        <a-table
          v-else
          :columns="columns"
          :data-source="overviewData"
          :pagination="false"
          :row-key="record => record.id"
          :scroll="{ x: 1400, y: 'calc(100vh - 550px)' }"
        >
          <!-- 排班日期 -->
          <template #bodyCell="{ column, record }">
            <template v-if="column.key === 'scheduleDate'">
              <span>{{ formatDate(record.scheduleDate) }}</span>
            </template>

            <!-- 排班类型 -->
            <template v-else-if="column.key === 'scheduleTypeName'">
              <a-tag :color="getScheduleTypeColor(record.scheduleType)">
                {{ getScheduleTypeLabel(record.scheduleType) }}
              </a-tag>
            </template>

            <!-- 班次 -->
            <template v-else-if="column.key === 'shiftName'">
              <a-tag color="blue">{{ record.shiftName || '-' }}</a-tag>
            </template>

            <!-- 人员信息 -->
            <template v-else-if="column.key === 'personInfo'">
              <div class="person-info">
                <div class="person-name">{{ record.personName || '-' }}</div>
                <div class="person-details" v-if="record.rankName || record.titleName">
                  <a-tag size="small" v-if="record.rankName">{{ record.rankName }}</a-tag>
                  <a-tag size="small" v-if="record.titleName">{{ record.titleName }}</a-tag>
                </div>
              </div>
            </template>

            <!-- 联系电话 -->
            <template v-else-if="column.key === 'phone'">
              <span v-if="record.phone">
                <PhoneOutlined /> {{ record.phone }}
              </span>
              <span v-else>-</span>
            </template>

            <!-- 备注 -->
            <template v-else-if="column.key === 'remark'">
              <a-tooltip v-if="record.remark" :title="record.remark">
                <span class="remarks-text">{{ truncateText(record.remark, 20) }}</span>
              </a-tooltip>
              <span v-else>-</span>
            </template>
          </template>
        </a-table>

        <!-- Pagination -->
        <div v-if="overviewData.length > 0" class="pagination-wrapper">
          <a-pagination
            v-model:current="pagination.current"
            v-model:page-size="pagination.pageSize"
            :page-size-options="['10', '20', '50', '100']"
            :total="pagination.total"
            :show-total="(total) => `共 ${total} 条`"
            show-size-changer
            show-quick-jumper
            @change="handleTableChange"
          />
        </div>
      </a-spin>
    </a-card>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import {
  SearchOutlined,
  DownloadOutlined,
  PhoneOutlined
} from '@ant-design/icons-vue'
import {
  getScheduleOverview,
  getScheduleStatistics,
  exportSchedules
} from '../api/schedule.api.js'
import { getDepartments, getHospitals } from '../api/basicData.api.js'
import { SCHEDULE_TYPE } from '../config/api.config.js'
import { useAuthStore } from '@/stores/auth.store'
import dayjs from 'dayjs'

// 数据状态
const loading = ref(false)
const statisticsLoading = ref(false)
const overviewData = ref([])
const statistics = ref({
  totalCount: 0,
  bureauCount: 0,
  hospitalCount: 0,
  directorCount: 0
})

// Auth
const authStore = useAuthStore()
const isSystemAdmin = computed(() => authStore.isSystemAdmin)
const user = computed(() => authStore.user)

// 筛选条件
const dateRange = ref([
  dayjs().startOf('week'),
  dayjs().endOf('week')
])
const filters = ref({
  startDate: undefined,
  endDate: undefined,
  scheduleType: undefined,
  departmentId: undefined,
  hospitalId: undefined
})

// 分页
const pagination = ref({
  current: 1,
  pageSize: 20,
  total: 0
})

// 基础数据
const departments = ref([])
const hospitals = ref([])

// 排班类型选项
const scheduleTypeOptions = ref([
  { value: SCHEDULE_TYPE.BUREAU, label: '局级排班' },
  { value: SCHEDULE_TYPE.HOSPITAL, label: '院内行政排班' },
  { value: SCHEDULE_TYPE.DIRECTOR, label: '院内主任排班' }
])

// 表格列配置
const baseColumns = [
  {
    title: '排班日期',
    dataIndex: 'scheduleDate',
    key: 'scheduleDate',
    width: 120,
    fixed: 'left'
  },
  {
    title: '排班类型',
    dataIndex: 'scheduleTypeName',
    key: 'scheduleTypeName',
    width: 120
  },
  {
    title: '科室',
    dataIndex: 'departmentName',
    key: 'departmentName',
    width: 120
  },
  {
    title: '班次',
    dataIndex: 'shiftName',
    key: 'shiftName',
    width: 100
  },
  {
    title: '值班人员',
    key: 'personInfo',
    width: 180
  },
  {
    title: '联系电话',
    key: 'phone',
    width: 140
  },
  {
    title: '备注',
    dataIndex: 'remark',
    key: 'remark',
    width: 150
  }
]

const columns = computed(() => {
  const hospitalColumn = {
    title: '医院',
    dataIndex: 'hospitalName',
    key: 'hospitalName',
    width: 150
  }
  // 插入到排班类型之后
  return [
    ...baseColumns.slice(0, 2),
    hospitalColumn,
    ...baseColumns.slice(2)
  ]
})

/**
 * 判断是否有激活的筛选条件
 */
const hasActiveFilters = computed(() => {
  return filters.value.scheduleType !== undefined ||
         filters.value.departmentId !== undefined ||
         filters.value.hospitalId !== undefined
})

/**
 * 格式化日期
 */
function formatDate(date) {
  return dayjs(date).format('YYYY-MM-DD')
}

/**
 * 截断文本
 */
function truncateText(text, maxLength) {
  if (!text) return ''
  return text.length > maxLength ? text.substring(0, maxLength) + '...' : text
}

/**
 * 获取排班类型标签
 */
function getScheduleTypeLabel(type) {
  const labels = {
    [SCHEDULE_TYPE.BUREAU]: '局级排班',
    [SCHEDULE_TYPE.HOSPITAL]: '院内行政排班',
    [SCHEDULE_TYPE.DIRECTOR]: '院内主任排班'
  }
  return labels[type] || '未知类型'
}

/**
 * 获取排班类型颜色
 */
function getScheduleTypeColor(type) {
  const colors = {
    [SCHEDULE_TYPE.BUREAU]: 'blue',
    [SCHEDULE_TYPE.HOSPITAL]: 'green',
    [SCHEDULE_TYPE.DIRECTOR]: 'orange'
  }
  return colors[type] || 'default'
}

/**
 * 日期范围变化
 */
function handleDateRangeChange(dates) {
  if (dates && dates.length === 2 && dates[0] && dates[1]) {
    filters.value.startDate = dates[0].format('YYYY-MM-DD')
    filters.value.endDate = dates[1].format('YYYY-MM-DD')
  } else {
    filters.value.startDate = undefined
    filters.value.endDate = undefined
  }
}

/**
 * 加载统计数据
 */
async function loadStatistics() {
  statisticsLoading.value = true
  try {
    // 非管理员用户：使用用户的 hospitalId
    let queryHospitalId = filters.value.hospitalId
    if (!queryHospitalId && !isSystemAdmin.value && authStore.user?.hospitalId) {
      queryHospitalId = authStore.user.hospitalId
    }

    const params = {
      startDate: filters.value.startDate,
      endDate: filters.value.endDate
    }
    // 只有当 hospitalId 有值时才添加
    if (queryHospitalId) {
      params.hospitalId = queryHospitalId
    }

    const response = await getScheduleStatistics(params)
    if (response.success && response.data) {
      statistics.value = response.data
    }
  } catch (error) {
    console.error('加载统计数据失败:', error)
    // 失败时不显示错误消息，使用默认值
  } finally {
    statisticsLoading.value = false
  }
}

/**
 * 加载概览数据
 */
async function loadOverviewData() {
  loading.value = true
  try {
    // 非管理员用户：使用用户的 hospitalId
    let queryHospitalId = filters.value.hospitalId
    if (!queryHospitalId && !isSystemAdmin.value && authStore.user?.hospitalId) {
      queryHospitalId = authStore.user.hospitalId
    }

    const params = {
      ...filters.value,
      page: pagination.value.current,
      pageSize: pagination.value.pageSize
    }
    // 只有当 hospitalId 有值时才添加
    if (queryHospitalId) {
      params.hospitalId = queryHospitalId
    }

    const response = await getScheduleOverview(params)
    if (response.success && response.data) {
      overviewData.value = response.data.items || []
      pagination.value.total = response.data.total || 0
    } else {
      overviewData.value = []
      pagination.value.total = 0
    }
  } catch (error) {
    console.error('加载概览数据失败:', error)
    message.error('加载数据失败：' + (error.message || '未知错误'))
    overviewData.value = []
    pagination.value.total = 0
  } finally {
    loading.value = false
  }
}

/**
 * 搜索
 */
async function handleSearch() {
  pagination.value.current = 1
  await Promise.all([
    loadOverviewData(),
    loadStatistics()
  ])
}

/**
 * 重置筛选条件
 */
async function handleResetFilters() {
  const defaultDateRange = [
    dayjs().startOf('week'),
    dayjs().endOf('week')
  ]

  dateRange.value = defaultDateRange
  filters.value = {
    startDate: defaultDateRange[0].format('YYYY-MM-DD'),
    endDate: defaultDateRange[1].format('YYYY-MM-DD'),
    scheduleType: undefined,
    departmentId: undefined,
    hospitalId: undefined
  }

  pagination.value.current = 1

  await Promise.all([
    loadOverviewData(),
    loadStatistics()
  ])

  message.success('筛选条件已重置')
}

/**
 * 导出报表
 */
async function handleExport() {
  try {
    const params = {
      startDate: filters.value.startDate,
      endDate: filters.value.endDate,
      scheduleType: filters.value.scheduleType,
      departmentId: filters.value.departmentId,
      hospitalId: filters.value.hospitalId,
      shiftId: filters.value.shiftId,
      keyword: ''
    }

    const blob = await exportSchedules(params)

    // 创建下载链接
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `排班报表_${dayjs().format('YYYYMMDD_HHmmss')}.xlsx`
    link.click()
    window.URL.revokeObjectURL(url)

    message.success('导出成功')
  } catch (error) {
    message.error('导出失败：' + (error.message || '未知错误'))
  }
}

/**
 * 表格变化（分页）
 */
async function handleTableChange(page, pageSize) {
  pagination.value.current = page
  pagination.value.pageSize = pageSize
  await loadOverviewData()
}

/**
 * 加载科室列表
 */
async function loadDepartments() {
  try {
    const response = await getDepartments()
    if (response.success && response.data) {
      departments.value = response.data
    }
  } catch (error) {
    console.error('加载科室列表失败:', error)
  }
}

/**
 * 加载医院列表
 */
async function loadHospitals() {
  try {
    const response = await getHospitals()
    if (response.success) {
      hospitals.value = response.data || []
    }
  } catch (error) {
    console.error('加载医院列表失败:', error)
  }
}

// 生命周期
onMounted(async () => {
  // 初始化日期范围（将dayjs对象转换为字符串）
  if (dateRange.value && dateRange.value.length === 2) {
    filters.value.startDate = dateRange.value[0].format('YYYY-MM-DD')
    filters.value.endDate = dateRange.value[1].format('YYYY-MM-DD')
  }

  // 并行加载数据
  const loadTasks = [
    loadDepartments(),
    loadOverviewData(),
    loadStatistics()
  ]

  // 仅管理员加载医院列表
  if (isSystemAdmin.value) {
    loadTasks.push(loadHospitals())
  }

  await Promise.all(loadTasks)
})
</script>

<style scoped>
.schedule-overview-page {
  padding: 0 16px;
}

.stats-row {
  margin-bottom: 16px;
}

.stat-card {
  text-align: center;
}

.stat-meta {
  font-size: 12px;
  color: #999;
  margin-left: 8px;
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

.person-info {
  line-height: 1.5;
}

.person-name {
  font-weight: 500;
  margin-bottom: 4px;
}

.person-details {
  font-size: 12px;
}

.person-details .ant-tag {
  margin-right: 4px;
  margin-bottom: 2px;
}

.contact-info {
  font-size: 13px;
  line-height: 1.5;
}

.remarks-text {
  color: #666;
}
</style>
