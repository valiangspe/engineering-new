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
          <v-card-title class="headline">Outs</v-card-title>
          <v-card-text>
            <v-data-table :headers="headers" :items="items" class="elevation-1">
              <template v-slot:footer>
                <tr>
                  <td>Grand Total</td>
                  <td>{{ grandTotal.prePoActivity }}</td>
                  <td>{{ grandTotal.prePoProcess }}</td>
                  <td>{{ grandTotal.postPoActivity }}</td>
                  <td>{{ grandTotal.postPoProcess }}</td>
                  <td>{{ grandTotal.othersActivity }}</td>
                  <td>{{ grandTotal.othersProcess }}</td>

                  <td>{{ grandTotal.totalActivity }}</td>
                  <td>{{ grandTotal.totalProcess }}</td>
                </tr>
              </template>
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
  fetchUsers,
} from "./fetchers";

const users = ref([]);
const departments = ref([]);
const activities = ref([]);

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
const isCompleted = ref(false);

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

fetchUsersData();
fetchDepartmentsData();
fetchEngineeringActivitiesData();

const headers = ref([
  { title: "Engineer", value: "engineer" },
  { title: "Pre-PO Activity", value: "prePoActivity" },
  { title: "Pre-PO T. Process", value: "prePoProcess" },
  { title: "Post-PO Activity", value: "postPoActivity" },
  { title: "Post-PO T. Process", value: "postPoProcess" },
  { title: "Others Activity", value: "othersActivity" },
  { title: "Others T. Process", value: "othersProcess" },

  { title: "Total Activity", value: "totalActivity" },
  { title: "Total T. Process", value: "totalProcess" },
]);

const items = computed(() => {
  return users.value
    .filter((u) => {
      const dName = departments?.value?.find(
        (d) => `${d?.id}` === `${u?.departmentId}`
      )?.name;

      return ["eng", "estimat"].find((s) => dName?.toLowerCase()?.includes(s));
      return true;
    })
    .map((u) => {
      const foundActivities = activities.value.filter(
        (a) =>
          a?.tasks?.find((t) =>
            t?.inCharges?.find((i) => `${i?.extUserId}` === `${u?.id}`)
          ) &&
          (isCompleted.value !== null
            ? isCompleted.value === true
              ? !a?.tasks.find((t) => !t?.completedDate)
              : a?.tasks.find((t) => !t?.completedDate)
            : true)
      );

      const data = {
        engineer: u?.name ?? "",
        prePoActivity: foundActivities?.filter((a) => a.type === "PrePO")
          ?.length,
        prePoProcess: foundActivities
          ?.filter((a) => a.type === "PrePO")
          ?.reduce(
            (acc, a) =>
              acc +
              (a?.tasks?.reduce((acc, t) => acc + (t?.hours ?? 0), 0.0) ?? 0),
            0.0
          ),
        postPoActivity: foundActivities?.filter((a) => a.type === "PostPO")
          ?.length,
        postPoProcess: foundActivities
          ?.filter((a) => a.type === "PostPO")
          ?.reduce(
            (acc, a) =>
              acc +
              (a?.tasks?.reduce((acc, t) => acc + (t?.hours ?? 0), 0.0) ?? 0),
            0.0
          ),
        othersActivity: foundActivities?.filter((a) => a.type === "Others")
          ?.length,
        othersProcess: foundActivities
          ?.filter((a) => a.type === "Others")
          ?.reduce(
            (acc, a) =>
              acc +
              (a?.tasks?.reduce((acc, t) => acc + (t?.hours ?? 0), 0.0) ?? 0),
            0.0
          ),
      };

      // console.log(u?.name, foundActivities);
      return {
        ...data,
        totalActivity:
          data.prePoActivity + data.postPoActivity + data.othersActivity,
        totalProcess:
          data.prePoProcess + data.postPoProcess + data.othersProcess,
      };
    });
});

const grandTotal = computed(() => {
  return {
    prePoActivity: items.value.reduce(
      (sum, item) => sum + item.prePoActivity,
      0
    ),
    prePoProcess: items.value.reduce((sum, item) => sum + item.prePoProcess, 0),
    postPoActivity: items.value.reduce(
      (sum, item) => sum + item.postPoActivity,
      0
    ),
    postPoProcess: items.value.reduce(
      (sum, item) => sum + item.postPoProcess,
      0
    ),
    othersActivity: items.value.reduce(
      (sum, item) => sum + item.othersActivity,
      0
    ),
    othersProcess: items.value.reduce(
      (sum, item) => sum + item.othersProcess,
      0
    ),
    totalActivity: items.value.reduce(
      (sum, item) =>
        sum + item.prePoActivity + item.postPoActivity + item.othersActivity,
      0
    ),
    totalProcess: items.value.reduce(
      (sum, item) =>
        sum + item.prePoProcess + item.postPoProcess + item.othersProcess,
      0
    ),
  };
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
