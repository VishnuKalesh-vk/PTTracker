export function statusBadgeClass(status: string): string {
  switch (status) {
    case 'New':
      return 'border-transparent bg-blue-50 text-blue-700 ring-1 ring-inset ring-blue-600/20 dark:bg-blue-500/10 dark:text-blue-300 dark:ring-blue-400/30'
    case 'Open':
      return 'border-transparent bg-indigo-50 text-indigo-700 ring-1 ring-inset ring-indigo-600/20 dark:bg-indigo-500/10 dark:text-indigo-300 dark:ring-indigo-400/30'
    case 'Pending':
      return 'border-transparent bg-amber-50 text-amber-700 ring-1 ring-inset ring-amber-600/25 dark:bg-amber-500/10 dark:text-amber-300 dark:ring-amber-400/30'
    case 'OnHold':
      return 'border-transparent bg-orange-50 text-orange-700 ring-1 ring-inset ring-orange-600/25 dark:bg-orange-500/10 dark:text-orange-300 dark:ring-orange-400/30'
    case 'Resolved':
      return 'border-transparent bg-emerald-50 text-emerald-700 ring-1 ring-inset ring-emerald-600/20 dark:bg-emerald-500/10 dark:text-emerald-300 dark:ring-emerald-400/30'
    case 'Closed':
      return 'border-transparent bg-slate-100 text-slate-600 ring-1 ring-inset ring-slate-500/20 dark:bg-slate-500/10 dark:text-slate-300 dark:ring-slate-400/25'
    default:
      return 'border-transparent bg-slate-100 text-slate-600 ring-1 ring-inset ring-slate-500/20 dark:bg-slate-500/10 dark:text-slate-300 dark:ring-slate-400/25'
  }
}

export function priorityBadgeClass(priority: string): string {
  switch (priority) {
    case 'Low':
      return 'border-transparent bg-slate-100 text-slate-600 ring-1 ring-inset ring-slate-500/20 dark:bg-slate-500/10 dark:text-slate-300 dark:ring-slate-400/25'
    case 'Normal':
      return 'border-transparent bg-sky-50 text-sky-700 ring-1 ring-inset ring-sky-600/20 dark:bg-sky-500/10 dark:text-sky-300 dark:ring-sky-400/30'
    case 'High':
      return 'border-transparent bg-orange-50 text-orange-700 ring-1 ring-inset ring-orange-600/25 dark:bg-orange-500/10 dark:text-orange-300 dark:ring-orange-400/30'
    case 'Urgent':
      return 'border-transparent bg-red-50 text-red-700 ring-1 ring-inset ring-red-600/20 dark:bg-red-500/10 dark:text-red-300 dark:ring-red-400/30'
    default:
      return 'border-transparent bg-slate-100 text-slate-600 ring-1 ring-inset ring-slate-500/20 dark:bg-slate-500/10 dark:text-slate-300 dark:ring-slate-400/25'
  }
}

export function formatDate(value: string | null): string {
  if (!value) return '—'
  return new Date(value).toLocaleString()
}
