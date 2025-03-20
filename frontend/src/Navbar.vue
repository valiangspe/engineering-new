<template>
  <v-app-bar color="primary" prominent>
    <v-app-bar-nav-icon variant="text" @click.stop="ctx.drawer = !ctx.drawer"></v-app-bar-nav-icon>

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

  <v-navigation-drawer v-model="ctx.drawer" :location="$vuetify.display.mobile ? 'bottom' : undefined" temporary>
    <v-list density="compact" nav>
      <v-list-item href="/#/" prepend-icon="mdi-view-dashboard" title="Dashboard"></v-list-item>
      <v-list-item href="/#/mpdashboard" prepend-icon="mdi-view-dashboard-outline" title="Dashboard MP"></v-list-item>
      <v-list-item href="/#/template" prepend-icon="mdi-file-multiple" title="Dept Template"></v-list-item>
      <v-list-item href="/#/wo" prepend-icon="mdi-run-fast" title="Activity"></v-list-item>

      <v-list-item href="/#/addRole" prepend-icon="mdi-account-multiple-plus" title="Add Role"></v-list-item>
      <v-list-item href="/#/supportDoc" prepend-icon="mdi-clipboard-text-outline" title="Support Table"></v-list-item>
      <v-list-item href="/#/engineerTimeProcess" prepend-icon="mdi-clock-outline" title="Engineer Time Process"></v-list-item>

      <v-list-item href="/#/notifications" prepend-icon="mdi-bell-outline" title="Notifications"></v-list-item>
      <v-list-item href="/#/SupportEngineeringDocument" prepend-icon="mdi-file-cog-outline" title="Support Engineering Doc"></v-list-item>
      <v-list-item href="/#/ecn" prepend-icon="mdi-tools" title="ECN/CCN"></v-list-item>
      <v-list-item href="/#/manpower" prepend-icon="mdi-human-male-board" title="Manpower"></v-list-item>
      <v-list-item href="/#/bomapproval" prepend-icon="mdi-file-check-outline" title="BOM Approval"></v-list-item>
      <v-list-item href="/#/ai-document-analyzer-inq" prepend-icon="mdi-brain" title="AI Document Analyzer Inq"></v-list-item>
      <v-list-item href="/#/ai-document-analyzer" prepend-icon="mdi-brain" title="AI Document Analyzer"></v-list-item>

      <a href="https://eng.iotech.my.id" target="_blank">
        <v-list-item prepend-icon="mdi-link-variant" title="Eng Document (Link)"></v-list-item>
      </a>

      <!-- Report Menu -->
      <v-list-item prepend-icon="mdi-chart-line" title="Report" @click="toggleReport">
        <template v-slot:append>
          <v-icon>{{ isReportExpanded ? 'mdi-chevron-up' : 'mdi-chevron-down' }}</v-icon>
        </template>
      </v-list-item>

      <!-- Submenu Report -->
      <template v-if="isReportExpanded">
        <v-list-item href="/#/inputdata" prepend-icon="mdi-pencil-outline" title="Edit"></v-list-item>
        <v-list-item href="/#/donepo" prepend-icon="mdi-checkbox-marked-circle-outline" title="Grafik Done PO"></v-list-item>
        <v-list-item href="/#/outspo" prepend-icon="mdi-chart-bar" title="Grafik Outs PO"></v-list-item>
        <v-list-item href="/#/donebyweek" prepend-icon="mdi-calendar-check-outline" title="Report Done By Week"></v-list-item>
        <v-list-item href="/#/detailoverdeadline" prepend-icon="mdi-clock-alert-outline" title="Detail Over Deadline"></v-list-item>
        <v-list-item href="/#/loadpekerjaan" prepend-icon="mdi-briefcase-outline" title="Load Pekerjaan"></v-list-item>
        <v-list-item href="/#/outsporeport" prepend-icon="mdi-file-chart-outline" title="Report Activity"></v-list-item>
        <v-list-item href="/#/outsmpreport" prepend-icon="mdi-file-chart-outline" title="Report MP"></v-list-item>
      </template>

      <v-list-item>
        <button class="btn btn-sm btn-danger" @click="logout">Logout</button>
      </v-list-item>
    </v-list>
  </v-navigation-drawer>
</template>

<script setup>
import { ref, watch, onMounted, onBeforeUnmount } from "vue";
import { ctx } from "./main";
import axios from "axios";

const userRole = ref(localStorage.getItem("userRole") || null);
let intervalId = null;

// Watch untuk menyimpan perubahan userRole di localStorage
watch(userRole, (newRole) => {
  if (newRole) {
    localStorage.setItem("userRole", newRole);
  } else {
    localStorage.removeItem("userRole");
  }
});

// Menggunakan `userRole` dalam `ctx`
ctx.userRole = userRole.value;

// Fungsi untuk mendapatkan role pengguna dari backend
const fetchUserRole = async () => {
  const userId = localStorage.getItem("userId");
  if (userId) {
    try {
      const roleResp = await axios.get(`${import.meta.env.VITE_APP_BASE_URL}/api/userroles/byUserId/${userId}`);
      if (roleResp.data && roleResp.data.length > 0) {
        userRole.value = roleResp.data[0].role;
        ctx.userRole = userRole.value;
        console.log("Role berhasil diambil:", userRole.value);
      } else {
        console.warn("Tidak ada role ditemukan untuk user ini.");
      }
    } catch (error) {
      console.error("Error fetching user role:", error);
    }
  }
};

// Polling untuk memperbarui role user setiap 5 detik
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

// **⬇️ Perbaikan: Toggle Report dengan Variabel Ref**
const isReportExpanded = ref(false);

const toggleReport = () => {
  isReportExpanded.value = !isReportExpanded.value;
};

// Fungsi untuk logout
const logout = () => {
  localStorage.removeItem("token");
  localStorage.removeItem("userId");
  localStorage.removeItem("userRole");
  userRole.value = null;
  ctx.value.user = null;
  ctx.value.token = null;
  ctx.value.userRole = null;
};
</script>
