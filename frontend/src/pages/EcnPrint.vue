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
  const d = await fetchJobsProtoSimple({ all: true, withProducts: true });

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

const handleSave = async () => {
  try {
    // Fill snapshots

    // window.alert(inventory.value?.length);

    ecn.value = {
      ...ecn.value,
      items: ecn.value.items.map((i, i_) => {
        const foundInv = inventory.value.find(
          (ix) => `${ix?.products?.id}` === `${i?.extItemId}`
        );

        if (
          (foundInv?.priceRp ?? 0) &&
          (!i?.snapshotPrice || i?.snapshotPrice === 0)
        ) {
          i.snapshotPrice = foundInv?.priceRp ?? 0;
          i.snapshotDate = new Date().toISOString();
        }

        return i;
      }),
    };

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

const fetchUsersData = async () => {
  const d = await fetchUsers();
  console.log("users", d);
  users.value = d;
};

const fetchDepartmentsData = async () => {
  const d = await fetchDepartments();
  departments.value = d;
};

fetchUsersData();
fetchDepartmentsData();
fetchPosData();
fetchJobsData();
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
  <div class="container-fluid mt-4">
    <div>
      <div
        class="card-header bg-light border-dark d-flex justify-content-between align-items-center py-1 bg-dark text-light"
      >
        <div class="d-flex align-items-center">
          <!-- <img alt="GSPE Logo" class="me-2" style="height: 30px;"> -->
          <h5 class="mb-0">GSPE</h5>
        </div>
        <h5 class="mb-0">QUALITY FORM</h5>
      </div>
      <div class="card-body border border-dark p-0">
        <form>
          <div class="border-bottom border-dark py-2 px-3">
            <h5 class="mb-0">CHANGE REQUEST</h5>
          </div>

          <!-- Header Information -->
          <div class="d-flex border-bottom border-dark">
            <div class="flex-grow-1 p-2 border-end border-dark">
              <div class="d-flex justify-content-between">
                <span class="fw-bold">Process</span>
                <span>Change Request</span>
              </div>
            </div>
            <div class="flex-grow-1 p-2 border-end border-dark">
              <div class="d-flex justify-content-between">
                <span class="fw-bold">No. Document</span>
                <span>QF-HRD-0096</span>
              </div>
            </div>
            <div class="flex-grow-1 p-2">
              <div class="d-flex justify-content-between">
                <span class="fw-bold">Department</span>
                <span>HRD</span>
              </div>
            </div>
          </div>

          <div class="d-flex border-bottom border-dark">
            <div class="flex-grow-1 p-2 border-end border-dark">
              <div class="d-flex justify-content-between">
                <span class="fw-bold">Effective Date</span>
                <span>{{
                  Intl.DateTimeFormat("en-US", {
                    dateStyle: "full",
                  }).format(new Date(2024, 1, 5))
                }}</span>
              </div>
            </div>
            <div class="flex-grow-1 p-2 border-end border-dark">
              <div class="d-flex justify-content-between">
                <span class="fw-bold">No. Revision</span>
                <span>00</span>
              </div>
            </div>
            <div class="flex-grow-1 p-2">
              <div class="d-flex justify-content-between">
                <span class="fw-bold">Total Page</span>
                <span>1</span>
              </div>
            </div>
          </div>

          <!-- Project Information -->
          <div class="d-flex mt-4 border-bottom border-top border-dark">
            <div class="col-4 p-2 border-end border-dark">
              <div class="d-flex flex-column">
                <span class="fw-bold">Reference Number</span>
                <span
                  ><template
                    v-for="d in [
                      {
                        po: pos.find(
                          (p) => `${p?.id}` === `${ecn?.extPurchaseOrderId}`
                        ),
                        product: jobs?.jobs
                          ?.flatMap((j) => j?.panelCodes)
                          ?.find(
                            (c) =>
                              `${c?.masterJavaBaseModel?.id}` ===
                              `${ecn?.extPanelCodeId}`
                          ),
                      },
                    ]"
                    ><div>
                      <div>
                        {{ d?.po?.purchaseOrderNumber }} ({{
                          d?.po?.account?.name
                        }})
                      </div>
                      <div>
                        Product:
                        {{ d?.product?.type }}: {{ d?.product?.name }}
                      </div>
                    </div></template
                  ></span
                >
              </div>
            </div>
            <div class="col-4 p-2 border-end border-dark">
              <div class="d-flex flex-column">
                <span class="fw-bold">Change Request No</span>
                <span>ECN#{{ ecn?.id }}</span>
              </div>
            </div>
            <div class="col-4 p-2">
              <div class="d-flex flex-column">
                <span class="fw-bold">Date Requested</span>
                <span>{{
                  ecn?.tgl
                    ? Intl.DateTimeFormat("en-US", {
                        dateStyle: "full",
                      }).format(new Date(ecn?.tgl?.split("T")?.[0]))
                    : ""
                }}</span>
              </div>
            </div>
          </div>

          <div class="d-flex border-bottom border-dark">
            <div class="col-3 p-2 border-end border-dark">
              <div class="d-flex flex-column">
                <span class="fw-bold">Project Name</span>
                <span>{{
                  jobs.jobs?.find(
                    (j) =>
                      `${j?.masterJavaBaseModel?.id}` === `${ecn?.extJobId}`
                  )?.name
                }}</span>
              </div>
            </div>

            <div class="col-3 p-2 border-end border-dark">
              <div class="d-flex flex-column">
                <span class="fw-bold">Requested By</span>
                <div class="d-flex justify-content-between">
                  <span></span>
                  <span
                    ><template
                      v-for="d in [
                        {
                          foundUser: users.find(
                            (u) => `${u?.id}` === `${ecn?.extUserId}`
                          ),
                        },
                      ]"
                      >{{ d?.foundUser?.name }} ({{
                        departments?.find(
                          (dx) =>
                            `${dx?.id}` === `${d?.foundUser?.departmentId}`
                        )?.name
                      }})</template
                    ></span
                  >
                </div>
              </div>
            </div>

            <div class="col-3 p-2 border-end border-dark">
              <span class="fw-bold">Margin Before</span>
              <div>{{ ecn?.marginBefore ? `${ecn?.marginBefore}%` : `` }}</div>
            </div>

            <div class="col-3 p-2">
              <span class="fw-bold">Margin After</span>
              <div>{{ ecn?.marginAfter ? `${ecn?.marginAfter}%` : `` }}</div>
            </div>
          </div>

          <!-- Change Request Description -->
          <div class="border-bottom border-dark p-2">
            <div class="fw-bold mb-2">Change Request Description</div>
            <div class="d-flex">
              <div class="form-check me-3">
                <input
                  class="form-check-input"
                  type="checkbox"
                  :checked="ecn?.typeEcnCcn === 0"
                  id="ecnCheck"
                  checked
                />
                <label class="form-check-label" for="ecnCheck">ECN</label>
              </div>
              <div class="form-check me-3">
                <input
                  class="form-check-input"
                  type="checkbox"
                  :checked="ecn?.typeEcnCcn === 1"
                  id="ccnCheck"
                />
                <label class="form-check-label" for="ccnCheck">CCN</label>
              </div>
              <div class="form-check">
                <input
                  class="form-check-input"
                  type="checkbox"
                  id="othersCheck"
                />
                <label class="form-check-label" for="othersCheck"
                  >Others.....</label
                >
              </div>
            </div>
          </div>

          <!-- Reason of Change -->
          <div class="border-bottom border-dark p-2">
            <div class="fw-bold mb-2">Reason Of Change</div>
            <p class="mb-0" style="white-space: word-wrap">
              {{ ecn?.detailProblem }} - {{ ecn?.engineering }}
            </p>
          </div>

          <!-- Business or Technical justification -->
          <div class="border-bottom border-dark p-2">
            <div class="fw-bold mb-2">Business or Technical justification</div>
            <p class="mb-0"></p>
          </div>

          <!-- Priority -->
          <div class="border-bottom border-dark p-2">
            <div class="fw-bold mb-2">Priority</div>
            <div class="d-flex">
              <div class="form-check me-3">
                <input
                  class="form-check-input"
                  type="radio"
                  name="priority"
                  id="topPriority"
                  checked
                />
                <label class="form-check-label" for="topPriority">Top</label>
              </div>
              <div class="form-check me-3">
                <input
                  class="form-check-input"
                  type="radio"
                  name="priority"
                  id="highPriority"
                />
                <label class="form-check-label" for="highPriority">High</label>
              </div>
              <div class="form-check me-3">
                <input
                  class="form-check-input"
                  type="radio"
                  name="priority"
                  id="mediumPriority"
                />
                <label class="form-check-label" for="mediumPriority"
                  >Medium</label
                >
              </div>
              <div class="form-check">
                <input
                  class="form-check-input"
                  type="radio"
                  name="priority"
                  id="lowPriority"
                />
                <label class="form-check-label" for="lowPriority">Low</label>
              </div>
            </div>
          </div>

          <!-- Change Impact Analysis -->
          <div class="border-bottom border-dark p-2">
            <div class="fw-bold mb-2">Change Impact Analysis</div>
            <div class="mb-2">
              <span class="fw-bold">Impact of not making the change :</span>
              <span></span>
            </div>
            <div class="mb-2">
              <span class="fw-bold">Impact on Project risk :</span>
              <span></span>
            </div>
            <div class="mb-2">
              <span class="fw-bold">Impact on Schedule :</span>
              <span>Terhambat</span>
            </div>
            <div>
              <span class="fw-bold">Impact on Project Cost :</span>
              <div class="d-flex align-items-center">
                <span class="me-2">Will Customer Pay?</span>
                <div class="form-check form-check-inline">
                  <input
                    class="form-check-input"
                    type="radio"
                    name="customerPay"
                    id="customerPayNo"
                  />
                  <label class="form-check-label" for="customerPayNo">No</label>
                </div>
                <div class="form-check form-check-inline">
                  <input
                    class="form-check-input"
                    type="radio"
                    name="customerPay"
                    id="customerPayYes"
                  />
                  <label class="form-check-label" for="customerPayYes"
                    >Yes</label
                  >
                </div>
                <span class="ms-2">If Yes, Customer Ref. :</span>
              </div>
            </div>
            <div>
              <span class="fw-bold">Internal Impact : PIC Problem ....</span>
            </div>
          </div>

          <!-- Items Table -->
          <table class="table table-bordered border-dark mb-0">
            <thead>
              <tr>
                <th class="text-center align-middle" rowspan="2">No</th>
                <th class="text-center align-middle" rowspan="2">Mfr</th>
                <th class="text-center align-middle" rowspan="2">P/N</th>
                <th class="text-center align-middle" rowspan="2">
                  Description
                </th>
                <th class="text-center align-middle" rowspan="2">Qty</th>
                <th class="text-center align-middle" rowspan="2">Um</th>
                <th class="text-center align-middle" colspan="2">Time</th>
                <th class="text-center align-middle" colspan="2">Cost</th>
              </tr>
              <tr>
                <th class="text-center">Reduction</th>
                <th class="text-center">Increase</th>
                <th class="text-center">Reduction</th>
                <th class="text-center">Increase</th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="(i, i_) in ecn?.items?.filter((i) => !i?.deletedAt) ??
                []"
              >
                <template
                  v-for="d in [
                    {
                      foundItem: items.find(
                        (ix) => `${i?.extItemId}` === `${ix?.id}`
                      ),
                    },
                  ]"
                >
                  <td>{{ i_ + 1 }}.</td>
                  <td>{{ d.foundItem?.mfr }}</td>
                  <td>{{ d.foundItem?.partNum }}</td>
                  <td>{{ d.foundItem?.partDesc }}</td>
                  <td>{{ i?.qty }}</td>
                  <td>{{ d.foundItem?.defaultUm }}</td>
                  <td></td>
                  <td></td>
                  <td>
                    {{
                      i?.typeIncreaseDecrease === 1
                        ? Intl.NumberFormat("en-US", {
                            maximumFractionDigits: 2,
                            minimumFractionDigits: 2,
                          }).format((i?.qty ?? 0) * (i?.snapshotPrice ?? 0))
                        : ""
                    }}
                  </td>
                  <td>
                    {{
                      !i?.typeIncreaseDecrease || i?.typeIncreaseDecrease === 0
                        ? Intl.NumberFormat("en-US", {
                            maximumFractionDigits: 2,
                            minimumFractionDigits: 2,
                          }).format((i?.qty ?? 0) * (i?.snapshotPrice ?? 0))
                        : ""
                    }}
                  </td>
                </template>
              </tr>
            </tbody>
          </table>

          <!-- Change Request Resolution -->
          <div class="border-bottom border-dark p-2">
            <div class="fw-bold mb-2">Change Request Resolution</div>
            <div class="d-flex align-items-center mb-2">
              <span class="me-2">Change Request Decision</span>
              <div class="form-check form-check-inline">
                <input
                  class="form-check-input"
                  type="radio"
                  name="decision"
                  id="approved"
                />
                <label class="form-check-label" for="approved">Approved</label>
              </div>
              <div class="form-check form-check-inline">
                <input
                  class="form-check-input"
                  type="radio"
                  name="decision"
                  id="hold"
                />
                <label class="form-check-label" for="hold">Hold</label>
              </div>
              <div class="form-check form-check-inline">
                <input
                  class="form-check-input"
                  type="radio"
                  name="decision"
                  id="denied"
                />
                <label class="form-check-label" for="denied">Denied</label>
              </div>
            </div>
            <div class="mb-2">
              <span class="fw-bold">Decision Date</span>
            </div>
            <div>
              <span class="fw-bold">Justification</span>
            </div>
          </div>

          <!-- Decision By -->
          <div class="border-bottom border-dark p-2">
            <div class="fw-bold mb-2">Decision By</div>
            <div class="d-flex">
              <div class="form-check me-3">
                <input
                  class="form-check-input"
                  type="checkbox"
                  value=""
                  id="engineering"
                />
                <label class="form-check-label" for="engineering"
                  >Engineering</label
                >
              </div>
              <div class="form-check me-3">
                <input
                  class="form-check-input"
                  type="checkbox"
                  value=""
                  id="salesPM"
                />
                <label class="form-check-label" for="salesPM">Sales/PM</label>
              </div>
              <div class="form-check me-3">
                <input
                  class="form-check-input"
                  type="checkbox"
                  value=""
                  id="others"
                />
                <label class="form-check-label" for="others">Others</label>
              </div>
              <div class="form-check">
                <input
                  class="form-check-input"
                  type="checkbox"
                  value=""
                  id="bod"
                />
                <label class="form-check-label" for="bod">BOD</label>
              </div>
            </div>
          </div>

          <!-- Signature Section -->
          <div
            class="d-flex border-bottom border-dark text-center"
            style="height: 100px"
          >
            <div class="flex-grow-1 border-end border-dark p-2">
              <div class="fw-bold">Requester</div>
            </div>
            <div class="flex-grow-1 border-end border-dark p-2">
              <div class="fw-bold">Eng. Head</div>
            </div>
            <div class="flex-grow-1 border-end border-dark p-2">
              <div class="fw-bold">Sales Head</div>
            </div>
            <div class="flex-grow-1 border-end border-dark p-2">
              <div class="fw-bold">Operational Head</div>
            </div>
            <div class="flex-grow-1 border-end border-dark p-2">
              <div class="fw-bold">FAT Head</div>
            </div>
            <div class="flex-grow-1 p-2">
              <div class="fw-bold">BOD</div>
            </div>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "ChangeRequestForm",
  // Add any necessary component logic here
};
</script>

<style scoped>
.form-control,
.form-check-input,
.btn {
  border-radius: 0;
}
.card {
  border-radius: 0;
}
</style>
