define(["sitecore", "jquery", "jqueryui"], function (_sc, $, ui) {
    var overlaySelectorDialog = _sc.Definitions.App.extend({
        initialized: function () {
            var app = this;
            var scale = 1;

            var itemUriString = _sc.Helpers.url.getQueryParameters(window.location.href)['fo'] || "";
            var itemPath = null;
            try {
                var itemUri = new URL(itemUriString);
                itemPath = itemUri.pathname;
                if (itemPath == "" || itemPath.indexOf("?") > -1) throw "Invalid URL";
            } catch (e) {
                // Doesn't support URL (IE and pretty much FF as well)
                var slashes = itemUriString.indexOf("//");
                var query = itemUriString.indexOf("?");
                if (slashes > -1) itemPath = itemUriString.substring(slashes, query > -1 ? query : itemUriString.length);
            }

            var mainImage = document.querySelector('[data-sc-id="MainImage"]');
            if (itemPath == null || itemPath == "") {
                alert("Couldn't parse item URL for your background image");
            } else {
                var itemUriSplit = itemPath.substring(2).split("/");
                var database = new _sc.Definitions.Data.Database(new _sc.Definitions.Data.DatabaseUri(itemUriSplit[0]));
                database.getItem(itemUriSplit[1], function (item) {
                    if (item == null) alert("Couldn't find background image item in database for unknown reason");
                    else {
                        var imgWidth = parseInt(item.Width);
                        var imgHeight = parseInt(item.Height);
                        if (imgWidth > imgHeight) {
                            scale = 500 / imgWidth;
                            mainImage.style.height = Math.round(scale * imgHeight) + "px";
                        } else {
                            scale = 500 / imgHeight;
                            mainImage.style.width = Math.round(scale * imgWidth) + "px";
                        }
                        
                        mainImage.style.backgroundImage = "url('" + item.$mediaurl.replace("thn=1", "") + "&w=500')";

                        var coords = _sc.Helpers.url.getQueryParameters(window.location.href)['coords'];
                        if (coords != null && coords != "") {
                            app.Coordinates.set('text', coords);
                            var coordsSplit = coords.split(",");
                            jQuery('[data-sc-id="OverlayImage"]').css({ "left": (parseInt(coordsSplit[0]) * scale) + "px", "top": (parseInt(coordsSplit[1]) * scale) + "px" });
                        }
                    }
                });
            }

            mainImage.style.height = "500px";
            mainImage.style.width = "500px";
            mainImage.style.backgroundSize = "cover";

            jQuery('[data-sc-id="OverlayImage"]').draggable({
                containment: '[data-sc-id="MainImage"]',
                scroll: false,
                start: function (e, ui) {
                    app.Coordinates.set('text', Math.round(ui.position.left / scale) + "," + Math.round(ui.position.top / scale));
                },
                drag: function (e, ui) {
                    app.Coordinates.set('text', Math.round(ui.position.left / scale) + "," + Math.round(ui.position.top / scale));
                },
                stop: function (e, ui) {
                    app.Coordinates.set('text', Math.round(ui.position.left / scale) + "," + Math.round(ui.position.top / scale));
                }
            });
        }
    });
    return overlaySelectorDialog;
});