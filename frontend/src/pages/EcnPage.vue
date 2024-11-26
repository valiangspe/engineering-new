<script setup>
import { ref } from "vue";
import ExcelJS from "exceljs";
import {
  fetchEngineeringDetailProblems,
  fetchExtCrmPurchaseOrdersProtoSimple,
  fetchItems,
  fetchJobsProtoSimple,
} from "./fetchers";

const ecns = ref([]);
const pos = ref([]);
const jobs = ref([]);
const items = ref([]);

const fetchEngineeringDetailProblemsData = async () => {
  const d = await fetchEngineeringDetailProblems();

  if (d) {
    ecns.value = d;
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


const exportToExcel = async () => {
  const workbook = new ExcelJS.Workbook();
  const worksheet = workbook.addWorksheet("Report");

  // Tambahkan header
  worksheet.columns = [
    { header: "No", key: "no", width: 5 },
    { header: "ID", key: "id", width: 10 },
    { header: "Date", key: "date", width: 15 },
    { header: "PO Number", key: "po_number", width: 20 },
    { header: "Customer Name", key: "customer_name", width: 20 },
    { header: "Project Name", key: "project_name", width: 20 },
    { header: "Description", key: "description", width: 30 },
    { header: "Qty", key: "qty", width: 10 },
    { header: "Reduction (Rp)", key: "reduction", width: 15 },
    { header: "Increase (Rp)", key: "increase", width: 15 },
    { header: "Remark", key: "remark", width: 20 },
  ];

  // Tambahkan data
  ecns.value.forEach((e, i) => {
    const foundPO = pos.value.find(
      (p) => `${p?.id}` === `${e?.extPurchaseOrderId}`
    );
    const foundJob = jobs.value?.jobs?.find(
      (j) => `${j?.masterJavaBaseModel?.id}` === `${e?.extJobId}`
    );

    worksheet.addRow({
      no: i + 1,
      id: e?.id || "",
      date: e?.tgl
        ? Intl.DateTimeFormat("en-US", { dateStyle: "medium" }).format(
            new Date(e?.tgl ?? "")
          )
        : "",
      po_number: foundPO?.purchaseOrderNumber || "",
      customer_name: foundPO?.account?.name || "",
      project_name: foundJob?.name || "",
      description: e?.detailProblem || "",
      qty: e?.items?.length || 0,
      reduction: Intl.NumberFormat("en-US", {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
      }).format(e?.reduction || 0),
      increase: Intl.NumberFormat("en-US", {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
      }).format(e?.increase || 0),
      remark: e?.remark || "",
    });
  });

  // Simpan file
  const buffer = await workbook.xlsx.writeBuffer();
  const blob = new Blob([buffer], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });

  // Unduh file
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

fetchEngineeringDetailProblemsData();
fetchPosData();
fetchJobsData();
fetchWarehouseItemsData();
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
                'Created',
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
          <tr v-for="(e, i) in ecns">
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
                    (j) => `${j?.masterJavaBaseModel?.id}` === `${e?.extJobId}`
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
            </template>
          </tr>
        </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
