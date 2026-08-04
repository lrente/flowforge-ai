import { SearchIcon } from './Icons';

export default function Header({ title, subtitle }) {
  return (
    <header className="flex flex-col gap-4 border-b border-white/10 px-4 py-4 sm:px-6 lg:flex-row lg:items-center lg:justify-between">
      <div>
        <h1 className="text-2xl font-semibold text-white">{title}</h1>
        <p className="mt-1 text-sm text-slate-400">{subtitle}</p>
      </div>
      <div className="flex items-center gap-3">
        <label className="flex items-center gap-2 rounded-2xl border border-white/10 bg-white/5 px-3 py-2 text-sm text-slate-400">
          <SearchIcon className="h-4 w-4" />
          <input className="w-32 bg-transparent text-sm outline-none sm:w-48" placeholder="Search" />
        </label>
        <div className="flex items-center gap-3 rounded-2xl border border-cyan-500/20 bg-cyan-500/10 px-3 py-2 text-sm text-cyan-200">
          <div className="h-2.5 w-2.5 rounded-full bg-cyan-400" />
          Live workspace
        </div>
      </div>
    </header>
  );
}
