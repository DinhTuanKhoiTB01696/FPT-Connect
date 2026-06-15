import axios from 'axios'

export const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE ?? 'http://localhost:5080/api/v1',
  headers: { 'Content-Type': 'application/json' }
})

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('accessToken')
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

let refreshing: Promise<string | null> | null = null

async function tryRefresh(): Promise<string | null> {
  const refreshToken = localStorage.getItem('refreshToken')
  if (!refreshToken) return null
  try {
    const { data } = await axios.post(
      `${api.defaults.baseURL}/auth/refresh`,
      { refreshToken },
      { headers: { 'Content-Type': 'application/json' } }
    )
    localStorage.setItem('accessToken', data.data.accessToken)
    localStorage.setItem('refreshToken', data.data.refreshToken)
    return data.data.accessToken
  } catch {
    return null
  }
}

// Tự refresh một lần khi gặp 401, rồi retry; thất bại thì đăng xuất cục bộ.
api.interceptors.response.use(
  (res) => res,
  async (error) => {
    const original = error.config
    if (error.response?.status === 401 && original && !original._retry && !original.url?.includes('/auth/')) {
      original._retry = true
      refreshing ??= tryRefresh()
      const newToken = await refreshing
      refreshing = null
      if (newToken) {
        original.headers.Authorization = `Bearer ${newToken}`
        return api(original)
      }
      localStorage.removeItem('accessToken')
      localStorage.removeItem('refreshToken')
      localStorage.removeItem('user')
      if (typeof window !== 'undefined') window.location.assign('/login')
    }
    return Promise.reject(error)
  }
)
