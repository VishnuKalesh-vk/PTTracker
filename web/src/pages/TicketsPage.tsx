import { useRef, useState, type ChangeEvent } from 'react'
import { useNavigate } from 'react-router-dom'
import { ArrowDownWideNarrow, ArrowUpNarrowWide, Inbox, Plus, Search, Trash2, Upload } from 'lucide-react'
import { useDeleteTicket, useIngestEml, useTickets, type TicketQuery } from '@/lib/tickets'
import { TICKET_PRIORITIES, TICKET_STATUSES } from '@/lib/types'
import { formatDate, priorityBadgeClass, statusBadgeClass } from '@/lib/badges'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Select } from '@/components/ui/select'
import { Badge } from '@/components/ui/badge'
import { Card } from '@/components/ui/card'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'

export function TicketsPage() {
  const navigate = useNavigate()
  const fileInput = useRef<HTMLInputElement>(null)
  const ingest = useIngestEml()
  const del = useDeleteTicket()
  const [uploadMsg, setUploadMsg] = useState<string | null>(null)

  const [query, setQuery] = useState<TicketQuery>({
    page: 1,
    pageSize: 20,
    sortBy: 'createdAt',
    sortDescending: true,
  })

  const { data, isLoading, isError } = useTickets(query)

  const patch = (changes: Partial<TicketQuery>) => setQuery((q) => ({ ...q, page: 1, ...changes }))

  const onUpload = async (e: ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (!file) return
    setUploadMsg(null)
    try {
      const result = await ingest.mutateAsync(file)
      setUploadMsg(`${result.outcome}: ticket #${result.ticket.ticketNumber}`)
    } catch {
      setUploadMsg('Upload failed (must be a valid .eml).')
    } finally {
      if (fileInput.current) fileInput.current.value = ''
    }
  }

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-end justify-between gap-3">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Tickets</h1>
          <p className="text-sm text-muted-foreground">
            {data ? `${data.totalCount} ticket${data.totalCount === 1 ? '' : 's'} in view` : 'Manage support requests'}
          </p>
        </div>
        <div className="flex items-center gap-2">
          {uploadMsg && <span className="text-sm text-muted-foreground">{uploadMsg}</span>}
          <input ref={fileInput} type="file" accept=".eml" className="hidden" onChange={onUpload} />
          <Button variant="outline" onClick={() => fileInput.current?.click()} disabled={ingest.isPending}>
            <Upload size={15} />
            {ingest.isPending ? 'Uploading…' : 'Upload .eml'}
          </Button>
          <Button onClick={() => navigate('/tickets/new')}>
            <Plus size={15} />
            Create ticket
          </Button>
        </div>
      </div>

      <Card className="p-4">
        <div className="flex flex-wrap items-end gap-3">
          <div className="flex flex-col gap-1">
            <span className="text-xs font-medium text-muted-foreground">Search</span>
            <div className="relative">
              <Search size={14} className="absolute left-2.5 top-1/2 -translate-y-1/2 text-muted-foreground" />
              <Input
                className="pl-8"
                placeholder="Subject…"
                defaultValue={query.search ?? ''}
                onKeyDown={(e) => {
                  if (e.key === 'Enter') patch({ search: (e.target as HTMLInputElement).value || undefined })
                }}
              />
            </div>
          </div>
          <div className="flex flex-col gap-1">
            <span className="text-xs font-medium text-muted-foreground">Status</span>
            <Select value={query.status ?? ''} onChange={(e) => patch({ status: e.target.value || undefined })}>
              <option value="">All</option>
              {TICKET_STATUSES.map((s) => (
                <option key={s} value={s}>
                  {s}
                </option>
              ))}
            </Select>
          </div>
          <div className="flex flex-col gap-1">
            <span className="text-xs font-medium text-muted-foreground">Priority</span>
            <Select value={query.priority ?? ''} onChange={(e) => patch({ priority: e.target.value || undefined })}>
              <option value="">All</option>
              {TICKET_PRIORITIES.map((p) => (
                <option key={p} value={p}>
                  {p}
                </option>
              ))}
            </Select>
          </div>
          <div className="flex flex-col gap-1">
            <span className="text-xs font-medium text-muted-foreground">Sort</span>
            <Select value={query.sortBy} onChange={(e) => patch({ sortBy: e.target.value })}>
              <option value="createdAt">Created</option>
              <option value="updatedAt">Updated</option>
              <option value="ticketNumber">Number</option>
              <option value="priority">Priority</option>
              <option value="status">Status</option>
              <option value="subject">Subject</option>
            </Select>
          </div>
          <Button
            variant="ghost"
            onClick={() => setQuery((q) => ({ ...q, sortDescending: !q.sortDescending }))}
          >
            {query.sortDescending ? <ArrowDownWideNarrow size={15} /> : <ArrowUpNarrowWide size={15} />}
            {query.sortDescending ? 'Desc' : 'Asc'}
          </Button>
        </div>
      </Card>

      <Card className="overflow-hidden">
        <Table>
          <TableHeader className="bg-muted/50">
            <TableRow className="hover:bg-transparent">
              <TableHead className="w-16">#</TableHead>
              <TableHead>Subject</TableHead>
              <TableHead>Status</TableHead>
              <TableHead>Priority</TableHead>
              <TableHead>Customer</TableHead>
              <TableHead>Updated</TableHead>
              <TableHead className="w-10"></TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {isLoading && (
              <TableRow>
                <TableCell colSpan={7} className="py-8 text-center text-muted-foreground">
                  Loading…
                </TableCell>
              </TableRow>
            )}
            {isError && (
              <TableRow>
                <TableCell colSpan={7} className="py-8 text-center text-destructive">
                  Failed to load tickets.
                </TableCell>
              </TableRow>
            )}
            {data?.items.map((t) => (
              <TableRow key={t.id} className="cursor-pointer" onClick={() => navigate(`/tickets/${t.id}`)}>
                <TableCell className="font-mono text-xs text-muted-foreground">#{t.ticketNumber}</TableCell>
                <TableCell className="max-w-md truncate font-medium">{t.subject}</TableCell>
                <TableCell>
                  <Badge className={statusBadgeClass(t.status)}>{t.status}</Badge>
                </TableCell>
                <TableCell>
                  <Badge className={priorityBadgeClass(t.priority)}>{t.priority}</Badge>
                </TableCell>
                <TableCell>{t.customerName}</TableCell>
                <TableCell className="text-muted-foreground">{formatDate(t.updatedAt)}</TableCell>
                <TableCell onClick={(e) => e.stopPropagation()}>
                  <Button
                    variant="ghost"
                    size="icon"
                    className="h-8 w-8 text-muted-foreground hover:text-destructive"
                    disabled={del.isPending}
                    title={`Delete ticket #${t.ticketNumber}`}
                    onClick={() => {
                      if (window.confirm(`Delete ticket #${t.ticketNumber} "${t.subject}"? This cannot be undone.`)) {
                        del.mutate(t.id)
                      }
                    }}
                  >
                    <Trash2 className="h-4 w-4" />
                  </Button>
                </TableCell>
              </TableRow>
            ))}
            {data && data.items.length === 0 && (
              <TableRow>
                <TableCell colSpan={7} className="py-12 text-center">
                  <div className="flex flex-col items-center gap-2 text-muted-foreground">
                    <Inbox size={28} className="opacity-40" />
                    <span className="text-sm">No tickets found.</span>
                  </div>
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </Card>

      {data && (
        <div className="flex items-center justify-between text-sm">
          <span className="text-muted-foreground">
            {data.totalCount} tickets · page {data.page} of {Math.max(data.totalPages, 1)}
          </span>
          <div className="flex gap-2">
            <Button
              variant="outline"
              size="sm"
              disabled={!data.hasPrevious}
              onClick={() => setQuery((q) => ({ ...q, page: q.page - 1 }))}
            >
              Previous
            </Button>
            <Button
              variant="outline"
              size="sm"
              disabled={!data.hasNext}
              onClick={() => setQuery((q) => ({ ...q, page: q.page + 1 }))}
            >
              Next
            </Button>
          </div>
        </div>
      )}
    </div>
  )
}
