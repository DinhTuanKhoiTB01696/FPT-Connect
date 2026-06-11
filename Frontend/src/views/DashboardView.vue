<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { api } from '@/lib/api'

const auth = useAuthStore()
const router = useRouter()
const customers = ref<Array<{ id: string; fullName: string; statusCode: string; phoneE164?: string }>>([])
const error = ref('')

onMounted(async () => {
  try {
    const { data } = await api.get('/customers')
    customers.value = data.data
  } catch {
    error.value = 'Không tải được danh sách khách hàng.'
  }
})

function logout() {
  auth.logout()
  router.push({ name: 'login' })
}
</script>

<template>
  <div class="min-h-screen bg-background">
    <header class="border-b border-border bg-surface">
      <div class="mx-auto max-w-6xl px-4 py-3 flex items-center justify-between">
        <h1 class="text-lg font-medium text-foreground">FPT Connect</h1>
        <div class="flex items-center gap-3 text-sm">
          <span class="text-muted">{{ auth.user?.name }}</span>
          <button class="rounded-control border border-border px-3 py-1 text-foreground" @click="logout">
            Đăng xuất
          </button>
        </div>
      </div>
    </header>

    <main class="mx-auto max-w-6xl px-4 py-6 space-y-4">
      <h2 class="text-xl font-medium text-foreground">Khách hàng</h2>
      <p v-if="error" class="text-sm text-danger">{{ error }}</p>

      <div class="overflow-hidden rounded-card border border-border bg-surface">
        <table class="w-full text-sm">
          <thead class="bg-background text-muted">
            <tr>
              <th class="px-4 py-2 text-left font-medium">Tên</th>
              <th class="px-4 py-2 text-left font-medium">Điện thoại</th>
              <th class="px-4 py-2 text-left font-medium">Trạng thái</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="c in customers" :key="c.id" class="border-t border-border">
              <td class="px-4 py-2 text-foreground">{{ c.fullName }}</td>
              <td class="px-4 py-2 text-muted">{{ c.phoneE164 ?? '—' }}</td>
              <td class="px-4 py-2">
                <span class="rounded-full bg-secondary/10 px-2 py-0.5 text-xs text-secondary">{{ c.statusCode }}</span>
              </td>
            </tr>
            <tr v-if="customers.length === 0">
              <td colspan="3" class="px-4 py-6 text-center text-muted">Chưa có khách hàng. Hãy seed dữ liệu mẫu.</td>
            </tr>
          </tbody>
        </table>
      </div>
    </main>
  </div>
</template>
