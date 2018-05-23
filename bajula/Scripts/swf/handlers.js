function fileQueueError(file, errorCode, message) {
	try {
		var imageName = "error.gif";
		var errorName = "";
		if (errorCode === SWFUpload.errorCode_QUEUE_LIMIT_EXCEEDED) {
			errorName = "You have attempted to queue too many files.";
		}

		if (errorName !== "") {
			alert("Error: " + errorName);
			return;
		}

		switch (errorCode) {
		case SWFUpload.QUEUE_ERROR.ZERO_BYTE_FILE:
			imageName = "zerobyte.gif";
			break;
		case SWFUpload.QUEUE_ERROR.FILE_EXCEEDS_SIZE_LIMIT:
			imageName = "toobig.gif";
			break;
		case SWFUpload.QUEUE_ERROR.ZERO_BYTE_FILE:
		case SWFUpload.QUEUE_ERROR.INVALID_FILETYPE:
		default:
		    alert("You have attempted to queue too many files. Max number of files: " + message);
			break;
		}
		addImage(",/Content/img/swf/" + imageName);
	} catch (ex) {
        this.debug(ex);
    }
}

function fileDialogComplete(numFilesSelected, numFilesQueued) 
{
	try {
		if (numFilesQueued > 0) {
			this.startUpload();
		}
	} catch (ex)  {
        this.debug(ex);
	}
}

/*********** uploading images *********************/
function uploadImageProgress(file, bytesLoaded) {
	try {
		var percent = Math.ceil((bytesLoaded / file.size) * 100);

		var progress = new FileProgress(file,  this.customSettings.upload_target);
		progress.setProgress(percent);
		if (percent === 100) {
			progress.setStatus("Creating thumbnail...");
			progress.toggleCancel(false, this);
		} else {
			progress.setStatus("Uploading...");
			progress.toggleCancel(true, this);
		}
	} catch (ex) {
		this.debug(ex);
	}
}

function uploadImageSuccess(file, serverData) {
    try {
        if (serverData == '') {
            return;
        }
	    addImage(serverData);
		var progress = new FileProgress(file,  this.customSettings.upload_target);

		progress.setStatus("Complete.");
		progress.toggleCancel(false);
	} catch (ex) {
		this.debug(ex);
	}
}

function uploadImageComplete(file) {
	try {
		/*  I want the next upload to continue automatically so I'll call startUpload here */
		if (this.getStats().files_queued > 0) {
			this.startUpload();
		} else {
			var progress = new FileProgress(file,  this.customSettings.upload_target);
			progress.setComplete();
			progress.setStatus("Photo successfully uploaded!");
			progress.toggleCancel(false);
			progress.disappear();
		}
	} catch (ex) {
		this.debug(ex);
	}
}

function uploadImageError(file, errorCode, message) {
    var imageName = "error.gif";
    var progress;
    try {
        switch (errorCode) {
            case SWFUpload.UPLOAD_ERROR.FILE_CANCELLED:
                try {
                    progress = new FileProgress(file, this.customSettings.upload_target);
                    progress.setCancelled();
                    progress.setStatus("Cancelled");
                    progress.toggleCancel(false);
                }
                catch (ex1) {
                    this.debug(ex1);
                }
                break;
            case SWFUpload.UPLOAD_ERROR.UPLOAD_STOPPED:
                try {
                    progress = new FileProgress(file, this.customSettings.upload_target);
                    progress.setCancelled();
                    progress.setStatus("Stopped");
                    progress.toggleCancel(true);
                }
                catch (ex2) {
                    this.debug(ex2);
                }
            case SWFUpload.UPLOAD_ERROR.UPLOAD_LIMIT_EXCEEDED:
                imageName = "uploadlimit.gif";
                break;
            case SWFUpload.UPLOAD_ERROR.HTTP_ERROR:
                if (message == '403') {
                    alert("Session expired. Redirecting to login page ...");
                    window.location = '/login';
                }
                else {
                    alert("Upload Error: " + message);
                }
                break;
            default:
                alert("Upload Error: " + message);
                break;
        }

        addImage(",/Content/img/swf/" + imageName);

    } catch (ex3) {
        this.debug(ex3);
    }

}

/*********** uploading file *********************/
function uploadFileProgress(file, bytesLoaded) {
    try {
        var percent = Math.ceil((bytesLoaded / file.size) * 100);

        var progress = new FileProgress(file, this.customSettings.upload_target);
        progress.setProgress(percent);
        if (percent === 100) {
            progress.setStatus("Processing file ...");
            progress.toggleCancel(false, this);
        } else {
            progress.setStatus("Uploading...");
            progress.toggleCancel(true, this);
        }
    } catch (ex) {
        this.debug(ex);
    }
}

function uploadFileSuccess(file, serverData) {
    alert('success');
    try {
        var progress = new FileProgress(file, this.customSettings.upload_target);
        var json_data = $.evalJSON(serverData);
        if(json_data.success)
        {
            var filedups = json_data.data[0];
            var dbdups = json_data.data[1];
            if (filedups.length != 0) {
                var msg1 = filedups.length + " duplicate(s) found in file uploaded with the following SKU(s): " + filedups.toString();
                var result1 = $('#swfu_container .result').append("<div class='error_post'>" + msg1 + '</div>').fadeIn();
                setTimeout(function() {
                    $(result1).fadeOut(function() {
                        $(this).remove();
                    });
                }, 10000);
            }
            if (dbdups.length != 0) { 
                var msg2 = dbdups.length + " duplicate(s) found in the database with the following SKU(s): " + dbdups.toString();
                var result2 = $('#swfu_container .result').append("<div class='error_post'>" + msg2 + '</div>').fadeIn();
                setTimeout(function() {
                    $(result2).fadeOut(function() {
                        $(this).remove();
                    });
                }, 10000);
            }
            var okresult = $('#swfu_container .result').append("<div class='ok_post'>Products list updated!</div>").fadeIn();
            setTimeout(function() {
            $(okresult).fadeOut(function() { 
                $(this).remove();
                });
            }, 10000);
            // close dialog box and reload page
            dialogBox_close();
            window.location.reload();
        }
        else {
            $.jGrowl(json_data.messsage);
        }
        progress.toggleCancel(false);
        
    } catch (ex) {
        this.debug(ex);
    }
}

function uploadFileComplete(file, json_data) {
    alert('complete');
    try {
        /*  I want the next upload to continue automatically so I'll call startUpload here */
        if (this.getStats().files_queued > 0) {
            this.startUpload();
        } else {
            var progress = new FileProgress(file, this.customSettings.upload_target);
            progress.setComplete();
            progress.setStatus("New items successfully uploaded!");
            progress.toggleCancel(false);
            progress.disappear();
        }
    } catch (ex) {
        this.debug(ex);
    }
}

function uploadFileError(file, errorCode, message) {
    alert('error');
    var imageName = "error.gif";
    var progress;
    try {
        switch (errorCode) {
            case SWFUpload.UPLOAD_ERROR.FILE_CANCELLED:
                try {
                    progress = new FileProgress(file, this.customSettings.upload_target);
                    progress.setCancelled();
                    progress.setStatus("Cancelled");
                    progress.toggleCancel(false);
                }
                catch (ex1) {
                    this.debug(ex1);
                }
                break;
            case SWFUpload.UPLOAD_ERROR.UPLOAD_STOPPED:
                try {
                    progress = new FileProgress(file, this.customSettings.upload_target);
                    progress.setCancelled();
                    progress.setStatus("Stopped");
                    progress.toggleCancel(true);
                }
                catch (ex2) {
                    this.debug(ex2);
                }
            case SWFUpload.UPLOAD_ERROR.UPLOAD_LIMIT_EXCEEDED:
                imageName = "uploadlimit.gif";
                break;
            case SWFUpload.UPLOAD_ERROR.HTTP_ERROR:
                if (message == '403') {
                    alert("Session expired. Redirecting to login page ...");
                    window.location = '/login';
                }
                else {
                    alert('Upload Error: ' + message);
                }
                break;
            default:
                alert('Upload Error: ' + message);
                break;
        }
    } catch (ex3) {
        this.debug(ex3);
    }

}    
    
    
    
    
/******* misc *********/    
    
function fadeIn(element, opacity) {
	var reduceOpacityBy = 5;
	var rate = 30;	// 15 fps


	if (opacity < 100) {
		opacity += reduceOpacityBy;
		if (opacity > 100) {
			opacity = 100;
		}

		if (element.filters) {
			try {
				element.filters.item("DXImageTransform.Microsoft.Alpha").opacity = opacity;
			} catch (e) {
				// If it is not set initially, the browser will throw an error.  This will set it if it is not set yet.
				element.style.filter = 'progid:DXImageTransform.Microsoft.Alpha(opacity=' + opacity + ')';
			}
		} else {
			element.style.opacity = opacity / 100;
		}
	}

	if (opacity < 100) {
		setTimeout(function () {
			fadeIn(element, opacity);
		}, rate);
	}
}

/*
A simple class for displaying file information and progress
Note: This is a demonstration only and not part of SWFUpload.
Note: Some have had problems adapting this class in IE7. It may not be suitable for your application.
*/

// Constructor
// file is a SWFUpload file object
// targetID is the HTML element id attribute that the FileProgress HTML structure will be added to.
// Instantiating a new FileProgress object with an existing file will reuse/update the existing DOM elements
function FileProgress(file, targetID) {
    this.fileProgressID = file.id;

    this.opacity = 100;
    this.height = 0;

    this.fileProgressWrapper = document.getElementById(this.fileProgressID);
    if (!this.fileProgressWrapper) {
        this.fileProgressWrapper = document.createElement("div");
        this.fileProgressWrapper.className = "progressWrapper";
        this.fileProgressWrapper.id = this.fileProgressID;

        this.fileProgressElement = document.createElement("div");
        this.fileProgressElement.className = "progressContainer";

        var progressCancel = document.createElement("a");
        progressCancel.className = "progressCancel";
        progressCancel.href = "#";
        progressCancel.style.visibility = "hidden";
        progressCancel.appendChild(document.createTextNode(" "));

        var progressText = document.createElement("div");
        progressText.className = "progressName";
        progressText.appendChild(document.createTextNode(file.name));

        var progressBar = document.createElement("div");
        progressBar.className = "progressBarInProgress";

        var progressStatus = document.createElement("div");
        progressStatus.className = "progressBarStatus";
        progressStatus.innerHTML = "&nbsp;";

        this.fileProgressElement.appendChild(progressCancel);
        this.fileProgressElement.appendChild(progressText);
        this.fileProgressElement.appendChild(progressStatus);
        this.fileProgressElement.appendChild(progressBar);

        this.fileProgressWrapper.appendChild(this.fileProgressElement);

        document.getElementById(targetID).appendChild(this.fileProgressWrapper);
    } else {
        this.fileProgressElement = this.fileProgressWrapper.firstChild;
    }

    this.height = this.fileProgressWrapper.offsetHeight;

}
FileProgress.prototype.setProgress = function(percentage) {
    this.fileProgressElement.className = "progressContainer";
    this.fileProgressElement.childNodes[3].className = "progressBarInProgress";
    this.fileProgressElement.childNodes[3].style.width = percentage + "%";
};
FileProgress.prototype.setComplete = function() {
    this.fileProgressElement.className = "progressContainer blue";
    this.fileProgressElement.childNodes[3].className = "progressBarComplete";
    this.fileProgressElement.childNodes[3].style.width = "";

    var oSelf = this;
    setTimeout(function() {
        oSelf.disappear();
    }, 10000);
};
FileProgress.prototype.setError = function() {
    this.fileProgressElement.className = "progressContainer red";
    this.fileProgressElement.childNodes[3].className = "progressBarError";
    this.fileProgressElement.childNodes[3].style.width = "";

    var oSelf = this;
    setTimeout(function() {
        oSelf.disappear();
    }, 5000);
};
FileProgress.prototype.setCancelled = function() {
    this.fileProgressElement.className = "progressContainer";
    this.fileProgressElement.childNodes[3].className = "progressBarError";
    this.fileProgressElement.childNodes[3].style.width = "";

    var oSelf = this;
    setTimeout(function() {
        oSelf.disappear();
    }, 2000);
};
FileProgress.prototype.setStatus = function(status) {
    this.fileProgressElement.childNodes[2].innerHTML = status;
};

// Show/Hide the cancel button
FileProgress.prototype.toggleCancel = function(show, swfUploadInstance) {
    this.fileProgressElement.childNodes[0].style.visibility = show ? "visible" : "hidden";
    if (swfUploadInstance) {
        var fileID = this.fileProgressID;
        this.fileProgressElement.childNodes[0].onclick = function() {
            swfUploadInstance.cancelUpload(fileID);
            return false;
        };
    }
};

// Fades out and clips away the FileProgress box.
FileProgress.prototype.disappear = function() {
    var id = '#' + this.fileProgressID;
    setTimeout(function() {
        $(id).fadeOut('normal', function() {
            $(this).remove();
        });
    }, 3000);

};