import { RedirectsAuth } from "@skybrud-redirects/auth";

const baseUrl = "/umbraco/management/api/v1/skybrud/redirects";

function hi(url, config) {

    // TODO: simplify this code to use async/await instead of promises

    if (!config) config = {};
    if (!config.method) config.method = "GET";
    if (!config.headers) config.headers = {};

    return new Promise((resolve, reject) => {

        RedirectsAuth.TOKEN().then(function (token) {

            config.headers.Authorization = "Bearer " + token;

            //console.log(config.method + " " + url);

            const response = fetch(url, config);

            response.then(function (res) {

                res.json().then(function (json) {
                    res.data = json;
                    if (res.status < 400) {
                        resolve(res);
                    } else {
                        reject(res);
                    }
                });

            }, function (res) {

                // sending the request failed (before actually calling the URL)

                console.log("failed", arguments);

            });

        });

    });

}

function get(url) {
    return hi(url);
}

function patch(url, config) {
    if (!config) config = {};
    config.method = "PATCH";
    return hi(url, config);
}

function patchJson(url, body, config) {
    if (!config) config = {};
    if (!config.headers) config.headers = {};
    config.headers["Content-Type"] = "application/json";
    config.body = JSON.stringify(body);
    return patch(url, config);
}

function put(url, config) {
    if (!config) config = {};
    config.method = "PUT";
    return hi(url, config);
}

function putJson(url, body, config) {
    if (!config) config = {};
    if (!config.headers) config.headers = {};
    config.headers["Content-Type"] = "application/json";
    config.body = JSON.stringify(body);
    return put(url, config);
}

function _delete(url, config) {
    if (!config) config = {};
    config.method = "DELETE";
    return hi(url, config);
}

export class RedirectsService {

    static getRedirects(query) {
        return get(baseUrl + (query ? "?" + new URLSearchParams(query) : ""));
    }

    static addRedirect(redirect) {
        if (!redirect) return;
        return putJson(baseUrl, redirect);
    }

    static saveRedirect(redirect) {
        if (!redirect) return;
        if (!redirect.key) return;
        return patchJson(`${baseUrl}/${redirect.key}`, redirect);
    }

    static deleteRedirect(redirect) {
        if (!redirect) return;
        if (redirect.key) redirect = redirect.key;
        if (typeof redirect !== "string") return;
        return _delete(`${baseUrl}/${redirect}`);
    }

    static getContent(key) {

        return new Promise((resolve, reject) => {

            get("/umbraco/management/api/v1/document/" + key).then(function (res1) {

                get("/umbraco/management/api/v1/document/urls?id=" + key).then(function (res2) {
                    res1.data.urls = res2.data[0].urlInfos.map(x => x.url);
                    res1.data.url = res1.data.urls[0];
                    resolve(res1);
                });

            })

        });

    }

    static getCultures(key) {
        return get(`${baseUrl}/content/${key}/cultures`);
    }

    static getMedia(key) {

        return new Promise((resolve, reject) => {

            get("/umbraco/management/api/v1/media/" + key).then(function (res1) {

                get("/umbraco/management/api/v1/media/urls?id=" + key).then(function (res2) {
                    res1.data.urls = res2.data[0].urlInfos.map(x => x.url);
                    res1.data.url = res1.data.urls[0];
                    resolve(res1);
                });

            })

        });

    }

    static getRootNodes() {
        return get(`${baseUrl}/rootNodes`);
    }

    static getServerVariables() {
        return get(`${baseUrl}/serverVariables`);
    }

    static getRedirectsForNode(type, key) {
        return get(`${baseUrl}/${type}/${key}`);
    }

    static async getCurrentUser() {
        const response = await get(`${baseUrl}/users/current`);
        return response.data;
    }

}

export default RedirectsService;