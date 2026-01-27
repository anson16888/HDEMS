import { ref, reactive, watch, computed } from 'vue'
import { message } from 'ant-design-vue'
import dayjs from 'dayjs'
import { getMaterialTypes } from '../api/materialType.api.js'
import { getHospitals } from '../api/basicData.api.js'

/**
 * 物资表单 Composable
 * 封装物资表单的业务逻辑和验证规则
 */
export function useMaterialForm() {
  // 表单引用
  const formRef = ref(null)

  // 表单数据模型
  const formData = reactive({
    id: undefined,
    material_code: '',
    material_name: '',
    material_type: undefined,
    specification: '',
    quantity: 0,
    unit: '',
    production_date: undefined,
    shelf_life: undefined,
    location: '',
    hospitalId: undefined,
    remark: ''
  })

  // 计算过期日期
  const expiryDate = computed(() => {
    if (formData.production_date && formData.shelf_life) {
      // 生产日期 + 质保期（月） = 过期日期
      return dayjs(formData.production_date).add(formData.shelf_life, 'month')
    }
    return undefined
  })

  // 表单验证规则
  const rules = {
    material_code: [
      { max: 50, message: '物资编号最多50个字符', trigger: 'blur' }
    ],
    material_name: [
      { required: true, message: '物资名称不能为空', trigger: 'blur' },
      { min: 2, max: 50, message: '物资名称长度在2到50个字符', trigger: 'blur' }
    ],
    material_type: [
      { required: true, message: '请选择物资类型', trigger: 'change' }
    ],
    quantity: [
      { required: true, message: '库存数量不能为空', trigger: 'blur' },
      { type: 'number', min: 0, message: '库存数量不能为负数', trigger: 'blur' }
    ],
    location: [
      { required: true, message: '存放位置不能为空', trigger: 'blur' },
      { max: 100, message: '存放位置最多100个字符', trigger: 'blur' }
    ],
    specification: [
      { max: 100, message: '物资规格最多100个字符', trigger: 'blur' }
    ],
    remark: [
      { max: 500, message: '备注最多500个字符', trigger: 'blur' }
    ]
  }

  // 物资类型选项（从服务器获取）
  const materialTypeOptions = ref([])

  // 医院选项（从服务器获取）
  const hospitalOptions = ref([])

  /**
   * 重置表单为初始值
   */
  function resetForm() {
    formData.id = undefined
    formData.material_code = ''
    formData.material_name = ''
    formData.material_type = undefined
    formData.specification = ''
    formData.quantity = 0
    formData.unit = ''
    formData.production_date = undefined
    formData.shelf_life = undefined
    formData.location = ''
    formData.hospitalId = undefined
    formData.remark = ''

    // 重置表单验证状态
    if (formRef.value) {
      formRef.value.clearValidate()
    }
  }

  /**
   * 加载物资类型选项
   */
  async function loadMaterialTypeOptions() {
    try {
      const response = await getMaterialTypes({ page: 1, pageSize: 1000 })
      if (response.success && response.data) {
        // 转换格式为 { label, value }
        materialTypeOptions.value = (response.data.items || []).map(item => ({
          label: item.typeName,
          value: item.id
        }))
      }
    } catch (error) {
      console.error('加载物资类型失败:', error)
      message.error('加载物资类型失败：' + (error.message || '未知错误'))
    }
  }

  /**
   * 加载医院选项
   */
  async function loadHospitalOptions() {
    try {
      const response = await getHospitals()
      // 响应格式: { code, success, data: { items: [...] } } 或 { code, success, data: [...] }
      const hospitals = response.data?.items || response.data || []
      hospitalOptions.value = Array.isArray(hospitals) ? hospitals : []
    } catch (error) {
      console.error('加载医院列表失败:', error)
    }
  }

  /**
   * 加载物资数据到表单
   * @param {Object} material - 物资对象
   */
  function loadMaterial(material) {
    formData.id = material.id
    formData.material_code = material.material_code || ''
    formData.material_name = material.material_name || ''
    formData.material_type = material.material_type
    formData.specification = material.specification || ''
    formData.quantity = material.quantity || 0
    formData.unit = material.unit || ''
    formData.location = material.location || ''
    formData.hospitalId = material.hospitalId
    formData.remark = material.remark || ''

    // 处理日期字段
    if (material.production_date) {
      formData.production_date = dayjs(material.production_date)
    } else {
      formData.production_date = undefined
    }

    formData.shelf_life = material.shelf_life || undefined
  }

  /**
   * 生成雪花ID（简单实现）
   * 生产环境应使用后端生成或更完善的雪花算法
   */
  function generateSnowflakeId() {
    const timestamp = Date.now().toString(36)
    const random = Math.random().toString(36).substring(2, 9)
    return `MAT-${timestamp}-${random}`.toUpperCase()
  }

  /**
   * 验证表单
   * @returns {Promise<boolean>}
   */
  async function validateForm() {
    if (!formRef.value) {
      return false
    }

    try {
      await formRef.value.validate()
      return true
    } catch (error) {
      console.log('表单验证失败:', error)
      return false
    }
  }

  /**
   * 准备提交数据（格式转换）
   * @returns {Object}
   */
  function prepareSubmitData() {
    const data = { ...formData }

    // 转换日期格式
    if (data.production_date) {
      data.production_date = dayjs(data.production_date).format('YYYY-MM-DD')
    }

    // 如果没有 ID，生成雪花ID
    if (!data.id) {
      data.material_code = data.material_code || generateSnowflakeId()
    }

    return data
  }

  /**
   * 监听日期变化，自动计算过期日期（仅用于展示）
   */
  watch([() => formData.production_date, () => formData.shelf_life], () => {
    // 过期日期通过 computed 自动计算
  })

  return {
    // 表单引用
    formRef,

    // 表单数据
    formData,
    expiryDate,

    // 验证规则
    rules,
    materialTypeOptions,
    hospitalOptions,

    // 方法
    resetForm,
    loadMaterial,
    validateForm,
    prepareSubmitData,
    generateSnowflakeId,
    loadMaterialTypeOptions,
    loadHospitalOptions
  }
}
