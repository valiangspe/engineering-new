<template>
  <v-container>
    <div>
      <v-row>
        <v-col class="d-flex justify-end mb-2">
          <v-btn color="primary" @click="exportToExcel">
            <v-icon left>mdi-download</v-icon>
            Export to Excel
          </v-btn>
        </v-col>
      </v-row>

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
        <div class="me-2" style="width: 150px">
          <div>Type</div>
          <v-autocomplete
            class="w-100"
            :items="activityTypes.map((t) => ({ label: t, value: t }))"
            :item-title="(item) => item.label"
            :model-value="
              activityTypes
                .map((t) => ({ label: t, value: t }))
                .find((t) => t.value === selectedActivityType)
            "
            @update:model-value="
              (selected) => {
                selectedActivityType = selected;
                // fetchEngineeringActivitiesData();
              }
            "
          ></v-autocomplete>
        </div>

        <div class="me-2" style="width: 150px">
          <div>Status</div>
          <v-autocomplete
            class="w-100"
            :items="[
              { label: 'All', value: null },
              { label: 'Outs', value: false },
              { label: 'Done', value: true },
            ]"
            :item-title="(item) => item.label"
            :model-value="
              [
                { label: 'All', value: null },
                { label: 'Outs', value: false },
                { label: 'Done', value: true },
              ].find((t) => `${t.value}` === `${isCompleted}`)
            "
            @update:model-value="
              (selected) => {
                // alertx(selected);
                isCompleted = selected;
                // fetchEngineeringActivitiesData();
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
    <v-row>
      <v-col>
        <v-card>
          <v-card-title class="headline">{{ cardTitle }}</v-card-title>
          <v-card-text>
            <v-data-table :headers="headers" :items="items" class="elevation-1">
              <template v-slot:footer> </template>
            </v-data-table>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup>
import ExcelJS from "exceljs";
import { ref, computed } from "vue";
import {
  fetchDepartments,
  fetchEngineeringActivities,
  fetchExtCrmPurchaseOrdersProtoSimple,
  fetchInquiries,
  fetchJobsProtoSimple,
  fetchUsers,
} from "./fetchers";


const exportToExcel = async () => {
  const workbook = new ExcelJS.Workbook();
  const worksheet = workbook.addWorksheet("Report");

  // Tambahkan header
  worksheet.columns = [
    { header: "Customer", key: "customer" },
    { header: "PO", key: "po" },
    { header: "Inquiry", key: "inquiry" },
    { header: "Done/Outs", key: "doneOuts" },
    { header: "Project", key: "project" },
    { header: "Product", key: "product" },
    { header: "PIC", key: "type" },
    { header: "Done PIC", key: "donePic" },
    { header: "Done SPV", key: "doneSpv" },
    { header: "Done Manager", key: "doneManager" },
    { header: "Days (Deadline)", key: "daysDeadline" },
    { header: "Tasks", key: "tasks" },
  ];

  // Tambahkan data
  items.value.forEach((item) => {
    worksheet.addRow(item);
  });

  // Simpan file
  const buffer = await workbook.xlsx.writeBuffer();
  const blob = new Blob([buffer], {
    type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
  });

  const link = document.createElement("a");
  link.href = URL.createObjectURL(blob);
  link.download = `report_activity_${new Date().toISOString()}.xlsx`;
  link.click();
};



const users = ref([]);
const departments = ref([]);
const activities = ref([]);
const pos = ref([]);
const jobs = ref([]);
const inquiries = ref([]);

const selectedActivityType = ref("PostPO");
const isCompleted = ref(false);

const activityTypes = ["PrePO", "PostPO", "Others"];

const fetchPosData = async () => {
  const d = await fetchExtCrmPurchaseOrdersProtoSimple();

  if (d) {
    pos.value = d;
  }
};
const fetchInquiriesData = async () => {
  const d = await fetchInquiries();
  console.log("inquiries", d);
  inquiries.value = d;
};

const fetchUsersData = async () => {
  const d = await fetchUsers();
  console.log("users", d);
  users.value = d;
};
const fetchDepartmentsData = async () => {
  const d = await fetchDepartments();
  departments.value = d;
};

// Create refs for selected month and year
const selectedMonth = ref(new Date().getMonth());
const selectedYear = ref(new Date().getFullYear());

const requiredRule = [(v) => !!v || "Required."];

const menu = ref(false);
const alertx = (d) => {
  window.alert(d);
};
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
  console.log("Engineering Activities:", d); // Debug log
  activities.value = d;
};

const fetchJobsProtoSimpleData = async () => {
  const d = await fetchJobsProtoSimple({ all: true, withProducts: true });

  if (d) {
    jobs.value = d;
  }
};

fetchUsersData();
fetchDepartmentsData();
fetchJobsProtoSimpleData();
fetchEngineeringActivitiesData();
fetchPosData();
fetchInquiriesData();

const headers = ref([
  { title: "Customer", value: "customer" },
  { title: "PO", value: "po" },
  { title: "Inquiry", value: "inquiry" },

  { title: "Done/Outs", value: "doneOuts" },
  { title: "Project", value: "project" },
  { title: "Product", value: "product" },
  { title: "PIC", value: "type" },

  { title: "Done PIC", value: "donePic" },
  { title: "Done SPV", value: "doneSpv" },
  { title: "Done Manager", value: "doneManager" },

  { title: "Days (Deadline)", value: "daysDeadline" },
  { title: "Tasks", value: "tasks" },
]);

const cardTitle = computed(() => {
  if (isCompleted.value === true) {
    return "Done";
  } else if (isCompleted.value === false) {
    return "Outs";
  }
  return "All Activities";
});
// const cardtype = computed (() =>{
// if ()
// });
const items = computed(() => {
  return activities.value
    ?.map((activity) => {
      const allTasks = activity?.tasks || [];
      const totalTasks = allTasks.length;
      const doneTasks =
        allTasks.filter(
          (task) =>
            task?.completedDatePic &&
            task?.completedDateSpv &&
            task?.completedDateManager
        ).length || 0;

      // Menentukan status Done/Outs berdasarkan jumlah tasks selesai
      const doneOuts =
        totalTasks === doneTasks && totalTasks > 0
          ? "Done"
          : `Outs (${doneTasks}/${totalTasks})`;

      // Ambil PO berdasarkan extPurchaseOrderId
      const foundPO = pos.value.find(
        (po) => `${po?.id}` === `${activity?.extPurchaseOrderId}`
      );

      // Ambil Inquiry berdasarkan extInquiryId
      const foundInquiry = inquiries?.value?.find(
        (inquiry) => `${inquiry?.id}` === `${activity?.extInquiryId}`
      );

      // Ambil Job berdasarkan extJobId
      const foundJob = jobs.value.jobs?.find(
        (job) =>
          `${job?.masterJavaBaseModel?.id}` === `${activity?.extJobId}`
      );

      // Ambil Panel Code berdasarkan extPanelCodeId
      const foundPanelCode = jobs.value.jobs
        ?.flatMap((job) => job?.panelCodes)
        .find(
          (panelCode) =>
            `${panelCode?.masterJavaBaseModel?.id}` ===
            `${activity?.extPanelCodeId}`
        );

      // Ambil daftar PIC berdasarkan ID
      const foundUsers = [
        ...new Set(
          activity?.tasks?.flatMap((task) =>
            task?.inCharges.map((charge) => charge.extUserId)
          )
        ),
      ]
        .map((id) => users.value?.find((user) => `${user?.id}` === `${id}`)?.name)
        .join(", ");

      // Format customer: Prioritaskan dari `activity.customer`, lalu PO, lalu Inquiry
      const customer = activity?.customer || foundPO?.account?.name || foundInquiry?.customer?.name || "Belum Memilih";

      // Format tanggal done
      const formatDoneDate = (date) =>
        date
          ? new Date(date).toLocaleDateString("id-ID", {
              day: "2-digit",
              month: "long",
              year: "numeric",
            })
          : "-";

      const donePic = activity?.tasks?.find(
        (task) => task.completedDatePic
      )?.completedDatePic;
      const doneSpv = activity?.tasks?.find(
        (task) => task.completedDateSpv
      )?.completedDateSpv;
      const doneManager = activity?.tasks?.find(
        (task) => task.completedDateManager
      )?.completedDateManager;

      // Format deadline dengan jumlah hari tersisa
      const daysDeadline = (() => {
        if (activity?.toCache) {
          const deadlineDate = new Date(activity.toCache);
          const remainingDays = Math.round(
            (deadlineDate.getTime() - new Date().getTime()) / 86400000
          );
          return `${deadlineDate.toLocaleDateString("id-ID", {
            day: "2-digit",
            month: "long",
            year: "numeric",
          })} (${remainingDays} hari)`;
        }
        return "-";
      })();

      return {
        customer, // Perbaikan customer agar muncul
        po: foundPO?.purchaseOrderNumber || "-",
        inquiry: foundInquiry ? `${foundInquiry?.inquiryNumber}` : "-",
        doneOuts,
        project: foundJob?.name || "-",
        product: foundPanelCode
          ? `${foundPanelCode?.type}: ${foundPanelCode?.name}`
          : "-",
        type: activity?.type || "-",
        daysDeadline,
        donePic: formatDoneDate(donePic),
        doneSpv: formatDoneDate(doneSpv),
        doneManager: formatDoneDate(doneManager),
        tasks: totalTasks || "-",
      };
    })
    ?.filter((item) => {
      // Filter berdasarkan status Done/Outs
      if (isCompleted.value !== null) {
        if (isCompleted.value === true && item.doneOuts !== "Done") {
          return false;
        }
        if (isCompleted.value === false && item.doneOuts === "Done") {
          return false;
        }
      }

      // Filter berdasarkan type
      if (
        selectedActivityType.value &&
        item.type !== selectedActivityType.value
      ) {
        return false;
      }

      return true;
    });
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
