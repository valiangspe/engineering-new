<template>
  <v-container>
    <v-row>
      <v-col>
        <v-card>
          <v-card-title class="headline">Support Others</v-card-title>
          <v-card-text>
            <v-data-table :headers="headers" :items="items" class="elevation-1">
              <template v-slot:item.supportDates="{ item }">
                <td>
                  <v-chip
                    v-for="date in item.supportDates.split(',')"
                    :key="date"
                    text-color="black"
                    class="ma-2"
                  >
                    {{ date }}
                  </v-chip>
                </td>
              </template>
            </v-data-table>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';

const headers = ref([
  { title: 'Engineer', value: 'engineer' },
  { title: 'Project', value: 'project' },
  { title: 'Requester', value: 'requester' },
  { title: 'Support Dates', value: 'supportDates' }
]);

const items = ref([]);

const fetchData = async () => {
  try {
    const response = await axios.get('${import.meta.env.VITE_APP_BASE_URL}/engineerSupports');
    items.value = response.data.map(item => ({
      ...item,
      supportDates: item.supportDates || '' // Ensure supportDates is not null
    }));
  } catch (error) {
    console.error('Error fetching data:', error);
  }
};

onMounted(() => {
  fetchData();
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