var gSearchInputDelay = 1000;
var gConfirmDeleteMsg = "Are you sure you wish to delete this entry? You will not be able to undo this action. Click OK to continue.";

// show string on ellipsis
$(document).on('mouseenter', 'dd', function (e) {
    if ($(this).find('input').length === 0 && e.currentTarget.offsetWidth < e.currentTarget.scrollWidth) {
        showToolTip($('#help-tooltip'), $(this), $(this).text());
    }
}).on('mouseleave', 'dl', function () {
    if ($('#help-tooltip:hover').length === 0) {
        $('#help-tooltip').hide();
    }
});

// show field validation error on hover for dd
$(document).on('mouseenter', 'dd', function () {
    var fieldValidationError = $(this).find('.field-validation-error:not(.ng-hide)');
    if (fieldValidationError.length == 0) {
        return;
    }

    var message = fieldValidationError.html();
    showToolTip($('#validation-tooltip'), $(this), message);
}).on('mouseleave', 'dl', function () {
    $('#validation-tooltip').hide();
});




function showToolTip(toolTipSelector, hoveringElementSelector, message) {
    var offset = hoveringElementSelector.offset();
    var bodyHeight = $('body').outerHeight();
    toolTipSelector.html(message).css({ bottom: bodyHeight - offset.top + 6, left: offset.left }).show();
    if (toolTipSelector.position().left + toolTipSelector.outerWidth() + 30 >= $('body').outerWidth()) {
        toolTipSelector.css({ left: offset.left - toolTipSelector.outerWidth() });
    }

    if (toolTipSelector.position().top <= 0) {
        toolTipSelector.css({ bottom: bodyHeight - offset.top - toolTipSelector.outerHeight() - hoveringElementSelector.outerHeight() - 10 });
    }
};

// auto extend session every 5 minutes (the session timeout is set to be 20 minutes but javascript setInterval is inaccurate so to be REALLY safe we do 5 minutes)
//$(document).ready(function () {
//    gAutoExtendSession = setInterval(function () {
//        extendSession();
//    }, 300000);

//    // show un-authorized text on menu
//    $(".menu-item.not-authorized").hover(
//        function () {
//            showToolTip($('#help-tooltip'), $(this), 'You are not authorized to enter this section.');
//        },
//        function () {
//            $('#help-tooltip').hide();
//        }
//    );

//    $(".menu-item.not-authorized").click(function (e) {
//        e.preventDefault();
//    });
//})

//function extendSession() {
//    $.getJSON('/Account/ExtendSession/', function (data, status) {
//    });
//};

/**** USER SESSION WARNING ****/
var gSessionWarning;
var gCheckSessionWarning;
var gShowSessionTimeOutDate;
var gShowSessionWarningDate;
var gAutoExtendSession = false;
function setSessionWarning(timeLeft) {
    clearInterval(gCheckSessionWarning);
    clearInterval(gCountDownTimerWatch);

    if (timeLeft === -1) { // not applicable: probably not logged in yet. We want to cancel the previous count down if it exists
        return;
    }

    $('#session-warning').hide();
    timeLeft = Math.floor(timeLeft);

    // calc date in future for session timeout: date now + session time left in milliseconds - milliseconds to show warning
    gShowSessionTimeOutDate = new Date(Date.now() + timeLeft);
    gShowSessionWarningDate = new Date(Date.now() + timeLeft - gShowSessionWarningTime);

    // repeatedly check if the browser's current date reaches the show session warning date
    gCheckSessionWarning = setInterval(function () {
        if (new Date() > gShowSessionWarningDate) {
            if (gAutoExtendSession) { // if autoExtendSession is enabled, just automatically extend session and don't show box
                extendSession();
            } else {
                clearInterval(gCheckSessionWarning);

                countDownTimer(gShowSessionTimeOutDate - Date.now());
                setTimeout(function () {
                    $('#session-warning').css({ 'display': 'table' }); // show warning
                }, 2000);
            }
        }
    }, 10000);
};

var gCountDownTimerWatch;
var gCountDownTimeLeft;
function countDownTimer(_timeLeft) {
    gCountDownTimeLeft = _timeLeft;

    gCountDownTimerWatch = setInterval(function () {

        var timeLeft = gCountDownTimeLeft;
        timeLeft = Math.floor(timeLeft / 1000);

        var seconds = Math.floor(timeLeft % 60) + "";
        if (seconds.length == 1) {
            seconds = "0" + seconds;
        }

        timeLeft = timeLeft / 60;
        var minutes = Math.floor(timeLeft % 60);

        $('#session-warning').find('.time-left').html(minutes + ':' + seconds);

        if (gCountDownTimeLeft <= 0) {
            location.href = '/Public/Index'; // time reaches 0, automatically go to login screen
            return;
        }

        gCountDownTimeLeft = gCountDownTimeLeft - 1000;
    }, 1000);
};

function extendSession() {
    $.getJSON('/Account/ExtendSession/', function (data, status) {
        $.getJSON('/Account/ExtendSession/', function (data, status) {
            // call twice because the first JSON will reset expiration but the JSON result won't carry the reflected expiration
            setSessionWarning(data.SessionTimeLeft);
        });
    });
    $('#session-warning').hide();
};

function logout() {
    location.href = '/Public/Index';
    $('#session-warning').hide();
};