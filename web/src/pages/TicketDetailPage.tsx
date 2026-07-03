import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { ArrowLeft, MessagesSquare, Save } from 'lucide-react'
import { useSetJiraId, useTicket, useUpdateTicket } from '@/lib/tickets'
import { TICKET_PRIORITIES, TICKET_STATUSES } from '@/lib/types'
import { formatDate, priorityBadgeClass, statusBadgeClass } from '@/lib/badges'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Select } from '@/components/ui/select'
import { Badge } from '@/components/ui/badge'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { MessageTimeline } from '@/components/MessageTimeline'
import { RichTextEditor } from '@/components/RichTextEditor'

function plainToHtml(text: string): string {
  const escaped = text.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
  return `<p>${escaped.replace(/\n/g, '<br>')}</p>`
}

export function TicketDetailPage() {
  const { id = '' } = useParams()
  const { data: ticket, isLoading, isError } = useTicket(id)
  const update = useUpdateTicket(id)
  const setJiraId = useSetJiraId(id)

  const [form, setForm] = useState({ subject: '', status: '', priority: '' })
  const [descHtml, setDescHtml] = useState('')
  const [descText, setDescText] = useState('')
  const [jira, setJira] = useState('')
  const [saved, setSaved] = useState(false)

  useEffect(() => {
    if (ticket) {
      setForm({ subject: ticket.subject, status: ticket.status, priority: ticket.priority })
      setDescHtml(ticket.descriptionHtml ?? '')
      setDescText(ticket.description)
      setJira(ticket.jiraId ?? '')
    }
  }, [ticket])

  if (isLoading) return <p className="text-muted-foreground">Loading…</p>
  if (isError || !ticket) return <p className="text-destructive">Ticket not found.</p>

  const initialHtml = ticket.descriptionHtml ?? plainToHtml(ticket.description)

  const onSave = async () => {
    setSaved(false)
    await update.mutateAsync({
      subject: form.subject,
      description: descText.trim() ? descText : ticket.description,
      descriptionHtml: descHtml || null,
      status: form.status,
      priority: form.priority,
      assignedUserId: ticket.assignedUserId,
    })
    setSaved(true)
  }

  return (
    <div className="space-y-4">
      <div className="flex items-center gap-2 text-sm text-muted-foreground">
        <Link to="/" className="inline-flex items-center gap-1 transition-colors hover:text-foreground">
          <ArrowLeft size={14} />
          Tickets
        </Link>
        <span>/</span>
        <span className="rounded-md bg-muted px-1.5 py-0.5 font-mono text-xs">#{ticket.ticketNumber}</span>
      </div>

      <div className="grid gap-4 md:grid-cols-3">
        <Card className="min-w-0 md:col-span-2">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <span>Edit ticket</span>
              <Badge className={statusBadgeClass(form.status)}>{form.status}</Badge>
              <Badge className={priorityBadgeClass(form.priority)}>{form.priority}</Badge>
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="space-y-1">
              <Label htmlFor="subject">Subject</Label>
              <Input id="subject" value={form.subject} onChange={(e) => setForm({ ...form, subject: e.target.value })} />
            </div>
            <div className="space-y-1">
              <Label>Description</Label>
              <RichTextEditor
                initialHtml={initialHtml}
                onChange={(html, text) => {
                  setDescHtml(html)
                  setDescText(text)
                }}
              />
            </div>
            <div className="grid grid-cols-2 gap-3">
              <div className="space-y-1">
                <Label>Status</Label>
                <Select
                  className="w-full"
                  value={form.status}
                  onChange={(e) => setForm({ ...form, status: e.target.value })}
                >
                  {TICKET_STATUSES.map((s) => (
                    <option key={s} value={s}>
                      {s}
                    </option>
                  ))}
                </Select>
              </div>
              <div className="space-y-1">
                <Label>Priority</Label>
                <Select
                  className="w-full"
                  value={form.priority}
                  onChange={(e) => setForm({ ...form, priority: e.target.value })}
                >
                  {TICKET_PRIORITIES.map((p) => (
                    <option key={p} value={p}>
                      {p}
                    </option>
                  ))}
                </Select>
              </div>
            </div>
            <div className="flex items-center gap-3">
              <Button onClick={onSave} disabled={update.isPending}>
                <Save size={15} />
                {update.isPending ? 'Saving…' : 'Save changes'}
              </Button>
              {saved && <span className="text-sm text-green-600 dark:text-green-400">Saved.</span>}
              {update.isError && <span className="text-sm text-destructive">Save failed.</span>}
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Details</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2 text-sm">
            <div className="space-y-1 border-b pb-3">
              <span className="text-muted-foreground">JIRA ID</span>
              <div className="flex gap-2">
                <Input
                  value={jira}
                  onChange={(e) => setJira(e.target.value)}
                  placeholder="DART-123"
                  className="h-8"
                />
                <Button
                  size="sm"
                  disabled={setJiraId.isPending || jira.trim() === (ticket.jiraId ?? '')}
                  onClick={() => setJiraId.mutate(jira.trim() ? jira.trim() : null)}
                >
                  {setJiraId.isPending ? 'Saving…' : 'Assign'}
                </Button>
              </div>
              {setJiraId.isError && (
                <p className="text-xs text-destructive">Invalid JIRA ID — use a key like DART-123.</p>
              )}
              {setJiraId.isSuccess && !setJiraId.isPending && (
                <p className="text-xs text-green-600 dark:text-green-400">Saved.</p>
              )}
            </div>
            <Row label="Customer" value={ticket.customerName} />
            <Row label="Email" value={ticket.customerEmail} />
            <Row label="Assignee" value={ticket.assignedUserName ?? 'Unassigned'} />
            <Row label="Tags" value={ticket.tags.length ? ticket.tags.join(', ') : '—'} />
            <Row label="Created" value={formatDate(ticket.createdAt)} />
            <Row label="Updated" value={formatDate(ticket.updatedAt)} />
            <Row label="Resolved" value={formatDate(ticket.resolvedAt)} />
            <Row label="Closed" value={formatDate(ticket.closedAt)} />
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <MessagesSquare size={17} className="text-muted-foreground" />
            Conversation
            <span className="rounded-full bg-muted px-2 py-0.5 text-xs font-medium text-muted-foreground">
              {ticket.messages.length}
            </span>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <MessageTimeline messages={ticket.messages} />
        </CardContent>
      </Card>
    </div>
  )
}

function Row({ label, value }: { label: string; value: string }) {
  return (
    <div className="flex justify-between gap-4">
      <span className="text-muted-foreground">{label}</span>
      <span className="text-right">{value}</span>
    </div>
  )
}
