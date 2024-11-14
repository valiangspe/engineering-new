<template>
  <div class="notification-page">
    <v-container>
      <v-row>
        <v-col cols="12">
          <h3>Notifikasi Task Baru | {{ userRole }}</h3>
          <div v-if="!userRole">
            <v-alert type="warning">
              Memuat role pengguna, mohon tunggu...
            </v-alert>
          </div>
          <div v-else-if="filteredNotifications.length === 0">
            <v-alert type="info" class="border-left-info">
              Tidak ada notifikasi baru untuk Anda.
            </v-alert>
          </div>
          <div v-else>
            <v-row dense>
              <v-col v-for="notification in filteredNotifications" :key="notification.id" cols="12" md="4">
                <v-card class="notification-item">
                  <v-card-title class="headline">
                    {{ notification.title }}
                  </v-card-title>
                  <v-card-subtitle>
                    <span>{{ notification.message }}</span>
                  </v-card-subtitle>
                  <v-card-text>
                    <span class="text-muted">{{ notification.createdAt }}</span>
                  </v-card-text>
                  <v-card-actions>
                    <v-btn color="red" @click="deleteNotification(notification.id)" small>
                      Delete
                    </v-btn>
                    <v-btn
                      color="info"
                      @click="goToActivity(notification.taskId)"
                      small
                    >
                      Go to Task
                    </v-btn>
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
// import { ref, computed, onMounted, onUnmounted } from 'vue';
import { useRouter } from 'vue-router';
import { useNotificationStore } from './notificationStore';
import { ctx } from '../main';
import { fetchEngineeringActivities } from './fetchers';
import { ref, watchEffect, computed, onMounted, onUnmounted } from 'vue';


const router = useRouter();
const notificationStore = useNotificationStore();
const notifications = computed(() => notificationStore.notifications);
const userRole = ref(localStorage.getItem('userRole') || ctx.userRole);
const activities = ref([]);

// Fungsi untuk memuat data activities jika belum ada
const loadEngineeringActivities = async () => {
  if (activities.value.length === 0) {
    activities.value = await fetchEngineeringActivities(); // Memanggil API untuk mendapatkan data activities
  }
};

// Fungsi untuk mendapatkan `engineeringActivityId`, `from`, dan `to` berdasarkan `taskId`
const getEngineeringActivityDataByTaskId = async (taskId) => {
  await loadEngineeringActivities();
  const activity = activities.value.find(activity =>
    activity.tasks.some(task => task.id === taskId)
  );
  return activity ? { id: activity.id, from: activity.from, to: activity.to } : null;
};

// Fungsi untuk navigasi ke halaman activity berdasarkan `taskId`
const goToActivity = async (taskId) => {
  console.log(`Navigating to taskId: ${taskId}`);
  const activityData = await getEngineeringActivityDataByTaskId(taskId);

  if (activityData) {
    router.push({
      path: '/wo',
      query: {
        taskId,
        engineeringActivityId: activityData.id
      }
    });
  }
};


watchEffect(() => {
  userRole.value = localStorage.getItem('userRole') || ctx.userRole;
  if (!userRole.value) {
    console.warn('User Role belum diambil atau tidak ada');
  } else {
    console.log('User Role berhasil diambil:', userRole.value);
  }
});

const filteredNotifications = computed(() => {
  if (!userRole.value) {
    return [];
  }
  return notifications.value
    .filter(notification => notification.role === userRole.value || notification.role === null)
    .reverse();
});

const deleteNotification = (id) => {
  console.log(`Deleting notification: ${id}`);
  notificationStore.removeNotification(id);
};

const handleDone = async (notification) => {
  console.log(`Melakukan done untuk notifikasi: ${notification.title}`);
  try {
    const currentDate = new Date().toISOString();
    const updatedTask = { ...notification };

    if (userRole.value === "pic" && !updatedTask.completedDatePic) {
      updatedTask.completedDatePic = currentDate;
      updatedTask.completedByPicId = localStorage.getItem("userId");
      notificationStore.updateNotification(updatedTask, 'spv');
    } else if (userRole.value === "spv" && !updatedTask.completedDateSpv) {
      updatedTask.completedDateSpv = currentDate;
      updatedTask.completedBySpvId = localStorage.getItem("userId");
      notificationStore.updateNotification(updatedTask, 'manager');
    } else if (userRole.value === "manager" && !updatedTask.completedDateManager) {
      updatedTask.completedDateManager = currentDate;
      updatedTask.completedByManagerId = localStorage.getItem("userId");
      notificationStore.updateNotification(updatedTask, null);
    }

    notificationStore.syncTaskToBackend(updatedTask); // Sinkronisasi dengan backend
    deleteNotification(notification.id);
  } catch (error) {
    console.error('Gagal menandai task sebagai done:', error);
  }
};

onMounted(() => {
  const pollingInterval = setInterval(() => {
    notificationStore.notifications = JSON.parse(localStorage.getItem('notifications')) || [];
  }, 10000);

  onUnmounted(() => {
    clearInterval(pollingInterval);
  });
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
