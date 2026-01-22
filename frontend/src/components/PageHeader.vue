<template>
  <div class="page-header">
    <div>
      <h2>{{ title }}</h2>
      <p>{{ description }}</p>
    </div>
    <div v-if="ctaText" class="page-header-cta">
      <button class="primary-button" type="button">{{ ctaText }}</button>
    </div>
  </div>
</template>

<script setup>
defineProps({
  title: {
    type: String,
    required: true
  },
  description: {
    type: String,
    required: true
  },
  ctaText: {
    type: String,
    default: ''
  }
})
</script>
