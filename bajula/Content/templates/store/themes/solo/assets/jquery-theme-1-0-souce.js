// Clear Inputs
$.cleared = function(field) {
	
	if ($(field).length) {
		$(field).each(function() {
			var text = $(this).val();
			$(this).focus(function() { if ($(this).val() == text) { $(this).val(''); }; });
			$(this).blur(function() { if ($(this).val() == '') { $(this).val(text); }; });
		});
	};
	
};

// Validate Email
$.validate = function(form) {
	
	if ($(form).length) {
	
		$(form).submit(function() {
			var action = $(this).attr("action");
			var fname = $(this).find("input#fname").attr("name");
			var lname = $(this).find("input#lname").attr("name");
			var email = $(this).find("input#email").attr("name");
			if (action == "" || fname == "" || lname == "" || email == "") {
				alert("The newsletter form is not configured properly. Update the newsletter settings in the theme editor in your shop admin.");
				return false;
			};
			var address = $(this).find("input#email").val();
			if (!$.regex(address)) {
				alert("Please enter a valid email address to subscribe.");
				$(form).find("input#email").focus();
				return false;
			};
		});
		
		$.regex = function(email) {
			var regex = new RegExp(/^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i);
			return regex.test(email);
		};
		
	};
	
};

// Slider
$.slider = function(container, slider, prev, next) {
	
	if ($(container).length > 0 ) {
		
		var width = parseInt($(slider + " li").length) * 116;
		if ($(slider + " li").length < 6) { $(next).addClass("inactive"); };
		
		$(prev).unbind("click");
		$(prev).click(function() {
			if ($(prev).hasClass("inactive") || $(prev).hasClass("disabled")) {
				return false;
			} else {
				$(prev).addClass("disabled");
				var position = parseInt($(slider).css("margin-left"));
				var move = (position + 580) + "px";
				$(slider).animate({ marginLeft: move }, 1000, "easeInOutQuint", function() {
					var next_click = parseInt($(slider).css("margin-left")) + 580;
					if (next_click > 0) { $(prev).addClass("inactive"); };
					$(next).removeClass("inactive");
					$(prev).removeClass("disabled");
				});
			};
			return false;
		});
		
		$(next).unbind("click");
		$(next).click(function() {
			if ($(next).hasClass("inactive") || $(next).hasClass("disabled")) {
				return false;
			} else {
				$(next).addClass("disabled");
				var position = parseInt($(slider).css("margin-left"));
				var move = (position - 580) + "px";
				$(slider).animate({ marginLeft: move }, 1000, "easeInOutQuint", function() {
					var next_click = parseInt($(slider).css("margin-left")) + -580;
					if ((next_click + width) <= 0) { $(next).addClass("inactive"); };
					$(prev).removeClass("inactive");
					$(next).removeClass("disabled");
				});
			};
			return false;
		});
		
	};
	
};


// Initialize page elements once the DOM is ready
$(document).ready(function() {
	
	$.cleared("input.field");
	$.validate("li.newsletter form");
        $.slider("div#slider", "div#slider ul", "div#prev a", "div#next a");
	
});