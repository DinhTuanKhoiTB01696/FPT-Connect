<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const router = useRouter()
const identifier = ref('admin@fptconnect.vn')
const password = ref('Admin@12345')
const error = ref('')
const loading = ref(false)

async function submit() {
  error.value = ''
  loading.value = true
  try {
    await auth.login(identifier.value, password.value)
    router.push({ name: 'dashboard' })
  } catch {
    error.value = 'Sai thông tin đăng nhập. Vui lòng thử lại.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <main class="min-h-screen grid place-items-center bg-background px-4">
    <form
      class="w-full max-w-sm bg-surface border border-border rounded-card p-6 space-y-4"
      @submit.prevent="submit"
    >
      <div class="space-y-1">
        <h1 class="text-2xl font-bold text-foreground">FPT Connect</h1>
        <p class="text-sm text-muted">Đăng nhập hệ thống CRM hiện trường</p>
      </div>

      <label class="block space-y-1">
        <span class="text-sm text-foreground">Email / Mã nhân viên</span>
        <input
          v-model="identifier"
          type="text"
          autocomplete="username"
          class="w-full rounded-control border border-border bg-background px-3 py-2 text-foreground focus:outline-none focus:ring-2 focus:ring-primary"
        />
      </label>

      <label class="block space-y-1">
        <span class="text-sm text-foreground">Mật khẩu</span>
        <input
          v-model="password"
          type="password"
          autocomplete="current-password"
          class="w-full rounded-control border border-border bg-background px-3 py-2 text-foreground focus:outline-none focus:ring-2 focus:ring-primary"
        />
      </label>

      <p v-if="error" class="text-sm text-danger" role="alert">{{ error }}</p>

      <button
        type="submit"
        :disabled="loading"
        class="w-full rounded-control bg-primary py-2 font-medium text-white disabled:opacity-60"
      >
        {{ loading ? 'Đang đăng nhập…' : 'Đăng nhập' }}
      </button>
    </form>
  </main>
</template>
