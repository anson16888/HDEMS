<template>
  <a-modal
    :visible="true"
    title="确认删除用户"
    :confirm-loading="isDeleting"
    ok-text="确认删除"
    ok-type="danger"
    cancel-text="取消"
    @ok="handleDelete"
    @cancel="$emit('cancel')"
    width="500px"
  >
    <a-space direction="vertical" :size="16" style="width: 100%">
      <!-- Warning Icon -->
      <a-alert
        message="警告"
        :description="errorMessage || '此操作不可撤销，用户数据将被永久删除。'"
        :type="errorMessage ? 'error' : 'warning'"
        show-icon
      />

      <!-- User Info Card -->
      <a-card size="small">
        <a-descriptions :column="1" size="small">
          <a-descriptions-item label="姓名">
            {{ user.realName }}
          </a-descriptions-item>
          <a-descriptions-item label="账号">
            {{ user.username }}
          </a-descriptions-item>
          <a-descriptions-item label="角色">
            <a-space>
              <a-tag
                v-for="role in user.roles"
                :key="role"
                :color="getRoleColor(role)"
              >
                {{ getRoleDisplayName(role) }}
              </a-tag>
            </a-space>
          </a-descriptions-item>
        </a-descriptions>
      </a-card>

      <!-- Protection Warnings -->
      <a-alert
        v-if="isDeletingSelf"
        message="不能删除自己的账号"
        type="error"
        show-icon
      />

      <a-alert
        v-if="isLastSystemAdmin"
        message="不能删除唯一的系统管理员"
        type="error"
        show-icon
      />
    </a-space>
  </a-modal>
</template>

<script setup>
import { ref, computed } from 'vue'
import { message } from 'ant-design-vue'
import { useUserStore } from '@/stores/user.store'
import { useAuth } from '@/composables/useAuth'

const props = defineProps({
  user: {
    type: Object,
    required: true
  }
})

const emit = defineEmits(['confirm', 'cancel'])

const userStore = useUserStore()
const { user: currentUser } = useAuth()

const isDeleting = ref(false)
const errorMessage = ref('')

// Check if user is deleting themselves
const isDeletingSelf = computed(() => {
  return currentUser.value && props.user.id === currentUser.value.id
})

// Check if this is the last system admin
const isLastSystemAdmin = computed(() => {
  if (!props.user.roles.includes('SYSTEM_ADMIN')) {
    return false
  }
  const systemAdmins = userStore.users.filter(u =>
    u.roles.includes('SYSTEM_ADMIN') && u.status === 'ACTIVE'
  )
  return systemAdmins.length === 1
})

const handleDelete = async () => {
  // Security checks
  if (isDeletingSelf.value) {
    errorMessage.value = '不能删除自己的账号'
    message.error(errorMessage.value)
    return
  }

  if (isLastSystemAdmin.value) {
    errorMessage.value = '不能删除唯一的系统管理员'
    message.error(errorMessage.value)
    return
  }

  isDeleting.value = true
  errorMessage.value = ''

  try {
    await userStore.deleteUser(props.user.id)
    message.success('用户已删除')
    emit('confirm')
  } catch (error) {
    errorMessage.value = error.message || '删除失败'
    message.error(errorMessage.value)
  } finally {
    isDeleting.value = false
  }
}

const getRoleDisplayName = (role) => {
  const roleNames = {
    SYSTEM_ADMIN: '系统管理员',
    SCHEDULE_ADMIN: '值班管理员',
    MATERIAL_ADMIN: '物资管理员'
  }
  return roleNames[role] || role
}

const getRoleColor = (role) => {
  const colors = {
    SYSTEM_ADMIN: 'blue',
    SCHEDULE_ADMIN: 'green',
    MATERIAL_ADMIN: 'orange'
  }
  return colors[role] || 'default'
}
</script>
