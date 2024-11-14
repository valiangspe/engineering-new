<script setup lang="ts">
import { computed } from "@vue/reactivity";
import { ref } from "vue";
import VueMarkdown from "vue-markdown-render";
import { useRoute } from "vue-router";
import { fetchInquiries } from "./fetchers";

const route = useRoute();
const loading = ref(false);
const result = ref("");
const selectedModel = ref("gpt4");
const inquiries = ref([] as any[]);

const fetchInquiriesData = async () => {
  const d = await fetchInquiries();
  d.reverse();
  console.log("inquiries", d);
  inquiries.value = d;
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


      <div
        class="overflow-auto border border-dark"
      >
        <table class="table table-sm" style="border-collapse: separate">
          <tr>
            <th
              class="bg-dark text-light"
              style="position: sticky; top: 0"
              v-for="h in ['#', 'ID', 'Inq Number', 'Title', 'Action']"
            >
              {{ h }}
            </th>
          </tr>

          <tr v-for="(i, i_) in []">
            <td class="border border-dark">{{ i_ + 1 }}</td>
            <td class="border border-dark">{{ i?.id }}</td>
            <td class="border border-dark">
              <div><button class="btn btn-sm btn-primary">Preview</button></div>
            </td>
          </tr>
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
