export default function AgentCard({
    agent,
    onEdit,
    onDelete
}) {
    return (
        <div className="rounded-3xl border border-white/10 bg-slate-900/70 p-6 transition hover:border-cyan-500/40 hover:shadow-lg">

            <div className="flex justify-between">

                <div>

                    <div className="flex items-center gap-3">

                        <h3 className="text-xl font-semibold text-white">
                            {agent.name}
                        </h3>

                        <span
                            className={`rounded-full px-3 py-1 text-xs font-semibold ${
                                agent.isActive
                                    ? "bg-emerald-500/20 text-emerald-300"
                                    : "bg-slate-700 text-slate-400"
                            }`}
                        >
                            {agent.isActive ? "Active" : "Inactive"}
                        </span>

                    </div>

                    <p className="mt-2 text-slate-400">
                        {agent.businessType}
                    </p>

                    <p className="mt-4 text-sm text-slate-500">
                        {agent.description}
                    </p>

                </div>

                <div className="flex flex-col items-end gap-2">

                    <span className="rounded-full bg-cyan-500/20 px-3 py-1 text-xs text-cyan-300">
                        {agent.model}
                    </span>

                    <div className="flex gap-2">

                        <button
                            onClick={() => onEdit(agent)}
                            className="rounded-xl bg-slate-800 px-3 py-2 hover:bg-slate-700"
                        >
                            ✏️
                        </button>

                        <button
                            onClick={() => onDelete(agent.id)}
                            className="rounded-xl bg-red-600 px-3 py-2 hover:bg-red-500"
                        >
                            🗑
                        </button>

                    </div>

                </div>

            </div>

        </div>
    );
}