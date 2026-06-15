<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { api } from '@/lib/api'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const router = useRouter()

interface Session { id: string; createdAtUtc: string; expiresAtUtc: string; revokedAtUtc: string | null; isCurrent: boolean; isActive: boolean }
interface Device { id: string; name: string; platform: string | null; riskStatus: string; lastSeenAtUtc: string }

const sessions = ref<Session[]>([])
const devices = ref<Device[]>([])
const error = ref('')

// MFA enrollment state
const mfaEnabled = ref(false)
const enroll = ref<{ secret: string; otpauthUri: string } | null>(null)
const confirmCode = ref('')
const recoveryCodes = ref<string[]>([])
const mfaMsg = ref('')

async function loadAll() {
  error.value = ''
  try {
    const [me, s, d] = await Promise.all([api.get('/auth/me'), api.get('/sessions'), api.get('/devices')])
    mfaEnabled.value = me.data.data.mfaEnabled
    sessions.value = s.data.data
    devices.value = d.data.data
  } catch {
    error.value = 'Không tải được dữ liệu hồ sơ.'
  }
}
onMounted(loadAll)

async function revokeSession(id: string) {
  await api.delete(`/sessions/${id}`)
  await loadAll()
}
async function renameDevice(d: Device) {
  const name = window.prompt('Tên thiết bị mới', d.name)
  if (!name) return
  await api.patch(`/devices/${d.id}`, { name })
  await loadAll()
}
async function revokeDevice(id: string) {
  await api.delete(`/devices/${id}`)
  await loadAll()
}

async function startEnroll() {
  mfaMsg.value = ''
  recoveryCodes.value = []
  const { data } = await api.post('/auth/mfa/enroll')
  enroll.value = data.data
}
async function confirmEnroll() {
  mfaMsg.value = ''
  try {
    const { data } = await api.post('/auth/mfa/confirm', { code: confirmCode.value })
    recoveryCodes.value = data.data.recoveryCodes
    enroll.value = null
    confirmCode.value = ''
    mfaEnabled.value = true
  } catch {
    mfaMsg.value = 'Mã không đúng, thử lại.'
  }
}

async function logoutAll() {
  await auth.logout('all')
  router.push({ name: 'login' })
}
</script>

<template>
  <div class="min-h-screen bg-background">
    <header class="border-b border-border bg-surface">
      <div class="mx-auto max-w-4xl px-4 py-3 flex items-center justify-between">
        <RouterLink :to="{ name: 'dashboard' }" class="text-sm text-muted">← Dashboard</RouterLink>
        <h1 class="text-lg font-medium text-foreground">Hồ sơ &amp; bảo mật</h1>
        <span class="text-sm text-muted">{{ auth.user?.email }}</span>
      </div>
    </header>

    <main class="mx-auto max-w-4xl px-4 py-6 space-y-8">
      <p v-if="error" class="text-sm text-danger">{{ error }}</p>

      <!-- MFA -->
      <section class="space-y-3">
        <h2 class="text-base font-medium text-foreground">Xác thực hai lớp (MFA)</h2>
        <div class="rounded-card border border-border bg-surface p-4 space-y-3">
          <p class="text-sm" :class="mfaEnabled ? 'text-success' : 'text-muted'">
            Trạng thái: {{ mfaEnabled ? 'Đã bật' : 'Chưa bật' }}
          </p>

          <button v-if="!enroll && !recoveryCodes.length" type="button"
            class="rounded-control border border-border px-3 py-1.5 text-sm text-foreground"
            @click="startEnroll">
            {{ mfaEnabled ? 'Thiết lập lại MFA' : 'Bật MFA' }}
          </button>

          <div v-if="enroll" class="space-y-2">
            <p class="text-sm text-foreground">Quét mã trong app Authenticator hoặc nhập secret thủ công:</p>
            <code class="block break-all rounded bg-background px-2 py-1 text-xs text-muted">{{ enroll.secret }}</code>
            <label class="block space-y-1">
              <span class="text-sm text-foreground">Nhập mã 6 số để xác nhận</span>
              <input v-model="confirmCode" inputmode="numeric" maxlength="6"
                class="w-40 rounded-control border border-border bg-background px-3 py-2 text-center tracking-widest text-foreground focus:outline-none focus:ring-2 focus:ring-primary" />
            </label>
            <p v-if="mfaMsg" class="text-sm text-danger">{{ mfaMsg }}</p>
            <button type="button" class="rounded-control bg-primary px-3 py-1.5 text-sm text-white" @click="confirmEnroll">
              Xác nhận
            </button>
          </div>

          <div v-if="recoveryCodes.length" class="space-y-2">
            <p class="text-sm text-warning">Lưu các mã khôi phục này (chỉ hiển thị một lần):</p>
            <ul class="grid grid-cols-2 gap-1 text-sm font-mono text-foreground">
              <li v-for="c in recoveryCodes" :key="c" class="rounded bg-background px-2 py-1">{{ c }}</li>
            </ul>
          </div>
        </div>
      </section>

      <!-- Sessions -->
      <section class="space-y-3">
        <div class="flex items-center justify-between">
          <h2 class="text-base font-medium text-foreground">Phiên đăng nhập</h2>
          <button type="button" class="text-sm text-danger" @click="logoutAll">Đăng xuất mọi thiết bị</button>
        </div>
        <div class="overflow-hidden rounded-card border border-border bg-surface">
          <table class="w-full text-sm">
            <thead class="bg-background text-muted">
              <tr>
                <th class="px-4 py-2 text-left font-medium">Tạo lúc</th>
                <th class="px-4 py-2 text-left font-medium">Hết hạn</th>
                <th class="px-4 py-2 text-left font-medium">Trạng thái</th>
                <th class="px-4 py-2"></th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="s in sessions" :key="s.id" class="border-t border-border">
                <td class="px-4 py-2 text-foreground">{{ new Date(s.createdAtUtc).toLocaleString('vi-VN') }}</td>
                <td class="px-4 py-2 text-muted">{{ new Date(s.expiresAtUtc).toLocaleDateString('vi-VN') }}</td>
                <td class="px-4 py-2">
                  <span v-if="s.isCurrent" class="text-success">Hiện tại</span>
                  <span v-else-if="s.isActive" class="text-foreground">Hoạt động</span>
                  <span v-else class="text-muted">Đã thu hồi</span>
                </td>
                <td class="px-4 py-2 text-right">
                  <button v-if="s.isActive && !s.isCurrent" type="button" class="text-sm text-danger"
                    @click="revokeSession(s.id)">Thu hồi</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>

      <!-- Devices -->
      <section class="space-y-3">
        <h2 class="text-base font-medium text-foreground">Thiết bị</h2>
        <div class="overflow-hidden rounded-card border border-border bg-surface">
          <table class="w-full text-sm">
            <thead class="bg-background text-muted">
              <tr>
                <th class="px-4 py-2 text-left font-medium">Tên</th>
                <th class="px-4 py-2 text-left font-medium">Nền tảng</th>
                <th class="px-4 py-2 text-left font-medium">Lần cuối</th>
                <th class="px-4 py-2"></th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="d in devices" :key="d.id" class="border-t border-border">
                <td class="px-4 py-2 text-foreground">{{ d.name }}</td>
                <td class="px-4 py-2 text-muted">{{ d.platform ?? '—' }}</td>
                <td class="px-4 py-2 text-muted">{{ new Date(d.lastSeenAtUtc).toLocaleDateString('vi-VN') }}</td>
                <td class="px-4 py-2 text-right space-x-3">
                  <button type="button" class="text-sm text-secondary" @click="renameDevice(d)">Đổi tên</button>
                  <button type="button" class="text-sm text-danger" @click="revokeDevice(d.id)">Thu hồi</button>
                </td>
              </tr>
              <tr v-if="devices.length === 0">
                <td colspan="4" class="px-4 py-6 text-center text-muted">Chưa có thiết bị đăng ký.</td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>
    </main>
  </div>
</template>
