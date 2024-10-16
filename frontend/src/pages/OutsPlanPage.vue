<template>
    <v-container>
        <div class="d-flex w-100">
            <div class="flex-grow-1">
                    <v-card-title class="headline">Done</v-card-title>
                    <v-card-text>
                        <v-data-table :headers="headers" :items="items" class="elevation-1" hide-default-footer>
                            <template v-slot:footer>
                                <tr>
                                    <td>Grand Total</td>
                                    <td>{{ grandTotalOuts }}</td>
                                    <td>{{ grandTotalHours }}</td>
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

                        <!-- <BarChart :chartData="chartData" /> -->
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
    { title: 'Job Category', value: 'jobCategory' },
    { title: 'Detail', value: 'detail' },
    { title: 'Outs', value: 'outs' },
    { title: 'T. Prcs (H)', value: 'hours' }
]);

const items = ref([
    { jobCategory: 'Pre-PO', detail: 'Done', outs: 73, hours: 894 },
    { jobCategory: 'Post-PO', detail: 'Done', outs: 48, hours: 400.5 },
    { jobCategory: 'Others', detail: 'Done', outs: 15, hours: 331 }
]);

const grandTotalOuts = computed(() => items.value.reduce((sum, item) => sum + item.outs, 0));
const grandTotalHours = computed(() => items.value.reduce((sum, item) => sum + item.hours, 0));

const chartData = ref({
    labels: items.value.map(item => item.jobCategory),
    datasets: [
        {
            label: 'Outs',
            backgroundColor: '#f87979',
            data: items.value.map(item => item.outs)
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