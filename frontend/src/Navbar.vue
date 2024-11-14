<template>
  <v-app-bar color="primary" prominent>
    <v-app-bar-nav-icon
      variant="text"
      @click.stop="ctx.drawer = !ctx.drawer"
    ></v-app-bar-nav-icon>

    <v-toolbar-title>
      GSPE Engineering | {{ ctx?.user?.username }} | {{ userRole }}
    </v-toolbar-title>

    <v-spacer></v-spacer>

    <template v-if="$vuetify.display.mdAndUp">
      <v-btn icon="mdi-magnify" variant="text"></v-btn>
      
      <v-btn icon="mdi-filter" variant="text"></v-btn>
    </template>

    <v-btn icon="mdi-dots-vertical" variant="text"></v-btn>
  </v-app-bar>

  <v-navigation-drawer
    v-model="ctx.drawer"
    :location="$vuetify.display.mobile ? 'bottom' : undefined"
    temporary
  >
    <v-list density="compact" nav>
      <v-list-item
        href="/#/"
        prepend-icon="mdi-view-dashboard"
        title="Dashboard"
        value="home"
      ></v-list-item>

      <v-list-item
        href="/#/mpdashboard"
        prepend-icon="mdi-view-dashboard"
        title="Dashboard MP"
        value="home"
      ></v-list-item>

      <v-list-item
        href="/#/template"
        prepend-icon="mdi-content-copy"
        title="Dept Template"
        value="template"
      ></v-list-item>
      <v-list-item
        href="/#/wo"
        prepend-icon="mdi-run-fast"
        title="Activity"
        value="activity"
      ></v-list-item>

      <!-- Tambahkan Item untuk Add Role -->
      <v-list-item
        href="/#/addRole"
        prepend-icon="mdi-account-plus"
        title="Add Role"
        value="addrole"
      ></v-list-item>

      <!-- Tambahkan Item untuk Notifications -->
      <v-list-item
        href="/#/notifications"
        prepend-icon="mdi-bell"
        title="Notifications"
        value="notifications"
      ></v-list-item>

      <v-list-item
        href="/#/ecn"
        prepend-icon="mdi-wrench-check"
        title="ECN/CCN"
        value="ecn"
      ></v-list-item>
      <v-list-item
        href="/#/manpower"
        prepend-icon="mdi-account-group"
        title="Manpower"
        value="manpower"
      ></v-list-item>

      <v-list-item
        href="/#/bomapproval"
        prepend-icon="mdi-file-document"
        title="BOM Approval"
        value="bomapproval"
      ></v-list-item>

      <v-list-item
        href="/#/ai-document-analyzer-inq"
        prepend-icon="mdi-file-document"
        title="AI Document Analyzer Inq"
        value="ai-document-analyzer"
      ></v-list-item>

      <v-list-item
        href="/#/ai-document-analyzer"
        prepend-icon="mdi-file-document"
        title="AI Document Analyzer"
        value="ai-document-analyzer"
      ></v-list-item>

      <a href="https://eng.iotech.my.id" target="_blank">
        <v-list-item
          prepend-icon="mdi-file-document"
          title="Eng Document (Link)"
          value="file-document"
        ></v-list-item>
      </a>

      <v-list-item
        prepend-icon="mdi-file-document"
        title="Report"
        @click="
          () => {
            if (ctx.expanded.find((e) => e === 'Report')) {
              ctx.expanded = ctx.expanded.filter((e) => e !== 'Report');
            } else {
              ctx.expanded = [...ctx.expanded, 'Report'];
            }
          }
        "
        value="manpower"
        ><v-icon icon="mdi-chevron-down"
      /></v-list-item>

      <template v-if="ctx.expanded.find((e) => e === 'Report')">
        <v-list-item
          prepend-icon="mdi-pencil"
          href="/#/inputdata"
          title="Edit"
          value="pencil"
        ></v-list-item>

        <v-list-item
          prepend-icon="mdi-file-document"
          href="/#/donepo"
          title="Grafik Done PO"
          value="file-document"
        ></v-list-item>

        <v-list-item
          prepend-icon="mdi-file-document"
          href="/#/outspo"
          title="Grafik Outs PO"
          value="file-document"
        ></v-list-item>

        <v-list-item
          prepend-icon="mdi-file-document"
          href="/#/donebyweek"
          title="Report Done By Week"
          value="file-document"
        ></v-list-item>

        <v-list-item
          prepend-icon="mdi-file-document"
          href="/#/detailoverdeadline"
          title="Detail Over Deadline"
          value="file-document"
        ></v-list-item>

        <v-list-item
          prepend-icon="mdi-file-document"
          href="/#/loadpekerjaan"
          title="Load Pekerjaan"
          value="file-document"
        ></v-list-item>

        <v-list-item
          prepend-icon="mdi-file-document"
          href="/#/outsporeport"
          title="Report Activity"
          value="file-document"
        ></v-list-item>

        <v-list-item
          prepend-icon="mdi-file-document"
          href="/#/outsmpreport"
          title="Report MP"
          value="file-document"
        ></v-list-item>
      </template>

      <v-list-item
        prepend-icon=""
        @click="logout"
      >
        <button class="btn btn-sm btn-danger">Logout</button>
      </v-list-item>
    </v-list>
  </v-navigation-drawer>
</template>

<script setup>
import { ref, watch, onMounted, onBeforeUnmount } from 'vue';
import { ctx } from "./main";
import axios from 'axios';

const userRole = ref(localStorage.getItem("userRole") || null);
let intervalId = null;

// Watch untuk memastikan setiap perubahan pada userRole disimpan di localStorage
watch(userRole, (newRole) => {
  if (newRole) {
    localStorage.setItem("userRole", newRole);
  } else {
    localStorage.removeItem("userRole");
  }
});

// Menggunakan `userRole` ref dalam `ctx`
ctx.userRole = userRole.value;

// Fungsi untuk mendapatkan role pengguna dari backend
const fetchUserRole = async () => {
  const userId = localStorage.getItem("userId");
  if (userId) {
    try {
      const roleResp = await axios.get(`${import.meta.env.VITE_APP_BASE_URL}/api/userroles/byUserId/${userId}`);
      if (roleResp.data && roleResp.data.length > 0) {
        userRole.value = roleResp.data[0].role; // Menyimpan `userRole` ke ref
        ctx.userRole = userRole.value; // Menyimpan juga di `ctx`
        console.log("Role berhasil diambil:", userRole.value); // Debugging Role
      } else {
        console.warn("Tidak ada data role yang ditemukan untuk user ini."); // Log ketika tidak ada role
      }
    } catch (error) {
      console.error("Error fetching user role:", error);
    }
  }
};

// Fungsi untuk polling role pengguna setiap 5 detik
const startPollingUserRole = () => {
  intervalId = setInterval(() => {
    fetchUserRole();
  }, 5000);
};

// Mengambil data role saat mounted dan mulai polling
onMounted(() => {
  fetchUserRole();
  startPollingUserRole();
});

// Bersihkan interval saat komponen tidak lagi digunakan
onBeforeUnmount(() => {
  if (intervalId) {
    clearInterval(intervalId);
  }
});

// Fungsi untuk logout dan membersihkan localStorage
const logout = () => {
  localStorage.removeItem("token");
  localStorage.removeItem("userId");
  localStorage.removeItem("userRole");
  userRole.value = null; // Menghapus userRole saat logout
  ctx.value.user = null;
  ctx.value.token = null;
  ctx.value.userRole = null;
};
</script>
