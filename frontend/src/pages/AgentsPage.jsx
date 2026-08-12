import { useEffect, useState } from "react";
import Header from "../components/Header";
import AgentCard from "../components/AgentCard";
import AgentModal from "../components/AgentModal";

const API_URL = "http://localhost:5152/api/agents";

export default function AgentsPage() {
    const [agents, setAgents] = useState([]);
    const [filteredAgents, setFilteredAgents] = useState([]);
    const [search, setSearch] = useState("");

    const [modalOpen, setModalOpen] = useState(false);
    const [editingAgent, setEditingAgent] = useState(null);

    const [loading, setLoading] = useState(true);

    useEffect(() => {
        loadAgents();
    }, []);

    useEffect(() => {
        if (!search) {
            setFilteredAgents(agents);
            return;
        }

        const text = search.toLowerCase();

        setFilteredAgents(
            agents.filter(a =>
                a.name.toLowerCase().includes(text) ||
                (a.businessType ?? "").toLowerCase().includes(text)
            )
        );
    }, [search, agents]);

    async function loadAgents() {
        try {
            setLoading(true);

            const token = localStorage.getItem("token");

            const response = await fetch(API_URL, {
                headers: {
                    Authorization: `Bearer ${token}`
                }
            });

            if (!response.ok)
                throw new Error("Unable to load agents.");

            const data = await response.json();

            setAgents(data);
            setFilteredAgents(data);
        }
        catch (err) {
            console.error(err);
            alert("Failed to load agents.");
        }
        finally {
            setLoading(false);
        }
    }

    function editAgent(agent) {
        setEditingAgent(agent);
        setModalOpen(true);
    }

    async function deleteAgent(id) {

        if (!window.confirm("Delete this agent?"))
            return;

        try {

            const token = localStorage.getItem("token");

            const response = await fetch(`${API_URL}/${id}`, {
                method: "DELETE",
                headers: {
                    Authorization: `Bearer ${token}`
                }
            });

            if (!response.ok)
                throw new Error();

            await loadAgents();

        } catch (err) {

            console.error(err);
            alert("Failed to delete agent.");

        }

    }

    async function saveAgent(agent) {

        try {

            const token = localStorage.getItem("token");

            const response = await fetch(
                editingAgent
                    ? `${API_URL}/${editingAgent.id}`
                    : API_URL,
                {
                    method: editingAgent ? "PUT" : "POST",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${token}`
                    },
                    body: JSON.stringify(agent)
                });

            if (!response.ok) {

                const error = await response.text();

                console.error(error);

                alert(error);

                return;
            }

            closeModal();

            await loadAgents();

        }
        catch (err) {

            console.error(err);

            alert("Failed to save agent.");

        }

    }

    function closeModal() {
        setEditingAgent(null);
        setModalOpen(false);
    }

    return (
        <>
            <Header
                title="Agents"
                subtitle="Manage your AI assistants."
            />

            <div className="mb-8 flex justify-between gap-4">

                <input
                    value={search}
                    onChange={(e) => setSearch(e.target.value)}
                    placeholder="Search agents..."
                    className="w-80 rounded-xl bg-slate-800 p-3 text-white outline-none"
                />

                <button
                    onClick={() => {
                        setEditingAgent(null);
                        setModalOpen(true);
                    }}
                    className="rounded-xl bg-cyan-500 px-6 py-3 font-semibold text-black hover:bg-cyan-400"
                >
                    + New Agent
                </button>

            </div>

            {loading ? (

                <div className="py-20 text-center text-slate-400">
                    Loading agents...
                </div>

            ) : filteredAgents.length === 0 ? (

                <div className="rounded-2xl border border-slate-800 bg-slate-900 p-12 text-center">

                    <h2 className="mb-2 text-xl font-semibold text-white">
                        No agents found
                    </h2>

                    <p className="text-slate-400">
                        Create your first AI agent.
                    </p>

                </div>

            ) : (

                <div className="grid gap-6 md:grid-cols-2 xl:grid-cols-3">

                    {filteredAgents.map(agent => (

                        <AgentCard
                            key={agent.id}
                            agent={agent}
                            onEdit={editAgent}
                            onDelete={deleteAgent}
                        />

                    ))}

                </div>

            )}

            <AgentModal
                open={modalOpen}
                editing={editingAgent}
                onClose={closeModal}
                onSave={saveAgent}
            />
        </>
    );
}