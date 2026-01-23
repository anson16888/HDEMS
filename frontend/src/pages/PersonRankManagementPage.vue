<template>
  <div class="person-rank-management-page">
    <!-- Page Header -->
    <a-page-header title="人员职级管理" sub-title="查看人员职级信息,按类别分类展示" />

    <!-- Content Card -->
    <a-card class="content-card" :bordered="false">
      <Tabs v-model:activeKey="activeCategory" @change="handleCategoryChange">
        <TabPane key="all" tab="全部职级">
          <Table
            :columns="columns"
            :data-source="currentRanks"
            :loading="loading"
            :pagination="false"
            :scroll="{ y: 'calc(100vh - 370px)' }"
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
        </TabPane>
        <TabPane key="局级" tab="局级职级">
          <Table
            :columns="columns"
            :data-source="currentRanks"
            :loading="loading"
            :pagination="false"
            :scroll="{ y: 'calc(100vh - 370px)' }"
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
        </TabPane>
        <TabPane key="院级" tab="院级职级">
          <Table
            :columns="columns"
            :data-source="currentRanks"
            :loading="loading"
            :pagination="false"
            :scroll="{ y: 'calc(100vh - 370px)' }"
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
        </TabPane>
        <TabPane key="行政" tab="行政职级">
          <Table
            :columns="columns"
            :data-source="currentRanks"
            :loading="loading"
            :pagination="false"
            :scroll="{ y: 'calc(100vh - 370px)' }"
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
        </TabPane>
      </Tabs>
    </a-card>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, reactive } from 'vue'
import { message } from 'ant-design-vue'
import { Tabs, TabPane, Table, Tag } from 'ant-design-vue'
import { getPersonRanks } from '../api/basicData.api.js'

const activeCategory = ref('all')
const loading = ref(false)
const allRanks = ref([])

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

const currentRanks = computed(() => {
  if (activeCategory.value === 'all') {
    return allRanks.value
  }
  return allRanks.value.filter(rank => rank.rankCategory === activeCategory.value)
})

const CATEGORY_NAMES = {
  '局级': '局级',
  '院级': '院级',
  '行政': '行政'
}

const CATEGORY_COLORS = {
  '局级': 'red',
  '院级': 'blue',
  '行政': 'green'
}

function getCategoryName(category) {
  return category || '未分类'
}

function getCategoryColor(category) {
  return CATEGORY_COLORS[category] || 'default'
}

async function loadRanks() {
  try {
    loading.value = true
    const response = await getPersonRanks()
    if (response.success) {
      allRanks.value = response.data || []
    }
  } catch (error) {
    message.error('加载职级列表失败: ' + error.message)
  } finally {
    loading.value = false
  }
}

function handleCategoryChange(category) {
  activeCategory.value = category
}

onMounted(() => {
  loadRanks()
})
</script>

<style scoped>
.person-rank-management-page {
  padding: 0 16px;
}

.content-card {
  margin-bottom: 0;
}
</style>
