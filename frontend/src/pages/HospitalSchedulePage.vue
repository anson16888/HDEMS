<template>
  <section class="page">
    <PageHeader
      title="院级行政排班"
      description="管理医院端行政值班安排，支持按院区筛选。"
      cta-text="导入排班"
    />

    <div class="grid two-col">
      <div class="card">
        <h4>排班完成度</h4>
        <p class="muted">本月已录入 92%，尚有 3 天待补录。</p>
        <div class="progress">
          <span class="progress-bar" style="width: 92%"></span>
        </div>
      </div>
      <div class="card">
        <h4>院区覆盖</h4>
        <p class="muted">11 家医院已完成排班上传。</p>
        <div class="pill-list">
          <span class="pill">宝安人民医院</span>
          <span class="pill">石岩人民医院</span>
          <span class="pill">松岗人民医院</span>
        </div>
      </div>
    </div>

    <div class="card data-card">
      <div class="card-header">
        <div>
          <h4>今日排班</h4>
          <p>一线/二线班次实时同步。</p>
        </div>
        <span class="badge success">今日 24 人</span>
      </div>
      <div class="table-placeholder">
        <div class="table-row header">
          <span>医院</span>
          <span>班次</span>
          <span>人员</span>
          <span>角色</span>
          <span>电话</span>
        </div>
        <div class="table-row">
          <span>宝安人民医院</span>
          <span>一线</span>
          <span>张三</span>
          <span>值班领导</span>
          <span>13800138000</span>
        </div>
        <div class="table-row">
          <span>石岩人民医院</span>
          <span>二线</span>
          <span>李四</span>
          <span>值班主任</span>
          <span>13800138001</span>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import PageHeader from '../components/PageHeader.vue'
</script>
