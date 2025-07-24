<script setup>
import { ref, computed, watch } from "vue";
import ExcelJS from "exceljs";
import {
  fetchEngineeringDetailProblems,
  fetchExtCrmPurchaseOrdersProtoSimple,
  fetchItems,
  fetchJobsProtoSimple,
  fetchUsers,
} from "./fetchers";
import axios from "axios";
import { debounce } from "lodash";

const ecns = ref([]);
const users = ref([]);
const pos = ref([]);
const jobs = ref([]);
const items = ref([]);
const poSearch = ref("");
const poSearchDebounced = ref("");
const ecnCcnFilter = ref("All"); // Diubah defaultnya ke 'All'

const filteredEcns = computed(() => {
  return ecns.value
    .filter((e) => {
      if (ecnCcnFilter.value === "All") return true;
      if (ecnCcnFilter.value === "ECN") return e.typeEcnCcn === 0;
      if (ecnCcnFilter.value === "CCN") return e.typeEcnCcn === 1;
      return true;
    })
    .filter((e) => {
      const poNum = e.poNumber || "";
      return poNum.toLowerCase().includes(poSearchDebounced.value.toLowerCase());
    });
});

const fetchEngineeringDetailProblemsData = async () => {
  try {
    const response = await axios.get(
      `${import.meta.env.VITE_APP_BASE_URL}/engineeringDetailProblems`
    );
    if (response.data) {
      ecns.value = Array.isArray(response.data)
        ? response.data.sort((a, b) => new Date(b.updatedAt) - new Date(a.updatedAt))
        : [response.data];
    }
  } catch (error) {
    console.error("Failed to fetch ECN data:", error);
  }
};

const fetchUsersData = async () => {
  try {
    const d = await fetchUsers();
    users.value = Array.isArray(d) ? d : [];
  } catch (error) {
    console.error("Error fetching users:", error);
    users.value = [];
  }
};

const fetchJobsData = async () => {
  const d = await fetchJobsProtoSimple({ all: true });
  if (d) {
    jobs.value = d;
  }
};

const fetchWarehouseItemsData = async () => {
  const d = await fetchItems();
  if (d) {
    items.value = d;
  }
};

watch(
  poSearch,
  debounce((value) => {
    poSearchDebounced.value = value.trim();
  }, 300)
);

const exportToExcel = async () => {
  const workbook = new ExcelJS.Workbook();
  const worksheet = workbook.addWorksheet("Report");

  worksheet.columns = [
    { header: "No", key: "no", width: 5 },
    { header: "ID", key: "id", width: 10 },
    { header: "Date", key: "date", width: 15 },
    { header: "PO Number", key: "po_number", width: 20 },
    { header: "Customer Name", key: "customer_name", width: 20 },
    { header: "Project Name", key: "project_name", width: 25 },
    { header: "ECN/CCN", key: "ecn_ccn", width: 10 },
    { header: "Description", key: "description", width: 30 },
    { header: "Increase (Rp)", key: "increase", width: 15 },
    { header: "Decrease (Rp)", key: "decrease", width: 15 },
  ];

  for (let i = 0; i < filteredEcns.value.length; i++) {
    const e = filteredEcns.value[i];

    const increase = e.items
      ?.filter((item) => !item.deletedAt && item.typeIncreaseDecrease !== 1)
      .reduce((acc, item) => acc + (item.snapshotPrice || 0) * (item.qty || 0), 0) || 0;

    const decrease = e.items
      ?.filter((item) => !item.deletedAt && item.typeIncreaseDecrease === 1)
      .reduce((acc, item) => acc + (item.snapshotPrice || 0) * (item.qty || 0), 0) || 0;

    worksheet.addRow({
      no: i + 1,
      id: e.id || "",
      date: e.tgl ? new Date(e.tgl).toLocaleDateString("id-ID") : "",
      po_number: e.poNumber || "",
      customer_name: e.cust || "",
      project_name: e.projectName || "",
      ecn_ccn: e.typeEcnCcn === 0 ? "ECN" : e.typeEcnCcn === 1 ? "CCN" : "Lainnya",
      description: e.detailProblem || "",
      increase: increase,
      decrease: decrease,
    });
  }

  const buffer = await workbook.xlsx.writeBuffer();
  const blob = new Blob([buffer], {
    type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
  });

  const url = window.URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.download = "Engineering_Report.xlsx";
  a.click();
  window.URL.revokeObjectURL(url);
};

const getUsernameById = (id) => {
  const user = users.value.find((u) => `${u?.id}` === `${id}`);
  return user ? user.name : "Unknown";
};

fetchEngineeringDetailProblemsData();
fetchUsersData();
fetchJobsData();
fetchWarehouseItemsData();
</script>

<template>
  <div class="m-3">
    <v-container>
      <v-row align="center" class="mb-4">
        <v-col cols="6">
          <h2 class="text-h5 font-weight-bold">Engineering Change Notice (ECN)</h2>
        </v-col>
        <v-col cols="6" class="text-right">
          <v-btn color="success" @click="exportToExcel">Export to Excel</v-btn>
          <v-btn color="primary" class="ml-2" to="/ecn/add">Add</v-btn>
        </v-col>
      </v-row>
    </v-container>

    <v-row>
      <v-col cols="6">
        <v-select
          v-model="ecnCcnFilter"
          label="Filter ECN/CCN"
          :items="['All', 'ECN', 'CCN']"
          outlined
          dense
        ></v-select>
      </v-col>
      <v-col cols="6">
        <v-text-field
          v-model="poSearch"
          label="Search PO Number"
          outlined
          clearable
          dense
        ></v-text-field>
      </v-col>
    </v-row>

    <div><hr /></div>
    <div>
      <div
        class="overflow-auto border border-dark"
        style="height: 60vh; resize: vertical"
      >
        <table class="table table-sm table-hover" style="border-collapse: separate">
          <thead>
            <tr>
              <th
                style="position: sticky; top: 0"
                class="bg-dark text-light p-2"
                v-for="h in [
                  '#', 'ID', 'Date', 'PO', 'Project Name', 'Eng Note',
                  'Problem Detail', 'Items', 'ECN/CCN', 'Cost',
                  'Created', 'Updated', 'Action', 'Approval',
                  'Status', 'Has PO', 'Requested By',
                ]"
              >
                {{ h }}
              </th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(e, i) in filteredEcns" :key="e.id || `ecn-${i}`">
              <td class="border-bottom">{{ i + 1 }}</td>
              <td class="border-bottom">{{ e?.id }}</td>
              <td class="border-bottom">
                {{
                  e?.tgl
                    ? new Date(e.tgl).toLocaleDateString('id-ID', { day: '2-digit', month: 'short', year: 'numeric' })
                    : ''
                }}
              </td>
              <td class="border-bottom">
                <div v-if="e.poNumber || e.cust" style="font-size: 0.9em;">
                  <strong>{{ e.poNumber }}</strong><br>
                  <small>{{ e.cust }}</small>
                </div>
              </td>
              <td class="border-bottom">{{ e.projectName }}</td>
              <td class="border-bottom">{{ e.engineering }}</td>
              <td class="border-bottom">{{ e.detailProblem }}</td>
              <td class="border-bottom text-center">{{ e.items?.length ?? 0 }}</td>
              <td class="border-bottom text-center">
                <span :class="e?.typeEcnCcn === 0 ? 'badge bg-primary' : 'badge bg-secondary'">
                  {{ e?.typeEcnCcn === 0 ? 'ECN' : e?.typeEcnCcn === 1 ? 'CCN' : 'Lainnya' }}
                </span>
              </td>
              <td class="border-bottom text-end">
                {{
                  new Intl.NumberFormat('id-ID', { style: 'currency', currency: 'IDR' }).format(
                    e?.items
                      ?.filter((item) => !item?.deletedAt)
                      ?.reduce((acc, item) => {
                        const val = (item?.snapshotPrice ?? 0) * (item?.qty ?? 0);
                        return item?.typeIncreaseDecrease !== 1 ? acc + val : acc - val;
                      }, 0) ?? 0
                  )
                }}
              </td>
              <td class="border-bottom">
                <small>{{ e?.createdAt ? new Date(e.createdAt).toLocaleString('id-ID') : '' }}</small>
              </td>
              <td class="border-bottom">
                <small>{{ e?.updatedAt ? new Date(e.updatedAt).toLocaleString('id-ID') : '' }}</small>
              </td>
              <td class="border-bottom">
                <a :href="`/#/ecn/${e?.id}`">
                  <button class="btn btn-sm btn-outline-primary px-1 py-0">Edit</button>
                </a>
              </td>
              <td class="border-bottom">
                <a class="d-flex" :href="`/#/ecn/${e?.id}/approval`">
                  <button class="btn btn-sm btn-outline-info px-1 py-0">Approval</button>
                </a>
              </td>
              <td class="border-bottom text-center">
                <span
                  :class="`badge ${
                    e?.status === 1 ? 'bg-success' : e?.status === 2 ? 'bg-danger' : 'bg-dark'
                  }`"
                >
                  {{ e?.status === 1 ? 'Approved' : e?.status === 2 ? 'Rejected' : 'Pending' }}
                </span>
              </td>
              <td class="border-bottom text-center">
                <span :class="e?.hasPo ? 'text-success' : 'text-danger'">
                  {{ e?.hasPo ? 'Yes' : 'No' }}
                </span>
              </td>
              <td class="border-bottom">
                {{ getUsernameById(e?.extUserId) }}
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>