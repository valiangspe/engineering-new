<script setup>
import { computed, ref } from "vue";
import {
  fetchDepartments,
  fetchEngineeringDetailProblem,
  fetchEngineeringDetailProblems,
  fetchExtCrmPurchaseOrdersProtoSimple,
  fetchInventory,
  fetchItems,
  fetchJobsProtoSimple,
  fetchUsers,
} from "./fetchers";
import { useRoute, useRouter } from "vue-router";

const route = useRoute();
const ecn = ref([]);
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
  console.log("users", d);
  users.value = d;
};

const handleSave = async () => {
  try {
    // Fill snapshots

    // window.alert(inventory.value?.length);

    // return;

    const resp = await fetch(
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

const foundItem = computed(() => {
  return items.value.find((i) => `${i?.id}` === `${ecn?.value?.extItemId}`);
});
</script>
<template>
  <div class="my-5 container">
    <div class="d-flex">
      <h4>ECN Detail</h4>
      <div>
        <!-- <a href="/#/ecn/add"> -->
        <button
          class="btn btn-sm btn-primary mx-2 px-1 py-0"
          @click="
            () => {
              handleSave();
            }
          "
        >
          Save
        </button>
        <!-- </a> -->
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
            ecn.extJobId = isNaN(parseInt(a?.masterJavaBaseModel?.id ?? ''))
              ? 0
              : parseInt(a?.masterJavaBaseModel?.id ?? '');
          }
        "
      ></v-autocomplete>
      <!-- {{ ecn.extJobId }} -->
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
              ? 0
              : parseInt(a?.masterJavaBaseModel?.id ?? '');
          }
        "
      ></v-autocomplete>
      <!-- {{ ecn.extJobId }} -->
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
            ecn.extPurchaseOrderId = isNaN(parseInt(a?.id ?? ''))
              ? 0
              : parseInt(a?.id ?? '');
          }
        "
      ></v-autocomplete>
      <!-- {{ ecn?.extPurchaseOrderId }} -->
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

    <div><strong>Problem Detail</strong></div>
    <div class="d-flex">
      <textarea
        class="form-control form-control-sm"
        placeholder="Problem Detail..."
        :value="ecn?.detailProblem"
        @blur="
          (e) => {
            ecn.detailProblem = e.target.value;
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

    <!-- {{ foundItem?.mfr }} {{ foundItem?.partNum }} {{ foundItem?.partDesc }} -->

    <strong>Items</strong>
    <div class="border border-dark">
      <table class="table table-sm">
        <thead>
          <th
            v-for="h in [
              '#',
              'MFR',
              'PN',
              'Part Name',
              'Part Desc',
              'Qty',
              'Get Price',
              'Snapshot price',
              'Snapshot x qty',
              'Last snapshot',
              'Type',
              'UM',
              'Action',
            ]"
          >
            {{ h }}
          </th>
        </thead>
        <tbody>
          <tr v-for="(i, i_) in ecn?.items?.filter((i) => !i.deletedAt) ?? []">
            <template
              v-for="d in [
                { foundItem: items.find((ix) => ix.id === i?.extItemId) },
              ]"
            >
              <td class="border border-dark">{{ i_ + 1 }}</td>
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
                        i.qty = parseFloat(e.target.value);
                      }
                    }
                  "
                  :value="i?.qty"
                />
              </td>
              <td class="border border-dark">
                <button
                  class="btn btn-sm btn-primary px-1 py-0"
                  @click="
                    () => {
                      const foundInv = inventory.find(
                        (ix) => `${ix?.products?.id}` === `${i?.extItemId}`
                      );

                      if (foundInv?.priceRp ?? 0 !== 0) {
                        i.snapshotPrice = foundInv?.priceRp ?? 0;
                        i.snapshotDate = new Date().toISOString();
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
                        i.snapshotPrice = parseFloat(e.target.value);
                      }
                    }
                  "
                  :value="i?.snapshotPrice"
                />
              </td>
              <td class="border border-dark">
                {{
                  Intl.NumberFormat("en-US", {
                    minimumFractionDigits: 2,
                    maximumFractionDigits: 2,
                  }).format((i?.snapshotPrice ?? 0) * (i?.qty ?? 0))
                }}
              </td>
              <td class="border border-dark">
                {{
                  i?.snapshotDate
                    ? Intl.DateTimeFormat("en-US", {
                        dateStyle: "medium",
                        timeStyle: "short",
                      }).format(new Date(i?.snapshotDate))
                    : ""
                }}
              </td>
              <td class="border border-dark">
                <div class="d-flex">
                  <button
                    @click="
                      () => {
                        i.typeIncreaseDecrease = 0;
                      }
                    "
                    :class="`px-1 py-0 btn btn-sm ${
                      i?.typeIncreaseDecrease === 0 || !i?.typeIncreaseDecrease
                        ? `btn-primary`
                        : 'btn-outline-primary'
                    }`"
                  >
                    +
                  </button>
                  <button
                    @click="
                      () => {
                        i.typeIncreaseDecrease = 1;
                      }
                    "
                    :class="`px-1 py-0 btn btn-sm ${
                      i?.typeIncreaseDecrease === 1
                        ? `btn-primary`
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
                    @click="
                      () => {
                        i.deletedAt = new Date().toISOString();
                      }
                    "
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

    <!-- <div><strong>Qty</strong></div>
    <div class="d-flex">
      <input
        class="form-control form-control-sm"
        placeholder="Qty..."
        type="number"
        :value="ecn?.qty"
        @blur="
          (e) => {
            if (!isNaN(parseFloat(e.target.value))) {
              ecn.qty = parseFloat(e.target.value);
            }
          }
        "
      />
    </div> -->

    <div class="d-flex p-2 border border-dark my-3">
      <div>Has PO?</div>
      <div class="mx-2">
        <input
          type="checkbox"
          :value="ecn?.hasPo"
          @click="
            () => {
              ecn.hasPo = !ecn.hasPo;
            }
          "
          :checked="ecn.hasPo ? true : false"
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
        <button class="btn btn-sm btn-primary">Penambahan</button>
      </div>
      <div class="mx-1">
        <button class="btn btn-sm btn-outline-primary">Pengurangan</button>
      </div>
    </div>

    <div><strong>Type (ECN/CCN)</strong></div>
    <div class="d-flex">
      <div class="mx-1">
        <button
          :class="`btn btn-sm ${
            ecn?.typeEcnCcn === 0 ? `btn-primary` : `btn-outline-primary`
          }`"
          @click="
            () => {
              ecn = { ...ecn, typeEcnCcn: 0 };
            }
          "
        >
          ECN
        </button>
      </div>
      <div class="mx-1">
        <button
          :class="`btn btn-sm ${
            ecn?.typeEcnCcn === 1 ? `btn-primary` : `btn-outline-primary`
          }`"
          @click="
            () => {
              ecn = { ...ecn, typeEcnCcn: 1 };
            }
          "
        >
          CCN
        </button>
      </div>
      <div class="mx-1">
        <button
          :class="`btn btn-sm ${
            ecn?.typeEcnCcn === 2 ? `btn-primary` : `btn-outline-primary`
          }`"
          @click="
            () => {
              ecn = { ...ecn, typeEcnCcn: 2 };
            }
          "
        >
          Others
        </button>
      </div>
      <div class="mx-1">
        <button
          :class="`btn btn-sm ${
            ecn?.typeEcnCcn === 3 ? `btn-primary` : `btn-outline-primary`
          }`"
          @click="
            () => {
              ecn = { ...ecn, typeEcnCcn: 3 };
            }
          "
        >
          FAB
        </button>
      </div>
    </div>
  </div>
  <div>
    <v-dialog v-model="dialog">
      <v-card prepend-icon="mdi-run" title="Select Item">
        <template v-slot:actions> </template>

        <div class="m-3">
          <!-- <v-container> -->
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
            <table class="table table-sm" :style="`border-collapse: separate`">
              <thead>
                <tr>
                  <th
                    v-for="h in [
                      '#',
                      'PN',
                      'Part Name',
                      'Part Desc',
                      'UM',
                      'Action',
                    ]"
                    style="position: sticky; top: 0"
                    class="bg-dark text-light"
                  >
                    {{ h }}
                  </th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(i, i_) in itemsFiltered">
                  <td class="border border-dark">{{ i_ + 1 }}</td>
                  <td class="border border-dark">{{ i?.partNum }}</td>
                  <td class="border border-dark">{{ i?.partName }}</td>
                  <td class="border border-dark">{{ i?.partDesc }}</td>
                  <td class="border border-dark">{{ i?.defaultUm }}</td>
                  <td class="border border-dark">
                    <div>
                      <button
                        class="btn btn-sm btn-primary"
                        @click="
                          () => {
                            ecn.extItemId = i?.id;

                            if (!ecn.items) {
                              ecn.items = [];
                            }

                            ecn.items = [...ecn.items, { extItemId: i?.id }];

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
  </div>
</template>
