<template>
  <section class="page">
    <PageHeader
      title="系统管理"
      description="用户、基础数据、阈值配置等系统参数维护。"
      cta-text="创建用户"
    />

    <div class="grid two-col">
      <div class="card">
        <h4>用户与权限</h4>
        <p class="muted">系统管理员与业务管理员权限配置。</p>
        <ul class="list">
          <li>角色分配与账号状态管理</li>
          <li>密码重置与初始化密钥</li>
          <li>登录日志与安全策略</li>
        </ul>
      </div>
      <div class="card">
        <h4>基础数据字典</h4>
        <p class="muted">人员、科室、班次与物资类型维护。</p>
        <ul class="list">
          <li>支持 Excel 导入导出</li>
          <li>编码唯一性校验</li>
          <li>引用关系检查</li>
        </ul>
      </div>
    </div>
  </section>
</template>

<script setup>
import PageHeader from '../components/PageHeader.vue'
</script>
