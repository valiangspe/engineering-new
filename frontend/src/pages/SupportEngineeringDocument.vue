<template>
  <v-container class="py-4 d-flex justify-center">
    <v-card elevation="2" class="pa-4 card-container">
      <v-card-title class="d-flex justify-space-between align-center">
        <h2>Job Selection</h2>
        <v-btn color="primary" @click="showCreateForm">
          <v-icon left>mdi-plus</v-icon> Add New Job
        </v-btn>
      </v-card-title>

      <!-- Tabel yang menampilkan job name & checkbox untuk kategori -->
      <v-data-table
        :headers="headers"
        :items="selectedJobs"
        class="elevation-1 mt-3"
        dense
        fixed-header
        height="400px"
      >
        <template v-slot:headers>
          <tr>
            <th v-for="header in headers" :key="header.value">
              {{ header.text }}
            </th>
          </tr>
        </template>

        <!-- Render Data Rows -->
        <template v-slot:item="{ item }">
          <tr>
            <td>{{ item.name }}</td>
            <td v-for="category in supportTables" :key="category.id">
              <v-checkbox
                v-model="item.selectedCategories"
                :value="category.id"
                dense
                :disabled="!item.isNew && !item.isEditing"
              ></v-checkbox>
            </td>
            <td>
              <v-btn icon small color="blue" @click="toggleEdit(item)">
                <v-icon>{{ item.isEditing ? "mdi-check" : "mdi-pencil" }}</v-icon>
              </v-btn>
              <v-btn
                v-if="item.isEditing || item.isNew"
                icon
                small
                color="red"
                @click="deleteJob(item)"
              >
                <v-icon>mdi-delete</v-icon>
              </v-btn>
            </td>
          </tr>
        </template>
      </v-data-table>

      <!-- Tombol untuk simpan ke backend, hanya muncul saat menambah job -->
      <!-- Tombol untuk simpan ke backend, hanya muncul saat menambah job -->
      <v-btn
        v-if="isAdding"
        color="success"
        @click="submitToBackend"
        :disabled="!canSubmit"
      >
        <v-icon left>mdi-cloud-upload</v-icon> Submit Jobs to Backend
      </v-btn>

    </v-card>

    <!-- Form Tambah Job -->
    <v-dialog v-model="isFormVisible" persistent max-width="400px">
      <v-card>
        <v-card-title>
          <h3>Select Job</h3>
        </v-card-title>
        <v-card-text>
          <v-form @submit.prevent="saveJob">
            <v-select
              v-model="selectedJob"
              :items="jobs"
              item-title="name"
              item-value="id"
              label="Select Job"
              required
            ></v-select>

            <div class="d-flex justify-end mt-4">
              <v-btn color="grey" @click="cancelEdit" class="mr-2">Cancel</v-btn>
              <v-btn type="submit" color="primary">
                <v-icon left>mdi-content-save</v-icon> Save
              </v-btn>
            </div>
          </v-form>
        </v-card-text>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import axios from "axios";

const jobs = ref([]);
const supportTables = ref([]);
const selectedJobs = ref([]);
const isFormVisible = ref(false);
const selectedJob = ref(null);
const isAdding = ref(false);

// Headers untuk tabel (menampilkan Job Name + Nama Support Tables + Actions)
const headers = ref([{ text: "Job Name", value: "name" }]);

// Cek apakah bisa submit (job baru harus memiliki minimal 1 checkbox dicentang)
// Cek apakah bisa submit (job baru harus memiliki minimal 1 checkbox dicentang)
const canSubmit = computed(() => {
  return selectedJobs.value.some(job => job.isNew && job.selectedCategories.length > 0);
});

const submitToBackend = async () => {
  try {
    const job = selectedJobs.value.find(j => j.isNew);

    if (!job || job.selectedCategories.length === 0) {
      console.error("❌ Harus memilih minimal satu checkbox sebelum submit!");
      return;
    }

    const payload = {
      jobId: parseInt(job.id, 10),
      jobName: job.name,
      supportTableIds: job.selectedCategories.filter(id => id !== null && id !== undefined),
    };

    console.log("📤 Submitting Payload:", JSON.stringify(payload, null, 2));

    await axios.post(
      `${import.meta.env.VITE_APP_BASE_URL}/api/support-engineering-documents`,
      payload,
      { headers: { "Content-Type": "application/json" } }
    );

    console.log("✅ Jobs submitted successfully!");

    // **Reset form setelah submit sukses**
    isAdding.value = false;
    fetchExistingJobs(); // Refresh data dari backend
  } catch (error) {
    console.error("❌ Error submitting jobs:", error.response?.data || error);
  }
};


const showCreateForm = () => {
  isAdding.value = true;
  selectedJob.value = null;
  isFormVisible.value = true;
};

// Fetch daftar job
const fetchJobs = async () => {
  try {
    const response = await axios.get(
      "https://ppic-backend.iotech.my.id/jobs-proto-simple?all=true&withProducts=true&withPurchaseOrders=true"
    );
    jobs.value = response.data.jobs.map((job) => ({
      id: job.masterJavaBaseModel.id,
      name: job.name,
    }));
  } catch (error) {
    console.error("Error fetching jobs:", error);
  }
};

// Fetch daftar support tables
const fetchSupportTables = async () => {
  try {
    const response = await axios.get(
      `${import.meta.env.VITE_APP_BASE_URL}/supporttables`
    );
    supportTables.value = response.data.sort((a, b) => a.id - b.id);

    // Tambahkan ke headers tabel
    headers.value = [
      { text: "Job Name", value: "name" },
      ...supportTables.value.map((category) => ({
        text: category.name,
        value: category.id,
      })),
      { text: "Actions", value: "actions", sortable: false },
    ];
  } catch (error) {
    console.error("Error fetching support tables:", error);
  }
};

// Fetch data dari database
const fetchExistingJobs = async () => {
  try {
    const response = await axios.get(
      `${import.meta.env.VITE_APP_BASE_URL}/api/support-engineering-documents`
    );

    if (response.data && response.data.length > 0) {
      selectedJobs.value = response.data.map((job) => ({
        id: job.jobId,
        name: job.jobName,
        selectedCategories: job.supportTableIds || [],
        isNew: false,
        isEditing: false,
      }));
    }
    isAdding.value = false;
  } catch (error) {
    console.error("Error fetching existing jobs:", error);
  }
};

// Simpan job yang dipilih
const saveJob = () => {
  if (selectedJob.value) {
    const selectedJobData = jobs.value.find((job) => job.id === selectedJob.value);

    selectedJobs.value.push({
      id: selectedJobData.id,
      name: selectedJobData.name,
      selectedCategories: [],
      isNew: true,
      isEditing: true,
    });

    selectedJob.value = null;
    isFormVisible.value = false;
  }
};

// Toggle Edit Mode
const toggleEdit = (job) => {
  if (job.isEditing) {
    updateJob(job);
  }
  job.isEditing = !job.isEditing;
};

// Hapus job
const deleteJob = async (job) => {
  try {
    await axios.delete(`${import.meta.env.VITE_APP_BASE_URL}/api/support-engineering-documents/${job.id}`);
    selectedJobs.value = selectedJobs.value.filter((j) => j.id !== job.id);
    console.log("✅ Job deleted successfully");
  } catch (error) {
    console.error("❌ Error deleting job:", error);
  }
};

// **Update job ke backend setelah edit selesai (PUT)**
const updateJob = async (job) => {
  try {
    const payload = {
      jobId: job.id,
      jobName: job.name,
      supportTableIds: job.selectedCategories.filter(id => id !== null && id !== undefined),
    };

    console.log("🔄 Updating Job:", payload);

    await axios.put(
      `${import.meta.env.VITE_APP_BASE_URL}/api/support-engineering-documents/${job.id}/supporttableid`,
      payload.supportTableIds, // Pastikan ini adalah array `int[]`
      { headers: { "Content-Type": "application/json" } }
    );

    console.log("✅ Job updated successfully!");
    fetchExistingJobs(); // Refresh UI tanpa reload
  } catch (error) {
    console.error("❌ Error updating job:", error.response?.data || error);
  }
};


onMounted(() => {
  fetchJobs();
  fetchSupportTables();
  fetchExistingJobs();
});
</script>
