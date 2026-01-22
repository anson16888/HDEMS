<template>
  <div class="material-type-form">
    <Form
      :model="formData"
      :rules="rules"
      ref="formRef"
      :label-col="{ span: 6 }"
      :wrapper-col="{ span: 14 }"
    >
      <FormItem label="类型编码" name="typeCode">
        <Input
          v-model:value="formData.typeCode"
          placeholder="请输入类型编码"
          :disabled="!!materialType"
        />
      </FormItem>

      <FormItem label="类型名称" name="typeName">
        <Input
          v-model:value="formData.typeName"
          placeholder="请输入类型名称"
        />
      </FormItem>

      <FormItem label="颜色" name="color">
        <Input
          v-model:value="formData.color"
          placeholder="例如 #1890ff 或 blue"
        />
      </FormItem>

      <FormItem label="排序号" name="sortOrder">
        <InputNumber
          v-model:value="formData.sortOrder"
          :min="0"
          :max="9999"
          style="width: 100%"
        />
      </FormItem>

      <FormItem label="状态" name="isEnabled">
        <RadioGroup v-model:value="formData.isEnabled">
          <Radio :value="true">启用</Radio>
          <Radio :value="false">停用</Radio>
        </RadioGroup>
      </FormItem>

      <FormItem label="备注" name="remark">
        <Input.TextArea
          v-model:value="formData.remark"
          :rows="3"
          placeholder="补充说明"
        />
      </FormItem>

      <FormItem :wrapper-col="{ span: 14, offset: 6 }">
        <Space>
          <Button type="primary" @click="handleSubmit" :loading="submitting">
            {{ materialType ? '更新' : '创建' }}
          </Button>
          <Button @click="handleCancel">取消</Button>
        </Space>
      </FormItem>
    </Form>
  </div>
</template>

<script setup>
import { ref, reactive, watch } from 'vue'
import { message, Form, FormItem, Input, InputNumber, RadioGroup, Radio, Button, Space } from 'ant-design-vue'

const props = defineProps({
  materialType: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(['submit', 'cancel'])

const formRef = ref()
const submitting = ref(false)

const formData = reactive({
  typeCode: '',
  typeName: '',
  color: '',
  sortOrder: 0,
  isEnabled: true,
  remark: ''
})

const rules = {
  typeCode: [
    { required: true, message: '请输入类型编码', trigger: 'blur' }
  ],
  typeName: [
    { required: true, message: '请输入类型名称', trigger: 'blur' }
  ]
}

watch(() => props.materialType, (newType) => {
  if (newType) {
    Object.assign(formData, {
      typeCode: newType.typeCode || '',
      typeName: newType.typeName || '',
      color: newType.color || '',
      sortOrder: newType.sortOrder || 0,
      isEnabled: newType.isEnabled ?? true,
      remark: newType.remark || ''
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
    typeCode: '',
    typeName: '',
    color: '',
    sortOrder: 0,
    isEnabled: true,
    remark: ''
  })
  formRef.value?.resetFields()
}
</script>

<style scoped>
.material-type-form {
  max-width: 800px;
  padding: 20px 0;
}
</style>
