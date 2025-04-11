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

      <div class="d-flex align-items-end flex-wrap" style="gap: 12px">
        <div style="width: 180px">
          <v-text-field
            v-model="from"
            label="From"
            type="date"
            density="compact"
            variant="outlined"
            hide-details
            @blur="fetchEngineeringActivitiesData"
          />
        </div>
        <div style="width: 180px">
          <v-text-field
            v-model="to"
            label="To"
            type="date"
            density="compact"
            variant="outlined"
            hide-details
            @blur="fetchEngineeringActivitiesData"
          />
        </div>
        <div style="width: 180px">
          <v-autocomplete
            label="Type"
            v-model="selectedActivityType"
            :items="activityTypes"
            item-title="label"
            item-value="value"
            density="compact"
            variant="outlined"
            hide-details
          />
        </div>
        <div style="width: 180px">
          <v-autocomplete
            label="Status"
            v-model="isCompleted"
            :items="[
              { label: 'All', value: null },
              { label: 'Outs', value: false },
              { label: 'Done', value: true },
            ]"
            item-title="label"
            item-value="value"
            density="compact"
            variant="outlined"
            hide-details
          />
        </div>
        <div style="width: 180px">
          <v-autocomplete
            label="Filter by PIC"
            v-model="selectedPicId"
            :items="users.map(user => ({ label: user.name, value: user.id }))"
            item-title="label"
            item-value="value"
            density="compact"
            variant="outlined"
            hide-details
            clearable
          />
        </div>
        <!-- <v-btn class="ml-2" @click="() => {
          selectedPicId = null;
          selectedActivityType.value = null;
          isCompleted.value = null;
        }">
          Reset Filters
        </v-btn> -->

      </div>

    </div>
    <v-row>
      <v-col>
        <!-- <v-card>
          <v-card-title class="headline">{{ cardTitle }}</v-card-title>
          <v-card-text>
            <v-data-table :headers="headers" :items="items" class="elevation-1">
              <template v-slot:footer> </template>
            </v-data-table>
          </v-card-text>
        </v-card> -->
        <v-card>
          <v-card-title class="headline">{{ cardTitle }}</v-card-title>
          <v-card-text>
            <v-data-table :headers="headers" :items="items" class="elevation-1">
              <template v-slot:item.actions="{ item }">
                <v-btn
                  v-if="item.customer === 'Belum Memilih'"
                  size="small"
                  color="primary"
                  @click="openCustomerDialog(item)"
                >
                  Add Customer
                </v-btn>
              </template>
            </v-data-table>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>


    <!-- Dialog untuk pengisian customer -->
    <v-dialog v-model="customerDialog" max-width="500">
      <v-card>
        <v-card-title>Fix Customer for Task</v-card-title>
        <v-card-text>
          <div v-if="selectedTaskForFix?.type === 'PrePO'">
            <v-btn color="info" block @click="autoFixCustomer('PrePO')">
              Search Customer by Inquiry ID
            </v-btn>
              <p><strong>Task:</strong> {{ selectedTaskForFix?.description }}</p>
              <p><strong>Type:</strong> {{ selectedActivityForFix?.type }}</p>
              <p><strong>Inquiry:</strong> {{ selectedActivityForFix?.extInquiryId }}</p>

              <!-- Jika hasil pencarian ditemukan -->
              <p v-if="foundCustomerPreview"><strong>Preview Customer:</strong> {{ foundCustomerPreview }}</p>
          </div>

          <div v-if="selectedTaskForFix?.type === 'PostPO'">
            <v-btn color="info" block @click="autoFixCustomer('PostPO')">
              Search Customer by PO ID
            </v-btn>
              <p><strong>Task:</strong> {{ selectedTaskForFix?.description }}</p>
              <p><strong>Type:</strong> {{ selectedActivityForFix?.type }}</p>
              <p><strong>PO:</strong> {{ selectedActivityForFix?.extPurchaseOrderId }}</p>
                        <!-- Jika hasil pencarian ditemukan -->
              <p v-if="foundCustomerPreview"><strong>Preview Customer:</strong> {{ foundCustomerPreview }}</p>
          </div>

          <div v-if="selectedTaskForFix?.type === 'Others'">
            <v-autocomplete
              v-model="selectedCustomerForDialog"
              :items="customers.map(c => ({ label: `${c.businessType} ${c.name}`, value: c }))"
              item-title="label"
              item-value="value"
              label="Select Customer"
              outlined
            />
            <div v-if="foundCustomerPreview">
              <v-alert type="info" class="mt-2">
                Found Customer: <strong>{{ foundCustomerPreview }}</strong>
              </v-alert>
            </div>

          </div>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="primary" @click="saveFixedCustomer">Save</v-btn>
          <v-btn color="grey" @click="customerDialog = false">Close</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script setup>
import axios from "axios";
import ExcelJS from "exceljs";
import { ref, computed } from "vue";
import {
  fetchActivityByTaskId,
  fetchDepartments,
  fetchEngineeringActivities,
  fetchExtCrmPurchaseOrdersProtoSimple,
  fetchInquiries,
  fetchJobsProtoSimple,
  fetchUsers,
  // fetchCustomers,
} from "./fetchers";

const exportToExcel = async () => {
  try {
    if (!items.value || items.value.length === 0) {
      alert("Tidak ada data yang bisa di-export.");
      return;
    }

    const workbook = new ExcelJS.Workbook();
    const worksheet = workbook.addWorksheet("Report");

    // Tambahkan header kolom
    worksheet.columns = [
      { header: "TaskName", key: "description" },
      { header: "Customer", key: "customer" },
      { header: "PO", key: "po" },
      { header: "Inquiry", key: "inquiry" },
      { header: "Done/Outs", key: "doneOuts" },
      { header: "Project", key: "project" },
      { header: "Product", key: "product" },
      { header: "Type", key: "type" },
      { header: "Hours", key: "hours" },
      { header: "Done PIC", key: "donePic" },
      { header: "Done SPV", key: "doneSpv" },
      { header: "Done Manager", key: "doneManager" },
      { header: "Days (Deadline)", key: "daysDeadline" },
    ];

    // Tambahkan data dari items
    items.value.forEach((item) => {
      worksheet.addRow({
        description: item.description,
        customer: item.customer,
        po: item.po,
        inquiry: item.inquiry,
        doneOuts: item.doneOuts,
        project: item.project,
        product: item.product,
        type: item.type,
        hours: item.hours,
        donePic: item.donePic,
        doneSpv: item.doneSpv,
        doneManager: item.doneManager,
        daysDeadline: item.daysDeadline,
      });
    });

    // Simpan file
    const buffer = await workbook.xlsx.writeBuffer();
    const blob = new Blob([buffer], {
      type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    });

    const link = document.createElement("a");
    link.href = URL.createObjectURL(blob);
    link.download = `report_activity_${new Date().toISOString().slice(0, 10)}.xlsx`;
    link.click();
  } catch (error) {
    console.error("Gagal export ke Excel:", error);
    alert("Export to Excel gagal. Cek console untuk detail.");
  }
};

const selectedActivityType = ref(null); // default: All
const users = ref([]);
const customers = ref([]);
const departments = ref([]);
const activities = ref([]);
const pos = ref([]);
const jobs = ref([]);
const inquiries = ref([]);
// const task = ref ([]);
// const selectedActivityType = ref("PostPO");
const isCompleted = ref(false);
const customerDialog = ref(false);
const selectedTaskForFix = ref(null);
const selectedCustomerForDialog = ref(null);
const selectedActivityForFix = ref(null); // activity dari task tersebut
const foundCustomerPreview = ref(null); // untuk Preview nama customer hasil pencarian
const selectedPicId = ref(null);


const openCustomerDialog = async (task) => {
  // ambil activity yang terkait dengan task ini
  const activity = activities.value.find(a =>
    a.tasks?.some(t => t.description === task.description)
  );
  if (!activity) return;

  selectedTaskForFix.value = task;
  selectedActivityForFix.value = activity;

  foundCustomerPreview.value = null;
  selectedCustomerForDialog.value = null;

  // if (activity.type === "Others") {
  //   await fetchCustomersData(); // fetch manual
  // }

  if (task.type === "Others" && customers.value.length === 0) {
    await fetchCustomersData(); // Ambil data jika belum ada
  }
  customerDialog.value = true;
};

const autoFixCustomer = (type) => {
  const act = activities.value.find((a) =>
    a.tasks?.some((t) => t.description === selectedTaskForFix.value.description)
  );
  if (!act) return;

  if (type === "PrePO") {
    const inquiry = inquiries.value.find(
      (i) => `${i.id}` === `${act.extInquiryId}`
    );
    foundCustomerPreview.value = inquiry?.account?.name || "Belum Memilih";
  }

  if (type === "PostPO") {
    const po = pos.value.find(
      (p) => `${p.id}` === `${act.extPurchaseOrderId}`
    );
    foundCustomerPreview.value = po?.account?.name || "Belum Memilih";
  }
};


// const saveFixedCustomer = () => {
//   if (selectedTaskForFix.value.type === 'Others' && selectedCustomerForDialog.value) {
//     selectedTaskForFix.value.customer = `${selectedCustomerForDialog.value.businessType} ${selectedCustomerForDialog.value.name}`;
//   } else if (
//     ['PrePO', 'PostPO'].includes(selectedTaskForFix.value.type) &&
//     foundCustomerPreview.value
//   ) {
//     selectedTaskForFix.value.customer = foundCustomerPreview.value;
//   }

//   customerDialog.value = false;
//   foundCustomerPreview.value = null;
//   selectedCustomerForDialog.value = null;
// };

const saveFixedCustomer = async () => {
  let newCustomerValue = null;

  if (selectedTaskForFix.value.type === 'Others' && selectedCustomerForDialog.value) {
    newCustomerValue = `${selectedCustomerForDialog.value.businessType} ${selectedCustomerForDialog.value.name}`;
  }

  if (selectedTaskForFix.value.type === 'PrePO') {
    const act = activities.value.find(a =>
      a.tasks?.some(t => t.description === selectedTaskForFix.value.description)
    );
    const inquiry = inquiries.value.find(i => `${i.id}` === `${act?.extInquiryId}`);
    newCustomerValue = inquiry?.account?.name || 'Belum Memilih';
  }

  if (selectedTaskForFix.value.type === 'PostPO') {
    const act = activities.value.find(a =>
      a.tasks?.some(t => t.description === selectedTaskForFix.value.description)
    );
    const po = pos.value.find(p => `${p.id}` === `${act?.extPurchaseOrderId}`);
    newCustomerValue = po?.account?.name || 'Belum Memilih';
  }

  // Update task di frontend
  selectedTaskForFix.value.customer = newCustomerValue;

  const taskId = selectedTaskForFix.value?.id;
  const activity = activities.value.find(a =>
    a.tasks?.some(t => t.id === taskId)
  );

  try {
    // Gunakan URL yang diambil dari variabel lingkungan
    await fetch(`${import.meta.env.VITE_APP_BASE_URL}/api/dashboard/activities/task/${taskId}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ customer: newCustomerValue }),
    });

    if (activity) {
      activity.customer = newCustomerValue;
    }

    console.log('Customer updated successfully');
  } catch (err) {
    console.error('Gagal menyimpan customer ke backend:', err);
    alert('Gagal menyimpan customer. Coba lagi!');
    return;
  }

  customerDialog.value = false;
};

const fetchInChargeData = async (taskId) => {
  try {
    const res = await fetch(`${import.meta.env.VITE_APP_BASE_URL}/api/dashboard/incharges/${taskId}`);
    const data = await res.json();
    // Proses data InCharge sesuai kebutuhan
    return data;
  } catch (error) {
    console.error("Error fetching InCharge data:", error);
  }
};

const fetchCustomersData = async () => {
  try {
    const res = await axios.get("https://crm-local.iotech.my.id/api/external/customers");
    customers.value = res.data || [];
  } catch (err) {
    console.error("Gagal fetch customers:", err);
    customers.value = [];
  }
};

const activityTypes = [
  { label: "All", value: null },  // ✅ Tambahkan ini
  { label: "PrePO", value: "PrePO" },
  { label: "PostPO", value: "PostPO" },
  { label: "Others", value: "Others" }
];

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

// const fetchActivityByTaskId = async () => {
//   const d = await fetchActivityByTaskId ();
//   console.log("task",d);
//   task.value = d;
// }
const fetchUsersData = async () => {
  const d = await fetchUsers();
  console.log("users", d);
  users.value = d;
};
const fetchDepartmentsData = async () => {
  const d = await fetchDepartments();
  departments.value = d;
};
const fetchTaskById = async (taskId) => {
  try {
    const res = await fetch(`/api/dashboard/activities/task/${taskId}`);
    const json = await res.json();

    if (json.length > 0 && json[0].tasks.length > 0) {
      return json[0].tasks[0].description ?? "-";
    }
  } catch (err) {
    console.error("Gagal mengambil task:", err);
  }
  return "-";
};

// const fetchTaskById = async (taskId) => {
//   try {
//     const res = await fetch(`${import.meta.env.VITE_APP_BASE_URL}/api/dashboard/activities/task/${taskId}`);
//     if (!res.ok) {
//       throw new Error(`HTTP error! status: ${res.status}`);
//     }
//     const data = await res.json();
//     return data;
//   } catch (error) {
//     console.error("Error fetching task:", error);
//   }
// };

// Create refs for selected month and year
// const selectedMonth = ref(new Date().getMonth());
// const selectedYear = ref(new Date().getFullYear());

// FILTER FROM TO
const from = ref(new Date().toISOString().slice(0, 10));
const to = ref(new Date().toISOString().slice(0, 10));
const fetchEngineeringActivitiesData = async () => {
  const d = await fetchEngineeringActivities({
    from: new Date(`${from.value}T00:00:00`).toISOString(),
    to: new Date(`${to.value}T23:59:39`).toISOString(),
  });
  activities.value = d;
};


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
// const dateRange = computed(() => {
//   console.log(selectedYear.value, selectedMonth.value);
//   const firstDay = new Date(selectedYear.value, selectedMonth.value, 1);
//   const lastDay = new Date(selectedYear.value, selectedMonth.value + 1, 0);

//   console.log(firstDay, lastDay);
//   return {
//     from: firstDay.toISOString().split("T")[0],
//     to: lastDay.toISOString().split("T")[0],
//   };
// });


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
fetchTaskById();

const headers = ref([
  { title: "TaskName", value: "description" },  
  { title: "Customer", value: "customer" },
  { title: "PO", value: "po" },
  { title: "Inquiry", value: "inquiry" },

  { title: "Done/Outs", value: "doneOuts" },
  { title: "Project", value: "project" },
  { title: "Product", value: "product" },
  { title: "TYpe", value: "type" },
  { title: "Hours", value: "hours" },  
  { title: "PIC", value: "pic" },  
  { title: "Done PIC", value: "donePic" },
  { title: "Done SPV", value: "doneSpv" },
  { title: "Done Manager", value: "doneManager" },

  { title: "Days (Deadline)", value: "daysDeadline" },
  { title: "Action", value: "actions", sortable: false }, // Tambahan ini
  // { title: "Tasks", value: "tasks" },
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
    ?.flatMap((activity) => {
      const foundPO = pos.value.find(
        (po) => `${po?.id}` === `${activity?.extPurchaseOrderId}`
      );

      const foundInquiry = inquiries?.value?.find(
        (inquiry) => `${inquiry?.id}` === `${activity?.extInquiryId}`
      );

      const foundJob = jobs.value.jobs?.find(
        (job) => `${job?.masterJavaBaseModel?.id}` === `${activity?.extJobId}`
      );

      const foundPanelCode = jobs.value.jobs
        ?.flatMap((job) => job?.panelCodes)
        .find(
          (panelCode) =>
            `${panelCode?.masterJavaBaseModel?.id}` ===
            `${activity?.extPanelCodeId}`
        );

      return activity.tasks.map((task) => {
        const picNames = task?.inCharges
          ?.map((charge) => {
            const user = users.value?.find(
              (user) => `${user?.id}` === `${charge.extUserId}`
            );
            return user ? user.name : null;
          })
          .filter(Boolean);

        const pic = picNames.join(", ") || "No PIC Assigned";

        const customer =
          activity?.customer ||
          foundPO?.account?.name ||
          foundInquiry?.customer?.name ||
          "Belum Memilih";

        const formatDoneDate = (date) =>
          date
            ? new Date(date).toLocaleDateString("id-ID", {
                day: "2-digit",
                month: "long",
                year: "numeric",
              })
            : "-";

        const doneAll =
          task.completedDatePic &&
          task.completedDateSpv &&
          task.completedDateManager;

        const doneOuts = doneAll ? "Done" : "Outs (0/1)";

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
          id: task.id,
          description: task.description ?? "-",
          customer,
          po: foundPO?.purchaseOrderNumber || "-",
          inquiry: foundInquiry ? `${foundInquiry?.inquiryNumber}` : "-",
          doneOuts,
          project: foundJob?.name || "-",
          product: foundPanelCode
            ? `${foundPanelCode?.type}: ${foundPanelCode?.name}`
            : "-",
          type: activity?.type || "-",
          hours: task.hours ?? 0,
          daysDeadline,
          donePic: formatDoneDate(task.completedDatePic),
          doneSpv: formatDoneDate(task.completedDateSpv),
          doneManager: formatDoneDate(task.completedDateManager),
          pic,
          inCharges: task.inCharges, // Tambahkan ini untuk keperluan filter PIC
          tasks: 1,
        };
      });
    })
    ?.filter((item) => {
      if (isCompleted.value !== null) {
        if (isCompleted.value === true && item.doneOuts !== "Done") return false;
        if (isCompleted.value === false && item.doneOuts === "Done") return false;
      }

      if (selectedActivityType.value !== null && item.type !== selectedActivityType.value) {
        return false;
      }

      // Tambahkan filter berdasarkan PIC
      if (
        selectedPicId.value &&
        !item.inCharges?.some(ic => `${ic.extUserId}` === `${selectedPicId.value}`)
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

.v-data-table {
  margin-bottom: 20px;
}
.v-card {
  margin-bottom: 20px;
}
</style>