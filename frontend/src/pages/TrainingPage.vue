<template>
    <v-container>
        <v-row>
        <v-col>
            <v-card-title class="headline">Schedule Training Knowledge 2024</v-card-title>
            <v-card-subtitle>Graha Sumber Prima Elektronik (GSPE)</v-card-subtitle>
            <v-card-text>
              <v-data-table :headers="headers" :items="items" class="elevation-1">
                <template v-slot:item.module="{ item }">
                  <v-icon v-if="item.module">mdi-check</v-icon>
                </template>
              </v-data-table>
            </v-card-text>
            <v-card-subtitle>Note:</v-card-subtitle>
            <v-card-text>
              Target peserta training seluruh pegawai department Engineering, Production dan QA
            </v-card-text>
        </v-col>
        <v-col>
          <!-- <img src="path/to/your/image.png"    alt="Training Image" class="w-100" /> -->
        </v-col>
      </v-row>
       
    </v-container>
  </template>
  
  <script setup>
  import { ref } from 'vue';
  
  const headers = ref([
    { title: 'No', value: 'no' },
    { title: 'Waktu', value: 'waktu' },
    { title: 'Kegiatan', value: 'kegiatan' },
    { title: 'Trainer', value: 'trainer' },
    { title: 'Tempat', value: 'tempat' },
    { title: 'Modul', value: 'module' }
  ]);
  
  const items = ref([
    { no: 1, waktu: 'Selasa, 07 Mei 2024 (DONE)', kegiatan: 'Training Low Voltage Requirement', trainer: 'Asp', tempat: 'Smart Room', module: true },
    { no: 2, waktu: 'Selasa, 14 Mei 2024 (DONE)', kegiatan: 'Training Medium Voltage Requirement', trainer: 'Asp', tempat: 'Smart Room', module: true },
    { no: 3, waktu: 'Jumat, 09 Agustus 2024', kegiatan: 'Training Type Test & Form Enclosure Requirement', trainer: 'Asp', tempat: 'Smart Room', module: true },
    { no: 4, waktu: 'Jumat, 16 Agustus 2024', kegiatan: 'Training Requirement LV Panel (BOQ & Estimation)', trainer: 'Asp', tempat: 'Smart Room', module: true },
    { no: 5, waktu: 'Jumat, 23 Agustus 2024', kegiatan: 'Training Busbar (By Rating Load Label, Short Circuit & External connection)', trainer: 'Asp', tempat: 'Smart Room', module: true },
    { no: 6, waktu: 'Jumat, 30 Agustus 2024', kegiatan: 'Basic Training of Technical Drawing & Measurement Tools Metalwork', trainer: 'Asp', tempat: 'Smart Room', module: true },
    { no: 7, waktu: 'Jumat, 13 September 2024', kegiatan: 'Training Create 2D Technical Drawing (VM, FS, KD, ID/Outdoor)', trainer: 'Asp', tempat: 'Smart Room', module: true },
    { no: 8, waktu: 'Jumat, 20 September 2024', kegiatan: 'Training Knowledge Punching (Basic knowledge, Proses, dan Tooling)', trainer: 'Asp', tempat: 'Smart Room', module: true },
    { no: 9, waktu: 'Jumat, 04 Oktober 2024', kegiatan: 'Training Knowledge Bending (Basic knowledge, Proses, dan Tooling)', trainer: 'Asp', tempat: 'Smart Room', module: true },
    { no: 10, waktu: 'Jumat, 11 Oktober 2024', kegiatan: 'Training Knowledge Welding (Basic knowledge, Proses, dan Tooling)', trainer: 'Asp', tempat: 'Smart Room', module: true }
  ]);
  </script>
  
  <style>
  .v-data-table {
    margin-bottom: 20px;
  }
  
  .v-card {
    margin-bottom: 20px;
  }
  
  .w-100 {
    width: 100%;
  }
  </style>
  