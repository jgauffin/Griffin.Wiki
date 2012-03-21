$.loadedMethods = [];
$.loaded = function (func) {
    $.loadedMethods.push(func);
};

$.triggerLoaded = function (parent) {
    $.each($.loadedMethods, function (index, func) {
        func.apply(parent);
    });
    jQuery.validator.unobtrusive.parse($(parent));
    $.loadedMethods = [];
};

$(function () {
    $('#toc-container').floatmenu();
    //$('#toc').toc();
    // remove all that were added during document load
    $.triggerLoaded($('body')[0]);
});


///elementOverlay plugin is used to display an overlay over an item
//
(function ($) {
    
    //globals
    $.elementOverlay = {
        texts: {
            title: 'Please wait, loading..'
        },
        translations: []
    };

    var methods = {
        init: function (options) {
            var settings = $.extend({
            }, options);

            return this.each(function () {
                var $this = $(this);
                var data = $this.data('overlay');


                this.reposition = function () {
                    var pos = $this.offset();
                    data.overlay.css({
                        position: 'absolute',
                        top: pos.top,
                        left: pos.left,
                        width: $this.width() + "px",
                        height: $this.height() + "px"
                    });
                    $('div', data.overlay).css('padding-top', (($this.height() / 2) - 20) + 'px');
                };

                if (typeof data === 'undefined') {
                    data = {};
                    var id = $this.attr('id') + '-overlay';
                    data.overlay = $(id);
                    if (data.overlay.length == 0) {
                        data.overlay = $('<div class="ui-widget-overlay element-overlay-container" id="' + id + '"><div>' + $.elementOverlay.texts.title + '</div></div>');

                        $('body').append(data.overlay);
                        this.reposition();
                    }

                    $(this).data('overlay', {
                        target: $this,
                        target2: this,
                        overlay: data.overlay,
                        settings: settings
                    });

                }

                return this;
            });
        },
        destroy: function () {

            return this.each(function () {

                var $this = $(this),
                    data = $this.data('overlay');

                    if (data == null)
                        return this;
                // Namespacing FTW
                $(window).unbind('.elementOverlay');
                data.overlay.remove();
                $this.removeData('overlay');

            });
        },

        show: function () {
            var $this = $(this),
                data = $this.data('overlay');

            data.target2.reposition();
            data.overlay.show();
            return this;
        },
        hide: function () {
            var $this = $(this),
                data = $this.data('overlay');

                if (data == null)
                    return this;
            data.overlay.hide();
            return this;
        }
    };

    $.fn.elementOverlay = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.elementOverlay');
        }

    };

})(jQuery);