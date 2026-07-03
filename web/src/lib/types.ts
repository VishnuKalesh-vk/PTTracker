// Mirrors PTTracker.Contracts. Regenerate from OpenAPI later to avoid drift.

export interface ApiResponse<T> {
  success: boolean
  data: T
  message?: string
  timestamp: string
}

export interface PagedResponse<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
  hasPrevious: boolean
  hasNext: boolean
}

export interface AuthResponse {
  id: string
  firstName: string
  lastName: string
  email: string
  accessToken: string
  accessTokenExpiresAt: string
  refreshToken: string
}

export interface TicketSummary {
  id: string
  ticketNumber: number
  subject: string
  status: string
  priority: string
  customerName: string
  assignedUserName: string | null
  jiraId: string | null
  createdAt: string
  updatedAt: string
}

export interface MessageAttachment {
  id: string
  fileName: string
  contentType: string
  sizeBytes: number
}

export interface Message {
  id: string
  senderType: string
  body: string
  bodyHtml: string | null
  sentAt: string
  attachments: MessageAttachment[]
}

export interface TicketDetail {
  id: string
  ticketNumber: number
  subject: string
  description: string
  descriptionHtml: string | null
  status: string
  priority: string
  customerId: string
  customerName: string
  customerEmail: string
  assignedUserId: string | null
  assignedUserName: string | null
  jiraId: string | null
  tags: string[]
  messages: Message[]
  createdAt: string
  updatedAt: string
  resolvedAt: string | null
  closedAt: string | null
}

export interface EmailIngestResult {
  outcome: string
  ticket: TicketDetail
}

export const TICKET_STATUSES = ['New', 'Open', 'Pending', 'OnHold', 'Resolved', 'Closed'] as const
export const TICKET_PRIORITIES = ['Low', 'Normal', 'High', 'Urgent'] as const
