const DB_NAME = "SchemaForge";
const DB_VERSION = 1;
const STORE_NAME = "projects";

let _db = null;
let _pendingProject = null;
let _lifecyclePersistenceInitialized = false;
let _pendingPersistPromise = Promise.resolve();

function openDb() {
    if (_db) return Promise.resolve(_db);
    return new Promise((resolve, reject) => {
        const request = indexedDB.open(DB_NAME, DB_VERSION);
        request.onupgradeneeded = (e) => {
            const db = e.target.result;
            if (!db.objectStoreNames.contains(STORE_NAME)) {
                db.createObjectStore(STORE_NAME, { keyPath: "id" });
            }
        };
        request.onsuccess = () => { _db = request.result; resolve(_db); };
        request.onerror = () => reject(request.error);
    });
}

function tx(mode) {
    return openDb().then(db => {
        const transaction = db.transaction(STORE_NAME, mode);
        return transaction.objectStore(STORE_NAME);
    });
}

function promisify(request) {
    return new Promise((resolve, reject) => {
        request.onsuccess = () => resolve(request.result);
        request.onerror = () => reject(request.error);
    });
}

export async function listProjects() {
    const store = await tx("readonly");
    const all = await promisify(store.getAll());
    return all.map(p => ({
        id: p.id,
        name: p.name,
        tableCount: p.schema?.tables?.length ?? 0,
        modifiedAt: p.modifiedAt
    })).sort((a, b) => new Date(b.modifiedAt) - new Date(a.modifiedAt));
}

export async function loadProject(projectId) {
    const store = await tx("readonly");
    const project = await promisify(store.get(projectId));
    return project || null;
}

export async function saveProject(project) {
    _pendingProject = project;
    const store = await tx("readwrite");
    await promisify(store.put(project));
    return true;
}

export async function deleteProject(projectId) {
    if (_pendingProject?.id === projectId) {
        _pendingProject = null;
    }
    const store = await tx("readwrite");
    await promisify(store.delete(projectId));
    return true;
}

export async function getProjectCount() {
    const store = await tx("readonly");
    return await promisify(store.count());
}

async function persistPendingProject() {
    if (!_pendingProject) {
        return false;
    }

    const store = await tx("readwrite");
    await promisify(store.put(_pendingProject));
    return true;
}

function queuePendingProjectPersist() {
    _pendingPersistPromise = _pendingPersistPromise
        .catch(() => {})
        .then(() => persistPendingProject())
        .catch(() => false);

    return _pendingPersistPromise;
}

export function setPendingProject(project) {
    _pendingProject = project;
    return true;
}

export function initializeLifecyclePersistence() {
    if (_lifecyclePersistenceInitialized) {
        return true;
    }

    document.addEventListener("visibilitychange", () => {
        if (document.visibilityState === "hidden") {
            void queuePendingProjectPersist();
        }
    });

    window.addEventListener("pagehide", () => {
        void queuePendingProjectPersist();
    });

    _lifecyclePersistenceInitialized = true;
    return true;
}
