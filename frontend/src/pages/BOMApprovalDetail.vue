<script setup>
import { computed, ref } from "vue";
import {
  fetchBomApproval,
  fetchBomDetail,
  fetchBomLeveledsProtoSimple,
  fetchDepartments,
  fetchItems,
  fetchJobsProtoSimple,
  fetchUsers,
} from "./fetchers";
import { useRoute, useRouter } from "vue-router";
import { intlFormat } from "../helpers";
import { ctx } from "../main";
import { merge } from "chart.js/helpers";

const bomApproval = ref({});
const boms = ref({});
const bomsGenesis = ref({});

const users = ref([]);
const route = useRoute();
const router = useRouter();
const ecn = ref(null);
const departments = ref([]);
const jobs = ref({});
const items = ref([]);
const bomDetail = ref({});
const viewModes = ["Approval", "BOM Detail", "Comparison"];
const viewMode = ref("Approval");

const bom1 = ref({});
const bom2 = ref({});

const fetchUsersData = async () => {
  const d = await fetchUsers();
  console.log("users", d);
  users.value = d;
};

const fetchDepartmentsData = async () => {
  const d = await fetchDepartments();
  departments.value = d;
};

const fetchBomLeveledsProtoSimpleData = async () => {
  const d = await fetchBomLeveledsProtoSimple();

  if (d) {
    boms.value = d;
  }
};
const fetchBomApprovals = async () => {
  const d = await fetchBomLeveledsProtoSimple();

  if (d) {
    boms.value = d;
  }
};

const fetchJobsProtoSimpleData = async () => {
  const d = await fetchJobsProtoSimple();

  if (d) {
    jobs.value = d;
  }
};

const fetchBomApprovalData = async () => {
  const d = await fetchBomApproval({ id: route.params.id });

  if (d) {
    bomApproval.value = d;
  }
};

const fetchBomDetailData = async () => {
  const d = await fetchBomDetail({ id: route.params.id });

  bom1.value = d;

  if (d) {
    bomDetail.value = d;
    // Check genesis
    let bomsGenesisData = null;

    if (d?.bomGenesisReferenceId) {
      bomsGenesisData = await fetchBomLeveledsProtoSimple({
        bomGenesisReferenceId: d?.bomGenesisReferenceId,
      });
    } else {
      bomsGenesisData = await fetchBomLeveledsProtoSimple({
        bomGenesisReferenceId: d?.masterJavaBaseModel?.id,
      });
    }

    bomsGenesis.value = bomsGenesisData;
  }
};

const foundBom = computed(() => {
  return boms.value.bomLeveleds?.find(
    (b) => `${b?.masterJavaBaseModel?.id}` === `${route?.params?.id}`
  );
});

bomApproval.value.extBomLeveledId = route.params.id;

const fetchWarehouseItemsData = async () => {
  const d = await fetchItems();

  if (d) {
    items.value = d;
  }
};

fetchWarehouseItemsData();
fetchBomLeveledsProtoSimpleData();
fetchJobsProtoSimpleData();
fetchDepartmentsData();
fetchUsersData();
fetchJobsProtoSimpleData();
fetchBomApprovalData();
fetchBomDetailData();

const handleSave = async () => {
  try {
    const resp = await fetch(
      `${import.meta.env.VITE_APP_BASE_URL}/bomapprovals`,
      {
        method: "post",
        headers: { "content-type": "application/json" },
        body: JSON.stringify(bomApproval.value),
      }
    );

    router.push("/bomapproval");
  } catch (e) {
    console.error(e);
  }
};

const windowx = window;

const bomsForComparison = computed(() => {
  if ((bomsGenesis?.value?.bomLeveleds.length ?? 0) > 1) {
    return bomsGenesis.value;
  } else {
    return boms.value;
  }
});

const compareItems = (item1, item2) => {
  if (!item2) return "deleted";
  if (!item1) return "new";
  if (item1.qty !== item2.qty) return "changed";
  return "unchanged";
};

const mergedItems = computed(() => {
  const allItems = [
    ...(bom1.value.children || []),
    ...(bom2.value.children || []),
  ];
  const uniqueItems = allItems.reduce((acc, item) => {
    const key = item.extItemId;
    if (!acc[key]) {
      acc[key] = { bom1: null, bom2: null };
    }
    if (bom1.value.children?.find((i) => i.extItemId === item.extItemId)) {
      acc[key].bom1 = item;
    }
    if (bom2.value.children?.find((i) => i.extItemId === item.extItemId)) {
      acc[key].bom2 = item;
    }
    return acc;
  }, {});

  return Object.values(uniqueItems);
});
</script>
<template>
  <div class="m-3">
    <div class="d-flex">
      <div>
        <h4>BOM Approval Detail: {{ foundBom?.name }}</h4>
      </div>
      <div class="mx-3">
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
    </div>
    <div><hr class="border border-dark" /></div>

    <div class="d-flex">
      <div v-for="v in viewModes">
        <button
          :class="`btn btn-sm ${
            viewMode === v ? `btn-primary` : `btn-outline-primary`
          }`"
          @click="
            () => {
              viewMode = v;
            }
          "
        >
          {{ v }}
        </button>
      </div>
    </div>

    <div><hr class="border border-dark" /></div>

    <template class="" v-if="viewMode === 'Approval'">
      <div>
        <button
          class="btn btn-sm btn-primary"
          @click="
            () => {
              if (!ctx.user?.id) {
                windowx.alert('User error');
                return;
              }

              if (
                bomApproval?.pics?.find(
                  (a) => `${a?.extUserId}` === `${ctx?.user?.id}`
                )
              ) {
                windowx.alert('User already exists');
                return;
              }

              if (!windowx.confirm('Approve?')) {
                return;
              }

              if (!bomApproval?.pics) {
                bomApproval.pics = [];
              }

              bomApproval = {
                ...bomApproval,
                pics: [
                  ...bomApproval.pics,
                  {
                    extUserId: ctx?.user?.id,
                    status: 1,
                    approvalDate: new Date().toISOString(),
                  },
                ],
              };
            }
          "
        >
          Approve
        </button>
      </div>

      <div
        class="border border-dark shadow shadow-md overflow-auto"
        style="height: 75vh; resize: vertical"
      >
        <table class="table table-sm" style="border-collapse: separate">
          <tr>
            <th
              v-for="(h, i) in [
                '#',
                'PIC',
                'Edit Status',
                'Status',
                'Remark',
                'Action',
              ]"
              class="bg-dark text-light"
              style="position: sticky; top: 0"
            >
              {{ h }}
            </th>
          </tr>

          <tr v-for="(b, i) in bomApproval?.pics?.filter((p) => !p?.deletedAt)">
            <template
              v-for="d in [
                {
                  foundJobs: jobs?.jobs?.filter((j) =>
                    j.jobBomLeveleds?.find(
                      (bx) =>
                        `${bx?.bomLeveledId}` ===
                        `${b?.masterJavaBaseModel?.id}`
                    )
                  ),
                },
              ]"
            >
              <td class="border border-dark p-0 m-0">{{ i + 1 }}</td>
              <td class="border border-dark p-0 m-0">
                {{
                  users
                    .map((t) => ({
                      label: `${t?.name} (${
                        departments.find(
                          (d) => `${d?.id}` === `${t?.departmentId}`
                        )?.name
                      })`,
                      value: t,
                    }))
                    .find((d) => `${d?.value?.id}` === `${b?.extUserId}`)?.value
                    ?.name
                }}
              </td>
              <td class="border border-dark p-0 m-0">
                <button
                  class="btn btn-sm btn-outline-primary"
                  @click="
                    () => {
                      b.status = 0;
                      b.approvalDate = null;
                    }
                  "
                >
                  Outs
                </button>
                <button
                  class="btn btn-sm btn-outline-primary"
                  @click="
                    () => {
                      b.status = 1;
                      b.approvalDate = new Date().toISOString();
                    }
                  "
                >
                  Approved
                </button>
                <button
                  class="btn btn-sm btn-outline-primary"
                  @click="
                    () => {
                      b.status = 2;
                      b.approvalDate = new Date().toISOString();
                    }
                  "
                >
                  Rejected
                </button>
              </td>
              <td class="border border-dark p-0 m-0">
                <div v-if="!b?.status || i?.status === 0">
                  <strong class="text-dark">Outs</strong>
                </div>
                <div v-if="b?.status === 1">
                  <strong class="text-success">Approved</strong>
                </div>
                <div v-if="b?.status === 2">
                  <strong class="text-danger">Rejected</strong>
                </div>

                <div v-if="b?.approvalDate">
                  {{
                    intlFormat({
                      date: b?.approvalDate,
                      dateStyle: "medium",
                      timeStyle: "short",
                    })
                  }}
                </div>
              </td>

              <td class="border border-dark p-0 m-0">
                <textarea
                  class="form-control form-control-sm"
                  placeholder="Remark..."
                  :value="b?.remark"
                  @blur="
                    (e) => {
                      b.remark = e.target.value;
                    }
                  "
                />
              </td>
              <td class="border border-dark p-0 m-0">
                <button
                  class="btn btn-sm btn-danger"
                  @click="
                    () => {
                      b.deletedAt = new Date().toISOString();
                    }
                  "
                >
                  Delete
                </button>
              </td>
            </template>
          </tr>
        </table>
      </div>
    </template>

    <template v-if="viewMode === 'BOM Detail'">
      <div
        class="border border-dark shadow shadow-md overflow-auto"
        style="height: 75vh; resize: vertical"
      >
        <table class="table table-sm" style="border-collapse: separate">
          <tr>
            <th
              v-for="(h, i) in [
                '#',
                'PN',
                'Part Name',
                'Part Desc',
                'Qty',
                'UM',
              ]"
              class="bg-dark text-light"
              style="position: sticky; top: 0"
            >
              {{ h }}
            </th>
          </tr>
          <tr
            v-for="(c, i) in bomDetail?.children?.filter((c) => c?.extItemId) ??
            []"
          >
            <template
              v-for="d in [
                {
                  foundItem: items?.find(
                    (i) => `${i?.id}` === `${c?.extItemId}`
                  ),
                },
              ]"
            >
              <td class="border border-dark">{{ i + 1 }}</td>
              <td class="border border-dark">{{ d.foundItem?.partNum }}</td>
              <td class="border border-dark">{{ d.foundItem?.partName }}</td>
              <td class="border border-dark">{{ d.foundItem?.partDesc }}</td>
              <td class="border border-dark">{{ c?.qty }}</td>
              <td class="border border-dark">{{ d?.foundItem?.defaultUm }}</td>
            </template>
          </tr>
        </table>
      </div>
    </template>

    <template v-if="viewMode === 'Comparison'">
      <div>
        <div><strong>Comparison</strong></div>
      </div>
      <div><hr class="border border-dark" /></div>
      <div class="d-flex">
        <div class="flex-grow-1">
          <div>BOM 1</div>
          <v-autocomplete
            :items="
              boms?.bomLeveleds?.map((t) => ({ label: t?.name, value: t }))
            "
            placeholder="BOM 1..."
            :item-title="(t) => t?.label"
            :modelValue="
              boms?.bomLeveleds
                ?.map((t) => ({ label: t?.name, value: t }))
                ?.find(
                  (t) =>
                    `${t?.value?.masterJavaBaseModel?.id}` ===
                    `${bom1?.masterJavaBaseModel?.id}`
                )
            "
            @update:modelValue="
              async (a) => {
                const d = await fetchBomDetail({
                  id: a?.masterJavaBaseModel?.id,
                });

                if (d) {
                  bom1 = d;
                }
              }
            "
          ></v-autocomplete>
        </div>
        <div class="flex-grow-1">
          <div>BOM 2</div>

          <v-autocomplete
            :items="
              bomsForComparison?.bomLeveleds?.map((t) => ({
                label: t?.name,
                value: t,
              }))
            "
            placeholder="BOM 2..."
            :item-title="(t) => t?.label"
            :modelValue="
              boms?.bomLeveleds
                ?.map((t) => ({ label: t?.name, value: t }))
                ?.find(
                  (t) =>
                    `${t?.value?.masterJavaBaseModel?.id}` ===
                    `${bom2?.masterJavaBaseModel?.id}`
                )
            "
            @update:modelValue="
              async (a) => {
                const d = await fetchBomDetail({
                  id: a?.masterJavaBaseModel?.id,
                });

                if (d) {
                  bom2 = d;
                }
              }
            "
          ></v-autocomplete>
        </div>
      </div>

      <div>
        <table class="table table-sm" style="border-collapse: separate">
          <tr>
            <th
              v-for="h in [
                '#',
                'Part Num',
                'Part Name',
                'Part Desc',
                'Qty Before',
                'Qty After',
              ]"
              class="bg-dark text-light"
            >
              {{ h }}
            </th>
          </tr>
          <tr
            v-for="(item, index) in mergedItems?.filter(
              (i) => i?.bom1?.extItemId || i?.bom2?.extItemId
            )"
            :key="index"
            :class="{
              'bg-warning': compareItems(item.bom1, item.bom2) === 'new',
              'bg-danger': compareItems(item.bom1, item.bom2) === 'deleted',
            }"
          >
            <template
              v-for="dx in [
                {
                  foundItem: items.find(
                    (i) =>
                      `${i?.id}` ===
                      `${item.bom1?.extItemId || item.bom2?.extItemId}`
                  ),
                },
              ]"
              ><td class="border border-dark">{{ index + 1 }}</td>
              <td class="border border-dark">
                {{ dx.foundItem?.partNum }}
              </td>
              <td class="border border-dark">
                {{ dx.foundItem?.partName }}
              </td>
              <td class="border border-dark">
                {{ dx.foundItem?.partDesc }}
              </td>
              <td class="border border-dark">
                {{
                  item.bom1?.qty
                    ? Intl.NumberFormat("en-US", {
                        minimumFractionDigits: 1,
                        maximumFractionDigits: 1,
                      }).format(item.bom1.qty)
                    : "-"
                }}
              </td>
              <td
                class="border border-dark"
                :class="{
                  'bg-warning':
                    compareItems(item.bom1, item.bom2) === 'changed',
                }"
              >
                {{
                  item.bom2?.qty
                    ? Intl.NumberFormat("en-US", {
                        minimumFractionDigits: 1,
                        maximumFractionDigits: 1,
                      }).format(item.bom2.qty)
                    : "-"
                }}
              </td></template
            >
          </tr>
        </table>
      </div>
    </template>
  </div>
</template>
