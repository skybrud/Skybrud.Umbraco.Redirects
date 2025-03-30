function meh(count, singular, plural) {
	count = parseInt(count);
	if (count === 1) return `${count} ${singular}`;
	if (count > 1 || count === 0) return `${count} ${plural}`;
	return plural;
}

const fromNow = {
	ago: "ago",
	in: "in",
	day: "day",
	days: (c) => meh(c, "day", "days"),
	minute: "minute",
	minutes: (c) => meh(c, "minute", "minutes"),
	hour: "hour",
	hours: (c) => meh(c, "hour", "hours"),
	second: "second",
	seconds: (c) => meh(c, "second", "seconds"),
	now: "now",
	and: "and",
	na: "N/A",
	items: (c) => meh(c, "item", "items"),
	properties: (c) => meh(c, "property", "properties")
};

const misc = {
	save: "Save",
	addRedirect: "Add redirect",
	addRedirectTitle: "Add new redirect",
	editRedirectTitle: "Edit redirect",
	reload: "Reload",
	enabled: "Enabled",
	disabled: "Disabled",
	temporary: "Temporary",
	permanent: "Permanent",
	content: "Content",
	media: "Media",
	url: "URL",
	originalUrl: "Original URL",
	type: "Type",
	destination: "Destination",
	site: "Site",
	allSites: "All sites",
	globalRedirects: "Global redirects",
	allTypes: "All types",
	contentNotPublished: "The selected page isn't published. You can not create redirects for unpublished content.",
	deleted: "Deleted",
	trashed: "Trashed",
	unpublished: "Unpublished"
};

export default {
	redirects: Object.assign({}, misc, fromNow),
	redirectsTabs: {
		settings: "Settings",
		info: "Info"
	},
	redirectsLabels: {
		advancedOptions: "Advanced Options",
		noRedirects: "There has not yet been added any redirects.",
		noSearchRedirects: "Your search did not match any redirects."
	},
	redirectsProperties: {
		site: "Site",
		siteDescription: "Select the site (or root node) the redirect should apply to. If a site is not selected, the redirect will apply to all domains/sites in the Umbraco solution.",
		originalUrl: "Original URL",
		originalUrlDescription: "Specify the original URL to match from which the user should be redirected to the destination.",
		destination: "Destination",
		destinationDescription: "Select the page or URL the user should be redirected to.",
		originalUrlDescription: "Specify the original URL to match from which the user should be redirected to the destination.",
		destinationCulture: "Culture",
		destinationCultureDescription: "Select the culture of the destination.",
		redirectType: "Redirect type",
		redirectTypeDescription: "Select the type of the redirect. Notice that browsers will remember permanent redirects.",
		forwardQueryString: "Forward query string",
		forwardQueryStringDescription: "When enabled, the query string of the original request is forwarded to the redirect location (pass through).",
		id: "ID",
		key: "Key",
		createDate: "Created",
		updateDate: "Last updated"
	},
	redirectsErrors: {
		redirectAlreadyExists: "A redirect with the same URL and query string already exists."
	}
}