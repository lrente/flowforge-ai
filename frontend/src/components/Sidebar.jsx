import { NavLink } from 'react-router-dom';
import { AgentsIcon, ArrowRightIcon, ConversationIcon, DashboardIcon, KnowledgeIcon, SettingsIcon, SparklesIcon } from './Icons';
import { useAuth } from '../auth';

const navItems = [
  { to: '/', label: 'Dashboard', icon: DashboardIcon },
  { to: '/agents', label: 'Agents', icon: AgentsIcon },
  { to: '/knowledge', label: 'Knowledge', icon: KnowledgeIcon },
  { to: '/conversations', label: 'Conversations', icon: ConversationIcon },
  { to: '/users', label: 'Users', icon: AgentsIcon, permission: 'Users.View' },
  { to: '/audit-logs', label: 'Audit Log', icon: ConversationIcon, permission: 'AuditLogs.View' },
  { to: '/settings', label: 'Settings', icon: SettingsIcon, permission: 'Client.Settings.View' }
];

export default function Sidebar({ collapsed, onToggle }) {
  const { can } = useAuth();
  return (
    <aside className={`hidden border-r border-white/10 bg-slate-950/85 backdrop-blur-xl lg:flex lg:flex-col ${collapsed ? 'w-20' : 'w-72'}`}>
      <div className="flex items-center justify-between border-b border-white/10 px-5 py-5">
        <div className="flex items-center gap-3">
          <div className="flex h-10 w-10 items-center justify-center rounded-2xl bg-cyan-500/15 text-cyan-300">
            <SparklesIcon className="h-5 w-5" />
          </div>
          {!collapsed ? <div><p className="text-sm font-semibold text-white">FlowForge AI</p><p className="text-xs text-slate-400">Studio</p></div> : null}
        </div>
        <button onClick={onToggle} className="rounded-full border border-white/10 p-2 text-slate-400 transition hover:bg-white/5 hover:text-white">
          <ArrowRightIcon className={`h-4 w-4 transition ${collapsed ? 'rotate-180' : ''}`} />
        </button>
      </div>

      <nav className="flex-1 space-y-1 px-3 py-4">
        {navItems.filter((item) => !item.permission || can(item.permission)).map((item) => {
          const Icon = item.icon;
          return (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) => `flex items-center gap-3 rounded-2xl px-3 py-3 text-sm font-medium transition ${isActive ? 'bg-cyan-500/15 text-cyan-200 shadow-[inset_0_0_0_1px_rgba(34,211,238,0.2)]' : 'text-slate-400 hover:bg-white/5 hover:text-white'}`}
            >
              <Icon className="h-5 w-5" />
              {!collapsed ? <span>{item.label}</span> : null}
            </NavLink>
          );
        })}
      </nav>

      <div className="border-t border-white/10 p-4">
        <div className={`rounded-2xl border border-white/10 bg-white/5 p-3 ${collapsed ? 'text-center' : ''}`}>
          <p className="text-sm font-semibold text-white">Upgrade</p>
          {!collapsed ? <p className="mt-1 text-xs text-slate-400">Unlock unlimited agents and shared workspaces.</p> : null}
        </div>
      </div>
    </aside>
  );
}
