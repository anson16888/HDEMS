<template>
  <div class="app-shell">
    <header class="app-header">
      <div class="header-left">
        <button class="icon-button" type="button" @click="toggleSidebar">
          <i class="fa-solid fa-bars"></i>
        </button>
        <div class="brand">
          <div class="brand-mark">
            <i class="fa-solid fa-shield"></i>
          </div>
          <div>
            <p class="brand-title">应急物资及值班管理系统</p>
            <p class="brand-subtitle">HDEMS 管理中心</p>
          </div>
        </div>
      </div>
      <div class="header-right">
        <button class="icon-button" type="button">
          <i class="fa-regular fa-bell"></i>
          <span class="notify-dot"></span>
        </button>
        <div class="user-dropdown">
          <button class="user-card" type="button">
            <div class="user-avatar">{{ user?.realName?.substring(0, 1) || '用' }}</div>
            <div class="user-meta">
              <span class="user-name">{{ user?.realName || '未登录' }}</span>
              <span class="user-role">{{ user?.username || '' }}</span>
            </div>
            <i class="fa-solid fa-angle-down"></i>
          </button>
          <div class="user-menu">
            <button class="user-menu-item" type="button">
              <i class="fa-regular fa-user"></i>
              个人资料
            </button>
            <button class="user-menu-item" type="button">
              <i class="fa-solid fa-gear"></i>
              系统设置
            </button>
            <div class="menu-divider"></div>
            <button class="user-menu-item danger" type="button" @click="handleLogout">
              <i class="fa-solid fa-right-from-bracket"></i>
              退出登录
            </button>
          </div>
        </div>
      </div>
    </header>

    <div class="app-body">
      <aside :class="['sidebar', { collapsed: isCollapsed }]">
        <nav class="nav-group" v-for="section in menuSections" :key="section.title">
          <p class="nav-title" :class="{ hidden: isCollapsed }">{{ section.title }}</p>
          <RouterLink
            v-for="item in section.items"
            :key="item.to"
            :to="item.to"
            class="menu-item"
            active-class="menu-item-active"
            :title="item.label"
          >
            <i :class="item.icon"></i>
            <span :class="{ hidden: isCollapsed }">{{ item.label }}</span>
          </RouterLink>
        </nav>
      </aside>

      <main class="app-content">
        <slot />
      </main>
    </div>

    <footer class="app-footer">
      <p>© 2026 宝安区域急救应急物资及值班管理系统 | 服务邮箱：service@example.com</p>
    </footer>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { useAuth } from '../composables/useAuth.js'

const router = useRouter()
const { logout, user, isAuthenticated, isSystemAdmin } = useAuth()
const isCollapsed = ref(false)

// 退出登录
async function handleLogout() {
  if (confirm('确定要退出登录吗？')) {
    try {
      await logout()
      // 跳转到登录页
      router.push('/login')
    } catch (error) {
      console.error('退出登录失败:', error)
      // 即使失败也清除本地状态并跳转
      router.push('/login')
    }
  }
}

const menuSections = computed(() => {
  const sections = [
    {
      title: '主菜单',
      items: [
        { label: '物资管理', to: '/materials', icon: 'fa-solid fa-boxes-stacked' }
      ]
    },
    {
      title: '值班管理',
      items: [
        { label: '局级行政排班', to: '/schedules/bureau', icon: 'fa-solid fa-building-columns' },
        { label: '院级行政排班', to: '/schedules/hospital', icon: 'fa-solid fa-hospital' },
        { label: '院内主任排班', to: '/schedules/director', icon: 'fa-solid fa-user-doctor' },
        { label: '排班一览表', to: '/schedules/overview', icon: 'fa-solid fa-table-list' }
      ]
    }
  ]

  // 系统设置部分（始终显示系统管理，仅系统管理员显示用户管理）
  const systemSettingsSection = {
    title: '系统设置',
    items: [
      { label: '系统管理', to: '/system', icon: 'fa-solid fa-gear' }
    ]
  }

  // 仅系统管理员可见用户管理
  if (isSystemAdmin.value) {
    systemSettingsSection.items.unshift(
      { label: '用户管理', to: '/users', icon: 'fa-solid fa-users' }
    )
  }

  sections.push(systemSettingsSection)

  return sections
})

const toggleSidebar = () => {
  isCollapsed.value = !isCollapsed.value
}
</script>
