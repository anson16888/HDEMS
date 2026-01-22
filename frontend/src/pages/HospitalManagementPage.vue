<template>
  <div class="hospital-management-page">
    <!-- Page Header -->
    <a-page-header
      title="医院信息"
      sub-title="编辑当前医院的基本信息"
    />

    <!-- Hospital Edit Form -->
    <a-card :bordered="false" class="form-card">
      <div class="hospital-form">
        <Form
          :model="formData"
          :rules="rules"
          ref="formRef"
          :label-col="{ span: 4 }"
          :wrapper-col="{ span: 12 }"
        >
          <FormItem label="医院名称" name="hospitalName">
            <Input
              v-model:value="formData.hospitalName"
              placeholder="请输入医院名称"
            />
          </FormItem>

          <FormItem label="值班电话" name="dutyPhone">
            <Input
              v-model:value="formData.dutyPhone"
              placeholder="请输入值班电话"
            />
          </FormItem>

          <FormItem :wrapper-col="{ span: 12, offset: 4 }">
            <Space>
              <Button type="primary" @click="handleSubmit" :loading="submitting">
                保存
              </Button>
            </Space>
          </FormItem>
        </Form>
      </div>
    </a-card>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { message, Form, FormItem, Input, Button, Space } from 'ant-design-vue'
import { getHospitals, updateHospital } from '../api/basicData.api.js'

const formRef = ref()
const submitting = ref(false)
const hospitalId = ref(null)

const formData = reactive({
  hospitalName: '',
  dutyPhone: ''
})

const rules = {
  hospitalName: [
    { required: true, message: '请输入医院名称', trigger: 'blur' }
  ],
  dutyPhone: [
    { required: true, message: '请输入值班电话', trigger: 'blur' },
    { pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号码', trigger: 'blur' }
  ]
}

async function loadHospitalInfo() {
  try {
    const response = await getHospitals()
    if (response.success && response.data && response.data.length > 0) {
      const hospital = response.data[0]
      hospitalId.value = hospital.id
      formData.hospitalName = hospital.hospitalName || ''
      formData.dutyPhone = hospital.dutyPhone || ''
    }
  } catch (error) {
    message.error('加载医院信息失败: ' + error.message)
  }
}

async function handleSubmit() {
  try {
    await formRef.value.validate()
    submitting.value = true

    const response = await updateHospital(hospitalId.value, {
      hospitalName: formData.hospitalName,
      dutyPhone: formData.dutyPhone
    })

    if (response.success) {
      message.success('保存成功')
      await loadHospitalInfo()
    }
  } catch (error) {
    if (error.errorFields) {
      message.error('请检查表单填写是否正确')
    } else {
      message.error('保存失败: ' + error.message)
    }
  } finally {
    submitting.value = false
  }
}

onMounted(() => {
  loadHospitalInfo()
})
</script>

<style scoped>
.hospital-management-page {
  padding: 16px;
  padding-bottom: 0;
}

.hospital-management-page :deep(.ant-page-header) {
  padding: 16px 24px;
  background: #fff;
  border-radius: 8px;
  margin-bottom: 16px;
}

.form-card {
  margin-bottom: 16px;
}

.hospital-form {
  max-width: 800px;
  padding: 20px;
}
</style>
