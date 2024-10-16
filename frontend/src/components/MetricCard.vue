<!-- MetricCard.vue -->
<template>
    <div class="card">
      <div class="card-body">
        <h3 class="card-title">{{ title }}</h3>
        <p class="card-text display-4">{{ value }}</p>
      </div>
    </div>
  </template>
  
  <script setup>
  defineProps({
    title: String,
    value: Number
  });
  </script>