$(document).ready(function() {
    $('#pricing tr:odd').find('td,th').addClass('odd');
    $('#pricing tr:even').find('td,th').addClass('even');
    
    $('#pricing th').tooltip({
        track: true,
        delay: 0,
        showBody: " - ",
        extraClass: "tooltip",
        fixPNG: true,
        opacity: 0.9
    });

    
    
});