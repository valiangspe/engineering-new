import { defineStore } from 'pinia';
import { ref, watch } from 'vue';
import { v4 as uuidv4 } from 'uuid';

export const useNotificationStore = defineStore('notificationStore', () => {
  const notifications = ref([]);

  if (localStorage.getItem('notifications')) {
    notifications.value = JSON.parse(localStorage.getItem('notifications'));
    console.log('Notifikasi dimuat dari localStorage:', notifications.value);
  }

  const addNotification = async (task, role = "pic") => {
    try {
      const newNotification = {
        title: `Task Baru: ${task.description}`,
        message: `Task dengan ID ${task.id} baru saja ditambahkan dan perlu dilakukan done.`,
        createdAt: new Date().toISOString(),
        taskId: task.id,
        role,
      };
  
      const response = await fetch(
        `${import.meta.env.VITE_APP_BASE_URL}/api/notifications`,
        {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(newNotification),
        }
      );
  
      if (response.ok) {
        notifications.value.push(await response.json());
        console.log("Notifikasi baru berhasil ditambahkan.");
      } else {
        console.error("Gagal menambahkan notifikasi baru:", response.status);
      }
    } catch (error) {
      console.error("Error saat menambahkan notifikasi baru:", error);
    }
  };
  

  const updateNotification = async (task, nextRole) => {
    try {
      const notification = notifications.value.find(
        (n) => n.taskId === task.id && n.role === nextRole
      );
  
      if (!notification) {
        await addNotification(task, nextRole);
        return;
      }
  
      notification.role = nextRole || null;
      notification.title = nextRole
        ? `Task Update: ${task.description}`
        : `Task Completed: ${task.description}`;
      notification.message = nextRole
        ? `Task dengan ID ${task.id} siap untuk dilakukan "done" oleh ${nextRole.toUpperCase()}.`
        : `Task dengan ID ${task.id} telah selesai.`;
  
      await fetch(
        `${import.meta.env.VITE_APP_BASE_URL}/api/notifications/${notification.id}`,
        {
          method: "PUT",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(notification),
        }
      );
  
      console.log("Notifikasi berhasil diperbarui.");
    } catch (error) {
      console.error("Error saat memperbarui notifikasi:", error);
    }
  };
  

  const removeNotification = (id) => {
    console.log(`Menghapus notifikasi dengan ID: ${id}`);
    notifications.value = notifications.value.filter(n => n.id !== id);
  };

  const syncTaskToBackend = async (task) => {
    try {
      const response = await fetch(
        `${import.meta.env.VITE_APP_BASE_URL}/api/tasks/${task.taskId}`,
        {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(task),
        }
      );
      if (!response.ok) {
        console.error('Gagal menyinkronkan task ke backend:', response.status);
      }
    } catch (error) {
      console.error('Error saat menyinkronkan task ke backend:', error);
    }
  };
  

  watch(notifications, (newNotifications) => {
    localStorage.setItem('notifications', JSON.stringify(newNotifications));
    console.log('Notifikasi disimpan ke localStorage:', newNotifications);
  }, { deep: true });

  return {
    notifications,
    addNotification,
    updateNotification,
    removeNotification,
    syncTaskToBackend,
  };
});
