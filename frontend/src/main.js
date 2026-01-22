import { createApp } from 'vue'
import { createPinia } from 'pinia'
import Antd from 'ant-design-vue'
import 'ant-design-vue/dist/reset.css'
import './style.css'
import App from './App.vue'
import router from './router'
import { useAuthStore } from './stores/auth.store.js'

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)
app.use(router)
app.use(Antd)

// Initialize auth session
async function initializeApp() {
  try {
    // Initialize auth store (this will check localStorage and sync state)
    const authStore = useAuthStore()
    await authStore.initialize()

    console.log('✅ HDEMS Authentication initialized')
  } catch (error) {
    console.error('❌ Error initializing app:', error)
  }
}

// Mount app and initialize
app.mount('#app')
initializeApp()
