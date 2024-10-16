<template>
  <v-container>
    <v-row>
      <v-col>
        <v-card>
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
                  <td>{{ grandTotal.onTime }}</td>
                  <td>{{ grandTotal.overDeadline }}</td>
                  <td>{{ grandTotal.total }}</td>
                </tr>
              </template>
            </v-data-table>
          </v-card-text>
        </v-card>
      </v-col>
      <v-col>
        <v-card>
          <v-card-title class="headline">Detail over deadline</v-card-title>
          <v-card-text>
            <Doughnut
              :data="donutChartData"
              :options="{
                responsive: true,
                maintainAspectRatio: false,
              }"
            />
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <v-card>
          <v-card-title class="headline">Pre-PO</v-card-title>
          <v-card-text>
            <v-data-table
              :headers="prePoHeaders"
              :items="prePoItems"
              class="elevation-1"
              hide-default-footer
            >
              <template v-slot:footer>
                <tr>
                  <td>Grand Total</td>
                  <td>{{ prePoGrandTotal.overDeadline }}</td>
                </tr>
              </template>
            </v-data-table>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup>
import { ref, computed } from "vue";
import { Doughnut } from "vue-chartjs";
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  ArcElement,
  CategoryScale,
  LinearScale,
} from "chart.js";
import { fetchEngineeringActivities } from "./fetchers";

ChartJS.register(
  Title,
  Tooltip,
  Legend,
  ArcElement,
  CategoryScale,
  LinearScale
);

// Create refs for selected month and year
const selectedMonth = ref(new Date().getMonth());
const selectedYear = ref(new Date().getFullYear());
const activities = ref([]);

const requiredRule = [(v) => !!v || "Required."];

const menu = ref(false);

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

const fetchEngineeringActivitiesData = async () => {
  const d = await fetchEngineeringActivities({
    from: new Date(`${dateRange.value.from}T00:00:00`).toISOString(),
    to: new Date(`${dateRange.value.to}T23:59:39`).toISOString(),
  });
  activities.value = d;
};
fetchEngineeringActivitiesData();

const headers = ref([
  { title: "Row Labels", value: "rowLabels" },
  { title: "On time", value: "onTime" },
  { title: "Over Deadline", value: "overDeadline" },
  { title: "Grand Total", value: "total" },
]);

const activityTypes = ["PrePO", "PostPO", "Others"];

const items = computed(() => {
  return activityTypes.map((t) => {
    const filteredActivities = activities.value.filter((a) => a.type === t);

    const tasks = filteredActivities.flatMap((a) => a.tasks);

    const tasksCompletedOnTime = tasks.filter((t) =>
      t.completedDate && t.to
        ? new Date(t.completedDate).getTime() <= new Date(t.to).getTime()
        : false
    );

    const tasksCompletedOverDeadline = tasks.filter((t) =>
      t.completedDate
        ? t.completedDate && t.to
          ? new Date(t.completedDate).getTime() >= new Date(t.to).getTime()
          : false
        : true
    );

    return {
      rowLabels: t,
      onTime: tasksCompletedOnTime?.length,
      overDeadline: tasksCompletedOverDeadline?.length,
      total:
        (tasksCompletedOnTime.length ?? 0) +
        (tasksCompletedOverDeadline.length ?? 0),
    };
  });
  return [{ rowLabels: "Pre-PO", onTime: 59, overDeadline: 14, total: 73 }];
});

const grandTotal = computed(() => {
  return {
    onTime: items.value.reduce((sum, item) => sum + item.onTime, 0),
    overDeadline: items.value.reduce((sum, item) => sum + item.overDeadline, 0),
    total: items.value.reduce((sum, item) => sum + item.total, 0),
  };
});

const prePoHeaders = ref([
  { title: "Job Category", value: "jobCategory" },
  { title: "Detail Over deadline", value: "detailOverDeadline" },
  { title: `W1 ${months[selectedMonth.value]?.label}`, value: "w1" },
  { title: `W2 ${months[selectedMonth.value]?.label}`, value: "w2" },
  { title: `W3 ${months[selectedMonth.value]?.label}`, value: "w3" },
  { title: `W4 ${months[selectedMonth.value]?.label}`, value: "w4" },
  { title: "Grand Total", value: "grandTotal" },
]);

const prePoItems = ref([
  {
    jobCategory: "Pre-PO",
    detailOverDeadline: "Capacity man power",
    w1: 1,
    w2: 1,
    w3: 1,
    w4: 1,
    grandTotal: 4,
  },
  {
    jobCategory: "Pre-PO",
    detailOverDeadline: "CRO quoted",
    w1: 0,
    w2: 1,
    w3: 0,
    w4: 0,
    grandTotal: 1,
  },
  {
    jobCategory: "Pre-PO",
    detailOverDeadline: "Problem data",
    w1: 1,
    w2: 1,
    w3: 1,
    w4: 0,
    grandTotal: 3,
  },
  {
    jobCategory: "Pre-PO",
    detailOverDeadline: "Wait price",
    w1: 1,
    w2: 1,
    w3: 1,
    w4: 2,
    grandTotal: 5,
  },
]);

const prePoGrandTotal = computed(() => {
  return {
    overDeadline: prePoItems.value.reduce(
      (sum, item) => sum + item.grandTotal,
      0
    ),
  };
});

const donutChartData = ref({
  labels: [
    "Wait price vendor",
    "Capacity man power",
    "Problem data",
    "CRO quoted",
  ],
  datasets: [
    {
      label: "Detail over deadline",
      backgroundColor: ["#FF6384", "#36A2EB", "#FFCE56", "#FF6384"],
      data: [7, 4, 3, 1],
    },
  ],
});
</script>

<style>
.v-data-table {
  margin-bottom: 20px;
}

.v-card {
  margin-bottom: 20px;
}

.doughnut-chart-container {
  position: relative;
  width: 100%;
  height: 400px;
}
</style>
