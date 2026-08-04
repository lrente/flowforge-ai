export default function AgentEditorPage() {
  return (
    <div className="rounded-[28px] border border-white/10 bg-slate-900/70 p-6">
      <h2 className="text-2xl font-semibold text-white">Agent editor</h2>
      <p className="mt-2 text-sm text-slate-400">Fine-tune your assistant personality, prompts, and defaults.</p>
      <div className="mt-6 grid gap-4 lg:grid-cols-2">
        <label className="block text-sm text-slate-300">
          <span className="mb-2 block">Agent name</span>
          <input className="w-full rounded-2xl border border-white/10 bg-slate-950/70 px-3 py-2 outline-none" placeholder="Support Copilot" />
        </label>
        <label className="block text-sm text-slate-300">
          <span className="mb-2 block">Model</span>
          <input className="w-full rounded-2xl border border-white/10 bg-slate-950/70 px-3 py-2 outline-none" defaultValue="gpt-4.1-mini" />
        </label>
        <label className="block text-sm text-slate-300 lg:col-span-2">
          <span className="mb-2 block">Description</span>
          <textarea rows={4} className="w-full rounded-2xl border border-white/10 bg-slate-950/70 px-3 py-2 outline-none" placeholder="Explain what this agent should do." />
        </label>
      </div>
      <button className="mt-6 rounded-2xl bg-cyan-500 px-4 py-2 text-sm font-semibold text-slate-950">Save changes</button>
    </div>
  );
}
