<script setup>
import { ref } from "vue";
import {
  fetchDepartments,
  fetchEngineeringActivities,
  fetchJobsProtoSimple,
  fetchUsers,
} from "./fetchers";
import { intlFormat, makeDateString } from "../helpers";
import chroma from "chroma-js";

const activities = ref([]);
const users = ref([]);
const departments = ref([]);
const dialog = ref(false);
const selectedActivity = ref(null);

const from = ref(makeDateString(new Date()));
const to = ref(makeDateString(new Date()));

const defaultForm = {
  type: "PrePO",
  tasks: [],
};

const form = ref({ ...defaultForm });

// const users = ref([{ id: 1, name: "valian" }]);
const activityTypes = ref(["PrePO", "PostPO", "Others"]);
const inCharges = ref([{ name: "John" }, { name: "Doe" }]);
const selectedInCharges = ref([]);

const jobs = ref({});

const requiredRule = [(v) => !!v || "Required."];

const menu = ref(false);

const addTask = () => {
  form.value = {
    ...form.value,
    tasks: [
      ...form.value.tasks,
      {
        from: new Date(`${makeDateString(new Date())}T00:00:00Z`).toISOString(),
        to: new Date(`${makeDateString(new Date())}T00:00:00Z`).toISOString(),
      },
    ],
  };
};

const removeTask = (index) => {
  form.value.tasks.splice(index, 1);
};

// const submit = handleSubmit((values) => {
//   console.log("Form submitted:", values);
// });

const fetchUsersData = async () => {
  const d = await fetchUsers();
  console.log("users", d);
  users.value = d;
};
const fetchDepartmentsData = async () => {
  const d = await fetchDepartments();
  departments.value = d;
};

const fetchJobsProtoSimpleData = async () => {
  const d = await fetchJobsProtoSimple({ all: true });
  jobs.value = d;
};

const fetchEngineeringActivitiesData = async () => {
  const d = await fetchEngineeringActivities();
  activities.value = d;
};

const handleSave = async () => {
  console.log(form.value);
  try {
    const resp = await fetch(
      `${import.meta.env.VITE_APP_BASE_URL}/api/dashboard/activities`,
      {
        method: "post",
        headers: { "content-type": "application/json" },
        body: JSON.stringify(form.value),
      }
    );

    dialog.value = false;
    form.value = { ...defaultForm };

    fetchEngineeringActivitiesData();
  } catch (e) {}
};

fetchUsersData();
fetchEngineeringActivitiesData();
fetchDepartmentsData();
fetchJobsProtoSimpleData();
</script>
<template>
  <div class="m-3">
    <div>
      <div>
        <h4>Activity</h4>
      </div>
    </div>
    <div>
      <hr class="border border-dark" />
    </div>
    <div class="d-flex align-items-end">
      <div>
        <div>From</div>
        <div>
          <input
            :value="from"
            type="date"
            class="form-control form-control-sm"
          />
        </div>
      </div>
      <div>
        <div>To</div>
        <div>
          <input :value="to" type="date" class="form-control form-control-sm" />
        </div>
      </div>

      <div class="mx-2">
        <button class="btn btn-sm btn-primary" @click="dialog = true">
          <v-icon icon="mdi-plus" /> Add
        </button>
      </div>
    </div>
    <div
      class="overflow-auto border border-dark"
      style="height: 60vh; resize: vertical"
    >
      <table class="table table-sm" style="border-collapse: separate">
        <tr>
          <th
            style="position: sticky; top: 0"
            class="bg-dark text-light p-0 m-0"
            v-for="(h, i) in [
              '#',
              'Type',
              'Inquiry',
              'Job',

              'From',
              'To',
              'PIC Depts',
              'Tasks',
              'Tasks Done',
              'TotalHrs',
              'Action',
            ]"
          >
            {{ h }}
          </th>
        </tr>
        <tr v-for="(a, i) in activities">
          <td class="border border-dark p-0 m-0">{{ i + 1 }}</td>
          <td class="border border-dark p-0 m-0">{{ a?.type }}</td>
          <td class="border border-dark p-0 m-0"></td>
          <td class="border border-dark p-0 m-0">
            {{
              jobs?.jobs?.find(
                (j) => `${j?.masterJavaBaseModel?.id}` === `${a?.extJobId}`
              )?.name
            }}
          </td>

          <td class="border border-dark p-0 m-0">
            {{
              intlFormat({
                date: a?.fromCache?.split("T")?.[0],
                dateStyle: "medium",
              })
            }}
          </td>
          <td class="border border-dark p-0 m-0">
            {{
              intlFormat({
                date: a?.toCache?.split("T")?.[0],
                dateStyle: "medium",
              })
            }}
          </td>
          <td class="border border-dark p-0 m-0">
            {{
              [
                ...new Set(
                  a?.tasks
                    ?.flatMap((t) => t?.inCharges)
                    ?.map((ic) => ic?.extUserId)
                    ?.filter((ui) => ui)
                    ?.map(
                      (ui) =>
                        departments.find(
                          (d) =>
                            `${d?.id}` ===
                            `${
                              users?.find((u) => `${u?.id}` === `${ui}`)
                                ?.departmentId
                            }`
                        )?.name
                    )
                    ?.filter((dn) => dn)
                ),
              ].join(", ")
            }}
          </td>
          <td class="border border-dark p-0 m-0">
            {{ a?.tasks?.length ?? 0 }}
          </td>
          <td class="border border-dark p-0 m-0">
            <div
              :style="`background-color: ${chroma
                .scale(['red', 'yellow', 'green'])(
                  (a?.tasks?.filter((t) => t?.completedDate)?.length ?? 0) /
                    (a?.tasks?.length ?? 1)
                )
                .alpha(0.6)
                .hex()}`"
            >
              <strong>
                {{ a?.tasks?.filter((t) => t?.completedDate)?.length ?? 0 }}
              </strong>
            </div>
          </td>
          <td class="border border-dark p-0 m-0">
            {{ a?.tasks?.reduce((acc, t) => acc + (t?.hours ?? 0), 0.0) ?? 0 }}
          </td>

          <td class="border border-dark p-0 m-0">
            <div>
              <button
                class="btn btn-sm btn-primary px-1 py-0"
                @click="
                  async () => {
                    form = a;
                    dialog = true;
                  }
                "
              >
                <v-icon icon="mdi-pencil" /> Edit
              </button>
            </div>
          </td>
        </tr>
      </table>
    </div>
  </div>

  <!-- WO dialog -->
  <div class="text-center pa-4">
    <v-dialog v-model="dialog" fullscreen>
      <v-card
        max-width="400"
        prepend-icon="mdi-run"
        title="Create/Edit Activity"
      >
        <template v-slot:actions> </template>

        <div>
          <v-container>
            <div class="w-100">
              <div class="d-flex">
                <div class="mx-2">
                  <button
                    class="btn btn-sm btn-primary"
                    @click="() => handleSave()"
                  >
                    <v-icon icon="mdi-content-save" /> Save
                  </button>
                </div>
                <div class="mx-2">
                  <button class="btn btn-sm btn-danger" @click="dialog = false">
                    <v-icon icon="mdi-close" /> Close
                  </button>
                </div>
              </div>
            </div>
            <div class="my-3">
              <strong>Activity Description</strong>
              <input
                placeholder="Description..."
                class="form-control form-control-sm"
                label="Description"
                @blur="
                  (d) => {
                    form.description = d;
                  }
                "
              />
              <strong>PO Type</strong>

              <v-autocomplete
                :items="activityTypes.map((t) => ({ label: t, value: t }))"
                :item-title="(t) => t?.label"
                :modelValue="activityTypes.find((t) => t === form?.type)"
                @update:modelValue="
                  (a) => {
                    form.type = a;
                  }
                "
              ></v-autocomplete>
            </div>

            <div>
              <strong>Job</strong>
            </div>

            <div>
              <v-autocomplete
                :items="jobs?.jobs?.map((j) => ({ label: j?.name, value: j }))"
                :item-title="(j) => j?.label"
                :modelValue="
                  jobs?.jobs?.find(
                    (t) =>
                      `${t?.masterJavaBaseModel?.id}` === `${form?.extJobId}`
                  )
                "
                @update:modelValue="
                  (a) => {
                    form.extJobId = isNaN(
                      parseInt(a?.masterJavaBaseModel?.id ?? '')
                    )
                      ? 0
                      : parseInt(a?.masterJavaBaseModel?.id ?? '');
                  }
                "
              ></v-autocomplete>
              <!-- {{ form?.extJobId }} -->
            </div>

            <!-- Tasks Table -->
            <v-row>
              <v-col cols="12">
                <h5>Tasks</h5>
                <div>
                  <button class="btn btn-primary" @click="() => addTask()">
                    Add Task
                  </button>
                </div>
                <div class="border border-dark">
                  <table
                    class="table table-sm"
                    style="border-collapse: separate"
                  >
                    <thead>
                      <tr>
                        <th
                          class="bg-dark text-light"
                          v-for="h in [
                            '#',
                            'Description',
                            'From',
                            'To',
                            'Done',
                            'Hours',

                            'PIC',
                            'Actions',
                          ]"
                        >
                          {{ h }}
                        </th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="(task, i) in form.tasks" :key="i">
                        <td class="border border-dark">{{ i + 1 }}</td>

                        <td class="border border-dark">
                          <input
                            class="form-control form-control-sm"
                            :value="task?.description"
                            @blur="
                              (e) => {
                                task.description = e.target.value;
                              }
                            "
                          />
                        </td>
                        <td class="border border-dark">
                          <input
                            type="date"
                            class="form-control form-control-sm"
                            :value="task?.from?.split('T')?.[0]"
                            @blur="
                              (e) => {
                                task.from = `${e.target.value}T00:00:00Z`;
                              }
                            "
                          />
                        </td>
                        <td class="border border-dark">
                          <input
                            type="date"
                            class="form-control form-control-sm"
                            :value="task?.to?.split('T')?.[0]"
                            @blur="
                              (e) => {
                                task.to = `${e.target.value}T00:00:00Z`;
                              }
                            "
                          />
                        </td>
                        <td class="border border-dark">
                          <div class="d-flex">
                            <input
                              type="date"
                              class="form-control form-control-sm"
                              :value="task?.completedDate?.split('T')?.[0]"
                              @blur="
                                (e) => {
                                  task.completedDate = `${e.target.value}T00:00:00Z`;
                                }
                              "
                            />
                            <div v-if="task?.completedDate">
                              <button
                                class="btn btn-sm btn-secondary"
                                @click="
                                  () => {
                                    task.completedDate = null;
                                  }
                                "
                              >
                                Undo
                              </button>
                            </div>
                          </div>
                        </td>

                        <td class="border border-dark">
                          <input
                            style="width: 75px"
                            placeholder="Hrs.."
                            class="form-control form-control-sm"
                            :value="task?.hours"
                            @blur="
                              (e) => {
                                if (!isNaN(parseFloat(e.target.value))) {
                                  task.hours = parseFloat(e.target.value);
                                }
                              }
                            "
                          />
                        </td>

                        <td class="border border-dark">
                          <v-autocomplete
                            :items="
                              users.map((t) => ({
                                label: `${t?.name} (${
                                  departments.find(
                                    (d) => `${d?.id}` === `${t?.departmentId}`
                                  )?.name
                                })`,
                                value: t,
                              }))
                            "
                            :item-title="(u) => u?.label"
                            @update:model-value="
                              (u) => {
                                if (!task.inCharges) {
                                  task.inCharges = [];
                                }
                                task.inCharges = [
                                  ...task.inCharges,
                                  { extUserId: u?.id },
                                ];
                              }
                            "
                          ></v-autocomplete>
                          <ol>
                            <li v-for="(ic, i) in task?.inCharges ?? []">
                              <template
                                v-for="d in [
                                  {
                                    user: users.find(
                                      (t) => t?.id === ic?.extUserId
                                    ),
                                  },
                                ]"
                              >
                                <div>
                                  <div>
                                    {{
                                      `${d.user?.name} (${
                                        departments.find(
                                          (dx) =>
                                            `${dx?.id}` ===
                                            `${d?.user?.departmentId}`
                                        )?.name
                                      })`
                                    }}
                                  </div>
                                </div>
                              </template>
                            </li>
                          </ol>
                        </td>
                        <td class="border border-dark">
                          <button
                            class="btn btn-danger btn-sm"
                            @click="removeTask(index)"
                          >
                            Remove
                          </button>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </v-col>
            </v-row>
          </v-container>
        </div>
      </v-card>
    </v-dialog>
  </div>
</template>
