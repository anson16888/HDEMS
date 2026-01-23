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

  // Material Type endpoints
  MATERIAL_TYPE: {
    LIST: '/MaterialType',
    DETAIL: (id) => `/MaterialType/${id}`,
    BY_CODE: (code) => `/MaterialType/code/${code}`,
    ENABLED: '/MaterialType/enabled',
    BATCH_DELETE: '/MaterialType/batch',
    TOGGLE: (id) => `/MaterialType/${id}/toggle`
  },

  // Material Threshold endpoints
  MATERIAL_THRESHOLD: {
    LIST: '/MaterialThreshold',
    DETAIL: (id) => `/MaterialThreshold/${id}`,
    TOGGLE: (id) => `/MaterialThreshold/${id}/toggle`
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
    PERSON_RANK_DETAIL: (id) => `/BasicData/person-ranks/${id}`,
    PERSON_RANKS_BY_CATEGORY: (category) => `/BasicData/person-ranks/by-category/${category}`,
    PERSON_TITLES: '/BasicData/person-titles',
    PERSON_TITLE_DETAIL: (id) => `/BasicData/person-titles/${id}`,
    PERSONS: '/BasicData/persons',
    PERSON_DETAIL: (id) => `/BasicData/persons/${id}`,
    PERSONS_BY_HOSPITAL: (hospitalId) => `/BasicData/persons/by-hospital/${hospitalId}`
  },

  // Hospital config endpoints
  HOSPITAL_CONFIG: {
    DETAIL: '/HospitalConfig',
    UPDATE: '/HospitalConfig'
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
  SYSTEM_ADMIN: 'SYSTEM_ADMIN',  // 系统管理员
  SCHEDULE_ADMIN: 'SCHEDULE_ADMIN',    // 值班管理员
  MATERIAL_ADMIN: 'MATERIAL_ADMIN', // 物资管理员
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
  NORMAL: 0,           // 正常
  LOW: 1,              // 库存偏低
  OUT: 2,              // 已耗尽
  EXPIRED: 3,          // 已过期
  EXPIRING_SOON: 4     // 即将过期
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
