export default {
	redirects: {
		save: "Gem",
		addRedirect: "Tilføj redirect",
		addRedirectTitle: "Tilføj nyt redirect",
		editRedirectTitle: "Rediger redirect",
		reload: "Genindlæs",
		enabled: "Aktiveret",
		disabled: "Deaktiveret",
		temporary: "Midlertidig",
		permanent: "Permanent",
		content: "Indhold",
		media: "Medie",
		url: "URL",
		originalUrl: "Original URL",
		type: "Type",
		destination: "Destination",
		site: "Site",
		allSites: "Alle sites",
		globalRedirects: "Globale redirects",
		allTypes: "Alle typer"
	},
	redirectsTabs: {
		settings: "Indstillinger",
		info: "Info"
	},
	redirectsLabels: {
		advancedOptions: "Avancerede indstillinger",
		noRedirects: "Der er endnu ikke blevet tilføjet nogle redirects.",
		noSearchRedirects: "Din søgning matchede ingen redirects."
	},
	redirectsProperties: {
		site: "Site",
		siteDescription: "Vælg det site (eller rodnode) som redirectet skal gælde for. Vælges der ikke noget site, vil redirectet virke for alle domæner/sites i Umbraco-løsningen.",
		originalUrl: "Oprindelig URL",
		originalUrlDescription: "Angiv den oprindelige URL, der skal sende brugeren videre til den valgte destination.",
		destination: "Destination",
		destinationDescription: "Vælg den side eller URL, som brugeren skal sendes videre til.",
		destinationCulture: "Kultur",
		destinationCultureDescription: "Vælg den kultur, som brugeren skal sendes videre til.",
		redirectType: "Type",
		redirectTypeDescription: "Vælg typen for dit redirect. Bemærk at browsere vil huske et permanent redirect.",
		forwardQueryString: "Videresend query string",
		forwardQueryStringDescription: "Når slået til, tilføjes en evt. query string fra det oprindelige request til redirect destinationen.",
		id: "ID",
		key: "Key",
		createDate: "Oprettelsesdato",
		updateDate: "Sidst opdateret"
	},
	redirectsErrors: {
		redirectAlreadyExists: "Et redirect med den samme URL og query string findes allerede."
	}
}