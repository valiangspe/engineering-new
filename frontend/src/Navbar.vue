<template>
  <v-app-bar color="primary" prominent>
    <v-app-bar-nav-icon
      variant="text"
      @click.stop="ctx.drawer = !ctx.drawer"
    ></v-app-bar-nav-icon>

    <v-toolbar-title
      >GSPE Engineering | {{ ctx?.user?.username }}</v-toolbar-title
    >

    <v-spacer></v-spacer>

    <template v-if="$vuetify.display.mdAndUp">
      <v-btn icon="mdi-magnify" variant="text"></v-btn>

      <v-btn icon="mdi-filter" variant="text"></v-btn>
    </template>

    <v-btn icon="mdi-dots-vertical" variant="text"></v-btn>
  </v-app-bar>

  <v-navigation-drawer
    v-model="ctx.drawer"
    :location="$vuetify.display.mobile ? 'bottom' : undefined"
    temporary
  >
    <v-list density="compact" nav>
      <v-list-item
        href="/#/"
        prepend-icon="mdi-view-dashboard"
        title="Dashboard"
        value="home"
      ></v-list-item>

      <v-list-item
        href="/#/mpdashboard"
        prepend-icon="mdi-view-dashboard"
        title="Dashboard MP"
        value="home"
      ></v-list-item>

      <v-list-item
        href="/#/template"
        prepend-icon="mdi-content-copy"
        title="Dept Template"
        value="template"
      ></v-list-item>
      <v-list-item
        href="/#/wo"
        prepend-icon="mdi-run-fast"
        title="Activity"
        value="activity"
      ></v-list-item>

      <v-list-item
        href="/#/ecn"
        prepend-icon="mdi-wrench-check"
        title="ECN/CCN"
        value="ecn"
      ></v-list-item>
      <!-- <v-list-item href="/#/ccn" prepend-icon="mdi-wrench-check-outline" title="CCN" value="home"></v-list-item> -->
      <v-list-item
        href="/#/manpower"
        prepend-icon="mdi-account-group"
        title="Manpower"
        value="manpower"
      ></v-list-item>

      <v-list-item
        href="/#/bomapproval"
        prepend-icon="mdi-file-document"
        title="BOM Approval"
        value="bomapproval"
      ></v-list-item>

      <a href="https://eng.iotech.my.id" target="_blank">
        <v-list-item
          prepend-icon="mdi-file-document"
          title="Eng Document (Link)"
          value="file-document"
        ></v-list-item>
      </a>

      <v-list-item
        prepend-icon="mdi-file-document"
        title="Report"
        @click="
          () => {
            if (ctx.expanded.find((e) => e === 'Report')) {
              ctx.expanded = ctx.expanded.filter((e) => e !== 'Report');
            } else {
              ctx.expanded = [...ctx.expanded, 'Report'];
            }
          }
        "
        value="manpower"
        ><v-icon icon="mdi-chevron-down"
      /></v-list-item>

      <template v-if="ctx.expanded.find((e) => e === 'Report')">
        <v-list-item
          prepend-icon="mdi-pencil"
          href="/#/inputdata"
          title="Edit"
          value="pencil"
        ></v-list-item>

        <v-list-item
          prepend-icon="mdi-file-document"
          href="/#/donepo"
          title="Grafik Done PO"
          value="file-document"
        ></v-list-item>

        <v-list-item
          prepend-icon="mdi-file-document"
          href="/#/outspo"
          title="Grafik Outs PO"
          value="file-document"
        ></v-list-item>

        <v-list-item
          prepend-icon="mdi-file-document"
          href="/#/donebyweek"
          title="Report Done By Week"
          value="file-document"
        ></v-list-item>

        <v-list-item
          prepend-icon="mdi-file-document"
          href="/#/detailoverdeadline"
          title="Detail Over Deadline"
          value="file-document"
        ></v-list-item>

        <v-list-item
          prepend-icon="mdi-file-document"
          href="/#/loadpekerjaan"
          title="Load Pekerjaan"
          value="file-document"
        ></v-list-item>

        <!-- <v-list-item
          prepend-icon="mdi-file-document"
          href="/#/manpower"
          title="Report Done MP"
          value="file-document"
        ></v-list-item> -->

        <v-list-item
          prepend-icon="mdi-file-document"
          href="/#/outsporeport"
          title="Report Activity"
          value="file-document"
        ></v-list-item>

        <v-list-item
          prepend-icon="mdi-file-document"
          href="/#/outsmpreport"
          title="Report MP"
          value="file-document"
        ></v-list-item>

        <!-- <v-list-item prepend-icon="mdi-file-document" href="/#/outsplan" title="Outs Plan"
                value="file-document"></v-list-item> -->

        <!-- <v-list-item
          prepend-icon="mdi-file-document"
          href="/#/ecnccnreport"
          title="ECN/CCN Report"
          value="file-document"
        ></v-list-item>

        <v-list-item
          prepend-icon="mdi-file-document"
          href="/#/supportothers"
          title="Support Others"
          value="file-document"
        ></v-list-item>

        <v-list-item
          href="/#/training"
          prepend-icon="mdi-safety-goggles"
          title="Training"
          value="manpower"
        ></v-list-item> -->
      </template>

      <v-list-item
        prepend-icon=""
        @click="
          () => {
            logout();
          }
        "
      >
        <buttton class="btn btn-sm btn-danger">Logout</buttton>
      </v-list-item>

      <!-- <v-list-item prepend-icon="mdi-account" title="My Account" value="account"></v-list-item>
          <v-list-item prepend-icon="mdi-account-group-outline" title="Users" value="users"></v-list-item> -->
    </v-list>
  </v-navigation-drawer>
</template>
<script setup>
import { ctx } from "./main";

const logout = () => {
  localStorage.removeItem("token");
  ctx.value.user = null;
  ctx.value.token = null;
};
</script>
