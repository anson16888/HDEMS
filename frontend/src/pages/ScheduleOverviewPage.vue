<template>
  <section class="page">
    <PageHeader
      title="排班一览表"
      description="汇总展示全区排班数据，支持按医院、类型、日期筛选。"
      cta-text="导出报表"
    />

    <div class="grid stats-grid">
      <div class="card stat-card">
        <p class="stat-label">总排班人数</p>
        <h3>248</h3>
        <span class="stat-meta">本周统计</span>
      </div>
      <div class="card stat-card">
        <p class="stat-label">局级排班</p>
        <h3>56</h3>
        <span class="stat-meta">卫健委端录入</span>
      </div>
      <div class="card stat-card">
        <p class="stat-label">院内主任排班</p>
        <h3>68</h3>
        <span class="stat-meta">院内专家值班</span>
      </div>
    </div>

    <div class="card data-card">
      <div class="card-header">
        <div>
          <h4>排班分布</h4>
          <p>快速查看各医院排班密度。</p>
        </div>
        <div class="chip-group">
          <span class="chip">全部医院</span>
          <span class="chip">行政值班</span>
        </div>
      </div>
      <div class="table-placeholder">
        <div class="table-row header">
          <span>日期</span>
          <span>医院</span>
          <span>类型</span>
          <span>班次</span>
          <span>人员</span>
        </div>
        <div class="table-row">
          <span>2026-01-22</span>
          <span>宝安人民医院</span>
          <span>院级行政</span>
          <span>一线</span>
          <span>张三</span>
        </div>
        <div class="table-row">
          <span>2026-01-22</span>
          <span>宝安人民医院</span>
          <span>主任排班</span>
          <span>上午</span>
          <span>李四</span>
        </div>
        <div class="table-row">
          <span>2026-01-22</span>
          <span>卫健委</span>
          <span>局级行政</span>
          <span>全天</span>
          <span>王五</span>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import PageHeader from '../components/PageHeader.vue'
</script>
