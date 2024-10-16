<template>
  <v-container>
    <v-row>
      <v-col>
        <v-card>
          <v-card-title class="headline">Outstanding Post PO</v-card-title>
          <v-card-text>
            <v-data-table
              :headers="headers"
              :items="items"
              class="elevation-1"
              hide-default-footer
            >
              <template v-slot:footer>
                <tr>
                  <td>Grand Total</td>
                  <td>{{ grandTotal.days }}</td>
                  <td>{{ grandTotal.tasks }}</td>
                </tr>
              </template>
            </v-data-table>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup>
import { ref, computed } from "vue";

const headers = ref([
  { title: "Customer", value: "customer" },
  { title: "Done/Outs", value: "doneOuts" },
  { title: "Project", value: "project" },
  { title: "Product", value: "product" },
  { title: "Type", value: "type" },

  { title: "Days (Deadline)", value: "daysDeadline" },
  { title: "Tasks", value: "tasks" },
]);

const items = ref([
  {
    customer: "E Enterprise",
    doneOuts: "Outstanding",
    project: "Subrack",
    product: "E01+CP1",
    type: "Elec",
    daysDeadline: 23,
    tasks: 1,
  },
  {
    customer: "Hitachi Sakti Energy",
    doneOuts: "Outstanding",
    project: "Subrack",
    product: "E01+CP1",
    type: "Mech",
    daysDeadline: 23,
    tasks: 1,
  },
  {
    customer: "Hitachi Sakti Energy",
    doneOuts: "Outstanding",
    product: "E01+CP1",
    type: "Mech",
    project: "Hitachi - DSS Bus Riser Phase I JIL",
    daysDeadline: 25,
    tasks: 1,
  },
  {
    customer: "Hitachi Sakti Energy Indonesia, PT",
    doneOuts: "Outstanding",
    project: "MULTIDRIVE&YFP PERIODE SEPT",
    product: "E01+CP1",
    type: "Mech",
    daysDeadline: 25,
    tasks: 1,
  },
  {
    customer: "Nepa Abadi Persada, PT",
    doneOuts: "Outstanding",
    project: "MCB Yeso",
    product: "E01+CP1",
    type: "Elec",
    daysDeadline: 25,
    tasks: 1,
  },
  {
    customer: "Moni Lestari, PT",
    doneOuts: "Outstanding",
    project: "MCP PANEL",
    product: "E01+CP1",
    type: "Elec",
    daysDeadline: 25,
    tasks: 1,
  },
  {
    customer: "KAARIN TEKNOLOGI PERMATA",
    doneOuts: "Outstanding",
    project: "PSPT",
    product: "E01+CP1",
    type: "Elec",
    daysDeadline: 23,
    tasks: 1,
  },
  {
    customer: "PT BRN Dasatek Indonesia",
    doneOuts: "Outstanding",
    project: "FTT",
    product: "E01+CP1",
    type: "Mech",
    daysDeadline: 23,
    tasks: 1,
  },
  {
    customer: "PT PELINDO",
    doneOuts: "Outstanding",
    project: "MP",
    product: "E01+CP1",
    type: "Elec",
    daysDeadline: 24,
    tasks: 1,
  },
  {
    customer: "Siemens",
    doneOuts: "Outstanding",
    project: "Transformer",
    product: "E01+CP1",
    type: "Elec",
    daysDeadline: 24,
    tasks: 1,
  },
  {
    customer: "Transamas Muliasantai International, PT",
    doneOuts: "Outstanding",
    project: "Visit KOM - CLS Tangerang Pulsk",
    product: "E01+CP1",
    type: "Mech",
    daysDeadline: 24,
    tasks: 1,
  },
  // Add the rest of the data here
]);

const grandTotal = computed(() => {
  return {
    days: items.value.reduce((sum, item) => sum + item.daysDeadline, 0),
    tasks: items.value.reduce((sum, item) => sum + item.tasks, 0),
  };
});
</script>

<style>
.v-data-table {
  margin-bottom: 20px;
}

.v-card {
  margin-bottom: 20px;
}
</style>
