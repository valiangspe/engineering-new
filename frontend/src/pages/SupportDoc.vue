<template>
    <v-container class="py-4 d-flex justify-center">
      <v-card elevation="2" class="pa-4 card-container">
        <v-card-title class="d-flex justify-space-between align-center">
          <h2>Support Table Management</h2>
          <v-btn color="primary" @click="showCreateForm">
            <v-icon left>mdi-plus</v-icon> Add New Support
          </v-btn>
        </v-card-title>
  
        <!-- Tabel Support Table -->
        <v-data-table
          :headers="headers"
          :items="supportTables"
          class="elevation-1 mt-3"
          dense
        >
          <!-- <template v-slot:item.file="{ item }">
            <v-btn v-if="item.filePath" color="blue" class="mr-2" small @click="previewFile(item.filePath)">
              <v-icon left>mdi-eye</v-icon> Preview
            </v-btn>
            
            <v-btn v-if="item.filePath" color="green" small @click="downloadFile(item.filePath)">
              <v-icon left>mdi-download</v-icon> Download
            </v-btn>
            <span v-else>No File</span>
          </template> -->

  
          <template v-slot:item.actions="{ item }">
            <v-btn icon small color="green" @click="editSupport(item)">
              <v-icon>mdi-pencil</v-icon>
            </v-btn>
            <v-btn icon small color="red" @click="deleteSupport(item.id)">
              <v-icon>mdi-delete</v-icon>
            </v-btn>
          </template>
        </v-data-table>
      </v-card>
  
      <!-- Form Tambah/Edit Support -->
      <v-dialog v-model="isFormVisible" persistent max-width="400px">
        <v-card>
          <v-card-title>
            <h3>{{ selectedSupport ? "Edit" : "Add" }} Support</h3>
          </v-card-title>
          <v-card-text>
            <v-form @submit.prevent="saveSupport">
              <v-text-field
                v-model="formData.name"
                label="Support Name"
                required
              ></v-text-field>
  
              <v-file-input
                label="Upload File"
                accept=".jpg,.png,.pdf,.doc,.docx,.xls,.xlsx,.zip"
                @change="handleFileUpload"
              ></v-file-input>
  
              <div class="d-flex justify-end mt-4">
                <v-btn color="grey" @click="cancelEdit" class="mr-2">Cancel</v-btn>
                <v-btn type="submit" color="primary">
                  <v-icon left>mdi-content-save</v-icon>
                  {{ selectedSupport ? "Update" : "Create" }}
                </v-btn>
              </div>
            </v-form>
          </v-card-text>
        </v-card>
      </v-dialog>

      <v-dialog v-model="isPreviewVisible" max-width="80%">
        <v-card>
          <v-card-title>
            <h3>Preview File</h3>
          </v-card-title>
          <v-card-text class="d-flex justify-center">
            <template v-if="previewFileType === 'image'">
              <img :src="previewFileUrl" style="max-width: 100%; max-height: 500px;" />
            </template>
            <template v-else-if="previewFileType === 'pdf'">
              <iframe :src="previewFileUrl" width="100%" height="500px"></iframe>
            </template>
            <template v-else-if="previewFileType === 'google-viewer'">
              <iframe :src="previewFileUrl" width="100%" height="500px"></iframe>
            </template>
            <template v-else>
              <p>Preview tidak tersedia untuk format ini.</p>
            </template>
          </v-card-text>
          <v-card-actions class="justify-end">
            <v-btn color="grey" @click="isPreviewVisible = false">Close</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>

    </v-container>
  </template>
  
  <script setup>
  import { ref, onMounted } from "vue";
  import axios from "axios";
//   import { ref } from "vue";

  const supportTables = ref([]);
  const isFormVisible = ref(false);
  const selectedSupport = ref(null);
  const formData = ref({
    name: "",
    file: null,
  });
  const fileInput = ref(null);
  const isPreviewVisible = ref(false);
  const previewFileUrl = ref("");
  const previewFileType = ref("");

  const previewFile = async (filePath) => {
    const fileExtension = filePath.split('.').pop().toLowerCase();
    const fileUrl = `${import.meta.env.VITE_APP_BASE_URL}/supporttables/download/${encodeURIComponent(filePath)}`;

    if (["jpg", "png", "jpeg", "gif", "svg"].includes(fileExtension)) {
      previewFileType.value = "image";
      previewFileUrl.value = fileUrl;
    } else if (fileExtension === "pdf") {
      previewFileType.value = "pdf";
      previewFileUrl.value = fileUrl;
    } else if (["doc", "docx", "xls", "xlsx"].includes(fileExtension)) {
      previewFileType.value = "google-viewer";
      previewFileUrl.value = `https://docs.google.com/gview?url=${fileUrl}&embedded=true`;
    } else {
      previewFileType.value = "unknown";
    }

    isPreviewVisible.value = true;
  };
  const headers = [
    { text: "ID", value: "id", sortable: false },
    { text: "Name", value: "name", sortable: false },
    { text: "File", value: "file", sortable: false },
    { text: "Actions", value: "actions", align: "end", sortable: false },
  ];
  
  const fetchSupportTables = async () => {
    try {
      const response = await axios.get(`${import.meta.env.VITE_APP_BASE_URL}/supporttables`);
      supportTables.value = response.data;
    } catch (error) {
      console.error("Error fetching support tables:", error);
    }
  };
  
  const showCreateForm = () => {
    isFormVisible.value = true;
    selectedSupport.value = null;
    formData.value = { name: "", file: null };
  };
  
  const editSupport = (support) => {
    isFormVisible.value = true;
    selectedSupport.value = support;
    formData.value = { name: support.name, id: support.id }; // Pastikan ID disimpan
  };

  
  const handleFileUpload = (event) => {
    formData.value.file = event.target.files[0];
  };
  
  const saveSupport = async () => {
    try {
      let supportId = null;

      if (selectedSupport.value) {
        supportId = selectedSupport.value.id; // Pastikan ID diambil dari data yang dipilih
        await axios.put(`${import.meta.env.VITE_APP_BASE_URL}/supporttables/${supportId}`, {
          id: supportId, // Pastikan ID dikirim dalam body request
          name: formData.value.name,
        });
      } else {
        const response = await axios.post(`${import.meta.env.VITE_APP_BASE_URL}/supporttables`, {
          name: formData.value.name,
        });
        supportId = response.data.id;
      }

      if (formData.value.file && supportId) {
        const formDataUpload = new FormData();
        formDataUpload.append("file", formData.value.file);

        await axios.post(`${import.meta.env.VITE_APP_BASE_URL}/supporttables/${supportId}/upload`, formDataUpload, {
          headers: { "Content-Type": "multipart/form-data" },
        });
      }

      isFormVisible.value = false;
      fetchSupportTables();
    } catch (error) {
      console.error("Error saving support:", error);
    }
  };

  const deleteSupport = async (id) => {
    try {
      await axios.delete(`${import.meta.env.VITE_APP_BASE_URL}/supporttables/${id}`);
      fetchSupportTables();
    } catch (error) {
      console.error("Error deleting support:", error);
    }
  };
  
  const cancelEdit = () => {
    isFormVisible.value = false;
    selectedSupport.value = null;
  };
  
  const downloadFile = async (filePath) => {
  try {
    const response = await axios.get(
      `${import.meta.env.VITE_APP_BASE_URL}/supporttables/download/${encodeURIComponent(filePath)}`,
      { responseType: "blob" }
    );

    const url = window.URL.createObjectURL(new Blob([response.data]));
    const link = document.createElement("a");
    link.href = url;
    link.setAttribute("download", filePath.split("/").pop());
    document.body.appendChild(link);
    link.click();
    link.remove();
  } catch (error) {
    console.error("Error downloading file:", error);
  }
};
  
  onMounted(() => {
    fetchSupportTables();
  });
  </script>
  
  <style scoped>
  .v-container {
    max-width: 1200px;
    margin: auto;
  }
  
  .card-container {
    max-width: 75%;
    width: 100%;
    padding: 24px;
    border-radius: 10px;
  }
  
  .v-btn {
    text-transform: none;
  }
  
  .v-data-table {
    margin-top: 20px;
    width: 100%;
  }
  
  .v-card {
    border-radius: 20px;
  }
  </style>
  