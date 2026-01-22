<template>
  <div class="user-management-page">
    <!-- Page Header -->
    <a-page-header title="用户管理" sub-title="管理系统用户和权限" />

    <!-- Search and Action Card -->
    <a-card class="search-card" :bordered="false">
      <a-form layout="inline">
        <a-row :gutter="16" style="width: 100%">
          <a-col :span="6">
            <a-form-item label="关键字">
              <a-input
                v-model:value="searchKeyword"
                placeholder="搜索姓名或账号"
                allow-clear
                @change="handleSearch"
              >
                <template #prefix>
                  <SearchOutlined />
                </template>
              </a-input>
            </a-form-item>
          </a-col>
          <a-col :span="5">
            <a-form-item label="角色">
              <a-select
                v-model:value="roleFilter"
                placeholder="选择角色"
                allow-clear
                style="width: 100%"
                @change="handleRoleFilter"
              >
                <a-select-option value="">全部</a-select-option>
                <a-select-option value="SYSTEM_ADMIN">系统管理员</a-select-option>
                <a-select-option value="DUTY_ADMIN">值班管理员</a-select-option>
                <a-select-option value="MATERIAL_ADMIN">物资管理员</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :span="5">
            <a-form-item label="状态">
              <a-select
                v-model:value="statusFilter"
                placeholder="选择状态"
                allow-clear
                style="width: 100%"
                @change="handleStatusFilter"
              >
                <a-select-option value="">全部</a-select-option>
                <a-select-option value="ACTIVE">启用</a-select-option>
                <a-select-option value="INACTIVE">禁用</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :span="8">
            <a-form-item :wrapper-col="{ span: 24 }">
              <a-space>
                <a-button type="primary" @click="handleSearch">
                  <template #icon><SearchOutlined /></template>
                  查询
                </a-button>
                <a-button @click="resetFilters">重置</a-button>
                <a-button type="primary" @click="openCreateModal">
                  <template #icon><PlusOutlined /></template>
                  新增用户
                </a-button>
              </a-space>
            </a-form-item>
          </a-col>
        </a-row>
      </a-form>
    </a-card>

    <!-- User Table -->
    <a-card :bordered="false" class="table-card">
      <a-spin :spinning="userStore.loading">
        <a-empty
          v-if="paginatedUsers.length === 0 && !userStore.loading"
          :description="hasActiveFilters ? '没有找到匹配的用户' : '暂无用户数据'"
        >
          <a-button type="primary" v-if="!hasActiveFilters" @click="openCreateModal">
            创建第一个用户
          </a-button>
        </a-empty>

        <a-table
          v-else
          :columns="columns"
          :data-source="paginatedUsers"
          :pagination="false"
          :row-key="(record) => record.id"
          :scroll="{ x: 1200 }"
        >
          <!-- 姓名 -->
          <template #bodyCell="{ column, record }">
            <template v-if="column.key === 'realName'">
              <a-space>
                <a-avatar :size="32" style="background-color: #1890ff;">
                  {{ record.realName?.substring(0, 1) || '用' }}
                </a-avatar>
                <span>{{ record.realName }}</span>
              </a-space>
            </template>

            <!-- 角色 -->
            <template v-else-if="column.key === 'roles'">
              <a-tag v-for="role in record.roles" :key="role" :color="getRoleColor(role)">
                {{ getRoleDisplayName(role) }}
              </a-tag>
            </template>

            <!-- 状态 -->
            <template v-else-if="column.key === 'status'">
              <a-badge
                :status="record.status === 'ACTIVE' ? 'success' : 'error'"
                :text="getStatusDisplayName(record.status)"
              />
            </template>

            <!-- 操作 -->
            <template v-else-if="column.key === 'actions'">
              <a-space>
                <a-tooltip title="查看详情">
                  <a-button type="text" size="small" @click="openUserDetail(record)">
                    <template #icon><EyeOutlined /></template>
                  </a-button>
                </a-tooltip>

                <a-tooltip title="编辑">
                  <a-button type="text" size="small" @click="openEditModal(record)">
                    <template #icon><EditOutlined /></template>
                  </a-button>
                </a-tooltip>

                <a-tooltip v-if="record.login_attempts >= 5" title="解锁账号">
                  <a-button type="text" size="small" danger @click="unlockAccount(record)">
                    <template #icon><UnlockOutlined /></template>
                  </a-button>
                </a-tooltip>

                <a-tooltip title="重置密码">
                  <a-button type="text" size="small" @click="openPasswordReset(record)">
                    <template #icon><KeyOutlined /></template>
                  </a-button>
                </a-tooltip>

                <a-popconfirm
                  title="确定要删除该用户吗？此操作不可撤销。"
                  ok-text="确认"
                  cancel-text="取消"
                  @confirm="handleDeleteUser(record)"
                >
                  <a-tooltip title="删除">
                    <a-button type="text" size="small" danger>
                      <template #icon><DeleteOutlined /></template>
                    </a-button>
                  </a-tooltip>
                </a-popconfirm>
              </a-space>
            </template>
          </template>
        </a-table>

        <!-- Pagination -->
        <div v-if="filteredUsers.length > 0" class="pagination-wrapper">
          <a-pagination
            v-model:current="currentPage"
            v-model:page-size="pageSize"
            :page-size-options="['10', '20', '50', '100']"
            :total="filteredUsers.length"
            :show-total="(total) => `共 ${total} 条`"
            show-size-changer
            show-quick-jumper
            @change="handlePageChange"
          />
        </div>
      </a-spin>
    </a-card>

    <!-- Modals -->
    <UserFormModal
      v-if="modalState.showUserForm"
      :mode="modalState.mode"
      :user="modalState.user"
      @save="handleSaveUser"
      @cancel="closeModals"
    />

    <PasswordResetModal
      v-if="modalState.showPasswordReset"
      :user="modalState.user"
      @confirm="handlePasswordResetResult"
      @cancel="closeModals"
    />

    <DeleteConfirmModal
      v-if="modalState.showDeleteConfirm"
      :user="modalState.user"
      @confirm="handleDeleteUser"
      @cancel="closeModals"
    />

    <UserDetailModal
      v-if="modalState.showUserDetail"
      :user="modalState.user"
      @edit="openEditModal"
      @delete="openDeleteConfirm"
      @close="closeModals"
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted, reactive } from 'vue'
import { message } from 'ant-design-vue'
import { useUserStore } from '@/stores/user.store'
import { storeToRefs } from 'pinia'
import {
  PlusOutlined,
  SearchOutlined,
  EyeOutlined,
  EditOutlined,
  DeleteOutlined,
  UnlockOutlined,
  KeyOutlined
} from '@ant-design/icons-vue'

// Components
import UserFormModal from '@/components/modals/UserFormModal.vue'
import PasswordResetModal from '@/components/modals/PasswordResetModal.vue'
import DeleteConfirmModal from '@/components/modals/DeleteConfirmModal.vue'
import UserDetailModal from '@/components/modals/UserDetailModal.vue'

const userStore = useUserStore()
const { users, loading, error } = storeToRefs(userStore)

// State
const searchKeyword = ref('')
const roleFilter = ref(undefined)
const statusFilter = ref(undefined)
const currentPage = ref(1)
const pageSize = ref(20)

const modalState = reactive({
  showUserForm: false,
  showPasswordReset: false,
  showDeleteConfirm: false,
  showUserDetail: false,
  mode: 'create',
  user: null
})

// Table columns
const columns = [
  {
    title: '姓名',
    key: 'realName',
    width: 200,
    fixed: 'left'
  },
  {
    title: '账号',
    dataIndex: 'username',
    key: 'username',
    width: 150
  },
  {
    title: '科室',
    dataIndex: 'department',
    key: 'department',
    width: 150,
    customRender: ({ text }) => text || '-'
  },
  {
    title: '角色',
    key: 'roles',
    width: 250
  },
  {
    title: '状态',
    key: 'status',
    width: 120
  },
  {
    title: '操作',
    key: 'actions',
    fixed: 'right',
    width: 200,
    align: 'center'
  }
]

// Computed
const filteredUsers = computed(() => {
  let result = users.value || []

  if (searchKeyword.value) {
    const keyword = searchKeyword.value.toLowerCase()
    result = result.filter(user =>
      user.realName.toLowerCase().includes(keyword) ||
      user.username.toLowerCase().includes(keyword)
    )
  }

  if (roleFilter.value) {
    result = result.filter(user => user.roles.includes(roleFilter.value))
  }

  if (statusFilter.value) {
    result = result.filter(user => user.status === statusFilter.value)
  }

  return result
})

const paginatedUsers = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  const end = start + pageSize.value
  return filteredUsers.value.slice(start, end)
})

const hasActiveFilters = computed(() => {
  return searchKeyword.value || roleFilter.value || statusFilter.value
})

// Methods
const handleSearch = () => {
  currentPage.value = 1
}

const handleRoleFilter = () => {
  currentPage.value = 1
}

const handleStatusFilter = () => {
  currentPage.value = 1
}

const resetFilters = () => {
  searchKeyword.value = ''
  roleFilter.value = undefined
  statusFilter.value = undefined
  currentPage.value = 1
}

const handlePageChange = () => {
  // Handle pagination
}

const openCreateModal = () => {
  modalState.showUserForm = true
  modalState.mode = 'create'
  modalState.user = null
}

const openEditModal = (user) => {
  modalState.showUserForm = true
  modalState.mode = 'edit'
  modalState.user = { ...user }
}

const openDeleteConfirm = (user) => {
  modalState.showDeleteConfirm = true
  modalState.user = user
}

const openPasswordReset = (user) => {
  modalState.showPasswordReset = true
  modalState.user = user
}

const openUserDetail = (user) => {
  modalState.showUserDetail = true
  modalState.user = user
}

const closeModals = () => {
  modalState.showUserForm = false
  modalState.showPasswordReset = false
  modalState.showDeleteConfirm = false
  modalState.showUserDetail = false
  modalState.user = null
}

const handleSaveUser = async () => {
  closeModals()
  await userStore.fetchUsers()
}

const handleDeleteUser = async (user) => {
  try {
    await userStore.deleteUser(user.id)
    message.success('用户已删除')
    closeModals()
    await userStore.fetchUsers()
  } catch (error) {
    message.error(error.message || '删除失败')
  }
}

const handlePasswordResetResult = () => {
  closeModals()
}

const unlockAccount = async (user) => {
  try {
    await userStore.unlockAccount(user.id)
    message.success('账号已解锁')
    await userStore.fetchUsers()
  } catch (error) {
    message.error(error.message || '解锁失败')
  }
}

const getRoleDisplayName = (role) => {
  const roleNames = {
    SYSTEM_ADMIN: '系统管理员',
    DUTY_ADMIN: '值班管理员',
    MATERIAL_ADMIN: '物资管理员'
  }
  return roleNames[role] || role
}

const getRoleColor = (role) => {
  const colors = {
    SYSTEM_ADMIN: 'blue',
    DUTY_ADMIN: 'green',
    MATERIAL_ADMIN: 'orange'
  }
  return colors[role] || 'default'
}

const getStatusDisplayName = (status) => {
  return status === 'ACTIVE' ? '启用' : '禁用'
}

// Error handling
if (error.value) {
  message.error(error.value)
}

// Lifecycle
onMounted(() => {
  userStore.fetchUsers()
})
</script>

<style scoped>
.user-management-page {
  padding: 24px;
  /* background-color: #f0f2f5; */
}

.search-card {
  margin-bottom: 16px;
}

.table-card {
  margin-bottom: 16px;
}

.pagination-wrapper {
  display: flex;
  justify-content: flex-end;
  margin-top: 16px;
}
</style>
