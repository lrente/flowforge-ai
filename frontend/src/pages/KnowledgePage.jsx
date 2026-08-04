const knowledgeBases = [
  { title: 'Support playbook', summary: 'Policies, FAQ, and escalation routes.', updated: '2h ago' },
  { title: 'Pricing docs', summary: 'Plans, add-ons, and enterprise terms.', updated: 'Yesterday' },
  { title: 'Product launches', summary: 'Release notes and launch checklist.', updated: '3 days ago' }
];

export default function KnowledgePage() {
  return (
    <div className="space-y-6">
      <div className="rounded-[28px] border border-white/10 bg-slate-900/70 p-6">
        <div className="flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
          <div>
            <h2 className="text-2xl font-semibold text-white">Knowledge</h2>
            <p className="mt-2 text-sm text-slate-400">Ground your agents in rich, structured information.</p>
          </div>
          <button className="rounded-2xl bg-cyan-500 px-4 py-2 text-sm font-semibold text-slate-950">Upload doc</button>
        </div>

        <div className="mt-6 grid gap-4 lg:grid-cols-3">
          {knowledgeBases.map((item) => (
            <div key={item.title} className="rounded-2xl border border-white/10 bg-white/5 p-4">
              <h3 className="font-semibold text-white">{item.title}</h3>
              <p className="mt-2 text-sm text-slate-400">{item.summary}</p>
              <p className="mt-4 text-xs uppercase tracking-[0.2em] text-slate-500">Updated {item.updated}</p>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
