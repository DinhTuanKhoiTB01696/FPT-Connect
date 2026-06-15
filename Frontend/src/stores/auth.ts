import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { api } from '@/lib/api'

interface AuthUser { id: string; name: string; email: string; roles: string[] }
export interface LoginResult { mfaRequired: boolean; mustEnrollMfa?: boolean }

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('accessToken'))
  const user = ref<AuthUser | null>(JSON.parse(localStorage.getItem('user') ?? 'null'))
  const challengeToken = ref<string | null>(null)
  const isAuthenticated = computed(() => !!token.value)

  function persist(accessToken: string, refreshToken: string, u: AuthUser) {
    token.value = accessToken
    user.value = u
    localStorage.setItem('accessToken', accessToken)
    localStorage.setItem('refreshToken', refreshToken)
    localStorage.setItem('user', JSON.stringify(u))
  }

  async function login(identifier: string, password: string): Promise<LoginResult> {
    const { data } = await api.post('/auth/login', { identifier, password })
    if (data.data.mfaRequired) {
      challengeToken.value = data.data.challengeToken
      return { mfaRequired: true }
    }
    persist(data.data.accessToken, data.data.refreshToken, data.data.user)
    return { mfaRequired: false, mustEnrollMfa: data.data.mustEnrollMfa }
  }

  async function verifyMfa(code: string) {
    const { data } = await api.post('/auth/mfa/verify', { challengeToken: challengeToken.value, code })
    persist(data.data.accessToken, data.data.refreshToken, data.data.user)
    challengeToken.value = null
  }

  function clear() {
    token.value = null
    user.value = null
    challengeToken.value = null
    localStorage.removeItem('accessToken')
    localStorage.removeItem('refreshToken')
    localStorage.removeItem('user')
  }

  async function logout(scope: 'current' | 'all' = 'current') {
    try { await api.post('/auth/logout', { scope }) } catch { /* vẫn clear local */ }
    clear()
  }

  return { token, user, challengeToken, isAuthenticated, login, verifyMfa, logout, clear }
})
