// show string on ellipsis
$(document).on('mouseenter', '*', function (e) {
    if (e.currentTarget.offsetWidth < e.currentTarget.scrollWidth) {
        showToolTip($('#help-tooltip'), $(this), $(this).text());
    }
}).on('mouseleave', '*', function () {
    if ($('#help-tooltip:hover').length === 0) {
        $('#help-tooltip').hide();
    }
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