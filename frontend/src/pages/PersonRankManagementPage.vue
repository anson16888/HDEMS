<template>
  <section class="page">
    <PageHeader
      title="人员职级管理"
      description="查看人员职级信息,按类别分类展示。"
    />

    <div class="content-section">
      <Tabs v-model:activeKey="activeCategory" @change="handleCategoryChange">
        <TabPane key="all" tab="全部职级">
          <PersonRankTable :data="allRanks" :loading="loading" />
        </TabPane>
        <TabPane key="bureau" tab="局级职级">
          <PersonRankTable :data="bureauRanks" :loading="loading" />
        </TabPane>
        <TabPane key="hospital" tab="院级职级">
          <PersonRankTable :data="hospitalRanks" :loading="loading" />
        </TabPane>
        <TabPane key="administrative" tab="行政职级">
          <PersonRankTable :data="administrativeRanks" :loading="loading" />
        </TabPane>
      </Tabs>
    </div>
  </section>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { message, Tabs, TabPane } from 'ant-design-vue'
import PageHeader from '../components/PageHeader.vue'
import PersonRankTable from '../components/tables/PersonRankTable.vue'
import { getPersonRanks } from '../api/basicData.api.js'

const activeCategory = ref('all')
const loading = ref(false)
const allRanks = ref([])

const bureauRanks = computed(() => {
  return allRanks.value.filter(rank => rank.rankCategory === 'bureau')
})

const hospitalRanks = computed(() => {
  return allRanks.value.filter(rank => rank.rankCategory === 'hospital')
})

const administrativeRanks = computed(() => {
  return allRanks.value.filter(rank => rank.rankCategory === 'administrative')
})

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
.content-section {
  margin-top: 20px;
}
</style>
