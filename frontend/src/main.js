import { createApp } from 'vue'
import { createPinia } from 'pinia'
import Antd from 'ant-design-vue'
import 'ant-design-vue/dist/reset.css'
import './style.css'
import App from './App.vue'
import router from './router'
import { userStore } from './mock/user-store.js'
import { authService } from './services/auth.service.js'

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)
app.use(router)
app.use(Antd)

// Initialize Mock data and auth
async function initializeApp() {
  try {
    // Initialize user store with seed data
    await userStore.initialize()

    // Check for existing auth session
    await authService.initializeSession()

    console.log('✅ HDEMS Mock Authentication initialized')
  } catch (error) {
    console.error('❌ Error initializing app:', error)
  }
}

// Mount app and initialize
app.mount('#app')
initializeApp()
