<template>
  <div class="material-threshold-form">
    <Form
      :model="formData"
      :rules="rules"
      ref="formRef"
      :label-col="{ span: 6 }"
      :wrapper-col="{ span: 14 }"
    >
      <FormItem label="物资类型" name="materialTypeId">
        <Select
          v-model:value="formData.materialTypeId"
          placeholder="请选择物资类型"
          :disabled="!!materialThreshold"
          :loading="materialTypesLoading"
          show-search
          :filter-option="filterOption"
        >
          <SelectOption
            v-for="type in materialTypes"
            :key="type.id"
            :value="type.id"
          >
            <Tag :color="type.color">{{ type.typeName }}</Tag>
          </SelectOption>
        </Select>
      </FormItem>

      <FormItem label="阈值数量" name="threshold">
        <InputNumber
          v-model:value="formData.threshold"
          :min="0"
          :max="99999"
          style="width: 100%"
          placeholder="请输入库存预警阈值"
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
            {{ materialThreshold ? '更新' : '创建' }}
          </Button>
          <Button @click="handleCancel">取消</Button>
        </Space>
      </FormItem>
    </Form>
  </div>
</template>

<script setup>
import { ref, reactive, watch, onMounted } from 'vue'
import { message, Form, FormItem, Input, InputNumber, Select, SelectOption, RadioGroup, Radio, Button, Space, Tag } from 'ant-design-vue'
import { getMaterialTypes } from '../../api/materialType.api.js'

const props = defineProps({
  materialThreshold: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(['submit', 'cancel'])

const formRef = ref()
const submitting = ref(false)
const materialTypesLoading = ref(false)
const materialTypes = ref([])

const formData = reactive({
  materialTypeId: undefined,
  threshold: 0,
  sortOrder: 0,
  isEnabled: true,
  remark: ''
})

const rules = {
  materialTypeId: [
    { required: true, message: '请选择物资类型', trigger: 'change' }
  ],
  threshold: [
    { required: true, message: '请输入阈值数量', trigger: 'blur' }
  ]
}

// 加载物资类型列表
async function loadMaterialTypes() {
  try {
    materialTypesLoading.value = true
    const response = await getMaterialTypes({ page: 1, pageSize: 1000 })
    if (response.success) {
      materialTypes.value = response.data.items || []
    }
  } catch (error) {
    message.error('加载物资类型失败：' + error.message)
  } finally {
    materialTypesLoading.value = false
  }
}

// 搜索过滤
function filterOption(input, option) {
  return option.children.default?.children?.[0]?.children?.toLowerCase().includes(input.toLowerCase())
}

watch(() => props.materialThreshold, (newThreshold) => {
  if (newThreshold) {
    Object.assign(formData, {
      materialTypeId: newThreshold.materialTypeId || undefined,
      threshold: newThreshold.threshold || 0,
      sortOrder: newThreshold.sortOrder || 0,
      isEnabled: newThreshold.isEnabled ?? true,
      remark: newThreshold.remark || ''
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
    materialTypeId: undefined,
    threshold: 0,
    sortOrder: 0,
    isEnabled: true,
    remark: ''
  })
  formRef.value?.resetFields()
}

onMounted(() => {
  loadMaterialTypes()
})
</script>

<style scoped>
.material-threshold-form {
  max-width: 800px;
  padding: 20px 0;
}
</style>
