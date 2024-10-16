<script setup>
import { ref } from "vue";
import { ctx } from "../main";
import { fetchUser } from "./fetchers";
import { jwtDecode } from "jwt-decode";

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

    const data = await resp.text();

    ctx.value.token = data;

    localStorage.setItem("token", data);

    ctx.value.user = await fetchUser({
      id: jwtDecode(data)?.jti,
      apiKey: data,
    });
  } catch (e) {
    alert(`${e}`);
  }
};
</script>
<template>
  <div
    class="d-flex justify-content-center align-items-center vh-100 vw-100 bg-primary"
  >
    <div class="bg-light p-4">
      <div class="my-2">
        <h4 class="text-dark">GSPE Engineering</h4>
      </div>
      <div class="my-2">
        <input
          class="form-control form-control-sm"
          placeholder="Username..."
          @blur="
            (e) => {
              username = e.target.value;
            }
          "
        />
      </div>
      <div class="my-2">
        <input
          class="form-control form-control-sm"
          placeholder="Password..."
          type="password"
          @blur="
            (e) => {
              password = e.target.value;
            }
          "
        />
      </div>
      <div class="my-2">
        <button
          class="btn btn-sm btn-secondary"
          @click="
            () => {
              handleSave();
            }
          "
        >
          Login
        </button>
      </div>
    </div>
  </div>
</template>
