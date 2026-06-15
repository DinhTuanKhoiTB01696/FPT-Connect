<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const router = useRouter()

const step = ref<'credentials' | 'mfa'>('credentials')
const identifier = ref('admin@fptconnect.vn')
const password = ref('Admin@12345')
const otp = ref('')
const error = ref('')
const loading = ref(false)

async function submitCredentials() {
  error.value = ''
  loading.value = true
  try {
    const res = await auth.login(identifier.value, password.value)
    if (res.mfaRequired) {
      step.value = 'mfa'
    } else {
      router.push({ name: 'dashboard' })
    }
  } catch {
    error.value = 'Sai thông tin đăng nhập. Vui lòng thử lại.'
  } finally {
    loading.value = false
  }
}

async function submitOtp() {
  error.value = ''
  loading.value = true
  try {
    await auth.verifyMfa(otp.value)
    router.push({ name: 'dashboard' })
  } catch {
    error.value = 'Mã OTP không đúng hoặc đã hết hạn.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <main class="min-h-screen grid place-items-center bg-background px-4">
    <div class="w-full max-w-sm bg-surface border border-border rounded-card p-6 space-y-5">
      <div class="space-y-1">
        <h1 class="text-2xl font-bold text-foreground">FPT Connect</h1>
        <p class="text-sm text-muted">
          {{ step === 'credentials' ? 'Đăng nhập hệ thống CRM hiện trường' : 'Nhập mã xác thực hai lớp (OTP)' }}
        </p>
      </div>

      <form v-if="step === 'credentials'" class="space-y-4" @submit.prevent="submitCredentials">
        <label class="block space-y-1">
          <span class="text-sm text-foreground">Email / Mã nhân viên</span>
          <input v-model="identifier" type="text" autocomplete="username"
            class="w-full rounded-control border border-border bg-background px-3 py-2 text-foreground focus:outline-none focus:ring-2 focus:ring-primary" />
        </label>
        <label class="block space-y-1">
          <span class="text-sm text-foreground">Mật khẩu</span>
          <input v-model="password" type="password" autocomplete="current-password"
            class="w-full rounded-control border border-border bg-background px-3 py-2 text-foreground focus:outline-none focus:ring-2 focus:ring-primary" />
        </label>
        <p v-if="error" class="text-sm text-danger" role="alert">{{ error }}</p>
        <button type="submit" :disabled="loading"
          class="w-full rounded-control bg-primary py-2 font-medium text-white disabled:opacity-60">
          {{ loading ? 'Đang đăng nhập…' : 'Đăng nhập' }}
        </button>
      </form>

      <form v-else class="space-y-4" @submit.prevent="submitOtp">
        <label class="block space-y-1">
          <span class="text-sm text-foreground">Mã 6 số từ ứng dụng Authenticator</span>
          <input v-model="otp" inputmode="numeric" autocomplete="one-time-code" maxlength="9"
            class="w-full rounded-control border border-border bg-background px-3 py-2 text-center text-lg tracking-widest text-foreground focus:outline-none focus:ring-2 focus:ring-primary" />
          <span class="text-xs text-muted">Có thể nhập mã khôi phục nếu mất thiết bị.</span>
        </label>
        <p v-if="error" class="text-sm text-danger" role="alert">{{ error }}</p>
        <button type="submit" :disabled="loading"
          class="w-full rounded-control bg-primary py-2 font-medium text-white disabled:opacity-60">
          {{ loading ? 'Đang xác thực…' : 'Xác thực' }}
        </button>
        <button type="button" class="w-full text-sm text-muted" @click="step = 'credentials'">← Quay lại</button>
      </form>
    </div>
  </main>
</template>
