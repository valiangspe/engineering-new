<template>
  <div class="m-3">
    <h1 class="text-h4 mb-4">Engineering Manpower Overview</h1>

    <!-- Vuetify Autocomplete input -->
    <v-autocomplete
      v-model="selectedDepartment"
      :items="departments"
      item-title="name"
      item-value="id"
      label="Select Department"
      placeholder="Type to search departments"
      return-object
      @update:model-value="addDepartment"
      class="mb-4"
    ></v-autocomplete>

    <!-- Filter button -->
    <v-btn color="primary" @click="filterDepartments" class="mb-4">
      Filter Departments
    </v-btn>

    <!-- Save button -->
    <v-btn color="success" @click="saveDeptConfigs" class="mb-4">
      Save Departments
    </v-btn>

    <div><h4>Engineering Overview</h4></div>

    <!-- Selected departments table -->
    <div
      class="overflow-auto border border-dark"
      style="height: 30vh; resize: vertical"
    >
      <table class="table table-sm" style="border-collapse: separate">
        <thead>
          <tr>
            <th
              :class="`border border-dark ${
                h.includes('T+5') || h.includes('5D')
                  ? `bg-light text-dark`
                  : `bg-dark text-light`
              } `"
              style="position: sticky; top: 0"
              v-for="h in [
                '#',
                'ID',
                'Name',
                'Manpowers',
                'SPV',
                'Staff',
                'SPV hr',
                'Staff hr',

                'NW+5D',
                'pre',
                'post',
                'others',

                'target',
                'total',

                'N2W+5D',
                'pre',
                'post',
                'others',

                'target',
                'total',

                'Action',
              ]"
            >
              {{ h }}
            </th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(d, i) in deptConfigsMapped" :key="d?.department?.id">
            <template
              v-for="res in [
                {
                  spvs: d.users?.filter((u) => u?.isHead),
                  staffs: d.users?.filter((u) => !u?.isHead),

                  t5Pre: filterActivities({
                    activities: activities,
                    departmentId: d?.department?.id,
                    type: 'PrePO',
                    from: makeDateString(new Date()),
                    to: makeDateString(
                      new Date(
                        new Date().getFullYear(),
                        new Date().getMonth(),
                        new Date().getDate() + 6
                      )
                    ),
                  })
                    .flatMap((a) => a.tasks)
                    .reduce((acc, t) => acc + (t?.hours ?? 0), 0.0),
                  t5Post: filterActivities({
                    activities: activities,
                    departmentId: d?.department?.id,
                    type: 'PostPO',
                    from: makeDateString(new Date()),
                    to: makeDateString(
                      new Date(
                        new Date().getFullYear(),
                        new Date().getMonth(),
                        new Date().getDate() + 6
                      )
                    ),
                  })
                    .flatMap((a) => a.tasks)
                    .reduce((acc, t) => acc + (t?.hours ?? 0), 0.0),
                  t5Others: filterActivities({
                    activities: activities,
                    departmentId: d?.department?.id,
                    type: 'Others',
                    from: makeDateString(new Date()),
                    to: makeDateString(
                      new Date(
                        new Date().getFullYear(),
                        new Date().getMonth(),
                        new Date().getDate() + 6
                      )
                    ),
                  })
                    .flatMap((a) => a.tasks)
                    .reduce((acc, t) => acc + (t?.hours ?? 0), 0.0),
                  nont5Pre: filterActivities({
                    activities: activities,
                    departmentId: d?.department?.id,
                    type: 'PrePO',
                    from: makeDateString(
                      new Date(
                        new Date().getFullYear(),
                        new Date().getMonth(),
                        new Date().getDate() + 7
                      )
                    ),
                  })
                    .flatMap((a) => a.tasks)
                    .reduce((acc, t) => acc + (t?.hours ?? 0), 0.0),
                  nont5Post: filterActivities({
                    activities: activities,
                    departmentId: d?.department?.id,
                    type: 'PostPO',
                    from: makeDateString(
                      new Date(
                        new Date().getFullYear(),
                        new Date().getMonth(),
                        new Date().getDate() + 7
                      )
                    ),
                  })
                    .flatMap((a) => a.tasks)
                    .reduce((acc, t) => acc + (t?.hours ?? 0), 0.0),
                  nont5Others: filterActivities({
                    activities: activities,
                    departmentId: d?.department?.id,
                    type: 'Others',
                    from: makeDateString(
                      new Date(
                        new Date().getFullYear(),
                        new Date().getMonth(),
                        new Date().getDate() + 7
                      )
                    ),
                  })
                    .flatMap((a) => a.tasks)
                    .reduce((acc, t) => acc + (t?.hours ?? 0), 0.0),
                },
              ]"
            >
              <template
                v-for="res2 in [
                  {
                    spvHrs: res.spvs.length * 20,
                    staffHrs: res.staffs.length * 40,

                    nonspvHrs: res.spvs.length * 20 * 3,
                    nonstaffHrs: res.staffs.length * 40 * 3,

                    t5Total: filterActivities({
                      activities: activities,
                      departmentId: d?.department?.id,
                      from: makeDateString(new Date()),
                      to: makeDateString(
                        new Date(
                          new Date().getFullYear(),
                          new Date().getMonth(),
                          new Date().getDate() + 6
                        )
                      ),
                    })
                      .flatMap((a) => a.tasks)
                      .reduce((acc, t) => acc + (t?.hours ?? 0), 0.0),
                    nont5Total: filterActivities({
                      activities: activities,
                      departmentId: d?.department?.id,
                      from: makeDateString(
                        new Date(
                          new Date().getFullYear(),
                          new Date().getMonth(),
                          new Date().getDate() + 7
                        )
                      ),
                    })
                      .flatMap((a) => a.tasks)
                      .reduce((acc, t) => acc + (t?.hours ?? 0), 0.0),
                  },
                ]"
              >
                <td class="border border-dark">{{ i + 1 }}</td>
                <td class="border border-dark">{{ d?.department?.id }}</td>
                <td class="border border-dark">{{ d?.department?.name }}</td>
                <td class="border border-dark">
                  {{ d?.users?.length }}
                </td>
                <td class="border border-dark">
                  {{ res.spvs.length }}
                </td>
                <td class="border border-dark">
                  {{ res.staffs.length }}
                </td>
                <td class="border border-dark">
                  {{ res2.spvHrs }}
                </td>
                <td class="border border-dark">
                  {{ res2.staffHrs }}
                </td>

                <td class="border border-dark"></td>
                <td class="border border-dark">
                  {{ res.t5Pre }}
                </td>
                <td class="border border-dark">
                  {{ res.t5Post }}
                </td>
                <td class="border border-dark">
                  {{ res.t5Others }}
                </td>
                <td class="border border-dark">
                  <strong>{{ res2.spvHrs + res2.staffHrs }}</strong>
                </td>
                <td
                  :class="`border border-dark ${
                    res2.t5Total > res2.spvHrs + res2.staffHrs
                      ? `bg-warning`
                      : `bg-success`
                  }`"
                >
                  {{ res2.t5Total }}
                </td>

                <td class="border border-dark">
                  <strong></strong>
                </td>
                <td class="border border-dark">
                  {{ res.nont5Pre }}
                </td>
                <td class="border border-dark">
                  {{ res.nont5Post }}
                </td>
                <td class="border border-dark">
                  {{ res.nont5Others }}
                </td>
                <td class="border border-dark">
                  <strong>{{ res2.nonspvHrs + res2.nonstaffHrs }}</strong>
                </td>
                <td
                  :class="`border border-dark ${
                    res2.nont5Total > res2.nonspvHrs + res2.nonstaffHrs
                      ? `bg-warning`
                      : `bg-success`
                  }`"
                >
                  {{ res2.nont5Total }}
                </td>

                <td class="border border-dark">
                  <v-btn
                    color="error"
                    size="small"
                    @click="removeDepartment(dept)"
                  >
                    Remove
                  </v-btn>
                </td>
              </template>
            </template>
          </tr>
        </tbody>
      </table>
    </div>

    <div><h4>Engineering Manpower Overview</h4></div>

    <!-- Selected departments table -->
    <div
      class="overflow-auto border border-dark"
      style="height: 30vh; resize: vertical"
    >
      <table class="table table-sm" style="border-collapse: separate">
        <thead>
          <tr>
            <th
              :class="`border border-dark ${
                h.includes('T+5') || h.includes('5D')
                  ? `bg-light text-dark`
                  : `bg-dark text-light`
              } `"
              style="position: sticky; top: 0"
              v-for="h in [
                '#',
                'ID',
                'Name',
                'Department',
                // 'Manpowers',
                // 'SPV',
                // 'Staff',
                // 'SPV hr',
                // 'Staff hr',
                'hr',

                'NW+5D',
                'pre',
                'post',
                'others',

                'target',
                'total',

                'N2W+5D',
                'pre',
                'post',
                'others',

                'target',
                'total',

                'Action',
              ]"
            >
              {{ h }}
            </th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(d, i) in deptConfigsMappedUsers" :key="d?.department?.id">
            <template
              v-for="res in [
                {
                  spvs: users
                    .filter((u) => u?.departmentId === d?.department?.id)
                    ?.filter((u) => u?.isHead),
                  staffs: users
                    .filter((u) => u?.departmentId === d?.department?.id)
                    ?.filter((u) => !u?.isHead),

                  t5Pre: filterActivities({
                    activities: activities,
                    departmentId: d?.department?.id,
                    userId: d?.user?.id,
                    type: 'PrePO',
                    from: makeDateString(new Date()),
                    to: makeDateString(
                      new Date(
                        new Date().getFullYear(),
                        new Date().getMonth(),
                        new Date().getDate() + 6
                      )
                    ),
                  })
                    .flatMap((a) => a.tasks)
                    .reduce((acc, t) => acc + (t?.hours ?? 0), 0.0),
                  t5Post: filterActivities({
                    activities: activities,
                    departmentId: d?.department?.id,
                    userId: d?.user?.id,
                    type: 'PostPO',
                    from: makeDateString(new Date()),
                    to: makeDateString(
                      new Date(
                        new Date().getFullYear(),
                        new Date().getMonth(),
                        new Date().getDate() + 6
                      )
                    ),
                  })
                    .flatMap((a) => a.tasks)
                    .reduce((acc, t) => acc + (t?.hours ?? 0), 0.0),
                  t5Others: filterActivities({
                    activities: activities,
                    departmentId: d?.department?.id,
                    userId: d?.user?.id,
                    type: 'Others',
                    from: makeDateString(new Date()),
                    to: makeDateString(
                      new Date(
                        new Date().getFullYear(),
                        new Date().getMonth(),
                        new Date().getDate() + 6
                      )
                    ),
                  })
                    .flatMap((a) => a.tasks)
                    .reduce((acc, t) => acc + (t?.hours ?? 0), 0.0),
                  nont5Pre: filterActivities({
                    activities: activities,
                    departmentId: d?.department?.id,
                    userId: d?.user?.id,
                    type: 'PrePO',
                    from: makeDateString(
                      new Date(
                        new Date().getFullYear(),
                        new Date().getMonth(),
                        new Date().getDate() + 7
                      )
                    ),
                  })
                    .flatMap((a) => a.tasks)
                    .reduce((acc, t) => acc + (t?.hours ?? 0), 0.0),
                  nont5Post: filterActivities({
                    activities: activities,
                    departmentId: d?.department?.id,
                    userId: d?.user?.id,
                    type: 'PostPO',
                    from: makeDateString(
                      new Date(
                        new Date().getFullYear(),
                        new Date().getMonth(),
                        new Date().getDate() + 7
                      )
                    ),
                  })
                    .flatMap((a) => a.tasks)
                    .reduce((acc, t) => acc + (t?.hours ?? 0), 0.0),
                  nont5Others: filterActivities({
                    activities: activities,
                    departmentId: d?.department?.id,
                    userId: d?.user?.id,
                    type: 'Others',
                    from: makeDateString(
                      new Date(
                        new Date().getFullYear(),
                        new Date().getMonth(),
                        new Date().getDate() + 7
                      )
                    ),
                  })
                    .flatMap((a) => a.tasks)
                    .reduce((acc, t) => acc + (t?.hours ?? 0), 0.0),
                },
              ]"
            >
              <template
                v-for="res2 in [
                  {
                    // spvHrs: res.spvs.length * 20,
                    // staffHrs: res.staffs.length * 40,

                    // nonspvHrs: res.spvs.length * 20 * 3,
                    // nonstaffHrs: res.staffs.length * 40 * 3,
                    hrs: d?.user?.isHead ? 20 : 40,

                    t5Total: filterActivities({
                      activities: activities,
                      departmentId: d?.department?.id,
                      userId: d?.user?.id,
                      from: makeDateString(new Date()),
                      to: makeDateString(
                        new Date(
                          new Date().getFullYear(),
                          new Date().getMonth(),
                          new Date().getDate() + 6
                        )
                      ),
                    })
                      .flatMap((a) => a.tasks)
                      .reduce((acc, t) => acc + (t?.hours ?? 0), 0.0),
                    nont5Total: filterActivities({
                      activities: activities,
                      departmentId: d?.department?.id,
                      userId: d?.user?.id,
                      from: makeDateString(
                        new Date(
                          new Date().getFullYear(),
                          new Date().getMonth(),
                          new Date().getDate() + 7
                        )
                      ),
                    })
                      .flatMap((a) => a.tasks)
                      .reduce((acc, t) => acc + (t?.hours ?? 0), 0.0),
                  },
                ]"
              >
                <td class="border border-dark">{{ i + 1 }}</td>
                <td class="border border-dark">{{ d?.user?.id }}</td>
                <td class="border border-dark">{{ d?.user?.name }}</td>
                <td class="border border-dark">{{ d?.department?.name }}</td>

                <!-- <td class="border border-dark">
              {{ d?.users?.length }}
            </td>
            <td class="border border-dark">
              {{ res.spvs.length }}
            </td>
            <td class="border border-dark">
              {{ res.staffs.length }}
            </td>
            <td class="border border-dark">
              {{ res2.spvHrs }}
            </td>
            <td class="border border-dark">
              {{ res2.staffHrs }}
            </td> -->

                <td class="border border-dark">
                  {{ d?.user?.isHead ? 20 : 40 }}
                </td>

                <td class="border border-dark"></td>
                <td class="border border-dark">
                  {{ res.t5Pre }}
                </td>
                <td class="border border-dark">
                  {{ res.t5Post }}
                </td>
                <td class="border border-dark">
                  {{ res.t5Others }}
                </td>
                <td class="border border-dark">
                  <strong> {{ d?.user?.isHead ? 20 : 40 }} </strong>
                </td>
                <td
                  :class="`border border-dark ${
                    res2.t5Total > res2.hrs ? `bg-warning` : `bg-success`
                  }`"
                >
                  {{ res2.t5Total }}
                </td>

                <td class="border border-dark">
                  <strong></strong>
                </td>
                <td class="border border-dark">
                  {{ res.nont5Pre }}
                </td>
                <td class="border border-dark">
                  {{ res.nont5Post }}
                </td>
                <td class="border border-dark">
                  {{ res.nont5Others }}
                </td>
                <td class="border border-dark">
                  <strong> {{ (d?.user?.isHead ? 20 : 40) * 3 }} </strong>
                </td>
                <td
                  :class="`border border-dark ${
                    res2.nont5Total > res2.hrs * 3 ? `bg-warning` : `bg-success`
                  }`"
                >
                  {{ res2.nont5Total }}
                </td>

                <td class="border border-dark">
                  <v-btn
                    color="error"
                    size="small"
                    @click="removeDepartment(dept)"
                  >
                    Remove
                  </v-btn>
                </td>
              </template>
            </template>
          </tr>
        </tbody>
      </table>
    </div>

    <div><h4>T+5 Overview</h4></div>

    <!-- Selected departments table -->
    <div
      class="overflow-auto border border-dark"
      style="height: 30vh; resize: vertical"
    >
      <table class="table table-sm" style="border-collapse: separate">
        <thead>
          <tr>
            <th
              :class="`border border-dark ${
                days.includes(h) ? `bg-light text-dark` : `bg-dark text-light`
              }`"
              style="position: sticky; top: 0"
              v-for="h in [
                '#',
                'ID',
                'Dept',
                ...dateArrNow
                  .map((n) => [
                    days[
                      (() => {
                        let day = n.getDay();

                        if (day > 6) {
                          day = day - 6;
                        }

                        return [day];
                      })()
                    ],
                    'Beg',
                    'In',
                    'Out',
                  ])
                  .flat(),
                'Balance',
                'Action',
              ]"
            >
              {{ h }}
            </th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(d, i) in deptConfigsMapped" :key="d?.department?.id">
            <template
              v-for="dy in [
                {
                  dates: dateArrNow
                    .map((dz) => ({
                      date: dz,
                      activities: filterActivities({
                        activities: activities,
                        departmentId: d?.department?.id,
                        // type: 'PrePO',
                        from: makeDateString(new Date(dz)),
                        to: makeDateString(new Date(dz)),
                      }),
                    }))
                    .reduce((acc, dz) => {
                      console.log(
                        `phase acc dept ${
                          departments?.find(
                            (dx) => `${dx?.id}` === `${d?.department?.id}`
                          )?.name
                        }`,
                        acc
                      );
                      return [
                        ...acc,
                        {
                          ...dz,
                          accumulatedHrs:
                            (acc?.slice(-1)?.[0]?.accumulatedHrs ?? 0) +
                            dz?.activities
                              ?.flatMap((a) => a.tasks)
                              ?.reduce((acc, t) => acc + t.hours, 0),
                        },
                      ];
                    }, []),
                },
              ]"
            >
              <td class="border border-dark">{{ i + 1 }}</td>
              <td class="border border-dark">{{ d?.department?.id }}</td>
              <td class="border border-dark">{{ d?.department?.name }}</td>
              <template v-for="dz in dy.dates">
                <template
                  v-for="db in [
                    {
                      activitiesHours: dz.activities
                        .flatMap((a) => a.tasks)
                        .reduce((acc, t) => acc + (t?.hours ?? 0), 0.0),
                    },
                  ]"
                >
                  <td class="border border-dark"></td>
                  <td class="border border-dark">
                    {{ dz?.accumulatedHrs }}
                  </td>
                  <td class="border border-dark">
                    {{ 0 }}
                  </td>
                  <td class="border border-dark">
                    {{ 0 }}
                  </td>
                </template>
              </template>

              <td class="border border-dark">
                {{ dy.dates?.slice(-1)?.[0]?.accumulatedHrs }}
              </td>
            </template>

            <td class="border border-dark">
              <v-btn color="error" size="small" @click="removeDepartment(dept)">
                Remove
              </v-btn>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from "vue";
import { days, filterActivities, makeDateString } from "../helpers";
import { fetchEngineeringActivities } from "./fetchers";

const departments = ref([]);
const deptConfigs = ref([]);
const users = ref([]);
const activities = ref([]);

const deptConfigsMapped = computed(() => {
  const dc = deptConfigs.value?.map((c) => ({
    config: c,
    department: departments.value.find(
      (d) => `${d?.id}` === `${c?.departmentId}`
    ),
    users: users.value.filter(
      (u) => `${u?.departmentId}` === `${c?.departmentId}`
    ),
  }));

  console.log(dc);

  return dc;
});

const deptConfigsMappedUsers = computed(() => {
  const dc = deptConfigs.value
    ?.map((c) => ({
      config: c,
      department: departments.value.find(
        (d) => `${d?.id}` === `${c?.departmentId}`
      ),
      users: users.value.filter(
        (u) => `${u?.departmentId}` === `${c?.departmentId}`
      ),
    }))
    .flatMap((d) =>
      d.users.map((u) => ({
        user: u,
        department: d.department,
        config: d.config,
      }))
    );

  console.log(dc);

  return dc;
});

const fetchDepartments = async () => {
  try {
    const response = await fetch(
      "https://meeting-backend.iotech.my.id/ext-departments"
    );
    const data = await response.json();
    departments.value = data;
  } catch (error) {
    console.error("Error fetching departments:", error);
  }
};

const fetchUsers = async () => {
  try {
    const response = await fetch(
      "https://meeting-backend.iotech.my.id/ext-users"
    );
    const data = await response.json();
    users.value = data;
  } catch (error) {
    console.error("Error fetching departments:", error);
  }
};

const fetchDeptConfigs = async () => {
  try {
    const response = await fetch(
      `${import.meta.env.VITE_APP_BASE_URL}/api/deptconfigs`
    );
    const data = await response.json();
    deptConfigs.value = data;
  } catch (error) {
    console.error("Error fetching departments:", error);
  }
};

const addDepartment = (department) => {
  console.log(
    deptConfigs.value?.some((dept) => dept.departmentId === department.id)
  );
  if (
    department &&
    !deptConfigs.value?.some((dept) => dept.departmentId === department.id)
  ) {
    deptConfigs.value = [
      ...deptConfigs.value,
      { departmentId: department?.id },
    ];
    console.log(deptConfigs.value);
    return;
  }
  deptConfigs.value = null;
};

const removeDepartment = (department) => {
  deptConfigs.value = deptConfigs.value.filter(
    (dept) => dept?.departmentId !== department.id
  );
};

const filterDepartments = () => {
  departments.value = departments.value.filter(
    (dept) =>
      !deptConfigs.value.some(
        (selectedDept) => selectedDept?.departmentId === dept.id
      )
  );
};

const saveDeptConfigs = async () => {
  try {
    const response = await fetch(
      `${import.meta.env.VITE_APP_BASE_URL}/api/deptconfigs`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(deptConfigs.value),
      }
    );
    if (response.ok) {
      console.log("Departments saved successfully.");
    } else {
      console.error("Failed to save departments.");
    }
  } catch (error) {
    console.error("Error saving departments:", error);
  }
};

const fetchEngineeringActivitiesData = async () => {
  const d = await fetchEngineeringActivities();
  activities.value = d;
};

onMounted(() => {
  fetchDepartments();
  fetchDeptConfigs();
  fetchEngineeringActivitiesData();
  fetchUsers();
});

const dateArrNow = [...Array(6)]
  .map((_, i) => i)
  .map(
    (n) =>
      new Date(
        new Date().getFullYear(),
        new Date().getMonth(),
        new Date().getDate() + n
      )
  );
</script>
