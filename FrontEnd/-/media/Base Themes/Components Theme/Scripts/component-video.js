/* global MediaElementPlayer:false */
/**
 * Component Video
 * @module Video
 * @param  {jQuery} $ Instance of jQuery
 * @return {Object} List of Video component methods
 */
XA.component.video = (function($, document) {
    /**
     * In this object stored all public api methods
     * @type {Object.<Methods>}
     * @memberOf module:Video
     * */
    var api = {};
    /**
     * Set class name to video element depend 
     * in video width size. </br>
        <p>"video-small" - if videoWidth < 481 && videoWidth >= 321</p>
        <p>"hide-controls" - if videoWidth < 321</p>
     * @memberOf module:Video
     * @method
     * @param {jQuery.<Element>} video Video element
     */
    function checkSize(video) {
        var videoWidth = video.width();
        video.removeClass("video-small hide-controls");

        if (videoWidth < 481 && videoWidth >= 321) {
            video.addClass("video-small");
        } else if (videoWidth < 321) {
            video.addClass("hide-controls");
        }
    }

    /**
     * Initialize video component and create instance of 
     * [MediaElementPlayer]{@link https://www.mediaelementjs.com/} that 
     * handle all work with video player.
     * @memberOf module:Video
     * @method
     * @param {jQuery.<Element>} video Video element
     * @param {Object} properties Video element properties
     * @return undefined;
     * 
     */
    function initVideo(video, properties) {
        var content = video.find("video");

        if (!content.length) {
            return;
        }
       
        var callback = function(mediaElement) {
            mediaElement.movieName = "Movie";

            $(mediaElement).on("ended", function() {
                if (properties.fromPlaylist) {
                    $(properties.playlist).trigger("change-video");
                }
            });
            $(mediaElement)
                .closest(".component-content")
                .find(".video-init")
                .hide();
        };

        $.extend(properties, {
            plugins: ["youtube", "flash", "silverlight"],
            silverlightName: "silverlightmediaelement.xap",
            classPrefix: "mejs-",
            success: callback,
            stretching: "auto",
            pluginPath: "../other/"
        });

        content.each(function(key) {
            var $elem = $(content[key]),
                _this = this;
            if ($elem.attr("poster")) {
                var initBtn = $elem.parent().find(".video-init");
                $elem.add(initBtn).on("click", function() {
                    properties.success = (function(arg) {
                        return function() {
                            try {
                                arguments[0].load();
                                arguments[0].play();
                            } catch (e) {
                                /* eslint-disable */
                                console.warn("Error while loading video");
                                /* eslint-enable  */
                            }

                            return arg.apply(this, arguments);
                        };
                    })(properties.success);
                    initBtn.hide();
                    new MediaElementPlayer(_this, properties);
                });
            } else {
                new MediaElementPlayer(content[key], properties);
            }
        });
        return;
    }
    /**
     * This methods called by 
     * [Playlist.replaceSource]{@link module:Playlist.Playlist.replaceSource}
     * and extend properties of video component.
     * @memberOf module:Video
     * @method
     * @param {jQuery} video Root DOM element of video component wrapped by jQuery
     * @param {Object} playlist setted by playlist component
     * @alias module:Video.initVideoFromPlaylist
     */
    api.initVideoFromPlaylist = function(video, playlist) {
        var properties = $(video).data("properties");

        $.extend(properties, {
            fromPlaylist: true,
            playlist: playlist
        });

        return initVideo(video, properties);
    };
    /**
     * For each video component run
     * [initVideo]{@link module:Video.initVideo}, [checkSize]{@link module:Video.checkSize}
     * methods and add [checkSize]{@link module:Video.checkSize} as a callback for
     * eventListener to <b>window.resize</b>
     * @memberOf module:Video
     * @method
     * @param {jQuery} component Root DOM element of video component wrapped by jQuery
     * @param {Object} prop Properties setted in data attribute
     *  of video component
     * @alias module:Video.initInstance
     */
    api.initInstance = function(component, prop) {
        var $component = $(component);
        initVideo($component, prop);
        checkSize($component);

        $(window).resize(function() {
            checkSize($component);
        });
    };
    /**
     * Find all not initialized yet Video components and in a loop for each of them
     * run ["initInstance"]{@link module:Video.initInstance} method.
     * In case if component has property "playlist installed" it call
     * [XA.component.playlist.init]{@link module:Playlist.init}
     * @memberOf module:Video
     * @alias module:Video.init
     */
    api.init = function() {
        if (XA.component.hasOwnProperty("playlist")) {
            XA.component.playlist.init();
        }

        var video = $(".video.component:not(.initialized)");

        video.each(function() {
            var properties = $(this).data("properties");

            api.initInstance(this, properties);

            $(this).addClass("initialized");
        });

        $(document).on("mozfullscreenchange", function() {
            setTimeout(function() {
                $(window).resize();
            }, 200); //mozilla bug fix
        });
    };

    return api;
})(jQuery, document);

XA.register("video", XA.component.video);
