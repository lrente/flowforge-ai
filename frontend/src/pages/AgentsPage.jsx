const agents = [
  { name: 'Support Copilot', description: 'Answers product questions and triages tickets.', status: 'Live' },
  { name: 'Sales Scout', description: 'Qualifies leads and drafts follow-up emails.', status: 'Draft' },
  { name: 'Ops Analyst', description: 'Summarizes incidents and recommends next steps.', status: 'Live' }
];

export default function AgentsPage() {
  return (
    <div className="space-y-6">
      <div className="rounded-[28px] border border-white/10 bg-slate-900/70 p-6">
        <div className="flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
          <div>
            <h2 className="text-2xl font-semibold text-white">Agents</h2>
            <p className="mt-2 text-sm text-slate-400">Create and organize the assistants that power your workflows.</p>
          </div>
          <button className="rounded-2xl bg-cyan-500 px-4 py-2 text-sm font-semibold text-slate-950">New agent</button>
        </div>
        <div className="mt-6 grid gap-4 lg:grid-cols-3">
          {agents.map((agent) => (
            <div key={agent.name} className="rounded-2xl border border-white/10 bg-white/5 p-4">
              <div className="flex items-center justify-between">
                <h3 className="font-semibold text-white">{agent.name}</h3>
                <span className="rounded-full border border-cyan-500/20 bg-cyan-500/10 px-2.5 py-1 text-xs text-cyan-300">{agent.status}</span>
              </div>
              <p className="mt-3 text-sm text-slate-400">{agent.description}</p>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
