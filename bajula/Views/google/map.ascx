<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.google.GoogleMapData>" %>
<%@ Import Namespace="tradelr.Common.Constants" %>
<div id="map_content">
<h3 id="headingMarker">Set Your Location</h3>
    <span class="tip">Click on the map to set your location</span>
    <div style="margin:5px 0px">
        <%= Html.DropDownList("countryMap", Model.countryList)%>
    </div>
    <div id="map" style="width: 745px; height: 270px">
    </div>
    <div>
    <div class="pad5">
            <button id="saveButton" type="button" class="green ajax">
                save location</button>
            <button id="cancelButton">
                cancel</button>
        </div>
    </div>
<script type="text/javascript">

    $(document).ready(function () {
        var countryData = $.parseJSON($('#countryData').text());

        $('#cancelButton').click(function () {
            dialogBox_close();
            return false;
        });
        
        // handle country select
        $('#countryMap').bind('change', function () {
            var id = $(this).val();
            var country = null;
            $.each(countryData, function (i, val) {
                if (val.id == id) {
                    country = val;
                    return false;
                }
            });

            var enableMap = country != null;
            var zoomLevel = enableMap ? 3 : 1;
            var point = new GLatLng(country.latitude, country.longtitude);
            //map.setCenter(new GLatLng(country.latitude, country.longtitude), zoomLevel);
            map.setZoom(zoomLevel);
            if (enableMap) {
                setMarker(point);
            } else {
                removeMarker();
                map.setCenter(point, zoomLevel);
            }
            geocoder.setBaseCountryCode(country.code);

            // update hiddenfields
            saveMarker();
        });
    });

    var map;
    function googleMapInitialise() {
        var center;

        var zoomLevel = parseInt($('#mapzoom', '#map_content').val(), 10);
        var latitude = $('#latitude', '#map_content').val();
        var longtitude = $('#longtitude', '#map_content').val();

        if (latitude != '0') {
            center = new GLatLng(latitude, longtitude);
            if (zoomLevel == '0') {
                zoomLevel = 1;
            }
            hasLoc = true;
        } else {
            center = new GLatLng(0, 0);
            zoomLevel = 1;
            hasLoc = false;
        }

        geocoder = new GClientGeocoder();
        map = new GMap2(document.getElementById("map"));
        map.addControl(new GSmallMapControl());
        map.addControl(new GMapTypeControl());
        map.setCenter(center, zoomLevel);
        map.setZoom(zoomLevel);
        //map.enableScrollWheelZoom();
        map.enableContinuousZoom();

        GEvent.addListener(map, "click", function (marker, point) { setMarker(point); });
        GEvent.addListener(map, "zoomend", function (oldzoom, zoom) {
            $('#mapzoom', '#map_content').val(zoom);
        });

        getMarkerLatLon = function () {
            return map.tradelrMarker ? map.tradelrMarker.getPoint() : { lat: 0, lon: 0 };
        }

        saveMarker = function () {
            var marker = getMarkerLatLon();
            if (marker.lat == 0) {
                $('#latitude', '#map_content').val(marker.lat);
                $('#longtitude', '#map_content').val(marker.lon);
            }
            else {
                $('#latitude', '#map_content').val(marker.lat());
                $('#longtitude', '#map_content').val(marker.lng());
            }
        }

        setMarker = function(point) {
            if (map.tradelrMarker) {
                map.tradelrMarker.setPoint(point);
            } else {
                var marker = new GMarker(point, { draggable: true });
                map.tradelrMarker = marker;
                map.addOverlay(marker);
                panToMarker = function() {
                    map.panTo(marker.getPoint());
                };
                GEvent.addListener(marker, "dragend", panToMarker);
            }
            map.panTo(point);
            saveMarker();
        };

        if (hasLoc)
            setMarker(center);

        removeMarker = function() {
            if (map.tradelrMarker) {
                map.removeOverlay(map.tradelrMarker);
                map.tradelrMarker = null;
            }
        };
    }

    google.load("maps", "2", { "callback": googleMapInitialise });
    
    $('#saveButton').click(function() {
        var latitude = $('#latitude', '#map_content').val();
        var longtitude = $('#longtitude', '#map_content').val();
        var mapzoom = $('#mapzoom', '#map_content').val();

        if (latitude == 0) {
            $.jGrowl('Please specify your location');
            return false;
        }
            
        var postdata = "id=" + <%= Model.orgid %> + "&latitude=" + latitude + "&longtitude=" + longtitude + "&mapzoom=" + mapzoom;
        $(this).buttonDisable();
        $.ajax({
            type: "POST",
            url: "/google/map",
            dataType: "json",
            data: postdata,
            success: function(json_data) {
                if (json_data.success) {
                    $.jGrowl(json_data.message);
                    dialogBox_close();
                }
                else {
                    $.jGrowl(json_data.message, { sticky: true });
                }
                $('#saveButton').buttonEnable();
                return false;
            }
        });
        return false;
    });
</script>
        
    <%= Html.Hidden("latitude", Model.latitude) %>
    <%= Html.Hidden("longtitude", Model.longtitude) %>
    <%= Html.Hidden("mapzoom", Model.mapZoom) %>
<span id="countryData" class="hidden"><%= Model.countryData %></span>
</div>
