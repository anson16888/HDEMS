<template>
  <div class="hospital-management-page">
    <!-- Page Header -->
    <a-page-header title="医院信息" sub-title="编辑当前医院的基本信息" />

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

          <FormItem label="医院电话" name="hospitalPhone">
            <Input
              v-model:value="formData.hospitalPhone"
              placeholder="请输入医院电话"
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
import { getHospitalConfig, updateHospitalConfig } from '../api/hospitalConfig.api.js'

const formRef = ref()
const submitting = ref(false)
const formData = reactive({
  hospitalName: '',
  hospitalPhone: ''
})

const rules = {
  hospitalName: [
    { required: true, message: '请输入医院名称', trigger: 'blur' }
  ],
  hospitalPhone: [
    { required: true, message: '请输入医院电话', trigger: 'blur' },
    { pattern: /^[0-9\-+()\s]{6,20}$/, message: '请输入正确的联系电话', trigger: 'blur' }
  ]
}

async function loadHospitalInfo() {
  try {
    const response = await getHospitalConfig()
    if (response.success && response.data) {
      formData.hospitalName = response.data.hospitalName || ''
      formData.hospitalPhone = response.data.hospitalPhone || ''
    }
  } catch (error) {
    message.error('加载医院信息失败: ' + error.message)
  }
}

async function handleSubmit() {
  try {
    await formRef.value.validate()
    submitting.value = true

    const response = await updateHospitalConfig({
      hospitalName: formData.hospitalName,
      hospitalPhone: formData.hospitalPhone
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
  padding: 0 16px;
}

.form-card {
  margin-bottom: 16px;
}

.hospital-form {
  max-width: 800px;
  padding: 20px;
}
</style>
