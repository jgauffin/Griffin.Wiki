/* 

Wiki image dialog for griffin.editor.
Created by Jonas Gauffin. http://www.gauffin.org
 
Usage:
 
Include the script below the main editor script.
 
*/
(function ($) {
    "use strict";

    $.griffinEditorExtension.imageDialog = function (options) {

        /*
        var $url = $('input[name="url"]');
        if (options.url !== null && typeof options.url !== 'undefined') {
        $url.val(options.url);
        }        
        */
        $('body').elementOverlay();
        $.get(window.RootUri + 'wiki/adm/image/', function (html) {
            $('body').elementOverlay('destroy');

            var $dialogContent = $('<div style="display:none" id="upload-dialog"></div>').html(html);

            var $title = $('input[name="title"]', $dialogContent);
            if (options.title !== null && typeof options.title !== 'undefined') {
                $title.val(options.title);
            }

            var dialogOptions = {
                autoOpen: true,
                modal: true,
                title: 'Select image',
                open: function () {
                    var self = $(this);
                    initializeUploadDialog(options.success, function () {
                        self.dialog('close');
                        $dialogContent.remove();
                    });
                },
                buttons: {
                    Cancel: function () {
                        $(this).dialog("close");
                    }
                }
            };
            $dialogContent.appendTo('body');
            $dialogContent.dialog(dialogOptions);
        });

    };

})(jQuery);	