/* This code comes from http://trac.sitecore.net/GoogleMaps
and has been modified to match XAContext JS Framework guidelines */
/*global google:false */
XA.component.GMap = (function($, document) {
    var api = {},
        mapData = [],
        map = [],
        mapLoaded = false;

    function pushMap(map) {
        mapData.push(map);
    }

    // initializes the Google Maps by translating the serialized Sitecore 
    // items into Google Maps objects
    api.initialize = function() {
        // extend the google maps object to add a pointer to the current info window
        // and a function to close the current info window
        google.maps.Map.prototype.latestInfoWindow;
        google.maps.Map.prototype.closeInfoWindow = function() {
            if (this.latestInfoWindow != undefined) {
                this.latestInfoWindow.close();
            }

        };

        // initialize all maps on the page
        for (var mapCount = 0; mapCount < mapData.length; mapCount++) {
            // figure out the initial map type (first one in the list or default to a roadmap if nothing's defined)
            var allowedMapTypes = new Array(),
                initialMapType;
            if (mapData[mapCount].MapTypes.length > 0) {
                initialMapType = eval(mapData[mapCount].MapTypes[0]);
                for (var k = 0; k < mapData[mapCount].MapTypes.length; k++) {
                    allowedMapTypes.push(eval(mapData[mapCount].MapTypes[k]));
                }
            } else {
                initialMapType = google.maps.MapTypeId.ROADMAP;
            }

            // initial map options
            var mapOptions = {
                center: new google.maps.LatLng(mapData[mapCount].Center.Latitude, mapData[mapCount].Center.Longitude),
                disableDefaultUI: mapData[mapCount].DisableAllDefaultUIElements,
                disableDoubleClickZoom: !mapData[mapCount].EnableDoubleClickZoom,
                draggable: mapData[mapCount].EnableDragging,
                draggableCursor: mapData[mapCount].DraggableCursor,
                draggingCursor: mapData[mapCount].DraggingCursor,
                keyboardShortcuts: mapData[mapCount].EnableKeyboardFunctionality,
                mapTypeControlOptions: {
                    mapTypeIds: allowedMapTypes
                },
                mapTypeId: initialMapType,
                maxZoom: mapData[mapCount].MaxZoomLevel,
                minZoom: mapData[mapCount].MinZoomLevel,
                overviewMapControl: mapData[mapCount].EnableOverview,
                overviewMapControlOptions: {
                    opened: mapData[mapCount].EnableOverview
                },
                panControl: mapData[mapCount].EnablePanControl,
                scaleControl: mapData[mapCount].EnableScaleControl,
                scrollwheel: mapData[mapCount].EnableScrollWheelZoom,
                streetViewControl: mapData[mapCount].EnableStreetViewControl,
                zoom: mapData[mapCount].Zoom,
                zoomControl: mapData[mapCount].EnableZoomControl
            };

            // initialize the map
            map.push(new google.maps.Map(document.getElementById(mapData[mapCount].CanvasID), mapOptions));

            // add marker, lines and polygons as arrays on the map object to support multiple maps per page
            map[mapCount].markers = [];
            map[mapCount].lines = [];
            map[mapCount].polygons = [];


            // close any open info windows when clicking on the map
            google.maps.event.addListener(map[mapCount], 'click', function() {
                this.closeInfoWindow();
            });

            // initialize all markers        
            for (var i = 0; i < mapData[mapCount].Markers.length; i++) {
                var m = new google.maps.Marker({
                    position: new google.maps.LatLng(mapData[mapCount].Markers[i].Position.Latitude, mapData[mapCount].Markers[i].Position.Longitude),
                    map: map[mapCount],
                    title: mapData[mapCount].Markers[i].Title
                });

                m.infowindow = new google.maps.InfoWindow({
                    content: mapData[mapCount].Markers[i].InfoWindow
                });

                map[mapCount].markers.push(m);

                if (mapData[mapCount].Markers[i].InfoWindow.length > 0) {
                    google.maps.event.addListener(m, 'click', function() {
                        this.map.closeInfoWindow();
                        this.infowindow.open(this.map, this);
                        this.map.latestInfoWindow = this.infowindow;
                    });
                }
                if (mapData[mapCount].Markers[i].CustomIcon != undefined) {
                    m.icon = new google.maps.MarkerImage(mapData[mapCount].Markers[i].CustomIcon.ImageURL,
                        new google.maps.Size(mapData[mapCount].Markers[i].CustomIcon.ImageDimensions.Width, mapData[mapCount].Markers[i].CustomIcon.ImageDimensions.Height),
                        new google.maps.Point(0, 0),
                        new google.maps.Point(mapData[mapCount].Markers[i].CustomIcon.Anchor.X, mapData[mapCount].Markers[i].CustomIcon.Anchor.Y));

                    if (mapData[mapCount].Markers[i].CustomIcon.ShadowURL != undefined) {
                        m.shadow = new google.maps.MarkerImage(mapData[mapCount].Markers[i].CustomIcon.ShadowURL,
                            new google.maps.Size(mapData[mapCount].Markers[i].CustomIcon.ShadowDimensions.Width, mapData[mapCount].Markers[i].CustomIcon.ShadowDimensions.Height),
                            new google.maps.Point(0, 0),
                            new google.maps.Point(mapData[mapCount].Markers[i].CustomIcon.ShadowAnchor.X, mapData[mapCount].Markers[i].CustomIcon.ShadowAnchor.Y));
                    }
                    if (mapData[mapCount].Markers[i].CustomIcon.ClickablePolygon != undefined && mapData[mapCount].Markers[i].CustomIcon.ClickablePolygon.length > 0) {
                        var coords = "[" + mapData[mapCount].Markers[i].CustomIcon.ClickablePolygon + "]";
                        m.shape = {
                            coord: eval(coords),
                            type: 'poly'
                        };
                    }
                }
            }

            // initialize all lines
            for (var t = 0; t < mapData[mapCount].Lines.length; t++) {
                var lineOptions = {
                    path: [], //new Array(),
                    strokeColor: mapData[mapCount].Lines[t].StrokeColor,
                    strokeOpacity: mapData[mapCount].Lines[t].StrokeOpacity,
                    strokeWeight: mapData[mapCount].Lines[t].StrokeWeight,
                    clickable: false,
                    map: map[mapCount]
                };
                for (var j = 0; j < mapData[mapCount].Lines[t].Points.length; j++) {
                    lineOptions.path.push(new google.maps.LatLng(mapData[mapCount].Lines[t].Points[j].Latitude, mapData[mapCount].Lines[t].Points[j].Longitude));
                }
                var line = new google.maps.Polyline(lineOptions);
                map[mapCount].lines.push(line);
            }

            // initialize all polygons
            for (var io = 0; i < mapData[mapCount].Polygons.length; io++) {
                var polyOptions = {
                    paths: [], //new Array(),
                    strokeColor: mapData[mapCount].Polygons[io].StrokeColor,
                    strokeOpacity: mapData[mapCount].Polygons[io].StrokeOpacity,
                    strokeWeight: mapData[mapCount].Polygons[io].StrokeWeight,
                    fillColor: mapData[mapCount].Polygons[io].FillColor,
                    fillOpacity: mapData[mapCount].Polygons[io].FillOpacity,
                    clickable: mapData[mapCount].Polygons[io].Clickable,
                    map: map[mapCount]
                };
                for (var jo = 0; jo < mapData[mapCount].Polygons[io].Points.length; jo++) {
                    polyOptions.paths.push(new google.maps.LatLng(mapData[mapCount].Polygons[io].Points[jo].Latitude, mapData[mapCount].Polygons[io].Points[jo].Longitude));
                }
                var poly = new google.maps.Polygon(polyOptions);
                map[mapCount].polygons.push(poly);

                // add info window
                if (polyOptions.clickable && mapData[mapCount].Polygons[io].InfoWindow.length > 0) {
                    poly.infowindow = new google.maps.InfoWindow({
                        content: mapData[mapCount].Polygons[io].InfoWindow
                    });
                    google.maps.event.addListener(map[mapCount].polygons[io], 'click', function(event) {
                        this.map.closeInfoWindow();
                        this.infowindow.setPosition(event.latLng);
                        this.infowindow.open(this.map);
                        this.map.latestInfoWindow = this.infowindow;
                    });
                }
            }
        }
    };


    function loadGMap() {
        var script = document.createElement('script');
        script.type = 'text/javascript';
        script.src = '//maps.googleapis.com/maps/api/js?sensor=false&callback=XA.component.GMap.initialize';
        document.body.appendChild(script);

        mapLoaded = true;
    }

    api.init = function() {
        var gmap = $(".map.component:not(.initialized)");

        if (gmap.length > 0) {
            if (!mapLoaded) {
                loadGMap();
            }
        }

        gmap.each(function() {
            var properties = $(this).data("properties");
            pushMap(properties);

            $(this).addClass("initialized");
        });
    };

    return api;
}(jQuery, document));

XA.register("gmap", XA.component.GMap);