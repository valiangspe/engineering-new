<template>
  <div class="notification-page">
    <v-container>
      <v-row>
        <v-col cols="12">
          <h3>Notifikasi Task Baru | {{ userRole }}</h3>
          <!-- Dropdown Filter Role -->
          <v-select
            v-model="selectedRole"
            :items="['Semua', 'pic', 'spv', 'manager']"
            label="Filter Berdasarkan Role"
            outlined
            dense
            class="mb-3"
          ></v-select>

          <!-- Loading Role -->
          <div v-if="!userRole">
            <v-alert type="warning">Memuat role pengguna, mohon tunggu...</v-alert>
          </div>

          <!-- Tidak Ada Notifikasi -->
          <div v-else-if="filteredNotifications.length === 0">
            <v-alert type="info" class="border-left-info">
              Tidak ada notifikasi baru untuk Anda.
            </v-alert>
          </div>

          <!-- Tampilkan Notifikasi -->
          <div v-else>
            <v-row dense>
              <v-col
                v-for="notification in filteredNotifications"
                :key="notification.id"
                cols="12"
                md="4"
              >
                <v-card class="notification-item">
                  <v-card-title class="headline">
                    {{ notification.title }}
                  </v-card-title>
                  <v-card-subtitle>
                    <span>
                      <!-- Perjelas notifikasi dengan status dan role -->
                      <template v-if="notification.previousRole">
                        Task telah diselesaikan oleh {{ notification.previousRole.toUpperCase() }} 
                        dan sekarang menunggu tindakan oleh {{ notification.role.toUpperCase() }}.
                      </template>
                      <template v-else>
                        {{ notification.message || `Task dengan ID ${notification.taskId} untuk role ${notification.role.toUpperCase()}.` }}
                      </template>
                    </span>
                  </v-card-subtitle>


                  <v-card-text>
                    <span class="text-muted">
                      {{ new Date(notification.createdAt).toLocaleString("id-ID") }}
                    </span>
                  </v-card-text>
                  <v-card-actions>
                    <v-btn @click="goToActivity(notification.taskId)">
                      Go to Task
                    </v-btn>
                  </v-card-actions>
                </v-card>
              </v-col>
            </v-row>
          </div>
        </v-col>
      </v-row>
    </v-container>
  </div>
</template>

<script setup>
import { useRouter } from "vue-router";
import { useNotificationStore } from "./notificationStore";
import { ref, computed, onMounted } from "vue";

const router = useRouter();
const notificationStore = useNotificationStore();

const notifications = computed(() => notificationStore.notifications);
const userRole = ref(localStorage.getItem("userRole") || null);
const selectedRole = ref("Semua"); // State untuk filter role

// Fungsi memuat semua notifikasi dari backend
const loadNotifications = async () => {
  try {
    const response = await fetch(
      `${import.meta.env.VITE_APP_BASE_URL}/api/notifications`
    );
    if (response.ok) {
      const data = await response.json();
      notificationStore.notifications = data;
      console.log("Notifikasi berhasil dimuat:", data);
    } else {
      console.error("Gagal memuat notifikasi:", response.status);
    }
  } catch (error) {
    console.error("Error saat memuat notifikasi:", error);
  }
};

// Fungsi navigasi ke halaman Task
const goToActivity = (taskId) => {
  router.push({
    path: "/wo", // Path ke halaman Activity.vue
    query: { taskId }, // Kirim taskId sebagai query parameter
  });
};

// Filter notifikasi berdasarkan role
const filteredNotifications = computed(() => {
  return notifications.value
    .filter((notification) => {
      if (selectedRole.value === "Semua") return true; // Tampilkan semua notifikasi
      return notification.role === selectedRole.value; // Filter berdasarkan role
    })
    .sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt)); // Urutkan berdasarkan tanggal terbaru
});

onMounted(() => {
  loadNotifications();
});
</script>

<style scoped>
.notification-page {
  padding: 20px;
}
.notification-item {
  margin-bottom: 15px;
  border: 1px solid #ccc;
  border-radius: 5px;
}
.v-container {
  padding: 0;
}
.v-alert {
  margin-top: 20px;
}
.border-left-info {
  border-left: 5px solid #2196F3;
}
</style>
