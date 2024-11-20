<script setup>
import { ref, computed , onMounted} from "vue";
// import { ref } from "vue";
import {
  fetchDepartments,
  fetchEngineeringActivities,
  fetchExtCrmPurchaseOrdersProtoSimple,
  fetchInquiries,
  fetchJobsProtoSimple,
  fetchUsers,
  fetchWoTemplates,
  getEngineeringActivitiesUrl,
  // engineTimeProcesses,
} from "./fetchers";
import { intlFormat, makeDateString } from "../helpers";
import chroma from "chroma-js";
import { ctx } from "../main";  // Mengimpor ctx untuk mendapatkan role pengguna
import { useNotificationStore } from "./notificationStore";
import { v4 as uuidv4 } from 'uuid'; // Tambahkan UUID untuk id unik
import { useRoute } from "vue-router";
// import axios from 'axios';

const fetchUsersData = async () => {
  try {
    const d = await fetchUsers();
    console.log("users", d);
    users.value = d;
  } catch (error) {
    console.error("Error fetching users:", error);
  }
};



// import { useRoute } from "vue-router";  // Menambahkan import useRoute

// Metode untuk mengambil engineeringActivityId berdasarkan taskId
const getEngineeringActivityIdByTaskId = (taskId) => {
  for (const activity of activities.value) {
    if (activity.tasks.some(task => task.id === taskId)) {
      return activity.id; // Kembalikan ID dari engineering activity
    }
  }
  console.warn(`Engineering activity dengan task ID ${taskId} tidak ditemukan.`);
  return null; // Kembalikan null jika tidak ditemukan
};
// const handlePanelTypeSelection = (selectedPanelType) => {
//   if (selectedPanelType) {
//     // Tambahkan task baru ke tabel
//     form.value.tasks.push({
//       description: selectedPanelType.processName,
//       hours: selectedPanelType.minutes / 60, // Konversi menit ke jam
//       from: new Date().toISOString(),
//       to: new Date().toISOString(),
//       inCharges: [],
//       remark: "",
//     });
//   }
// };

// const processes = ref([]);

// // Fetch all panel processes
// const fetchProcesses = async () => {
//     try {
//       const response = await axios.get(`${import.meta.env.VITE_APP_BASE_URL}/panelprocess`);
//       processes.value = response.data;
//     } catch (error) {
//       console.error('Error fetching panel processes:', error);
//     }
//   };


// Ambil data role dari `ctx` dan `localStorage`
const userRoles = computed(() => {
  const ctxRole = ctx?.userRole ? [ctx.userRole] : [];
  const localStorageRole = localStorage.getItem("userRole") ? [localStorage.getItem("userRole")] : [];
  return [...new Set([...ctxRole, ...localStorageRole])];
});




// Memuat data aktivitas saat halaman dimuat dan memeriksa apakah ada query taskId
onMounted(() => {
  const taskId = route.query.taskId; // Ambil taskId dari query
  if (taskId) {
    openTaskById(taskId); // Buka task berdasarkan taskId
  }
});



const openTaskById = (taskId) => {
  const foundActivity = activities.value.find((activity) =>
    activity.tasks.some((task) => task.id === taskId)
  );

  if (foundActivity) {
    selectedActivity.value = foundActivity;
    dialog.value = true;
  } else {
    console.warn(`Task dengan ID ${taskId} tidak ditemukan.`);
  }
};


// const openTaskById = (taskId) => {
//   const foundActivity = activities.value.find(activity => 
//     activity.tasks.some(task => task.id === taskId)
//   );
  
//   if (foundActivity) {
//     selectedActivity.value = foundActivity;
//     dialog.value = true;
//   } else {
//     console.warn(`Task dengan ID ${taskId} tidak ditemukan.`);
//   }
// };


// Fungsi untuk memuat data aktivitas
const fetchEngineeringActivitiesData = async () => {
  const d = await fetchEngineeringActivities({
    from: new Date(`${from.value}T00:00:00`).toISOString(),
    to: new Date(`${to.value}T23:59:39`).toISOString(),
    userId: selectedFilterUser.value?.id,
  });
  activities.value = d;
};



const undoDone = async (task, role) => {
  try {
    let previousRole = null;

    if (role === 'spv') {
      previousRole = 'pic';
    } else if (role === 'manager') {
      previousRole = 'spv';
    }

    // Update notification store dengan role sebelumnya atau set ulang ke null
    const notificationStore = useNotificationStore();
    notificationStore.updateNotification(task, previousRole);

    // Simpan perubahan ke backend jika diperlukan
    await fetch(
      `${import.meta.env.VITE_APP_BASE_URL}/api/dashboard/tasks/${task.id}`,
      {
        method: 'put',
        headers: { 'content-type': 'application/json' },
        body: JSON.stringify(task),
      }
    );

    console.log(`Task ID ${task.id} undone for role: ${role}`);
  } catch (error) {
    console.error('Error undoing task done status:', error);
  }
};



const route = useRoute(); // Menangkap route untuk query parameter
const activities = ref([]);
const users = ref([]);
const departments = ref([]);
const dialog = ref(false);
const selectedActivity = ref(null);
const woTemplates = ref(null);
const pos = ref([]);

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
const selectedFilterUser = ref(null);
const inquiries = ref([]);

const loadNotifications = async () => {
  if (!userRole.value) {
    console.warn("User Role belum diambil.");
    return;
  }

  try {
    const response = await fetch(
      `${import.meta.env.VITE_APP_BASE_URL}/api/notifications/active?role=${userRole.value}`
    );
    if (response.ok) {
      const data = await response.json();
      notificationStore.notifications = data;
      console.log("Notifikasi berhasil dimuat:", data);
    } else {
      console.error("Gagal memuat notifikasi:", response.status);
    }
  } catch (error) {
    console.error("Error memuat notifikasi:", error);
  }
};

// onMounted(() => {
//   loadNotifications();
// });


const requiredRule = [(v) => !!v || "Required."];

const menu = ref(false);

const fetchJobsProtoSimpleData = async () => {
  const d = await fetchJobsProtoSimple({
    all: true,
    withProducts: true,
    withPurchaseOrders: true,
  });
  jobs.value = d;
};

const fetchPosData = async () => {
  const d = await fetchExtCrmPurchaseOrdersProtoSimple();

  if (d) {
    pos.value = d;
  }
};

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
  form.value = {
    ...form.value,
    tasks: form.value.tasks.filter((_, i) => i !== index),
  };
};

// const submit = handleSubmit((values) => {
//   console.log("Form submitted:", values);
// });

// const fetchUsersData = async () => {
//   const d = await fetchUsers();
//   console.log("users", d);
//   users.value = d;
// };

const fetchInquiriesData = async () => {
  const d = await fetchInquiries();
  console.log("inquiries", d);
  inquiries.value = d;
};

const fetchDepartmentsData = async () => {
  const d = await fetchDepartments();
  departments.value = d;
};

// const fetchEngineeringActivitiesData = async () => {
//   const d = await fetchEngineeringActivities({
//     from: new Date(`${from.value}T00:00:00`).toISOString(),
//     to: new Date(`${to.value}T23:59:39`).toISOString(),
//     userId: selectedFilterUser.value?.id,
//   });
//   activities.value = d;
// };

const fetchWoTemplatesData = async () => {
  const d = await fetchWoTemplates();
  woTemplates.value = d;
};


const handleDone = async (task, role) => {
  try {
    let nextRole = null;

    if (role === "pic" && task.completedDatePic) {
      nextRole = "spv";
    } else if (role === "spv" && task.completedDateSpv) {
      nextRole = "manager";
    } else if (role === "manager" && task.completedDateManager) {
      nextRole = null;
    }

    // Update notifikasi di backend
    await fetch(
      `${import.meta.env.VITE_APP_BASE_URL}/api/notifications/update-task-status?taskId=${task.id}&role=${role}`,
      {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
      }
    );

    if (nextRole) {
      console.log(`Notifikasi untuk role ${nextRole} berhasil diperbarui.`);
    } else {
      console.log(`Task ${task.id} selesai dan tidak ada role berikutnya.`);
    }

    console.log(`Task ID ${task.id} updated successfully`);
  } catch (error) {
    console.error("Error updating task:", error);
  }
};



// const handleDone = async (task, role) => {
//   try {
//     let nextRole = null;

//     if (role === 'pic' && task.completedDatePic) {
//       nextRole = 'spv';
//     } else if (role === 'spv' && task.completedDateSpv) {
//       nextRole = 'manager';
//     } else if (role === 'manager' && task.completedDateManager) {
//       nextRole = null;
//     }

//     const notificationStore = useNotificationStore();
//     notificationStore.updateNotification(task, nextRole);

//     if (nextRole) {
//       notificationStore.addNotification({
//         id: uuidv4(),
//         title: `Task Update: ${task.description}`,
//         message: `Task dengan ID ${task.id} siap untuk dilakukan "done" oleh ${nextRole.toUpperCase()}.`,
//         role: nextRole,
//         taskId: task.id,
//         createdAt: new Date().toLocaleString('id-ID', {
//           weekday: 'long',
//           year: 'numeric',
//           month: 'long',
//           day: 'numeric',
//           hour: '2-digit',
//           minute: '2-digit',
//           second: '2-digit',
//         }),
//       });
//     }

//     console.log(`Task ID ${task.id} updated successfully`);
//   } catch (error) {
//     console.error('Error updating task:', error);
//   }
// };



const handleSave = async () => {
  try {
    const updatedForm = {
      ...form.value,
      tasks: form.value.tasks.map((task) => ({
        ...task,
        completedByPicId: task.completedByPicId || null,
        completedBySpvId: task.completedBySpvId || null,
        completedByManagerId: task.completedByManagerId || null,
      })),
    };

    const resp = await fetch(
      `${import.meta.env.VITE_APP_BASE_URL}/api/dashboard/activities`,
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(updatedForm),
      }
    );

    if (resp.ok) {
      console.log("Activity saved successfully.");

      // Kirim notifikasi ke backend untuk setiap task baru
      await Promise.all(
        updatedForm.tasks.map(async (task) => {
          await fetch(`${import.meta.env.VITE_APP_BASE_URL}/api/notifications`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
              title: `Task Baru: ${task.description}`,
              message: `Task dengan ID ${task.id} telah dibuat.`,
              role: "pic", // Awal untuk PIC
              taskId: task.id,
              createdAt: new Date().toISOString(),
            }),
          });
        })
      );

      console.log("Notifikasi berhasil dibuat.");
    }

    dialog.value = false;
    form.value = { ...defaultForm };

    fetchEngineeringActivitiesData();
  } catch (error) {
    console.error("Error saat menyimpan task:", error);
  }
};


// const handleSave = async () => {
//   console.log(form.value);
//   try {
//     const updatedTasks = form.value.tasks.map((task) => {
//       let updatedTask = { ...task };

//       // Jangan mengubah status 'done' saat mengedit
//       if (!task.completedDatePic) {
//         updatedTask.completedByPicId = task.completedByPicId || null;
//         updatedTask.completedDatePic = task.completedDatePic || null;
//       }
//       if (!task.completedDateSpv) {
//         updatedTask.completedBySpvId = task.completedBySpvId || null;
//         updatedTask.completedDateSpv = task.completedDateSpv || null;
//       }
//       if (!task.completedDateManager) {
//         updatedTask.completedByManagerId = task.completedByManagerId || null;
//         updatedTask.completedDateManager = task.completedDateManager || null;
//       }

//       return updatedTask;
//     });

//     const updatedForm = {
//       ...form.value,
//       tasks: updatedTasks,
//     };

//     const resp = await fetch(
//       `${import.meta.env.VITE_APP_BASE_URL}/api/dashboard/activities`,
//       {
//         method: "post",
//         headers: { "content-type": "application/json" },
//         body: JSON.stringify(updatedForm),
//       }
//     );

//     if (resp.ok) {
//       // Bagian notifikasi dihapus untuk mencegah pengiriman notifikasi
//       /*
//       const notificationStore = useNotificationStore();
//       updatedTasks.forEach((task) => {
//         let nextRole = !task.completedDatePic ? 'pic'
//                      : !task.completedDateSpv ? 'spv'
//                      : !task.completedDateManager ? 'manager'
//                      : null;

//         // Tambahkan notifikasi untuk role berikutnya
//         if (nextRole) {
//           notificationStore.addNotification({
//             ...task,
//             id: uuidv4(),
//             role: nextRole,
//           });
//         }
//       });
//       */
//     }

//     dialog.value = false;
//     form.value = { ...defaultForm };

//     fetchEngineeringActivitiesData();
//   } catch (e) {
//     console.error("Error saat menyimpan task:", e);
//   }
// };



fetchUsersData();
fetchEngineeringActivitiesData();
fetchDepartmentsData();
fetchJobsProtoSimpleData();
fetchWoTemplatesData();
fetchInquiriesData();
fetchPosData();
loadNotifications();
// fetchProcesses();

const alertx = (content) => {
  alert(content);
};
</script>
<template>
  <div class="m-3">
    <div>
      <div><h4>Activity</h4></div>
    </div>
    <div><hr class="border border-dark" /></div>
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

      <div class="mx-2 d-flex align-items-end">
        <div>
          <button class="btn btn-sm btn-primary" @click="dialog = true">
            <v-icon icon="mdi-plus" /> Add
          </button>
        </div>

        <div>
          <a
            :href="`${getEngineeringActivitiesUrl({
              from: new Date(`${from}T00:00:00`).toISOString(),
              to: new Date(`${to}T23:59:39`).toISOString(),
              excel: true,
              withUserNames: true,
            })}`"
          >
            <button class="btn btn-sm btn-success">
              <v-icon icon="mdi-download" /> Download
            </button>
          </a>
        </div>

        <div class="d-flex align-items-end mx-2">
          <v-autocomplete
            placeholder="Filter user..."
            width="300"
            :items="
              users.map((t) => ({
                label: `${t?.name} (${
                  departments.find((d) => `${d?.id}` === `${t?.departmentId}`)
                    ?.name
                })`,
                value: t,
              }))
            "
            :item-title="(u) => u?.label"
            @update:model-value="
              (u) => {
                selectedFilterUser = u;

                fetchEngineeringActivitiesData();
              }
            "
          ></v-autocomplete>
        </div>
      </div>
    </div>
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
      </thead>
      <tbody>

     
        <tr v-for="(a, i) in activities">
          <template
            v-for="d in [
              { filteredTasks: a?.tasks?.filter((t) => !t?.deletedAt) },
            ]"
          >
            <td class="border border-dark p-0 m-0">{{ i + 1 }}</td>
            <td class="border border-dark p-0 m-0">{{ a?.type }}</td>
            <td class="border border-dark p-0 m-0">
              <template
                v-for="d in [
                  {
                    foundInquiry: inquiries?.find(
                      (t) => `${t?.id}` === `${a?.extInquiryId}`
                    ),
                  },
                ]"
                >{{ d?.foundInquiry?.inquiryNumber }} -
                {{ d?.foundInquiry?.account?.name }}</template
              >
            </td>
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
              <div>
                <ul>
                  <li
                    v-for="u in [
                      ...new Set(
                        d.filteredTasks
                          ?.flatMap((t) => t?.inCharges)
                          ?.filter((ic) => !ic?.deletedAt)
                          ?.map((ic) => ic?.extUserId)
                          ?.filter((ui) => ui)
                      ),
                    ]"
                  >
                    <template
                      v-for="d in [
                        { user: users.find((ux) => `${ux?.id}` === `${u}`) },
                      ]"
                    >
                      {{ d?.user?.name }}
                      ({{
                        departments?.find(
                          (dx) => `${dx?.id}` === `${d?.user?.departmentId}`
                        )?.name
                      }})</template
                    >
                  </li>
                </ul>
              </div>
            </td>
            <td class="border border-dark p-0 m-0">
              {{ d.filteredTasks?.filter((t) => !t?.deletedAt)?.length ?? 0 }}
            </td>
            <td class="border border-dark p-0 m-0">
              <div
                :style="`background-color: ${chroma
                  .scale(['red', 'yellow', 'green'])(
                    (d.filteredTasks?.filter((t) => t?.completedDate)?.length ??
                      0) / (d.filteredTasks?.length ?? 1)
                  )
                  .alpha(0.6)
                  .hex()}`"
              >
                <strong>
                  {{
                    d.filteredTasks?.filter((t) => t?.completedDate)?.length ??
                    0
                  }}
                </strong>
              </div>
            </td>
            <td class="border border-dark p-0 m-0">
              {{
                d.filteredTasks?.reduce(
                  (acc, t) => acc + (t?.hours ?? 0),
                  0.0
                ) ?? 0
              }}
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
          </template>
        </tr>
      </tbody>
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

        <div class="m-3">
          <!-- <v-container> -->
          <div class="w-100">
            <div class="d-flex">
              <div class="mx-2">
                <button class="btn btn-sm btn-primary" @click="() => handleSave()">
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
                (e) => {
                  form.description = e.target.value;
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

            <!-- <div>
  <strong>Panel Type</strong>
  <v-autocomplete
  :items="processes.map((p) => ({ label: `${p.panelType} - ${p.processName}`, value: p }))"
  :item-title="(p) => p.label"
  @update:model-value="(selected) => {
    if (selected) {
      handlePanelTypeSelection(selected.value);
    }
  }"
></v-autocomplete>

</div> -->


          </div>

          <template v-if="form.type === 'PrePO'">
            <div>
              <strong>Inquiry</strong>
            </div>

            <div>
              <v-autocomplete
                :items="
                  inquiries?.map((i) => ({
                    label: `${i?.inquiryNumber} - ${i?.account?.name}`,
                    value: i,
                  }))
                "
                :item-title="(j) => j?.label"
                :modelValue="
                  form?.extInquiryId
                    ? inquiries?.find(
                        (t) => `${t?.id}` === `${form?.extInquiryId}`
                      )
                    : null
                "
                @update:modelValue="
                  (a) => {
                    form.extInquiryId = a?.id;

                    // alertx(a?.id);
                  }
                "
              ></v-autocomplete>
              <!-- {{ form?.extInquiryId }} -->
            </div>
          </template>

          <template v-if="form.type === 'PostPO'">
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

            <div>
              <strong>Product</strong>
            </div>

            <div>
              <v-autocomplete
                :items="
                  jobs?.jobs
                    ?.find(
                      (j) =>
                        `${j?.masterJavaBaseModel?.id}` === `${form?.extJobId}`
                    )
                    ?.panelCodes?.map((j) => ({
                      label: `${j?.type} - ${j?.name}`,
                      value: j,
                    }))
                "
                :item-title="(j) => j?.label"
                :modelValue="
                  jobs?.jobs
                    ?.find(
                      (j) =>
                        `${j?.masterJavaBaseModel?.id}` === `${form?.extJobId}`
                    )
                    ?.panelCodes?.find(
                      (t) =>
                        `${t?.masterJavaBaseModel?.id}` ===
                        `${form?.extPanelCodeId}`
                    )
                "
                @update:modelValue="
                  (a) => {
                    form.extPanelCodeId = isNaN(
                      parseInt(a?.masterJavaBaseModel?.id ?? '')
                    )
                      ? 0
                      : parseInt(a?.masterJavaBaseModel?.id ?? '');
                  }
                "
              ></v-autocomplete>
              <!-- {{ form?.extJobId }} -->
            </div>

            <div>
              <strong>Purchase Order</strong>
            </div>

            <v-autocomplete
              placeholder="PO.."
              :items="
                pos
                  ?.filter((p) => {
                    const foundJob = jobs?.jobs?.find(
                      (t) =>
                        `${t?.masterJavaBaseModel?.id}` === `${form?.extJobId}`
                    );

                    return foundJob?.jobPurchaseOrders?.find(
                      (jp) => `${jp?.extPurchaseOrderId}` === `${p?.id}`
                    );
                  })
                  ?.map((j) => ({
                    label: `${j?.purchaseOrderNumber} (${j?.account?.name})`,
                    value: j,
                  }))
              "
              :item-title="(j) => j?.label"
              :modelValue="
                pos
                  ?.map((j) => ({
                    label: `${j?.purchaseOrderNumber} (${j?.account?.name})`,
                    value: j,
                  }))
                  ?.find(
                    (t) => `${t?.value?.id}` === `${form?.extPurchaseOrderId}`
                  )
              "
              @update:modelValue="
                (a) => {
                  form.extPurchaseOrderId = isNaN(parseInt(a?.id ?? ''))
                    ? 0
                    : parseInt(a?.id ?? '');
                }
              "
            ></v-autocomplete>
          </template>

          <!-- Tasks Table -->
          <v-row>
            <v-col cols="12">
              <h5>Tasks</h5>
              <div class="d-flex">
                <div>
                  <button class="btn btn-primary" @click="() => addTask()">
                    Add Task
                  </button>
                </div>
                <div class="flex-grow-1">
                  <v-autocomplete
                    placeholder="Activity Template..."
                    :items="
                      woTemplates?.templates?.map((t) => ({
                        label: t?.name,
                        value: t,
                      }))
                    "
                    :item-title="(u) => u?.label"
                    @update:model-value="
                      (d) => {
                        form = { ...form, tasks: [] };

                        form = {
                          ...form,
                          tasks: d?.items.map((i) => ({
                            description: i?.name ?? '',
                            hours: (i?.workingMins ?? 0) / 60,
                          })),
                        };
                      }
                    "
                  ></v-autocomplete>
                </div>
              </div>
              <!-- {{ JSON.stringify(form) }} -->
              <div class="border border-dark">
                <table class="table table-sm" style="border-collapse: separate">
                  <thead>
                    <tr>
                      <th
                        class="bg-dark text-light"
                        v-for="h in [
                          '#',
                          'Description',
                          'From',
                          'To',
                          'Done PIC',
                          'Done SPV',

                          'Done Mgr',

                          'Hours',

                          'PIC',
                          'Remark',

                          'Actions',
                        ]"
                      >
                        {{ h }}
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr
                      v-for="(task, i) in form.tasks?.filter(
                        (t) => !t?.deletedAt
                      )"
                      :key="i"
                    >
                      <td class="border border-dark">{{ i + 1 }}</td>

                      <td class="border border-dark">
                        <textarea
                          placeholder="Description..."
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
                     
                 <!-- Done PIC -->

<!-- Done PIC -->
<!-- <td class="border border-dark">
  <div
    v-if="
      !task.completedDatePic &&
      userRoles.includes('pic') &&
      task.inCharges.some(ic => ic.extUserId === ctx.user.id)
    "
  >
    <input
      type="date"
      class="form-control form-control-sm"
      @blur="
        (e) => {
          task.completedDatePic = new Date().toISOString();
          task.completedByPicId = ctx?.user?.id;
          handleDone(task, 'pic'); // Notify and move to the next role
        }
      "
    />
  </div>
  <div v-else-if="task.completedDatePic">
    {{ new Date(task.completedDatePic).toLocaleString('id-ID', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric', hour: '2-digit', minute: '2-digit', second: '2-digit' }) }}
    <br /> Done by: {{ ctx?.user?.username }} || PIC
    <button
      class="btn btn-sm btn-secondary mt-2"
      @click="
        () => {
          task.completedDatePic = null;
          task.completedByPicId = null;
          undoDone(task, 'pic'); // Undo action and update notification
        }
      "
    >
      Undo
    </button>
  </div>
</td> -->

<td class="border border-dark">
  <div v-if="!task.completedDatePic && userRoles.includes('pic')">
    <input
      type="date"
      class="form-control form-control-sm"
      @blur="
        (e) => {
          task.completedDatePic = new Date().toISOString();
          task.completedByPicId = ctx?.user?.id;
          handleDone(task, 'pic'); // Notify and move to the next role
        }
      "
    />
  </div>
  <div v-else-if="task.completedDatePic">
    {{ new Date(task.completedDatePic).toLocaleString('id-ID', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric', hour: '2-digit', minute: '2-digit', second: '2-digit' }) }}
    <br />
    Done by: 
    {{ users.find(user => user.id === task.completedByPicId)?.name }} || PIC
    <button
      class="btn btn-sm btn-secondary mt-2"
      @click="
        () => {
          task.completedDatePic = null;
          task.completedByPicId = null;
          undoDone(task, 'pic'); // Undo action and update notification
        }
      "
    >
      Undo
    </button>
  </div>
</td>
<td class="border border-dark">
  <div v-if="!task.completedDateSpv && task.completedDatePic && userRoles.includes('spv')">
    <input
      type="date"
      class="form-control form-control-sm"
      @blur="
        (e) => {
          task.completedDateSpv = new Date().toISOString();
          task.completedBySpvId = ctx?.user?.id;
          handleDone(task, 'spv'); // Notify and move to the next role
        }
      "
    />
  </div>
  <div v-else-if="task.completedDateSpv">
    {{ new Date(task.completedDateSpv).toLocaleString('id-ID', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric', hour: '2-digit', minute: '2-digit', second: '2-digit' }) }}
    <br />
    Done by: 
    {{ users.find(user => user.id === task.completedBySpvId)?.name }} || SPV
    <button
      class="btn btn-sm btn-secondary mt-2"
      @click="
        () => {
          task.completedDateSpv = null;
          task.completedBySpvId = null;
          undoDone(task, 'spv'); // Undo action and update notification
        }
      "
    >
      Undo
    </button>
  </div>
</td>
<td class="border border-dark">
  <div v-if="!task.completedDateManager && task.completedDateSpv && userRoles.includes('manager')">
    <input
      type="date"
      class="form-control form-control-sm"
      @blur="
        (e) => {
          task.completedDateManager = new Date().toISOString();
          task.completedByManagerId = ctx?.user?.id;
          handleDone(task, 'manager'); // Notify task completion
        }
      "
    />
  </div>
  <div v-else-if="task.completedDateManager">
    {{ new Date(task.completedDateManager).toLocaleString('id-ID', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric', hour: '2-digit', minute: '2-digit', second: '2-digit' }) }}
    <br />
    Done by: 
    {{ users.find(user => user.id === task.completedByManagerId)?.name }} || Manager
    <button
      class="btn btn-sm btn-secondary mt-2"
      @click="
        () => {
          task.completedDateManager = null;
          task.completedByManagerId = null;
          undoDone(task, 'manager'); // Undo action and update notification
        }
      "
    >
      Undo
    </button>
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
                          <li
                            v-for="(ic, i) in task?.inCharges?.filter(
                              (ic) => !ic?.deletedAt
                            ) ?? []"
                          >
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
                                <span
                                  ><button
                                    @click="
                                      () => {
                                        ic.deletedAt = new Date().toISOString();
                                      }
                                    "
                                    class="btn btn-sm btn-outline-danger px-1 py-0"
                                  >
                                    Delete
                                  </button></span
                                >
                              </div></template
                            >
                          </li>
                        </ol>
                      </td>
                      <td class="border border-dark">
                        <textarea
                          placeholder="Remark..."
                          class="form-control form-control-sm"
                          :value="task?.remark"
                          @blur="
                            (e) => {
                              task.remark = e.target.value;
                            }
                          "
                        />
                      </td>
                      <td class="border border-dark">
                        <button
                          class="btn btn-danger btn-sm"
                          @click="
                            () => {
                              task.deletedAt = new Date().toISOString();
                            }
                          "
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
          <!-- </v-container> -->
        </div>
      </v-card>
    </v-dialog>
  </div>
</template>