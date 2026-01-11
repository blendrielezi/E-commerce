(function () {
    // Helper: set a cookie (name, value, days)
    function setCookie(name, value, days) {
        var expires = "";
        if (days) {
            var date = new Date();
            date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
            expires = "; expires=" + date.toUTCString();
        }
        document.cookie = name + "=" + (value || "") + expires + "; path=/";
    }

    // Hide the banner
    function hideBanner() {
        var banner = document.getElementById("cookie-consent-banner");
        if (banner) banner.style.display = "none";
    }

    // On save click
    document.addEventListener("DOMContentLoaded", function () {
        var btn = document.getElementById("cc-save-btn");
        if (!btn) return;

        btn.addEventListener("click", function () {
            var func = true;  // always true
            var analytics = document.getElementById("cc-analytics").checked;
            var marketing = document.getElementById("cc-marketing").checked;

            // POST to API
            fetch("/api/CookieConsent/Save", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "RequestVerificationToken": getAntiForgeryToken()
                },
                body: JSON.stringify({
                    functional: func,
                    analytics: analytics,
                    marketing: marketing
                })
            })
                .then(function (resp) {
                    if (!resp.ok) throw new Error("Network response was not ok");
                    return resp.json();
                })
                .then(function (json) {
                    // URL-encode JSON so it's safe in a cookie
                    var consentValue = encodeURIComponent(JSON.stringify({
                        functional: json.functional,
                        analytics: json.analytics,
                        marketing: json.marketing
                    }));
                    setCookie("UserCookieConsent", consentValue, 365);

                    hideBanner();
                })
                .catch(function (err) {
                    console.error("CookieConsent save failed:", err);
                });
        });
    });

    // Extract antiforgery token from the form
    function getAntiForgeryToken() {
        var el = document.querySelector('input[name="__RequestVerificationToken"]');
        return el ? el.value : "";
    }
})();
