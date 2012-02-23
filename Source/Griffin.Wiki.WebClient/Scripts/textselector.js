/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this file,
 * You can obtain one at http://mozilla.org/MPL/2.0/. 
 
 Created by Jonas Gauffin. http://www.gauffin.org
 
 Usage:
 
 // get selection
 var selection = new textSelector($('#mytextArea'));
 selection.replace('Will replace selected text');
 
 */

function TextSelector(elem) {
    "use strict";
    
    if (this.jquery) {
        elem = elem[0];
    }
    this.parent = elem;

    /** @returns object {start: X, end: Y, length: Z} 
      * x = start character
      * y = end character
      * length: number of selected characters
      */
    this.get = function() {
        if (elem.selectionStart) {
            return { start: elem.selectionStart, end: elem.selectionEnd, length: elem.selectionEnd-elem.selectionStart };
        }
        
        /*
        elem.focus();
        var range = document.selection.createRange();
        var length = range.text.length;
        range.moveStart ('character', elem.value.length);
        var pos = elem.value.length - length;
        */
        var range = document.selection.createRange();
        var stored_range = range.duplicate();
        stored_range.moveToElementText( elem );
        stored_range.setEndPoint( 'EndToEnd', range );
        var start = stored_range.text.length - range.text.length;
        var end = start + range.text.length;

        return { start: start, end: end, length: range.text.length };
    };
    
    /** Replace selected text with the specified one */
    this.replace = function(newText) {
        if (elem.selectionStart) {
            elem.value = elem.value.substr(0, elem.selectionStart) + newText + elem.value.substr(elem.selectionEnd);
            return this;
        }
        
        elem.focus();
        document.selection.createRange().text = newText;
        return this;
    };
    
    /** Selected the specified range
     * @param start Start character
     * @param end End character
     */
    this.select = function(start, end) {
        if (elem.setSelectionRange) {
            elem.focus();
            elem.setSelectionRange(start, end);
        }
        else if (elem.createTextRange) {
        
            var range = elem.createTextRange();
            range.collapse(true);
            range.moveEnd('character', end);
            range.moveStart('character', start);
            range.select();
        }
        
        return this;
    };
    
    /** @returns if anything is selected */
    this.isSelected = function() {
        return this.get().length !== 0;
    };
    
    /** @returns selected text */
    this.text = function() {
        if (document.selection) {
            //elem.focus();
            //console.log(document.selection.createRange().text);
            return document.selection.createRange().text;
        }
        
        return elem.value.substr(elem.selectionStart, elem.selectionEnd - elem.selectionStart);
    };
}
