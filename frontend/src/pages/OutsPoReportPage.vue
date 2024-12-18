<template>
  <v-container>
    <div>
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
          <v-card-title class="headline">Done</v-card-title>
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
import { ref, computed } from "vue";
import {
  fetchDepartments,
  fetchEngineeringActivities,
  fetchExtCrmPurchaseOrdersProtoSimple,
  fetchInquiries,
  fetchJobsProtoSimple,
  fetchUsers,
} from "./fetchers";

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
  // { title: "Done SPV", value: "doneSpv" },
  // { title: "Done Manager", value: "doneManager" },

  { title: "Days (Deadline)", value: "daysDeadline" },
  { title: "Tasks", value: "tasks" },
]);

const items = computed(() => {
  return activities.value
    ?.filter(
      (a) =>
        (selectedActivityType.value
          ? `${a?.type}` === `${selectedActivityType.value}`
          : true) &&
        (isCompleted.value !== null
          ? isCompleted.value === true
            ? !a?.tasks?.find((a) => !a?.completedDate)
            : a?.tasks?.find((a) => !a?.completedDate)
          : true) &&
        (a?.type === "PostPO" ? a?.extPurchaseOrderId : true) &&
        (a?.type === "PrePO" ? a?.extInquiryId : true)
    )
    .map((a) => {
      const foundPO = pos.value.find(
        (p) => `${p?.id}` === `${a?.extPurchaseOrderId}`
      );

      const foundJob = jobs.value.jobs?.find(
        (j) => `${j?.masterJavaBaseModel?.id}` === `${a?.extJobId}`
      );

      const foundPanelCode = jobs.value.jobs
        ?.flatMap((j) => j?.panelCodes)
        .find(
          (c) => `${c?.masterJavaBaseModel?.id}` === `${a?.extPanelCodeId}`
        );

      const foundUsers = [
        ...new Set(
          a?.tasks?.flatMap((t) => t?.inCharges.map((c) => c.extUserId))
        ),
      ]
        .map((id) => users.value?.find((u) => `${u?.id}` === `${id}`)?.name)
        .join(", ");

      const foundInq = inquiries?.value?.find(
        (t) => `${t?.id}` === `${a?.extInquiryId}`
      );

      // Format tanggal done
      const formatDoneDate = (date) =>
        date
          ? new Date(date).toLocaleDateString("id-ID", {
              day: "2-digit",
              month: "long",
              year: "numeric",
            })
          : "-";

      // Ambil tanggal Done PIC, SPV, dan Manager
      const donePic = a?.tasks?.find((t) => t.completedDatePic)?.completedDatePic;
      const doneSpv = a?.tasks?.find((t) => t.completedDateSpv)?.completedDateSpv;
      const doneManager = a?.tasks?.find((t) => t.completedDateManager)
        ?.completedDateManager;

      const data = {
        po: foundPO?.purchaseOrderNumber,
        customer: foundPO?.account?.name ?? foundInq?.customer?.name ?? "",
        inquiry: (() => {
          return foundInq ? `${foundInq?.inquiryNumber}` : "";
        })(),
        doneOuts: (() => {
          const totalTasks = a?.tasks?.length || 0; // Total task
          const doneTasks = a?.tasks?.filter((t) =>
            t?.completedDatePic || t?.completedDateSpv || t?.completedDateManager
          )?.length || 0; // Task yang selesai (PIC, SPV, atau Manager)

          return totalTasks === doneTasks && totalTasks > 0
            ? "Done" // Jika semua task selesai
            : `Outs (${doneTasks}/${totalTasks})`; // Jika belum selesai
        })(),
        project: foundJob?.name,
        product: foundPanelCode
          ? `${foundPanelCode?.type}: ${foundPanelCode?.name}`
          : "",
        type: foundUsers,
        daysDeadline: (() => {
          if (a?.toCache) {
            const deadlineDate = new Date(a.toCache);
            const remainingDays = Math.round(
              (deadlineDate.getTime() - new Date().getTime()) / 86400000
            );
            return `${deadlineDate.toLocaleDateString("id-ID", {
              day: "2-digit",
              month: "long",
              year: "numeric",
            })} (${remainingDays} hari)`;
          }
          return "";
        })(),
        donePic: formatDoneDate(donePic),
        doneSpv: formatDoneDate(doneSpv),
        doneManager: formatDoneDate(doneManager),
        tasks: a?.tasks?.length,
      };

      return {
        ...data,
      };
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
