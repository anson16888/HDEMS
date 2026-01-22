<template>
  <section class="page">
    <PageHeader
      title="院内主任排班"
      description="管理医院内专家值班与多科室协同安排。"
      cta-text="新增专家排班"
    />

    <div class="card data-card">
      <div class="card-header">
        <div>
          <h4>重点科室值班</h4>
          <p>急诊、创伤、卒中等科室专家排班。</p>
        </div>
        <span class="badge warning">本周 68 人次</span>
      </div>
      <div class="grid three-col">
        <div class="card sub-card">
          <h5>急诊科</h5>
          <p>主任医师 6 人 · 副主任 4 人</p>
        </div>
        <div class="card sub-card">
          <h5>创伤中心</h5>
          <p>主任医师 4 人 · 副主任 3 人</p>
        </div>
        <div class="card sub-card">
          <h5>卒中中心</h5>
          <p>主任医师 3 人 · 副主任 2 人</p>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import PageHeader from '../components/PageHeader.vue'
</script>
