const APP_VERSION_KEY = "app_version";

/**
 * Returns the app version currently stored in localStorage, or null.
 * @returns {string | null}
 */
export function getStoredVersion() {
    return localStorage.getItem(APP_VERSION_KEY);
}

/**
 * Persists the given version string in localStorage.
 * @param {string} version
 */
export function setStoredVersion(version) {
    localStorage.setItem(APP_VERSION_KEY, version);
}

/**
 * Removes every localStorage key that is NOT related to MSAL authentication.
 * Preserved key patterns:
 *   - Keys starting with "msal."
 *   - Keys containing "login.windows.net" or "login.microsoftonline.com"
 *   - The app_version key itself
 */
export function clearNonMsalStorage() {
    const keysToRemove = [];

    for (let i = 0; i < localStorage.length; i++) {
        const key = localStorage.key(i);
        if (key === null) continue;

        const isMsal =
            key.startsWith("msal.") ||
            key.includes("login.windows.net") ||
            key.includes("login.microsoftonline.com");

        if (!isMsal && key !== APP_VERSION_KEY) {
            keysToRemove.push(key);
        }
    }

    for (const key of keysToRemove) {
        localStorage.removeItem(key);
    }
}
