/**
 * Toast Notification System
 * Provides a simple toast notification mechanism for user feedback
 *
 * @usage
 * import { useToast } from '@/composables/useToast'
 * const { showToast } = useToast()
 * showToast('操作成功', 'success')
 */

import { nextTick } from 'vue'

// Toast container element (created once)
let toastContainer = null

// Toast types with their configurations
const toastTypes = {
  success: {
    icon: '✓',
    bgColor: '#4CAF50',
    color: '#fff'
  },
  error: {
    icon: '✕',
    bgColor: '#F44336',
    color: '#fff'
  },
  warning: {
    icon: '⚠',
    bgColor: '#FF9800',
    color: '#fff'
  },
  info: {
    icon: 'ℹ',
    bgColor: '#2196F3',
    color: '#fff'
  }
}

/**
 * Show a toast notification
 * @param {string} message - The message to display
 * @param {string} type - Toast type: 'success' | 'error' | 'warning' | 'info'
 * @param {number} duration - Auto-dismiss duration in milliseconds (default: 3000)
 */
export function showToast(message, type = 'info', duration = 3000) {
  // Create toast container if it doesn't exist
  if (!toastContainer) {
    toastContainer = document.createElement('div')
    toastContainer.id = 'toast-container'
    toastContainer.style.cssText = `
      position: fixed;
      top: 20px;
      right: 20px;
      z-index: 9999;
      display: flex;
      flex-direction: column;
      gap: 10px;
      pointer-events: none;
    `
    document.body.appendChild(toastContainer)
  }

  // Get toast type configuration
  const typeConfig = toastTypes[type] || toastTypes.info

  // Create toast element
  const toast = document.createElement('div')
  toast.className = `toast toast-${type}`
  toast.style.cssText = `
    display: flex;
    align-items: center;
    gap: 10px;
    padding: 12px 16px;
    background: ${typeConfig.bgColor};
    color: ${typeConfig.color};
    border-radius: 8px;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
    font-size: 14px;
    font-weight: 500;
    min-width: 300px;
    max-width: 400px;
    pointer-events: auto;
    opacity: 0;
    transform: translateX(100%);
    transition: opacity 0.3s ease, transform 0.3s ease;
    cursor: pointer;
  `

  // Toast content
  toast.innerHTML = `
    <span style="font-size: 18px; flex-shrink: 0;">${typeConfig.icon}</span>
    <span style="flex: 1;">${escapeHtml(message)}</span>
    <span class="toast-close" style="font-size: 16px; opacity: 0.7; flex-shrink: 0;">×</span>
  `

  // Add hover styles for close button
  toast.addEventListener('mouseenter', () => {
    toast.style.boxShadow = '0 6px 16px rgba(0, 0, 0, 0.2)'
  })
  toast.addEventListener('mouseleave', () => {
    toast.style.boxShadow = '0 4px 12px rgba(0, 0, 0, 0.15)'
  })

  // Dismiss on click
  toast.addEventListener('click', () => {
    dismissToast(toast)
  })

  // Add to container
  toastContainer.appendChild(toast)

  // Trigger animation
  nextTick(() => {
    toast.style.opacity = '1'
    toast.style.transform = 'translateX(0)'
  })

  // Auto-dismiss after duration
  if (duration > 0) {
    setTimeout(() => {
      dismissToast(toast)
    }, duration)
  }
}

/**
 * Dismiss a toast with animation
 * @param {HTMLElement} toast - The toast element to dismiss
 */
function dismissToast(toast) {
  if (!toast || !toast.parentNode) return

  toast.style.opacity = '0'
  toast.style.transform = 'translateX(100%)'

  setTimeout(() => {
    if (toast.parentNode) {
      toast.parentNode.removeChild(toast)
    }

    // Remove container if empty
    if (toastContainer && toastContainer.children.length === 0) {
      toastContainer.remove()
      toastContainer = null
    }
  }, 300) // Wait for animation to complete
}

/**
 * Escape HTML to prevent XSS
 * @param {string} text - Text to escape
 * @returns {string} Escaped text
 */
function escapeHtml(text) {
  const div = document.createElement('div')
  div.textContent = text
  return div.innerHTML
}

/**
 * Composable for using toast notifications
 * @returns {Object} Toast functions
 */
export function useToast() {
  return {
    showToast,
    success: (message, duration) => showToast(message, 'success', duration),
    error: (message, duration) => showToast(message, 'error', duration),
    warning: (message, duration) => showToast(message, 'warning', duration),
    info: (message, duration) => showToast(message, 'info', duration)
  }
}

export default useToast
