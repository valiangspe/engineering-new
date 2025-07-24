<script setup>
import { ref, onMounted } from "vue";
import { fetchEngineeringDetailProblem, fetchUsers } from "./fetchers";
import { ctx } from "../main";
import { useRoute, useRouter } from "vue-router";

const route = useRoute();
const router = useRouter();
const ecn = ref(null);
const users = ref([]);
const baseUrl = import.meta.env.VITE_APP_BASE_URL;
const fileBase64 = ref("");
const currentUserId = ref(null);

// State untuk UI/UX
const isLoading = ref(true);
const isSaving = ref(false);

onMounted(async () => {
  try {
    const storedUserId = localStorage.getItem("userId");
    if (storedUserId) {
      currentUserId.value = parseInt(storedUserId, 10);
      if (!ctx.value.userId) {
        ctx.value.userId = currentUserId.value;
      }
    } else {
      alert("Sesi Anda tidak valid. Silakan login kembali.");
      router.push("/");
      return;
    }
    
    // Jalankan pengambilan data secara paralel untuk performa lebih baik
    await Promise.all([
      fetchEngineeringDetailProblemData(),
      fetchUsersData(),
    ]);

  } catch (error) {
    console.error("Gagal memuat halaman approval:", error);
  } finally {
    isLoading.value = false;
  }
});

const fetchUsersData = async () => {
  users.value = await fetchUsers();
};

const fetchEngineeringDetailProblemData = async () => {
  const d = await fetchEngineeringDetailProblem(route.params?.id ?? "");
  if (d) {
    d.approvals = d.approvals || [];
    ecn.value = d;
  }
};

const uploadFile = (event) => {
  const file = event.target.files[0];
  if (file) {
    const reader = new FileReader();
    reader.onload = (e) => {
      fileBase64.value = e.target.result;
      ecn.value.approvalFileName = file.name;
    };
    reader.readAsDataURL(file);
  }
};

const handleApprovalAction = (newStatus) => {
  if (!ecn.value) return;

  const approverUserId = currentUserId.value;
  if (!approverUserId) {
    alert("User tidak terdeteksi, tidak bisa melakukan approval.");
    return;
  }

  const newApproval = {
    extUserId: approverUserId,
    approvalDate: new Date().toISOString(),
    status: newStatus,
  };

  ecn.value.approvals = ecn.value.approvals.filter(app => app.extUserId !== approverUserId);
  ecn.value.approvals.push(newApproval);

  ecn.value.status = newStatus;
  ecn.value.approvalDate = newApproval.approvalDate;
  ecn.value.approvalExtUserId = newApproval.extUserId;
};

const handleSave = async () => {
  if (!ecn.value || isSaving.value) return;

  isSaving.value = true;
  try {
    const resp = await fetch(
      `${baseUrl}/engineeringDetailProblems`,
      {
        method: "post",
        headers: { "content-type": "application/json" },
        body: JSON.stringify(ecn.value),
      }
    );
    const d = await resp.json();

    if (fileBase64.value) {
      await fetch(
        `${baseUrl}/engineeringDetailProblems/${d.id}/photo`,
        {
          method: "post",
          headers: { "content-type": "application/json" },
          body: JSON.stringify({ photo: fileBase64.value }),
        }
      );
    }
    router.push("/ecn");
  } catch (e) {
    console.error(e);
  } finally {
    isSaving.value = false;
  }
};
</script>

<template>
  <div class="container my-4">
    <div v-if="isLoading" class="text-center p-5">
      <div class="spinner-border" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
      <p class="mt-2">Loading Approval Data...</p>
    </div>

    <div v-else-if="ecn">
      <div class="d-flex justify-content-between align-items-center mb-3">
        <h3>Approval for ECN #{{ ecn.id }}</h3>
        <button class="btn btn-primary" @click="handleSave" :disabled="isSaving">
          <span v-if="isSaving" class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
          {{ isSaving ? ' Saving...' : 'Submit Approval' }}
        </button>
      </div>

      <div class="card mb-3">
        <div class="card-header fw-bold">ECN Details</div>
        <div class="card-body">
          <p class="mb-1"><strong>Project:</strong> {{ ecn.projectName || 'N/A' }}</p>
          <p class="mb-1"><strong>PO Number:</strong> {{ ecn.poNumber || 'N/A' }}</p>
          <p class="mb-1"><strong>Customer:</strong> {{ ecn.cust || 'N/A' }}</p>
          <p class="mb-0"><strong>Problem Detail:</strong> {{ ecn.detailProblem || 'N/A' }}</p>
        </div>
      </div>
      
      <div class="card mb-3">
        <div class="card-header fw-bold">Your Action</div>
        <div class="card-body">
          <div class="mb-3">
            <label class="form-label">Choose Action</label>
            <div class="btn-group w-100">
              <button
                :class="['btn', ecn.status === 1 ? 'btn-success' : 'btn-outline-success']"
                @click="handleApprovalAction(1)"
              >
                Approve
              </button>
              <button
                :class="['btn', ecn.status === 2 ? 'btn-danger' : 'btn-outline-danger']"
                @click="handleApprovalAction(2)"
              >
                Reject
              </button>
              <button
                :class="['btn', ecn.status === 0 ? 'btn-secondary' : 'btn-outline-secondary']"
                @click="handleApprovalAction(0)"
              >
                Set to Pending
              </button>
            </div>
          </div>
          <div class="mb-3">
            <label for="remark" class="form-label">Remark</label>
            <textarea
              id="remark"
              :value="ecn.approvalRemark"
              @blur="(e) => (ecn.approvalRemark = e.target.value)"
              class="form-control"
              placeholder="Tambahkan catatan jika perlu..."
            ></textarea>
          </div>
          <div>
            <label for="fileUpload" class="form-label">
              Upload Approval Document
              <a
                v-if="ecn.approvalFileName"
                :href="`${baseUrl}/engineeringDetailProblems/${ecn.id}/photo`"
                target="_blank"
                class="ms-2"
              >
                (View Current: {{ ecn.approvalFileName }})
              </a>
            </label>
            <input id="fileUpload" type="file" @change="uploadFile" class="form-control" />
          </div>
        </div>
      </div>

      <div class="card" v-if="ecn.approvals && ecn.approvals.length > 0">
        <div class="card-header fw-bold">Approval History</div>
        <ul class="list-group list-group-flush">
          <li v-for="app in ecn.approvals.slice().reverse()" :key="app.id" class="list-group-item d-flex justify-content-between align-items-center">
            <div>
              <span
                :class="`badge me-2 ${
                  app.status === 1 ? 'bg-success' : app.status === 2 ? 'bg-danger' : 'bg-dark'
                }`"
              >
                {{ app.status === 1 ? 'Approved' : app.status === 2 ? 'Rejected' : 'Pending' }}
              </span>
              by <strong>{{ users.find(u => u.id === app.extUserId)?.name || 'Unknown User' }}</strong>
            </div>
            <small class="text-muted">{{ new Date(app.approvalDate).toLocaleString('id-ID') }}</small>
          </li>
        </ul>
      </div>
    </div>
    
    <div v-else class="alert alert-danger">
      Failed to load ECN data. Please check the ID and try again.
    </div>
  </div>
</template>