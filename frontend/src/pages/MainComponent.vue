<template>
  <div v-if="ctx.user">
    <v-layout>
      <Navbar />
      <v-main style="height: 500px">
        <RouterView />
      </v-main>
    </v-layout>
  </div>
  <div v-else>
    <LoginPage />
  </div>
</template>
<script setup>
import { RouterView } from "vue-router";
import { ctx } from "../main";
import Navbar from "../Navbar.vue";
import LoginPage from "./LoginPage.vue";
import { fetchUser } from "./fetchers";
import { jwtDecode } from "jwt-decode";

if (localStorage.getItem("token")) {
  (async () => {
    ctx.value.token = localStorage.getItem("token");
    const d = await fetchUser({
      id: jwtDecode(localStorage.getItem("token"))?.jti,
      apiKey: ctx.value.token,
    });

    ctx.value.user = d;
  })();
}

const group = null;
</script>
