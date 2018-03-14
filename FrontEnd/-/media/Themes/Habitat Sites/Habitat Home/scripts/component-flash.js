XA.component.flash = (function($) {

    var api = {};

    function loadjQueryFlash() {
        /**
         * Flash (http://jquery.lukelutman.com/plugins/flash)
         * A jQuery plugin for embedding Flash movies.
         * 
         * Version 1.0
         * November 9th, 2006
         *
         * Copyright (c) 2006 Luke Lutman (http://www.lukelutman.com)
         * Dual licensed under the MIT and GPL licenses.
         * http://www.opensource.org/licenses/mit-license.php
         * http://www.opensource.org/licenses/gpl-license.php
         * 
         * Inspired by:
         * SWFObject (http://blog.deconcept.com/swfobject/)
         * UFO (http://www.bobbyvandersluis.com/ufo/)
         * sIFR (http://www.mikeindustries.com/sifr/)
         * 
         * IMPORTANT: 
         * The packed version of jQuery breaks ActiveX control
         * activation in Internet Explorer. Use JSMin to minifiy
         * jQuery (see: http://jquery.lukelutman.com/plugins/flash#activex).
         *
         **/
        /* eslint-disable */
        ! function() {
            function a() { var a = ""; for (var t in this) "function" != typeof this[t] && (a += t + '="' + this[t] + '" '); return a }

            function t() { var a = ""; for (var t in this) "function" != typeof this[t] && (a += t + "=" + encodeURIComponent(this[t]) + "&"); return a.replace(/&$/, "") }
            var e;
            e = jQuery.fn.flash = function(a, t, n, r) {
                var s = n || e.replace;
                if (t = e.copy(e.pluginOptions, t), !e.hasFlash(t.version))
                    if (t.expressInstall && e.hasFlash(6, 0, 65)) var i = { flashvars: { MMredirectURL: location, MMplayerType: "PlugIn", MMdoctitle: jQuery("title").text() } };
                    else {
                        if (!t.update) return this;
                        s = r || e.update
                    }
                return a = e.copy(e.htmlOptions, i, a), this.each(function() { s.call(this, e.copy(a)) })
            }, e.copy = function() {
                for (var a = {}, t = {}, e = 0; e < arguments.length; e++) {
                    var n = arguments[e];
                    void 0 != n && (jQuery.extend(a, n), void 0 != n.flashvars && jQuery.extend(t, n.flashvars))
                }
                return a.flashvars = t, a
            }, e.hasFlash = function() { if (/hasFlash\=true/.test(location)) return !0; if (/hasFlash\=false/.test(location)) return !1; for (var a = e.hasFlash.playerVersion().match(/\d+/g), t = String([arguments[0], arguments[1], arguments[2]]).match(/\d+/g) || String(e.pluginOptions.version).match(/\d+/g), n = 0; 3 > n; n++) { if (a[n] = parseInt(a[n] || 0), t[n] = parseInt(t[n] || 0), a[n] < t[n]) return !1; if (a[n] > t[n]) return !0 } return !0 }, e.hasFlash.playerVersion = function() { try { try { var a = new ActiveXObject("ShockwaveFlash.ShockwaveFlash.6"); try { a.AllowScriptAccess = "always" } catch (t) { return "6,0,0" } } catch (t) {} return new ActiveXObject("ShockwaveFlash.ShockwaveFlash").GetVariable("$version").replace(/\D+/g, ",").match(/^,?(.+),?$/)[1] } catch (t) { try { if (navigator.mimeTypes["application/x-shockwave-flash"].enabledPlugin) return (navigator.plugins["Shockwave Flash 2.0"] || navigator.plugins["Shockwave Flash"]).description.replace(/\D+/g, ",").match(/^,?(.+),?$/)[1] } catch (t) {} } return "0,0,0" }, e.htmlOptions = { height: 240, flashvars: {}, pluginspage: "http://www.adobe.com/go/getflashplayer", src: "#", type: "application/x-shockwave-flash", width: 320 }, e.pluginOptions = { expressInstall: !1, update: !0, version: "6.0.65" }, e.replace = function(a) { this.innerHTML = '<div class="alt">' + this.innerHTML + "</div>", jQuery(this).addClass("flash-replaced").prepend(e.transform(a)) }, e.update = function(a) {
                var t = String(location).split("?");
                t.splice(1, 0, "?hasFlash=true&"), t = t.join("");
                var e = '<p>This content requires the Flash Player. <a href="http://www.adobe.com/go/getflashplayer">Download Flash Player</a>. Already have Flash Player? <a href="' + t + '">Click here.</a></p>';
                this.innerHTML = '<span class="alt">' + this.innerHTML + "</span>", jQuery(this).addClass("flash-update").prepend(e)
            }, e.transform = function(e) { return e.toString = a, e.flashvars && (e.flashvars.toString = t), "<embed " + String(e) + "/>" }, window.attachEvent && window.attachEvent("onbeforeunload", function() { __flash_unloadHandler = function() {}, __flash_savedUnloadHandler = function() {} })
        }();
        /* eslint-enable */
    }

    function setSize(object) {
        var oldHeight = object.attr('height');
        var oldWidth = object.attr('width');
        var newWidth = object.width();
        var newHeight = oldHeight * newWidth / oldWidth;
        object.height(newHeight);
    }

    function initFlash(component, properties) {
        var content = component.find('.component-content > div');
        content.flash(properties);
    }

    function attachEvents(component) {
        $(window).load(function() {
            var object = component.find('embed');
            object.css('width', '100%');
            setSize(object);

            $(window).resize(function() {
                setSize(object);
            });
        });
    }

    api.init = function() {
        var flash = $('.flash:not(.initialized)');
        if (flash.length > 0) {
            loadjQueryFlash();
            flash.each(function() {
                var properties = $(this).data('properties');
                initFlash($(this), properties);
                attachEvents($(this));

                $(this).addClass('initialized');
            });
        }
    };

    return api;
}(jQuery, document));

XA.register('flash', XA.component.flash);