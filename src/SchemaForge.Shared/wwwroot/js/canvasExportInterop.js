export function exportSvg(svgSelector) {
    const svg = document.querySelector(svgSelector);
    if (!svg) return null;
    const clone = svg.cloneNode(true);
    // Remove Blazor attributes
    clone.querySelectorAll('[blazor\\:elementReference]').forEach(el => {
        el.removeAttribute('blazor:elementReference');
    });
    // Remove all event-related attributes
    for (const el of clone.querySelectorAll('*')) {
        const attrs = [...el.attributes];
        for (const attr of attrs) {
            if (attr.name.startsWith('_bl') || attr.name.startsWith('blazor')) {
                el.removeAttribute(attr.name);
            }
        }
    }
    const serializer = new XMLSerializer();
    return serializer.serializeToString(clone);
}

export function exportPng(svgSelector, scale) {
    return new Promise((resolve) => {
        const svgString = exportSvg(svgSelector);
        if (!svgString) { resolve(null); return; }

        const svg = document.querySelector(svgSelector);
        const bbox = svg.getBBox();
        const width = (bbox.width + bbox.x + 100) * (scale || 2);
        const height = (bbox.height + bbox.y + 100) * (scale || 2);

        const canvas = document.createElement('canvas');
        canvas.width = width;
        canvas.height = height;
        const ctx = canvas.getContext('2d');
        ctx.fillStyle = 'white';
        ctx.fillRect(0, 0, width, height);

        const img = new Image();
        const blob = new Blob([svgString], { type: 'image/svg+xml;charset=utf-8' });
        const url = URL.createObjectURL(blob);

        img.onload = () => {
            ctx.drawImage(img, 0, 0, width, height);
            URL.revokeObjectURL(url);
            resolve(canvas.toDataURL('image/png'));
        };
        img.onerror = () => { resolve(null); };
        img.src = url;
    });
}

export function downloadDataUrl(dataUrl, filename) {
    const a = document.createElement('a');
    a.href = dataUrl;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
}
