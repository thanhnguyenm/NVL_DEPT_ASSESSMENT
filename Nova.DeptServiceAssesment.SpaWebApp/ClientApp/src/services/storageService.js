export const StorageService = {

    retrieve(key) {
        let item = sessionStorage.getItem(key);

        if (item && item !== 'undefined') {
            return JSON.parse(sessionStorage.getItem(key));
        }

        return;
    },

    store(key, value) {
        sessionStorage.setItem(key, JSON.stringify(value));
    }
}
