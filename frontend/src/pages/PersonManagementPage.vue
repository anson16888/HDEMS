<template>
  <section class="page">
    <PageHeader
      title="人员信息管理"
      description="维护医院人员基本信息,包括姓名、科室、职级、职称等。"
    />

    <div class="card">
      <!-- 搜索栏 -->
      <div class="search-bar">
        <Space>
          <Input
            v-model:value="searchKeyword"
            placeholder="搜索人员姓名或科室"
            style="width: 300px"
            @pressEnter="handleSearch"
          />
          <Button type="primary" @click="handleSearch">搜索</Button>
          <Button @click="handleReset">重置</Button>
        </Space>
      </div>

      <!-- 人员列表 -->
      <Table
        :columns="columns"
        :data-source="persons"
        :loading="loading"
        :pagination="pagination"
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
    </div>
  </section>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { message, Table, Button, Space, Popconfirm, Tag, Input } from 'ant-design-vue'
import PageHeader from '../components/PageHeader.vue'
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
.search-bar {
  margin-bottom: 16px;
}
</style>
