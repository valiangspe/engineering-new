<template>
    <div class="panel-process-page">
      <h1>Panel Process Management</h1>
      <button class="btn btn-add" @click="showCreateForm">Add New Panel Process</button>
  
      <!-- Form for adding/editing panel process -->
      <div v-if="isFormVisible" class="form-container">
        <h2>{{ selectedProcess ? 'Edit' : 'Add' }} Panel Process</h2>
        <form @submit.prevent="saveProcess" class="process-form">
          <div class="form-group">
            <label>Panel Type:</label>
            <input v-model="formData.panelType" required />
          </div>
          <div class="form-group">
            <label>Process Name:</label>
            <input v-model="formData.processName" required />
          </div>
          <div class="form-group">
            <label>Minutes:</label>
            <input type="number" v-model="formData.minutes" required />
          </div>
          <div class="form-actions">
            <button type="submit" class="btn btn-primary">{{ selectedProcess ? 'Update' : 'Create' }}</button>
            <button type="button" class="btn btn-secondary" @click="cancelEdit">Cancel</button>
          </div>
        </form>
      </div>
  
      <!-- Panel Process List -->
      <div class="table-container">
        <h2>Panel Process List</h2>
        <table class="panel-process-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Panel Type</th>
              <th>Process Name</th>
              <th>Minutes</th>
              <!-- <th>Actions</th> -->
            </tr>
          </thead>
          <tbody>
            <tr v-for="process in processes" :key="process.id">
              <td>{{ process.id }}</td>
              <td>{{ process.panelType }}</td>
              <td>{{ process.processName }}</td>
              <td>{{ process.minutes }}</td>
              <!-- <td>
                <button class="btn btn-edit" @click="editProcess(process)">Edit</button>
                <button class="btn btn-delete" @click="deleteProcess(process.id)">Delete</button>
              </td> -->
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </template>
  
  <script setup>
  import { ref, onMounted } from 'vue';
  import axios from 'axios';
  
  const processes = ref([]);
  const isFormVisible = ref(false);
  const selectedProcess = ref(null);
  const formData = ref({
    panelType: '',
    processName: '',
    minutes: 0
  });
  
  // Fetch all panel processes
  const fetchProcesses = async () => {
    try {
      const response = await axios.get(`${import.meta.env.VITE_APP_BASE_URL}/panelprocess`);
      processes.value = response.data;
    } catch (error) {
      console.error('Error fetching panel processes:', error);
    }
  };
  
  // Show the create form
  const showCreateForm = () => {
    isFormVisible.value = true;
    selectedProcess.value = null;
    formData.value = {
      panelType: '',
      processName: '',
      minutes: 0
    };
  };
  
  // Edit an existing process
  const editProcess = (process) => {
    isFormVisible.value = true;
    selectedProcess.value = process;
    formData.value = {
      panelType: process.panelType,
      processName: process.processName,
      minutes: process.minutes
    };
  };
  
  // Save (create or update) the process
  const saveProcess = async () => {
    if (selectedProcess.value) {
      // Update existing process
      try {
        await axios.put(`${import.meta.env.VITE_APP_BASE_URL}/panelprocess/${selectedProcess.value.id}`, formData.value);
        fetchProcesses();
        isFormVisible.value = false;
      } catch (error) {
        console.error('Error updating panel process:', error);
      }
    } else {
      // Create new process
      try {
        await axios.post(`${import.meta.env.VITE_APP_BASE_URL}/panelprocess`, formData.value);
        fetchProcesses();
        isFormVisible.value = false;
      } catch (error) {
        console.error('Error creating panel process:', error);
      }
    }
  };
  
  // Delete a process
  const deleteProcess = async (id) => {
    try {
      await axios.delete(`${import.meta.env.VITE_APP_BASE_URL}/panelprocess/${id}`);
      fetchProcesses();
    } catch (error) {
      console.error('Error deleting panel process:', error);
    }
  };
  
  // Cancel editing
  const cancelEdit = () => {
    isFormVisible.value = false;
    selectedProcess.value = null;
  };
  
  onMounted(() => {
    fetchProcesses();
  });
  </script>
  
  <style scoped>
  .panel-process-page {
    padding: 20px;
    font-family: Arial, sans-serif;
  }
  
  .form-container {
    margin-bottom: 20px;
  }
  
  .process-form {
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
  
  .form-group input {
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
  
  .panel-process-table {
    width: 100%;
    border-collapse: collapse;
    margin-top: 20px;
  }
  
  .panel-process-table th, .panel-process-table td {
    border: 1px solid #ddd;
    padding: 10px;
    text-align: left;
  }
  
  .panel-process-table th {
    background-color: #f4f4f4;
  }
  
  .panel-process-table td {
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
  