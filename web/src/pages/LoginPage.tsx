import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Ticket } from 'lucide-react'
import { useAuth } from '@/context/AuthContext'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { ThemeToggle } from '@/components/ThemeToggle'

const schema = z.object({
  firstName: z.string().optional(),
  lastName: z.string().optional(),
  email: z.string().email('Enter a valid email'),
  password: z.string().min(8, 'At least 8 characters'),
})
type FormValues = z.infer<typeof schema>

export function LoginPage() {
  const { login, register: registerUser } = useAuth()
  const navigate = useNavigate()
  const [mode, setMode] = useState<'login' | 'register'>('login')
  const [serverError, setServerError] = useState<string | null>(null)

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>({ resolver: zodResolver(schema) })

  const onSubmit = async (values: FormValues) => {
    setServerError(null)
    try {
      if (mode === 'login') {
        await login(values.email, values.password)
      } else {
        await registerUser(values.firstName ?? '', values.lastName ?? '', values.email, values.password)
      }
      navigate('/')
    } catch {
      setServerError(mode === 'login' ? 'Invalid email or password.' : 'Could not register (email may be in use).')
    }
  }

  return (
    <div className="relative flex min-h-screen items-center justify-center overflow-hidden bg-gradient-to-br from-indigo-50 via-background to-sky-50 p-4 dark:from-indigo-950/40 dark:via-background dark:to-slate-900">
      <div className="pointer-events-none absolute -top-24 left-1/2 h-64 w-[36rem] -translate-x-1/2 rounded-full bg-indigo-200/40 blur-3xl dark:bg-indigo-600/15" />
      <div className="absolute right-4 top-4">
        <ThemeToggle />
      </div>
      <div className="w-full max-w-sm">
        <div className="mb-6 flex flex-col items-center gap-3">
          <span className="flex h-12 w-12 items-center justify-center rounded-2xl bg-gradient-to-br from-indigo-500 to-violet-600 text-white shadow-pop">
            <Ticket size={22} />
          </span>
          <div className="text-center">
            <h1 className="text-xl font-bold tracking-tight">PTTracker</h1>
            <p className="text-sm text-muted-foreground">Production ticket tracking</p>
          </div>
        </div>
        <Card className="shadow-pop">
          <CardHeader>
            <CardTitle>{mode === 'login' ? 'Welcome back' : 'Create an account'}</CardTitle>
            <CardDescription>
              {mode === 'login' ? 'Sign in to continue to your tickets' : 'A few details and you are in'}
            </CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
              {mode === 'register' && (
                <div className="grid grid-cols-2 gap-3">
                  <div className="space-y-1.5">
                    <Label htmlFor="firstName">First name</Label>
                    <Input id="firstName" {...register('firstName')} />
                  </div>
                  <div className="space-y-1.5">
                    <Label htmlFor="lastName">Last name</Label>
                    <Input id="lastName" {...register('lastName')} />
                  </div>
                </div>
              )}
              <div className="space-y-1.5">
                <Label htmlFor="email">Email</Label>
                <Input id="email" type="email" placeholder="you@company.com" {...register('email')} />
                {errors.email && <p className="text-xs text-destructive">{errors.email.message}</p>}
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="password">Password</Label>
                <Input id="password" type="password" placeholder="••••••••" {...register('password')} />
                {errors.password && <p className="text-xs text-destructive">{errors.password.message}</p>}
              </div>
              {serverError && <p className="text-sm text-destructive">{serverError}</p>}
              <Button type="submit" className="w-full" disabled={isSubmitting}>
                {isSubmitting ? 'Please wait…' : mode === 'login' ? 'Sign in' : 'Register'}
              </Button>
            </form>
            <button
              type="button"
              className="mt-4 w-full text-center text-xs text-muted-foreground transition-colors hover:text-foreground hover:underline"
              onClick={() => {
                setMode(mode === 'login' ? 'register' : 'login')
                setServerError(null)
              }}
            >
              {mode === 'login' ? "No account? Register" : 'Have an account? Sign in'}
            </button>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}
