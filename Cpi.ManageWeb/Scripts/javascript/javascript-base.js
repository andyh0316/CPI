var gSearchInputDelay = 1000;
var gConfirmDeleteMsg = "Are you sure you wish to delete this entry? You will not be able to undo this action. Click OK to continue.";

// From Bootsrap: for drop down button
+function ($) {
    'use strict';

    // DROPDOWN CLASS DEFINITION
    // =========================

    var backdrop = '.dropdown-backdrop'
    var toggle = '[data-toggle="dropdown"]'
    var Dropdown = function (element) {
        $(element).on('click.bs.dropdown', this.toggle)
    }

    Dropdown.VERSION = '3.3.5'

    function getParent($this) {
        var selector = $this.attr('data-target')

        if (!selector) {
            selector = $this.attr('href')
            selector = selector && /#[A-Za-z]/.test(selector) && selector.replace(/.*(?=#[^\s]*$)/, '') // strip for ie7
        }

        var $parent = selector && $(selector)

        return $parent && $parent.length ? $parent : $this.parent()
    }

    function clearMenus(e) {
        if (e && e.which === 3) return
        $(backdrop).remove()
        $(toggle).each(function () {
            var $this = $(this)
            var $parent = getParent($this)
            var relatedTarget = { relatedTarget: this }

            if (!$parent.hasClass('open')) return

            if (e && e.type == 'click' && /input|textarea/i.test(e.target.tagName) && $.contains($parent[0], e.target)) return

            $parent.trigger(e = $.Event('hide.bs.dropdown', relatedTarget))

            if (e.isDefaultPrevented()) return

            $this.attr('aria-expanded', 'false')
            $parent.removeClass('open').trigger('hidden.bs.dropdown', relatedTarget)
        })
    }

    Dropdown.prototype.toggle = function (e) {
        var $this = $(this)

        if ($this.is('.disabled, :disabled')) return

        var $parent = getParent($this)
        var isActive = $parent.hasClass('open')

        clearMenus()

        if (!isActive) {
            if ('ontouchstart' in document.documentElement && !$parent.closest('.navbar-nav').length) {
                // if mobile we use a backdrop because click events don't delegate
                $(document.createElement('div'))
                    .addClass('dropdown-backdrop')
                    .insertAfter($(this))
                    .on('click', clearMenus)
            }

            var relatedTarget = { relatedTarget: this }
            $parent.trigger(e = $.Event('show.bs.dropdown', relatedTarget))

            if (e.isDefaultPrevented()) return

            $this
                .trigger('focus')
                .attr('aria-expanded', 'true')

            $parent
                .toggleClass('open')
                .trigger('shown.bs.dropdown', relatedTarget)
        }

        return false
    }

    Dropdown.prototype.keydown = function (e) {
        if (!/(38|40|27|32)/.test(e.which) || /input|textarea/i.test(e.target.tagName)) return

        var $this = $(this)

        e.preventDefault()
        e.stopPropagation()

        if ($this.is('.disabled, :disabled')) return

        var $parent = getParent($this)
        var isActive = $parent.hasClass('open')

        if (!isActive && e.which != 27 || isActive && e.which == 27) {
            if (e.which == 27) $parent.find(toggle).trigger('focus')
            return $this.trigger('click')
        }

        var desc = ' li:not(.disabled):visible a'
        var $items = $parent.find('.dropdown-menu' + desc)

        if (!$items.length) return

        var index = $items.index(e.target)

        if (e.which == 38 && index > 0) index--         // up
        if (e.which == 40 && index < $items.length - 1) index++         // down
        if (!~index) index = 0

        $items.eq(index).trigger('focus')
    }


    // DROPDOWN PLUGIN DEFINITION
    // ==========================

    function Plugin(option) {
        return this.each(function () {
            var $this = $(this)
            var data = $this.data('bs.dropdown')

            if (!data) $this.data('bs.dropdown', (data = new Dropdown(this)))
            if (typeof option == 'string') data[option].call($this)
        })
    }

    var old = $.fn.dropdown

    $.fn.dropdown = Plugin
    $.fn.dropdown.Constructor = Dropdown


    // DROPDOWN NO CONFLICT
    // ====================

    $.fn.dropdown.noConflict = function () {
        $.fn.dropdown = old
        return this
    }


    // APPLY TO STANDARD DROPDOWN ELEMENTS
    // ===================================

    $(document)
        .on('click.bs.dropdown.data-api', clearMenus)
        .on('click.bs.dropdown.data-api', '.dropdown form', function (e) { e.stopPropagation() })
        .on('click.bs.dropdown.data-api', toggle, Dropdown.prototype.toggle)
        .on('keydown.bs.dropdown.data-api', toggle, Dropdown.prototype.keydown)
        .on('keydown.bs.dropdown.data-api', '.dropdown-menu', Dropdown.prototype.keydown)

}(jQuery);

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