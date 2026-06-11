import { describe, it, expect, beforeEach } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useAuthStore } from '@/stores/auth'

describe('auth store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    localStorage.clear()
  })

  it('starts unauthenticated', () => {
    const auth = useAuthStore()
    expect(auth.isAuthenticated).toBe(false)
  })

  it('logout clears token', () => {
    const auth = useAuthStore()
    auth.logout()
    expect(auth.token).toBeNull()
    expect(localStorage.getItem('accessToken')).toBeNull()
  })
})
