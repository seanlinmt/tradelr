<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.subdomain.VisitorStatistics>" %>
<div id="visits_graph" style="width:750px;height:100px;"> 
</div>
<div id="visits_summary">
<ul>
<li><div><strong class="larger"><%= Model.pageViews %></strong> PAGE VIEWS</div>
<span class="smaller"><%= Model.pagesPerVisit %> pages per visit</span>
</li>
<li><div><strong class="larger"><%= Model.visitorsTotal %></strong> VISITORS</div>
<span class="smaller"><%= Model.visitorsPerDay %> visits per day</span></li>
<li><div><span class="larger"><%= Model.averageTimeSpent %></span></div>
<span class="smaller">average time spent</span></li>
</ul>
<div class="clear"></div>
</div>
<div class="stats">
<h5>Top Referrers</h5>
<%= Model.referrerStats %>
</div>
<div class="stats">
<h5>Traffic Overview</h5>
<%= Model.trafficOverviewStats %>
</div>
<div class="stats">
<h5>Search Keywords</h5>
<%= Model.searchKeywordStats %>
</div>
<div class="stats">
<h5>Top Countries</h5>
<%= Model.countriesStats %>
</div>
<div class="clear"></div>

<script type="text/javascript">
var xaxis = <%= Model.graphticksdata %>;
$(document).ready(function () {
    var d1 = <%= Model.graphdata %>;
    $.plot($('#visits_graph'),
        [{ data: d1, bars: { show: true }}],
        {
            grid: { hoverable: true },
            xaxis: { ticks: 0 },
            yaxis: { tickDecimals: 0 },
            colors: ["#B4CF5E"]
        });
});

function showTooltip(x, y, contents) {
    $('<div id="stats_tooltip">' + contents + '</div>').css({
        position: 'absolute',
        display: 'none',
        top: y + 20,
        left: x + 20,
        border: '1px solid #ccc',
        padding: '5px',
        lineHeight: '18px',
        fontSize: 'small',
        backgroundColor: '#fff',
        textAlign: 'center',
        opacity: 0.80
    }).appendTo("body").fadeIn(200);
}

var previousIndexPoint = null;
$("#visits_graph").bind("plothover", function (event, pos, item) {
    $("#x").text(pos.x.toFixed(2));
    $("#y").text(pos.y.toFixed(2));
    
    if (item) {
        if (previousIndexPoint != item.dataIndex) {
            previousIndexPoint = item.dataIndex;

            $("#stats_tooltip").remove();
            var x = item.datapoint[0].toFixed(2), y = item.datapoint[1].toFixed(2);
            var content = ["<div class='bold'>",parseInt(y.toString(),10), " Visits</div>", "<div>", xaxis[item.dataIndex], "</div>"];
            showTooltip(item.pageX, item.pageY,  content.join(''));
        }
    }
    else {
        $("#stats_tooltip").remove();
        previousPoint = null;
    }
});
</script>
