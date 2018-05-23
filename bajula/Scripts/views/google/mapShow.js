var map = null;
$(document).ready(function() {
    var zoomLevel = parseInt($('#mapzoom').val(), 10);
    var latitude = $('#latitude').val();
    var longtitude = $('#longtitude').val();
    
    if (latitude != '0') {
        var center = new GLatLng(latitude, longtitude);
        if (zoomLevel == '0') {
            zoomLevel = 1;
        }

        map = new GMap2(document.getElementById("profile_map"));
        map.addControl(new GSmallMapControl());
        map.addControl(new GMapTypeControl());
        map.setCenter(center, zoomLevel);
        map.setZoom(zoomLevel);

        if (map.tradelrMarker) {
            map.tradelrMarker.setPoint(center);
        } else {
            var marker = new GMarker(center, { draggable: true });
            map.tradelrMarker = marker;
            map.addOverlay(marker);
            panToMarker = function() {
                map.panTo(marker.getPoint());
            };
            GEvent.addListener(marker, "dragend", panToMarker);
        }
        map.panTo(center);
    }
});

$('#buttonClose').click(function() {
    dialogBox_close();
});