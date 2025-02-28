import { createApp, ref } from "vue";
// import './style.css'
import App from "./App.vue";
// pinia
import { createPinia } from "pinia";

// Vuetify
import "vuetify/styles";

import role from "../src/pages/addRole.vue"
import "bootstrap/dist/css/bootstrap.min.css";
import "bootstrap/dist/js/bootstrap.bundle.min.js";

import { createVuetify } from "vuetify";
import * as components from "vuetify/components";
import * as directives from "vuetify/directives";
import "@mdi/font/css/materialdesignicons.css"; // Ensure you are using css-loader
import Dashboard from "./pages/Dashboard.vue";
import { createRouter, createWebHashHistory } from "vue-router";
import DonePOPage from "./pages/DonePOPage.vue";
import DoneByWeekPage from "./pages/DoneByWeekPage.vue";
import OutsPOPage from "./pages/OutsPOPage.vue";
import DetailOverDeadlinePage from "./pages/DetailOverDeadlinePage.vue";
import LoadPekerjaanPage from "./pages/LoadPekerjaanPage.vue";
import ManpowerPage from "./pages/ManpowerPage.vue";
import OutsMPPage from "./pages/OutsMPPage.vue";
import OutsPlanPage from "./pages/OutsPlanPage.vue";
import SupportOthersPage from "./pages/SupportOthersPage.vue";
import ECNCCNReportPage from "./pages/ECNCCNReportPage.vue";
import OutsPoReportPage from "./pages/OutsPoReportPage.vue";
import TrainingPage from "./pages/TrainingPage.vue";
import InputDataPage from "./pages/InputDataPage.vue";
import MPDashboard from "./pages/MPDashboard.vue";
import Activity from "./pages/Activity.vue";
import { fetchUsers } from "./pages/fetchers";
import EcnPage from "./pages/EcnPage.vue";
import EcnPageDetail from "./pages/EcnPageDetail.vue";
import EcnPrint from "./pages/EcnPrint.vue";
import ECNApproval from "./pages/ECNApproval.vue";
import BOMApproval from "./pages/BOMApproval.vue";
import BOMApprovalDetail from "./pages/BOMApprovalDetail.vue";
import DeptTemplatePage from "./pages/DeptTemplatePage.vue";
// import NotificationsPage from "./pages/Notifications.vue";
import NotificationsPage from "./pages/Notifications.vue";
import AIDocumentAnalyzerPage from "./pages/AIDocumentAnalyzerPage.vue";
import AIDocumentAnalyzerInqPage from "./pages/AIDocumentAnalyzerInqPage.vue";
import engTimeProcess from "./EngineerTImeProcess.vue"
import SupportDoc from "./pages/SupportDoc.vue";

const routes = [
  // dynamic segments start with a colon
  { path: "/", component: Dashboard },
  { path: "/donepo", component: DonePOPage },
  { path: "/donebyweek", component: DoneByWeekPage },
  { path: "/outspo", component: OutsPOPage },
  { path: "/detailoverdeadline", component: DetailOverDeadlinePage },
  { path: "/loadpekerjaan", component: LoadPekerjaanPage },
  { path: "/manpower", component: ManpowerPage },
  { path: "/template", component: DeptTemplatePage },

  { path: "/outsporeport", component: OutsPoReportPage },
  { path: "/outsmpreport", component: OutsMPPage },
  // { path: "/outsplan", component: OutsPlanPage },
  { path: "/supportothers", component: SupportOthersPage },
  { path: "/ecnccnreport", component: ECNCCNReportPage },
  { path: "/ecn", component: EcnPage },
  { path: "/ecn/:id", component: EcnPageDetail },
  { path: "/ecn/:id/print", component: EcnPrint },
  { path: "/ecn/:id/approval", component: ECNApproval },

  { path: "/training", component: TrainingPage },
  { path: "/inputdata", component: InputDataPage },
  { path: "/mpdashboard", component: MPDashboard },
  { path: "/wo", component: Activity },
  { path: "/bomapproval", component: BOMApproval },
  { path: "/bomapproval/:id", component: BOMApprovalDetail },

  { path: "/ai-document-analyzer/:id", component: AIDocumentAnalyzerPage },
  { path: "/ai-document-analyzer-inq", component: AIDocumentAnalyzerInqPage },

  { path: "/notifications", component: NotificationsPage },

  { path: "/addRole", component: role},
  { path: "/supportDoc", component: SupportDoc }, 

  { path: "/engineerTimeProcess", component: engTimeProcess },
  // { 
  //   path: "/activity/:taskId?", 
  //   name: "activity", 
  //   component: Activity, 
  //   props: true,
  //   alias: "/wo" // Alias untuk mendukung /wo
  // },
  

];

const router = createRouter({
  history: createWebHashHistory(),
  routes,
});
// const app = createApp(App);
const pinia = createPinia();

export const ctx = ref({
  drawer: false,
  expanded: [],
  users: [],
  login: false,
  token: null,
  user: null,
  items: [
    {
      title: "Foo",
      value: "foo",
    },
    {
      title: "Bar",
      value: "bar",
    },
    {
      title: "Fizz",
      value: "fizz",
    },
    {
      title: "Buzz",
      value: "buzz",
    },
  ],
});

const vuetify = createVuetify({
  components,
  directives,
});

(async () => {
  const users = await fetchUsers();

  ctx.value.users = users;
})();

createApp(App).use(vuetify).use(router).use(pinia).mount("#app");
