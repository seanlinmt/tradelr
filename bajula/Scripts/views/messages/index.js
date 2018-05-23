function showLoader(messageType) {
    $('#' + messageType).showLoading();
}

/**
* Shows the selected message
*/
function showMessage(messageType, messageId) {
    showLoader(messageType);
    $.get('/messages/get', { 'messageId': messageId, 'messageType': messageType }, function(data, textStatus) {
        // assign the message html to the content area
        $('#' + messageType).html(data);
        // set the hover event for the back button
        $('#' + messageType + ' .button').hover(
				function() { $(this).addClass('ui-state-hover'); },
				function() { $(this).removeClass('ui-state-hover'); }
		);
        // bind the back button to load the tab's content
        $('#' + messageType + ' .button').bind('click', function() {
            selectMessagesTab(0, messageType);
        });
    });
}

/**
* Shows the messages for the selected tab
*/
function selectMessagesTab(index, type) {
    // 0 = inbox, 1 = sent
    var messageType = '';
    if (type != undefined) messageType = type;
    else if (index == 0) messageType = 'inbox';
    else if (index == 1) messageType = 'sent';

    // set loading message
    showLoader(messageType);

    // and load the tab's content (we reload every time so new messages pop up right away, it's the easy way out ok!:)
    $.get('/messages/' + messageType, null,
		function(data, textStatus) {
		    // assign the recieved html to the tab's content div
		    $('#' + messageType).html(data);

		    // add hover to the entire message div
		    $('#' + messageType + ' .message').hover(
				    function() { $(this).addClass('message-hover'); },
				    function() { $(this).removeClass('message-hover'); }
			);

		    // add hover to the icons
		    $('#' + messageType + ' .message .ui-state-default').hover(
					    function() { $(this).addClass('ui-state-hover'); },
					    function() { $(this).removeClass('ui-state-hover'); }
			);

		    // hook up the delete icon to a confirmation dialog, and the removal code to it's 'remove' button event
		    $('#' + messageType + ' .message .ui-icon-closethick').bind('click', function() {
		        // get the message id from the span's id and cast it as a int (* 1)
		        var messageId = $(this).attr('id').replace('removeIcon', '') * 1;
		        $("#dialog" + messageId).dialog({
		            bgiframe: true,
		            resizable: false,
		            height: 140,
		            modal: true,
		            closeOnEscape: true,
		            overlay: {
		                backgroundColor: '#000',
		                opacity: 0.5
		            },
		            buttons: {
		                'Remove': function() {
		                    showLoader(messageType);
		                    $(this).dialog('destroy');
		                    $.get('/messages/delete/' + messageId, null, function(data, textStatus) {
		                        selectMessagesTab(index, messageType);
		                    });
		                },
		                'No': function() {
		                    $(this).dialog('destroy');
		                }
		            }
		        });
		        return false;
		    });

		    // The click on the persons name, handled in javascript since otherwise we can't stop the event propergation to the parent div click event
		    $('#' + messageType + ' .message a').bind('click', function() {
		        window.location = $(this).attr('href');
		        return false;
		    });

		    // Hook up the actual message click (aka: read message)
		    $('#' + messageType + ' .message').bind('click', function() {
		        var messageId = $(this).attr('id').replace('message', '') * 1;
		        showMessage(messageType, messageId);
		        return false;
		    });
		});
}

$(document).ready(function() {
    // create the tabs and call selectMessagesTab on click
    var $tabs = $('#messageTabs').tabs({
        select: function(e, ui) {
            selectMessagesTab(ui.index);
        }
    });
    if (querySt('tab') != undefined) {
        $tabs.tabs('select', '#' + querySt('tab'));
    }

    // hook up the compose button
    $('#messageCompose').bind('click', function() {
        $.get('/messages/compose', null,
			function(data, textStatus) {
			    // assign the recieved html to the tab's content div
			    var index = $tabs.data('selected.tabs');
			    var messageType = '';
			    if (index == 0) messageType = 'inbox';
			    else if (index == 1) messageType = 'sent';
			    $('#' + messageType).html(data);
			    $('#compose_send').bind('click', function() {
			        var to = $("select#to").val();
			        var subject = $("input#subject").val();
			        var message = $('textarea#message').val();
			        if (to == '' || subject == '' || message == '') {
			            alert('Select a recipient and fill in a subject before sending');
			        } else {
			            showLoader(messageType);
			            $.post('/messages/send', { 'to': to, 'subject': subject, 'message': message }, function() {
			                selectMessagesTab(index);
			            });
			        }
			    });
			    $('#compose_cancel').bind('click', function() {
			        selectMessagesTab(index);
			    });
			    $('#compose_send, #compose_cancel').hover(
						function() { $(this).addClass('ui-state-hover'); },
						function() { $(this).removeClass('ui-state-hover'); }
				);
			});
    });
    if (querySt('cmd') != undefined && querySt('cmd') == 'compose') {
        $('#messageCompose').click();
    }
    else {
        // populate the current live tab
        var selected = $tabs.data('selected.tabs');
        selectMessagesTab(selected);
    }
});
