function meh(count, singular, plural) {
    count = parseInt(count);

    if (count === 1) return `${count} ${singular}`;
    if (count > 1 || count === 0) return `${count} ${plural}`;

    return plural;
}

const fromNow = {
    ago: "sedan",
    in: "om",
    day: "dag",
    days: (c) => meh(c, "dag", "dagar"),
    minute: "minut",
    minutes: (c) => meh(c, "minut", "minuter"),
    hour: "timme",
    hours: (c) => meh(c, "timme", "timmar"),
    second: "sekund",
    seconds: (c) => meh(c, "sekund", "sekunder"),
    now: "nu",
    and: "och",
    na: "N/A",
    items: (c) => meh(c, "objekt", "objekt"),
    properties: (c) => meh(c, "egenskap", "egenskaper")
};

const misc = {
    save: "Spara",
    addRedirect: "Lägg till redirect",
    addRedirectTitle: "Lägg till ny redirect",
    editRedirectTitle: "Redigera redirect",
    deleteRedirectTitle: "Ta bort redirect",
    deleteRedirectMessage: "Är du säker på att du vill ta bort denna redirect?",
    addRedirectSuccess: "Ny redirect har lagts till",
    editRedirectSuccess: "Redirecten har uppdaterats",
    deleteRedirectSuccess: "Redirecten har tagits bort",
    deleteRedirectFailed: "Det gick inte att ta bort redirecten",
    reload: "Ladda om",

    enabled: "På",
    disabled: "Av",

    temporary: "Temporär",
    permanent: "Permanent",
    content: "Innehåll",
    media: "Media",
    url: "URL",
    originalUrl: "Ursprunglig URL",
    type: "Typ",
    destination: "Destination",
    site: "Webbplats",
    allSites: "Alla webbplatser",
    globalRedirects: "Globala redirects",
    allTypes: "Alla typer",
    contentNotPublished: "Den valda sidan är inte publicerad.\nDu kan inte skapa redirects till opublicerat innehåll.",
    deleted: "Borttagen",
    trashed: "I papperskorgen",
    unpublished: "Opublicerad",
    pageExistsAtUrl: "Det finns redan en sida med den här URL:en.",
    mediaExistsAtUrl: "Det finns redan ett mediaobjekt med den här URL:en."
};

export default {
    redirects: Object.assign({}, misc, fromNow),

    redirectsTabs: {
        settings: "Inställningar",
        info: "Info"
    },

    redirectsLabels: {
        advancedOptions: "Avancerade inställningar",
        noRedirects: "Inga redirects har lagts till ännu.",
        noSearchRedirects: "Din sökning matchade inga redirects."
    },

    redirectsProperties: {
        site: "Webbplats",
        siteDescription: "Välj vilken webbplats eller rotnod redirecten ska gälla för.\nOm ingen webbplats väljs gäller redirecten för alla domäner och webbplatser i Umbraco-lösningen.",

        originalUrl: "Ursprunglig URL",
        originalUrlDescription: "Ange den ursprungliga URL:en som ska matchas och skicka besökaren vidare till destinationen.",

        destination: "Destination",
        destinationDescription: "Välj sida eller URL som besökaren ska skickas vidare till.",

        destinationCulture: "Kultur",
        destinationCultureDescription: "Välj kultur för destinationen.",

        redirectType: "Redirecttyp",
        redirectTypeDescription: "Välj typ av redirect.\nObservera att webbläsare kommer ihåg permanenta redirects.",

        forwardQueryString: "Vidarebefordra query string",
        forwardQueryStringDescription: "När detta är aktiverat skickas query string från den ursprungliga URL:en vidare till redirectens destination.",

        id: "ID",
        key: "Nyckel",
        createDate: "Skapad",
        updateDate: "Senast uppdaterad"
    },

    redirectsErrors: {
        validationErrors: "Valideringsfel",
        fieldRequired: "Fältet <strong>%0%</strong> är obligatoriskt.",
        fieldInvalid: "Fältet <strong>%0%</strong> är ogiltigt.",
        redirectAlreadyExists: "Det finns redan en redirect med samma URL och query string."
    }
};
