<template>
  <a-modal
    :visible="true"
    :title="mode === 'create' ? '创建用户' : '编辑用户'"
    :confirm-loading="isSubmitting"
    @ok="handleSubmit"
    @cancel="handleCancel"
    width="600px"
    :ok-text="isSubmitting ? '保存中...' : '保存'"
    cancel-text="取消"
  >
    <a-form
      ref="formRef"
      :model="formData"
      :rules="rules"
      :label-col="{ span: 6 }"
      :wrapper-col="{ span: 18 }"
      @finish="handleSubmit"
    >
      <!-- Username -->
      <a-form-item label="账号" name="username">
        <a-input
          v-model:value="formData.username"
          :disabled="mode === 'edit'"
          placeholder="请输入账号"
        />
        <template v-if="mode === 'edit'" #extra>
          <a-typography-text type="secondary">账号不可修改</a-typography-text>
        </template>
      </a-form-item>

      <!-- Password (create mode only) -->
      <a-form-item v-if="mode === 'create'" label="密码" name="password">
        <a-input-password
          v-model:value="formData.password"
          placeholder="请输入密码（至少8位，包含字母和数字）"
        />
      </a-form-item>

      <!-- Real Name -->
      <a-form-item label="姓名" name="realName">
        <a-input
          v-model:value="formData.realName"
          placeholder="请输入真实姓名"
        />
      </a-form-item>

      <!-- Phone -->
      <a-form-item label="手机号" name="phone">
        <a-input
          v-model:value="formData.phone"
          placeholder="请输入11位手机号"
          :max-length="11"
        />
      </a-form-item>

      <!-- Department -->
      <a-form-item label="科室" name="department">
        <a-input
          v-model:value="formData.department"
          placeholder="请输入科室名称（可选）"
        />
      </a-form-item>

      <!-- Roles -->
      <a-form-item label="角色" name="roles">
        <a-checkbox-group v-model:value="formData.roles">
          <a-checkbox :value="'SYSTEM_ADMIN'">系统管理员</a-checkbox>
          <a-checkbox :value="'SCHEDULE_ADMIN'">值班管理员</a-checkbox>
          <a-checkbox :value="'MATERIAL_ADMIN'">物资管理员</a-checkbox>
        </a-checkbox-group>
      </a-form-item>

      <!-- Status (edit mode only) -->
      <a-form-item v-if="mode === 'edit'" label="状态" name="status">
        <a-radio-group v-model:value="formData.status">
          <a-radio :value="1">正常</a-radio>
          <a-radio :value="2">停用</a-radio>
          <a-radio :value="3">锁定</a-radio>
        </a-radio-group>
      </a-form-item>
    </a-form>
  </a-modal>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue'
import { message } from 'ant-design-vue'
import { useUserStore } from '@/stores/user.store'
import { useAuth } from '@/composables/useAuth'

const props = defineProps({
  mode: {
    type: String,
    required: true,
    validator: (value) => ['create', 'edit'].includes(value)
  },
  user: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(['save', 'cancel'])

const userStore = useUserStore()
const { user: currentUser } = useAuth()
const formRef = ref()

const formData = reactive({
  username: '',
  password: '',
  realName: '',
  phone: '',
  department: '',
  roles: [],
  status: 1  // Default to ACTIVE (1)
})

const isSubmitting = ref(false)

// Validation rules
const rules = computed(() => ({
  username: props.mode === 'create' ? [
    { required: true, message: '请输入账号', trigger: 'blur' },
    {
      pattern: /^[a-zA-Z0-9_]+$/,
      message: '账号只能包含字母、数字和下划线',
      trigger: 'blur'
    },
    {
      validator: async (_, value) => {
        const existing = userStore.users.find(u => u.username === value)
        if (existing) {
          return Promise.reject(new Error('该账号已存在'))
        }
        return Promise.resolve()
      },
      trigger: 'blur'
    }
  ] : [],
  password: props.mode === 'create' ? [
    { required: true, message: '请输入密码', trigger: 'blur' },
    { min: 8, message: '密码至少8个字符', trigger: 'blur' },
    {
      pattern: /^(?=.*[A-Za-z])(?=.*\d)/,
      message: '密码必须包含字母和数字',
      trigger: 'blur'
    }
  ] : [],
  realName: [
    { required: true, message: '请输入姓名', trigger: 'blur' },
    { min: 2, message: '姓名至少2个字符', trigger: 'blur' }
  ],
  phone: [
    { required: true, message: '请输入手机号', trigger: 'blur' },
    {
      pattern: /^1\d{10}$/,
      message: '请输入正确的11位手机号',
      trigger: 'blur'
    }
  ],
  department: [
    { max: 100, message: '科室名称最多100个字符', trigger: 'blur' }
  ],
  roles: [
    {
      type: 'array',
      required: true,
      message: '请至少选择一个角色',
      trigger: 'change'
    },
    {
      validator: async (_, value) => {
        if (props.mode === 'edit' && props.user && currentUser.value && props.user.id === currentUser.value.id) {
          const originalRoles = props.user.roles || []
          const newRoles = value || []
          if (originalRoles.sort().toString() !== newRoles.sort().toString()) {
            return Promise.reject(new Error('不能修改自己的角色'))
          }
        }
        return Promise.resolve()
      },
      trigger: 'change'
    }
  ],
  status: [
    {
      validator: async (_, value) => {
        if (props.mode === 'edit' && props.user && currentUser.value && props.user.id === currentUser.value.id) {
          if (value === 2 && props.user.status !== 2) {
            return Promise.reject(new Error('不能禁用自己的账号'))
          }
        }
        return Promise.resolve()
      },
      trigger: 'change'
    }
  ]
}))

onMounted(() => {
  if (props.mode === 'edit' && props.user) {
    Object.assign(formData, {
      username: props.user.username,
      realName: props.user.realName,
      phone: props.user.phone,
      department: props.user.department || '',
      roles: [...props.user.roles],  // Roles are already numbers from backend
      status: props.user.status      // Status is already a number from backend
    })
  }
})

const handleSubmit = async () => {
  try {
    await formRef.value.validate()
  } catch (error) {
    message.warning('请检查表单输入')
    return
  }

  isSubmitting.value = true

  try {
    if (props.mode === 'create') {
      await userStore.createUser(formData)
      message.success('用户创建成功')
    } else {
      // Remove username from updates when editing (it should not be changed)
      const { username, ...updates } = formData
      await userStore.updateUser(props.user.id, updates)
      message.success('用户信息已更新')
    }
    emit('save')
  } catch (error) {
    message.error(error.message || '操作失败')
  } finally {
    isSubmitting.value = false
  }
}

const handleCancel = () => {
  emit('cancel')
}
</script>
