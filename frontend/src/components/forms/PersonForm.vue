<template>
  <div class="person-form">
    <Form
      :model="formData"
      :rules="rules"
      ref="formRef"
      :label-col="{ span: 6 }"
      :wrapper-col="{ span: 16 }"
    >
      <FormItem label="人员编码" name="personCode">
        <Input
          v-model:value="formData.personCode"
          placeholder="请输入人员编码"
          :disabled="!!person"
        />
      </FormItem>

      <FormItem label="姓名" name="personName">
        <Input
          v-model:value="formData.personName"
          placeholder="请输入姓名"
        />
      </FormItem>

      <FormItem label="性别" name="gender">
        <Select
          v-model:value="formData.gender"
          placeholder="请选择性别"
        >
          <SelectOption value="male">男</SelectOption>
          <SelectOption value="female">女</SelectOption>
        </Select>
      </FormItem>

      <FormItem label="联系电话" name="phone">
        <Input
          v-model:value="formData.phone"
          placeholder="请输入联系电话"
        />
      </FormItem>

      <FormItem label="所属医院" name="hospitalId">
        <Select
          v-model:value="formData.hospitalId"
          placeholder="请选择所属医院"
          show-search
          :filter-option="filterHospitalOption"
        >
          <SelectOption
            v-for="hospital in hospitals"
            :key="hospital.id"
            :value="hospital.id"
          >
            {{ hospital.hospitalName }}
          </SelectOption>
        </Select>
      </FormItem>

      <FormItem label="科室" name="departmentId">
        <Select
          v-model:value="formData.departmentId"
          placeholder="请选择科室"
          show-search
          :filter-option="filterDepartmentOption"
          :disabled="!formData.hospitalId"
        >
          <SelectOption
            v-for="department in filteredDepartments"
            :key="department.id"
            :value="department.id"
          >
            {{ department.departmentName }}
          </SelectOption>
        </Select>
      </FormItem>

      <FormItem label="职级" name="rankId">
        <Select
          v-model:value="formData.rankId"
          placeholder="请选择职级"
          allow-clear
        >
          <SelectOption
            v-for="rank in ranks"
            :key="rank.id"
            :value="rank.id"
          >
            {{ rank.rankName }}
          </SelectOption>
        </Select>
      </FormItem>

      <FormItem label="职称" name="titleId">
        <Select
          v-model:value="formData.titleId"
          placeholder="请选择职称"
          allow-clear
        >
          <SelectOption
            v-for="title in titles"
            :key="title.id"
            :value="title.id"
          >
            {{ title.titleName }}
          </SelectOption>
        </Select>
      </FormItem>

      <FormItem label="状态" name="status">
        <Select
          v-model:value="formData.status"
          placeholder="请选择状态"
        >
          <SelectOption :value="1">在职</SelectOption>
          <SelectOption :value="2">离职</SelectOption>
        </Select>
      </FormItem>

      <FormItem :wrapper-col="{ span: 16, offset: 6 }">
        <Space>
          <Button type="primary" @click="handleSubmit" :loading="submitting">
            {{ person ? '更新' : '创建' }}
          </Button>
          <Button @click="handleCancel">取消</Button>
        </Space>
      </FormItem>
    </Form>
  </div>
</template>

<script setup>
import { ref, reactive, watch, computed, onMounted } from 'vue'
import { message, Form, FormItem, Input, Select, SelectOption, Button, Space } from 'ant-design-vue'
import { getHospitals } from '../../api/basicData.api'
import { getDepartments } from '../../api/basicData.api'
import { getPersonRanks } from '../../api/basicData.api'
import { getPersonTitles } from '../../api/basicData.api'

const props = defineProps({
  person: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(['submit', 'cancel'])

const formRef = ref()
const submitting = ref(false)
const hospitals = ref([])
const departments = ref([])
const ranks = ref([])
const titles = ref([])

const formData = reactive({
  personCode: '',
  personName: '',
  gender: 'male',
  phone: '',
  hospitalId: undefined,
  departmentId: undefined,
  rankId: undefined,
  titleId: undefined,
  status: 1
})

const rules = {
  personCode: [
    { required: true, message: '请输入人员编码', trigger: 'blur' }
  ],
  personName: [
    { required: true, message: '请输入姓名', trigger: 'blur' }
  ],
  gender: [
    { required: true, message: '请选择性别', trigger: 'change' }
  ],
  phone: [
    { required: true, message: '请输入联系电话', trigger: 'blur' },
    { pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号码', trigger: 'blur' }
  ],
  hospitalId: [
    { required: true, message: '请选择所属医院', trigger: 'change' }
  ],
  departmentId: [
    { required: true, message: '请选择科室', trigger: 'change' }
  ],
  status: [
    { required: true, message: '请选择状态', trigger: 'change' }
  ]
}

// Filter departments by selected hospital
const filteredDepartments = computed(() => {
  if (!formData.hospitalId) {
    return []
  }
  return departments.value.filter(dept => dept.hospitalId === formData.hospitalId)
})

// Watch for hospital change to reset department
watch(() => formData.hospitalId, (newHospitalId, oldHospitalId) => {
  if (oldHospitalId && newHospitalId !== oldHospitalId) {
    formData.departmentId = undefined
  }
})

// Filter options
function filterHospitalOption(input, option) {
  return option.children[0].children.toLowerCase().includes(input.toLowerCase())
}

function filterDepartmentOption(input, option) {
  return option.children[0].children.toLowerCase().includes(input.toLowerCase())
}

// Load data
async function loadHospitals() {
  try {
    const response = await getHospitals()
    if (response.success) {
      hospitals.value = response.data || []
    }
  } catch (error) {
    message.error('加载医院列表失败: ' + error.message)
  }
}

async function loadDepartments() {
  try {
    const response = await getDepartments()
    if (response.success) {
      departments.value = response.data || []
    }
  } catch (error) {
    message.error('加载科室列表失败: ' + error.message)
  }
}

async function loadRanks() {
  try {
    const response = await getPersonRanks()
    if (response.success) {
      ranks.value = response.data || []
    }
  } catch (error) {
    message.error('加载职级列表失败: ' + error.message)
  }
}

async function loadTitles() {
  try {
    const response = await getPersonTitles()
    if (response.success) {
      titles.value = response.data || []
    }
  } catch (error) {
    message.error('加载职称列表失败: ' + error.message)
  }
}

// Watch for person prop changes to populate form
watch(() => props.person, (newPerson) => {
  if (newPerson) {
    Object.assign(formData, {
      personCode: newPerson.personCode || '',
      personName: newPerson.personName || '',
      gender: newPerson.gender || 'male',
      phone: newPerson.phone || '',
      hospitalId: newPerson.hospitalId || undefined,
      departmentId: newPerson.departmentId || undefined,
      rankId: newPerson.rankId || undefined,
      titleId: newPerson.titleId || undefined,
      status: newPerson.status ?? 1
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
    personCode: '',
    personName: '',
    gender: 'male',
    phone: '',
    hospitalId: undefined,
    departmentId: undefined,
    rankId: undefined,
    titleId: undefined,
    status: 1
  })
  formRef.value?.resetFields()
}

onMounted(() => {
  loadHospitals()
  loadDepartments()
  loadRanks()
  loadTitles()
})
</script>

<style scoped>
.person-form {
  max-width: 800px;
  padding: 20px 0;
}
</style>
