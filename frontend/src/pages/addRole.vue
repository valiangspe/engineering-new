<template>
  <v-container class="py-4 d-flex justify-center">
    <v-card elevation="2" class="pa-4 card-container">
      <v-card-title class="d-flex justify-space-between align-center">
        <h2>User Role Management</h2>
        <v-btn color="primary" @click="showCreateForm">
          <v-icon left>mdi-plus</v-icon> Add New Role
        </v-btn>
      </v-card-title>

      <!-- Tabel User Role -->
      <v-data-table
        :headers="headers"
        :items="roles"
        class="elevation-1 mt-3"
        dense
      >
        <template v-slot:item.userId="{ item }">
          {{ getUserNameById(item.userId) }}
        </template>

        <template v-slot:item.actions="{ item }">
          <v-btn icon small color="green" @click="editRole(item)">
            <v-icon>mdi-pencil</v-icon>
          </v-btn>
          <v-btn icon small color="red" @click="deleteRole(item.id)">
            <v-icon>mdi-delete</v-icon>
          </v-btn>
        </template>
      </v-data-table>
    </v-card>

    <!-- Form Tambah/Edit Role -->
    <v-dialog v-model="isFormVisible" persistent max-width="400px">
      <v-card>
        <v-card-title>
          <h3>{{ selectedRole ? "Edit" : "Add" }} User Role</h3>
        </v-card-title>
        <v-card-text>
          <v-form @submit.prevent="saveRole">
            <v-select
              v-model="formData.userId"
              :items="users.map(user => ({ title: user.username, value: user.id }))"
              label="Select User"
              required
            ></v-select>

            <v-select
              v-model="formData.role"
              :items="rolesList"
              label="Select Role"
              required
            ></v-select>

            <div class="d-flex justify-end mt-4">
              <v-btn color="grey" @click="cancelEdit" class="mr-2">Cancel</v-btn>
              <v-btn type="submit" color="primary">
                <v-icon left>mdi-content-save</v-icon>
                {{ selectedRole ? "Update" : "Create" }}
              </v-btn>
            </div>
          </v-form>
        </v-card-text>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script setup>
import { ref, onMounted } from "vue";
import axios from "axios";

const roles = ref([]);
const users = ref([]);
const isFormVisible = ref(false);
const selectedRole = ref(null);
const formData = ref({
  userId: "",
  role: "pic",
});

const rolesList = [
  { title: "PIC", value: "pic" },
  { title: "SPV", value: "spv" },
  { title: "Manager", value: "manager" },
];

const headers = [
  { text: "ID", value: "id", sortable: false },
  { text: "User", value: "userId", sortable: false },
  { text: "Role", value: "role", sortable: false },
  { text: "Actions", value: "actions", align: "end", sortable: false },
];

const fetchRoles = async () => {
  try {
    const response = await axios.get(`${import.meta.env.VITE_APP_BASE_URL}/api/userroles`);
    roles.value = response.data;
  } catch (error) {
    console.error("Error fetching roles:", error);
  }
};

const fetchUsers = async () => {
  try {
    const response = await axios.get("https://meeting-backend.iotech.my.id/ext-users");
    users.value = response.data;
  } catch (error) {
    console.error("Error fetching users:", error);
  }
};

const getUserNameById = (userId) => {
  const user = users.value.find((user) => user.id === userId);
  return user ? user.username : "Unknown";
};

const showCreateForm = () => {
  isFormVisible.value = true;
  selectedRole.value = null;
  formData.value = { userId: "", role: "pic" };
};

const editRole = (role) => {
  isFormVisible.value = true;
  selectedRole.value = role;
  formData.value = { userId: role.userId, role: role.role };
};

const saveRole = async () => {
  try {
    if (selectedRole.value) {
      await axios.put(`${import.meta.env.VITE_APP_BASE_URL}/api/userroles/${selectedRole.value.id}`, formData.value);
    } else {
      await axios.post(`${import.meta.env.VITE_APP_BASE_URL}/api/userroles`, formData.value);
    }
    isFormVisible.value = false;
    fetchRoles();
  } catch (error) {
    console.error("Error saving role:", error);
  }
};

const deleteRole = async (id) => {
  try {
    await axios.delete(`${import.meta.env.VITE_APP_BASE_URL}/api/userroles/${id}`);
    fetchRoles();
  } catch (error) {
    console.error("Error deleting role:", error);
  }
};

const cancelEdit = () => {
  isFormVisible.value = false;
  selectedRole.value = null;
};

onMounted(() => {
  fetchRoles();
  fetchUsers();
});
</script>

<style scoped>
.v-container {
  max-width: 2000px; /* Perbesar agar lebih proporsional */
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
