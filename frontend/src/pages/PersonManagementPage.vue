<template>
  <div class="person-management-page">
    <!-- Page Header -->
    <a-page-header
      title="人员信息管理"
      sub-title="维护医院人员基本信息,包括姓名、科室、职级、职称等"
    />

    <!-- Search Card -->
    <a-card class="search-card" :bordered="false">
      <a-form layout="inline">
        <a-row :gutter="16" style="width: 100%">
          <a-col :span="8">
            <a-form-item label="关键字">
              <a-input
                v-model:value="searchKeyword"
                placeholder="搜索人员姓名或科室"
                allow-clear
                @pressEnter="handleSearch"
              >
                <template #prefix>
                  <SearchOutlined />
                </template>
              </a-input>
            </a-form-item>
          </a-col>
          <a-col :span="16">
            <a-form-item :wrapper-col="{ span: 24 }">
              <a-space>
                <a-button type="primary" @click="handleSearch">
                  <template #icon><SearchOutlined /></template>
                  查询
                </a-button>
                <a-button @click="handleReset">重置</a-button>
              </a-space>
            </a-form-item>
          </a-col>
        </a-row>
      </a-form>
    </a-card>

    <!-- Person Table Card -->
    <a-card class="table-card" :bordered="false">
      <Table
        :columns="columns"
        :data-source="persons"
        :loading="loading"
        :pagination="pagination"
        :scroll="{ y: 'calc(100vh - 380px)' }"
        row-key="id"
        @change="handleTableChange"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'hospital'">
            <Tag color="blue">{{ record.hospitalName || '-' }}</Tag>
          </template>
          <template v-else-if="column.key === 'department'">
            <Tag color="green">{{ record.departmentName || '-' }}</Tag>
          </template>
          <template v-else-if="column.key === 'action'">
            <Popconfirm
              title="确定要删除该人员吗?"
              ok-text="确定"
              cancel-text="取消"
              @confirm="handleDelete(record.id)"
            >
              <Button type="link" size="small" danger>删除</Button>
            </Popconfirm>
          </template>
        </template>
      </Table>
    </a-card>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import { SearchOutlined } from '@ant-design/icons-vue'
import { Table, Button, Space, Popconfirm, Tag, Input } from 'ant-design-vue'
import { getPersons, deletePerson } from '../api/basicData.api.js'

const loading = ref(false)
const persons = ref([])
const searchKeyword = ref('')

const pagination = reactive({
  current: 1,
  pageSize: 20,
  total: 0,
  showSizeChanger: true,
  showTotal: (total) => `共 ${total} 条记录`
})

const columns = [
  {
    title: '人员编码',
    dataIndex: 'personCode',
    key: 'personCode',
    width: 120
  },
  {
    title: '姓名',
    dataIndex: 'personName',
    key: 'personName',
    width: 120
  },
  {
    title: '所属医院',
    dataIndex: 'hospitalName',
    key: 'hospital',
    width: 150
  },
  {
    title: '科室',
    dataIndex: 'departmentName',
    key: 'department',
    width: 150
  },
  {
    title: '职级',
    dataIndex: 'rankName',
    key: 'rankName',
    width: 120
  },
  {
    title: '职称',
    dataIndex: 'titleName',
    key: 'titleName',
    width: 120
  },
  {
    title: '联系电话',
    dataIndex: 'phone',
    key: 'phone',
    width: 150
  },
  {
    title: '操作',
    key: 'action',
    width: 100,
    align: 'center',
    fixed: 'right'
  }
]

async function loadPersons() {
  try {
    loading.value = true
    const params = {
      page: pagination.current,
      pageSize: pagination.pageSize,
      keyword: searchKeyword.value || undefined
    }
    const response = await getPersons(params)
    if (response.success) {
      persons.value = response.data.items || []
      pagination.total = response.data.total || 0
    }
  } catch (error) {
    message.error('加载人员列表失败: ' + error.message)
  } finally {
    loading.value = false
  }
}

function handleSearch() {
  pagination.current = 1
  loadPersons()
}

function handleReset() {
  searchKeyword.value = ''
  pagination.current = 1
  loadPersons()
}

function handleTableChange(paginationConfig) {
  pagination.current = paginationConfig.current
  pagination.pageSize = paginationConfig.pageSize
  loadPersons()
}

async function handleDelete(id) {
  try {
    const response = await deletePerson(id)
    if (response.success) {
      message.success('删除成功')
      await loadPersons()
    }
  } catch (error) {
    message.error('删除失败: ' + error.message)
  }
}

onMounted(() => {
  loadPersons()
})
</script>

<style scoped>
.person-management-page {
  padding: 16px;
  padding-bottom: 0;
}

.person-management-page :deep(.ant-page-header) {
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
</style>
