import type { AuthResponse } from './types'

const KEY = 'pttracker.session'

export interface SessionUser {
  id: string
  firstName: string
  lastName: string
  email: string
}

export interface Session {
  accessToken: string
  accessTokenExpiresAt: string
  refreshToken: string
  user: SessionUser
}

export function setSession(res: AuthResponse): Session {
  const session: Session = {
    accessToken: res.accessToken,
    accessTokenExpiresAt: res.accessTokenExpiresAt,
    refreshToken: res.refreshToken,
    user: { id: res.id, firstName: res.firstName, lastName: res.lastName, email: res.email },
  }
  localStorage.setItem(KEY, JSON.stringify(session))
  return session
}

export function getSession(): Session | null {
  const raw = localStorage.getItem(KEY)
  return raw ? (JSON.parse(raw) as Session) : null
}

export function clearSession(): void {
  localStorage.removeItem(KEY)
}
