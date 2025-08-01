<script setup>
import { computed, ref } from "vue";
import {
  fetchDepartments,
  fetchEngineeringDetailProblem,
  fetchExtCrmPurchaseOrdersProtoSimple,
  fetchInventory,
  fetchItems,
  fetchJobsProtoSimple,
  fetchUsers,
} from "./fetchers";
import { useRoute, useRouter } from "vue-router";

const route = useRoute();
const ecn = ref({});
const pos = ref([]);
const jobs = ref([]);
const router = useRouter();
const dialog = ref(false);
const items = ref([]);
const users = ref([]);
const departments = ref([]);
const itemSearch = ref("");
const inventory = ref([]);

const fetchEngineeringDetailProblemData = async () => {
  const d = await fetchEngineeringDetailProblem(route.params?.id ?? "");
  if (d) {
    ecn.value = d;
  }
};

const fetchJobsData = async () => {
  const d = await fetchJobsProtoSimple({
    all: true,
    withProducts: true,
    withPurchaseOrders: true,
  });
  if (d) {
    jobs.value = d;
  }
};

const fetchPosData = async () => {
  const d = await fetchExtCrmPurchaseOrdersProtoSimple();
  if (d) {
    pos.value = d;
  }
};

const fetchInventoryData = async () => {
  const d = await fetchInventory();
  if (d) {
    inventory.value = d;
  }
};

const fetchWarehouseItemsData = async () => {
  const d = await fetchItems();
  if (d) {
    items.value = d;
  }
};

const fetchUsersData = async () => {
  const d = await fetchUsers();
  users.value = d;
};

const handleSave = async () => {
  try {
    await fetch(
      `${import.meta.env.VITE_APP_BASE_URL}/engineeringDetailProblems`,
      {
        method: "post",
        headers: { "content-type": "application/json" },
        body: JSON.stringify(ecn.value),
      }
    );
    router.push("/ecn");
  } catch (e) {
    console.error(e);
  }
};

const fetchDepartmentsData = async () => {
  const d = await fetchDepartments();
  departments.value = d;
};

fetchPosData();
fetchDepartmentsData();
fetchJobsData();
fetchUsersData();
fetchInventoryData();
fetchWarehouseItemsData();
if (!isNaN(parseInt(route?.params?.id ?? ""))) {
  fetchEngineeringDetailProblemData();
}

const itemsFiltered = computed(() => {
  return itemSearch.value && itemSearch.value !== ""
    ? items.value?.filter((i) => {
        const itemString =
          `${i?.mfr} ${i?.partNum} ${i?.partName} ${i?.partDesc}`.toLowerCase();
        return itemString
          .split(" ")
          .find((s) => itemSearch.value.toLowerCase().split(" ").includes(s));
      })
    : [];
});
</script>

<template>
  <div class="my-5 container">
    <div class="d-flex">
      <h4>ECN Detail</h4>
      <div>
        <button
          class="btn btn-sm btn-primary mx-2 px-1 py-0"
          @click="handleSave"
        >
          Save
        </button>
      </div>
    </div>
    <div><hr /></div>
    <div><strong>Date</strong></div>
    <div class="d-flex">
      <input
        class="form-control form-control-sm"
        type="date"
        :value="ecn?.tgl?.split('T')?.[0]"
        @blur="
          (e) => {
            ecn.tgl = `${e.target.value}T00:00:00Z`;
          }
        "
      />
    </div>

    <div>
      <strong>Project</strong>
    </div>
    <div>
      <v-autocomplete
        placeholder="Job Name.."
        :items="jobs?.jobs?.map((j) => ({ label: j?.name, value: j }))"
        :item-title="(j) => j?.label"
        :modelValue="
          jobs?.jobs?.find(
            (t) => `${t?.masterJavaBaseModel?.id}` === `${ecn?.extJobId}`
          )
        "
        @update:modelValue="
          (a) => {
            const jobId = a?.masterJavaBaseModel?.id;
            ecn.extJobId = isNaN(parseInt(jobId ?? '')) ? null : parseInt(jobId ?? '');
            ecn.projectName = a?.name ?? '';
          }
        "
      ></v-autocomplete>
    </div>

    <div>
      <strong>Product</strong>
    </div>
    <div>
      <v-autocomplete
        placeholder="Product.."
        :items="
          jobs?.jobs
            ?.find(
              (j) => `${j?.masterJavaBaseModel?.id}` === `${ecn?.extJobId}`
            )
            ?.panelCodes?.map((j) => ({
              label: `${j?.type}: ${j?.name}`,
              value: j,
            }))
        "
        :item-title="(j) => j?.label"
        :modelValue="
          jobs?.jobs
            ?.find(
              (j) => `${j?.masterJavaBaseModel?.id}` === `${ecn?.extJobId}`
            )
            ?.panelCodes?.map((j) => ({
              label: `${j?.type}: ${j?.name}`,
              value: j,
            }))
            ?.find(
              (t) =>
                `${t?.value?.masterJavaBaseModel?.id}` ===
                `${ecn?.extPanelCodeId}`
            )
        "
        @update:modelValue="
          (a) => {
            ecn.extPanelCodeId = isNaN(
              parseInt(a?.masterJavaBaseModel?.id ?? '')
            )
              ? null
              : parseInt(a?.masterJavaBaseModel?.id ?? '');
          }
        "
      ></v-autocomplete>
    </div>

    <div><strong>PO</strong></div>

    <div>
      <v-autocomplete
        placeholder="PO.."
        :items="
          pos
            ?.filter((p) => {
              const foundJob = jobs?.jobs?.find(
                (t) => `${t?.masterJavaBaseModel?.id}` === `${ecn?.extJobId}`
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
            ?.find((t) => `${t?.value?.id}` === `${ecn?.extPurchaseOrderId}`)
        "
        @update:modelValue="
          (a) => {
            const poId = a?.id;
            ecn.extPurchaseOrderId = isNaN(parseInt(poId ?? '')) ? null : parseInt(poId ?? '');
            ecn.poNumber = a?.purchaseOrderNumber ?? '';
            ecn.cust = a?.account?.name ?? '';
          }
        "
      ></v-autocomplete>
    </div>

    <div><strong>Engineering Note</strong></div>
    <div class="d-flex">
      <input
        class="form-control form-control-sm"
        placeholder="Note..."
        :value="ecn?.engineering"
        @blur="
          (e) => {
            ecn.engineering = e.target.value;
          }
        "
      />
    </div>

    <div><strong>Problem Detail / Description</strong></div>
    <div class="d-flex">
      <textarea
        class="form-control form-control-sm"
        placeholder="Problem Detail..."
        :value="ecn?.detailProblem"
        @blur="
          (e) => {
            ecn.detailProblem = e.target.value;
            ecn.description = e.target.value;
          }
        "
      />
    </div>

    <div><strong>GSPE Part Number</strong></div>
    <div class="d-flex">
      <button
        class="btn btn-sm btn-primary"
        @click="
          () => {
            dialog = true;
          }
        "
      >
        Select Item..
      </button>
    </div>

    <strong>Items</strong>
    <div class="border border-dark">
      <table class="table table-sm">
        <thead>
          <th
            v-for="h in [
              '#', 'MFR', 'PN', 'Part Name', 'Part Desc', 'Qty', 'Get Price',
              'Snapshot price', 'Snapshot x qty', 'Last snapshot', 'Type', 'UM', 'Action',
            ]"
          >
            {{ h }}
          </th>
        </thead>
        <tbody>
          <tr v-for="(item, index) in ecn?.items?.filter((i) => !i.deletedAt) ?? []">
            <template
              v-for="d in [
                { foundItem: items.find((ix) => ix.id === item?.extItemId) },
              ]"
            >
              <td class="border border-dark">{{ index + 1 }}</td>
              <td class="border border-dark">{{ d?.foundItem?.mfr }}</td>
              <td class="border border-dark">{{ d?.foundItem?.partNum }}</td>
              <td class="border border-dark">{{ d?.foundItem?.partName }}</td>
              <td class="border border-dark">{{ d?.foundItem?.partDesc }}</td>
              <td class="border border-dark">
                <input
                  type="number"
                  class="form-control form-control-sm"
                  style="width: 75px"
                  @blur="
                    (e) => {
                      if (!isNaN(parseFloat(e.target.value))) {
                        item.qty = parseFloat(e.target.value);
                      }
                    }
                  "
                  :value="item?.qty"
                />
              </td>
              <td class="border border-dark">
                <button
                  class="btn btn-sm btn-primary px-1 py-0"
                  @click="
                    () => {
                      const foundInv = inventory.find(
                        (ix) => `${ix?.products?.id}` === `${item?.extItemId}`
                      );

                      if (foundInv?.priceRp ?? 0 !== 0) {
                        item.snapshotPrice = foundInv?.priceRp ?? 0;
                        item.snapshotDate = new Date().toISOString();
                      }
                    }
                  "
                >
                  Get
                </button>
              </td>
              <td class="border border-dark">
                <input
                  type="number"
                  class="form-control form-control-sm"
                  style="width: 100px"
                  @blur="
                    (e) => {
                      if (!isNaN(parseFloat(e.target.value))) {
                        item.snapshotPrice = parseFloat(e.target.value);
                      }
                    }
                  "
                  :value="item?.snapshotPrice"
                />
              </td>
              <td class="border border-dark">
                {{
                  new Intl.NumberFormat('id-ID').format((item?.snapshotPrice ?? 0) * (item?.qty ?? 0))
                }}
              </td>
              <td class="border border-dark">
                {{
                  item?.snapshotDate
                    ? new Date(item.snapshotDate).toLocaleDateString('id-ID')
                    : ''
                }}
              </td>
              <td class="border border-dark">
                <div class="d-flex">
                  <button
                    @click="item.typeIncreaseDecrease = 0"
                    :class="`px-1 py-0 btn btn-sm ${
                      item?.typeIncreaseDecrease !== 1
                        ? 'btn-primary'
                        : 'btn-outline-primary'
                    }`"
                  >
                    +
                  </button>
                  <button
                    @click="item.typeIncreaseDecrease = 1"
                    :class="`px-1 py-0 btn btn-sm ${
                      item?.typeIncreaseDecrease === 1
                        ? 'btn-primary'
                        : 'btn-outline-primary'
                    }`"
                  >
                    -
                  </button>
                </div>
              </td>
              <td class="border border-dark">{{ d?.foundItem?.defaultUm }}</td>
              <td class="border border-dark">
                <div>
                  <button
                    class="btn btn-sm btn-danger"
                    @click="item.deletedAt = new Date().toISOString()"
                  >
                    Delete
                  </button>
                </div>
              </td>
            </template>
          </tr>
        </tbody>
      </table>
    </div>

    <div class="d-flex p-2 border border-dark my-3">
      <div>Has PO?</div>
      <div class="mx-2">
        <input
          type="checkbox"
          :value="ecn?.hasPo"
          @click="ecn.hasPo = !ecn.hasPo"
          :checked="ecn.hasPo"
        />
      </div>
    </div>

    <div><strong>Requested By</strong></div>
    <div>
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
        :model-value="
          users
            .map((t) => ({
              label: `${t?.name} (${
                departments.find((d) => `${d?.id}` === `${t?.departmentId}`)
                  ?.name
              })`,
              value: t,
            }))
            .find((d) => `${d?.value?.id}` === `${ecn?.extUserId}`)
        "
        @update:model-value="
          (u) => {
            ecn.extUserId = u?.id;
          }
        "
      ></v-autocomplete>
    </div>

    <div><strong>Type (Add/Subtract)</strong></div>
    <div class="d-flex">
      <div class="mx-1">
        <button
          :class="`btn btn-sm ${ecn.type === 0 ? 'btn-primary' : 'btn-outline-primary'}`"
          @click="ecn.type = 0"
        >
          Penambahan
        </button>
      </div>
      <div class="mx-1">
        <button
          :class="`btn btn-sm ${ecn.type === 1 ? 'btn-primary' : 'btn-outline-primary'}`"
          @click="ecn.type = 1"
        >
          Pengurangan
        </button>
      </div>
    </div>

    <div><strong>Type (ECN/CCN)</strong></div>
    <div class="d-flex">
      <div class="mx-1">
        <button
          :class="`btn btn-sm ${
            ecn?.typeEcnCcn === 0 ? `btn-primary` : `btn-outline-primary`
          }`"
          @click="ecn.typeEcnCcn = 0"
        >
          ECN
        </button>
      </div>
      <div class="mx-1">
        <button
          :class="`btn btn-sm ${
            ecn?.typeEcnCcn === 1 ? `btn-primary` : `btn-outline-primary`
          }`"
          @click="ecn.typeEcnCcn = 1"
        >
          CCN
        </button>
      </div>
      <div class="mx-1">
        <button
          :class="`btn btn-sm ${
            ecn?.typeEcnCcn === 2 ? `btn-primary` : `btn-outline-primary`
          }`"
          @click="ecn.typeEcnCcn = 2"
        >
          Others
        </button>
      </div>
      <div class="mx-1">
        <button
          :class="`btn btn-sm ${
            ecn?.typeEcnCcn === 3 ? `btn-primary` : `btn-outline-primary`
          }`"
          @click="ecn.typeEcnCcn = 3"
        >
          FAB
        </button>
      </div>
    </div>
    
    </div>
  
  <v-dialog v-model="dialog">
    <v-card prepend-icon="mdi-run" title="Select Item">
      <div class="m-3">
        <div>
          <input
            class="form-control form-control-sm"
            placeholder="Search by PN, MFR, Part Name, Part Desc.."
            @blur="
              (e) => {
                itemSearch = e.target.value;
              }
            "
          />
        </div>
        <div><hr /></div>
        <div
          class="overflow-auto border border-dark"
          style="height: 65vh; resize: vertical"
        >
          <table class="table table-sm table-hover" :style="`border-collapse: separate`">
            <thead>
              <tr>
                <th
                  v-for="h in ['#', 'PN', 'Part Name', 'Part Desc', 'UM', 'Action']"
                  style="position: sticky; top: 0"
                  class="bg-dark text-light"
                >
                  {{ h }}
                </th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(i, i_) in itemsFiltered">
                <td>{{ i_ + 1 }}</td>
                <td>{{ i?.partNum }}</td>
                <td>{{ i?.partName }}</td>
                <td>{{ i?.partDesc }}</td>
                <td>{{ i?.defaultUm }}</td>
                <td>
                  <div>
                    <button
                      class="btn btn-sm btn-primary"
                      @click="
                        () => {
                          if (!ecn.items) {
                            ecn.items = [];
                          }
                          ecn.items.push({ extItemId: i?.id, qty: 1 });

                          if (ecn.items.length === 1) {
                            ecn.pn = i?.partNum;
                            ecn.qty = 1;
                            ecn.uom = i?.defaultUm;
                          }
                          dialog = false;
                        }
                      "
                    >
                      Select
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </v-card>
  </v-dialog>
</template>