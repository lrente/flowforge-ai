import { useEffect, useRef } from "react";
import MessageBubble from "./MessageBubble";

export default function ChatWindow({
    conversation,
    loading
}) {
    const bottomRef = useRef(null);

    useEffect(() => {
        bottomRef.current?.scrollIntoView({
            behavior: "smooth"
        });
    }, [conversation, loading]);

    if (!conversation) {
        return (
            <div className="flex h-full items-center justify-center text-slate-500">
                Select a conversation or start a new one.
            </div>
        );
    }

    return (
        <div className="flex-1 overflow-y-auto bg-slate-950 p-6">

            <h2 className="mb-6 text-2xl font-semibold text-white">
                {conversation.title}
            </h2>

            {conversation.messages.map((message, index) => (

                <MessageBubble
                    key={index}
                    role={message.role}
                    content={message.content}
                />

            ))}

            {loading && (

                <MessageBubble
                    role="assistant"
                    content="Thinking..."
                />

            )}

            <div ref={bottomRef} />

        </div>
    );
}