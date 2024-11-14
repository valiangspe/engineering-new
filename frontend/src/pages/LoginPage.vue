<script setup>
import { ref } from "vue";
import { ctx } from "../main";
import { fetchUser } from "./fetchers";
import { jwtDecode } from "jwt-decode";
import axios from 'axios';

const username = ref("");
const password = ref("");

const handleSave = async () => {
  try {
    const resp = await fetch(`${import.meta.env.VITE_APP_AUTHSERVER_URL}/login`, {
      method: "post",
      headers: { "content-type": "application/json" },
      body: JSON.stringify({
        username: username.value,
        password: password.value,
      }),
    });

    if (resp.status !== 200) {
      throw await resp.text();
    }

    const token = await resp.text();
    ctx.value.token = token;

    // Simpan token ke localStorage
    localStorage.setItem("token", token);

    // Decode token untuk mendapatkan user ID
    const decoded = jwtDecode(token);
    const userId = decoded?.jti; // Mengambil user ID dari token
    ctx.value.userId = userId; // Menyimpan user ID di ctx

    // Simpan userId ke localStorage untuk persisten
    localStorage.setItem("userId", userId);

    console.log("User ID berhasil didapatkan:", userId); // Debugging User ID

    // Fetch user data berdasarkan userId
    ctx.value.user = await fetchUser({
      id: userId,
      apiKey: token,
    });

    // Mengambil user role menggunakan axios dengan metode yang diberikan
    const roleResp = await axios.get(`${import.meta.env.VITE_APP_BASE_URL}/api/userroles/byUserId/${userId}`);
    if (roleResp.data && roleResp.data.length > 0) {
      const role = roleResp.data[0].role;
      // Menyimpan role pengguna sebagai string di ctx dan localStorage
      ctx.value.userRole = role;
      localStorage.setItem("userRole", role);
      console.log("Role berhasil diambil:", ctx.value.userRole); // Debugging Role
    } else {
      console.warn("Tidak ada data role yang ditemukan untuk user ini."); // Log ketika tidak ada role
    }

  } catch (e) {
    alert(`${e}`);
    console.error("Login Error:", e);
  }
};
</script>

<template>
  <div class="d-flex justify-content-center align-items-center vh-100 vw-100 bg-primary">
    <div class="bg-light p-4">
      <div class="my-2">
        <h4 class="text-dark">GSPE Engineering</h4>
      </div>
      <div class="my-2">
        <input
          class="form-control form-control-sm"
          placeholder="Username..."
          v-model="username"
        />
      </div>
      <div class="my-2">
        <input
          class="form-control form-control-sm"
          placeholder="Password..."
          type="password"
          v-model="password"
        />
      </div>
      <div class="my-2">
        <button
          class="btn btn-sm btn-secondary"
          @click="handleSave"
        >
          Login
        </button>
      </div>
    </div>
  </div>
</template>
