<script setup lang="ts">
import { computed } from "@vue/reactivity";
import { ref } from "vue";
import VueMarkdown from "vue-markdown-render";
import { useRoute } from "vue-router";
import { fetchGenerations, fetchInquiries } from "./fetchers";

const route = useRoute();
const loading = ref(false);
const result = ref("");
const selectedModel = ref("gpt4");
const inquiries = ref([] as any[]);
const inquiryAIDocs = ref([] as any[]);

const fetchInquiriesData = async () => {
  const d = await fetchInquiries();
  d.reverse();
  console.log("inquiries", d);
  inquiries.value = d;
};
const fetchGenerationsData = async () => {
  const d = await fetchGenerations({ inquiryId: route.params?.id });
  console.log("gens", d);
  inquiryAIDocs.value = d;
};

const foundinquiry = computed(() => {
  console.log(inquiries.value, route?.params?.id);
  return inquiries.value.find((i) => `${i?.id}` === `${route?.params?.id}`);
});

const handleFileUpload = (e: Event) => {
  const f = (e.target as HTMLInputElement).files?.[0];
  const fr = new FileReader();

  if (f) {
    fr.readAsDataURL(f);

    fr.onload = async (e) => {
      const res = (fr.result as string).split("base64,")?.[1];
      console.log(res);

      if (res) {
        try {
          loading.value = true;

          const resp = await fetch(
            `https://estimator-ai.iotech.my.id/process_pdf`,
            {
              method: "POST",
              headers: { "content-type": "application/json" },
              body: JSON.stringify({
                file: res,
                option_type: 1,
                model: selectedModel.value,
              }),
            }
          );

          const d = await resp.json();

          if (d) {
            result.value = d?.results?.[0];

            const newAiDoc = {
              fileName: f.name,
              content: result.value,
              model: selectedModel.value,
              inquiryId: isNaN(parseInt(route.params?.id ?? ""))
                ? null
                : parseInt(route.params?.id ?? ""),
            };

            if (!result.value || result.value === "") {
              alert("Blank result retrieved.");
              return;
            }

            const resp = await fetch(
              `${import.meta.env.VITE_APP_BASE_URL}/generations`,
              {
                method: "POST",
                headers: { "content-type": "application/json" },
                body: JSON.stringify(newAiDoc),
              }
            );

            fetchGenerationsData();
          }
        } catch (e) {
          console.error(e);
        } finally {
          loading.value = false;
        }
      }
    };

    fr.onerror = (e) => {
      alert("Error upload");
      console.error(e);
    };
  }
};

fetchGenerationsData();
fetchInquiriesData();
</script>
<template>
  <div class="m-3">
    <div>
      <div>
        <h4>
          AI Document Analyzer: Inquiry {{ foundinquiry?.inquiryNumber }} -
          {{ foundinquiry?.title }}
        </h4>
      </div>
    </div>
    <div><hr class="border border-dark" /></div>
    <div class="d-flex">
      <div>
        <button
          :class="`btn btn-sm ${
            selectedModel === 'gpt4' ? `btn-primary` : `btn-outline-primary`
          }`"
          @click="
            () => {
              selectedModel = 'gpt4';
            }
          "
        >
          ChatGPT
        </button>
      </div>

      <div>
        <button
          :class="`btn btn-sm ${
            selectedModel === 'claude' ? `btn-primary` : `btn-outline-primary`
          }`"
          @click="
            () => {
              selectedModel = 'claude';
            }
          "
        >
          Claude
        </button>
      </div>
    </div>
    <div><hr class="border border-dark" /></div>

    <div>
      <input
        type="file"
        @input="
          (e) => {
            handleFileUpload(e);
          }
        "
      />

      <div>
        <div><h4>AI Inq Documents</h4></div>
      </div>
      <div><hr class="border border-dark" /></div>

      <div class="overflow-auto border border-dark">
        <table class="table table-sm" style="border-collapse: separate">
          <thead>
            <tr>
              <th
                class="bg-dark text-light"
                style="position: sticky; top: 0"
                v-for="h in [
                  '#',
                  'ID',
                  'Model',
                  'File Name',
                  'No. of Words',
                  'Created',
                  'Action',
                ]"
              >
                {{ h }}
              </th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(i, i_) in inquiryAIDocs">
              <td class="border border-dark">{{ i_ + 1 }}</td>
              <td class="border border-dark">{{ i?.id }}</td>
              <td class="border border-dark">{{ i?.model }}</td>
              <td class="border border-dark">{{ i?.fileName }}</td>
              <td class="border border-dark">{{ i?.content?.length }}</td>
              <td class="border border-dark">
                {{
                  Intl.DateTimeFormat("en-US", {
                    dateStyle: "medium",
                    timeStyle: "short",
                  }).format(new Date(i?.createdAt ?? ""))
                }}
              </td>

              <td class="border border-dark">
                <div>
                  <button
                    class="btn btn-sm btn-primary"
                    @click="
                      () => {
                        result = i?.content;
                      }
                    "
                  >
                    Preview
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <div><hr class="border border-dark" /></div>

    <div v-if="loading">
      <div class="spinner spinner-border"></div>
      Loading...
    </div>

    <VueMarkdown :source="result" />
  </div>
</template>
