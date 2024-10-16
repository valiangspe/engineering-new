<script setup>
import { ref } from "vue";
import {
  fetchDepartments,
  fetchEngineeringDetailProblem,
  fetchUsers,
} from "./fetchers";
import { ctx } from "../main";
import { useRoute, useRouter } from "vue-router";

const users = ref([]);
const route = useRoute();
const router = useRouter();
const ecn = ref(null);
const departments = ref([]);

const fetchUsersData = async () => {
  const d = await fetchUsers();
  console.log("users", d);
  users.value = d;
};

const fetchEngineeringDetailProblemData = async () => {
  const d = await fetchEngineeringDetailProblem(route.params?.id ?? "");

  if (d) {
    ecn.value = d;
  }
};

const fetchDepartmentsData = async () => {
  const d = await fetchDepartments();
  departments.value = d;
};
const baseUrl = import.meta.env.VITE_APP_BASE_URL;

fetchDepartmentsData();

fetchEngineeringDetailProblemData();
fetchUsersData();

const fileBase64 = ref("");

const uploadFile = (event) => {
  const file = event.target.files[0];
  if (file) {
    const reader = new FileReader();

    reader.onload = function (e) {
      const base64String = e.target.result;

      ecn.value = { ...ecn.value, approvalFileName: file?.name };

      fileBase64.value = base64String;
    };

    reader.readAsDataURL(file); // Convert the file to Base64
  }
};

const handleSave = async () => {
  try {
    const resp = await fetch(
      `${import.meta.env.VITE_APP_BASE_URL}/engineeringDetailProblems`,
      {
        method: "post",
        headers: { "content-type": "application/json" },
        body: JSON.stringify(ecn.value),
      }
    );

    const d = await resp.json();

    if (fileBase64 != null) {
      const resp = await fetch(
        `${import.meta.env.VITE_APP_BASE_URL}/engineeringDetailProblems/${
          d?.id
        }/photo`,
        {
          method: "post",
          headers: { "content-type": "application/json" },
          body: JSON.stringify({
            photo: fileBase64.value,
          }),
        }
      );
    }

    router.push("/ecn");
  } catch (e) {
    console.error(e);
  }
};
</script>
<template>
  <div class="m-3">
    <div class="d-flex">
      <h4>ECN Approval: {{ route?.params?.id }}</h4>

      <button
        class="btn btn-sm btn-primary"
        @click="
          () => {
            handleSave();
          }
        "
      >
        Save
      </button>
    </div>
    <div><hr /></div>

    <div class="d-flex">
      <button
        class="btn btn-sm btn-outline-primary"
        @click="
          () => {
            ecn.status = 0;
            ecn.approvalDate = null;
          }
        "
      >
        Outs
      </button>
      <button
        class="btn btn-sm btn-outline-primary"
        @click="
          () => {
            ecn.status = 1;
            ecn.approvalDate = new Date().toISOString();
          }
        "
      >
        Approved
      </button>
      <button
        class="btn btn-sm btn-outline-primary"
        @click="
          () => {
            ecn.status = 2;
            ecn.approvalDate = new Date().toISOString();
          }
        "
      >
        Rejected
      </button>
    </div>

    <div>
      Status:
      <div v-if="!ecn?.status || i?.status === 0">
        <strong class="text-dark">Outs</strong>
      </div>
      <div v-if="ecn?.status === 1">
        <strong class="text-success">Approved</strong>
      </div>
      <div v-if="ecn?.status === 2">
        <strong class="text-danger">Rejected</strong>
      </div>
    </div>

    <div>
      Date:
      {{
        ecn?.approvalDate
          ? Intl.DateTimeFormat("en-US", {
              dateStyle: "medium",
              timeStyle: "short",
            }).format(new Date(ecn.approvalDate))
          : ""
      }}
    </div>

    <div><strong> Remark</strong></div>

    <div>
      <textarea
        :value="ecn?.approvalRemark"
        @blur="
          (e) => {
            ecn.approvalRemark = e.target.value;
          }
        "
        class="form-control form-control-sm"
        placeholder="Remark..."
      />
    </div>

    <div>
      <strong>
        Upload Approval Document

        <a
          :href="`${baseUrl}/engineeringDetailProblems/${ecn?.id}/photo`"
          target="_blank"
          >{{
            ecn?.approvalFileName ? `(Filename: ${ecn?.approvalFileName})` : ""
          }}</a
        >
      </strong>
    </div>

    <div>
      <input type="file" @input="(event) => uploadFile(event)" />
    </div>
  </div>
</template>
