import { describe, it, expect, beforeEach, vi } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'

vi.mock('@/lib/api', () => ({
  api: { post: vi.fn() }
}))

import { api } from '@/lib/api'
import { useAuthStore } from '@/stores/auth'

describe('auth store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    localStorage.clear()
    vi.clearAllMocks()
  })

  it('starts unauthenticated', () => {
    const auth = useAuthStore()
    expect(auth.isAuthenticated).toBe(false)
  })

  it('login without MFA persists tokens', async () => {
    ;(api.post as any).mockResolvedValue({
      data: { data: { accessToken: 'a', refreshToken: 'r', user: { id: '1', name: 'X', email: 'x', roles: ['SALE'] } } }
    })
    const auth = useAuthStore()
    const res = await auth.login('x', 'y')
    expect(res.mfaRequired).toBe(false)
    expect(auth.isAuthenticated).toBe(true)
    expect(localStorage.getItem('refreshToken')).toBe('r')
  })

  it('login with MFA stores challenge and stays unauthenticated', async () => {
    ;(api.post as any).mockResolvedValue({
      data: { data: { mfaRequired: true, challengeToken: 'chal' } }
    })
    const auth = useAuthStore()
    const res = await auth.login('admin', 'pw')
    expect(res.mfaRequired).toBe(true)
    expect(auth.challengeToken).toBe('chal')
    expect(auth.isAuthenticated).toBe(false)
  })

  it('logout clears state', async () => {
    ;(api.post as any).mockResolvedValue({ data: {} })
    const auth = useAuthStore()
    auth.clear()
    expect(auth.token).toBeNull()
    expect(localStorage.getItem('accessToken')).toBeNull()
  })
})
