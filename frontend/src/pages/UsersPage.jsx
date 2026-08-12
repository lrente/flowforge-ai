import { useEffect, useState } from 'react';
import api from '../api';
import { useAuth } from '../auth';

export default function UsersPage() {
  const { user, can } = useAuth(); const [users, setUsers] = useState([]); const [email, setEmail] = useState(''); const [role, setRole] = useState('Member'); const [message, setMessage] = useState('');
  const allowedRoles = user?.role === 'Admin' ? ['Admin', 'Editor', 'Member', 'Guest'] : ['Member', 'Guest'];
  const load = () => user?.client?.id && api.get(`/clients/${user.client.id}/users`).then(r => setUsers(r.data)).catch(() => setMessage('Unable to load users.'));
  useEffect(load, [user]);
  const invite = async (event) => { event.preventDefault(); try { await api.post(`/clients/${user.client.id}/users/invite`, { email, role }); setMessage('Invitation created.'); setEmail(''); } catch (error) { setMessage(error.response?.data?.message || 'Unable to send invitation.'); } };
  return <section className="space-y-6"><div><p className="text-sm text-cyan-300">Workspace</p><h1 className="text-3xl font-bold text-white">Users</h1></div>{can('Users.Create') && <form onSubmit={invite} className="flex flex-wrap gap-3 rounded-3xl border border-white/10 bg-slate-900/70 p-5"><input required type="email" value={email} onChange={e => setEmail(e.target.value)} placeholder="Email address" className="rounded-xl bg-slate-800 px-4 py-2 text-white" /><select value={role} onChange={e => setRole(e.target.value)} className="rounded-xl bg-slate-800 px-4 py-2 text-white">{allowedRoles.map(r => <option key={r}>{r}</option>)}</select><button className="rounded-xl bg-cyan-500 px-4 py-2 font-medium text-slate-950">Send invitation</button></form>}{message && <p className="text-sm text-slate-300">{message}</p>}<div className="rounded-3xl border border-white/10 bg-slate-900/70 p-5">{users.length ? users.map(u => <div className="border-b border-white/10 py-4 text-slate-200" key={u.userId}>{u.name} <span className="text-slate-400">{u.email} · {u.role}</span></div>) : <p className="text-slate-400">No users found.</p>}</div></section>;
}
