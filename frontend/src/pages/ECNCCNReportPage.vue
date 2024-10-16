<template>
  <v-container>
    <v-row>
      <v-col>
        <v-card>
          <v-card-title class="headline">ECN/CCN</v-card-title>
          <div>
            Configured Threshold:
            {{
              Intl.NumberFormat("en-US", {
                maximumFractionDigits: 2,
                minimumFractionDigits: 2,
              }).format(ccnThresholdData?.value ?? 0)
            }}
          </div>
          <v-card-text>
            <v-data-table :headers="headers" :items="items" class="elevation-1">
              <template v-slot:item.engineering="{ item }">
                <div>
                  <div
                    v-for="(text, index) in parseEngineering(item.engineering)"
                    :key="index"
                  >
                    <span
                      :style="{
                        'background-color': text.highlight
                          ? 'yellow'
                          : 'transparent',
                      }"
                      >{{ text.value }}</span
                    >
                  </div>
                </div>
              </template>
              <template v-slot:item.cost="{ item }">
                <div
                  :style="`background-color: ${
                    (item?.cost ?? 0) > (ccnThresholdData?.value ?? 5000000)
                      ? `yellow`
                      : ``
                  }`"
                >
                  {{
                    isNaN(parseFloat(item?.cost))
                      ? item?.cost
                      : Intl.NumberFormat("en-US", {
                          maximumFractionDigits: 2,
                          minimumFractionDigits: 2,
                        }).format(parseFloat(item?.cost))
                  }}
                </div>
              </template>
            </v-data-table>
          </v-card-text>
          <v-card-subtitle>Need Advice:</v-card-subtitle>
          <v-card-text>
            <ol>
              <li>Apakah bisa dibuat ada spare</li>
              <li>
                Apakah bisa PR tidak dibuat partial? karena bom list dibuat
                dalam jumlah 1 lot
              </li>
              <li>Jika Belum ada approval customer mohon tidak di PR</li>
            </ol>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup>
import { ref, onMounted, computed } from "vue";
import axios from "axios";

const headers = ref([
  { title: "No", value: "no" },
  { title: "Tgl", value: "tgl" },
  { title: "PO Number", value: "poNumber" },
  { title: "Cust", value: "cust" },
  { title: "Project Name", value: "projectName" },
  { title: "Engineering", value: "engineering" },
  { title: "Detail Problem", value: "detailProblem" },
  { title: "P/N", value: "pn" },
  { title: "Description", value: "description" },
  { title: "Qty", value: "qty" },
  { title: "Uom", value: "uom" },
  { title: "Cost (IDR)", value: "cost" },
]);

const items = ref([]);
const configurations = ref([]);

const ccnThresholdData = computed(() => {
  return configurations.value.find((c) => c?.name === "CCNThreshold");
});

const fetchData = async () => {
  try {
    const response = await axios.get(
      `${import.meta.env.VITE_APP_BASE_URL}/engineeringDetailProblems`
    );
    items.value = response.data.map((item, index) => ({
      ...item,
      no: index + 1, // Add a sequential number
      tgl: new Date(item.tgl).toLocaleDateString("en-GB", {
        day: "2-digit",
        month: "short",
        year: "2-digit",
      }),
      engineering: item.engineering || "", // Ensure engineering is not null
    }));
  } catch (error) {
    console.error("Error fetching data:", error);
  }
};

const fetchConfigurationsData = async () => {
  try {
    const response = await axios.get(`${import.meta.env.VITE_APP_BASE_URL}/configurations`);
    configurations.value = response.data;
  } catch (error) {
    console.error("Error fetching configs:", error);
  }
};

const parseEngineering = (engineering) => {
  // This is a simple parser. You might need to adjust it based on your actual data format
  return engineering.split(". ").map((text) => ({
    value: text,
    highlight:
      text.toLowerCase().includes("tidak") ||
      text.toLowerCase().includes("tdk"),
  }));
};

onMounted(() => {
  fetchData();
  fetchConfigurationsData();
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
