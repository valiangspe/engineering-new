<script setup>
import { ref, computed ,watch , onMounted} from "vue";
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

const activityTypes = ref(["PrePO", "PostPO", "Others"]); // Pilihan PO Type
const inquiries = ref([]);
const jobs = ref({ jobs: [] }); // Jika jobs memiliki struktur nested

// State
const dialog = ref(false);

const route = useRoute();
const taskId = route.query.taskId;
// Watch perubahan pada taskId
// watch(
//   () => taskId,
//   (newTaskId) => {
//     if (newTaskId) {
//       startupById(newTaskId); // Panggil fungsi untuk mengambil data berdasarkan taskId
//     }
//   },
//   { immediate: true } // Jalankan langsung saat komponen di-mount
// );


const taskDialog = ref(false); // Mengontrol dialog
const selectedTask = ref(null); // Menyimpan data task yang dipilih
const notificationStore = useNotificationStore(); 

const fetchUsersData = async () => {
  try {
    const d = await fetchUsers();
    console.log("users", d);
    users.value = d;
  } catch (error) {
    console.error("Error fetching users:", error);
  }
};


const formatDate = (date) => {
  if (!date) return "";
  return new Date(date).toLocaleDateString("id-ID", {
    day: "2-digit",
    month: "long",
    year: "numeric",
  });
};

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


const userRole = ref(localStorage.getItem('userRole') || ctx.userRole);


onMounted(async () => {
  await fetchEngineeringActivitiesData();
});

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
    const response = await fetch(
      `${import.meta.env.VITE_APP_BASE_URL}/api/notifications/undo-done?taskId=${task.id}&role=${role}`,
      {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
      }
    );

    if (response.ok) {
      console.log(`Undo done untuk task ID ${task.id} oleh role ${role} berhasil.`);
      loadNotifications();
    } else {
      console.error("Gagal melakukan undo done:", response.status);
    }
  } catch (error) {
    console.error("Error saat melakukan undo done:", error);
  }
};

const activities = ref([]);
const users = ref([]);
const departments = ref([]);
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

// Function untuk memuat data dari API
const startupById = async (taskId) => {
  if (taskId) {
    try {
      dialog.value = true;

      // Lakukan fetch data dari API
      const response = await fetch(`http://localhost:5172/api/dashboard/activities/task/${taskId}?withUserNames=true`);
      if (!response.ok) {
        throw new Error(`Failed to fetch data: ${response.status} ${response.statusText}`);
      }

      // Parsing hasil JSON dari API
      const fetched = await response.json();

      // Assign elemen pertama (task) ke form
      form.value = fetched[0]; // Pastikan mengambil elemen pertama
      console.log("Form data:", form.value); // Debugging untuk memastikan data
    } catch (error) {
      console.error('Error fetching data:', error);
    } finally {
      dialog.value = true;
    }
  }
};


// Load data saat halaman di-mount
onMounted(async () => {
  if (taskId) {
    await startupById(taskId);
  }
});

// Watch perubahan pada taskId untuk menjaga dialog tetap terbuka
watch(() => taskId, async (newTaskId) => {
  if (newTaskId) {
    await startupById(newTaskId);
  }
});


const selectedFilterUser = ref(null);


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
// async function loadActivityByTaskId(taskId) {
//     try {
//         const response = await fetch(`/api/dashboard/activities?taskId=${taskId}`);
//         if (!response.ok) {
//             throw new Error("Gagal memuat aktivitas");
//         }

//         const activityData = await response.json();
//         console.log("Activity data:", activityData);
//         selectedActivity.value = activityData[0]; // Ambil aktivitas pertama jika ada
//     } catch (error) {
//         console.error("Error:", error);
//     }
// }

async function loadTaskById(taskId) {
  try {
    const response = await fetch(`/api/dashboard/tasks/${taskId}`);
    if (!response.ok) {
      throw new Error("Failed to load task data");
    }
    const task = await response.json();
    selectedTask.value = task; // Simpan data task ke reactive state
    taskDialog.value = true; // Tampilkan dialog
  } catch (error) {
    console.error("Error loading task:", error);
  }
}


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
    const response = await fetch(
      `${import.meta.env.VITE_APP_BASE_URL}/api/notifications/done?taskId=${task.id}&role=${role}`,
      {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
      }
    );

    if (response.ok) {
      const updatedNotification = await response.json();
      console.log(`Task ${task.id} selesai untuk role ${role}.`);
      // Update notifikasi
      loadNotifications();
    } else {
      console.error("Gagal memperbarui status task:", response.status);
    }
  } catch (error) {
    console.error("Error saat memperbarui status task:", error);
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
    :style="`background-color: ${(() => {
      const totalTasks = d.filteredTasks?.length ?? 0;
      const doneTasks = d.filteredTasks?.filter((t) => 
        t?.completedDatePic || t?.completedDateSpv || t?.completedDateManager
      )?.length ?? 0;

      if (doneTasks === 0) {
        // Tidak ada yang done, warna merah
        return 'red';
      } else if (d.filteredTasks?.every((t) => 
        t?.completedDatePic && t?.completedDateSpv && t?.completedDateManager
      )) {
        // Semua sudah selesai, warna hijau
        return 'green';
      } else {
        // Sebagian sudah selesai, warna kuning
        return 'yellow';
      }
    })()}`"
  >
    <strong>
      {{
        d.filteredTasks?.filter((t) => 
          t?.completedDatePic && t?.completedDateSpv && t?.completedDateManager
        )?.length ?? 0
      }}
      /
      {{ d.filteredTasks?.length ?? 0 }}
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
  <keep-alive>
  <div class="text-center pa-4">
    <v-dialog :key="taskId" v-model="dialog" fullscreen>
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
              v-model="form.description"
            />
            <strong>PO Type</strong>

            <v-autocomplete
  :items="activityTypes.map((t) => ({ label: t, value: t }))"
  :item-title="(t) => t?.label"
  :modelValue="form?.type"
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
  :items="jobs?.jobs?.map((j) => ({ label: j.name, value: j.id })) || []"
  v-model="form.extJobId"
  :item-title="(item) => item.label"
  :return-object="false"
></v-autocomplete>

            </div>

            <div>
              <strong>Product</strong>
            </div>

            <div>
              <v-autocomplete
  :items="
    jobs?.jobs
      ?.find((j) => j.id === form.extJobId)
      ?.panelCodes?.map((p) => ({
        label: `${p.type} - ${p.name}`,
        value: p.id,
      })) || []
  "
  v-model="form.extPanelCodeId"
  :item-title="(item) => item.label"
></v-autocomplete>

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
                     
               


<!-- <td class="border border-dark">
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
</td> -->
<!-- PIC Section -->
<td class="border border-dark">
  <!-- PIC yang dipilih, manajer, atau spv bisa mengisi done -->
  <div 
    v-if="
      !task.completedDatePic &&
      (
        task.inCharges?.some(ic => ic.extUserId === ctx?.user?.id) && userRoles.includes('pic') || 
        userRoles.some(role => ['spv', 'manager'].includes(role))
      )
    "
  >
    <input
      type="date"
      class="form-control form-control-sm"
      @blur="
        (e) => {
          if (!e.target.value) return;
          task.completedDatePic = `${e.target.value}T00:00:00Z`;
          task.completedByPicId = ctx?.user?.id;
          handleDone(task, 'pic');
        }
      "
    />
  </div>

  <div v-else-if="task.completedDatePic">
    {{ formatDate(task.completedDatePic) }}
    <br />
    Done by: 
    {{ users.find(user => user.id === task.completedByPicId)?.name }} || PIC
    <button
      v-if="userRoles.some(role => ['pic', 'spv', 'manager'].includes(role))"
      class="btn btn-sm btn-secondary mt-2"
      @click="
        () => {
          task.completedDatePic = null;
          task.completedByPicId = null;
          undoDone(task, 'pic');
        }
      "
    >
      Undo
    </button>
  </div>
</td>


<!-- SPV Section -->
<td class="border border-dark">
  <!-- SPV atau manajer bisa mengisi done -->
  <div 
    v-if="
      !task.completedDateSpv &&
      task.completedDatePic &&
      (userRoles.includes('spv') || userRoles.includes('manager'))
    "
  >
    <input
      type="date"
      class="form-control form-control-sm"
      @blur="
        (e) => {
          if (!e.target.value) return;
          task.completedDateSpv = `${e.target.value}T00:00:00Z`;
          task.completedBySpvId = ctx?.user?.id;
          handleDone(task, 'spv');
        }
      "
    />
  </div>

  <!-- Tampilkan data jika sudah done -->
  <div v-else-if="task.completedDateSpv">
    {{ formatDate(task.completedDateSpv) }}
    <br />
    Done by: 
    {{ users.find(user => user.id === task.completedBySpvId)?.name }} || SPV
    <button
      v-if="userRoles.some(role => ['spv', 'manager'].includes(role))"
      class="btn btn-sm btn-secondary mt-2"
      @click="
        () => {
          task.completedDateSpv = null;
          task.completedBySpvId = null;
          undoDone(task, 'spv');
        }
      "
    >
      Undo
    </button>
  </div>
</td>


<!-- Manager Section -->
<td class="border border-dark">
  <div 
    v-if="
      !task.completedDateManager &&
      task.completedDateSpv &&
      userRoles.includes('manager')
    "
  >
    <input
      type="date"
      class="form-control form-control-sm"
      @blur="
        (e) => {
          if (!e.target.value) return;
          task.completedDateManager = `${e.target.value}T00:00:00Z`;
          task.completedByManagerId = ctx?.user?.id;
          handleDone(task, 'manager');
        }
      "
    />
  </div>

  <div v-else-if="task.completedDateManager">
    {{ formatDate(task.completedDateManager) }}
    <br />
    Done by: 
    {{ users.find(user => user.id === task.completedByManagerId)?.name }} || Manager
    <button
      v-if="userRoles.includes('manager')"
      class="btn btn-sm btn-secondary mt-2"
      @click="
        () => {
          task.completedDateManager = null;
          task.completedByManagerId = null;
          undoDone(task, 'manager');
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
  <!-- Autocomplete untuk memilih PIC -->
  <v-autocomplete
    :items="
      users.map((t) => ({
        label: `${t?.name} (${
          departments.find((d) => `${d?.id}` === `${t?.departmentId}`)?.name
        })`,
        value: t,
      }))
    "
    :item-title="(u) => u?.label"
    @update:model-value="
      (u) => {
        if (!u) return;

        if (!task.inCharges) {
          task.inCharges = [];
        }

        // Tambahkan user ke inCharges jika belum ada
        if (!task.inCharges.some((ic) => ic.extUserId === u?.id)) {
          task.inCharges.push({ extUserId: u?.id, username: u?.username });
        }
      }
    "
  ></v-autocomplete>

  <!-- Daftar PIC yang dipilih -->
  <ol>
    <li
      v-for="(ic, i) in task?.inCharges?.filter((ic) => !ic?.deletedAt) ?? []"
      :key="i"
    >
      <template v-for="d in [{ user: users.find((t) => t?.id === ic?.extUserId) }]">
        <div>
          {{
            `${d.user?.name} (${
              departments.find((dx) => `${dx?.id}` === `${d?.user?.departmentId}`)?.name
            })`
          }}
        </div>
        <span>
          <button
            @click="
              () => {
                ic.deletedAt = new Date().toISOString(); // Tandai sebagai dihapus
              }
            "
            class="btn btn-sm btn-outline-danger px-1 py-0"
          >
            Delete
          </button>
        </span>
      </template>
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
</keep-alive>
</template>