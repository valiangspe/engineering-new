<script setup>
import { ref } from "vue";
import {
  fetchBomApprovals,
  fetchBomLeveledsProtoSimple,
  fetchJobsProtoSimple,
  fetchUsers,
} from "./fetchers";
import { intlFormat } from "../helpers";

const boms = ref({});
const jobs = ref({});
const users = ref([]);

const bomApprovals = ref({});

const fetchUsersData = async () => {
  const d = await fetchUsers();
  console.log("users", d);
  users.value = d;
};

const fetchBomLeveledsProtoSimpleData = async () => {
  const d = await fetchBomLeveledsProtoSimple();

  if (d) {
    boms.value = d;
  }
};

const fetchBomApprovalsData = async () => {
  const d = await fetchBomApprovals();

  if (d) {
    bomApprovals.value = d;
  }
};

const fetchJobsProtoSimpleData = async () => {
  const d = await fetchJobsProtoSimple();

  if (d) {
    jobs.value = d;
  }
};

fetchBomLeveledsProtoSimpleData();
fetchJobsProtoSimpleData();
fetchBomApprovalsData();
fetchUsersData();
</script>
<template>
  <div class="m-3">
    <div>
      <div><h4>BOM Approval</h4></div>
    </div>
    <div><hr class="border border-dark" /></div>
    <div
      class="border border-dark shadow shadow-md overflow-auto"
      style="height: 75vh; resize: vertical"
    >
      <table class="table table-sm" style="border-collapse: separate">
        <thead>
        <tr>
          <th
            v-for="(h, i) in [
              '#',
              'BOM',
              'Job',
              'Approval',
              'Remark',
              'Action',
              'Created At',
            ]"
            class="bg-dark text-light"
            style="position: sticky; top: 0"
          >
            {{ h }}
          </th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(b, i) in boms?.bomLeveleds">
          <template
            v-for="d in [
              {
                foundJobs: jobs?.jobs?.filter((j) =>
                  j.jobBomLeveleds?.find(
                    (bx) =>
                      `${bx?.bomLeveledId}` === `${b?.masterJavaBaseModel?.id}`
                  )
                ),
                foundApproval: bomApprovals.find(
                  (a) =>
                    `${a?.extBomLeveledId}` === `${b?.masterJavaBaseModel?.id}`
                ),
              },
            ]"
          >
            <td class="border border-dark p-0 m-0">{{ i + 1 }}</td>
            <td class="border border-dark p-0 m-0">{{ b?.name }}</td>
            <td class="border border-dark p-0 m-0">
              <div>
                <ol class="px-2 mx-2 py-0 my-0">
                  <li v-for="(j, i) in d?.foundJobs ?? []">
                    <div>
                      <div>{{ j?.name }}</div>
                    </div>
                  </li>
                </ol>
              </div>
            </td>
            <td
              :class="`border border-dark ${
                d?.foundApproval?.pics?.filter((p) => p?.status === 1)
                  ?.length === d?.foundApproval?.pics?.length &&
                (d?.foundApproval?.pics?.length ?? 0) > 0
                  ? `bg-success`
                  : `bg-danger`
              } p-0 m-0`"
              style="white-space: nowrap"
            >
              <div
                :class="`${
                  d?.foundApproval?.pics?.filter((p) => p?.status === 1)
                    ?.length === d?.foundApproval?.pics?.length &&
                  (d?.foundApproval?.pics?.length ?? 0) > 0
                    ? `bg-success`
                    : `bg-danger`
                }`"
              >
                <ol class="px-2 mx-2 py-0 my-0">
                  <li v-for="(p, i) in d?.foundApproval?.pics ?? []">
                    <div>
                      <div
                        :class="`bg-light my-1 px-1 ${(() => {
                          switch (p?.status) {
                            case 0:
                              return `text-dark`;
                            case 1:
                              return `text-success`;
                            case 2:
                              return `text-danger`;
                            default:
                              return `text-dark`;
                          }
                        })()}`"
                      >
                        <template
                          v-for="d in [
                            {
                              foundUser: users.find(
                                (u) => `${u?.id}` === `${p?.extUserId}`
                              ),
                            },
                          ]"
                        >
                          {{ d.foundUser?.name }}
                        </template>
                      </div>
                    </div>
                  </li>
                </ol>
              </div>
            </td>
            <td class="border border-dark p-0 m-0">
              {{
                d?.foundApproval?.pics
                  ?.map((p) => p?.remark ?? "")
                  .filter((r) => r && r !== "")
                  .join(", ")
              }}
            </td>
            <td class="border border-dark p-0 m-0">
              <div>
                <a :href="`/#/bomapproval/${b?.masterJavaBaseModel?.id}`"
                  ><button class="btn btn-sm btn-primary px-1 py-0">
                    Details
                  </button>
                </a>
              </div>
            </td>

            <td class="border border-dark p-0 m-0" style="white-space: nowrap">
              {{
                intlFormat({
                  date: b?.masterJavaBaseModel?.createdAt,
                  dateStyle: "medium",
                  timeStyle: "short",
                })
              }}
            </td>
          </template>
        </tr>
      </tbody>
      </table>
    </div>
  </div>
</template>
