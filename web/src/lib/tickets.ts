import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { api } from './api'
import type {
  ApiResponse,
  EmailIngestResult,
  PagedResponse,
  TicketDetail,
  TicketSummary,
} from './types'

export interface TicketQuery {
  page: number
  pageSize: number
  status?: string
  priority?: string
  search?: string
  sortBy: string
  sortDescending: boolean
}

export function useTickets(q: TicketQuery) {
  return useQuery({
    queryKey: ['tickets', q],
    queryFn: async () => {
      const params: Record<string, string | number | boolean> = {
        page: q.page,
        pageSize: q.pageSize,
        sortBy: q.sortBy,
        sortDescending: q.sortDescending,
      }
      if (q.status) params.status = q.status
      if (q.priority) params.priority = q.priority
      if (q.search) params.search = q.search
      const res = await api.get<ApiResponse<PagedResponse<TicketSummary>>>('/api/tickets', { params })
      return res.data.data
    },
  })
}

export function useTicket(id: string) {
  return useQuery({
    queryKey: ['ticket', id],
    queryFn: async () => {
      const res = await api.get<ApiResponse<TicketDetail>>(`/api/tickets/${id}`)
      return res.data.data
    },
  })
}

export interface UpdateTicketInput {
  subject: string
  description: string
  descriptionHtml: string | null
  status: string
  priority: string
  assignedUserId: string | null
}

export function useUpdateTicket(id: string) {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: async (input: UpdateTicketInput) => {
      const res = await api.put<ApiResponse<TicketDetail>>(`/api/tickets/${id}`, input)
      return res.data.data
    },
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: ['ticket', id] })
      void qc.invalidateQueries({ queryKey: ['tickets'] })
    },
  })
}

export interface CreateManualTicketInput {
  customerName: string
  customerEmail: string
  subject: string
  description: string
  descriptionHtml: string | null
  priority: string
}

export function useCreateManualTicket() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: async (input: CreateManualTicketInput) => {
      const res = await api.post<ApiResponse<TicketDetail>>('/api/tickets/manual', input)
      return res.data.data
    },
    onSuccess: () => void qc.invalidateQueries({ queryKey: ['tickets'] }),
  })
}

export function useSetJiraId(id: string) {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: async (jiraId: string | null) => {
      const res = await api.put<ApiResponse<TicketDetail>>(`/api/tickets/${id}/jira`, { jiraId })
      return res.data.data
    },
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: ['ticket', id] })
      void qc.invalidateQueries({ queryKey: ['tickets'] })
    },
  })
}

export function useDeleteTicket() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: async (id: string) => {
      await api.delete(`/api/tickets/${id}`)
    },
    onSuccess: () => void qc.invalidateQueries({ queryKey: ['tickets'] }),
  })
}

export function useIngestEml() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: async (file: File) => {
      const form = new FormData()
      form.append('file', file)
      const res = await api.post<ApiResponse<EmailIngestResult>>('/api/tickets/ingest-eml', form)
      return res.data.data
    },
    onSuccess: () => void qc.invalidateQueries({ queryKey: ['tickets'] }),
  })
}
