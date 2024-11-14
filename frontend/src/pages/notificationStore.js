import { defineStore } from 'pinia';
import { ref, watch } from 'vue';
import { v4 as uuidv4 } from 'uuid';

export const useNotificationStore = defineStore('notificationStore', () => {
  const notifications = ref([]);

  if (localStorage.getItem('notifications')) {
    notifications.value = JSON.parse(localStorage.getItem('notifications'));
    console.log('Notifikasi dimuat dari localStorage:', notifications.value);
  }

  const addNotification = (task, role = 'pic') => {
    console.log(`Menambahkan notifikasi untuk task: ${task.description}, role: ${role}`);
    notifications.value.push({
      id: uuidv4(),
      title: `Task Baru: ${task.description}`,
      message: `Task dengan ID ${task.id} baru saja ditambahkan dan perlu dilakukan done.`,
      createdAt: new Date().toLocaleString("id-ID", {
        weekday: "long",
        year: "numeric",
        month: "long",
        day: "numeric",
        hour: "2-digit",
        minute: "2-digit",
        second: "2-digit",
      }),
      taskId: task.id,
      role,
    });
  };

  const updateNotification = (task, nextRole) => {
    notifications.value = notifications.value.filter((n) => {
      return n.taskId !== task.id || (n.taskId === task.id && n.role === nextRole);
    });

    const notification = notifications.value.find((n) => n.taskId === task.id);

    if (notification) {
      if (!nextRole) {
        notification.title = `Task Completed: ${task.description}`;
        notification.message = `Task dengan ID ${task.id} telah selesai.`;
        notification.role = null;
      } else {
        notification.title = `Task Update: ${task.description}`;
        notification.message = `Task dengan ID ${task.id} siap untuk dilakukan "done" oleh ${nextRole.toUpperCase()}.`;
        notification.role = nextRole;
      }
    } else {
      addNotification(task, nextRole);
    }
  };

  const removeNotification = (id) => {
    console.log(`Menghapus notifikasi dengan ID: ${id}`);
    notifications.value = notifications.value.filter(n => n.id !== id);
  };

  const syncTaskToBackend = async (task) => {
    try {
      const response = await fetch(
        `${import.meta.env.VITE_APP_BASE_URL}/api/dashboard/tasks/${task.taskId}`,
        {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(task),
        }
      );
      if (response.ok) {
        console.log('Task berhasil disinkronkan ke backend:', task);
      } else {
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
