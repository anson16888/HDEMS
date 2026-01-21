<template>
  <section class="page">
    <PageHeader
      title="局级行政排班"
      description="管理卫健委端局级行政值班安排。"
      cta-text="新增排班"
    />

    <div class="card data-card">
      <div class="card-header">
        <div>
          <h4>本周排班概览</h4>
          <p>覆盖卫健委领导与主任岗位。</p>
        </div>
        <span class="badge info">本周 56 人次</span>
      </div>
      <div class="timeline">
        <div class="timeline-item">
          <span class="timeline-date">周一</span>
          <div>
            <p>全天值班：张三</p>
            <span>13800138000 · 值班领导</span>
          </div>
        </div>
        <div class="timeline-item">
          <span class="timeline-date">周二</span>
          <div>
            <p>全天值班：李四</p>
            <span>13800138001 · 值班主任</span>
          </div>
        </div>
        <div class="timeline-item">
          <span class="timeline-date">周三</span>
          <div>
            <p>全天值班：王五</p>
            <span>13800138002 · 值班领导</span>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import PageHeader from '../components/PageHeader.vue'
</script>
