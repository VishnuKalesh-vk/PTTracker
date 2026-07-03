import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { ArrowLeft } from 'lucide-react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useCreateManualTicket } from '@/lib/tickets'
import { TICKET_PRIORITIES } from '@/lib/types'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Select } from '@/components/ui/select'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { RichTextEditor } from '@/components/RichTextEditor'

const schema = z.object({
  customerName: z.string().min(1, 'Required'),
  customerEmail: z.string().email('Enter a valid email'),
  subject: z.string().min(1, 'Required').max(200),
  priority: z.enum(['Low', 'Normal', 'High', 'Urgent']),
})
type FormValues = z.infer<typeof schema>

export function CreateTicketPage() {
  const navigate = useNavigate()
  const create = useCreateManualTicket()

  const [descHtml, setDescHtml] = useState('')
  const [descText, setDescText] = useState('')
  const [descError, setDescError] = useState<string | null>(null)
  const [serverError, setServerError] = useState<string | null>(null)

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormValues>({ resolver: zodResolver(schema), defaultValues: { priority: 'Normal' } })

  const onSubmit = async (values: FormValues) => {
    setServerError(null)
    setDescError(null)
    if (!descText.trim()) {
      setDescError('Description is required.')
      return
    }
    try {
      const created = await create.mutateAsync({
        ...values,
        description: descText,
        descriptionHtml: descHtml || null,
      })
      navigate(`/tickets/${created.id}`)
    } catch {
      setServerError('Could not create the ticket. Check the fields and try again.')
    }
  }

  return (
    <div className="space-y-4">
      <div className="flex items-center gap-2 text-sm text-muted-foreground">
        <Link to="/" className="inline-flex items-center gap-1 transition-colors hover:text-foreground">
          <ArrowLeft size={14} />
          Tickets
        </Link>
        <span>/</span>
        <span>New</span>
      </div>

      <Card className="mx-auto max-w-2xl">
        <CardHeader>
          <CardTitle>Create ticket</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            <div className="grid grid-cols-2 gap-3">
              <div className="space-y-1">
                <Label htmlFor="customerName">Customer name</Label>
                <Input id="customerName" {...register('customerName')} />
                {errors.customerName && <p className="text-xs text-destructive">{errors.customerName.message}</p>}
              </div>
              <div className="space-y-1">
                <Label htmlFor="customerEmail">Customer email</Label>
                <Input id="customerEmail" type="email" {...register('customerEmail')} />
                {errors.customerEmail && <p className="text-xs text-destructive">{errors.customerEmail.message}</p>}
              </div>
            </div>

            <div className="grid grid-cols-3 gap-3">
              <div className="col-span-2 space-y-1">
                <Label htmlFor="subject">Subject</Label>
                <Input id="subject" {...register('subject')} />
                {errors.subject && <p className="text-xs text-destructive">{errors.subject.message}</p>}
              </div>
              <div className="space-y-1">
                <Label htmlFor="priority">Priority</Label>
                <Select id="priority" className="w-full" {...register('priority')}>
                  {TICKET_PRIORITIES.map((p) => (
                    <option key={p} value={p}>
                      {p}
                    </option>
                  ))}
                </Select>
              </div>
            </div>

            <div className="space-y-1">
              <Label>Description</Label>
              <RichTextEditor
                initialHtml=""
                onChange={(html, text) => {
                  setDescHtml(html)
                  setDescText(text)
                }}
              />
              {descError && <p className="text-xs text-destructive">{descError}</p>}
            </div>

            {serverError && <p className="text-sm text-destructive">{serverError}</p>}

            <div className="flex items-center gap-3">
              <Button type="submit" disabled={create.isPending}>
                {create.isPending ? 'Creating…' : 'Create ticket'}
              </Button>
              <Button type="button" variant="outline" onClick={() => navigate('/')}>
                Cancel
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
