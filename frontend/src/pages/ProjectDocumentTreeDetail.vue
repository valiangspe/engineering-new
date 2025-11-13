<template>
  <div style="border: 2px solid black;">
    <iframe  v-if="treeUrl"
    style="width: 95vw; height: 85vh"
    :src="treeUrl" />
    <div v-else>Loading...</div>
  </div>
</template>
<script setup lang="ts">
import { computed } from "vue";
import { useRoute } from "vue-router";
const route = useRoute();
const projectId = route.params.projectId as string;
const treeUrl = `${import.meta.env.VITE_APP_MEETING_FRONTEND_URL}/#/projects/${projectId}/document-tree/`;
</script>
