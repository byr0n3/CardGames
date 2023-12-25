/**
 * @param {String} value
 */
async function copyAsync(value) {
	try {
		await navigator.clipboard.writeText(value);
		// @todo Toast/notify client
	} catch (e) {
		console.error(`Error trying to copy '${value}' to clipboard:`, e);
		// @todo Toast/notify client
	}
}
