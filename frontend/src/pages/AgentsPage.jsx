import { useState } from "react";
import Header from "../components/Header";
import AgentCard from "../components/AgentCard";
import AgentModal from "../components/AgentModal";

export default function AgentsPage() {
    const [agents, setAgents] = useState([]);
    const [modalOpen, setModalOpen] = useState(false);
    const [editingAgent, setEditingAgent] = useState(null);

    const editAgent = (agent) => {
        setEditingAgent(agent);
        setModalOpen(true);
    };

    const deleteAgent = async (id) => {
        // TODO
    };

    const saveAgent = async (agent) => {
        // TODO
    };

    const closeModal = () => {
        setEditingAgent(null);
        setModalOpen(false);
    };

    return (
        <>
            <Header
                title="Agents"
                subtitle="Manage your AI assistants."
            />

            <div className="flex justify-between mb-8">
                <input
                    placeholder="Search agents..."
                    className="w-80 rounded-xl bg-slate-800 p-3"
                />

                <button
                    onClick={() => setModalOpen(true)}
                    className="rounded-xl bg-cyan-500 px-6 py-3 text-black font-semibold"
                >
                    + New Agent
                </button>
            </div>

            <div className="grid gap-6">
                {agents.map(agent => (
                    <AgentCard
                        key={agent.id}
                        agent={agent}
                        onEdit={editAgent}
                        onDelete={deleteAgent}
                    />
                ))}
            </div>

            <AgentModal
                open={modalOpen}
                editing={editingAgent}
                onClose={closeModal}
                onSave={saveAgent}
            />
        </>
    );
}