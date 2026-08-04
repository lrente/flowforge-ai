const stats = [
  { label: 'Active agents', value: '12', detail: '+3 this week' },
  { label: 'Knowledge docs', value: '84', detail: '23 pending sync' },
  { label: 'Conversations', value: '1.2k', detail: '92% answered' }
];

const recentActivity = [
  { title: 'Support agent updated', time: '12 min ago', tone: 'bg-cyan-500/15 text-cyan-200' },
  { title: 'New knowledge pack uploaded', time: '1 hr ago', tone: 'bg-violet-500/15 text-violet-200' },
  { title: 'Conversation summary generated', time: 'Today', tone: 'bg-emerald-500/15 text-emerald-200' }
];

export default function DashboardPage() {
  return (
    <div className="space-y-6">
      <section className="rounded-[28px] border border-white/10 bg-slate-900/70 p-6 shadow-2xl shadow-slate-950/20">
        <div className="flex flex-col gap-6 lg:flex-row lg:items-end lg:justify-between">
          <div>
            <p className="text-sm font-medium uppercase tracking-[0.24em] text-cyan-300">Operations overview</p>
            <h2 className="mt-2 text-3xl font-semibold text-white">Your AI workspace is running smoothly.</h2>
            <p className="mt-3 max-w-2xl text-sm text-slate-400">Build, inspect, and refine your agents from a calm, modern control center designed for high-velocity teams.</p>
          </div>
          <button className="rounded-2xl bg-cyan-500 px-4 py-2 text-sm font-semibold text-slate-950 transition hover:bg-cyan-400">Create new agent</button>
        </div>
        <div className="mt-8 grid gap-4 md:grid-cols-3">
          {stats.map((stat) => (
            <div key={stat.label} className="rounded-2xl border border-white/10 bg-white/5 p-4">
              <p className="text-sm text-slate-400">{stat.label}</p>
              <p className="mt-3 text-3xl font-semibold text-white">{stat.value}</p>
              <p className="mt-2 text-sm text-cyan-300">{stat.detail}</p>
            </div>
          ))}
        </div>
      </section>

      <section className="grid gap-6 xl:grid-cols-[1.35fr_0.95fr]">
        <div className="rounded-[28px] border border-white/10 bg-slate-900/70 p-6">
          <div className="flex items-center justify-between">
            <div>
              <h3 className="text-lg font-semibold text-white">Recent activity</h3>
              <p className="text-sm text-slate-400">A snapshot of what changed today.</p>
            </div>
            <button className="text-sm text-cyan-300">View all</button>
          </div>
          <div className="mt-6 space-y-3">
            {recentActivity.map((item) => (
              <div key={item.title} className="flex items-center justify-between rounded-2xl border border-white/10 bg-white/5 px-4 py-3">
                <div className="flex items-center gap-3">
                  <div className={`h-2.5 w-2.5 rounded-full ${item.tone}`} />
                  <div>
                    <p className="text-sm font-medium text-white">{item.title}</p>
                    <p className="text-xs text-slate-400">{item.time}</p>
                  </div>
                </div>
                <span className="text-xs text-slate-500">Synced</span>
              </div>
            ))}
          </div>
        </div>

        <div className="rounded-[28px] border border-white/10 bg-slate-900/70 p-6">
          <h3 className="text-lg font-semibold text-white">Quick actions</h3>
          <div className="mt-6 space-y-3">
            {['Create a new knowledge base', 'Review conversation insights', 'Tune agent tone'].map((action) => (
              <button key={action} className="flex w-full items-center justify-between rounded-2xl border border-white/10 bg-white/5 px-4 py-3 text-left text-sm text-slate-200 transition hover:bg-white/10">
                <span>{action}</span>
                <span className="text-cyan-300">→</span>
              </button>
            ))}
          </div>
        </div>
      </section>
    </div>
  );
}
