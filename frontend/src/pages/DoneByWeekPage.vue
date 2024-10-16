<script setup>
import { computed, ref } from "vue";
import {
  fetchDepartments,
  fetchEngineeringActivities,
  fetchExtCrmPurchaseOrdersProtoSimple,
  fetchInquiries,
  fetchJobsProtoSimple,
  fetchUsers,
  fetchWoTemplates,
} from "./fetchers";
import { intlFormat, makeDateString } from "../helpers";
import chroma from "chroma-js";
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
ChartJS.register(
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale
);

const activities = ref([]);
const users = ref([]);
const departments = ref([]);
const dialog = ref(false);
const selectedActivity = ref(null);
const woTemplates = ref(null);
const pos = ref([]);

const from = ref(makeDateString(new Date()));
const to = ref(makeDateString(new Date()));

const defaultForm = {
  type: "PrePO",
  tasks: [],
};

const form = ref({ ...defaultForm });

// const users = ref([{ id: 1, name: "valian" }]);
const activityTypes = ref(["PrePO", "PostPO", "Others"]);
const inCharges = ref([{ name: "John" }, { name: "Doe" }]);
const selectedInCharges = ref([]);
const jobs = ref({});
const inquiries = ref([]);
// Create refs for selected month and year
const selectedMonth = ref(new Date().getMonth());
const selectedYear = ref(new Date().getFullYear());

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

const fetchJobsProtoSimpleData = async () => {
  const d = await fetchJobsProtoSimple({
    all: true,
    withProducts: true,
    withPurchaseOrders: true,
  });
  jobs.value = d;
};

const fetchPosData = async () => {
  const d = await fetchExtCrmPurchaseOrdersProtoSimple();

  if (d) {
    pos.value = d;
  }
};

const addTask = () => {
  form.value = {
    ...form.value,
    tasks: [
      ...form.value.tasks,
      {
        from: new Date(`${makeDateString(new Date())}T00:00:00Z`).toISOString(),
        to: new Date(`${makeDateString(new Date())}T00:00:00Z`).toISOString(),
      },
    ],
  };
};

const removeTask = (index) => {
  form.value = {
    ...form.value,
    tasks: form.value.tasks.filter((_, i) => i !== index),
  };
};

// const submit = handleSubmit((values) => {
//   console.log("Form submitted:", values);
// });

const fetchUsersData = async () => {
  const d = await fetchUsers();
  console.log("users", d);
  users.value = d;
};

const fetchInquiriesData = async () => {
  const d = await fetchInquiries();
  console.log("inquiries", d);
  inquiries.value = d;
};

const fetchDepartmentsData = async () => {
  const d = await fetchDepartments();
  departments.value = d;
};

const fetchEngineeringActivitiesData = async () => {
  const d = await fetchEngineeringActivities({
    from: new Date(`${dateRange.value.from}T00:00:00`).toISOString(),
    to: new Date(`${dateRange.value.to}T23:59:39`).toISOString(),
  });
  activities.value = d;
};

const fetchWoTemplatesData = async () => {
  const d = await fetchWoTemplates();
  woTemplates.value = d;
};

const handleSave = async () => {
  console.log(form.value);
  try {
    const resp = await fetch(
      `${import.meta.env.VITE_APP_BASE_URL}/api/dashboard/activities`,
      {
        method: "post",
        headers: { "content-type": "application/json" },
        body: JSON.stringify(form.value),
      }
    );

    dialog.value = false;
    form.value = { ...defaultForm };

    fetchEngineeringActivitiesData();
  } catch (e) {}
};

fetchUsersData();
fetchEngineeringActivitiesData();
fetchDepartmentsData();
fetchJobsProtoSimpleData();
fetchWoTemplatesData();
fetchInquiriesData();
fetchPosData();

const alertx = (content) => {
  alert(content);
};

// Function to get the week number of a date
const getWeekNumber = (date) => {
  const d = new Date(
    Date.UTC(date.getFullYear(), date.getMonth(), date.getDate())
  );
  const dayNum = d.getUTCDay() || 7;
  d.setUTCDate(d.getUTCDate() + 4 - dayNum);
  const yearStart = new Date(Date.UTC(d.getUTCFullYear(), 0, 1));
  return Math.ceil(((d - yearStart) / 86400000 + 1) / 7);
};

// Function to get the first day of the month
const getFirstDayOfMonth = (date) => {
  return new Date(date.getFullYear(), date.getMonth(), 1);
};

// Function to get the first and last day of a week in a month
const getWeekBounds = (year, month, weekNumber) => {
  const firstDayOfMonth = new Date(year, month, 1);
  const firstMonday = new Date(firstDayOfMonth);
  firstMonday.setDate(firstMonday.getDate() + ((8 - firstMonday.getDay()) % 7));

  const weekStart = new Date(firstMonday);
  weekStart.setDate(weekStart.getDate() + 7 * (weekNumber - 1));

  const weekEnd = new Date(weekStart);
  weekEnd.setDate(weekEnd.getDate() + 6);

  return { start: weekStart, end: weekEnd };
};

// Process the data to group activities by week and type
const processedData = computed(() => {
  let result = {
    PrePO: { w1: 0, w2: 0, w3: 0, w4: 0 },
    PostPO: { w1: 0, w2: 0, w3: 0, w4: 0 },
    Others: { w1: 0, w2: 0, w3: 0, w4: 0 },
    Total: { w1: 0, w2: 0, w3: 0, w4: 0 },
  };

  if (activities.value.length === 0) return result;

  const firstActivity = new Date(activities.value[0].fromCache);
  const year = firstActivity.getFullYear();
  const month = firstActivity.getMonth();

  for (let week = 1; week <= 4; week++) {
    const { start, end } = getWeekBounds(year, month, week);

    activities.value.forEach((activity) => {
      const activityStart = new Date(activity.fromCache);
      const activityEnd = new Date(activity.toCache);

      if (activityStart <= end && activityEnd >= start) {
        result[activity.type][`w${week}`]++;
      }
    });
  }

  [...Array(4)]
    .map((_, i) => i + 1)
    .forEach((n) => {
      const wKey = `w${n}`;

      Object.entries(result).forEach((r) => {
        if (r?.[0] === "Total") {
          return;
        }

        const val = r?.[1]?.[wKey];

        console.log(wKey, val, "cur:", result.Total[wKey]);

        result.Total[wKey] += val;
      });
    });

  console.log(result);

  return result;
});

const headers = ref([
  { title: "Category", key: "category" },
  { title: "W1", key: "w1" },
  { title: "W2", key: "w2" },
  { title: "W3", key: "w3" },
  { title: "W4", key: "w4" },
  { title: "Total", key: "total" },
]);

const items = computed(() => {
  return Object.entries(processedData.value).map(([category, data]) => ({
    category,
    ...data,
    total: Object.values(data).reduce((sum, value) => sum + value, 0),
  }));
});

const totals = computed(() => ({
  w1: items.value?.reduce((sum, item) => sum + item.w1, 0),
  w2: items.value?.reduce((sum, item) => sum + item.w2, 0),
  w3: items.value?.reduce((sum, item) => sum + item.w3, 0),
  w4: items.value?.reduce((sum, item) => sum + item.w4, 0),
  total: items.value?.reduce((sum, item) => sum + item.total, 0),
}));

const chartData = computed(() => ({
  labels: ["Week 1", "Week 2", "Week 3", "Week 4"],
  datasets: [
    {
      label: "Pre-PO",
      backgroundColor: "#4CAF50",
      data: [
        processedData.value?.["PrePO"].w1,
        processedData.value?.["PrePO"].w2,
        processedData.value?.["PrePO"].w3,
        processedData.value?.["PrePO"].w4,
      ],
    },
    {
      label: "Post-PO",
      backgroundColor: "#2196F3",
      data: [
        processedData.value?.["PostPO"].w1,
        processedData.value?.["PostPO"].w2,
        processedData.value?.["PostPO"].w3,
        processedData.value?.["PostPO"].w4,
      ],
    },
    {
      label: "Others",
      backgroundColor: "#FFC107",
      data: [
        processedData.value?.["Others"].w1,
        processedData.value?.["Others"].w2,
        processedData.value?.["Others"].w3,
        processedData.value?.["Others"].w4,
      ],
    },
  ],
}));

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  scales: {
    x: {
      stacked: true,
    },
    y: {
      stacked: true,
    },
  },
};
</script>
<template>
  <div class="m-3">
    <div>
      <div><h4>Activity</h4></div>
    </div>
    <div><hr class="border border-dark" /></div>
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
  </div>

  <!-- WO dialog -->

  <div class="d-flex w-100">
    <div class="flex-grow-1">
      <v-card-title class="headline">Activities by Week</v-card-title>
      <v-card-text>
        <v-data-table :headers="headers" :items="items" class="elevation-1">
          <template v-slot:bottom>
            
          </template>
        </v-data-table>
      </v-card-text>
    </div>
    <div style="width: 600px">
      <v-card-title class="headline">Activity Distribution</v-card-title>
      <v-card-text>
        <Bar :data="chartData" :options="chartOptions" />
      </v-card-text>
    </div>
  </div>
</template>
