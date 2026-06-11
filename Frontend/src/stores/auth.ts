import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { api } from '@/lib/api'

interface AuthUser { id: string; name: string; email: string; roles: string[] }

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('accessToken'))
  const user = ref<AuthUser | null>(JSON.parse(localStorage.getItem('user') ?? 'null'))
  const isAuthenticated = computed(() => !!token.value)

  async function login(identifier: string, password: string) {
    const { data } = await api.post('/auth/login', { identifier, password })
    token.value = data.data.accessToken
    user.value = data.data.user
    localStorage.setItem('accessToken', token.value!)
    localStorage.setItem('user', JSON.stringify(user.value))
  }

  function logout() {
    token.value = null
    user.value = null
    localStorage.removeItem('accessToken')
    localStorage.removeItem('user')
  }

  return { token, user, isAuthenticated, login, logout }
})
