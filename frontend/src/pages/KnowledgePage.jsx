import { useEffect, useState } from "react";
import Header from "../components/Header";
import api from "../api";

export default function KnowledgePage() {
    const [agents, setAgents] = useState([]);
    const [documents, setDocuments] = useState([]);
    const [selectedAgent, setSelectedAgent] = useState("");
    const [file, setFile] = useState(null);
    const [progress, setProgress] = useState(0);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        loadAgents();
        loadDocuments();
    }, []);

    async function loadAgents() {
        try {
            const res = await api.get("/Agent");
            setAgents(res.data);

            if (res.data.length > 0)
                setSelectedAgent(res.data[0].id);

        } catch (err) {
            console.error(err);
        }
    }

    async function loadDocuments() {
        try {
            const res = await api.get("/Knowledge");
            setDocuments(res.data);
        } catch (err) {
            console.error(err);
        }
    }

    async function upload() {

        if (!selectedAgent) {
            alert("Select an agent.");
            return;
        }

        if (!file) {
            alert("Choose a file.");
            return;
        }

        const formData = new FormData();

        formData.append("AgentId", selectedAgent);
        formData.append("File", file);

        setLoading(true);

        try {

            await api.post("/Knowledge/upload", formData, {

                onUploadProgress: e => {

                    if (e.total) {
                        setProgress(
                            Math.round((e.loaded * 100) / e.total)
                        );
                    }

                }

            });

            setProgress(0);
            setFile(null);

            await loadDocuments();

            alert("Document uploaded.");

        }
        catch (err) {
            console.error(err);
            alert("Upload failed.");
        }

        setLoading(false);
    }

    async function deleteDocument(id) {

        if (!window.confirm("Delete this document?"))
            return;

        await api.delete(`/Knowledge/${id}`);

        loadDocuments();

    }

    return (

        <>
            <Header
                title="Knowledge"
                subtitle="Upload documents to your AI agents."
            />

            <div className="rounded-3xl bg-slate-900 p-8">

                <div className="space-y-5">

                    <select
                        value={selectedAgent}
                        onChange={(e) => setSelectedAgent(e.target.value)}
                        className="w-full rounded-xl bg-slate-800 p-3"
                    >
                        {agents.map(agent => (
                            <option
                                key={agent.id}
                                value={agent.id}
                            >
                                {agent.name}
                            </option>
                        ))}
                    </select>

                    <input
                        type="file"
                        accept=".pdf,.doc,.docx,.txt"
                        onChange={(e) => setFile(e.target.files[0])}
                    />

                    <button
                        onClick={upload}
                        disabled={loading}
                        className="rounded-xl bg-cyan-500 px-6 py-3 text-black font-semibold"
                    >
                        Upload Document
                    </button>

                    {progress > 0 && (

                        <div>

                            <div className="mb-2">
                                Uploading {progress}%
                            </div>

                            <div className="h-3 rounded-full bg-slate-700">

                                <div
                                    className="h-3 rounded-full bg-cyan-500"
                                    style={{
                                        width: `${progress}%`
                                    }}
                                />

                            </div>

                        </div>

                    )}

                </div>

            </div>

            <div className="mt-8 rounded-3xl bg-slate-900 p-8">

                <h2 className="mb-6 text-xl font-semibold">
                    Documents
                </h2>

                <div className="space-y-3">

                    {documents.map(doc => (

                        <div
                            key={doc.id}
                            className="flex items-center justify-between rounded-xl bg-slate-800 p-4"
                        >

                            <div>

                                <div className="font-semibold">
                                    {doc.fileName}
                                </div>

                                <div className="text-sm text-slate-400">
                                    {doc.status}
                                </div>

                            </div>

                            <button
                                onClick={() => deleteDocument(doc.id)}
                                className="rounded bg-red-500 px-4 py-2"
                            >
                                Delete
                            </button>

                        </div>

                    ))}

                </div>

            </div>

        </>

    );
}