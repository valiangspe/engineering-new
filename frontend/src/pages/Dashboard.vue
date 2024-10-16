<!-- src/components/Dashboard.vue -->
<template>
  <div class="container">
    <h1>Engineering Dashboard</h1>
    <div class="row mb-4">
      <div class="col-md-6">
        <div class="card">
          <div class="card-body">
            <h5 class="card-title">Number of Manpowers</h5>
            <h4>{{ numberOfManpowers }}</h4>
          </div>
        </div>
      </div>
      <div class="col-md-6">
        <div class="card">
          <div class="card-body">
            <h5 class="card-title">Number of ECNs this Month</h5>
            <h3>{{ numberOfECNsThisMonth }}</h3>
          </div>
        </div>
      </div>
      <div class="col-md-6 mt-4">
        <div class="card">
          <div class="card-body">
            <h5 class="card-title">Number of CCNs this Month</h5>
            <h3>{{ numberOfCCNsThisMonth }}</h3>
          </div>
        </div>
      </div>
      <div class="col-md-6 mt-4">
        <div class="card">
          <div class="card-body">
            <h5 class="card-title">Last Detected Process</h5>
            <h3>
              {{
                Intl.DateTimeFormat("en-US", { dateStyle: "long" }).format(
                  new Date(
                    new Date().getFullYear(),
                    new Date().getMonth(),
                    new Date().getDate() + 60
                  )
                )
              }}
            </h3>
          </div>
        </div>
      </div>
    </div>
    <div class="row">
      <div class="col-md-12 mt-4">
        <div class="card">
          <div class="card-body">
            <h5 class="card-title">Engineering Metrics</h5>
            <table class="table table-striped">
              <thead>
                <tr>
                  <th>Total Product</th>
                  <th>Total BOMs</th>
                  <th>Total SLDs</th>
                  <th>Total Drawings</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td>{{ totalProducts }}</td>
                  <td>{{ totalBOMs }}</td>
                  <td>{{ totalSLDs }}</td>
                  <td>{{ totalDrawings }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
    <div class="row">
      <div class="col-md-6">
        <strong>ECN</strong>
        <!-- <div>test</div> -->
        <LineChart :data="ecnData" />
        <!-- <line-chart :chart-data="ecnData" :chart-options="chartOptions"></line-chart> -->
      </div>
      <div class="col-md-6">
        <strong>CCN</strong>
        <LineChart :data="ccnData" />

        <!-- <line-chart :chart-data="ccnData" :chart-options="chartOptions"></line-chart> -->
      </div>
    </div>
    <div class="row mt-4">
      <div class="col-md-12">
        <h3>Engineering Activities Today</h3>
        <table class="table table-striped">
          <thead>
            <tr>
              <th>PIC</th>
              <th>Customer</th>
              <th>Description</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="activity in engineeringActivitiesToday"
              :key="activity.id"
            >
              <td>{{ activity.pic }}</td>
              <td>{{ activity?.customer }}</td>

              <td>{{ activity.description }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from "vue";
import LineChart from "../components/LineChart.vue";

const numberOfManpowers = ref(0);
const numberOfECNsThisMonth = ref(0);
const numberOfCCNsThisMonth = ref(0);
const lastDetectedProcess = ref(new Date());
const totalProducts = ref(0);
const totalBOMs = ref(0);
const totalSLDs = ref(0);
const totalDrawings = ref(0);
const ecnData = ref({
  labels: ["a"],
  datasets: [
    {
      label: "Monthly ECNs",
      backgroundColor: "#42A5F5",
      data: [1],
    },
  ],
});
const ccnData = ref({
  labels: [],
  datasets: [
    {
      label: "Monthly CCNs",
      backgroundColor: "#66BB6A",
      data: [],
    },
  ],
});
const engineeringActivitiesToday = ref([]);

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
};
// const engineeringActivitiesToday = [
//     { id: 1, pic: 'Agus', category: 'Design Drawing', customer: 'Moratel', description: 'Completed initial design for project X' },
//     { id: 2, pic: 'Hendi', category: 'General', customer: 'Telkomsigma', description: 'Reviewed project status and deliverables' },
//     { id: 3, pic: 'Rita', category: 'Customer Visit', customer: 'Hitachi', description: 'On-site inspection and feedback collection' },
//     { id: 4, pic: 'Yogi', category: 'Design Drawing', customer: 'Schneider', description: 'Finalized technical drawings for approval' },
//     { id: 5, pic: 'Agus', category: 'General', customer: 'Moratel', description: 'Prepared project documentation and reports' },
//     { id: 6, pic: 'Hendi', category: 'Customer Visit', customer: 'Telkomsigma', description: 'Conducted client meeting and demo' },
//     { id: 7, pic: 'Rita', category: 'Design Drawing', customer: 'Hitachi', description: 'Drafted new schematic diagrams' },
//     { id: 8, pic: 'Yogi', category: 'General', customer: 'Schneider', description: 'Updated project timelines and milestones' },
//     { id: 9, pic: 'Agus', category: 'Customer Visit', customer: 'Moratel', description: 'Follow-up visit for project feedback' },
//     { id: 10, pic: 'Hendi', category: 'Design Drawing', customer: 'Telkomsigma', description: 'Reviewed and revised initial designs' },
// ]

const fetchDashboardMetrics = async () => {
  try {
    const response = await fetch(`${import.meta.env.VITE_APP_BASE_URL}/api/dashboard/metrics`);
    const data = await response.json();
    numberOfManpowers.value = data.numberOfManpowers;
    numberOfECNsThisMonth.value = data.numberOfECNsThisMonth;
    numberOfCCNsThisMonth.value = data.numberOfCCNsThisMonth;
    lastDetectedProcess.value = new Date(data.lastDetectedProcess);
    totalProducts.value = data.totalProducts;
    totalBOMs.value = data.totalBOMs;
    totalSLDs.value = data.totalSLDs;
    totalDrawings.value = data.totalDrawings;
  } catch (error) {
    console.error("Error fetching dashboard metrics:", error);
  }
};

const fetchEngineeringActivities = async () => {
  try {
    const response = await fetch(
      `${import.meta.env.VITE_APP_BASE_URL}/api/dashboard/activities`
    );
    engineeringActivitiesToday.value = await response.json();
  } catch (error) {
    console.error("Error fetching engineering activities:", error);
  }
};

const fetchECNData = async () => {
  try {
    const response = await fetch(
      `${import.meta.env.VITE_APP_BASE_URL}/api/dashboard/ecn-data`
    );
    const data = await response.json();

    ecnData.value = {
      ...ecnData.value,
      labels: data.map((item) => item.month),
      datasets: ecnData.value.datasets.map((d) => ({
        ...d,
        data: data.map((item) => item.count),
      })),
    };
  } catch (error) {
    console.error("Error fetching ECN data:", error);
  }
};

const fetchCCNData = async () => {
  try {
    const response = await fetch(
      `${import.meta.env.VITE_APP_BASE_URL}/api/dashboard/ccn-data`
    );
    const data = await response.json();
    ccnData.value = {
      ...ccnData.value,
      labels: data.map((item) => item.month),
      datasets: ccnData.value.datasets.map((d) => ({
        ...d,
        data: data.map((item) => item.count),
      })),
    };
  } catch (error) {
    console.error("Error fetching CCN data:", error);
  }
};

fetchDashboardMetrics();
fetchEngineeringActivities();
fetchECNData();
fetchCCNData();
</script>

<style scoped>
.container {
  margin-top: 20px;
}
</style>
