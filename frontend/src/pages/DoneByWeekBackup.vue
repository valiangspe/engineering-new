<template>
    <v-container>
      <div class="d-flex w-100">
        <div class="flex-grow-1">
          <v-card-title class="headline">Done by week</v-card-title>
          <v-card-text>
            <v-data-table :headers="headers" :items="items" class="elevation-1" hide-default-footer>
              <template v-slot:footer>
                <tr>
                  <td>Done Total</td>
                  <td></td>
                  <td></td>
                  <td>{{ doneTotal.may }}</td>
                  <td>{{ doneTotal.june }}</td>
                  <td>{{ doneTotal.w1July }}</td>
                  <td>{{ doneTotal.w2July }}</td>
                  <td>{{ doneTotal.w3July }}</td>
                  <td>{{ doneTotal.w4July }}</td>
                  <td>{{ grandTotalOuts }}</td>
                </tr>
                <tr>
                  <td>Grand Total</td>
                  <td colspan="7"></td>
                  <td>{{ grandTotalOuts }}</td>
                </tr>
              </template>
            </v-data-table>
          </v-card-text>
        </div>
        <div style="width: 600px;">
          <v-card-title class="headline">Job Distribution</v-card-title>
          <v-card-text>
            <Bar :data="chartData" :options="{
                responsive: true,
                maintainAspectRatio: false
            }" />
          </v-card-text>
        </div>
      </div>
    </v-container>
  </template>
  
  <script setup>
  import { ref, computed } from 'vue';
  import { Bar } from 'vue-chartjs';
  import {
    Chart as ChartJS,
    Title,
    Tooltip,
    Legend,
    BarElement,
    CategoryScale,
    LinearScale
  } from 'chart.js';
  
  ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale);
  
  const headers = ref([
    { title: 'Outs', value: 'outs' },
    { title: 'Done/Outs', value: 'doneOuts' },
    { title: 'Job Category', value: 'jobCategory' },
    { title: 'Status', value: 'status' },
    { title: 'May', value: 'may' },
    { title: 'June', value: 'june' },
    { title: 'W1 July', value: 'w1July' },
    { title: 'W2 July', value: 'w2July' },
    { title: 'W3 July', value: 'w3July' },
    { title: 'W4 July', value: 'w4July' },
    { title: 'Grand Total', value: 'grandTotal' }
  ]);
  
  const items = ref([
    { outs: '', doneOuts: 'Done', jobCategory: 'Post-PO', status: 'Done', may: 1, june: 19, w1July: 8, w2July: 12, w3July: 8, w4July: 0, grandTotal: 48 },
    { outs: '', doneOuts: 'Done', jobCategory: 'Pre-PO', status: 'Done', may: 1, june: 3, w1July: 30, w2July: 23, w3July: 16, w4July: 0, grandTotal: 73 },
    { outs: '', doneOuts: 'Done', jobCategory: 'Others', status: 'Done', may: 0, june: 3, w1July: 20, w2July: 2, w3July: 8, w4July: 6, grandTotal: 39 }
  ]);
  
  const doneTotal = computed(() => {
    return {
      may: items.value.reduce((sum, item) => sum + item.may, 0),
      june: items.value.reduce((sum, item) => sum + item.june, 0),
      w1July: items.value.reduce((sum, item) => sum + item.w1July, 0),
      w2July: items.value.reduce((sum, item) => sum + item.w2July, 0),
      w3July: items.value.reduce((sum, item) => sum + item.w3July, 0),
      w4July: items.value.reduce((sum, item) => sum + item.w4July, 0)
    };
  });
  
  const grandTotalOuts = computed(() => items.value.reduce((sum, item) => sum + item.grandTotal, 0));
  
  const chartData = ref({
    labels: ['Before July', 'W1 July', 'W2 July', 'W3 July', 'W4 July'],
    datasets: [
      {
        label: 'Outs',
        backgroundColor: '#f87979',
        data: [doneTotal.value.may, doneTotal.value.june, doneTotal.value.w1July, doneTotal.value.w2July, doneTotal.value.w3July, doneTotal.value.w4July]
      }
    ]
  });
  </script>
  
  <style>
  .v-data-table {
    margin-bottom: 20px;
  }
  
  .v-card {
    margin-bottom: 20px;
  }
  </style>
  