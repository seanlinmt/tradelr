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

// Initialize page elements once the DOM is ready
$(document).ready(function() {
	
	$.cleared("input.field");
	
});