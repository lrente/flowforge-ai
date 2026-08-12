import { useEffect, useState } from 'react';
import api from '../api';
import { useAuth } from '../auth';
export default function AuditLogPage() { const { user } = useAuth(); const [items, setItems] = useState([]); useEffect(() => { if (user?.client?.id) api.get(`/clients/${user.client.id}/audit-logs`).then(r => setItems(r.data)); }, [user]); return <section><h1 className="text-3xl font-bold text-white">Audit log</h1><div className="mt-6 rounded-3xl border border-white/10 bg-slate-900/70 p-5">{items.map(i => <div className="border-b border-white/10 py-3 text-slate-300" key={i.id}>{new Date(i.createdAt).toLocaleString()} · {i.action} · {i.entityType}</div>)}{!items.length && <p className="text-slate-400">No audit entries yet.</p>}</div></section>; }
