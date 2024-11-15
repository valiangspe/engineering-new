<script setup lang="ts">
import { ref } from "vue";
import VueMarkdown from "vue-markdown-render";
import { fetchInquiries } from "./fetchers";

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
      <div><h4>AI Inq Documents</h4></div>
    </div>
    <div><hr class="border border-dark" /></div>

    <div><hr class="border border-dark" /></div>

    <div
      class="overflow-auto border border-dark"
      style="height: 75vh; resize: vertical"
    >
      <table class="table table-sm" style="border-collapse: separate">
        <thead>
          <th
            class="bg-dark text-light"
            style="position: sticky; top: 0"
            v-for="h in ['#', 'ID', 'Inq Number', 'Title', 'Action']"
          >
            {{ h }}
          </th>
        </thead>
        <tbody v-for="(i, i_) in inquiries">
          <td class="border border-dark">{{ i_ + 1 }}</td>
          <td class="border border-dark">{{ i?.id }}</td>
          <td class="border border-dark">{{ i?.inquiryNumber }}</td>
          <td class="border border-dark">{{ i?.title }}</td>
          <td class="border border-dark">
            <div>
              <a :href="`#/ai-document-analyzer/${i?.id}`">
                <button class="btn btn-sm btn-primary" @click="() => {}">
                  Detail
                </button>
              </a>
            </div>
          </td>
        </tbody>
      </table>
    </div>
  </div>
</template>
