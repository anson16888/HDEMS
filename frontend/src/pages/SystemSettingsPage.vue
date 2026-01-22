<template>
  <section class="page">
    <PageHeader
      title="系统管理"
      description="用户、基础数据、阈值配置等系统参数维护。"
      cta-text="创建用户"
    />

    <div class="grid two-col">
      <div class="card clickable" @click="goToUsers">
        <h4>用户与权限</h4>
        <p class="muted">系统管理员与业务管理员权限配置。</p>
        <ul class="list">
          <li>角色分配与账号状态管理</li>
          <li>密码重置与初始化密钥</li>
          <li>登录日志与安全策略</li>
        </ul>
        <div class="arrow">→</div>
      </div>
      <div class="card clickable" @click="goToDictionary">
        <h4>基础数据字典</h4>
        <p class="muted">人员、科室、班次与物资类型维护。</p>
        <ul class="list">
          <li>医院信息管理</li>
          <li>科室信息管理</li>
          <li>班次信息管理</li>
          <li>人员职级/职称/信息管理</li>
        </ul>
        <div class="arrow">→</div>
      </div>
    </div>

    <div class="card" style="margin-top: 20px;">
      <h4>快速导航</h4>
      <div class="quick-links">
        <RouterLink to="/dictionary/hospitals" class="link-item">
          <div class="link-icon">🏥</div>
          <div class="link-content">
            <div class="link-title">医院信息</div>
            <div class="link-desc">管理医院基本信息</div>
          </div>
        </RouterLink>
        <RouterLink to="/dictionary/departments" class="link-item">
          <div class="link-icon">🏢</div>
          <div class="link-content">
            <div class="link-title">科室信息</div>
            <div class="link-desc">管理科室分类与信息</div>
          </div>
        </RouterLink>
        <RouterLink to="/dictionary/shifts" class="link-item">
          <div class="link-icon">⏰</div>
          <div class="link-content">
            <div class="link-title">班次信息</div>
            <div class="link-desc">管理值班班次</div>
          </div>
        </RouterLink>
        <RouterLink to="/dictionary/person-ranks" class="link-item">
          <div class="link-icon">👤</div>
          <div class="link-content">
            <div class="link-title">人员职级</div>
            <div class="link-desc">查看人员职级分类</div>
          </div>
        </RouterLink>
        <RouterLink to="/dictionary/person-titles" class="link-item">
          <div class="link-icon">🎓</div>
          <div class="link-content">
            <div class="link-title">人员职称</div>
            <div class="link-desc">查看人员职称信息</div>
          </div>
        </RouterLink>
        <RouterLink to="/dictionary/persons" class="link-item">
          <div class="link-icon">👥</div>
          <div class="link-content">
            <div class="link-title">人员信息</div>
            <div class="link-desc">管理人员基本信息</div>
          </div>
        </RouterLink>
      </div>
    </div>
  </section>
</template>

<script setup>
import { useRouter } from 'vue-router'
import PageHeader from '../components/PageHeader.vue'

const router = useRouter()

function goToUsers() {
  router.push('/users')
}

function goToDictionary() {
  router.push('/dictionary/hospitals')
}
</script>

<style scoped>
.card.clickable {
  cursor: pointer;
  transition: transform 0.2s, box-shadow 0.2s;
  position: relative;
}

.card.clickable:hover {
  transform: translateY(-4px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.arrow {
  position: absolute;
  right: 20px;
  top: 50%;
  transform: translateY(-50%);
  font-size: 24px;
  color: #999;
}

.quick-links {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 16px;
  margin-top: 16px;
}

.link-item {
  display: flex;
  align-items: center;
  padding: 16px;
  border: 1px solid #e8e8e8;
  border-radius: 8px;
  text-decoration: none;
  color: inherit;
  transition: all 0.2s;
}

.link-item:hover {
  border-color: #165DFF;
  box-shadow: 0 2px 8px rgba(22, 93, 255, 0.1);
}

.link-icon {
  font-size: 32px;
  margin-right: 16px;
}

.link-content {
  flex: 1;
}

.link-title {
  font-size: 16px;
  font-weight: 600;
  margin-bottom: 4px;
}

.link-desc {
  font-size: 12px;
  color: #999;
}
</style>
