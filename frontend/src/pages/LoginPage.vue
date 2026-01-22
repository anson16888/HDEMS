<template>
  <div class="login-page">
    <div class="login-card">
      <div class="login-brand">
        <div class="brand-mark">
          <i class="fa-solid fa-shield"></i>
        </div>
        <div>
          <h1>欢迎登录</h1>
          <p>宝安区域急救应急物资及值班管理系统</p>
        </div>
      </div>

      <form class="login-form" @submit.prevent="handleLogin">
        <label :class="{ 'has-error': errors.username }">
          账号/工号
          <input
            type="text"
            v-model="form.username"
            placeholder="请输入账号"
            :disabled="loading"
            @input="clearError('username')"
          />
          <span class="error-message" v-if="errors.username">{{ errors.username }}</span>
        </label>

        <label :class="{ 'has-error': errors.password }">
          密码
          <div class="password-input">
            <input
              :type="showPassword ? 'text' : 'password'"
              v-model="form.password"
              placeholder="请输入密码"
              :disabled="loading"
              @input="clearError('password')"
            />
            <button
              type="button"
              class="toggle-password"
              @click="showPassword = !showPassword"
              :disabled="loading"
            >
              <i :class="showPassword ? 'fa-solid fa-eye-slash' : 'fa-solid fa-eye'"></i>
            </button>
          </div>
          <span class="error-message" v-if="errors.password">{{ errors.password }}</span>
        </label>

        <button class="primary-button" type="submit" :disabled="loading">
          <span v-if="!loading">登录</span>
          <span v-else><i class="fa-solid fa-spinner fa-spin"></i> 登录中...</span>
        </button>
      </form>

      <!-- Alert for login errors -->
      <div class="alert alert-error" v-if="loginError" :class="{ 'slide-in': loginError }">
        <i class="fa-solid fa-exclamation-circle"></i>
        <span>{{ loginError }}</span>
        <button class="close-alert" @click="clearLoginError">
          <i class="fa-solid fa-times"></i>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuth } from '../composables/useAuth.js'

const router = useRouter()
const route = useRoute()
const { login, lastError, clearError: clearAuthError } = useAuth()

// Form state
const form = reactive({
  username: '',
  password: ''
})

const loading = ref(false)
const showPassword = ref(false)
const loginError = ref(null)
const errors = reactive({
  username: '',
  password: ''
})

// Clear field error
function clearError(field) {
  errors[field] = ''
}

// Clear login error
function clearLoginError() {
  loginError.value = null
  clearAuthError()
}

// Validate form
function validateForm() {
  let isValid = true

  if (!form.username) {
    errors.username = '请输入账号'
    isValid = false
  }

  if (!form.password) {
    errors.password = '请输入密码'
    isValid = false
  }

  return isValid
}

// Handle login
async function handleLogin() {
  // Clear previous errors
  clearLoginError()

  // Validate form
  if (!validateForm()) {
    return
  }

  loading.value = true

  try {
    await login(form.username, form.password)

    // Get redirect path from query or default to home
    const redirect = route.query.redirect || '/'

    // Redirect after successful login
    router.push(redirect)
  } catch (error) {
    // Show error message
    loginError.value = lastError.value || '登录失败，请重试'

    // Handle specific errors
    if (error.code === 'ACCOUNT_LOCKED') {
      loginError.value = '账号已被锁定30分钟，请稍后再试'
    }
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  // Check if already logged in
  // If so, redirect to home
  // This will be handled by route guard
})
</script>

<style scoped>
.login-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 20px;
}

.login-card {
  background: white;
  border-radius: 12px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
  width: 100%;
  max-width: 450px;
  padding: 40px;
  position: relative;
}

.login-brand {
  text-align: center;
  margin-bottom: 32px;
}

.brand-mark {
  font-size: 48px;
  color: #667eea;
  margin-bottom: 16px;
}

.login-brand h1 {
  font-size: 28px;
  font-weight: 600;
  color: #1a1a1a;
  margin: 0 0 8px 0;
}

.login-brand p {
  font-size: 14px;
  color: #666;
  margin: 0;
}

.login-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.login-form > label {
  display: flex;
  flex-direction: column;
  gap: 8px;
  font-weight: 500;
  color: #333;
  font-size: 14px;
}

.login-form input {
  padding: 12px 16px;
  border: 1px solid #ddd;
  border-radius: 8px;
  font-size: 14px;
  transition: all 0.3s;
}

.login-form input:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.login-form input:disabled {
  background-color: #f5f5f5;
  cursor: not-allowed;
}

.password-input {
  position: relative;
  display: flex;
  align-items: center;
}

.password-input input {
  flex: 1;
  padding-right: 40px;
}

.toggle-password {
  position: absolute;
  right: 12px;
  background: none;
  border: none;
  color: #666;
  cursor: pointer;
  padding: 4px;
  font-size: 16px;
  transition: color 0.3s;
}

.toggle-password:hover:not(:disabled) {
  color: #667eea;
}

.toggle-password:disabled {
  cursor: not-allowed;
  opacity: 0.5;
}

.error-message {
  color: #e53e3e;
  font-size: 12px;
}

label.has-error input {
  border-color: #e53e3e;
}

label.has-error input:focus {
  box-shadow: 0 0 0 3px rgba(229, 62, 62, 0.1);
}

.primary-button {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  padding: 14px 24px;
  border-radius: 8px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: transform 0.2s, box-shadow 0.2s;
  margin-top: 8px;
}

.primary-button:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(102, 126, 234, 0.4);
}

.primary-button:active:not(:disabled) {
  transform: translateY(0);
}

.primary-button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.alert {
  position: fixed;
  top: 20px;
  right: 20px;
  padding: 16px 20px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  gap: 12px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
  z-index: 1000;
  max-width: 400px;
  animation: slideIn 0.3s ease-out;
}

@keyframes slideIn {
  from {
    transform: translateX(400px);
    opacity: 0;
  }
  to {
    transform: translateX(0);
    opacity: 1;
  }
}

.alert-error {
  background: #fee2e2;
  color: #991b1b;
  border: 1px solid #fecaca;
}

.close-alert {
  background: none;
  border: none;
  color: inherit;
  cursor: pointer;
  padding: 4px;
  opacity: 0.6;
  transition: opacity 0.2s;
  margin-left: auto;
}

.close-alert:hover {
  opacity: 1;
}

@media (max-width: 640px) {
  .login-card {
    padding: 24px;
  }

  .login-brand h1 {
    font-size: 24px;
  }

  .alert {
    left: 20px;
    right: 20px;
    max-width: none;
  }
}
</style>
