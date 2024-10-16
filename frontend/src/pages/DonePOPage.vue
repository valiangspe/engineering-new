<template>
  <v-container>
    <div class="d-flex w-100">
      <div class="flex-grow-1">
        <v-card-title class="headline">Done</v-card-title>

        <div class="d-flex align-items-end">
          <div class="me-2">
            <div>Month</div>
            <v-autocomplete
              :items="months"
              :item-title="(item) => item.label"
              :model-value="months.find((m) => m.value === selectedMonth)"
              @update:model-value="
                (selected) => {
                  selectedMonth = selected;
                  fetchEngineeringActivitiesData();
                }
              "
              style="width: 150px"
            ></v-autocomplete>
          </div>
          <div class="me-2" style="width: 150px">
            <div>Year</div>
            <v-autocomplete
              class="w-100"
              :items="years"
              :item-title="(item) => item.label"
              :model-value="years.find((y) => y.value === selectedYear)"
              @update:model-value="
                (selected) => {
                  selectedYear = selected;
                  fetchEngineeringActivitiesData();
                }
              "
            ></v-autocomplete>
          </div>
          <!-- <div class="mx-2">
        <button class="btn btn-sm btn-primary" @click="dialog = true">
          <v-icon icon="mdi-plus" /> Add
        </button>
      </div> -->
        </div>

        <v-card-text>
          <v-data-table
            :headers="headers"
            :items="items"
            class="elevation-1"
            hide-default-footer
          >
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
      <div style="width: 600px">
        <v-card-title class="headline">Job Distribution</v-card-title>
        <v-card-text>
          <Bar
            :data="chartData"
            :options="{
              responsive: true,
              maintainAspectRatio: false,
            }"
          />
        </v-card-text>
      </div>
    </div>
  </v-container>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import axios from "axios";
import { Bar } from "vue-chartjs";
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale,
} from "chart.js";
import { fetchEngineeringActivities } from "./fetchers";

ChartJS.register(
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale
);

// Create refs for selected month and year
const selectedMonth = ref(new Date().getMonth());
const selectedYear = ref(new Date().getFullYear());

// Generate months array
const months = [
  "January",
  "February",
  "March",
  "April",
  "May",
  "June",
  "July",
  "August",
  "September",
  "October",
  "November",
  "December",
].map((month, index) => ({ label: month, value: index }));

// Generate years array (-10 years, current year, +10 years)
const currentYear = new Date().getFullYear();
const years = Array.from({ length: 21 }, (_, i) => currentYear - 10 + i).map(
  (year) => ({ label: year.toString(), value: year })
);

// Compute the first and last day of the selected month
const dateRange = computed(() => {
  console.log(selectedYear.value, selectedMonth.value);
  const firstDay = new Date(selectedYear.value, selectedMonth.value, 1);
  const lastDay = new Date(selectedYear.value, selectedMonth.value + 1, 0);

  console.log(firstDay, lastDay);
  return {
    from: firstDay.toISOString().split("T")[0],
    to: lastDay.toISOString().split("T")[0],
  };
});

const headers = ref([
  { title: "Job Category", value: "jobCategory" },
  { title: "Detail", value: "detail" },
  { title: "Done", value: "done" },
  { title: "T. Prcs (H)", value: "tProcess" },
]);

const items = ref([]);

const fetchDonePO = async () => {
  try {
    const response = await axios.get(
      `${import.meta.env.VITE_APP_BASE_URL}/donePOs`
    );
    // items.value = response.data;
    updateChartData();
  } catch (error) {
    console.error("Error fetching data:", error);
  }
};

const jobCategories = ["PrePO", "PostPO", "Others"];

const itemsx = ref([
  { jobCategory: "PrePO", detail: "Done", done: 73, hours: 894 },
  { jobCategory: "PostPO", detail: "Done", done: 48, hours: 400.5 },
  { jobCategory: "Others", detail: "Done", done: 15, hours: 331 },
]);

onMounted(fetchDonePO);

const grandTotalOuts = computed(() =>
  items.value.reduce((sum, item) => sum + item.outs, 0)
);
const grandTotalHours = computed(() =>
  items.value.reduce((sum, item) => sum + item.hours, 0)
);

const chartData = computed(() => {
  return {
    labels: items.value.map((item) => item.jobCategory),
    datasets: [
      {
        label: "Done",
        backgroundColor: "#f87979",
        data: items.value.map((item) => item.done),
      },
    ],
  };
});

const activities = ref([]);

const updateChartData = () => {
  chartData.value.labels = items.value.map((item) => item.jobCategory);
  chartData.value.datasets[0].data = items.value.map((item) => item.outs);
};

const fetchEngineeringActivitiesData = async () => {
  const d = await fetchEngineeringActivities({
    from: new Date(`${dateRange.value.from}T00:00:00`).toISOString(),
    to: new Date(`${dateRange.value.to}T23:59:39`).toISOString(),
  });
  activities.value = d;

  items.value = jobCategories.map((c) => {
    const activitiesGot = d?.filter(
      (a) => a?.type === c && !a?.tasks.find((t) => !t?.completedDate)
    );

    return {
      jobCategory: c,
      detail: "Done",
      done: activitiesGot?.length,
      tProcess: activitiesGot.reduce(
        (acc, a) =>
          acc +
          a?.tasks
            ?.filter((t) => !t?.deletedAt)
            ?.reduce((acc, t) => acc + (t?.hours ?? 0), 0.0),
        0.0
      ),
    };
  });
};

fetchEngineeringActivitiesData();
</script>

<style>
.v-data-table {
  margin-bottom: 20px;
}

.v-card {
  margin-bottom: 20px;
}
</style>
