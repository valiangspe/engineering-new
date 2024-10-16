<template>
  <v-container>
    <v-row v-for="table in tables" :key="table.name">
      <v-col cols="12">
        <v-card-title>{{ table.title }}</v-card-title>
        <v-card-text>
          <v-data-table
            :headers="table.headers"
            :items="table.items"
            item-value="id"
            class="elevation-1"
          >
            <template v-slot:top>
              <v-toolbar flat>
                <v-toolbar-title>{{ table.title }} Table</v-toolbar-title>
                <v-spacer></v-spacer>
                <v-btn color="primary" @click="addItem(table.name)">
                  Add New
                </v-btn>
              </v-toolbar>
            </template>
            <template v-slot:item="{ item, column }">
              <tr>
                <td v-for="col in table.headers" :key="col.value">
                  <textarea
                    class="form-control form-control-sm"
                    v-if="col.multiline && col.value !== 'actions'"
                    v-model="item[col.value]"
                    :type="getInputType(col.type)"
                    dense
                    hide-details
                    single-line
                    @input="updateItem(table.name, item, col)"
                  />
                  <input
                    class="form-control form-control-sm"
                    v-else-if="col.value !== 'actions'"
                    v-model="item[col.value]"
                    :type="getInputType(col.type)"
                    dense
                    hide-details
                    single-line
                    @input="updateItem(table.name, item, col)"
                  />
                  <v-icon v-else small @click="saveItem(table.name, item)">
                    mdi-content-save
                  </v-icon>
                </td>
              </tr>
            </template>
          </v-data-table>
        </v-card-text>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup>
import { ref, onMounted } from "vue";
import axios from "axios";

const tables = ref([
  {
    name: "engineerSupports",
    title: "Engineer Supports",
    headers: [
      { title: "Engineer", value: "engineer", type: "string" },
      { title: "Project", value: "project", type: "string" },
      { title: "Requester", value: "requester", type: "string" },
      { title: "Support Dates", value: "supportDates", type: "string" },
      { title: "Actions", value: "actions" },
    ],
    items: [],
  },
  {
    name: "supportDetails",
    title: "Support Details",
    headers: [
      { title: "No", value: "no", type: "string" },
      { title: "Date", value: "tgl", type: "date" },
      { title: "PO Number", value: "poNumber", type: "string" },
      { title: "Customer", value: "cust", type: "string" },
      { title: "Project Name", value: "projectName", type: "string" },
      { title: "Engineering", value: "engineering", type: "string" },
      { title: "Detail Problem", value: "detailProblem", type: "string" },
      { title: "PN", value: "pn", type: "string" },
      { title: "Description", value: "description", type: "string" },
      { title: "Qty", value: "qty", type: "number" },
      { title: "UOM", value: "uom", type: "string" },
      { title: "Actions", value: "actions" },
    ],
    items: [],
  },
  {
    name: "outstandingPostPOs",
    title: "Outstanding Post POs",
    headers: [
      { title: "Engineer", value: "customer", type: "string" },
      { title: "Done/Outs", value: "doneOuts", type: "string" },
      { title: "Project", value: "project", type: "string", multiline: true },
      { title: "Days Deadline", value: "daysDeadline", type: "number" },
      { title: "Others Outs", value: "othersOuts", type: "number" },
      { title: "Others Process", value: "othersProcess", type: "number" },
      { title: "Post PO Outs", value: "postPoOuts", type: "number" },
      { title: "Post PO Process", value: "postPoProcess", type: "number" },
      { title: "Pre PO Outs", value: "prePoOuts", type: "number" },
      { title: "Pre PO Process", value: "prePoProcess", type: "number" },
      { title: "Total Outs", value: "totalOuts", type: "number" },
      { title: "Total Process", value: "totalProcess", type: "number" },
      { title: "Actions", value: "actions" },
    ],
    items: [],
  },
  {
    name: "donePOs",
    title: "Done POs",
    headers: [
      { title: "Job Category", value: "jobCategory", type: "string" },
      { title: "Detail", value: "detail", type: "string" },
      { title: "Outs", value: "outs", type: "number" },
      { title: "T. Process", value: "tProcess", type: "number" },
      { title: "Actions", value: "actions" },
    ],
    items: [],
  },
  {
    name: "engineeringDetailProblems",
    title: "Engineering Detail Problems",
    headers: [
      { title: "No", value: "no", type: "string" },
      { title: "Date", value: "tgl", type: "date" },
      { title: "PO Number", value: "poNumber", type: "string" },
      { title: "Customer", value: "cust", type: "string" },
      {
        title: "Project Name",
        value: "projectName",
        type: "string",
        multiline: true,
      },
      {
        title: "Engineering",
        value: "engineering",
        type: "string",
        multiline: true,
      },
      {
        title: "Detail Problem",
        value: "detailProblem",
        type: "string",
        multiline: true,
      },
      { title: "PN", value: "pn", type: "string" },
      {
        title: "Description",
        value: "description",
        type: "string",
        multiline: true,
      },
      { title: "Qty", value: "qty", type: "number" },
      { title: "UOM", value: "uom", type: "string" },
      { title: "Cost (IDR)", value: "cost" },
      { title: "Actions", value: "actions" },
    ],
    items: [],
  },
  {
    name: "configurations",
    title: "Configurations",
    headers: [
      { title: "Name", value: "name", type: "string" },
      {
        title: "By Value/Percentage",
        value: "byValuePercentage",
        type: "string",
      },
      { title: "Value", value: "value", type: "number" },
      { title: "Percentage", value: "percentage", type: "number" },
      { title: "Actions", value: "actions" },
    ],
    items: [],
  },
  {
    name: "api/dashboard/metrics/arr",
    title: "Dashboard Metrics",
    headers: [
      { title: "Manpowers", value: "numberOfManpowers", type: "number" },
      { title: "ECN", value: "numberOfECNsThisMonth", type: "number" },
      { title: "CCN", value: "numberOfCCNsThisMonth", type: "string" },
      {
        title: "LastDetectedProcess",
        value: "lastDetectedProcess",
        type: "date",
      },
      { title: "Products", value: "totalProducts", type: "number" },
      { title: "BOMs", value: "totalBOMs", type: "number" },
      { title: "SLDs", value: "totalSLDs", type: "number" },
      { title: "Drawings", value: "totalDrawings", type: "number" },
      { title: "Actions", value: "actions" },

    ],
    items: [],
  },
]);

const fetchItems = async (tableName) => {
  try {
    const response = await axios.get(`${import.meta.env.VITE_APP_BASE_URL}/${tableName}`);
    const table = tables.value.find((t) => t.name === tableName);
    if (table) {
      table.items = response.data;
    }
  } catch (error) {
    console.error(`Error fetching ${tableName}:`, error);
  }
};

const saveItem = async (tableName, item) => {
  try {
    await axios.post(`${import.meta.env.VITE_APP_BASE_URL}/${tableName}`, item);
    await fetchItems(tableName);
  } catch (error) {
    console.error(`Error saving ${tableName} item:`, error);
  }
};

const updateItem = (tableName, item, column) => {
  if (column.type === "number") {
    item[column.value] = parseFloat(item[column.value]) || 0;
  } else if (column.type === "date") {
    item[column.value] = new Date(item[column.value])
      .toISOString()
      .split("T")[0];
  }
};

const getInputType = (type) => {
  switch (type) {
    case "number":
      return "number";
    case "date":
      return "date";
    default:
      return "text";
  }
};

const addItem = (tableName) => {
  const table = tables.value.find((t) => t.name === tableName);
  if (table) {
    const newItem = {};
    table.headers.forEach((header) => {
      if (header.value !== "actions") {
        newItem[header.value] = header.type === "number" ? 0 : "";
      }
    });
    table.items.unshift(newItem);
  }
};

onMounted(() => {
  tables.value.forEach((table) => fetchItems(table.name));
});
</script>

<style scoped>
.v-data-table {
  margin-bottom: 20px;
}

.v-card {
  margin-bottom: 20px;
}
</style>
