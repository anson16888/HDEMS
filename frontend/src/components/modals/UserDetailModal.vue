<template>
  <a-modal
    :visible="true"
    title="用户详情"
    width="700px"
    :footer="null"
    @cancel="$emit('close')"
  >
    <a-descriptions bordered :column="2">
      <!-- 基本信息 -->
      <a-descriptions-item label="用户ID" :span="2">
        {{ user.id }}
      </a-descriptions-item>

      <a-descriptions-item label="账号">
        {{ user.username }}
      </a-descriptions-item>

      <a-descriptions-item label="姓名">
        {{ user.realName }}
      </a-descriptions-item>

      <a-descriptions-item label="手机号">
        {{ user.phone }}
      </a-descriptions-item>

      <a-descriptions-item label="科室">
        {{ user.department || '-' }}
      </a-descriptions-item>

      <a-descriptions-item label="医院">
        {{ user.hospitalName || '-' }}
      </a-descriptions-item>

      <!-- 权限信息 -->
      <a-descriptions-item label="角色" :span="2">
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

      <a-descriptions-item label="状态">
        <a-badge
          :status="getStatusBadgeStatus(user.status)"
          :text="getStatusDisplayName(user.status)"
        />
      </a-descriptions-item>

      <a-descriptions-item label="登录失败次数">
        {{ user.login_attempts || 0 }}
      </a-descriptions-item>

      <!-- 时间信息 -->
      <a-descriptions-item label="创建时间">
        {{ formatDate(user.created_at) }}
      </a-descriptions-item>

      <a-descriptions-item label="更新时间">
        {{ formatDate(user.updated_at) }}
      </a-descriptions-item>

      <a-descriptions-item label="最后登录" :span="2">
        {{ user.last_login ? formatDate(user.last_login) : '从未登录' }}
      </a-descriptions-item>
    </a-descriptions>

    <!-- Actions -->
    <div class="action-buttons">
      <a-space>
        <a-button @click="$emit('close')">关闭</a-button>
        <a-button type="primary" @click="$emit('edit', user)">编辑</a-button>
        <a-button danger @click="handleDelete">删除</a-button>
      </a-space>
    </div>
  </a-modal>
</template>

<script setup>
import { defineProps, defineEmits } from 'vue'
import { Modal } from 'ant-design-vue'

const props = defineProps({
  user: {
    type: Object,
    required: true
  }
})

const emit = defineEmits(['close', 'edit', 'delete'])

const getRoleDisplayName = (role) => {
  const roleNames = {
    1: '系统管理员',
    2: '值班管理员',
    3: '物资管理员'
  }
  return roleNames[role] || role
}

const getRoleColor = (role) => {
  const colors = {
    1: 'blue',
    2: 'green',
    3: 'orange'
  }
  return colors[role] || 'default'
}

const getStatusDisplayName = (status) => {
  const statusNames = {
    1: '正常',
    2: '停用',
    3: '锁定'
  }
  return statusNames[status] || status
}

const getStatusBadgeStatus = (status) => {
  const badgeStatus = {
    1: 'success',
    2: 'error',
    3: 'warning'
  }
  return badgeStatus[status] || 'default'
}

const formatDate = (dateString) => {
  if (!dateString) return '-'
  const date = new Date(dateString)
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  const hours = String(date.getHours()).padStart(2, '0')
  const minutes = String(date.getMinutes()).padStart(2, '0')
  const seconds = String(date.getSeconds()).padStart(2, '0')
  return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`
}

const handleDelete = () => {
  Modal.confirm({
    title: '确定要删除该用户吗？',
    content: '此操作不可撤销',
    okText: '确认',
    cancelText: '取消',
    onOk: () => {
      emit('delete', props.user)
    }
  })
}
</script>

<style scoped>
.action-buttons {
  margin-top: 24px;
  text-align: right;
}
</style>
