<script setup>
import { ref, computed ,watch , onMounted} from "vue";
import axios from "axios";
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
import ExcelJS from 'exceljs';
import { saveAs } from 'file-saver';


const activityTypes = ref(["PrePO", "PostPO", "Others"]); // Pilihan PO Type
const inquiries = ref([]);
const jobs = ref({ jobs: [] }); // Jika jobs memiliki struktur nested
const selectedDepartment = ref(""); // Departemen yang dipilih dari dropdown
const usersMap = ref({}); // Map untuk menyimpan extUserId -> Department Name
const filteredActivities = ref([]); // Hasil filter activity berdasarkan departemen
const supportTables = ref([]);

const users = ref([]);
const customers = ref([]);
const activities = ref([]);
// State
const dialog = ref(false);

const filteredUsers = ref([]); // Daftar users yang difilter
const route = useRoute();
const taskId = route.query.taskId;

const taskDialog = ref(false); // Mengontrol dialog
const selectedTask = ref(null); // Menyimpan data task yang dipilih
const notificationStore = useNotificationStore(); 
const showOtherDialog = ref(false);
const otherActivity = ref({ name: '', file: null });
const selectedFilterUser = ref(null);

const saveOtherActivity = async () => {
  try {
    let formData = new FormData();
    formData.append('name', otherActivity.value.name);
    formData.append('file', otherActivity.value.file);

    const response = await axios.post(
      `${import.meta.env.VITE_APP_BASE_URL}/supporttables`,
      formData
    );

    if (response.status === 201) {
      form.value.selectedSupportDocId = response.data.id;
      form.value.type = 'Others';
      showOtherDialog.value = false;
      fetchSupportTables(); // Refresh daftar Support Tables
    }
  } catch (error) {
    console.error("Error saving other activity:", error);
  }
};

const fetchUsersData = async () => {
  try {
    const d = await fetchUsers();
    console.log("Fetched users:", d);
    users.value = d;
  } catch (error) {
    console.error("Error fetching users:", error);
  }
};
const fetchSupportTables = async () => {
  try {
    const response = await axios.get(`${import.meta.env.VITE_APP_BASE_URL}/supporttables`);
    supportTables.value = response.data;
  } catch (error) {
    console.error("Error fetching support tables:", error);
  }
};
onMounted(async () => {
  await fetchSupportTables(); 
});

const fetchUsersAndDepartments = async () => {
  try {
    // Fetch daftar departemen
    const deptResponse = await fetch("https://meeting-backend.iotech.my.id/ext-departments");
    const deptData = await deptResponse.json();
    departments.value = [{ id: null, name: "All Departments" }, ...deptData];

    // Fetch daftar user
    const userResponse = await fetch("https://meeting-backend.iotech.my.id/ext-users");
    const userData = await userResponse.json();

    // Simpan user dalam map { extUserId: departmentName }
    usersMap.value = userData.reduce((map, user) => {
      map[user.id] = user.departmentName;
      return map;
    }, {});

  } catch (error) {
    console.error("Error fetching users and departments:", error);
  }
};
watch(selectedDepartment, () => {
  filterActivities();
});

// const filterActivities = () => {
//   if (!selectedDepartment.value || selectedDepartment.value === "All Departments") {
//     filteredActivities.value = activities.value; // Tampilkan semua jika tidak ada filter
//     return;
//   }

//   filteredActivities.value = activities.value.filter(activity => 
//     activity.tasks.some(task => 
//       task.inCharges.some(inCharge => 
//         usersMap.value[inCharge.extUserId] === selectedDepartment.value
//       )
//     )
//   );
// };

const fetchCustomers = async () => {
  try {
    const response = await fetch("https://crm-local.iotech.my.id/api/external/customers");
    if (!response.ok) {
      throw new Error(`Failed to fetch customers: ${response.statusText}`);
    }
    customers.value = await response.json(); // Simpan data pelanggan di state
  } catch (error) {
    console.error("Error fetching customers:", error);
  }
};
onMounted(async () => {
  await fetchCustomers(); // Memuat data pelanggan dari API
  await fetchUsersAndDepartments();
  await fetchEngineeringActivitiesData();
  filterActivities();
});

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
}

// Ambil data role dari `ctx` dan `localStorage`
const userRoles = computed(() => {
  const ctxRole = ctx?.userRole ? [ctx.userRole] : [];
  const localStorageRole = localStorage.getItem("userRole") ? [localStorage.getItem("userRole")] : [];
  return [...new Set([...ctxRole, ...localStorageRole])];
});


const userRole = ref(localStorage.getItem('userRole') || ctx.userRole);


// onMounted(async () => {
//   await fetchEngineeringActivitiesData();
// });
const fetchEngineeringActivitiesData = async () => {
  const d = await fetchEngineeringActivities({
    from: new Date(`${from.value}T00:00:00`).toISOString(),
    to: new Date(`${to.value}T23:59:39`).toISOString(),
    userId: selectedFilterUser.value?.id,
  });
  activities.value = d;
};
watch(selectedDepartment, () => {
  fetchEngineeringActivitiesData();
});

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

const departments = ref([]);
selectedDepartment.value = null;

const fetchDepartmentsData = async () => {
  const d = await fetchDepartments();
  departments.value = [{ id: null, name: "All Departments" }, ...d];
};


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

const displayedActivities = computed(() => {
  if (selectedDepartment.value || selectedFilterUser.value) {
    return filteredActivities.value;
  }
  return activities.value;
});
const filterActivities = () => {
  if (!selectedDepartment.value && !selectedFilterUser.value) {
    filteredActivities.value = activities.value;
    return;
  }

  filteredActivities.value = activities.value.filter(activity => {
    let matchesDepartment = true;
    let matchesUser = true;

    if (selectedDepartment.value && selectedDepartment.value !== "All Departments") {
      matchesDepartment = activity.tasks.some(task =>
        task.inCharges.some(inCharge =>
          usersMap.value[inCharge.extUserId] === selectedDepartment.value
        )
      );
    }

    if (selectedFilterUser.value) {
      matchesUser = activity.tasks.some(task =>
        task.inCharges.some(inCharge =>
          `${inCharge.extUserId}` === `${selectedFilterUser.value}`
        )
      );
    }

    return matchesDepartment && matchesUser;
  });
};

// Pastikan filter bekerja saat ada perubahan pada filter user
watch(selectedFilterUser, () => {
  filterActivities();
});

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

onMounted(async () => {
  await fetchUsersData(); // Ambil data pengguna
  // await fetchEngineeringActivitiesData(); // Ambil data aktivitas
});

// Watch perubahan pada taskId untuk menjaga dialog tetap terbuka
watch(() => taskId, async (newTaskId) => {
  if (newTaskId) {
    await startupById(newTaskId);
  }
});


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

const fetchInquiriesData = async () => {
  const d = await fetchInquiries();
  console.log("inquiries", d);
  inquiries.value = d;
};

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

const exportToExcel = async () => {
  try {
    console.log("Export to Excel function triggered");

    const workbook = new ExcelJS.Workbook();
    const worksheet = workbook.addWorksheet('Activities');

    worksheet.columns = [
      { header: '#', key: 'no', width: 5 },
      { header: 'Type', key: 'type', width: 15 },
      { header: 'Inquiry', key: 'inquiry', width: 20 },
      { header: 'Job', key: 'job', width: 20 },
      { header: 'Customer', key: 'customer', width: 25 },
      { header: 'From', key: 'from', width: 15 },
      { header: 'To', key: 'to', width: 15 },
      { header: 'PIC Depts', key: 'picDepts', width: 30 },
      { header: 'Tasks', key: 'tasks', width: 10 },
      { header: 'Tasks Done', key: 'tasksDone', width: 10 },
      { header: 'TotalHrs', key: 'totalHrs', width: 10 }
    ];

    if (!Array.isArray(displayedActivities.value)) {
      console.error("displayedActivities is not an array:", displayedActivities.value);
      return;
    }

    displayedActivities.value.forEach((a, i) => {
      const filteredTasks = a?.tasks?.filter(t => !t?.deletedAt);

      const picDepts = [...new Set(
        filteredTasks?.flatMap(t => t?.inCharges)
          ?.filter(ic => !ic?.deletedAt)
          ?.map(ic => ic?.extUserId)
          ?.filter(ui => ui)
      )].map(u => {
        const user = users.value.find(ux => `${ux?.id}` === `${u}`);
        const department = Array.isArray(departments.value) 
          ? departments.value.find(dx => `${dx?.id}` === `${user?.departmentId}`)
          : null;

        return `${user?.name} (${department?.name || 'Unknown'})`;
      }).join(', ');

      const doneTasks = filteredTasks?.filter(t => 
        t?.completedDatePic && t?.completedDateSpv && t?.completedDateManager
      ).length ?? 0;

      worksheet.addRow({
        no: i + 1,
        type: a?.type,
        inquiry: `${inquiries?.value?.find(t => `${t?.id}` === `${a?.extInquiryId}`)?.inquiryNumber} - ${inquiries?.value?.find(t => `${t?.id}` === `${a?.extInquiryId}`)?.account?.name || 'Belum Memilih'}`,
        job: jobs?.value?.jobs?.find(j => `${j?.masterJavaBaseModel?.id}` === `${a?.extJobId}`)?.name || 'Belum Memilih',
        customer: a?.customer || 'Belum Memilih',
        from: intlFormat({ date: a?.fromCache?.split("T")?.[0], dateStyle: "medium" }),
        to: intlFormat({ date: a?.toCache?.split("T")?.[0], dateStyle: "medium" }),
        picDepts: picDepts || 'Tidak Ada PIC',
        tasks: filteredTasks?.length || 0,
        tasksDone: doneTasks,
        totalHrs: a?.tasks?.reduce((acc, t) => acc + (t?.hours || 0), 0)
      });
    });

    // Simpan workbook sebagai file Excel
    const buffer = await workbook.xlsx.writeBuffer();
    saveAs(new Blob([buffer]), `Activities_${new Date().toISOString().slice(0, 10)}.xlsx`);
    console.log("Excel file has been generated successfully");
  } catch (error) {
    console.error("Error exporting to Excel:", error);
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

// const handleSave = async () => {
//   try {
//     // Pastikan `customers.value` adalah array sebelum memproses
//     if (!Array.isArray(customers.value)) {
//       console.error("Error: customers is not an array:", customers.value);
//       return;
//     }

//     // Ambil data customer berdasarkan ID yang dipilih
//     const selectedCustomer = customers.value.find(
//       (customer) => customer.id === form.value.selectedCustomerId
//     );

//     const updatedForm = {
//       ...form.value,
//       customerId: selectedCustomer?.id || null, // Pastikan ID customer dikirim
//       customer: selectedCustomer
//         ? `${selectedCustomer.businessType} ${selectedCustomer.name}`
//         : null, // Format nama customer
//       tasks: form.value.tasks.map((task) => ({
//         ...task,
//         completedByPicId: task.completedByPicId || null,
//         completedBySpvId: task.completedBySpvId || null,
//         completedByManagerId: task.completedByManagerId || null,
//       })),
//       selectedSupportDocId:
//         form.value.type === "PrePO" ? form.value.selectedSupportDocId : null, // Tambahkan support doc ID jika PrePO
//     };

//     const resp = await fetch(
//       `${import.meta.env.VITE_APP_BASE_URL}/api/dashboard/activities`,
//       {
//         method: "POST",
//         headers: { "Content-Type": "application/json" },
//         body: JSON.stringify(updatedForm),
//       }
//     );

//     if (resp.ok) {
//       console.log("Activity saved successfully.");

//       // Kirim notifikasi ke backend untuk setiap task baru
//       await Promise.all(
//         updatedForm.tasks.map(async (task) => {
//           await fetch(`${import.meta.env.VITE_APP_BASE_URL}/api/notifications`, {
//             method: "POST",
//             headers: { "Content-Type": "application/json" },
//             body: JSON.stringify({
//               title: `Task Baru: ${task.description}`,
//               message: `Task dengan ID ${task.id} telah dibuat.`,
//               role: "pic", // Awal untuk PIC
//               taskId: task.id,
//               createdAt: new Date().toISOString(),
//             }),
//           });
//         })
//       );

//       console.log("Notifikasi berhasil dibuat.");
//     }

//     dialog.value = false;
//     form.value = { ...defaultForm };

//     fetchEngineeringActivitiesData();
//   } catch (error) {
//     console.error("Error saat menyimpan task:", error);
//   }
// };
const handleSave = async () => {
  try {
    const updatedForm = JSON.parse(JSON.stringify(form.value));

    // Validasi khusus jika tipe "Others"
    // if (form.value.type === "Others") {
    //   if (!form.value.selectedCustomerId) {
    //     alert("Pilih customer terlebih dahulu!");
    //     return;
    //   }

    //   const cust = customers.value.find(c => c.id === form.value.selectedCustomerId);
    //   if (!cust) {
    //     alert("Data customer tidak valid!");
    //     return;
    //   }

    //   updatedForm.customer = `${cust.businessType} ${cust.name}`;
    //   updatedForm.customerId = cust.id;

    //   // Update ke form agar tampilannya konsisten
    //   form.value.customer = updatedForm.customer;
    // }
        // Validasi untuk type "Others"
    if (form.value.type === "Others") {
      // Tidak memaksa memilih customer untuk "Others"
      updatedForm.customer = null; // Tidak mengatur customer jika tidak diperlukan
      updatedForm.customerId = null; // Jangan set customerId
    }

    // Untuk PrePO
    else if (form.value.type === "PrePO") {
      const inquiry = inquiries.value.find(i => `${i.id}` === `${form.value.extInquiryId}`);
      updatedForm.customer = inquiry?.account?.name || null;
      updatedForm.customerId = null;
    }

    // Untuk PostPO
    else if (form.value.type === "PostPO") {
      const po = pos.value.find(p => `${p.id}` === `${form.value.extPurchaseOrderId}`);
      updatedForm.customer = po?.account?.name || null;
      updatedForm.customerId = null;
    }

    // Normalisasi setiap task
    updatedForm.tasks = updatedForm.tasks.map(task => ({
      ...task,
      completedByPicId: task.completedByPicId || null,
      completedBySpvId: task.completedBySpvId || null,
      completedByManagerId: task.completedByManagerId || null,
    }));

    const resp = await fetch(
      `${import.meta.env.VITE_APP_BASE_URL}/api/dashboard/activities`,
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(updatedForm),
      }
    );

    if (!resp.ok) {
      const errorText = await resp.text();
      console.error("Gagal menyimpan:", resp.status, errorText);
      return;
    }

    console.log("Activity saved successfully.");
    dialog.value = false;

    // Reset form
    form.value = { ...defaultForm };
    form.value.selectedCustomerId = null;
    form.value.customer = null;

    await fetchEngineeringActivitiesData();
  } catch (error) {
    console.error("Error saat menyimpan task:", error);
  }
};



const filterUsersByDepartment = () => {
  if (selectedDepartment) {
    filteredUsers.value = users.value.filter(
      (user) => user.departmentId === selectedDepartment
    );
  } else {
    filteredUsers.value = [...users.value]; // Jika tidak ada filter, tampilkan semua users
  }
};
onMounted(() => {
  filteredUsers.value = [...users.value]; // Salin semua users saat pertama kali dimuat
});

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


    <v-container class="px-4 py-2">
    <v-row class="align-center">
      <!-- Bagian Kiri: Input Tanggal & Filter -->
      <v-col cols="12" sm="8" class="d-flex align-center">
        <!-- Input "From" Date -->
        <v-text-field
          label="From"
          type="date"
          v-model="from"
          density="compact"
          variant="outlined"
          hide-details
          class="input-field mr-2"
          @blur="fetchEngineeringActivitiesData"
        ></v-text-field>

        <!-- Input "To" Date -->
        <v-text-field
          label="To"
          type="date"
          v-model="to"
          density="compact"
          variant="outlined"
          hide-details
          class="input-field mr-2"
          @blur="fetchEngineeringActivitiesData"
        ></v-text-field>

        <!-- Filter User -->
        <v-autocomplete
          label="Filter by User"
          :items="users.map(user => ({
            label: `${user?.name} (${departments.find(d => `${d?.id}` === `${user?.departmentId}`)?.name || 'Unknown'})`,
            value: user.id
          }))"
          item-title="label" 
          density="compact"
          variant="outlined"
          v-model="selectedFilterUser"
          hide-details
          clearable
          class="input-field mr-2"
          @update:model-value="filterActivities"
        ></v-autocomplete>


        <!-- Filter Department -->
        <v-autocomplete
          label="Filter by Department"
          :items="departments.map(department => ({
            label: department.name,
            value: department.name // Gunakan nama kembali agar filtering berjalan
          }))"
          item-title="label"
          density="compact"
          variant="outlined"
          v-model="selectedDepartment"
          hide-details
          clearable
          class="input-field mr-2"
          @update:model-value="filterActivities" 
        ></v-autocomplete>

      </v-col>

      <!-- Bagian Kanan: Tombol -->
      <v-col cols="12" sm="4" class="d-flex justify-end">
        <v-btn color="primary" variant="elevated" size="small" class="btn-custom mr-2" @click="dialog = true">
          <v-icon left>mdi-plus</v-icon> Add
        </v-btn>

        <a
          :href="`${getEngineeringActivitiesUrl({
            from: new Date(`${from}T00:00:00`).toISOString(),
            to: new Date(`${to}T23:59:39`).toISOString(),
            excel: true,
            withUserNames: true,
          })}`"
          class="text-decoration-none"
        >
        <v-btn color="success" variant="elevated" size="small" class="btn-custom" @click.prevent="exportToExcel">
          <v-icon left>mdi-download</v-icon> Download
        </v-btn>

        </a>
      </v-col>
    </v-row>
  </v-container>

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
              'Customer', 
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

     
        <tr v-for="(a, i) in displayedActivities">
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
                {{ d?.foundInquiry?.account?.name ||'Belum Memilih'}}</template
              >
            </td>
            <td class="border border-dark p-0 m-0">
              {{
                jobs?.jobs?.find(
                  (j) => `${j?.masterJavaBaseModel?.id}` === `${a?.extJobId}`
                )?.name|| 'Belum Memilih'
              }}
            </td>
            <td class="border border-dark p-0 m-0">
              {{ a?.customer && typeof a.customer === 'object'
                ? `${a.customer?.businessType ?? ''} ${a.customer?.name ?? ''}`
                : a.customer || 'Belum Memilih'
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
    :style="`background-color: ${
      d.filteredTasks?.every(t => t.completedDateManager) ? 'green' :
      d.filteredTasks?.every(t => t.completedDateSpv) ? 'yellow' :
      d.filteredTasks?.every(t => t.completedDatePic) ? 'orange' :
      'red'
    }`"
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

            <!-- <v-autocomplete
              :items="activityTypes.map((t) => ({ label: t, value: t }))"
              :item-title="(t) => t?.label"
              :modelValue="form?.type"
              @update:modelValue="
                (newType) => {
                  // Simpan data customer sebelum mengubah tipe
                  const prevCustomer = form.customer;
                  const prevCustomerId = form.selectedCustomerId;

                  form.type = newType;

                  // Kembalikan customer jika sebelumnya ada
                  if (prevCustomerId && prevCustomer) {
                    form.customer = prevCustomer;
                    form.selectedCustomerId = prevCustomerId;
                  }
                }
              "
            ></v-autocomplete> -->
            <v-autocomplete
              v-model="form.type"
              :items="activityTypes.map((t) => ({ label: t, value: t }))"
              item-title="label"
              item-value="value"
              label="PO Type"
              variant="outlined"
              hide-details
            />





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
          <!-- <template v-if="form.type === 'Others'">
            <div>
              <strong>Select Customer Name</strong>
            </div>
            <div>
              <v-autocomplete
                v-model="form.selectedCustomerId"
                :items="customers.map(c => ({
                  label: `${c.businessType} ${c.name}`,
                  value: c.id
                }))"
                item-title="label"
                item-value="value"
                label="Select a customer or company"
                outlined
                @update:model-value="val => {
                  const selected = customers.find(c => c.id === val);
                  form.customer = selected ? `${selected.businessType} ${selected.name}` : null;
                }"
              />
            </div>
          </template> -->
          <template v-if="form.type === 'Others'">
            <div>
              <strong>Select Customer Name</strong>
            </div>
            <div>
              <v-autocomplete
                v-model="form.selectedCustomerId"
                :items="customers.map(c => ({
                  label: `${c.businessType} ${c.name}`,
                  value: c.id
                }))"
                item-title="label"
                item-value="value"
                label="Select a customer or company"
                outlined
                :disabled="form.type !== 'Others'"  
                @update:model-value="val => {
                  const selected = customers.find(c => c.id === val);
                  form.customer = selected ? `${selected.businessType} ${selected.name}` : null;
                }"
              />
            </div>
          </template>

          
          <template v-if="form.type === 'PrePO'">
            <div>
              <strong>Support Document</strong>
            </div>
            <div>     
              <v-autocomplete
                v-model="form.selectedSupportDocId"
                :items="[
                  ...supportTables.map(doc => ({ label: doc.name, value: doc.id })),
                  // { label: 'Others', value: 'Others' }
                ]"
                item-title="label"
                item-value="value"
                label="Select Support Document"
                outlined
                @update:modelValue="(a) => {
                  console.log('Selected value:', a); // Debugging
                  if (a === 'Others') {
                    showOtherDialog = true;
                  }
                }"
              />
              <!-- <v-autocomplete
                v-model="form.type"
                :items="[...activityTypes.map((t) => ({ label: t, value: t })), { label: 'Others', value: 'Others' }]"
                item-title="label"
                item-value="value"
                label="Select Activity Type"
                outlined
                @update:modelValue="(a) => {
                  if (a === 'Others') {
                    showOtherDialog = true;
                  }
                }"
              ></v-autocomplete> -->
            </div>

            <!-- Modal untuk "Other" -->
            <v-dialog v-model="showOtherDialog" max-width="500px">
              <v-card>
                <v-card-title>Upload Other Activity</v-card-title>
                <v-card-text>
                  <v-text-field v-model="otherActivity.name" label="Activity Name"></v-text-field>
                  <v-file-input label="Upload File" v-model="otherActivity.file"></v-file-input>
                </v-card-text>
                <v-card-actions>
                  <v-btn color="primary" @click="saveOtherActivity">Save</v-btn>
                  <v-btn color="secondary" @click="showOtherDialog = false">Cancel</v-btn>
                </v-card-actions>
              </v-card>
            </v-dialog>
          </template>


          <template v-if="form.type === 'PrePO'">
            <div>
              <strong>Inquiry</strong>
            </div>

            <div>
              <v-autocomplete
                v-model="form.extInquiryId"
                :items="inquiries.map(i => ({
                  label: `${i.inquiryNumber} - ${i.account?.name}`,
                  value: i.id
                }))"
                item-title="label"
                item-value="value"
                label="Inquiry"
                outlined
              />

              <!-- {{ form?.extInquiryId }} -->
            </div>
          </template>

          <template v-if="form.type === 'PostPO'">
            <div>
              <strong>Job</strong>
            </div>

            <div>
               <!-- {{JSON.stringify(form)}} -->
               <v-autocomplete
                  v-model="form.extJobId"
                  :items="jobs.jobs.map(j => ({
                    label: j.name,
                    value: j.masterJavaBaseModel.id
                  }))"
                  item-title="label"
                  item-value="value"
                  label="Job"
                  outlined
                />

              <!-- {{ form?.extJobId }} -->

            </div>

            <div>
              <strong>Product</strong>
            </div>

            <div>
              <!-- {{JSON.stringify(form)}} -->
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

<style scoped>
.input-field {
  font-size: 14px;
  height: 38px; /* Ukuran yang seragam */
  min-width: 160px; /* Supaya input tidak terlalu kecil */
}

.btn-custom {
  min-width: 110px;
  padding: 4px 12px;
  font-size: 14px;
}

.mr-2 {
  margin-right: 8px;
}
</style>