<template>
  <div class="person-title-form">
    <a-form
      :model="formData"
      :rules="rules"
      ref="formRef"
      :label-col="{ span: 6 }"
      :wrapper-col="{ span: 16 }"
    >
      <a-form-item label="职称编码" name="titleCode">
        <a-input
          v-model:value="formData.titleCode"
          placeholder="请输入职称编码"
          :disabled="!!title"
        />
      </a-form-item>

      <a-form-item label="职称名称" name="titleName">
        <a-input
          v-model:value="formData.titleName"
          placeholder="请输入职称名称"
        />
      </a-form-item>

      <a-form-item label="职称级别" name="titleLevel">
        <a-select
          v-model:value="formData.titleLevel"
          placeholder="请选择职称级别"
          :options="titleLevelOptions"
        />
      </a-form-item>

      <a-form-item label="排序号" name="sortOrder">
        <a-input-number
          v-model:value="formData.sortOrder"
          :min="0"
          :max="9999"
          style="width: 100%"
        />
      </a-form-item>

      <a-form-item :wrapper-col="{ span: 16, offset: 6 }">
        <a-space>
          <a-button type="primary" @click="handleSubmit" :loading="submitting">
            {{ title ? '更新' : '创建' }}
          </a-button>
          <a-button @click="handleCancel">取消</a-button>
        </a-space>
      </a-form-item>
    </a-form>
  </div>
</template>

<script setup>
import { ref, reactive, watch } from 'vue'
import { message } from 'ant-design-vue'

const props = defineProps({
  title: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(['submit', 'cancel'])

const formRef = ref()
const submitting = ref(false)

const formData = reactive({
  titleCode: '',
  titleName: '',
  titleLevel: '初级',
  sortOrder: 0
})

// 职称级别选项
const titleLevelOptions = [
  { label: '正高', value: '正高' },
  { label: '副高', value: '副高' },
  { label: '中级', value: '中级' },
  { label: '初级', value: '初级' }
]

const rules = {
  titleCode: [
    { required: true, message: '请输入职称编码', trigger: 'blur' }
  ],
  titleName: [
    { required: true, message: '请输入职称名称', trigger: 'blur' }
  ],
  titleLevel: [
    { required: true, message: '请选择职称级别', trigger: 'change' }
  ]
}

// Watch for title prop changes to populate form
watch(() => props.title, (newTitle) => {
  if (newTitle) {
    Object.assign(formData, {
      titleCode: newTitle.titleCode || '',
      titleName: newTitle.titleName || '',
      titleLevel: newTitle.titleLevel || '初级',
      sortOrder: newTitle.sortOrder || 0
    })
  } else {
    resetForm()
  }
}, { immediate: true })

async function handleSubmit() {
  try {
    await formRef.value.validate()
    emit('submit', { ...formData })
  } catch (error) {
    message.error('请检查表单填写是否正确')
  }
}

function handleCancel() {
  emit('cancel')
}

function resetForm() {
  Object.assign(formData, {
    titleCode: '',
    titleName: '',
    titleLevel: '初级',
    sortOrder: 0
  })
  formRef.value?.resetFields()
}
</script>

<style scoped>
.person-title-form {
  max-width: 800px;
  padding: 20px 0;
}
</style>
