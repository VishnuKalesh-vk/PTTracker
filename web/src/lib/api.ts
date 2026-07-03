import axios from 'axios'
import { clearSession, getSession, setSession } from './auth'
import type { AuthResponse } from './types'

const baseURL = import.meta.env.VITE_API_URL ?? 'http://localhost:5080'

export const api = axios.create({ baseURL })

api.interceptors.request.use((config) => {
  const session = getSession()
  if (session?.accessToken) {
    config.headers.Authorization = `Bearer ${session.accessToken}`
  }
  return config
})

// Single in-flight refresh so concurrent 401s don't trigger multiple refreshes.
let refreshPromise: Promise<string | null> | null = null

async function refreshAccessToken(): Promise<string | null> {
  const session = getSession()
  if (!session?.refreshToken) return null
  try {
    const res = await axios.post<AuthResponse>(`${baseURL}/api/auth/refresh`, {
      refreshToken: session.refreshToken,
    })
    return setSession(res.data).accessToken
  } catch {
    clearSession()
    return null
  }
}

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const original = error.config as (typeof error.config & { _retry?: boolean }) | undefined

    if (error.response?.status === 401 && original && !original._retry) {
      original._retry = true
      refreshPromise = refreshPromise ?? refreshAccessToken()
      const newToken = await refreshPromise
      refreshPromise = null

      if (newToken) {
        original.headers.Authorization = `Bearer ${newToken}`
        return api(original)
      }
      // Refresh failed -> bounce to login.
      if (window.location.pathname !== '/login') {
        window.location.href = '/login'
      }
    }
    return Promise.reject(error)
  },
)
