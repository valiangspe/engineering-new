<template>
  <div class="notification-page">
    <v-container>
      <v-row>
        <v-col cols="12">
          <h3>Notifikasi Task Baru | {{ userRole }}</h3>
          <div v-if="!userRole">
            <v-alert type="warning">Memuat role pengguna, mohon tunggu...</v-alert>
          </div>
          <div v-else-if="filteredNotifications.length === 0">
            <v-alert type="info" class="border-left-info">
              Tidak ada notifikasi baru untuk Anda.
            </v-alert>
          </div>
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
                    <span>{{ notification.message }}</span>
                  </v-card-subtitle>
                  <v-card-text>
                    <span class="text-muted">
                      {{ new Date(notification.createdAt).toLocaleString("id-ID") }}
                    </span>
                  </v-card-text>
                  <v-card-actions>
                    <v-btn color="red" @click="deleteNotification(notification.id)" small>
                      Delete
                    </v-btn>
                    <v-btn @click="goToActivity(notification.taskId)">Go to Task</v-btn>


                    <v-btn
                      v-if="notification.role === userRole"
                      color="success"
                      @click="handleDone(notification)"
                      small
                    >
                      Done
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

const loadNotifications = async () => {
  if (!userRole.value) {
    console.warn("User Role belum tersedia.");
    return;
  }

  try {
    const response = await fetch(
      `${import.meta.env.VITE_APP_BASE_URL}/api/notifications/active?role=${userRole.value}`
    );
    if (response.ok) {
      const data = await response.json();

      // Update store dengan menghindari duplikat notifikasi
      data.forEach((newNotification) => {
        const index = notificationStore.notifications.findIndex(
          (n) => n.taskId === newNotification.taskId
        );
        if (index !== -1) {
          // Update notifikasi lama jika taskId sudah ada
          notificationStore.notifications[index] = newNotification;
        } else {
          // Tambahkan notifikasi baru jika tidak ada di store
          notificationStore.notifications.push(newNotification);
        }
      });

      console.log("Notifikasi berhasil diperbarui:", notificationStore.notifications);
    } else {
      console.error("Gagal memuat notifikasi:", response.status);
    }
  } catch (error) {
    console.error("Error saat memuat notifikasi:", error);
  }
};


const deleteNotification = async (id) => {
  try {
    const response = await fetch(
      `${import.meta.env.VITE_APP_BASE_URL}/api/notifications/${id}`,
      {
        method: "DELETE",
      }
    );

    if (response.ok) {
      console.log(`Notifikasi dengan ID ${id} berhasil dihapus.`);
      loadNotifications();
    } else {
      console.error("Gagal menghapus notifikasi:", response.status);
    }
  } catch (error) {
    console.error("Error saat menghapus notifikasi:", error);
  }
};


const handleDone = async (notification) => {
  try {
    const response = await fetch(
      `${import.meta.env.VITE_APP_BASE_URL}/api/notifications/done?taskId=${notification.taskId}&role=${notification.role}`,
      {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
      }
    );

    if (response.ok) {
      console.log(`Notifikasi selesai untuk task ${notification.taskId}.`);
      // Hapus notifikasi dari store lokal
      notificationStore.notifications = notificationStore.notifications.filter(
        (n) => n.id !== notification.id
      );
    } else {
      console.error("Gagal menandai notifikasi sebagai selesai:", response.status);
    }
  } catch (error) {
    console.error("Error saat menandai notifikasi sebagai selesai:", error);
  }
};


const goToActivity = (taskId) => {
  router.push({
    path: "/wo", // Path ke halaman Activity.vue
    query: { taskId }, // Kirim taskId sebagai query parameter
  });
};



const filteredNotifications = computed(() => 
  notifications.value
    .filter(
      (notification) =>
        notification.role === userRole.value || notification.role === null
    )
    .sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt)) // Urutkan berdasarkan tanggal
);


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
