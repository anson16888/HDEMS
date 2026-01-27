<template>
  <a-modal
    :visible="true"
    :title="mode === 'create' ? '新增医院' : '编辑医院'"
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
      <!-- 医院编码 -->
      <a-form-item label="医院编码" name="hospitalCode">
        <a-input
          v-model:value="formData.hospitalCode"
          placeholder="请输入医院编码"
        />
      </a-form-item>

      <!-- 医院名称 -->
      <a-form-item label="医院名称" name="hospitalName">
        <a-input
          v-model:value="formData.hospitalName"
          placeholder="请输入医院名称"
        />
      </a-form-item>

      <!-- 医院简称 -->
      <a-form-item label="医院简称" name="shortName">
        <a-input
          v-model:value="formData.shortName"
          placeholder="请输入医院简称"
        />
      </a-form-item>

      <!-- 地址 -->
      <a-form-item label="地址" name="address">
        <a-input
          v-model:value="formData.address"
          placeholder="请输入医院地址"
        />
      </a-form-item>

      <!-- 联系电话 -->
      <a-form-item label="联系电话" name="contactPhone">
        <a-input
          v-model:value="formData.contactPhone"
          placeholder="请输入联系电话"
        />
      </a-form-item>

      <!-- 联系人 -->
      <a-form-item label="联系人" name="contactPerson">
        <a-input
          v-model:value="formData.contactPerson"
          placeholder="请输入联系人姓名"
        />
      </a-form-item>

      <!-- 排序 -->
      <a-form-item label="排序" name="sortOrder">
        <a-input-number
          v-model:value="formData.sortOrder"
          placeholder="请输入排序号"
          :min="0"
          style="width: 100%"
        />
      </a-form-item>

      <!-- 状态 -->
      <a-form-item label="状态" name="status">
        <a-radio-group
          v-model:value="formData.status"
          :options="statusOptions"
        />
      </a-form-item>
    </a-form>
  </a-modal>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import { createHospital, updateHospital } from '@/api/basicData.api.js'

const props = defineProps({
  mode: {
    type: String,
    required: true,
    validator: (value) => ['create', 'edit'].includes(value)
  },
  hospital: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(['save', 'cancel'])

const formRef = ref()

const formData = reactive({
  hospitalCode: '',
  hospitalName: '',
  shortName: '',
  address: '',
  contactPhone: '',
  contactPerson: '',
  sortOrder: 0,
  status: 1
})

const isSubmitting = ref(false)

// 状态选项
const statusOptions = [
  { label: '启用', value: 1 },
  { label: '停用', value: 0 }
]

// Validation rules
const rules = {
  hospitalCode: [
    { required: true, message: '请输入医院编码', trigger: 'blur' }
  ],
  hospitalName: [
    { required: true, message: '请输入医院名称', trigger: 'blur' },
    { min: 2, message: '医院名称至少2个字符', trigger: 'blur' }
  ],
  shortName: [
    { required: true, message: '请输入医院简称', trigger: 'blur' }
  ],
  address: [
    { max: 200, message: '地址最多200个字符', trigger: 'blur' }
  ],
  contactPhone: [
    { pattern: /^[0-9\-+()\s]{6,20}$/, message: '请输入正确的联系电话', trigger: 'blur' }
  ],
  contactPerson: [
    { max: 50, message: '联系人姓名最多50个字符', trigger: 'blur' }
  ],
  sortOrder: [
    { type: 'number', message: '排序必须是数字', trigger: 'blur' }
  ]
}

onMounted(() => {
  if (props.mode === 'edit' && props.hospital) {
    Object.assign(formData, {
      hospitalCode: props.hospital.hospitalCode || '',
      hospitalName: props.hospital.hospitalName || '',
      shortName: props.hospital.shortName || '',
      address: props.hospital.address || '',
      contactPhone: props.hospital.contactPhone || '',
      contactPerson: props.hospital.contactPerson || '',
      sortOrder: props.hospital.sortOrder ?? 0,
      status: props.hospital.status ?? 1
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
      await createHospital(formData)
      message.success('医院创建成功')
    } else {
      await updateHospital(props.hospital.id, formData)
      message.success('医院信息已更新')
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
