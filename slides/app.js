(function () {
    var map;

    function createInfobox(name, pin) {
        var loc = pin.getLocation();
        return new Microsoft.Maps.Infobox(loc, {
            title: name,
            description: name + ' is located at Latitude: ' + loc.latitude + ', Longitude: ' + loc.longitude,
            visible: true,
            offset: new Microsoft.Maps.Point(0, 15)
        });
    }

    function addToMap(place) {
        var loc = new Microsoft.Maps.Location(place.location.latitude, place.location.longitude);
        var pin = new Microsoft.Maps.Pushpin(loc);
        var infobox = createInfobox(place.name, pin);
        map.entities.push(pin);
        map.entities.push(infobox);
        return loc;
    }

    function calculateDistance(start, dest) {
        var xhr = new XMLHttpRequest();
        xhr.open('GET', 'http://localhost:7000/api/calc?start=' + encodeURIComponent(start) + '&dest=' + encodeURIComponent(dest), true);
        xhr.setRequestHeader('Accept', 'application/json');
        xhr.onreadystatechange = function (e) {
            if (this.readyState === 4 && this.status === 200) {
                var data = JSON.parse(this.responseText);
                // Set the result value.
                document.getElementById('show-result').value = data.distance + ' feet' || 'Could not calculate';

                // Clear any previous entities.
                map.entities.clear();

                // Add the cities to the map.
                var startLoc = addToMap(data.start);
                var destLoc = addToMap(data.dest);

                var viewBoundaries = Microsoft.Maps.LocationRect.fromLocations(startLoc, destLoc);
                map.setView({ bounds: viewBoundaries });
            }
        };
        xhr.send();
    }

    function calculateClick() {
        var start = document.getElementById('start').value;
        var dest = document.getElementById('dest').value;
        calculateDistance(start, dest);
    }

    function onLoad() {
        map = new Microsoft.Maps.Map(document.getElementById('mapDiv'), { credentials: 'AoguaokGIrhOIy0jfJZExcfYNSWeliGCe88XXIL4AZtq35luAIkA33uIp6xWBwgS' });
    }

    document.getElementById('calculate').addEventListener('click', calculateClick, false);
    document.addEventListener('DOMContentLoaded', onLoad, false)
})();
