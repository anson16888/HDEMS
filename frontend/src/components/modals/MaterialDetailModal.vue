<template>
  <a-modal
    :visible="visible"
    title="物资详情"
    :width="800"
    :footer="null"
    @cancel="handleCancel"
  >
    <a-descriptions
      v-if="material"
      bordered
      :column="2"
      size="small"
    >
      <a-descriptions-item label="物资编号">
        {{ material.material_code }}
      </a-descriptions-item>

      <a-descriptions-item label="物资类型">
        <a-tag :color="getTypeColor(material.material_type)">
          {{ getTypeLabel(material.material_type) }}
        </a-tag>
      </a-descriptions-item>

      <a-descriptions-item label="物资名称">
        {{ material.material_name }}
      </a-descriptions-item>

      <a-descriptions-item label="物资规格">
        {{ material.specification || '-' }}
      </a-descriptions-item>

      <a-descriptions-item label="库存数量">
        <a-statistic
          :value="material.quantity"
          :value-style="{
            fontSize: '16px',
            fontWeight: 'bold',
            color: material.quantity === 0 ? '#ff4d4f' : '#1890ff'
          }"
        />
        <span v-if="material.unit" style="margin-left: 8px">{{ material.unit }}</span>
      </a-descriptions-item>

      <a-descriptions-item label="单位">
        {{ material.unit || '-' }}
      </a-descriptions-item>

      <a-descriptions-item label="库存状态">
        <a-tag :color="getStatusColor(material.status)">
          {{ getStatusLabel(material.status) }}
        </a-tag>
      </a-descriptions-item>

      <a-descriptions-item label="生产日期">
        {{ material.production_date ? formatDate(material.production_date) : '-' }}
      </a-descriptions-item>

      <a-descriptions-item label="质保期（月）">
        {{ material.shelf_life ? `${material.shelf_life} 个月` : '-' }}
      </a-descriptions-item>

      <a-descriptions-item label="过期日期">
        <span v-if="material.expiry_date">
          {{ formatDate(material.expiry_date) }}
          <a-tag
            v-if="isExpiringSoon(material.expiry_date)"
            color="orange"
            style="margin-left: 8px"
          >
            即将过期
          </a-tag>
          <a-tag
            v-if="isExpired(material.expiry_date)"
            color="default"
            style="margin-left: 8px"
          >
            已过期
          </a-tag>
        </span>
        <span v-else>-</span>
      </a-descriptions-item>

      <a-descriptions-item label="存放位置">
        {{ material.location }}
      </a-descriptions-item>

      <a-descriptions-item label="备注" :span="2">
        {{ material.remark || '-' }}
      </a-descriptions-item>

      <a-descriptions-item label="创建时间" :span="2">
        {{ material.created_at ? formatDateTime(material.created_at) : '-' }}
      </a-descriptions-item>

      <a-descriptions-item label="最后更新时间" :span="2">
        {{ material.updated_at ? formatDateTime(material.updated_at) : '-' }}
      </a-descriptions-item>

      <a-descriptions-item label="最后操作人" :span="2">
        {{ material.updated_by || '-' }}
      </a-descriptions-item>
    </a-descriptions>

    <a-empty v-else description="暂无物资信息" />
  </a-modal>
</template>

<script setup>
import { watch } from 'vue'
import dayjs from 'dayjs'

// Props
const props = defineProps({
  visible: {
    type: Boolean,
    required: true
  },
  material: {
    type: Object,
    default: null
  }
})

// Emits
const emit = defineEmits(['update:visible'])

/**
 * 获取物资类型颜色
 */
function getTypeColor(type) {
  const colors = {
    FOOD: 'orange',
    MEDICAL: 'green',
    EQUIPMENT: 'blue',
    CLOTHING: 'purple',
    OTHER: 'default'
  }
  return colors[type] || 'default'
}

/**
 * 获取物资类型标签
 */
function getTypeLabel(type) {
  const labels = {
    FOOD: '食品类',
    MEDICAL: '医疗用品',
    EQUIPMENT: '救援设备',
    CLOTHING: '衣物类',
    OTHER: '其他'
  }
  return labels[type] || type
}

/**
 * 获取库存状态颜色
 */
function getStatusColor(status) {
  const colors = {
    NORMAL: 'green',
    LOW: 'orange',
    OUT: 'red',
    EXPIRED: 'default'
  }
  return colors[status] || 'default'
}

/**
 * 获取库存状态标签
 */
function getStatusLabel(status) {
  const labels = {
    NORMAL: '正常',
    LOW: '库存偏低',
    OUT: '已耗尽',
    EXPIRED: '已过期'
  }
  return labels[status] || status
}

/**
 * 格式化日期
 */
function formatDate(date) {
  return dayjs(date).format('YYYY-MM-DD')
}

/**
 * 格式化日期时间
 */
function formatDateTime(date) {
  return dayjs(date).format('YYYY-MM-DD HH:mm:ss')
}

/**
 * 判断是否即将过期（30天内）
 */
function isExpiringSoon(date) {
  if (!date) return false
  const today = dayjs()
  const expiryDate = dayjs(date)
  const diffDays = expiryDate.diff(today, 'day')
  return diffDays <= 30 && diffDays >= 0
}

/**
 * 判断是否已过期
 */
function isExpired(date) {
  if (!date) return false
  return dayjs().isAfter(dayjs(date))
}

/**
 * 关闭弹窗
 */
function handleCancel() {
  emit('update:visible', false)
}

// 监听弹窗关闭，更新父组件
watch(() => props.visible, (newVal) => {
  if (!newVal) {
    emit('update:visible', false)
  }
})
</script>

<style scoped>
/* 无额外样式需要 */
</style>
