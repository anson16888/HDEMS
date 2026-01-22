/**
 * API Configuration
 * Defines API endpoints and constants
 */

export const API_ENDPOINTS = {
  // Auth endpoints
  AUTH: {
    LOGIN: '/Auth/login',
    LOGOUT: '/Auth/logout',
    REFRESH: '/Auth/refresh',
    CURRENT_USER: '/Auth/current-user',
    CHANGE_PASSWORD: '/Auth/change-password',
    RESET_ADMIN_PASSWORD: '/Auth/reset-admin-password'
  },

  // User endpoints
  USER: {
    LIST: '/User',
    DETAIL: (id) => `/User/${id}`,
    CREATE: '/User',
    UPDATE: (id) => `/User/${id}`,
    DELETE: (id) => `/User/${id}`,
    RESET_PASSWORD: (id) => `/User/${id}/reset-password`
  },

  // Material endpoints
  MATERIAL: {
    LIST: '/Material',
    DETAIL: (id) => `/Material/${id}`,
    CREATE: '/Material',
    UPDATE: (id) => `/Material/${id}`,
    DELETE: (id) => `/Material/${id}`,
    BATCH_DELETE: '/Material/batch-delete',
    IMPORT: '/Material/import',
    EXPORT: '/Material/export',
    STATISTICS: '/Material/statistics'
  },

  // Schedule endpoints
  SCHEDULE: {
    LIST: '/Schedule',
    DETAIL: (id) => `/Schedule/${id}`,
    CREATE: '/Schedule',
    UPDATE: (id) => `/Schedule/${id}`,
    DELETE: (id) => `/Schedule/${id}`,
    BATCH_CREATE: '/Schedule/batch',
    BATCH_DELETE: '/Schedule/batch-delete',
    OVERVIEW: '/Schedule/overview',
    STATISTICS: '/Schedule/statistics',
    EXPORT: '/Schedule/export',
    IMPORT: (scheduleType) => `/Schedule/import/${scheduleType}`
  },

  // Basic Data endpoints
  BASIC_DATA: {
    HOSPITALS: '/BasicData/hospitals',
    HOSPITAL_DETAIL: (id) => `/BasicData/hospitals/${id}`,
    DEPARTMENTS: '/BasicData/departments',
    DEPARTMENT_DETAIL: (id) => `/BasicData/departments/${id}`,
    SHIFTS: '/BasicData/shifts',
    SHIFT_DETAIL: (id) => `/BasicData/shifts/${id}`,
    PERSON_RANKS: '/BasicData/person-ranks',
    PERSON_RANKS_BY_CATEGORY: (category) => `/BasicData/person-ranks/by-category/${category}`,
    PERSON_TITLES: '/BasicData/person-titles',
    PERSONS: '/BasicData/persons',
    PERSON_DETAIL: (id) => `/BasicData/persons/${id}`,
    PERSONS_BY_HOSPITAL: (hospitalId) => `/BasicData/persons/by-hospital/${hospitalId}`
  },

  // Import/Export endpoints
  IMPORT_EXPORT: {
    MATERIAL_TEMPLATE: '/ImportExport/material-template',
    SCHEDULE_TEMPLATE: (scheduleType) => `/ImportExport/schedule-template/${scheduleType}`
  },

  // Health check
  HEALTH: '/health'
}

/**
 * User Roles Enum
 * Matches backend UserRole enum
 */
export const USER_ROLES = {
  SYSTEM_ADMIN: 1,  // 系统管理员
  DUTY_ADMIN: 2,    // 值班管理员
  MATERIAL_ADMIN: 3, // 物资管理员
  HOSPITAL_USER: 4   // 医院用户
}

/**
 * User Status Enum
 * Matches backend UserStatus enum
 */
export const USER_STATUS = {
  ACTIVE: 1,    // 正常
  INACTIVE: 2,  // 停用
  LOCKED: 3     // 锁定
}

/**
 * Material Type Enum
 * Matches backend MaterialType enum
 */
export const MATERIAL_TYPE = {
  MEDICAL: 1,     // 医疗设备
  MEDICINE: 2,    // 药品
  EMERGENCY: 3,   // 急救物资
  CONSUMABLE: 4,  // 耗材
  EQUIPMENT: 5    // 设备
}

/**
 * Material Status Enum
 * Matches backend MaterialStatus enum
 */
export const MATERIAL_STATUS = {
  NORMAL: 1,      // 正常
  EXPIRED: 2,     // 过期
  LOW_STOCK: 3,   // 库存不足
  DAMAGED: 4,     // 损坏
  EXPIRING: 5     // 临期
}

/**
 * Schedule Type Enum
 * Matches backend ScheduleType enum
 */
export const SCHEDULE_TYPE = {
  BUREAU: 1,      // 局值班
  HOSPITAL: 2,    // 医院值班
  DIRECTOR: 3     // 主任值班
}

/**
 * Pagination defaults
 */
export const PAGINATION = {
  DEFAULT_PAGE: 1,
  DEFAULT_PAGE_SIZE: 20,
  PAGE_SIZE_OPTIONS: [10, 20, 50, 100]
}

export default {
  API_ENDPOINTS,
  USER_ROLES,
  USER_STATUS,
  MATERIAL_TYPE,
  MATERIAL_STATUS,
  SCHEDULE_TYPE,
  PAGINATION
}
