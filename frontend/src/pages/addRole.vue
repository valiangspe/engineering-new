<template>
    <div class="user-role-page">
      <h1>User Role Management</h1>
      <button class="btn btn-add" @click="showCreateForm">Add New Role</button>
      <!-- Form untuk menambah atau mengedit role -->
      <div v-if="isFormVisible" class="form-container">
        <h2>{{ selectedRole ? 'Edit' : 'Add' }} User Role</h2>
        <form @submit.prevent="saveRole" class="role-form">
          <div class="form-group">
            <label>User:</label>
            <select v-model="formData.userId" required>
              <option value="" disabled>Pilih User</option>
              <option v-for="user in users" :key="user.id" :value="user.id">
                {{ user.username }} (ID: {{ user.id }})
              </option>
            </select>
          </div>
          <div class="form-group">
            <label>Role:</label>
            <select v-model="formData.role" required>
              <option value="pic">PIC</option>
              <option value="spv">SPV</option>
              <option value="manager">Manager</option>
            </select>
          </div>
          <div class="form-actions">
            <button type="submit" class="btn btn-primary">{{ selectedRole ? 'Update' : 'Create' }}</button>
            <button type="button" class="btn btn-secondary" @click="cancelEdit">Cancel</button>
          </div>
        </form>
      </div>
  
      <!-- Daftar User Role -->
      <div class="table-container">
        <h2>User Roles List</h2>
        <table class="user-role-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>User ID</th>
              <th>Role</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="role in roles" :key="role.id">
              <td>{{ role.id }}</td>
              <td>{{ getUserNameById(role.userId) }}</td>
              <td>{{ role.role }}</td>
              <td>
                <button class="btn btn-edit" @click="editRole(role)">Edit</button>
                <button class="btn btn-delete" @click="deleteRole(role.id)">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
  
        
      </div>
    </div>
  </template>
  
  <script>
  import axios from 'axios';
  
  export default {
    data() {
      return {
        roles: [],
        users: [], // Daftar pengguna dari API eksternal
        isFormVisible: false,
        selectedRole: null,
        formData: {
          userId: '',
          role: 'pic'
        }
      };
    },
    methods: {
      fetchRoles() {
        // Ambil data roles dari backend
        axios.get(`${import.meta.env.VITE_APP_BASE_URL}/api/userroles`)  // Sesuaikan URL di sini
          .then(response => {
            this.roles = response.data;
          })
          .catch(error => {
            console.error('Error fetching roles:', error);
          });
      },
      fetchUsers() {
        // Memanggil API eksternal untuk mengambil daftar pengguna
        axios.get('https://meeting-backend.iotech.my.id/ext-users')
          .then(response => {
            this.users = response.data;
          })
          .catch(error => {
            console.error('Error fetching users:', error);
          });
      },
      getUserNameById(userId) {
        const user = this.users.find(user => user.id === userId);
        return user ? user.username : 'Unknown';
      },
      showCreateForm() {
        this.isFormVisible = true;
        this.selectedRole = null;
        this.formData = {
          userId: '',
          role: 'pic'
        };
      },
      editRole(role) {
        this.isFormVisible = true;
        this.selectedRole = role;
        this.formData = {
          userId: role.userId,
          role: role.role
        };
      },
      saveRole() {
        if (this.selectedRole) {
  this.formData.id = this.selectedRole.id; // Tambahkan Id ke formData untuk mencocokkan di backend

  axios.put(`${import.meta.env.VITE_APP_BASE_URL}/api/userroles/${this.selectedRole.id}`, this.formData)
    .then(() => {
      this.fetchRoles();
      this.isFormVisible = false;
    })
    .catch(error => {
      console.error('Error updating role:', error);
    });
}
else {
          // Create new role
          axios.post(`${import.meta.env.VITE_APP_BASE_URL}/api/userroles`, this.formData)  // Perbaiki URL menjadi 'userroles'
            .then(() => {
              this.fetchRoles();
              this.isFormVisible = false;
            })
            .catch(error => {
              console.error('Error creating role:', error);
            });
        }
      },
      deleteRole(id) {
        axios.delete(`${import.meta.env.VITE_APP_BASE_URL}/api/userroles/${id}`)  // Sesuaikan URL delete
          .then(() => {
            this.fetchRoles();
          })
          .catch(error => {
            console.error('Error deleting role:', error);
          });
      },
      cancelEdit() {
        this.isFormVisible = false;
        this.selectedRole = null;
      }
    },
    mounted() {
      this.fetchRoles();
      this.fetchUsers(); // Memanggil data pengguna saat komponen dimuat
    }
  };
  </script>
  

  

<style scoped>
.user-role-page {
  padding: 20px;
  font-family: Arial, sans-serif;
}

.form-container {
  margin-bottom: 20px;
}

.role-form {
  display: flex;
  flex-direction: column;
  max-width: 400px;
  margin: 0 auto;
}

.form-group {
  margin-bottom: 15px;
}

.form-group label {
  display: block;
  font-weight: bold;
  margin-bottom: 5px;
}

.form-group select, .form-group input {
  width: 100%;
  padding: 8px;
  font-size: 14px;
}

.form-actions {
  display: flex;
  justify-content: space-between;
}

.btn {
  padding: 8px 15px;
  font-size: 14px;
  cursor: pointer;
  border: none;
  border-radius: 4px;
}

.btn-primary {
  background-color: #007bff;
  color: white;
}

.btn-secondary {
  background-color: #6c757d;
  color: white;
}

.table-container {
  margin-top: 20px;
}

.user-role-table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 20px;
}

.user-role-table th, .user-role-table td {
  border: 1px solid #ddd;
  padding: 10px;
  text-align: left;
}

.user-role-table th {
  background-color: #f4f4f4;
}

.user-role-table td {
  background-color: #fff;
}

.btn-edit {
  background-color: #28a745;
  color: white;
  margin-right: 5px;
}

.btn-delete {
  background-color: #dc3545;
  color: white;
}

.btn-add {
  background-color: #17a2b8;
  color: white;
  padding: 10px;
  margin-top: 20px;
}
</style>
