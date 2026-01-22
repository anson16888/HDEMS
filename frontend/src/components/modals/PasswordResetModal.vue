<template>
  <a-modal
    :visible="true"
    title="重置密码"
    :confirm-loading="isSubmitting"
    :ok-text="!isSuccess ? '确认重置' : '关闭'"
    :cancel-text="!isSuccess ? '取消' : undefined"
    @ok="handleReset"
    @cancel="$emit('cancel')"
    width="500px"
    :closable="true"
  >
    <a-space direction="vertical" :size="16" style="width: 100%">
      <!-- User Info -->
      <a-alert :message="`用户：${user.realName} (${user.username})`" type="info" show-icon />

      <!-- Password Input Form (if not reset yet) -->
      <div v-if="!isSuccess">
        <a-form ref="formRef" :model="formData" :rules="rules" layout="vertical">
          <a-form-item label="新密码" name="password">
            <a-input-password
              v-model:value="formData.password"
              placeholder="请输入新密码（至少8位，包含字母和数字）"
              :visibility-toggle="true"
            />
          </a-form-item>

          <a-form-item label="确认密码" name="confirmPassword">
            <a-input-password
              v-model:value="formData.confirmPassword"
              placeholder="请再次输入新密码"
              :visibility-toggle="true"
            />
          </a-form-item>
        </a-form>

        <a-alert
          message="提示"
          type="warning"
          show-icon
        >
          <template #description>
            <p>重置密码后，该用户将被强制退出登录，需要使用新密码重新登录。</p>
          </template>
        </a-alert>
      </div>

      <!-- Success State -->
      <div v-else>
        <a-result
          status="success"
          title="密码重置成功"
          sub-title="新密码已设置"
        >
          <template #extra>
            <a-space direction="vertical" :size="12" style="width: 100%">
              <a-typography-text strong>新密码：</a-typography-text>
              <a-input
                :value="displayPassword"
                :type="showPassword ? 'text' : 'password'"
                readonly
                size="large"
                style="font-family: monospace; letter-spacing: 2px;"
              >
                <template #suffix>
                  <a-button
                    type="text"
                    :icon="showPassword ? h(EyeInvisibleOutlined) : h(EyeOutlined)"
                    @click="showPassword = !showPassword"
                  />
                  <a-divider type="vertical" />
                  <a-tooltip title="复制密码">
                    <a-button
                      type="text"
                      :icon="h(CopyOutlined)"
                      @click="copyPassword"
                    />
                  </a-tooltip>
                </template>
              </a-input>

              <a-alert
                message="提示"
                description="该用户将被强制退出登录，需要使用新密码重新登录。"
                type="info"
                show-icon
              />
            </a-space>
          </template>
        </a-result>
      </div>
    </a-space>
  </a-modal>
</template>

<script setup>
import { ref, reactive, h } from 'vue'
import { message } from 'ant-design-vue'
import { EyeOutlined, EyeInvisibleOutlined, CopyOutlined } from '@ant-design/icons-vue'
import { useUserStore } from '@/stores/user.store'

const props = defineProps({
  user: {
    type: Object,
    required: true
  }
})

const emit = defineEmits(['confirm', 'cancel'])

const userStore = useUserStore()
const formRef = ref()
const isSuccess = ref(false)
const displayPassword = ref('')
const showPassword = ref(false)
const isSubmitting = ref(false)

const formData = reactive({
  password: '',
  confirmPassword: ''
})

const validatePassword = async (_, value) => {
  if (!value) {
    return Promise.reject(new Error('请输入新密码'))
  }
  if (value.length < 8) {
    return Promise.reject(new Error('密码至少8个字符'))
  }
  if (!/(?=.*[A-Za-z])(?=.*\d)/.test(value)) {
    return Promise.reject(new Error('密码必须包含字母和数字'))
  }
  return Promise.resolve()
}

const validateConfirmPassword = async (_, value) => {
  if (!value) {
    return Promise.reject(new Error('请再次输入新密码'))
  }
  if (value !== formData.password) {
    return Promise.reject(new Error('两次输入的密码不一致'))
  }
  return Promise.resolve()
}

const rules = {
  password: [
    { required: true, validator: validatePassword, trigger: 'blur' }
  ],
  confirmPassword: [
    { required: true, validator: validateConfirmPassword, trigger: 'blur' }
  ]
}

const handleReset = async () => {
  if (isSuccess.value) {
    emit('cancel')
    return
  }

  try {
    await formRef.value.validate()
  } catch (error) {
    message.warning('请检查表单输入')
    return
  }

  isSubmitting.value = true

  try {
    const result = await userStore.resetPassword(props.user.id, formData.password)
    displayPassword.value = formData.password
    isSuccess.value = true
    message.success('密码重置成功')
    emit('confirm')
  } catch (error) {
    message.error(error.message || '密码重置失败')
  } finally {
    isSubmitting.value = false
  }
}

const copyPassword = async () => {
  try {
    await navigator.clipboard.writeText(displayPassword.value)
    message.success('密码已复制到剪贴板')
  } catch (err) {
    message.error('复制失败，请手动复制')
  }
}
</script>
