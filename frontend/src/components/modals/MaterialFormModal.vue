<template>
  <a-modal
    :visible="visible"
    :title="mode === 'create' ? '新增物资' : '编辑物资'"
    :width="800"
    :confirm-loading="isSubmitting"
    @ok="handleSubmit"
    @cancel="handleCancel"
    :ok-text="'保存'"
    :cancel-text="'取消'"
  >
    <a-form
      ref="formRef"
      :model="formData"
      :rules="rules"
      :label-col="{ span: 6 }"
      :wrapper-col="{ span: 16 }"
    >
      <a-row :gutter="16">
        <a-col :span="12">
          <!-- 物资编号 -->
          <a-form-item label="物资编号" name="material_code">
            <a-input
              v-model:value="formData.material_code"
              placeholder="系统自动生成或手动输入"
              :disabled="mode === 'edit'"
            />
          </a-form-item>
        </a-col>

        <a-col :span="12">
          <!-- 物资类型 -->
          <a-form-item label="物资类型" name="material_type">
            <a-select
              v-model:value="formData.material_type"
              placeholder="请选择物资类型"
            >
              <a-select-option
                v-for="option in materialTypeOptions"
                :key="option.value"
                :value="option.value"
              >
                {{ option.label }}
              </a-select-option>
            </a-select>
          </a-form-item>
        </a-col>
      </a-row>

      <a-row :gutter="16">
        <a-col :span="12">
          <!-- 物资名称 -->
          <a-form-item label="物资名称" name="material_name">
            <a-input
              v-model:value="formData.material_name"
              placeholder="请输入物资名称"
            />
          </a-form-item>
        </a-col>

        <a-col :span="12">
          <!-- 物资规格 -->
          <a-form-item label="物资规格" name="specification">
            <a-input
              v-model:value="formData.specification"
              placeholder="请输入物资规格（可选）"
            />
          </a-form-item>
        </a-col>
      </a-row>

      <a-row :gutter="16">
        <a-col :span="12">
          <!-- 库存数量 -->
          <a-form-item label="库存数量" name="quantity" >
            <a-input-number
              v-model:value="formData.quantity"
              :min="0"
              :precision="2"
              style="width: 100%"
              placeholder="请输入库存数量"
            />
          </a-form-item>
        </a-col>

        <a-col :span="12">
          <!-- 单位 -->
          <a-form-item label="单位" name="unit" >
            <a-input
              v-model:value="formData.unit"
              placeholder="如：个、箱、件、台"
            />
          </a-form-item>
        </a-col>
      </a-row>

      <a-row :gutter="16">
        <a-col :span="12">
          <!-- 生产日期 -->
          <a-form-item label="生产日期" name="production_date" >
            <a-date-picker
              v-model:value="formData.production_date"
              format="YYYY-MM-DD"
              placeholder="请选择生产日期"
              style="width: 100%"
            />
          </a-form-item>
        </a-col>

        <a-col :span="12">
          <!-- 质保期（月） -->
          <a-form-item label="质保期（月）" name="shelf_life" >
            <a-input-number
              v-model:value="formData.shelf_life"
              :min="0"
              :precision="0"
              style="width: 100%"
              placeholder="请输入质保期月数"
            />
          </a-form-item>
        </a-col>
      </a-row>

      <a-row :gutter="16">
        <a-col :span="expiryDate ? 12 : 0">
          <!-- 过期日期（自动计算） -->
          <a-form-item v-if="expiryDate" label="过期日期">
            <a-input
              :value="expiryDate.format('YYYY-MM-DD')"
              disabled
              placeholder="根据生产日期和质保期自动计算"
            >
              <template #suffix>
                <a-tag color="blue">自动计算</a-tag>
              </template>
            </a-input>
          </a-form-item>
        </a-col>        
        <a-col :span="12">
          <!-- 存放位置 -->
          <a-form-item label="存放位置" name="location">
            <a-input
              v-model:value="formData.location"
              placeholder="请输入存放位置"
            />
          </a-form-item>
        </a-col>
      </a-row>

      <a-row :gutter="16">
        <a-col :span="24">
          <!-- 备注 -->
          <a-form-item label="备注" name="remark" :label-col="{ span: 3 }" :wrapper-col="{ span: 21 }">
            <a-textarea
              v-model:value="formData.remark"
              placeholder="请输入备注信息（可选）"
              :rows="3"
            />
          </a-form-item>
        </a-col>
      </a-row>
    </a-form>
  </a-modal>
</template>

<script setup>
import { ref, watch, computed, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import { useMaterialForm } from '../../composables/useMaterialForm'

// Props
const props = defineProps({
  visible: {
    type: Boolean,
    required: true
  },
  mode: {
    type: String,
    default: 'create',
    validator: (value) => ['create', 'edit'].includes(value)
  },
  material: {
    type: Object,
    default: null
  }
})

// Emits
const emit = defineEmits(['update:visible', 'save'])

// 使用 composable
const {
  formRef,
  formData,
  expiryDate,
  rules,
  materialTypeOptions,
  resetForm,
  loadMaterial,
  validateForm,
  prepareSubmitData,
  loadMaterialTypeOptions
} = useMaterialForm()

// 提交中状态
const isSubmitting = ref(false)

// 监听 visible 变化
watch(() => props.visible, (newVal) => {
  if (newVal) {
    // 打开弹窗时
    if (props.mode === 'edit' && props.material) {
      loadMaterial(props.material)
    } else if (props.mode === 'create') {
      resetForm()
    }
  }
})

// 监听弹窗关闭，更新父组件
watch(() => props.visible, (newVal) => {
  if (!newVal) {
    emit('update:visible', false)
  }
})

/**
 * 提交表单
 */
async function handleSubmit() {
  try {
    // 验证表单
    const isValid = await validateForm()
    if (!isValid) {
      message.error('请检查表单填写是否正确')
      return
    }

    isSubmitting.value = true

    // 准备提交数据
    const submitData = prepareSubmitData()

    // 触发保存事件
    emit('save', submitData)
  } catch (error) {
    console.error('提交失败:', error)
    message.error('提交失败：' + (error.message || '未知错误'))
  } finally {
    isSubmitting.value = false
  }
}

/**
 * 取消
 */
function handleCancel() {
  emit('update:visible', false)
  resetForm()
}

// 组件挂载时加载物资类型选项
onMounted(() => {
  loadMaterialTypeOptions()
})
</script>

<style scoped>
/* 无额外样式需要 */
</style>
