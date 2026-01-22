import { createRouter, createWebHistory } from 'vue-router'
import MaterialsPage from '../pages/MaterialsPage.vue'
import BureauSchedulePage from '../pages/BureauSchedulePage.vue'
import HospitalSchedulePage from '../pages/HospitalSchedulePage.vue'
import DirectorSchedulePage from '../pages/DirectorSchedulePage.vue'
import ScheduleOverviewPage from '../pages/ScheduleOverviewPage.vue'
import UserManagementPage from '../pages/UserManagementPage.vue'
import HospitalManagementPage from '../pages/HospitalManagementPage.vue'
import DepartmentManagementPage from '../pages/DepartmentManagementPage.vue'
import ShiftManagementPage from '../pages/ShiftManagementPage.vue'
import PersonRankManagementPage from '../pages/PersonRankManagementPage.vue'
import PersonTitleManagementPage from '../pages/PersonTitleManagementPage.vue'
import PersonManagementPage from '../pages/PersonManagementPage.vue'
import MaterialTypeManagementPage from '../pages/MaterialTypeManagementPage.vue'
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
    path: '/users',
    name: 'users',
    component: UserManagementPage,
    meta: { requiresAuth: true, roles: ['SYSTEM_ADMIN'] }
  },
  {
    path: '/system/hospitals',
    name: 'system-hospitals',
    component: HospitalManagementPage,
    meta: { requiresAuth: true }
  },
  {
    path: '/system/departments',
    name: 'system-departments',
    component: DepartmentManagementPage,
    meta: { requiresAuth: true }
  },
  {
    path: '/system/shifts',
    name: 'system-shifts',
    component: ShiftManagementPage,
    meta: { requiresAuth: true }
  },
  {
    path: '/system/person-ranks',
    name: 'system-person-ranks',
    component: PersonRankManagementPage,
    meta: { requiresAuth: true }
  },
  {
    path: '/system/person-titles',
    name: 'system-person-titles',
    component: PersonTitleManagementPage,
    meta: { requiresAuth: true }
  },
  {
    path: '/system/persons',
    name: 'system-persons',
    component: PersonManagementPage,
    meta: { requiresAuth: true }
  },
  {
    path: '/system/material-types',
    name: 'system-material-types',
    component: MaterialTypeManagementPage,
    meta: { requiresAuth: true }
  },
  {
    path: '/system',
    name: 'system',
    redirect: '/system/hospitals'
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
