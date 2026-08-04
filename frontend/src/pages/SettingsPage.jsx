export default function SettingsPage() {
  return (
    <div className="rounded-[28px] border border-white/10 bg-slate-900/70 p-6">
      <h2 className="text-2xl font-semibold text-white">Settings</h2>
      <p className="mt-2 text-sm text-slate-400">Adjust the look and behavior of your FlowForge workspace.</p>
      <div className="mt-6 grid gap-4 lg:grid-cols-2">
        <div className="rounded-2xl border border-white/10 bg-white/5 p-4">
          <p className="font-medium text-white">Appearance</p>
          <p className="mt-2 text-sm text-slate-400">Switch between dark and light themes.</p>
        </div>
        <div className="rounded-2xl border border-white/10 bg-white/5 p-4">
          <p className="font-medium text-white">Integrations</p>
          <p className="mt-2 text-sm text-slate-400">Connect Slack, Notion, and your OpenAI workspace.</p>
        </div>
      </div>
    </div>
  );
}
