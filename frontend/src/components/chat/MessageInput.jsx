
import { useState } from "react";

export default function MessageInput({ onSend, loading }) {
    const [message, setMessage] = useState("");

    const send = () => {
        const text = message.trim();

        if (!text || loading) return;

        onSend(text);
        setMessage("");
    };

    const handleKeyDown = (e) => {
        if (e.key === "Enter" && !e.shiftKey) {
            e.preventDefault();
            send();
        }
    };

    return (
        <div className="border-t border-slate-700 p-4 bg-slate-900">
            <div className="flex gap-3">

                <textarea
                    rows={2}
                    value={message}
                    onChange={(e) => setMessage(e.target.value)}
                    onKeyDown={handleKeyDown}
                    placeholder="Type your message..."
                    className="flex-1 resize-none rounded-xl bg-slate-800 p-3 text-white outline-none focus:ring-2 focus:ring-cyan-500"
                />

                <button
                    onClick={send}
                    disabled={loading}
                    className="rounded-xl bg-cyan-500 px-6 py-3 font-semibold text-black hover:bg-cyan-400 disabled:opacity-50"
                >
                    {loading ? "..." : "Send"}
                </button>

            </div>

            <div className="mt-2 text-xs text-slate-500">
                Press <b>Enter</b> to send • <b>Shift + Enter</b> for a new line
            </div>
        </div>
    );
}