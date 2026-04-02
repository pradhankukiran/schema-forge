export function downloadText(filename, content, mimeType) {
    const blob = new Blob([content], { type: mimeType || "text/plain" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
}

export function downloadJson(filename, obj) {
    const json = JSON.stringify(obj, null, 2);
    downloadText(filename, json, "application/json");
}

export async function copyToClipboard(text) {
    await navigator.clipboard.writeText(text);
    return true;
}
