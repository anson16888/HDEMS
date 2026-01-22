<template>
  <div class="shift-form">
    <h3>{{ shift ? '编辑班次信息' : '新增班次' }}</h3>
    <a-form
      :model="formData"
      :rules="rules"
      ref="formRef"
      :label-col="{ span: 6 }"
      :wrapper-col="{ span: 14 }"
    >
      <a-form-item label="班次编码" name="shiftCode">
        <a-input
          v-model:value="formData.shiftCode"
          placeholder="请输入班次编码"
          :disabled="!!shift"
        />
      </a-form-item>

      <a-form-item label="班次名称" name="shiftName">
        <a-input
          v-model:value="formData.shiftName"
          placeholder="请输入班次名称"
        />
      </a-form-item>

      <a-form-item label="时间段" name="timeRange">
        <a-input
          v-model:value="formData.timeRange"
          placeholder="例如: 08:00-12:00"
        />
        <div class="form-tip">格式示例: 08:00-12:00 或 全天</div>
      </a-form-item>

      <a-form-item label="排序号" name="sortOrder">
        <a-input-number
          v-model:value="formData.sortOrder"
          :min="0"
          :max="9999"
          style="width: 100%"
        />
      </a-form-item>

      <a-form-item :wrapper-col="{ span: 14, offset: 6 }">
        <a-space>
          <a-button type="primary" @click="handleSubmit" :loading="submitting">
            {{ shift ? '更新' : '创建' }}
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
  shift: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(['submit', 'cancel'])

const formRef = ref()
const submitting = ref(false)

const formData = reactive({
  shiftCode: '',
  shiftName: '',
  timeRange: '',
  sortOrder: 0
})

const rules = {
  shiftCode: [
    { required: true, message: '请输入班次编码', trigger: 'blur' }
  ],
  shiftName: [
    { required: true, message: '请输入班次名称', trigger: 'blur' }
  ],
  timeRange: [
    { required: true, message: '请输入时间段', trigger: 'blur' }
  ]
}

// Watch for shift prop changes to populate form
watch(() => props.shift, (newShift) => {
  if (newShift) {
    Object.assign(formData, {
      shiftCode: newShift.shiftCode || '',
      shiftName: newShift.shiftName || '',
      timeRange: newShift.timeRange || '',
      sortOrder: newShift.sortOrder || 0
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
    shiftCode: '',
    shiftName: '',
    timeRange: '',
    sortOrder: 0
  })
  formRef.value?.resetFields()
}
</script>

<style scoped>
.shift-form {
  max-width: 800px;
  padding: 20px;
}

.shift-form h3 {
  margin-bottom: 20px;
  font-size: 18px;
  font-weight: 600;
}

.form-tip {
  font-size: 12px;
  color: #999;
  margin-top: 4px;
}
</style>
