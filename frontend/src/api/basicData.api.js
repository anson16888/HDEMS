/**
 * Basic Data API
 * Dictionary data management API calls
 * Manages hospitals, departments, shifts, person ranks, titles, and persons
 */

import { request } from '../utils/http.js'
import { API_ENDPOINTS } from '../config/api.config.js'

// ==================== Hospitals ====================

/**
 * Get hospitals list
 * @returns {Promise<Array>} Hospitals list
 */
export function getHospitals() {
  return request.get(API_ENDPOINTS.BASIC_DATA.HOSPITALS)
}

/**
 * Get hospital by ID
 * @param {string} id - Hospital ID
 * @returns {Promise<object>} Hospital detail
 */
export function getHospitalById(id) {
  return request.get(API_ENDPOINTS.BASIC_DATA.HOSPITAL_DETAIL(id))
}

/**
 * Create new hospital
 * @param {object} data - Hospital data { hospitalCode, hospitalName, shortName, address, contactPhone, contactPerson, sortOrder, status }
 * @returns {Promise<object>} Created hospital
 */
export function createHospital(data) {
  return request.post(API_ENDPOINTS.BASIC_DATA.HOSPITALS, data)
}

/**
 * Update hospital
 * @param {string} id - Hospital ID
 * @param {object} data - Update data
 * @returns {Promise<object>} Updated hospital
 */
export function updateHospital(id, data) {
  return request.put(API_ENDPOINTS.BASIC_DATA.HOSPITAL_DETAIL(id), data)
}

/**
 * Delete hospital
 * @param {string} id - Hospital ID
 * @returns {Promise<object>}
 */
export function deleteHospital(id) {
  return request.delete(API_ENDPOINTS.BASIC_DATA.HOSPITAL_DETAIL(id))
}

// ==================== Departments ====================

/**
 * Get departments list
 * @returns {Promise<Array>} Departments list
 */
export function getDepartments() {
  return request.get(API_ENDPOINTS.BASIC_DATA.DEPARTMENTS)
}

/**
 * Get department by ID
 * @param {string} id - Department ID
 * @returns {Promise<object>} Department detail
 */
export function getDepartmentById(id) {
  return request.get(API_ENDPOINTS.BASIC_DATA.DEPARTMENT_DETAIL(id))
}

/**
 * Create new department
 * @param {object} data - Department data { departmentCode, departmentName, departmentType, sortOrder }
 * @returns {Promise<object>} Created department
 */
export function createDepartment(data) {
  return request.post(API_ENDPOINTS.BASIC_DATA.DEPARTMENTS, data)
}

/**
 * Update department
 * @param {string} id - Department ID
 * @param {object} data - Update data
 * @returns {Promise<object>} Updated department
 */
export function updateDepartment(id, data) {
  return request.put(API_ENDPOINTS.BASIC_DATA.DEPARTMENT_DETAIL(id), data)
}

/**
 * Delete department
 * @param {string} id - Department ID
 * @returns {Promise<object>}
 */
export function deleteDepartment(id) {
  return request.delete(API_ENDPOINTS.BASIC_DATA.DEPARTMENT_DETAIL(id))
}

// ==================== Shifts ====================

/**
 * Get shifts list
 * @returns {Promise<Array>} Shifts list
 */
export function getShifts() {
  return request.get(API_ENDPOINTS.BASIC_DATA.SHIFTS)
}

/**
 * Get shift by ID
 * @param {string} id - Shift ID
 * @returns {Promise<object>} Shift detail
 */
export function getShiftById(id) {
  return request.get(API_ENDPOINTS.BASIC_DATA.SHIFT_DETAIL(id))
}

/**
 * Create new shift
 * @param {object} data - Shift data { shiftCode, shiftName, timeRange, sortOrder }
 * @returns {Promise<object>} Created shift
 */
export function createShift(data) {
  return request.post(API_ENDPOINTS.BASIC_DATA.SHIFTS, data)
}

/**
 * Update shift
 * @param {string} id - Shift ID
 * @param {object} data - Update data
 * @returns {Promise<object>} Updated shift
 */
export function updateShift(id, data) {
  return request.put(API_ENDPOINTS.BASIC_DATA.SHIFT_DETAIL(id), data)
}

/**
 * Delete shift
 * @param {string} id - Shift ID
 * @returns {Promise<object>}
 */
export function deleteShift(id) {
  return request.delete(API_ENDPOINTS.BASIC_DATA.SHIFT_DETAIL(id))
}

// ==================== Person Ranks ====================

/**
 * Get person ranks list
 * @returns {Promise<Array>} Person ranks list
 */
export function getPersonRanks() {
  return request.get(API_ENDPOINTS.BASIC_DATA.PERSON_RANKS)
}

/**
 * Get person ranks by category
 * @param {string} category - Category (e.g., 'bureau', 'hospital', 'administrative')
 * @returns {Promise<Array>} Person ranks list filtered by category
 */
export function getPersonRanksByCategory(category) {
  return request.get(API_ENDPOINTS.BASIC_DATA.PERSON_RANKS_BY_CATEGORY(category))
}

/**
 * Get person rank by ID
 * @param {string} id - Person rank ID
 * @returns {Promise<object>} Person rank detail
 */
export function getPersonRankById(id) {
  return request.get(API_ENDPOINTS.BASIC_DATA.PERSON_RANK_DETAIL(id))
}

/**
 * Create new person rank
 * @param {object} data - Person rank data { rankCode, rankName, category, sortOrder }
 * @returns {Promise<object>} Created person rank
 */
export function createPersonRank(data) {
  return request.post(API_ENDPOINTS.BASIC_DATA.PERSON_RANKS, data)
}

/**
 * Update person rank
 * @param {string} id - Person rank ID
 * @param {object} data - Update data
 * @returns {Promise<object>} Updated person rank
 */
export function updatePersonRank(id, data) {
  return request.put(API_ENDPOINTS.BASIC_DATA.PERSON_RANK_DETAIL(id), data)
}

/**
 * Delete person rank
 * @param {string} id - Person rank ID
 * @returns {Promise<object>}
 */
export function deletePersonRank(id) {
  return request.delete(API_ENDPOINTS.BASIC_DATA.PERSON_RANK_DETAIL(id))
}

// ==================== Person Titles ====================

/**
 * Get person titles list
 * @returns {Promise<Array>} Person titles list
 */
export function getPersonTitles() {
  return request.get(API_ENDPOINTS.BASIC_DATA.PERSON_TITLES)
}

/**
 * Get person title by ID
 * @param {string} id - Person title ID
 * @returns {Promise<object>} Person title detail
 */
export function getPersonTitleById(id) {
  return request.get(API_ENDPOINTS.BASIC_DATA.PERSON_TITLE_DETAIL(id))
}

/**
 * Create new person title
 * @param {object} data - Person title data { titleCode, titleName, sortOrder }
 * @returns {Promise<object>} Created person title
 */
export function createPersonTitle(data) {
  return request.post(API_ENDPOINTS.BASIC_DATA.PERSON_TITLES, data)
}

/**
 * Update person title
 * @param {string} id - Person title ID
 * @param {object} data - Update data
 * @returns {Promise<object>} Updated person title
 */
export function updatePersonTitle(id, data) {
  return request.put(API_ENDPOINTS.BASIC_DATA.PERSON_TITLE_DETAIL(id), data)
}

/**
 * Delete person title
 * @param {string} id - Person title ID
 * @returns {Promise<object>}
 */
export function deletePersonTitle(id) {
  return request.delete(API_ENDPOINTS.BASIC_DATA.PERSON_TITLE_DETAIL(id))
}

// ==================== Persons ====================

/**
 * Get persons list with pagination and filters
 * @param {object} params - Query parameters { page, pageSize, keyword }
 * @returns {Promise<object>} Paginated persons list
 */
export function getPersons(params = {}) {
  return request.get(API_ENDPOINTS.BASIC_DATA.PERSONS, params)
}

/**
 * Get person by ID
 * @param {string} id - Person ID
 * @returns {Promise<object>} Person detail
 */
export function getPersonById(id) {
  return request.get(API_ENDPOINTS.BASIC_DATA.PERSON_DETAIL(id))
}

/**
 * Get persons by hospital
 * @param {string} hospitalId - Hospital ID
 * @returns {Promise<Array>} Persons list filtered by hospital
 */
export function getPersonsByHospital(hospitalId) {
  return request.get(API_ENDPOINTS.BASIC_DATA.PERSONS_BY_HOSPITAL(hospitalId))
}

/**
 * Create new person
 * @param {object} data - Person data { personCode, personName, gender, phone, departmentId, hospitalId, rankId, titleId, status }
 * @returns {Promise<object>} Created person
 */
export function createPerson(data) {
  return request.post(API_ENDPOINTS.BASIC_DATA.PERSONS, data)
}

/**
 * Update person
 * @param {string} id - Person ID
 * @param {object} data - Update data
 * @returns {Promise<object>} Updated person
 */
export function updatePerson(id, data) {
  return request.put(API_ENDPOINTS.BASIC_DATA.PERSON_DETAIL(id), data)
}

/**
 * Delete person
 * @param {string} id - Person ID
 * @returns {Promise<object>}
 */
export function deletePerson(id) {
  return request.delete(API_ENDPOINTS.BASIC_DATA.PERSON_DETAIL(id))
}

// ==================== Export ====================

export default {
  // Hospitals
  getHospitals,
  getHospitalById,
  createHospital,
  updateHospital,
  deleteHospital,

  // Departments
  getDepartments,
  getDepartmentById,
  createDepartment,
  updateDepartment,
  deleteDepartment,

  // Shifts
  getShifts,
  getShiftById,
  createShift,
  updateShift,
  deleteShift,

  // Person Ranks
  getPersonRanks,
  getPersonRanksByCategory,
  getPersonRankById,
  createPersonRank,
  updatePersonRank,
  deletePersonRank,

  // Person Titles
  getPersonTitles,
  getPersonTitleById,
  createPersonTitle,
  updatePersonTitle,
  deletePersonTitle,

  // Persons
  getPersons,
  getPersonById,
  getPersonsByHospital,
  createPerson,
  updatePerson,
  deletePerson
}
