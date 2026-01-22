<template>
  <section class="page">
    <PageHeader
      title="人员职称管理"
      description="查看人员职称信息,包括教授、副教授、讲师等。"
    />

    <div class="card">
      <Table
        :columns="columns"
        :data-source="titles"
        :loading="loading"
        :pagination="pagination"
        row-key="id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'titleLevel'">
            <Tag :color="getLevelColor(record.titleLevel)">
              {{ record.titleLevel || '未设置' }}
            </Tag>
          </template>
        </template>
      </Table>
    </div>
  </section>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { message, Table, Tag } from 'ant-design-vue'
import PageHeader from '../components/PageHeader.vue'
import { getPersonTitles } from '../api/basicData.api.js'

const loading = ref(false)
const titles = ref([])

const pagination = reactive({
  current: 1,
  pageSize: 20,
  showSizeChanger: true,
  showTotal: (total) => `共 ${total} 条记录`
})

const columns = [
  {
    title: '职称编码',
    dataIndex: 'titleCode',
    key: 'titleCode',
    width: 150
  },
  {
    title: '职称名称',
    dataIndex: 'titleName',
    key: 'titleName'
  },
  {
    title: '职称级别',
    dataIndex: 'titleLevel',
    key: 'titleLevel',
    width: 150
  },
  {
    title: '排序号',
    dataIndex: 'sortOrder',
    key: 'sortOrder',
    width: 100,
    align: 'center'
  }
]

async function loadTitles() {
  try {
    loading.value = true
    const response = await getPersonTitles()
    if (response.success) {
      titles.value = response.data || []
    }
  } catch (error) {
    message.error('加载职称列表失败: ' + error.message)
  } finally {
    loading.value = false
  }
}

function getLevelColor(level) {
  const colors = {
    '正高': 'red',
    '副高': 'orange',
    '中级': 'blue',
    '初级': 'green'
  }
  return colors[level] || 'default'
}

onMounted(() => {
  loadTitles()
})
</script>
