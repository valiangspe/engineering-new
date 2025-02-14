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
import { fetchUsers } from "./fetchers"; // Pastikan fungsi fetchUsers sudah tersedia

const ecns = ref([]);
const users = ref([]); // Menyimpan data pengguna
const pos = ref([]);
const jobs = ref([]);
const items = ref([]);

const filteredEcns = computed(() => {
  const byEcnCcn = filteredByEcnCcn.value; // Hasil filter ECN/CCN
  return byEcnCcn.filter((e) => {
    const foundPO = posMap.value.get(`${e?.extPurchaseOrderId}`);
    return (
      !poSearchDebounced.value ||
      (foundPO?.purchaseOrderNumber || "")
        .toLowerCase()
        .includes(poSearchDebounced.value.toLowerCase())
    );
  });
});

const ecnCcnFilter = ref(null); // null, 'ECN', atau 'CCN'

// Filter ECN/CCN
const filteredByEcnCcn = computed(() => {
  return ecns.value.filter((e) => {
    return (
      ecnCcnFilter.value === null ||
      (ecnCcnFilter.value === "ECN" && e.typeEcnCcn === 0) ||
      (ecnCcnFilter.value === "CCN" && e.typeEcnCcn === 1)
    );
  });
});

// const fetchEngineeringDetailProblemsData = async () => {
//   const d = await fetchEngineeringDetailProblems();

//   if (d) {
//     ecns.value = d;
//   }
// };
// Fungsi untuk mengambil data ECN dari API
const fetchEngineeringDetailProblemsData = async () => {
  try {
    const response = await axios.get(
      `${import.meta.env.VITE_APP_BASE_URL}/engineeringDetailProblems`
    );
    if (response.data) {
      ecns.value = Array.isArray(response.data)
        ? response.data
        : [response.data];
    }
  } catch (error) {
    console.error("Failed to fetch ECN data:", error);
  }
};

const fetchUsersData = async () => {
  try {
    const d = await fetchUsers(); // Fungsi fetchUsers Anda
    console.log("Users data fetched:", d); // Debug log
    users.value = Array.isArray(d) ? d : []; // Pastikan data berbentuk array
  } catch (error) {
    console.error("Error fetching users:", error);
    users.value = []; // Fallback ke array kosong jika terjadi error
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

// fungsi search
const poSearch = ref(""); // Input pencarian PO
const poSearchDebounced = ref(""); // Untuk debounce input PO

// Watcher dengan debounce untuk pencarian PO
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

// Filter ECN berdasarkan pencarian PO
const filteredByPo = computed(() => {
  return ecns.value.filter((e) => {
    const foundPO = posMap.value.get(`${e?.extPurchaseOrderId}`);
    return (
      !poSearchDebounced.value ||
      (foundPO?.purchaseOrderNumber || "")
        .toLowerCase()
        .includes(poSearchDebounced.value.toLowerCase())
    );
  });
});

// Fungsi untuk mengekspor ke Excel
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
    { header: "Part Numbers", key: "part_numbers", width: 30 },
    { header: "Part Number Count", key: "part_number_count", width: 20 },
    { header: "Reduction/Cost (Rp)", key: "reduction_cost", width: 15 },
    { header: "Increase (Rp)", key: "increase", width: 15 },
    { header: "Decrease (Rp)", key: "decrease", width: 15 },
    // { header: "Remark", key: "remark", width: 20 },
  ];

  for (let i = 0; i < ecns.value.length; i++) {
    const e = ecns.value[i];

    const foundPO = pos.value.find(
      (p) => `${p?.id}` === `${e?.extPurchaseOrderId}`
    );
    const projectName = foundPO
      ? `${foundPO.purchaseOrderNumber} (${foundPO?.account?.name || ""})`
      : "No Project Name";

    // Ambil detail part number
    let partNumbers = [];
    let partNumbersWithQty = [];
    try {
      const response = await axios.get(
        // `${import.meta.env.VITE_APP_BASE_URL}/engineeringDetailProblems/${e.id}`
        `${import.meta.env.VITE_APP_BASE_URL}/engineeringDetailProblems/${e.id}`
      );
      const detailData = response.data;

      // Proses items untuk mendapatkan part numbers dan qty
      detailData?.items?.forEach((item) => {
        const foundItem = items.value.find((ix) => ix.id === item?.extItemId);
        if (foundItem) {
          partNumbers.push(
            foundItem.partNum || `Unknown Part (${item?.extItemId})`
          );
          partNumbersWithQty.push(
            `${foundItem.partNum || "Unknown Part"} (${item?.qty})`
          );
        }
      });
    } catch (error) {
      console.error("Error fetching part numbers:", error);
    }

    const partNumberCount = partNumbers.length;

    const reductionCost = e?.items
      ?.filter((i) => !i?.deletedAt)
      ?.reduce((acc, i) => {
        const value = (i?.snapshotPrice ?? 0) * (i?.qty ?? 0);

        return i?.typeIncreaseDecrease !== 1 ? acc + value : acc - value;
      }, 0);

    const increase = e?.items
      ?.filter((i) => !i?.deletedAt && i?.typeIncreaseDecrease !== 1)
      ?.reduce((acc, i) => acc + (i?.snapshotPrice ?? 0) * (i?.qty ?? 0), 0);

    const decrease = e?.items
      ?.filter((i) => !i?.deletedAt && i?.typeIncreaseDecrease === 1)
      ?.reduce((acc, i) => acc + (i?.snapshotPrice ?? 0) * (i?.qty ?? 0), 0);

    worksheet.addRow({
      no: i + 1,
      id: e?.id || "",
      date: e?.tgl
        ? Intl.DateTimeFormat("en-US", { dateStyle: "medium" }).format(
            new Date(e?.tgl ?? "")
          )
        : "",
      po_number: foundPO?.purchaseOrderNumber || "",
      customer_name: foundPO?.account?.name || "Unknown Customer",
      project_name: projectName,
      ecn_ccn: e?.typeEcnCcn === 0 ? "ECN" : e?.typeEcnCcn === 1 ? "CCN" : "",
      description: e?.detailProblem || "",
      part_numbers: partNumbers.join(", "),
      part_number_count: partNumberCount,
      reduction_cost: reductionCost || 0,
      increase: increase || 0,
      decrease: decrease || 0,
      remark: e?.remark || "",
    });

    // Tambahkan data Part Number dan Qty di bawah setiap data
    partNumbersWithQty.forEach((partNumberQty) => {
      worksheet.addRow({
        no: "",
        part_numbers: partNumberQty,
        part_number_count: "",
      });
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

const fetchPosData = async () => {
  const d = await fetchExtCrmPurchaseOrdersProtoSimple();

  if (d) {
    pos.value = d;
  }
};

// Fungsi untuk mendapatkan nama pengguna berdasarkan extUserId
const getUsernameById = (id) => {
  const user = users.value.find((u) => `${u?.id}` === `${id}`);
  return user ? user.name : "Unknown";
};

fetchEngineeringDetailProblemsData();
fetchPosData();
fetchJobsData();
fetchWarehouseItemsData();
fetchUsersData();
</script>
<template>
  <div class="m-3">
    <div class="d-flex">
      <h4>ECN</h4>
      <div>
        <button @click="exportToExcel" class="btn btn-sm btn-success mx-2">
          Export to Excel
        </button>
      </div>
      <div>
        <a href="/#/ecn/add">
          <button class="btn btn-sm btn-primary mx-2 px-1 py-0">Add</button>
        </a>
      </div>
    </div>

    <div class="me-3">
      <label for="ecnCcnFilter" class="form-label">Filter ECN/CCN</label>
      <select id="ecnCcnFilter" class="form-select" v-model="ecnCcnFilter">
        <option :value="null">All</option>
        <option value="ECN">ECN</option>
        <option value="CCN">CCN</option>
      </select>
    </div>

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

    <div><hr /></div>
    <div>
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
                  'Updated',
                  'Action',
                  'Print',
                  'Approval',
                  'Approval Status',
                  'Has PO',
                  'Requested By',
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
                <td
                  :class="`border border-dark ${
                    e?.hasPo ? `bg-success text-dark` : `bg-dark text-light`
                  }`"
                >
                  {{ e?.hasPo ? `Yes` : `No` }}
                </td>
                <td class="border border-dark">
                  {{ getUsernameById(e?.extUserId) }}
                </td>
                <!-- Tampilkan nama pengguna -->
                <!-- <td class="border border-dark">{{ d?.foundItem?.partNum }}</td> -->
              </template>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
