<template>
  <div class="department-form">
    <h3>{{ department ? '编辑科室信息' : '新增科室' }}</h3>
    <a-form
      :model="formData"
      :rules="rules"
      ref="formRef"
      :label-col="{ span: 6 }"
      :wrapper-col="{ span: 14 }"
    >
      <a-form-item label="科室编码" name="departmentCode">
        <a-input
          v-model:value="formData.departmentCode"
          placeholder="请输入科室编码"
          :disabled="!!department"
        />
      </a-form-item>

      <a-form-item label="科室名称" name="departmentName">
        <a-input
          v-model:value="formData.departmentName"
          placeholder="请输入科室名称"
        />
      </a-form-item>

      <a-form-item label="科室类型" name="departmentType">
        <a-select
          v-model:value="formData.departmentType"
          placeholder="请选择科室类型"
          :options="departmentTypeOptions"
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

      <a-form-item :wrapper-col="{ span: 14, offset: 6 }">
        <a-space>
          <a-button type="primary" @click="handleSubmit" :loading="submitting">
            {{ department ? '更新' : '创建' }}
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
  department: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(['submit', 'cancel'])

const formRef = ref()
const submitting = ref(false)

const formData = reactive({
  departmentCode: '',
  departmentName: '',
  departmentType: 'clinical',
  sortOrder: 0
})

// 科室类型选项
const departmentTypeOptions = [
  { label: '临床科室', value: 'clinical' },
  { label: '医技科室', value: 'medical_tech' },
  { label: '行政科室', value: 'administrative' }
]

const rules = {
  departmentCode: [
    { required: true, message: '请输入科室编码', trigger: 'blur' }
  ],
  departmentName: [
    { required: true, message: '请输入科室名称', trigger: 'blur' }
  ],
  departmentType: [
    { required: true, message: '请选择科室类型', trigger: 'change' }
  ]
}

// Watch for department prop changes to populate form
watch(() => props.department, (newDepartment) => {
  if (newDepartment) {
    Object.assign(formData, {
      departmentCode: newDepartment.departmentCode || '',
      departmentName: newDepartment.departmentName || '',
      departmentType: newDepartment.departmentType || 'clinical',
      sortOrder: newDepartment.sortOrder || 0
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
    departmentCode: '',
    departmentName: '',
    departmentType: 'clinical',
    sortOrder: 0
  })
  formRef.value?.resetFields()
}
</script>

<style scoped>
.department-form {
  max-width: 800px;
  padding: 20px;
}

.department-form h3 {
  margin-bottom: 20px;
  font-size: 18px;
  font-weight: 600;
}
</style>
