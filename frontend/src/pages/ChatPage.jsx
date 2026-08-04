const conversations = [
  { id: 1, title: 'Product launch recap', summary: 'Summarized key release notes and next actions.' },
  { id: 2, title: 'Support escalation', summary: 'Drafted a customer-friendly response.' },
  { id: 3, title: 'Pricing strategy', summary: 'Compared annual vs monthly plan incentives.' }
];

const messageSeed = [
  { id: 1, role: 'assistant', content: 'Hello! I can help summarize work, draft replies, and answer product questions.' },
  { id: 2, role: 'user', content: 'Show me a polished status update for the team.' }
];

export default function ChatPage() {
  return (
    <div className="grid gap-6 xl:grid-cols-[320px_1fr]">
      <section className="rounded-[28px] border border-white/10 bg-slate-900/70 p-4">
        <div className="flex items-center justify-between">
          <div>
            <p className="text-sm font-semibold text-cyan-300">Conversations</p>
            <h2 className="text-xl font-semibold text-white">Recent chats</h2>
          </div>
          <button className="rounded-full border border-cyan-500/20 bg-cyan-500/10 px-3 py-1 text-sm text-cyan-200">New</button>
        </div>
        <div className="mt-4 space-y-3">
          {conversations.map((item) => (
            <div key={item.id} className="rounded-2xl border border-white/10 bg-white/5 p-3">
              <p className="font-medium text-white">{item.title}</p>
              <p className="mt-1 text-sm text-slate-400">{item.summary}</p>
            </div>
          ))}
        </div>
      </section>

      <section className="rounded-[28px] border border-white/10 bg-slate-900/70 p-4 sm:p-6">
        <div className="flex flex-wrap items-center justify-between gap-3 border-b border-white/10 pb-4">
          <div>
            <p className="text-sm font-semibold text-cyan-300">AI chat</p>
            <h2 className="text-xl font-semibold text-white">Ask anything</h2>
          </div>
          <div className="rounded-full border border-cyan-500/20 bg-cyan-500/10 px-3 py-1 text-sm text-cyan-200">Live demo</div>
        </div>

        <div className="mt-6 space-y-4">
          {messageSeed.map((message) => (
            <div key={message.id} className={`flex ${message.role === 'user' ? 'justify-end' : 'justify-start'}`}>
              <div className={`max-w-[85%] rounded-[24px] px-4 py-3 text-sm leading-7 ${message.role === 'user' ? 'bg-cyan-500 text-slate-950' : 'border border-white/10 bg-white/5 text-slate-200'}`}>
                {message.content}
              </div>
            </div>
          ))}
        </div>

        <div className="mt-6 rounded-[24px] border border-white/10 bg-slate-950/70 p-3">
          <textarea rows={4} placeholder="Type your message here..." className="w-full resize-none bg-transparent px-2 py-2 text-sm text-slate-100 outline-none placeholder:text-slate-500" />
          <div className="flex items-center justify-between px-2 pb-2 pt-1">
            <p className="text-xs text-slate-500">Responsive, polished, and ready for your app.</p>
            <button className="rounded-full bg-cyan-500 px-4 py-2 text-sm font-semibold text-slate-950">Send</button>
          </div>
        </div>
      </section>
    </div>
  );
}
