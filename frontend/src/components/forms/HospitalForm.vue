<template>
  <div class="hospital-form">
    <h3>{{ hospital ? '编辑医院信息' : '新增医院' }}</h3>
    <a-form
      :model="formData"
      :rules="rules"
      ref="formRef"
      :label-col="{ span: 6 }"
      :wrapper-col="{ span: 14 }"
    >
      <a-form-item label="医院编码" name="hospitalCode">
        <a-input
          v-model:value="formData.hospitalCode"
          placeholder="请输入医院编码"
          :disabled="!!hospital"
        />
      </a-form-item>

      <a-form-item label="医院名称" name="hospitalName">
        <a-input
          v-model:value="formData.hospitalName"
          placeholder="请输入医院名称"
        />
      </a-form-item>

      <a-form-item label="简称" name="shortName">
        <a-input
          v-model:value="formData.shortName"
          placeholder="请输入简称"
        />
      </a-form-item>

      <a-form-item label="地址" name="address">
        <a-input
          v-model:value="formData.address"
          placeholder="请输入医院地址"
        />
      </a-form-item>

      <a-form-item label="联系人" name="contactPerson">
        <a-input
          v-model:value="formData.contactPerson"
          placeholder="请输入联系人姓名"
        />
      </a-form-item>

      <a-form-item label="联系电话" name="contactPhone">
        <a-input
          v-model:value="formData.contactPhone"
          placeholder="请输入联系电话"
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

      <a-form-item label="状态" name="status">
        <a-radio-group
          v-model:value="formData.status"
          :options="statusOptions"
        />
      </a-form-item>

      <a-form-item :wrapper-col="{ span: 14, offset: 6 }">
        <a-space>
          <a-button type="primary" @click="handleSubmit" :loading="submitting">
            {{ hospital ? '更新' : '创建' }}
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
  hospital: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(['submit', 'cancel'])

const formRef = ref()
const submitting = ref(false)

const formData = reactive({
  hospitalCode: '',
  hospitalName: '',
  shortName: '',
  address: '',
  contactPerson: '',
  contactPhone: '',
  sortOrder: 0,
  status: 1
})

// 状态选项
const statusOptions = [
  { label: '启用', value: 1 },
  { label: '停用', value: 0 }
]

const rules = {
  hospitalCode: [
    { required: true, message: '请输入医院编码', trigger: 'blur' }
  ],
  hospitalName: [
    { required: true, message: '请输入医院名称', trigger: 'blur' }
  ],
  contactPhone: [
    { pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号码', trigger: 'blur' }
  ]
}

// Watch for hospital prop changes to populate form
watch(() => props.hospital, (newHospital) => {
  if (newHospital) {
    Object.assign(formData, {
      hospitalCode: newHospital.hospitalCode || '',
      hospitalName: newHospital.hospitalName || '',
      shortName: newHospital.shortName || '',
      address: newHospital.address || '',
      contactPerson: newHospital.contactPerson || '',
      contactPhone: newHospital.contactPhone || '',
      sortOrder: newHospital.sortOrder || 0,
      status: newHospital.status ?? 1
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
    hospitalCode: '',
    hospitalName: '',
    shortName: '',
    address: '',
    contactPerson: '',
    contactPhone: '',
    sortOrder: 0,
    status: 1
  })
  formRef.value?.resetFields()
}
</script>

<style scoped>
.hospital-form {
  max-width: 800px;
  padding: 20px;
}

.hospital-form h3 {
  margin-bottom: 20px;
  font-size: 18px;
  font-weight: 600;
}
</style>
