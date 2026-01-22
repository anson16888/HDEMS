<template>
  <div class="person-rank-form">
    <a-form
      :model="formData"
      :rules="rules"
      ref="formRef"
      :label-col="{ span: 6 }"
      :wrapper-col="{ span: 16 }"
    >
      <a-form-item label="职级编码" name="rankCode">
        <a-input
          v-model:value="formData.rankCode"
          placeholder="请输入职级编码"
          :disabled="!!rank"
        />
      </a-form-item>

      <a-form-item label="职级名称" name="rankName">
        <a-input
          v-model:value="formData.rankName"
          placeholder="请输入职级名称"
        />
      </a-form-item>

      <a-form-item label="职级类别" name="rankCategory">
        <a-select
          v-model:value="formData.rankCategory"
          placeholder="请选择职级类别"
          :options="rankCategoryOptions"
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

      <a-form-item :wrapper-col="{ span: 16, offset: 6 }">
        <a-space>
          <a-button type="primary" @click="handleSubmit" :loading="submitting">
            {{ rank ? '更新' : '创建' }}
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
  rank: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(['submit', 'cancel'])

const formRef = ref()
const submitting = ref(false)

const formData = reactive({
  rankCode: '',
  rankName: '',
  rankCategory: 'bureau',
  sortOrder: 0
})

// 职级类别选项
const rankCategoryOptions = [
  { label: '局级', value: 'bureau' },
  { label: '院级', value: 'hospital' },
  { label: '行政', value: 'administrative' }
]

const rules = {
  rankCode: [
    { required: true, message: '请输入职级编码', trigger: 'blur' }
  ],
  rankName: [
    { required: true, message: '请输入职级名称', trigger: 'blur' }
  ],
  rankCategory: [
    { required: true, message: '请选择职级类别', trigger: 'change' }
  ]
}

// Watch for rank prop changes to populate form
watch(() => props.rank, (newRank) => {
  if (newRank) {
    Object.assign(formData, {
      rankCode: newRank.rankCode || '',
      rankName: newRank.rankName || '',
      rankCategory: newRank.rankCategory || 'bureau',
      sortOrder: newRank.sortOrder || 0
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
    rankCode: '',
    rankName: '',
    rankCategory: 'bureau',
    sortOrder: 0
  })
  formRef.value?.resetFields()
}
</script>

<style scoped>
.person-rank-form {
  max-width: 800px;
  padding: 20px 0;
}
</style>
