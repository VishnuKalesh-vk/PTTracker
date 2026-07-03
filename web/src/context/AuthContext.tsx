import { createContext, useContext, useState, type ReactNode } from 'react'
import { api } from '@/lib/api'
import { clearSession, getSession, setSession, type SessionUser } from '@/lib/auth'
import type { AuthResponse } from '@/lib/types'

interface AuthContextValue {
  user: SessionUser | null
  login: (email: string, password: string) => Promise<void>
  register: (firstName: string, lastName: string, email: string, password: string) => Promise<void>
  logout: () => void
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<SessionUser | null>(getSession()?.user ?? null)

  const login = async (email: string, password: string) => {
    const res = await api.post<AuthResponse>('/api/auth/login', { email, password })
    setUser(setSession(res.data).user)
  }

  const register = async (firstName: string, lastName: string, email: string, password: string) => {
    const res = await api.post<AuthResponse>('/api/auth/register', { firstName, lastName, email, password })
    setUser(setSession(res.data).user)
  }

  const logout = () => {
    clearSession()
    setUser(null)
  }

  return <AuthContext.Provider value={{ user, login, register, logout }}>{children}</AuthContext.Provider>
}

// eslint-disable-next-line react-refresh/only-export-components
export function useAuth() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used within AuthProvider')
  return ctx
}
