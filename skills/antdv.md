# Ant Design Vue (AntDV) Frontend Development Skill

帮助你在这个 Vue 3 项目中使用 Ant Design Vue 组件库进行前端开发。

> 基于 ant-design-vue@4.2.6 | Vue 3.2+ | Vite

## 项目信息

- **当前安装版本**: ant-design-vue@4.2.6
- **Vue 版本要求**: >= 3.2.0
- **构建工具**: Vite
- **状态管理**: Pinia
- **路由**: Vue Router 4
- **图标库**: @ant-design/icons-vue (包含 1000+ 图标)

## 完整组件列表

### 基础组件 (Basic)
- **Button** 按钮 - 支持多种类型和样式
- **Icon** 图标 - 从 @ant-design/icons-vue 导入
- **Typography** 排版 - 文本、标题、段落样式
- **Space** 间距 - 统一控制子元素间距

### 布局组件 (Layout)
- **Layout** 布局容器
- **Grid** 栅格系统 (Row/Col)
- **Divider** 分割线
- **Flex** 弹性布局

### 导航组件 (Navigation)
- **Affix** 固钉
- **Breadcrumb** 面包屑
- **Dropdown** 下拉菜单
- **Menu** 导航菜单
- **Pagination** 分页
- **Steps** 步骤条

### 数据录入组件 (Data Entry)
- **AutoComplete** 自动完成
- **Cascader** 级联选择
- **Checkbox** 复选框
- **DatePicker** 日期选择器
- **Form** 表单
- **Input** 输入框
- **InputNumber** 数字输入框
- **Mentions** 提及
- **Radio** 单选框
- **Rate** 评分
- **Select** 选择器
- **Slider** 滑动输入条
- **Switch** 开关
- **TimePicker** 时间选择
- **Transfer** 穿梭框
- **TreeSelect** 树选择
- **Upload** 上传

### 数据展示组件 (Data Display)
- **Avatar** 头像
- **Badge** 徽标数
- **Calendar** 日历
- **Card** 卡片
- **Carousel** 走马灯
- **Collapse** 折叠面板
- **Descriptions** 描述列表
- **Empty** 空状态
- **Image** 图片
- **List** 列表
- **Popover** 气泡卡片
- **QRCode** 二维码
- **Segmented** 分段控制器
- **Statistic** 统计数值
- **Table** 表格
- **Tabs** 标签页
- **Tag** 标签
- **Timeline** 时间轴
- **Tooltip** 文字提示
- **Tree** 树形控件

### 反馈组件 (Feedback)
- **Alert** 警告提示
- **Drawer** 抽屉
- **Modal** 对话框
- **Message** 全局提示
- **Notification** 通知提示框
- **Popconfirm** 气泡确认框
- **Progress** 进度条
- **Result** 结果
- **Spin** 加载中

### 其他组件 (Other)
- **Anchor** 锚点
- **App** 应用包装组件
- **ConfigProvider** 全局化配置
- **FloatButton** 悬浮按钮
- **LocaleProvider** 国际化配置
- **Theme** 主题配置
- **Watermark** 水印
- **Tour** 漫游式引导

### 虚拟组件 (Virtual Components - 内部使用)
- vc-align, vc-cascader, vc-checkbox, vc-dialog, vc-drawer, vc-dropdown
- vc-image, vc-input, vc-mentions, vc-notification, vc-overflow
- vc-pagination, vc-picker, vc-progress, vc-resize-observer
- vc-select, vc-slick, vc-slider, vc-steps, vc-table, vc-tooltip
- vc-tour, vc-tree, vc-tree-select, vc-trigger, vc-upload
- vc-util, vc-virtual-list

## 安装与配置

### 检查安装状态

```bash
# 查看已安装的 ant-design-vue 版本
npm list ant-design-vue
```

当前项目已安装: ant-design-vue@4.2.6

### 方式 1: 完整引入（适合快速开发）

在 `frontend/src/main.js` 中：

```javascript
import { createApp } from 'vue'
import App from './App.vue'
import Antd from 'ant-design-vue'
import 'ant-design-vue/dist/reset.css'

const app = createApp(App)
app.use(Antd)
app.mount('#app')
```

### 方式 2: 按需引入（推荐生产环境）

1. 安装依赖：

```bash
cd frontend
npm install -D unplugin-vue-components unplugin-auto-import
```

2. 配置 `vite.config.js`：

```javascript
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { fileURLToPath, URL } from 'node:url'
import AutoImport from 'unplugin-auto-import/vite'
import Components from 'unplugin-vue-components/vite'
import { AntDesignVueResolver } from 'unplugin-vue-components/resolvers'

export default defineConfig({
  plugins: [
    vue(),
    AutoImport({
      imports: ['vue', 'vue-router', 'pinia'],
      dts: 'src/auto-imports.d.ts',
    }),
    Components({
      resolvers: [
        AntDesignVueResolver({
          importStyle: false, // css in js
        }),
      ],
      dts: 'src/components.d.ts',
    }),
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  }
})
```

3. 在 `src/main.js` 中移除完整引入，改为：

```javascript
import { createApp } from 'vue'
import App from './App.vue'

const app = createApp(App)
app.mount('#app')
```

## 常用组件使用指南

### 1. Button 按钮

```vue
<template>
  <a-space>
    <a-button type="primary">主要按钮</a-button>
    <a-button>默认按钮</a-button>
    <a-button type="dashed">虚线按钮</a-button>
    <a-button type="link">链接按钮</a-button>
    <a-button danger>危险按钮</a-button>
    <a-button :loading="loading" @click="handleClick">
      加载中
    </a-button>
    <a-button disabled>禁用</a-button>
  </a-space>
</template>

<script setup>
import { ref } from 'vue'

const loading = ref(false)
const handleClick = () => {
  loading.value = true
  setTimeout(() => {
    loading.value = false
  }, 2000)
}
</script>
```

按钮类型：
- `type="primary"` - 主要按钮
- `type="default"` - 默认按钮
- `type="dashed"` - 虚线按钮
- `type="link"` - 链接按钮
- `type="text"` - 文本按钮
- `danger` - 危险按钮（红色）
- `ghost` - 幽灵按钮（透明背景）
- `disabled` - 禁用状态
- `:loading` - 加载中状态

### 2. Table 表格

替换项目中的原生 table：

```vue
<template>
  <div>
    <a-table
      :columns="columns"
      :data-source="dataSource"
      :loading="loading"
      :pagination="pagination"
      :row-key="record => record.id"
      @change="handleTableChange"
    >
      <!-- 自定义列内容 -->
      <template #bodyCell="{ column, record }">
        <template v-if="column.key === 'status'">
          <a-tag :color="record.status === 'ACTIVE' ? 'green' : 'red'">
            {{ record.status === 'ACTIVE' ? '启用' : '禁用' }}
          </a-tag>
        </template>

        <template v-else-if="column.key === 'roles'">
          <a-tag v-for="role in record.roles" :key="role" color="blue">
            {{ getRoleDisplayName(role) }}
          </a-tag>
        </template>

        <template v-else-if="column.key === 'action'">
          <a-space>
            <a-button type="link" size="small" @click="handleView(record)">
              查看
            </a-button>
            <a-button type="link" size="small" @click="handleEdit(record)">
              编辑
            </a-button>
            <a-popconfirm
              title="确定要删除吗?"
              ok-text="确定"
              cancel-text="取消"
              @confirm="handleDelete(record)"
            >
              <a-button type="link" size="small" danger>删除</a-button>
            </a-popconfirm>
          </a-space>
        </template>
      </template>

      <!-- 自定义分页显示 -->
      <template #footer>
        <a-pagination
          v-model:current="pagination.current"
          v-model:pageSize="pagination.pageSize"
          :total="pagination.total"
          :show-size-changer="true"
          :show-total="(total) => `共 ${total} 条`"
        />
      </template>
    </a-table>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useUserStore } from '@/stores/user.store'

const userStore = useUserStore()

const columns = [
  { title: '姓名', dataIndex: 'real_name', key: 'real_name', width: 120 },
  { title: '账号', dataIndex: 'username', key: 'username', width: 120 },
  { title: '科室', dataIndex: 'department', key: 'department' },
  { title: '角色', key: 'roles', width: 200 },
  { title: '状态', key: 'status', width: 100 },
  { title: '操作', key: 'action', width: 200, fixed: 'right' }
]

const dataSource = ref([])
const loading = ref(false)

const pagination = reactive({
  current: 1,
  pageSize: 20,
  total: 0,
  showSizeChanger: true,
  showQuickJumper: true,
  showTotal: (total) => `共 ${total} 条`
})

const handleTableChange = (pag, filters, sorter) => {
  pagination.current = pag.current
  pagination.pageSize = pag.pageSize
  fetchData()
}

const fetchData = async () => {
  loading.value = true
  try {
    await userStore.fetchUsers({
      page: pagination.current,
      pageSize: pagination.pageSize
    })
    dataSource.value = userStore.users
    pagination.total = userStore.total
  } finally {
    loading.value = false
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

const handleView = (record) => { console.log('查看', record) }
const handleEdit = (record) => { console.log('编辑', record) }
const handleDelete = async (record) => {
  await userStore.deleteUser(record.id)
  fetchData()
}

onMounted(() => {
  fetchData()
})
</script>
```

### 3. Form 表单

替换项目中的表单模态框：

```vue
<template>
  <a-modal
    v-model:open="visible"
    :title="mode === 'create' ? '创建用户' : '编辑用户'"
    :width="600"
    :confirm-loading="isSubmitting"
    @ok="handleSubmit"
    @cancel="handleCancel"
  >
    <a-form
      ref="formRef"
      :model="formData"
      :rules="rules"
      :label-col="{ span: 6 }"
      :wrapper-col="{ span: 16 }"
    >
      <a-form-item label="账号" name="username">
        <a-input
          v-model:value="formData.username"
          placeholder="请输入账号（至少6个字符）"
          :disabled="mode === 'edit'"
        />
      </a-form-item>

      <a-form-item v-if="mode === 'create'" label="密码" name="password">
        <a-input-password
          v-model:value="formData.password"
          placeholder="请输入密码（至少8位，包含字母和数字）"
        />
      </a-form-item>

      <a-form-item label="姓名" name="real_name">
        <a-input
          v-model:value="formData.real_name"
          placeholder="请输入真实姓名"
        />
      </a-form-item>

      <a-form-item label="手机号" name="phone">
        <a-input
          v-model:value="formData.phone"
          placeholder="请输入11位手机号"
        />
      </a-form-item>

      <a-form-item label="科室" name="department">
        <a-input
          v-model:value="formData.department"
          placeholder="请输入科室名称（可选）"
        />
      </a-form-item>

      <a-form-item label="角色" name="roles">
        <a-checkbox-group v-model:value="formData.roles">
          <a-checkbox value="SYSTEM_ADMIN">系统管理员</a-checkbox>
          <a-checkbox value="DUTY_ADMIN">值班管理员</a-checkbox>
          <a-checkbox value="MATERIAL_ADMIN">物资管理员</a-checkbox>
        </a-checkbox-group>
      </a-form-item>

      <a-form-item v-if="mode === 'edit'" label="状态" name="status">
        <a-radio-group v-model:value="formData.status">
          <a-radio value="ACTIVE">启用</a-radio>
          <a-radio value="INACTIVE">禁用</a-radio>
        </a-radio-group>
      </a-form-item>
    </a-form>
  </a-modal>
</template>

<script setup>
import { ref, reactive, watch } from 'vue'

const props = defineProps({
  modelValue: Boolean,
  mode: {
    type: String,
    validator: (value) => ['create', 'edit'].includes(value)
  },
  user: Object
})

const emit = defineEmits(['update:modelValue', 'save', 'cancel'])

const visible = ref(false)
const formRef = ref()
const isSubmitting = ref(false)

const formData = reactive({
  username: '',
  password: '',
  real_name: '',
  phone: '',
  department: '',
  roles: [],
  status: 'ACTIVE'
})

const rules = {
  username: [
    { required: true, message: '账号不能为空', trigger: 'blur' },
    { min: 6, message: '账号至少6个字符', trigger: 'blur' },
    {
      pattern: /^[a-zA-Z0-9_]+$/,
      message: '账号只能包含字母、数字和下划线',
      trigger: 'blur'
    }
  ],
  password: [
    { required: true, message: '密码不能为空', trigger: 'blur' },
    { min: 8, message: '密码至少8个字符', trigger: 'blur' },
    {
      pattern: /^(?=.*[A-Za-z])(?=.*\d)/,
      message: '密码必须包含字母和数字',
      trigger: 'blur'
    }
  ],
  real_name: [
    { required: true, message: '姓名不能为空', trigger: 'blur' },
    { min: 2, message: '姓名至少2个字符', trigger: 'blur' }
  ],
  phone: [
    { required: true, message: '手机号不能为空', trigger: 'blur' },
    {
      pattern: /^1\d{10}$/,
      message: '请输入正确的11位手机号',
      trigger: 'blur'
    }
  ],
  roles: [
    {
      required: true,
      type: 'array',
      min: 1,
      message: '请至少选择一个角色',
      trigger: 'change'
    }
  ]
}

// 监听 modelValue 变化
watch(() => props.modelValue, (newVal) => {
  visible.value = newVal

  if (newVal && props.mode === 'edit' && props.user) {
    Object.assign(formData, {
      username: props.user.username,
      real_name: props.user.real_name,
      phone: props.user.phone,
      department: props.user.department || '',
      roles: [...props.user.roles],
      status: props.user.status
    })
  } else if (newVal && props.mode === 'create') {
    Object.assign(formData, {
      username: '',
      password: '',
      real_name: '',
      phone: '',
      department: '',
      roles: [],
      status: 'ACTIVE'
    })
  }
})

watch(visible, (newVal) => {
  emit('update:modelValue', newVal)
})

const handleSubmit = async () => {
  try {
    await formRef.value.validate()
    isSubmitting.value = true

    // 调用保存逻辑
    emit('save', { ...formData })

    visible.value = false
    formRef.value.resetFields()
  } catch (error) {
    console.log('验证失败:', error)
  } finally {
    isSubmitting.value = false
  }
}

const handleCancel = () => {
  visible.value = false
  formRef.value.resetFields()
  emit('cancel')
}
</script>
```

### 4. Input 和 Search

```vue
<template>
  <a-space direction="vertical" style="width: 100%">
    <!-- 基础输入框 -->
    <a-input
      v-model:value="inputValue"
      placeholder="请输入内容"
      allow-clear
    />

    <!-- 搜索框 -->
    <a-input-search
      v-model:value="searchKeyword"
      placeholder="搜索姓名或账号..."
      enter-button="搜索"
      size="large"
      @search="handleSearch"
      @change="handleSearchChange"
    />

    <!-- 带前后缀的输入框 -->
    <a-input-group compact>
      <a-select
        v-model:value="filterType"
        style="width: 120px"
      >
        <a-select-option value="name">姓名</a-select-option>
        <a-select-option value="phone">手机号</a-select-option>
      </a-select>
      <a-input
        v-model:value="filterValue"
        style="width: calc(100% - 120px)"
        placeholder="请输入搜索内容"
      />
    </a-input-group>

    <!-- 文本域 -->
    <a-textarea
      v-model:value="description"
      placeholder="请输入描述"
      :rows="4"
      :maxlength="200"
      show-count
    />
  </a-space>
</template>

<script setup>
import { ref } from 'vue'

const inputValue = ref('')
const searchKeyword = ref('')
const filterType = ref('name')
const filterValue = ref('')
const description = ref('')

const handleSearch = (value) => {
  console.log('搜索:', value)
}

const handleSearchChange = (e) => {
  console.log('搜索内容变化:', e.target.value)
}
</script>
```

### 5. Select 选择器

```vue
<template>
  <a-space direction="vertical" style="width: 100%">
    <!-- 单选 -->
    <a-select
      v-model:value="roleValue"
      placeholder="请选择角色"
      style="width: 200px"
      :options="roleOptions"
      allow-clear
    />

    <!-- 多选 -->
    <a-select
      v-model:value="rolesValue"
      mode="multiple"
      placeholder="请选择多个角色"
      style="width: 100%"
      :options="roleOptions"
      :max-tag-count="2"
    />

    <!-- 带搜索的选择器 -->
    <a-select
      v-model:value="userValue"
      show-search
      placeholder="搜索用户"
      style="width: 100%"
      :filter-option="filterUser"
      :options="userOptions"
      :field-names="{ label: 'real_name', value: 'id' }"
    />

    <!-- 分组选择器 -->
    <a-select
      v-model:value="departmentValue"
      placeholder="请选择科室"
      style="width: 200px"
    >
      <a-select-opt-group label="内科">
        <a-select-option value="cardiology">心内科</a-select-option>
        <a-select-option value="neurology">神经内科</a-select-option>
      </a-select-opt-group>
      <a-select-opt-group label="外科">
        <a-select-option value="orthopedics">骨科</a-select-option>
        <a-select-option value="general">普外科</a-select-option>
      </a-select-opt-group>
    </a-select>
  </a-space>
</template>

<script setup>
import { ref } from 'vue'

const roleValue = ref()
const rolesValue = ref([])
const userValue = ref()
const departmentValue = ref()

const roleOptions = [
  { value: 'SYSTEM_ADMIN', label: '系统管理员' },
  { value: 'DUTY_ADMIN', label: '值班管理员' },
  { value: 'MATERIAL_ADMIN', label: '物资管理员' }
]

const userOptions = [
  { id: 1, real_name: '张三', username: 'zhangsan' },
  { id: 2, real_name: '李四', username: 'lisi' },
  { id: 3, real_name: '王五', username: 'wangwu' }
]

const filterUser = (input, option) => {
  return option.real_name.toLowerCase().includes(input.toLowerCase()) ||
         option.username.toLowerCase().includes(input.toLowerCase())
}
</script>
```

### 6. Message 消息提示

替换 `useToast` composable：

```javascript
import { message } from 'ant-design-vue'

// 成功提示
message.success('操作成功')
message.success('用户创建成功', 3) // 3秒后自动关闭

// 错误提示
message.error('操作失败')
message.error('网络错误，请稍后重试')

// 警告提示
message.warning('请注意')
message.warning('密码即将过期')

// 信息提示
message.info('加载中...')
message.info('数据已更新')

// 加载中（需手动关闭）
const hide = message.loading('加载中...', 0)
setTimeout(() => {
  hide()
}, 2000)

// Promise 用法
message.loading('正在提交...', 2.5)
  .then(() => {
    message.success('提交成功', 2.5)
  })
```

### 7. Modal 对话框

```vue
<template>
  <div>
    <!-- 基础用法 -->
    <a-button type="primary" @click="showModal">打开对话框</a-button>

    <a-modal
      v-model:open="visible"
      title="对话框标题"
      @ok="handleOk"
      @cancel="handleCancel"
      :ok-text="'确定'"
      :cancel-text="'取消'"
    >
      <p>对话框内容...</p>
    </a-modal>

    <!-- 确认对话框 -->
    <a-popconfirm
      title="确定要删除这条数据吗?"
      ok-text="确定"
      cancel-text="取消"
      @confirm="handleDelete"
    >
      <a-button danger>删除</a-button>
    </a-popconfirm>

    <!-- 异步确认对话框 -->
    <a-popconfirm
      title="确定要执行此操作吗?"
      ok-text="确定"
      cancel-text="取消"
      :confirm-loading="confirmLoading"
      @confirm="handleAsyncConfirm"
    >
      <a-button>异步操作</a-button>
    </a-popconfirm>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { Modal } from 'ant-design-vue'

const visible = ref(false)
const confirmLoading = ref(false)

const showModal = () => {
  visible.value = true
}

const handleOk = () => {
  visible.value = false
}

const handleCancel = () => {
  visible.value = false
}

const handleDelete = () => {
  console.log('删除操作')
}

const handleAsyncConfirm = () => {
  confirmLoading.value = true
  setTimeout(() => {
    confirmLoading.value = false
    console.log('异步操作完成')
  }, 2000)
}

// 方法调用方式
const showConfirm = () => {
  Modal.confirm({
    title: '确认',
    content: '确定要执行此操作吗?',
    okText: '确定',
    cancelText: '取消',
    onOk() {
      console.log('确认')
    },
    onCancel() {
      console.log('取消')
    }
  })
}
</script>
```

### 8. Layout 布局

替换 `DefaultLayout.vue`：

```vue
<template>
  <a-layout style="min-height: 100vh">
    <!-- 顶部导航 -->
    <a-layout-header style="background: #001529; padding: 0 24px; display: flex; align-items: center;">
      <div style="color: white; font-size: 20px; font-weight: bold;">
        医院器械应急管理系统
      </div>
      <a-space style="margin-left: auto;">
        <a-avatar :size="32" style="background-color: #1890ff;">
          <template #icon>
            <UserOutlined />
          </template>
        </a-avatar>
        <a-dropdown>
          <a class="ant-dropdown-link" style="color: white;" @click.prevent>
            {{ currentUser?.username || '用户' }}
            <DownOutlined />
          </a>
          <template #overlay>
            <a-menu>
              <a-menu-item key="profile">
                <UserOutlined />
                个人中心
              </a-menu-item>
              <a-menu-divider />
              <a-menu-item key="logout" @click="handleLogout">
                <LogoutOutlined />
                退出登录
              </a-menu-item>
            </a-menu>
          </template>
        </a-dropdown>
      </a-space>
    </a-layout-header>

    <a-layout>
      <!-- 侧边栏 -->
      <a-layout-sider
        :width="200"
        style="background: #fff"
        :collapsed="collapsed"
        :trigger="null"
        collapsible
      >
        <a-menu
          v-model:selectedKeys="selectedKeys"
          v-model:openKeys="openKeys"
          mode="inline"
          style="height: 100%; border-right: 0"
        >
          <a-menu-item key="overview">
            <router-link to="/schedule/overview">
              <DashboardOutlined />
              <span>总览</span>
            </router-link>
          </a-menu-item>

          <a-sub-menu key="schedule">
            <template #icon>
              <CalendarOutlined />
            </template>
            <template #title>排班管理</template>
            <a-menu-item key="hospital">
              <router-link to="/schedule/hospital">医院排班</router-link>
            </a-menu-item>
            <a-menu-item key="bureau">
              <router-link to="/schedule/bureau">科室排班</router-link>
            </a-menu-item>
            <a-menu-item key="director">
              <router-link to="/schedule/director">主任排班</router-link>
            </a-menu-item>
          </a-sub-menu>

          <a-menu-item key="materials">
            <router-link to="/materials">
              <InboxOutlined />
              <span>物资管理</span>
            </router-link>
          </a-menu-item>

          <a-menu-item key="users">
            <router-link to="/users">
              <TeamOutlined />
              <span>用户管理</span>
            </router-link>
          </a-menu-item>

          <a-menu-item key="settings">
            <router-link to="/settings">
              <SettingOutlined />
              <span>系统设置</span>
            </router-link>
          </a-menu-item>
        </a-menu>
      </a-layout-sider>

      <!-- 主内容区 -->
      <a-layout-content style="margin: 24px 16px; padding: 24px; background: #fff; min-height: 280px;">
        <router-view v-slot="{ Component }">
          <transition name="fade" mode="out-in">
            <component :is="Component" />
          </transition>
        </router-view>
      </a-layout-content>
    </a-layout>
  </a-layout>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '@/composables/useAuth'
import {
  UserOutlined,
  DownOutlined,
  LogoutOutlined,
  DashboardOutlined,
  CalendarOutlined,
  InboxOutlined,
  TeamOutlined,
  SettingOutlined
} from '@ant-design/icons-vue'

const router = useRouter()
const { user: currentUser, logout } = useAuth()

const collapsed = ref(false)
const selectedKeys = ref(['overview'])
const openKeys = ref(['schedule'])

const handleLogout = async () => {
  await logout()
  router.push('/login')
}
</script>

<style scoped>
.fade-enter-active, .fade-leave-active {
  transition: opacity 0.2s ease;
}

.fade-enter-from, .fade-leave-to {
  opacity: 0;
}

.ant-dropdown-link {
  cursor: pointer;
}
</style>
```

### 9. Tag 标签

```vue
<template>
  <a-space>
    <!-- 基础标签 -->
    <a-tag>默认标签</a-tag>
    <a-tag color="blue">蓝色</a-tag>
    <a-tag color="green">绿色</a-tag>
    <a-tag color="gold">金色</a-tag>
    <a-tag color="red">红色</a-tag>

    <!-- 可关闭的标签 -->
    <a-tag closable @close="handleClose">
      可关闭标签
    </a-tag>

    <!-- 带图标的标签 -->
    <a-tag color="success">
      <CheckCircleOutlined />
      成功
    </a-tag>

    <!-- 状态标签 -->
    <a-tag v-if="status === 'ACTIVE'" color="success">启用</a-tag>
    <a-tag v-else color="error">禁用</a-tag>

    <!-- 角色标签 -->
    <a-tag
      v-for="role in roles"
      :key="role"
      :color="getRoleColor(role)"
    >
      {{ getRoleLabel(role) }}
    </a-tag>
  </a-space>
</template>

<script setup>
import { ref } from 'vue'
import { CheckCircleOutlined } from '@ant-design/icons-vue'

const status = ref('ACTIVE')
const roles = ref(['SYSTEM_ADMIN', 'DUTY_ADMIN'])

const handleClose = () => {
  console.log('标签关闭')
}

const getRoleColor = (role) => {
  const colors = {
    SYSTEM_ADMIN: 'blue',
    DUTY_ADMIN: 'green',
    MATERIAL_ADMIN: 'orange'
  }
  return colors[role] || 'default'
}

const getRoleLabel = (role) => {
  const labels = {
    SYSTEM_ADMIN: '系统管理员',
    DUTY_ADMIN: '值班管理员',
    MATERIAL_ADMIN: '物资管理员'
  }
  return labels[role] || role
}
</script>
```

### 10. Empty 空状态

```vue
<template>
  <a-space direction="vertical" style="width: 100%" size="large">
    <!-- 基础空状态 -->
    <a-empty description="暂无数据" />

    <!-- 带操作的空状态 -->
    <a-empty description="还没有用户">
      <a-button type="primary" @click="handleCreate">
        创建用户
      </a-button>
    </a-empty>

    <!-- 自定义图片 -->
    <a-empty
      :image="Empty.PRESENTED_IMAGE_SIMPLE"
      description="自定义图片"
    />

    <!-- 无网络 -->
    <a-empty description="网络连接失败">
      <a-button type="primary" @click="handleRetry">
        重试
      </a-button>
    </a-empty>
  </a-space>
</template>

<script setup>
import { Empty } from 'ant-design-vue'

const handleCreate = () => {
  console.log('创建用户')
}

const handleRetry = () => {
  console.log('重试')
}
</script>
```

### 11. Spin 加载中

```vue
<template>
  <!-- 包裹内容 -->
  <a-spin :spinning="loading" tip="加载中...">
    <div>
      内容区域...
    </div>
  </a-spin>

  <!-- 整页加载 -->
  <a-spin :spinning="pageLoading" tip="加载中..." size="large">
    <a-alert
      message="提示"
      description="内容区域..."
      type="info"
    />
  </a-spin>
</template>

<script setup>
import { ref } from 'vue'

const loading = ref(false)
const pageLoading = ref(false)
</script>
```

### 12. DatePicker 日期选择器

```vue
<template>
  <a-space direction="vertical" style="width: 100%">
    <!-- 日期选择 -->
    <a-date-picker
      v-model:value="dateValue"
      placeholder="选择日期"
      format="YYYY-MM-DD"
      :disabled-date="disabledDate"
    />

    <!-- 日期范围选择 -->
    <a-range-picker
      v-model:value="rangeValue"
      format="YYYY-MM-DD"
      :placeholder="['开始日期', '结束日期']"
    />

    <!-- 月份选择 -->
    <a-month-picker
      v-model:value="monthValue"
      placeholder="选择月份"
      format="YYYY-MM"
    />
  </a-space>
</template>

<script setup>
import { ref } from 'vue'
import dayjs from 'dayjs'

const dateValue = ref()
const rangeValue = ref()
const monthValue = ref()

const disabledDate = (current) => {
  // 禁用今天之后的日期
  return current && current > dayjs().endOf('day')
}
</script>
```

### 13. Upload 上传

```vue
<template>
  <a-upload
    v-model:file-list="fileList"
    name="file"
    :action="uploadUrl"
    :headers="uploadHeaders"
    :before-upload="beforeUpload"
    @change="handleChange"
  >
    <a-button>
      <UploadOutlined />
      点击上传
    </a-button>
  </a-upload>
</template>

<script setup>
import { ref } from 'vue'
import { UploadOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'

const fileList = ref([])
const uploadUrl = ref('/api/upload')
const uploadHeaders = ref({
  Authorization: `Bearer ${localStorage.getItem('token')}`
})

const beforeUpload = (file) => {
  const isJpgOrPng = file.type === 'image/jpeg' || file.type === 'image/png'
  if (!isJpgOrPng) {
    message.error('只能上传 JPG/PNG 格式的文件!')
  }
  const isLt2M = file.size / 1024 / 1024 < 2
  if (!isLt2M) {
    message.error('图片大小不能超过 2MB!')
  }
  return isJpgOrPng && isLt2M
}

const handleChange = (info) => {
  if (info.file.status === 'done') {
    message.success(`${info.file.name} 上传成功`)
  } else if (info.file.status === 'error') {
    message.error(`${info.file.name} 上传失败`)
  }
}
</script>
```

## 图标使用

ant-design-vue@4.2.6 使用 @ant-design/icons-vue

### 导入图标

```vue
<template>
  <a-space>
    <UserOutlined style="font-size: 24px; color: #1890ff;" />
    <LockOutlined />
    <SearchOutlined />
    <EditOutlined />
    <DeleteOutlined />
    <PlusOutlined />
  </a-space>
</template>

<script setup>
import {
  UserOutlined,
  LockOutlined,
  SearchOutlined,
  EditOutlined,
  DeleteOutlined,
  PlusOutlined
} from '@ant-design/icons-vue'
</script>
```

### 常用图标列表

#### 用户相关
- `UserOutlined` - 用户
- `UserAddOutlined` - 添加用户
- `UserDeleteOutlined` - 删除用户
- `TeamOutlined` - 团队
- `LoginOutlined` - 登录
- `LogoutOutlined` - 登出

#### 操作相关
- `EditOutlined` - 编辑
- `DeleteOutlined` - 删除
- `PlusOutlined` - 添加
- `CloseOutlined` - 关闭
- `CheckOutlined` - 确认
- `SearchOutlined` - 搜索
- `ReloadOutlined` - 刷新
- `SaveOutlined` - 保存

#### 导航相关
- `HomeOutlined` - 首页
- `SettingOutlined` - 设置
- `DashboardOutlined` - 仪表盘
- `MenuOutlined` - 菜单
- `DownOutlined` - 下拉箭头
- `UpOutlined` - 上拉箭头
- `LeftOutlined` - 左箭头
- `RightOutlined` - 右箭头

#### 状态相关
- `CheckCircleOutlined` - 成功
- `CloseCircleOutlined` - 失败
- `ExclamationCircleOutlined` - 警告
- `InfoCircleOutlined` - 信息
- `LoadingOutlined` - 加载中

#### 文件相关
- `FileOutlined` - 文件
- `FolderOutlined` - 文件夹
- `DownloadOutlined` - 下载
- `UploadOutlined` - 上传

#### 时间相关
- `CalendarOutlined` - 日历
- `ClockCircleOutlined` - 时钟
- `HistoryOutlined` - 历史

更多图标请访问：https://antdv.com/components/icon-cn

## 国际化配置

```javascript
// src/main.js
import { ConfigProvider } from 'ant-design-vue'
import zhCN from 'ant-design-vue/es/locale/zh_CN'
import dayjs from 'dayjs'
import 'dayjs/locale/zh-cn'

dayjs.locale('zh-cn')

app.use(ConfigProvider)
```

在组件中使用：

```vue
<template>
  <a-config-provider :locale="zhCN">
    <App />
  </a-config-provider>
</template>

<script setup>
import zhCN from 'ant-design-vue/es/locale/zh_CN'
</script>
```

## 主题定制

### 方式 1: 使用 ConfigProvider

```vue
<template>
  <a-config-provider
    :theme="{
      token: {
        colorPrimary: '#2196F3',
        colorSuccess: '#52c41a',
        colorWarning: '#faad14',
        colorError: '#f5222d',
        fontSize: 14,
        borderRadius: 6
      }
    }"
  >
    <App />
  </a-config-provider>
</template>
```

### 方式 2: 使用 CSS 变量

```css
:root {
  --ant-primary-color: #2196F3;
  --ant-success-color: #52c41a;
  --ant-warning-color: #faad14;
  --ant-error-color: #f5222d;
}
```

## 与现有代码集成

### 1. 渐进式替换策略

优先替换顺序：
1. **表格** - `a-table` 功能更强大
2. **表单** - `a-form` 验证更完善
3. **模态框** - `a-modal` 交互更友好
4. **按钮** - `a-button` 样式更统一
5. **消息提示** - `message` 替换 `useToast`
6. **布局** - `a-layout` 替换手动布局

保留原生的组件：
- 简单的展示型组件
- 自定义样式要求高的组件

### 2. 样式适配

Ant Design Vue 默认主题色为蓝色（#1890ff），项目主题色为 #2196F3，需要统一：

```vue
<template>
  <a-config-provider
    :theme="{
      token: {
        colorPrimary: '#2196F3'
      }
    }"
  >
    <router-view />
  </a-config-provider>
</template>
```

### 3. 响应式设计

使用 Grid 系统：

```vue
<template>
  <a-row :gutter="[16, 16]">
    <a-col :xs="24" :sm="12" :md="8" :lg="6">
      <!-- 移动端 100%，平板 50%，桌面 25% -->
    </a-col>
  </a-row>
</template>
```

断点：
- xs: <576px
- sm: ≥576px
- md: ≥768px
- lg: ≥992px
- xl: ≥1200px
- xxl: ≥1600px

### 4. 与 Pinia 集成

```vue
<template>
  <a-table
    :columns="columns"
    :data-source="userStore.users"
    :loading="userStore.loading"
    :pagination="{
      current: userStore.currentPage,
      pageSize: userStore.pageSize,
      total: userStore.total
    }"
    @change="handleTableChange"
  />
</template>

<script setup>
import { useUserStore } from '@/stores/user.store'

const userStore = useUserStore()

const handleTableChange = (pagination) => {
  userStore.fetchUsers({
    page: pagination.current,
    pageSize: pagination.pageSize
  })
}
</script>
```

## 最佳实践

### 1. 组件命名规范

```vue
<!-- 推荐：使用 a- 前缀 -->
<a-button type="primary">保存</a-button>
<a-table :columns="columns" :data-source="data" />

<!-- 避免 -->
<AntdButton type="primary">保存</AntdButton>
```

### 2. 事件处理

```vue
<template>
  <a-button @click="handleClick">点击</a-button>
  <a-table @change="handleTableChange" />
</template>

<script setup>
const handleClick = (e) => {
  console.log('点击事件', e)
}

const handleTableChange = (pagination, filters, sorter) => {
  console.log('分页变化', pagination)
  console.log('筛选变化', filters)
  console.log('排序变化', sorter)
}
</script>
```

### 3. 表单验证最佳实践

```javascript
const rules = {
  username: [
    { required: true, message: '账号不能为空', trigger: 'blur' },
    { min: 6, max: 20, message: '账号长度在 6 到 20 个字符', trigger: 'blur' },
    {
      pattern: /^[a-zA-Z0-9_]+$/,
      message: '账号只能包含字母、数字和下划线',
      trigger: 'blur'
    }
  ],
  email: [
    { required: true, message: '邮箱不能为空', trigger: 'blur' },
    {
      type: 'email',
      message: '请输入正确的邮箱地址',
      trigger: ['blur', 'change']
    }
  ]
}
```

### 4. 性能优化

- 大数据表格使用虚拟滚动
- 使用 `v-if` 而不是 `v-show` 来减少初始渲染
- 合理使用 `computed` 缓存计算结果
- 按需引入组件减小打包体积

## 常见问题解决

### Q1: 组件样式不生效

```vue
<style scoped>
/* 使用 :deep() 修改组件内部样式 */
:deep(.ant-btn-primary) {
  background: #2196F3;
  border-color: #2196F3;
}
</style>
```

### Q2: 日期显示为英文

```javascript
import 'dayjs/locale/zh-cn'
import dayjs from 'dayjs'

dayjs.locale('zh-cn')
```

### Q3: 表单重置不生效

```vue
<script setup>
const formRef = ref()

const resetForm = () => {
  formRef.value.resetFields() // 重置为初始值
}
</script>
```

### Q4: Table 分页不更新

```javascript
// 确保 pagination 是响应式对象
const pagination = reactive({
  current: 1,
  pageSize: 20,
  total: 0
})

// 更新时直接修改
pagination.current = newPage
```

## 参考资源

- **官方文档**: https://antdv.com/
- **中文文档**: https://www.antdv.com/
- **组件列表**: https://www.antdv.com/components/overview-cn
- **GitHub**: https://github.com/vueComponent/ant-design-vue
- **设计规范**: https://ant.design/docs/spec/values-cn
- **更新日志**: https://github.com/vueComponent/ant-design-vue/releases

## 快速参考

### 安装命令

```bash
# 当前已安装: ant-design-vue@4.2.6
# 如需重新安装
npm install ant-design-vue@4.2.6

# 安装图标库（通常已包含）
npm install @ant-design/icons-vue

# 按需引入插件
npm install -D unplugin-vue-components unplugin-auto-import
```

### 项目配置文件

- **入口文件**: `frontend/src/main.js`
- **Vite 配置**: `frontend/vite.config.js`
- **当前版本**: ant-design-vue@4.2.6

---

使用此 skill 可以快速在项目中集成和使用 Ant Design Vue，提升开发效率和 UI 质量。
