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
$(document).ready(function () {
    gAutoExtendSession = setInterval(function () {
        extendSession();
    }, 300000);

    // show un-authorized text on menu
    $(".menu-item.not-authorized").hover(
        function () {
            showToolTip($('#help-tooltip'), $(this), 'You are not authorized to enter this section.');
        },
        function () {
            $('#help-tooltip').hide();
        }
    );

    $(".menu-item.not-authorized").click(function (e) {
        e.preventDefault();
    });
})

function extendSession() {
    $.getJSON('/Account/ExtendSession/', function (data, status) {
    });
};