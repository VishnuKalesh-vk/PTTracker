import { useEffect, type ReactNode } from 'react'
import { EditorContent, useEditor } from '@tiptap/react'
import StarterKit from '@tiptap/starter-kit'
import { Table } from '@tiptap/extension-table'
import { TableRow } from '@tiptap/extension-table-row'
import { TableHeader } from '@tiptap/extension-table-header'
import { TableCell } from '@tiptap/extension-table-cell'
import { cn } from '@/lib/utils'

interface RichTextEditorProps {
  initialHtml: string
  onChange: (html: string, text: string) => void
}

export function RichTextEditor({ initialHtml, onChange }: RichTextEditorProps) {
  const editor = useEditor({
    extensions: [
      StarterKit,
      Table.configure({ resizable: true }),
      TableRow,
      TableHeader,
      TableCell,
    ],
    content: initialHtml,
    onUpdate: ({ editor }) => onChange(editor.getHTML(), editor.getText()),
  })

  // Reset content when switching to a different ticket (initialHtml is stable while typing).
  useEffect(() => {
    if (editor && initialHtml !== editor.getHTML()) {
      editor.commands.setContent(initialHtml, { emitUpdate: false })
    }
  }, [initialHtml, editor])

  if (!editor) return null

  return (
    <div className="rounded-md border">
      <div className="flex flex-wrap items-center gap-1 border-b p-1">
        <ToolbarButton active={editor.isActive('bold')} onClick={() => editor.chain().focus().toggleBold().run()}>
          <span className="font-bold">B</span>
        </ToolbarButton>
        <ToolbarButton active={editor.isActive('italic')} onClick={() => editor.chain().focus().toggleItalic().run()}>
          <span className="italic">I</span>
        </ToolbarButton>
        <ToolbarButton
          active={editor.isActive('bulletList')}
          onClick={() => editor.chain().focus().toggleBulletList().run()}
        >
          • List
        </ToolbarButton>
        <span className="mx-1 h-5 w-px bg-border" />
        <ToolbarButton
          onClick={() => editor.chain().focus().insertTable({ rows: 3, cols: 3, withHeaderRow: true }).run()}
        >
          + Table
        </ToolbarButton>
        <ToolbarButton onClick={() => editor.chain().focus().addColumnAfter().run()}>+ Col</ToolbarButton>
        <ToolbarButton onClick={() => editor.chain().focus().addRowAfter().run()}>+ Row</ToolbarButton>
        <ToolbarButton onClick={() => editor.chain().focus().deleteTable().run()}>✕ Table</ToolbarButton>
      </div>
      <EditorContent
        editor={editor}
        className="email-html min-h-32 max-w-none p-3 text-sm [&_.ProseMirror]:outline-none"
      />
    </div>
  )
}

function ToolbarButton({
  active,
  onClick,
  children,
}: {
  active?: boolean
  onClick: () => void
  children: ReactNode
}) {
  return (
    <button
      type="button"
      onClick={onClick}
      className={cn(
        'rounded px-2 py-1 text-xs hover:bg-accent',
        active && 'bg-accent text-accent-foreground',
      )}
    >
      {children}
    </button>
  )
}
