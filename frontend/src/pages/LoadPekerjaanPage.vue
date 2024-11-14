<template>
  <div class="m-3">
    <v-row>
      <v-col>
        <v-card>
          <v-card-title class="headline">Done</v-card-title>
          <!-- <div>
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
            </div>
          </div> -->

          <div>
            <div class="d-flex align-items-end">
              <div>
                <div>From</div>
                <div>
                  <input
                    :value="from"
                    type="date"
                    class="form-control form-control-sm"
                    @blur="
                      (e) => {
                        from = e.target.value;

                        fetchEngineeringActivitiesData();
                      }
                    "
                  />
                </div>
              </div>
              <div>
                <div>To</div>
                <div>
                  <input
                    :value="to"
                    type="date"
                    class="form-control form-control-sm"
                    @blur="
                      (e) => {
                        to = e.target.value;

                        fetchEngineeringActivitiesData();
                      }
                    "
                  />
                </div>
              </div>
            </div>
          </div>

          <v-card-text>
            <v-data-table
              :headers="headers"
              :items="items"
              class="elevation-1"
              hide-default-footer
            >
              <template v-slot:footer>
                <tr>
                  <td>Actual</td>
                  <td>{{ actual.days }}</td>
                  <td>hari</td>
                </tr>
              </template>
            </v-data-table>
          </v-card-text>
        </v-card>
      </v-col>
      <v-col>
        <v-card-title class="headline">Details</v-card-title>
        <div
          class="overflow-auto border border-dark"
          style="resize: vertical; height: 75vh"
        >
          <table class="table table-sm" style="border-collapse: separate">
           
           <thead>

           
            <tr>
              <th
                v-for="h in [
                  '#',
                  'Name',
                  'PIC',
                  'Dept',
                  'Date',
                  'Type',
                  'PO',
                  'Inquiry',
                ]"
                class="bg-dark text-light"
                style="position: sticky; top: 0"
              >
                {{ h }}
              </th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(d, i) in activitiesMapped">
              <template
                v-for="dx in [
                  {
                    foundPO: pos.find(
                      (p) => `${p?.id}` === `${d?.activity?.extPurchaseOrderId}`
                    ),
                    foundInquiry: inquiries.find(
                      (p) => `${p?.id}` === `${d?.activity?.extInquiryId}`
                    ),
                  },
                ]"
              >
                <td class="border border-dark">{{ i + 1 }}</td>
                <td class="border border-dark">{{ d?.task?.description }}</td>
                <td class="border border-dark">
                  {{
                    d?.task?.inCharges
                      ?.filter((p) => {
                        const foundUser = users.find(
                          (u) => `${u?.id}` === `${p?.extUserId}`
                        );

                        return foundUser;
                      })
                      ?.map((p) => {
                        const foundUser = users.find(
                          (u) => `${u?.id}` === `${p?.extUserId}`
                        );

                        return `${foundUser?.username} (${d?.task?.hours}h)`;
                      })
                      .join(", ")
                  }}
                </td>
                <td class="border border-dark">
                  {{
                    [
                      ...new Set(
                        d?.task?.inCharges?.map((p) => {
                          const foundUser = users.find(
                            (u) => `${u?.id}` === `${p?.extUserId}`
                          );

                          return foundUser?.departmentId;
                        })
                      ),
                    ]
                      .filter((did) => did)
                      .map((did) => {
                        const foundDept = departments.find(
                          (d) => `${d?.id}` === `${did}`
                        );
                        return foundDept?.name;
                      })
                      .join(",")
                  }}
                </td>
                <td class="border border-dark">
                  {{
                    d?.task?.createdAt
                      ? Intl.DateTimeFormat("en-US", {
                          dateStyle: "short",
                          timeStyle: "short",
                        }).format(new Date(d?.task?.createdAt ?? ""))
                      : ""
                  }}
                </td>

                <td class="border border-dark">{{ d?.activity?.type }}</td>
                <td class="border border-dark">
                  {{ dx?.foundPO?.purchaseOrderNumber }}
                </td>
                <td class="border border-dark">
                  {{ dx?.foundInquiry?.inquiryNumber }}
                </td>
              </template>
            </tr>
          </tbody>
          </table>
        </div>
      </v-col>
    </v-row>
  </div>
</template>

<script setup>
import { ref, computed } from "vue";
import { Doughnut } from "vue-chartjs";
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  ArcElement,
  CategoryScale,
  LinearScale,
} from "chart.js";
import {
  fetchDepartments,
  fetchEngineeringActivities,
  fetchExtCrmPurchaseOrdersProtoSimple,
  fetchInquiries,
  fetchUsers,
} from "./fetchers";
import { makeDateString } from "../helpers";

ChartJS.register(
  Title,
  Tooltip,
  Legend,
  ArcElement,
  CategoryScale,
  LinearScale
);

// Create refs for selected month and year
const selectedMonth = ref(new Date().getMonth());
const selectedYear = ref(new Date().getFullYear());
const activities = ref([]);
const pos = ref([]);
const inquiries = ref([]);
const users = ref([]);
const requiredRule = [(v) => !!v || "Required."];

const from = ref(
  makeDateString(new Date(new Date().getFullYear(), new Date().getMonth(), 1))
);
const to = ref(
  makeDateString(
    new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0)
  )
);

const menu = ref(false);

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
    from: new Date(`${from.value}T00:00:00`).toISOString(),
    to: new Date(`${to.value}T23:59:39`).toISOString(),
  });
  activities.value = d;
};
const fetchPosData = async () => {
  const d = await fetchExtCrmPurchaseOrdersProtoSimple({});
  pos.value = d;
};
const fetchInquiriesData = async () => {
  const d = await fetchInquiries({});
  inquiries.value = d;
};

fetchPosData();
fetchInquiriesData();
fetchEngineeringActivitiesData();

const departments = ref([]);

const fetchUsersData = async () => {
  const d = await fetchUsers();

  users.value = d;
};

const fetchDepartmentsData = async () => {
  const d = await fetchDepartments();
  departments.value = d;
};

fetchUsersData();
fetchDepartmentsData();

const engEstimatorUsers = computed(() => {
  return users.value.filter((u) => {
    const foundDept = departments.value.find(
      (d) => `${d?.id}` === `${u?.departmentId}`
    );

    return (
      ["eng", "estimat"].find((s) =>
        foundDept?.name?.toLowerCase()?.includes(s)
      ) &&
      (u?.entryDate
        ? new Date(u?.entryDate).getTime() <
          new Date(selectedYear.value, selectedMonth.value, 1).getTime()
        : true)
    );
  });
});

const headers = ref([
  { title: "Perhitungan load pekerjaan", value: "description" },
  { title: "", value: "value" },
  { title: "", value: "unit" },
]);

const items = computed(() => {
  const activitiesHours = activities.value
    ?.flatMap((a) => a?.tasks)
    ?.filter((t) => !t?.deletedAt)
    ?.reduce((acc, t) => acc + (t?.hours ?? 0), 0.0);
  return [
    { description: "Outs", value: activitiesHours, unit: "Jam" },
    {
      description: "Jumlah man power",
      value: engEstimatorUsers.value?.length,
      unit: "orang",
    },
    {
      description: "Load per orang",
      value: (
        activitiesHours / (engEstimatorUsers.value?.length ?? 1)
      )?.toFixed(1),
      unit: "Jam",
    },
    {
      description: "Dapat diselesaikan dalam",
      value: (
        activitiesHours /
        (engEstimatorUsers.value?.length ?? 1) /
        8
      )?.toFixed(1),
      unit: "hari",
    },
  ];
});

const actual = computed(() => {
  return {
    days: 22,
  };
});

const activitiesMapped = computed(() => {
  return activities.value.flatMap((a) =>
    a.tasks.map((t) => ({ task: t, activity: a }))
  );
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
