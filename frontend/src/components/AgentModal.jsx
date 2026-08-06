import { useEffect, useState } from "react";

const emptyAgent = {
    name: "",
    businessType: "",
    description: "",
    companyName: "",
    systemPrompt: "",
    model: "gpt-4.1-mini",
    temperature: 0.7,
    isActive: true
};

export default function AgentModal({
    open,
    onClose,
    onSave,
    editing
}) {
    const [form, setForm] = useState(emptyAgent);

    useEffect(() => {
        setForm(editing ?? emptyAgent);
    }, [editing]);

    if (!open)
        return null;

    function change(e) {
        const { name, value, checked, type } = e.target;

        setForm({
            ...form,
            [name]:
                type === "checkbox"
                    ? checked
                    : type === "number"
                    ? Number(value)
                    : value
        });
    }

    return (
        <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50">

            <div className="w-full max-w-3xl rounded-3xl bg-slate-900 p-8">

                <h2 className="text-2xl font-bold text-white mb-6">
                    {editing ? "Edit Agent" : "New Agent"}
                </h2>

                <div className="grid gap-4">

                    <input
                        name="name"
                        value={form.name}
                        onChange={change}
                        placeholder="Agent name"
                        className="rounded-xl bg-slate-800 p-3"
                    />

                    <input
                        name="businessType"
                        value={form.businessType}
                        onChange={change}
                        placeholder="Business"
                        className="rounded-xl bg-slate-800 p-3"
                    />

                    <textarea
                        rows={3}
                        name="description"
                        value={form.description}
                        onChange={change}
                        placeholder="Description"
                        className="rounded-xl bg-slate-800 p-3"
                    />

                    <textarea
                        rows={6}
                        name="systemPrompt"
                        value={form.systemPrompt}
                        onChange={change}
                        placeholder="System Prompt"
                        className="rounded-xl bg-slate-800 p-3"
                    />

                    <div className="flex justify-end gap-3">

                        <button
                            onClick={onClose}
                            className="rounded-xl bg-slate-700 px-5 py-3"
                        >
                            Cancel
                        </button>

                        <button
                            onClick={() => onSave(form)}
                            className="rounded-xl bg-cyan-500 px-5 py-3 text-black font-semibold"
                        >
                            Save
                        </button>

                    </div>

                </div>

            </div>

        </div>
    );
}