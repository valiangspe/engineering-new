<template>
    <div class="container mx-auto p-4">
      <div class="flex items-center mb-4">
        <!-- <img src="/api/placeholder/50/50" alt="GSPE Logo" class="mr-2" /> -->
        <h1 class="text-2xl font-bold">Graha Surabaya Prime Elektronik</h1>
      </div>
      <h2 class="text-3xl font-bold mb-4">Done</h2>
      <div class="grid grid-cols-2 gap-4">
        <div>
          <table class="w-full border-collapse border border-gray-300">
            <thead>
              <tr class="bg-blue-500 text-white">
                <th class="border border-gray-300 p-2">Job Category</th>
                <th class="border border-gray-300 p-2">Detail</th>
                <th class="border border-gray-300 p-2">Outs</th>
                <th class="border border-gray-300 p-2">T. Prcs (H)</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(row, index) in tableData" :key="index" :class="{'bg-blue-100': index % 2 === 0}">
                <td class="border border-gray-300 p-2">{{ row.category }}</td>
                <td class="border border-gray-300 p-2">{{ row.detail }}</td>
                <td class="border border-gray-300 p-2">{{ row.outs }}</td>
                <td class="border border-gray-300 p-2">{{ row.tPrcs }}</td>
              </tr>
            </tbody>
            <tfoot>
              <tr class="font-bold">
                <td class="border border-gray-300 p-2" colspan="2">Grand Total</td>
                <td class="border border-gray-300 p-2">{{ grandTotal.outs }}</td>
                <td class="border border-gray-300 p-2">{{ grandTotal.tPrcs }}</td>
              </tr>
            </tfoot>
          </table>
        </div>
        <div>
          <bar-chart :chart-data="chartData" />
        </div>
      </div>
    </div>
  </template>
  
  <script setup>
  import { ref, computed } from 'vue';
  import { Bar } from 'vue-chartjs';
  import { Chart as ChartJS, Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale } from 'chart.js';
  
  ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale);
  
  const tableData = ref([
    { category: 'Pre-PO', detail: 'Done', outs: 73, tPrcs: 894 },
    { category: 'Post-PO', detail: 'Done', outs: 48, tPrcs: 400.5 },
    { category: 'Others', detail: 'Done', outs: 15, tPrcs: 331 },
  ]);
  
  const grandTotal = computed(() => ({
    outs: tableData.value.reduce((sum, row) => sum + row.outs, 0),
    tPrcs: tableData.value.reduce((sum, row) => sum + row.tPrcs, 0),
  }));
  
  const chartData = computed(() => ({
    labels: tableData.value.map(row => row.category),
    datasets: [{
      label: 'T. Prcs (H)',
      data: tableData.value.map(row => row.tPrcs),
      backgroundColor: 'rgba(255, 99, 132, 0.8)',
    }],
  }));
  
  const BarChart = {
    extends: Bar,
    props: ['chartData'],
    mounted() {
      this.renderChart(this.chartData, {
        responsive: true,
        maintainAspectRatio: false,
      });
    },
  };
  </script>