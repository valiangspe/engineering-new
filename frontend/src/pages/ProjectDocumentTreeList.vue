<template>
  <v-container class="py-6">
    <v-row>
      <v-col cols="12">
        <v-card>
          <v-card-title class="d-flex flex-wrap align-center justify-space-between gap-4">
            <span class="text-h6">Project Document Tree</span>
            <div class="d-flex align-center gap-3">
              <v-text-field
                v-model="searchQuery"
                density="compact"
                placeholder="Search project or manager"
                hide-details
                clearable
                prepend-inner-icon="mdi-magnify"
                style="min-width: 260px"
              />
              <v-btn
                icon="mdi-refresh"
                variant="text"
                :loading="loading"
                @click="fetchProjects"
                aria-label="Reload projects"
              />
            </div>
          </v-card-title>
          <v-divider />
          <v-card-text>
            <v-alert
              v-if="error"
              type="error"
              variant="tonal"
              class="mb-4"
              density="comfortable"
            >
              {{ error }}
            </v-alert>
            <v-data-table
              :headers="headers"
              :items="filteredItems"
              :items-per-page="10"
              :loading="loading"
              item-key="projectId"
              class="elevation-1"
              hover
              density="comfortable"
            >
              <template #item.projectName="{ item }">
                <span class="font-weight-medium">{{ item.projectName }}</span>
              </template>
              <template #item.actions="{ item }">
                <v-btn
                  color="primary"
                  size="small"
                  @click="goToDetail(item)"
                  variant="flat"
                >
                  View Detail
                </v-btn>
              </template>
              <template #loading>
                <div class="text-center w-100 py-6">
                  <v-progress-circular indeterminate color="primary" />
                </div>
              </template>
              <template #no-data>
                <div class="text-center w-100 py-6">
                  No projects found.
                </div>
              </template>
            </v-data-table>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { useRouter } from "vue-router";

type RawProject = {
  id?: number;
  name?: string;
  projectName?: string;
  leader?: string;
  [key: string]: unknown;
};

type TableItem = {
  order: number;
  projectId: string | number;
  projectName: string;
  projectManager: string;
};

const router = useRouter();
const projects = ref<RawProject[]>([]);
const loading = ref(false);
const error = ref<string | null>(null);
const searchQuery = ref("");

const headers = [
  { title: "#", key: "order", align: "center", sortable: false, width: "72px" },
  { title: "Project Name", key: "projectName" },
  { title: "Project Manager", key: "projectManager" },
  { title: "Action", key: "actions", sortable: false, width: "140px" },
];

const tableItems = computed<TableItem[]>(() =>
  projects.value.map((project, index) => {

    return {
      order: index + 1, 
      projectId: project.id,
      projectName:
        project.name ??
        "Unnamed Project",
      projectManager:
        usersMap.value[project.leader as string] ??
        "-",
    };
  })
);

const filteredItems = computed(() => {
  if (!searchQuery.value.trim()) {
    return tableItems.value;
  }

  const keyword = searchQuery.value.toLowerCase();
  return tableItems.value.filter(
    (item) =>
      item.projectName.toLowerCase().includes(keyword) ||
      item.projectManager.toLowerCase().includes(keyword)
  );
});

const parseProjectsPayload = (payload: unknown): RawProject[] => {
  if (Array.isArray(payload)) {
    return payload;
  }

  if (
    payload &&
    typeof payload === "object" &&
    Array.isArray((payload as { results?: RawProject[] }).results)
  ) {
    return (payload as { results: RawProject[] }).results;
  }

  return [];
};

const fetchProjects = async () => {
  loading.value = true;
  error.value = null;
  try {
    const response = await fetch(
      `${import.meta.env.VITE_APP_MEETING_URL}/projects-idname`
    );

    if (!response.ok) {
      throw new Error(
        `Failed to fetch projects (status ${response.status ?? "unknown"})`
      );
    }

    const payload = await response.json();
    projects.value = parseProjectsPayload(payload);
  } catch (err) {
    error.value =
      err instanceof Error ? err.message : "Unable to load projects.";
    projects.value = [];
  } finally {
    loading.value = false;
  }
};

const goToDetail = (item: TableItem) => {
  if (!item?.projectId) {
    return;
  }

  router.push(`/project-document-detail/${item.projectId}`);
};
const usersMap = ref<{ [key: string]: string }>({}); // Map extUserId to departmentName
const fetchusers =  async () => {
     // Fetch daftar user
    const userResponse = await fetch(`${import.meta.env.VITE_APP_MEETING_URL}/ext-users?showHidden=true`);
    const userData = await userResponse.json();

    // Simpan user dalam map { extUserId: departmentName }
    usersMap.value = userData.reduce((map, user) => {
      map[user.id] = user.name;
      return map;
    }, {});
}

onMounted(async () => {
    await fetchusers();
    await fetchProjects(); 
});
</script>
