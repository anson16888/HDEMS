import { createRouter, createWebHistory } from 'vue-router'
import MaterialsPage from '../pages/MaterialsPage.vue'
import BureauSchedulePage from '../pages/BureauSchedulePage.vue'
import HospitalSchedulePage from '../pages/HospitalSchedulePage.vue'
import DirectorSchedulePage from '../pages/DirectorSchedulePage.vue'
import ScheduleOverviewPage from '../pages/ScheduleOverviewPage.vue'
import SystemSettingsPage from '../pages/SystemSettingsPage.vue'
import UserManagementPage from '../pages/UserManagementPage.vue'
import LoginPage from '../pages/LoginPage.vue'
import NotFoundPage from '../pages/NotFoundPage.vue'

const routes = [
  {
    path: '/',
    redirect: '/materials'
  },
  {
    path: '/materials',
    name: 'materials',
    component: MaterialsPage
  },
  {
    path: '/schedules/bureau',
    name: 'bureau-schedule',
    component: BureauSchedulePage
  },
  {
    path: '/schedules/hospital',
    name: 'hospital-schedule',
    component: HospitalSchedulePage
  },
  {
    path: '/schedules/director',
    name: 'director-schedule',
    component: DirectorSchedulePage
  },
  {
    path: '/schedules/overview',
    name: 'schedule-overview',
    component: ScheduleOverviewPage
  },
  {
    path: '/system',
    name: 'system',
    component: SystemSettingsPage
  },
  {
    path: '/users',
    name: 'users',
    component: UserManagementPage,
    meta: { requiresAuth: true, roles: ['SYSTEM_ADMIN'] }
  },
  {
    path: '/login',
    name: 'login',
    component: LoginPage,
    meta: { layout: 'blank' }
  },
  {
    path: '/:pathMatch(.*)*',
    name: 'not-found',
    component: NotFoundPage,
    meta: { layout: 'blank' }
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior() {
    return { top: 0 }
  }
})

export default router
