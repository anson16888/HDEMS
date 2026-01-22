<template>
  <a-modal
    :visible="visible"
    title="导入Excel"
    :width="700"
    @cancel="handleCancel"
    :footer="null"
  >
    <a-space direction="vertical" :size="24" style="width: 100%">
      <!-- 说明文字 -->
      <a-alert
        message="导入说明"
        description="请先下载模板，填写物资信息后上传。单次最多导入5000条记录，文件大小不超过10MB。"
        type="info"
        show-icon
      />

      <!-- 下载模板 -->
      <a-card size="small" title="步骤1: 下载模板">
        <a-space>
          <a-button type="primary" @click="handleDownloadTemplate">
            <template #icon>
              <DownloadOutlined />
            </template>
            下载Excel模板
          </a-button>
          <span style="color: #999">模板包含所有必填字段和示例数据</span>
        </a-space>
      </a-card>

      <!-- 上传文件 -->
      <a-card size="small" title="步骤2: 上传文件">
        <a-upload
          :file-list="fileList"
          :before-upload="beforeUpload"
          :remove="handleRemove"
          @change="handleChange"
          :accept="'.xlsx,.xls'"
        >
          <a-button>
            <template #icon>
              <UploadOutlined />
            </template>
            选择文件
          </a-button>
        </a-upload>

        <div style="margin-top: 8px; color: #999; font-size: 12px">
          支持 .xlsx 和 .xls 格式，文件大小 ≤ 10MB
        </div>
      </a-card>

      <!-- 导入结果 -->
      <a-card v-if="importResult" size="small" title="导入结果">
        <a-space direction="vertical" :size="8" style="width: 100%">
          <a-row :gutter="16">
            <a-col :span="8">
              <a-statistic
                title="总记录数"
                :value="importResult.total"
                :value-style="{ color: '#1890ff' }"
              />
            </a-col>
            <a-col :span="8">
              <a-statistic
                title="成功"
                :value="importResult.success"
                :value-style="{ color: '#52c41a' }"
              />
            </a-col>
            <a-col :span="8">
              <a-statistic
                title="失败"
                :value="importResult.failed"
                :value-style="{ color: '#ff4d4f' }"
              />
            </a-col>
          </a-row>

          <!-- 错误列表 -->
          <a-collapse v-if="importResult.errors && importResult.errors.length > 0" ghost>
            <a-collapse-panel header="错误详情">
              <a-list
                size="small"
                :data-source="importResult.errors"
                :split="false"
              >
                <template #renderItem="{ item }">
                  <a-list-item>
                    <a-space>
                      <a-tag color="red">第{{ item.row }}行</a-tag>
                      <span>{{ item.message }}</span>
                    </a-space>
                  </a-list-item>
                </template>
              </a-list>
            </a-collapse-panel>
          </a-collapse>
        </a-space>
      </a-card>

      <!-- 操作按钮 -->
      <a-space style="width: 100%; justify-content: flex-end">
        <a-button @click="handleCancel">关闭</a-button>
        <a-button
          type="primary"
          :loading="importing"
          :disabled="!selectedFile"
          @click="handleImport"
        >
          开始导入
        </a-button>
      </a-space>
    </a-space>
  </a-modal>
</template>

<script setup>
import { ref, watch, computed } from 'vue'
import { message } from 'ant-design-vue'
import { UploadOutlined, DownloadOutlined } from '@ant-design/icons-vue'
import { materialService } from '../../services/material.service'

// Props
const props = defineProps({
  visible: {
    type: Boolean,
    required: true
  }
})

// Emits
const emit = defineEmits(['update:visible', 'success'])

// State
const fileList = ref([])
const selectedFile = computed(() => fileList.value.length > 0 ? fileList.value[0] : null)
const importing = ref(false)
const importResult = ref(null)

/**
 * 监听弹窗关闭
 */
watch(() => props.visible, (newVal) => {
  if (!newVal) {
    emit('update:visible', false)
    // 重置状态
    fileList.value = []
    importResult.value = null
  }
})

/**
 * 下载模板
 */
async function handleDownloadTemplate() {
  try {
    const blob = await materialService.downloadTemplate()

    // 创建下载链接
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = '物资导入模板.xlsx'
    link.click()
    window.URL.revokeObjectURL(url)

    message.success('模板下载成功')
  } catch (error) {
    message.error('模板下载失败：' + (error.message || '未知错误'))
  }
}

/**
 * 上传前校验
 */
function beforeUpload(file) {
  // 检查文件格式
  const isExcel = file.type === 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' ||
                   file.type === 'application/vnd.ms-excel'
  if (!isExcel) {
    message.error('只能上传 Excel 文件（.xlsx 或 .xls）')
    return false
  }

  // 检查文件大小（10MB）
  const isLt10M = file.size / 1024 / 1024 < 10
  if (!isLt10M) {
    message.error('文件大小不能超过 10MB')
    return false
  }

  return false // 阻止自动上传，手动处理
}

/**
 * 文件变化
 */
function handleChange(info) {
  if (info.fileList.length > 0) {
    fileList.value = [info.fileList[info.fileList.length - 1]]
  } else {
    fileList.value = []
  }

  // 重置导入结果
  importResult.value = null
}

/**
 * 移除文件
 */
function handleRemove() {
  fileList.value = []
  importResult.value = null
}

/**
 * 导入Excel
 */
async function handleImport() {
  if (!selectedFile.value) {
    message.warning('请先选择要导入的文件')
    return
  }

  importing.value = true
  importResult.value = null

  try {
    const response = await materialService.import(selectedFile.value.originFileObj)

    // 解析响应
    const result = {
      total: response.total || 0,
      success: response.success || 0,
      failed: response.failed || 0,
      errors: response.errors || []
    }

    importResult.value = result

    // 显示结果
    if (result.failed === 0) {
      message.success(`导入成功！共导入 ${result.success} 条记录`)
      emit('success')
    } else {
      message.warning(`导入完成，成功 ${result.success} 条，失败 ${result.failed} 条`)
    }
  } catch (error) {
    message.error('导入失败：' + (error.message || '未知错误'))

    // 显示错误
    importResult.value = {
      total: 0,
      success: 0,
      failed: 1,
      errors: [{
        row: 0,
        message: error.message || '未知错误'
      }]
    }
  } finally {
    importing.value = false
  }
}

/**
 * 关闭弹窗
 */
function handleCancel() {
  emit('update:visible', false)
}
</script>

<style scoped>
/* 无额外样式需要 */
</style>
