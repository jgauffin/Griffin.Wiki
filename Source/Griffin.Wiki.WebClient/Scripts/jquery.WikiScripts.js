/**
* A small script for floating menus (or other elements) with the screen, as
* the the window is scrolled.
* Great for having the menu visible at all times to the user.
*
* The script is free to use, donations are always welcomed :)
*
* @author jimmiw
* @since 2011-06-16
* @homepage http://westsworld.dk
* @version 1.0
*/
(function ($) {
    $.fn.floatmenu = function (options) {
        // merges the given options with some default options
        var options = $.extend({
            topPadding: 10
        }, options);

        return this.each(function () {
            // fetches and initializes the current element.
            var obj = $(this);

            var position = $(obj).offset(),
      cssPosition = $(obj).css('position');

            // tests if a "position" was set on the element, if not, sets a default
            if (cssPosition == '') {
                cssPosition = 'static';
            }

            // attaches an event to listen for scroll events
            $(window).scroll(function (e) {
                // if the window's inner frame passes the top of the element,
                // start moving the menu
                if ($(window).scrollTop() > (position.top - Number(options.topPadding))) {
                    $(obj).
            css('position', 'fixed').
            css('top', options.topPadding);
                }
                // the window's inner frame has not passed the menu, reset
                // the objects position
                else {
                    $(obj).css('position', cssPosition);
                }
            });
        });
    };
})(jQuery);