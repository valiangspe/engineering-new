import { defineStore } from 'pinia';
import { ref, watch } from 'vue';

export const useNotificationStore = defineStore('notificationStore', () => {
  const notifications = ref([]);

  // Muat notifikasi dari localStorage saat inisialisasi store
  if (localStorage.getItem('notifications')) {
    notifications.value = JSON.parse(localStorage.getItem('notifications'));
    console.log('Notifikasi dimuat dari localStorage:', notifications.value);
  }

  // Fungsi untuk menambahkan notifikasi baru (mencegah duplikat berdasarkan taskId dan role)
  const addNotification = async (task, role = "pic") => {
    try {
      // Cek duplikat berdasarkan taskId dan role
      const existingNotification = notifications.value.find(
        (n) => n.taskId === task.id && n.role === role
      );

      if (existingNotification) {
        console.log(`Notifikasi untuk task ID ${task.id} dan role ${role} sudah ada.`);
        return; // Hentikan jika notifikasi sudah ada
      }

      const newNotification = {
        title: `Task Baru: ${task.description}`,
        message: `Task dengan ID ${task.id} perlu dilakukan done.`,
        createdAt: new Date().toISOString(),
        taskId: task.id,
        role,
      };

      // Kirim notifikasi ke backend
      const response = await fetch(
        `${import.meta.env.VITE_APP_BASE_URL}/api/notifications`,
        {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(newNotification),
        }
      );

      if (response.ok) {
        const savedNotification = await response.json();
        notifications.value.push(savedNotification); // Tambahkan ke store
        console.log("Notifikasi baru berhasil ditambahkan:", savedNotification);
      } else {
        console.error("Gagal menambahkan notifikasi baru:", response.status);
      }
    } catch (error) {
      console.error("Error saat menambahkan notifikasi baru:", error);
    }
  };

  // Fungsi untuk memperbarui notifikasi (jika sudah ada)
  const updateNotification = async (task, nextRole = null) => {
    try {
      // Cari notifikasi berdasarkan taskId
      const index = notifications.value.findIndex((n) => n.taskId === task.id);

      if (index !== -1) {
        // Update notifikasi lama
        notifications.value[index] = {
          ...notifications.value[index],
          role: nextRole || null,
          title: nextRole
            ? `Task Update: ${task.description}`
            : `Task Completed: ${task.description}`,
          message: nextRole
            ? `Task dengan ID ${task.id} siap dilakukan "done" oleh ${nextRole.toUpperCase()}.`
            : `Task dengan ID ${task.id} telah selesai.`,
          createdAt: new Date().toISOString(),
        };

        // Kirim pembaruan ke backend
        const response = await fetch(
          `${import.meta.env.VITE_APP_BASE_URL}/api/notifications/${notifications.value[index].id}`,
          {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(notifications.value[index]),
          }
        );

        if (response.ok) {
          console.log("Notifikasi berhasil diperbarui:", notifications.value[index]);
        } else {
          console.error("Gagal memperbarui notifikasi:", response.status);
        }
      } else {
        // Jika notifikasi tidak ditemukan, tambahkan sebagai notifikasi baru
        console.log("Notifikasi belum ada, menambahkan notifikasi baru.");
        await addNotification(task, nextRole);
      }
    } catch (error) {
      console.error("Error saat memperbarui notifikasi:", error);
    }
  };

  // Fungsi untuk menghapus notifikasi
  const removeNotification = async (id) => {
    try {
      const response = await fetch(
        `${import.meta.env.VITE_APP_BASE_URL}/api/notifications/${id}`,
        {
          method: "DELETE",
        }
      );

      if (response.ok) {
        notifications.value = notifications.value.filter((n) => n.id !== id);
        console.log(`Notifikasi dengan ID ${id} berhasil dihapus.`);
      } else {
        console.error("Gagal menghapus notifikasi:", response.status);
      }
    } catch (error) {
      console.error("Error saat menghapus notifikasi:", error);
    }
  };

  // Simpan notifikasi ke localStorage setiap ada perubahan
  watch(
    notifications,
    (newNotifications) => {
      localStorage.setItem("notifications", JSON.stringify(newNotifications));
      console.log("Notifikasi disimpan ke localStorage:", newNotifications);
    },
    { deep: true }
  );

  return {
    notifications,
    addNotification,
    updateNotification,
    removeNotification,
  };
});
