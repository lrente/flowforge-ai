export default function DocumentCard({ document, onDelete }) {
    return (
        <div className="flex justify-between items-center rounded-2xl bg-slate-900 p-5 border border-slate-700">

            <div>

                <div className="font-semibold">
                    {document.fileName}
                </div>

                <div className="text-sm text-slate-400">
                    {document.status}
                </div>

            </div>

            <button
                onClick={() => onDelete(document.id)}
                className="text-red-400 hover:text-red-300"
            >
                Delete
            </button>

        </div>
    );
}