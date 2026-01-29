/**
 * Schedule API
 * Scheduling management API calls
 * Manages bureau, hospital, and director schedules
 */

import { request } from '../utils/http.js'
import { API_ENDPOINTS, SCHEDULE_TYPE } from '../config/api.config.js'

// ==================== Schedule CRUD ====================

/**
 * Get schedules list with pagination and filters
 * @param {object} params - Query parameters { startDate, endDate, scheduleType, departmentId, hospitalId, shiftId, personId, keyword, page, pageSize }
 * @returns {Promise<object>} Paginated schedules list
 */
export function getSchedules(params = {}) {
  return request.get(API_ENDPOINTS.SCHEDULE.LIST, params)
}

/**
 * Get schedule by ID
 * @param {string} id - Schedule ID
 * @returns {Promise<object>} Schedule detail
 */
export function getScheduleById(id) {
  return request.get(API_ENDPOINTS.SCHEDULE.DETAIL(id))
}

/**
 * Create new schedule
 * @param {object} data - Schedule data { scheduleDate, scheduleType, shiftId, personId, departmentId, hospitalId, remarks }
 * @returns {Promise<object>} Created schedule
 */
export function createSchedule(data) {
  return request.post(API_ENDPOINTS.SCHEDULE.CREATE, data)
}

/**
 * Update schedule
 * @param {string} id - Schedule ID
 * @param {object} data - Update data
 * @returns {Promise<object>} Updated schedule
 */
export function updateSchedule(id, data) {
  return request.put(API_ENDPOINTS.SCHEDULE.UPDATE(id), data)
}

/**
 * Delete schedule
 * @param {string} id - Schedule ID
 * @returns {Promise<object>}
 */
export function deleteSchedule(id) {
  return request.delete(API_ENDPOINTS.SCHEDULE.DELETE(id))
}

// ==================== Batch Operations ====================

/**
 * Batch create schedules
 * @param {Array<object>} data - Array of schedule objects
 * @returns {Promise<Array>} Created schedules
 */
export function batchCreateSchedules(data) {
  return request.post(API_ENDPOINTS.SCHEDULE.BATCH_CREATE, data)
}

/**
 * Batch delete schedules
 * @param {Array<string>} ids - Array of schedule IDs
 * @returns {Promise<object>}
 */
export function batchDeleteSchedules(ids) {
  return request.post(API_ENDPOINTS.SCHEDULE.BATCH_DELETE, ids)
}

// ==================== Schedule Overview ====================

/**
 * Get schedule overview (calendar view)
 * @param {object} params - Query parameters { startDate, endDate, scheduleType, departmentId, hospitalId, page, pageSize }
 * @returns {Promise<object>} Paginated schedule overview
 */
export function getScheduleOverview(params = {}) {
  return request.get(API_ENDPOINTS.SCHEDULE.OVERVIEW, params)
}

// ==================== Statistics ====================

/**
 * Get schedule statistics
 * @param {object} params - Query parameters { startDate, endDate, scheduleType, departmentId, hospitalId }
 * @returns {Promise<object>} Schedule statistics
 */
export function getScheduleStatistics(params = {}) {
  return request.get(API_ENDPOINTS.SCHEDULE.STATISTICS, params)
}

// ==================== Import/Export ====================

/**
 * Export schedules to Excel
 * @param {object} data - Query parameters { startDate, endDate, scheduleType, departmentId, hospitalId }
 * @returns {Promise<Blob>} Excel file
 */
export function exportSchedules(data) {
  return request.post(API_ENDPOINTS.SCHEDULE.EXPORT, data, {
    responseType: 'blob'
  }).then(response => response.data)
}

/**
 * Import schedules from Excel
 * @param {number} scheduleType - Schedule type (1=Bureau, 2=Hospital, 3=Director)
 * @param {FormData} formData - FormData with file
 * @returns {Promise<object>} Import result
 */
export function importSchedules(scheduleType, formData) {
  return request.post(API_ENDPOINTS.SCHEDULE.IMPORT(scheduleType), formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  })
}

/**
 * Download schedule template
 * @param {number} scheduleType - Schedule type (1=Bureau, 2=Hospital, 3=Director)
 * @returns {Promise<Blob>} Template file
 */
export function downloadScheduleTemplate(scheduleType) {
  return request.get(API_ENDPOINTS.IMPORT_EXPORT.SCHEDULE_TEMPLATE(scheduleType), {}, {
    responseType: 'blob'
  }).then(response => response.data)
}

// ==================== Helper Functions ====================

/**
 * Get schedule type label
 * @param {number} type - Schedule type code
 * @returns {string} Schedule type name
 */
export function getScheduleTypeLabel(type) {
  const labels = {
    [SCHEDULE_TYPE.BUREAU]: '局值班',
    [SCHEDULE_TYPE.HOSPITAL]: '医院值班',
    [SCHEDULE_TYPE.DIRECTOR]: '主任值班'
  }
  return labels[type] || '未知类型'
}

/**
 * Get schedule type badge class
 * @param {number} type - Schedule type code
 * @returns {string} CSS class name
 */
export function getScheduleTypeBadgeClass(type) {
  const classes = {
    [SCHEDULE_TYPE.BUREAU]: 'primary',
    [SCHEDULE_TYPE.HOSPITAL]: 'success',
    [SCHEDULE_TYPE.DIRECTOR]: 'warning'
  }
  return classes[type] || 'default'
}

// ==================== Export ====================

export default {
  // CRUD
  getSchedules,
  getScheduleById,
  createSchedule,
  updateSchedule,
  deleteSchedule,

  // Batch operations
  batchCreateSchedules,
  batchDeleteSchedules,

  // Overview & Statistics
  getScheduleOverview,
  getScheduleStatistics,

  // Import/Export
  exportSchedules,
  importSchedules,
  downloadScheduleTemplate,

  // Helpers
  getScheduleTypeLabel,
  getScheduleTypeBadgeClass
}
