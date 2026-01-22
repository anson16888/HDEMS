<template>
  <div class="shift-form">
    <h3>{{ shift ? '编辑班次信息' : '新增班次' }}</h3>
    <Form
      :model="formData"
      :rules="rules"
      ref="formRef"
      :label-col="{ span: 6 }"
      :wrapper-col="{ span: 14 }"
    >
      <FormItem label="班次编码" name="shiftCode">
        <Input
          v-model:value="formData.shiftCode"
          placeholder="请输入班次编码"
          :disabled="!!shift"
        />
      </FormItem>

      <FormItem label="班次名称" name="shiftName">
        <Input
          v-model:value="formData.shiftName"
          placeholder="请输入班次名称"
        />
      </FormItem>

      <FormItem label="时间段" name="timeRange">
        <Input
          v-model:value="formData.timeRange"
          placeholder="例如: 08:00-12:00"
        />
        <div class="form-tip">格式示例: 08:00-12:00 或 全天</div>
      </FormItem>

      <FormItem label="排序号" name="sortOrder">
        <InputNumber
          v-model:value="formData.sortOrder"
          :min="0"
          :max="9999"
          style="width: 100%"
        />
      </FormItem>

      <FormItem :wrapper-col="{ span: 14, offset: 6 }">
        <Space>
          <Button type="primary" @click="handleSubmit" :loading="submitting">
            {{ shift ? '更新' : '创建' }}
          </Button>
          <Button @click="handleCancel">取消</Button>
        </Space>
      </FormItem>
    </Form>
  </div>
</template>

<script setup>
import { ref, reactive, watch } from 'vue'
import { message, Form, FormItem, Input, InputNumber, Button, Space } from 'ant-design-vue'

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
