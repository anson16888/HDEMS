/**
 * HTTP Request Utility
 * Wraps axios with common configuration for the HDEMS API
 */

import axios from 'axios'
import { message } from 'ant-design-vue'

// Create axios instance
const http = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api',
  timeout: 15000,
  headers: {
    'Content-Type': 'application/json'
  }
})

// Request interceptor
http.interceptors.request.use(
  (config) => {
    // Add auth token if available
    // Token is stored with 'hdems_' prefix by StorageService
    const token = localStorage.getItem('hdems_auth_token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Response interceptor
http.interceptors.response.use(
  (response) => {
    // Handle API response wrapper
    const { data } = response

    // Check if API call was successful
    if (data.success !== false && data.code >= 200 && data.code < 300) {
      return data
    }

    // API returned an error
    const error = {
      code: data.code,
      message: data.message || '请求失败',
      response
    }
    return Promise.reject(error)
  },
  (error) => {
    // Handle network errors and non-2xx responses
    if (error.response) {
      // Server responded with error status
      const { status, data } = error.response

      // Handle authentication errors
      if (status === 401) {
        // Clear token and redirect to login
        // Remove with 'hdems_' prefix as used by StorageService
        localStorage.removeItem('hdems_auth_token')
        localStorage.removeItem('hdems_auth_user')

        // Only redirect if not already on login page
        if (window.location.pathname !== '/login') {
          window.location.href = '/login'
        }

        return Promise.reject({
          code: 401,
          message: '登录已过期，请重新登录',
          response: error
        })
      }

      // Handle other HTTP errors
      const errorMessage = data?.message || getStatusMessage(status)
      message.error(errorMessage)

      return Promise.reject({
        code: status,
        message: errorMessage,
        response: error
      })
    } else if (error.request) {
      // Request was made but no response received
      message.error('网络错误，请检查网络连接')
      return Promise.reject({
        code: 'NETWORK_ERROR',
        message: '网络错误，请检查网络连接',
        error
      })
    } else {
      // Error setting up the request
      message.error(error.message || '请求失败')
      return Promise.reject({
        code: 'REQUEST_ERROR',
        message: error.message || '请求失败',
        error
      })
    }
  }
)

/**
 * Get human-readable error message from HTTP status code
 */
function getStatusMessage(status) {
  const messages = {
    400: '请求参数错误',
    401: '未授权，请重新登录',
    403: '拒绝访问',
    404: '请求的资源不存在',
    405: '请求方法不允许',
    500: '服务器内部错误',
    502: '网关错误',
    503: '服务不可用',
    504: '网关超时'
  }
  return messages[status] || `请求失败 (${status})`
}

/**
 * HTTP methods wrapper
 */
export const request = {
  get(url, params, config = {}) {
    return http.get(url, { params, ...config })
  },

  post(url, data, config = {}) {
    return http.post(url, data, config)
  },

  put(url, data, config = {}) {
    return http.put(url, data, config)
  },

  delete(url, params, config = {}) {
    return http.delete(url, { params, ...config })
  },

  // For file uploads
  upload(url, formData, config = {}) {
    return http.post(url, formData, {
      ...config,
      headers: {
        'Content-Type': 'multipart/form-data',
        ...config.headers
      }
    })
  },

  // For downloading files
  download(url, params, config = {}) {
    return http.get(url, {
      params,
      ...config,
      responseType: 'blob'
    })
  }
}

export default http
