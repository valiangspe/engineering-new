<script setup>
import { ref, computed, watch, onMounted } from "vue";

import ExcelJS from "exceljs";
import {
  fetchEngineeringDetailProblems,
  fetchExtCrmPurchaseOrdersProtoSimple,
  fetchItems,
  fetchJobsProtoSimple,
} from "./fetchers";
import axios from "axios";
import { debounce } from "lodash";

onMounted(() => {
  fetchUsers(); // Pastikan dipanggil di lifecycle awal
  // await fetchAllData(); // Load semua data termasuk pengguna
});
// Deklarasi variabel utama
const poSearch = ref("");
const poSearchDebounced = ref("");
const ecns = ref([]);
const pos = ref([]);
const jobs = ref([]);
const items = ref([]);
const ecnCcnFilter = ref(null); // null, 'ECN', atau 'CCN'
const loading = ref(true);

// Debounce untuk pencarian PO
watch(
  poSearch,
  debounce((value) => {
    poSearchDebounced.value = value.trim();
  }, 300)
);

// Pemetaan PO untuk efisiensi pencarian
const posMap = computed(() => {
  const map = new Map();
  pos.value.forEach((p) => {
    map.set(`${p?.id}`, p);
  });
  return map;
});
const users = ref([]); // Harus diinisialisasi sebagai array

const getUsernameById = (id) => {
  if (!Array.isArray(users.value) || users.value.length === 0) {
    return "Unknown";
  }
  const user = users.value.find((u) => `${u?.id}` === `${id}`);
  return user ? user.name : "Unknown";
};


const fetchUsers = async () => {
  try {
    const response = await axios.get(`${import.meta.env.VITE_API_BASE_URL}/users`);
    users.value = Array.isArray(response.data) ? response.data : []; // Validasi jika bukan array
  } catch (error) {
    console.error("Failed to fetch users:", error);
    users.value = []; // Set ke array kosong jika ada kesalahan
    console.log("Fetched users:", users.value);
  }
};


// Filter data ECN/CCN
const filteredEcns = computed(() => {
  return ecns.value.filter((e) => {
    const matchesEcnCcn =
      ecnCcnFilter.value === null ||
      (ecnCcnFilter.value === "ECN" && e.typeEcnCcn === 0) ||
      (ecnCcnFilter.value === "CCN" && e.typeEcnCcn === 1);

    const foundPO = posMap.value.get(`${e?.extPurchaseOrderId}`);
    const matchesPo =
      !poSearchDebounced.value ||
      (foundPO?.purchaseOrderNumber || "")
        .toLowerCase()
        .includes(poSearchDebounced.value.toLowerCase());

    return matchesEcnCcn && matchesPo;
  });
});

// Fungsi untuk ekspor ke Excel
const exportToExcel = async () => {
  try {
    const workbook = new ExcelJS.Workbook();
    const worksheet = workbook.addWorksheet("ECN Report");

    // Header kolom
    worksheet.columns = [
      { header: "No", key: "no", width: 5 },
      { header: "ID", key: "id", width: 10 },
      { header: "Date", key: "date", width: 15 },
      { header: "Requested By", key: "requestedBy", width: 20 }, // Tambahkan kolom
      { header: "PO Number", key: "po_number", width: 20 },
      { header: "Project Name", key: "project_name", width: 25 },
      { header: "ECN/CCN", key: "ecn_ccn", width: 10 },
      { header: "Description", key: "description", width: 30 },
      { header: "Cost", key: "cost", width: 15 },
      { header: "Increase", key: "increase", width: 15 },
      { header: "Decrease", key: "decrease", width: 15 },
      { header: "Created", key: "created", width: 20 },
      { header: "Updated", key: "updated", width: 20 },
    ];

    // Tambahkan data ECN
    filteredEcns.value.forEach((ecn, index) => {
      const foundPO = posMap.value.get(`${ecn?.extPurchaseOrderId}`);
      const projectName = foundPO
        ? `${foundPO.purchaseOrderNumber} (${foundPO?.account?.name || ""})`
        : "Unknown Project";

      const cost = ecn?.items.reduce(
        (total, item) => total + (item?.snapshotPrice || 0) * (item?.qty || 0),
        0
      );
      const increase = ecn?.items
        .filter((item) => item?.typeIncreaseDecrease === 0)
        .reduce(
          (total, item) => total + (item?.snapshotPrice || 0) * (item?.qty || 0),
          0
        );
      const decrease = ecn?.items
        .filter((item) => item?.typeIncreaseDecrease === 1)
        .reduce(
          (total, item) => total + (item?.snapshotPrice || 0) * (item?.qty || 0),
          0
        );

      worksheet.addRow({
        no: index + 1,
        id: ecn?.id,
        date: ecn?.tgl
          ? new Date(ecn?.tgl).toLocaleDateString("en-US")
          : "N/A",
        requestedBy: getUsernameById(ecn.extUserId), // Tambahkan username
        po_number: foundPO?.purchaseOrderNumber || "Unknown PO",
        project_name: projectName,
        ecn_ccn: ecn?.typeEcnCcn === 0 ? "ECN" : "CCN",
        description: ecn?.detailProblem || "No Description",
        cost: cost || 0,
        increase: increase || 0,
        decrease: decrease || 0,
        created: ecn?.createdAt
          ? new Date(ecn?.createdAt).toLocaleString("en-US")
          : "N/A",
        updated: ecn?.updatedAt
          ? new Date(ecn?.updatedAt).toLocaleString("en-US")
          : "N/A",
      });
    });

    // Buat file Excel dan unduh
    const buffer = await workbook.xlsx.writeBuffer();
    const blob = new Blob([buffer], {
      type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = "ECN_Report.xlsx";
    a.click();
    URL.revokeObjectURL(url);
  } catch (error) {
    console.error("Failed to export Excel:", error);
  }
};

// Fetch data utama
const fetchAllData = async () => {
  loading.value = true;
  try {
    await Promise.all([
      fetchEngineeringDetailProblemsData(),
      fetchPosData(),
      fetchJobsData(),
      fetchWarehouseItemsData(),
    ]);
  } catch (error) {
    console.error("Failed to fetch all data:", error);
  } finally {
    loading.value = false;
  }
};

// Fetch individual data functions
const fetchEngineeringDetailProblemsData = async () => {
  try {
    const response = await axios.get(
      "http://localhost:5172/engineeringDetailProblems/1"
    );
    const data = response.data;
    if (data) {
      ecns.value = Array.isArray(data) ? data : [data]; // Validasi jika respons berupa objek tunggal
      // Fetch pengguna terkait berdasarkan extUserId
      const userIds = [...new Set(ecns.value.map((ecn) => ecn.extUserId))];
      await fetchUsersByIds(userIds); // Fetch hanya pengguna yang relevan
    }
  } catch (err) {
    console.error("Failed to fetch ECN data:", err);
  }
};
const fetchUsersByIds = async (userIds) => {
  try {
    const response = await axios.get(
      `${import.meta.env.VITE_API_BASE_URL}/users`,
      {
        params: {
          ids: userIds.join(","), // Kirim daftar ID sebagai parameter
        },
      }
    );
    users.value = Array.isArray(response.data) ? response.data : [];
  } catch (error) {
    console.error("Failed to fetch users by IDs:", error);
    users.value = [];
  }
};

const uniqueUsers = computed(() => {
  const userMap = new Map();
  ecns.value.forEach((ecn) => {
    if (ecn.createdBy) {
      const user = ecn.createdBy;
      if (!userMap.has(user.id)) {
        userMap.set(user.id, { ...user, ecnCount: 1 });
      } else {
        userMap.get(user.id).ecnCount += 1;
      }
    }
  });
  return Array.from(userMap.values());
});

const fetchPosData = async () => {
  try {
    const d = await fetchExtCrmPurchaseOrdersProtoSimple();
    if (d) pos.value = d;
  } catch (err) {
    console.error("Failed to fetch PO data:", err);
  }
};

const fetchJobsData = async () => {
  try {
    const d = await fetchJobsProtoSimple({ all: true });
    if (d) jobs.value = d;
  } catch (err) {
    console.error("Failed to fetch jobs data:", err);
  }
};

const fetchWarehouseItemsData = async () => {
  try {
    const d = await fetchItems();
    if (d) items.value = d;
  } catch (err) {
    console.error("Failed to fetch warehouse items data:", err);
  }
};

// Panggil fetch data utama
fetchAllData();
</script>

<template>
  <div class="m-3">
    <div class="d-flex">
      <h4>ECN</h4>
      <div>
        <button @click="exportToExcel" class="btn btn-success">
          Export to Excel
        </button>
      </div>
      <div>
        <a href="/#/ecn/add">
          <button class="btn btn-success bg-blue mx-3">Add</button>
        </a>
      </div>
    </div>
  <div class="mb-3">
    <div class="mb-3">
      <div class="d-flex">
        <!-- Filter ECN/CCN -->
        <div class="me-3">
          <label for="ecnCcnFilter" class="form-label">Filter ECN/CCN</label>
          <select id="ecnCcnFilter" class="form-select" v-model="ecnCcnFilter">
            <option :value="null">All</option>
            <option value="ECN">ECN</option>
            <option value="CCN">CCN</option>
          </select>
        </div>
        <!-- Pencarian PO -->
        <div>
          <label for="poSearch" class="form-label">Search PO</label>
          <input
            id="poSearch"
            type="text"
            class="form-control"
            v-model="poSearch"
            placeholder="Search by PO Number"
          />
        </div>
      </div>
    </div>
  </div>

  <div v-if="loading" class="text-center my-3">
    <div class="spinner-border text-primary" role="status">
      <span class="visually-hidden">Loading...</span>
    </div>
  </div>
  <div v-else>
      <div
        class="overflow-auto border border-dark"
        style="height: 60vh; resize: vertical"
      >
        <table class="table table-sm" style="border-collapse: separate">
          <thead>
            <tr>
              <th
                style="position: sticky; top: 0"
                class="bg-dark text-light p-0 m-0"
                v-for="(h, i) in [
                  '#',
                  'ID',
                  'Date',
                  'PO',
                  'Project Name',
                  'Eng Note',
                  'Problem Detail',
                  'No. of Items',
                  // 'Part Desc',
                  // 'Qty',
                  'Add/Subtract',
                  'ECN/CCN',
                  // 'UM',
                  'Cost',
                  'Increase',
                  'Decrease',
                  'Created',
                  'Request By',
                  'Updated',
                  'Action',
                  'Print',
                  'Approval',
                  'Approval Status',
                ]"
              >
                {{ h }}
              </th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(e, i) in filteredEcns" :key="e.id || `ecn-${i}`">
              <template
                v-for="d in [
                  {
                    foundItem: items.find(
                      (i) => `${i?.id}` === `${e?.extItemId}`
                    ),
                  },
                ]"
              >
                <td class="border border-dark">{{ i + 1 }}</td>
                <td class="border border-dark">{{ e?.id }}</td>

                <td class="border border-dark">
                  {{
                    e?.tgl
                      ? Intl.DateTimeFormat("en-US", {
                          dateStyle: "medium",
                        }).format(new Date(e?.tgl ?? ""))
                      : ""
                  }}
                </td>
                <td class="border border-dark">
                  <template
                    v-for="d in [
                      {
                        foundPO: pos.find(
                          (p) => `${p?.id}` === `${e?.extPurchaseOrderId}`
                        ),
                      },
                    ]"
                    >{{ d?.foundPO?.purchaseOrderNumber }}
                    {{
                      d?.foundPO ? `(${d?.foundPO?.account?.name})` : ""
                    }}</template
                  >
                </td>
                <td class="border border-dark">
                  {{
                    jobs?.jobs?.find(
                      (j) =>
                        `${j?.masterJavaBaseModel?.id}` === `${e?.extJobId}`
                    )?.name
                  }}
                </td>
                <td class="border border-dark">{{ e?.engineering }}</td>
                <td class="border border-dark">{{ e?.detailProblem }}</td>
                <td class="border border-dark">{{ e?.items?.length }}</td>

                <!-- <td class="border border-dark">{{ d?.foundItem?.partDesc }}</td>
              <td class="border border-dark">{{ e?.qty }}</td> -->
                <td class="border border-dark">Penambahan</td>
                <td class="border border-dark">
                  {{ e?.typeEcnCcn === 0 ? `ECN` : `` }}
                  {{ e?.typeEcnCcn === 1 ? `CCN` : `` }}
                </td>

                <!-- <td class="border border-dark">EA</td> -->
                <td class="border border-dark">
                  {{
                    Intl.NumberFormat("en-US", {
                      minimumFractionDigits: 2,
                      maximumFractionDigits: 2,
                    }).format(
                      e?.items
                        ?.filter((i) => !i?.deletedAt)
                        ?.reduce((acc, i) => {
                          const val = (i?.snapshotPrice ?? 0) * (i?.qty ?? 0);

                          if (i?.typeIncreaseDecrease !== 1) {
                            return acc + val;
                          } else {
                            return acc - val;
                          }
                        }, 0) ?? 0
                    )
                  }}
                </td>
                <td class="border border-dark">
                  {{
                    Intl.NumberFormat("en-US", {
                      minimumFractionDigits: 2,
                      maximumFractionDigits: 2,
                    }).format(
                      e?.items
                        ?.filter(
                          (i) => !i?.deletedAt && i?.typeIncreaseDecrease !== 1
                        )
                        ?.reduce(
                          (acc, i) =>
                            acc + (i?.snapshotPrice ?? 0) * (i?.qty ?? 0),
                          0
                        ) ?? 0
                    )
                  }}
                </td>
                <td class="border border-dark">
                  {{
                    Intl.NumberFormat("en-US", {
                      minimumFractionDigits: 2,
                      maximumFractionDigits: 2,
                    }).format(
                      e?.items
                        ?.filter(
                          (i) => !i?.deletedAt && i?.typeIncreaseDecrease === 1
                        )
                        ?.reduce(
                          (acc, i) =>
                            acc + (i?.snapshotPrice ?? 0) * (i?.qty ?? 0),
                          0
                        ) ?? 0
                    )
                  }}
                </td>

                <td class="border border-dark">
                  {{
                    e?.createdAt
                      ? Intl.DateTimeFormat("en-US", {
                          dateStyle: "short",
                          timeStyle: "short",
                        }).format(new Date(e?.createdAt))
                      : ""
                  }}
                </td>

                <td>{{ users.value.length ? getUsernameById(e.extUserId) : "Loading..." }}</td>


                <td class="border border-dark">
                  {{
                    e?.updatedAt
                      ? Intl.DateTimeFormat("en-US", {
                          dateStyle: "short",
                          timeStyle: "short",
                        }).format(new Date(e?.updatedAt))
                      : ""
                  }}
                </td>
                <td class="border border-dark">
                  <a :href="`/#/ecn/${e?.id}`"
                    ><button class="btn btn-sm btn-primary px-1 py-0">
                      Edit
                    </button></a
                  >
                </td>
                <td class="border border-dark">
                  <a :href="`/#/ecn/${e?.id}/print`"
                    ><button class="btn btn-sm btn-primary px-1 py-0">
                      Print
                    </button></a
                  >
                </td>
                <td class="border border-dark">
                  <a class="d-flex" :href="`/#/ecn/${e?.id}/approval`"
                    ><button class="btn btn-sm btn-primary px-1 py-0">
                      Approval
                    </button>
                  </a>
                </td>
                <td
                  :class="`border border-dark text-light ${(() => {
                    switch (e?.status) {
                      case 0:
                        return 'bg-dark';
                      case 1:
                        return 'bg-success';
                      case 2:
                        return 'bg-danger';
                      default:
                        return `bg-dark`;
                    }
                  })()}`"
                >
                  {{
                    e?.approvalDate
                      ? Intl.DateTimeFormat("en-US", {
                          dateStyle: "medium",
                        }).format(new Date(e?.approvalDate ?? ""))
                      : ""
                  }}
                </td>
                <!-- <td class="border border-dark">{{ d?.foundItem?.partNum }}</td> -->
              </template>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
  
</template>
