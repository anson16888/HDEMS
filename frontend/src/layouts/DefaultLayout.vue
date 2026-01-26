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
        <a-menu
          v-model:selectedKeys="selectedKeys"
          v-model:openKeys="openKeys"
          mode="inline"
          :inline-collapsed="isCollapsed"
          :items="menuItems"
          @click="handleMenuClick"
        />
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
import { ref, computed, watch, h } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuth } from '../composables/useAuth.js'
import * as Icons from '@ant-design/icons-vue'

const router = useRouter()
const route = useRoute()
const { logout, user, isAuthenticated, isSystemAdmin, isDutyAdmin, isMaterialAdmin, isHospitalSystem, isBureauSystem } = useAuth()
const isCollapsed = ref(false)
const selectedKeys = ref([])
const openKeys = ref([])

// 退出登录
async function handleLogout() {
  if (confirm('确定要退出登录吗？')) {
    try {
      await logout()
      router.push('/login')
    } catch (error) {
      console.error('退出登录失败:', error)
      router.push('/login')
    }
  }
}

// 图标映射
const iconMap = {
  'fa-solid fa-boxes-stacked': 'InboxOutlined',
  'fa-solid fa-building-columns': 'BankOutlined',
  'fa-solid fa-hospital': 'MedicineBoxOutlined',
  'fa-solid fa-user-doctor': 'UserOutlined',
  'fa-solid fa-table-list': 'TableOutlined',
  'fa-solid fa-users': 'TeamOutlined',
  'fa-solid fa-gear': 'SettingOutlined',
  'fa-solid fa-sitemap': 'NodeIndexOutlined',
  'fa-solid fa-clock': 'ClockCircleOutlined',
  'fa-solid fa-ranking-star': 'TrophyOutlined',
  'fa-solid fa-award': 'StarOutlined',
  'fa-solid fa-user-tag': 'TagOutlined',
  'fa-solid fa-tags': 'TagsOutlined',
  'fa-solid fa-bell': 'BellOutlined'
}

// 获取图标组件
function getIcon(iconName) {
  const iconKey = iconMap[iconName]
  if (iconKey && Icons[iconKey]) {
    return h(Icons[iconKey])
  }
  return null
}

// 菜单项生成函数
function getItem(label, key, icon, children) {
  return {
    key,
    icon,
    children,
    label
  }
}

// 菜单配置
const menuItems = computed(() => {
  const items = []

  // ========== 根据SRS权限矩阵（表2-2和表2-3）配置菜单 ==========

  // 【物资管理】仅医院系统的物资管理员可见
  if (isHospitalSystem.value && isMaterialAdmin.value) {
    items.push(
      getItem('物资管理', '/materials', getIcon('fa-solid fa-boxes-stacked'))
    )
  }

  // 【值班管理】根据系统类型和角色显示不同菜单
  const scheduleItems = []

  if (isSystemAdmin.value || (isBureauSystem.value && isDutyAdmin.value)) {
    // 卫健委系统值班管理员：仅可见局级行政排班
    scheduleItems.push(
      getItem('局级行政排班', '/schedules/bureau', getIcon('fa-solid fa-building-columns'))
    )
  }
  if (isSystemAdmin.value || (isHospitalSystem.value && isDutyAdmin.value)) {
    // 医院系统值班管理员：可见院级行政排班和院内专家排班
    scheduleItems.push(
      getItem('院级行政排班', '/schedules/hospital', getIcon('fa-solid fa-hospital')),
      getItem('院内专家排班', '/schedules/director', getIcon('fa-solid fa-user-doctor'))
    )
  }

  // 【排班一览表】医院系统和卫健委系统都可见（根据角色权限）
  if (isSystemAdmin.value || (isHospitalSystem.value || isBureauSystem.value) && isDutyAdmin.value) {
    scheduleItems.push(
      getItem('排班一览表', '/schedules/overview', getIcon('fa-solid fa-table-list'))
    )
  }

  // 如果有排班菜单项，添加值班管理主菜单
  if (scheduleItems.length > 0) {
    items.push(
      getItem('值班管理', '/schedules', getIcon('fa-solid fa-building-columns'), scheduleItems)
    )
  }

  // 【用户管理】仅系统管理员可见
  if (isSystemAdmin.value) {
    items.push(
      getItem('用户管理', '/users', getIcon('fa-solid fa-users'))
    )
  }

  // 【系统管理】根据系统类型和角色显示不同菜单
  const systemItems = []

  // 医院信息管理：仅系统管理员可见
  if (isSystemAdmin.value) {
    systemItems.push(
      getItem('医院信息', '/system/hospitals', getIcon('fa-solid fa-hospital'))
    )
  }

  // 基础数据管理：系统管理员和值班管理员可见
  if (isSystemAdmin.value || isDutyAdmin.value) {
    systemItems.push(
      getItem('科室信息', '/system/departments', getIcon('fa-solid fa-sitemap')),
      getItem('班次信息', '/system/shifts', getIcon('fa-solid fa-clock')),
      getItem('人员职级', '/system/person-ranks', getIcon('fa-solid fa-ranking-star')),
      getItem('人员职称', '/system/person-titles', getIcon('fa-solid fa-award'))
    )
  }

  // 物资类型：仅物资管理员可见
  if (isSystemAdmin.value || isMaterialAdmin.value) {
    systemItems.push(
      getItem('物资类型', '/system/material-types', getIcon('fa-solid fa-tags')),
      getItem('库存阈值', '/system/material-thresholds', getIcon('fa-solid fa-bell'))
    )
  }

  // 如果有系统管理菜单项，添加系统管理主菜单
  if (systemItems.length > 0) {
    items.push(
      getItem('系统管理', '/system', getIcon('fa-solid fa-gear'), systemItems)
    )
  }

  return items
})

// 菜单点击处理
function handleMenuClick({ key }) {
  router.push(key)
}

// 自动展开当前路由所在的子菜单并设置选中项
watch(
  () => route.path,
  (newPath) => {
    selectedKeys.value = [newPath]

    // 查找父级菜单并展开
    menuItems.value.forEach(group => {
      if (group.children) {
        // 检查是否有子菜单项匹配当前路由
        const hasActiveChild = group.children.some(child => {
          // 精确匹配或前缀匹配（处理三级菜单的情况）
          return child.key === newPath || newPath.startsWith(child.key + '/')
        })

        if (hasActiveChild && !openKeys.value.includes(group.key)) {
          openKeys.value.push(group.key)
        }
      }
    })
  },
  { immediate: true }
)

const toggleSidebar = () => {
  isCollapsed.value = !isCollapsed.value
}
</script>

<style scoped>
/* Ant Design Vue Menu 组件样式覆盖 */
.sidebar :deep(.ant-menu) {
  background: transparent;
  border-right: none;
}

.sidebar :deep(.ant-menu-inline) {
  border-right: none;
}

.sidebar :deep(.ant-menu-item) {
  margin: 0;
  padding: 0.75rem 1rem;
  height: auto;
  line-height: 1.5;
  color: var(--color-text-secondary);
  border-radius: 0.5rem;
  transition: all 0.2s;
}

.sidebar :deep(.ant-menu-item:hover) {
  background-color: var(--color-bg-light);
  color: var(--color-text-primary);
}

.sidebar :deep(.ant-menu-item-selected) {
  background-color: var(--color-primary-light);
  color: var(--color-primary);
}

.sidebar :deep(.ant-menu-submenu) {
  margin: 0;
}

.sidebar :deep(.ant-menu-submenu-title) {
  margin: 0;
  padding: 0.75rem 1rem;
  height: auto;
  line-height: 1.5;
  color: var(--color-text-secondary);
  border-radius: 0.5rem;
  transition: all 0.2s;
}

.sidebar :deep(.ant-menu-submenu-title:hover) {
  background-color: var(--color-bg-light);
  color: var(--color-text-primary);
}

.sidebar :deep(.ant-menu-submenu-selected > .ant-menu-submenu-title) {
  background-color: var(--color-primary-light);
  color: var(--color-primary);
}

.sidebar :deep(.ant-menu-item .anticon) {
  font-size: 1rem;
  min-width: 1.25rem;
  text-align: center;
}

.sidebar :deep(.ant-menu-submenu-title .anticon) {
  font-size: 1rem;
  min-width: 1.25rem;
  text-align: center;
}

.sidebar :deep(.ant-menu-group-list) {
  margin: 0;
  padding: 0;
}

.sidebar :deep(.ant-menu-group-title) {
  padding: 0.5rem 1rem;
  font-size: 0.75rem;
  color: var(--color-text-secondary);
  font-weight: 500;
}

.sidebar :deep(.ant-menu-sub) {
  background: transparent;
}

.sidebar :deep(.ant-menu-sub .ant-menu-item) {
  padding-left: 2.5rem !important;
}

/* 折叠状态 */
.sidebar.collapsed :deep(.ant-menu-group-title) {
  display: none;
}
</style>
