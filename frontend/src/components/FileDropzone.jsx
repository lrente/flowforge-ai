import { useDropzone } from "react-dropzone";

export default function FileDropzone({ file, setFile }) {
    const { getRootProps, getInputProps, isDragActive } = useDropzone({
        multiple: false,
        accept: {
            "application/pdf": [".pdf"],
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document": [".docx"],
            "text/plain": [".txt"]
        },
        onDrop: acceptedFiles => {
            if (acceptedFiles.length)
                setFile(acceptedFiles[0]);
        }
    });

    return (
        <div
            {...getRootProps()}
            className={`border-2 border-dashed rounded-3xl p-12 text-center cursor-pointer transition
            ${
                isDragActive
                    ? "border-cyan-400 bg-cyan-500/10"
                    : "border-slate-700 bg-slate-900/50"
            }`}
        >
            <input {...getInputProps()} />

            <div className="space-y-3">

                <div className="text-5xl">
                    📄
                </div>

                <h3 className="text-xl font-semibold">
                    Drag & Drop your knowledge files
                </h3>

                <p className="text-slate-400">
                    PDF • DOCX • TXT
                </p>

                {file && (
                    <div className="mt-6 rounded-xl bg-slate-800 p-4">
                        <strong>{file.name}</strong>

                        <div className="text-slate-400 text-sm">
                            {(file.size / 1024).toFixed(1)} KB
                        </div>
                    </div>
                )}

            </div>

        </div>
    );
}