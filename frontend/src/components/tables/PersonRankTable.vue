<template>
  <Table
    :columns="columns"
    :data-source="data"
    :loading="loading"
    :pagination="paginationConfig"
    row-key="id"
  >
    <template #bodyCell="{ column, record }">
      <template v-if="column.key === 'rankCategory'">
        <Tag :color="getCategoryColor(record.rankCategory)">
          {{ getCategoryName(record.rankCategory) }}
        </Tag>
      </template>
    </template>
  </Table>
</template>

<script setup>
import { ref } from 'vue'
import { Table, Tag } from 'ant-design-vue'

const props = defineProps({
  data: {
    type: Array,
    default: () => []
  },
  loading: {
    type: Boolean,
    default: false
  }
})

const columns = [
  {
    title: '职级编码',
    dataIndex: 'rankCode',
    key: 'rankCode',
    width: 150
  },
  {
    title: '职级名称',
    dataIndex: 'rankName',
    key: 'rankName'
  },
  {
    title: '职级类别',
    dataIndex: 'rankCategory',
    key: 'rankCategory',
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

const paginationConfig = ref({
  pageSize: 20,
  showSizeChanger: true,
  showTotal: (total) => `共 ${total} 条记录`
})

const CATEGORY_NAMES = {
  bureau: '局级',
  hospital: '院级',
  administrative: '行政'
}

const CATEGORY_COLORS = {
  bureau: 'red',
  hospital: 'blue',
  administrative: 'green'
}

function getCategoryName(category) {
  return CATEGORY_NAMES[category] || category
}

function getCategoryColor(category) {
  return CATEGORY_COLORS[category] || 'default'
}
</script>
