import DOMPurify from 'dompurify'
import type { Message } from '@/lib/types'
import { Badge } from '@/components/ui/badge'
import { formatDate } from '@/lib/badges'

export function MessageTimeline({ messages }: { messages: Message[] }) {
  if (messages.length === 0) {
    return <p className="text-sm text-muted-foreground">No messages.</p>
  }

  return (
    <div className="space-y-4">
      {messages.map((m) => (
        <div key={m.id} className="rounded-lg border bg-muted/30 p-4 transition-colors hover:bg-muted/50">
          <div className="mb-2 flex items-center justify-between text-xs text-muted-foreground">
            <Badge className="border-transparent bg-secondary text-secondary-foreground">{m.senderType}</Badge>
            <span>{formatDate(m.sentAt)}</span>
          </div>
          {m.bodyHtml ? (
            <div
              className="email-html text-sm"
              dangerouslySetInnerHTML={{ __html: DOMPurify.sanitize(m.bodyHtml) }}
            />
          ) : (
            <pre className="whitespace-pre-wrap font-sans text-sm">{m.body}</pre>
          )}
          {m.attachments.length > 0 && (
            <div className="mt-3 flex flex-wrap gap-2">
              {m.attachments.map((a) => (
                <Badge key={a.id} className="border bg-background font-normal text-foreground">
                  📎 {a.fileName} ({Math.max(Math.round(a.sizeBytes / 1024), 1)} KB)
                </Badge>
              ))}
            </div>
          )}
        </div>
      ))}
    </div>
  )
}
