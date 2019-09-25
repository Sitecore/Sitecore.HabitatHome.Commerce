/* global mejs:false */
/**
 * Component Playlist
 * @module Playlist
 * @param  {jQuery} $ Instance of jQuery
 * @return {Object} List of Playlist component methods
 */
XA.component.playlist = (function($) {
    /**
     * In this object stored all public api methods
     * @type {Object.<Methods>}
     * @memberOf module:Playlist
     * */
    var api = {};
    /**
     * This class used by [Playlist]{@link module:Playlist} module
     * @class Playlist
     * @memberOf module:Playlist
     * @param {jQuery} playlist Root DOM element of playlist component wrapped by jQuery
     * @param {Object} properties Properties setted in data attribute
     */
    function Playlist(playlist, properties) {
        this.properties = properties;
        this.playlist = playlist;
        this.activeVideo = 0;
        this.playlistItems = 0;
    }
    /**
     * Create new source element, seted up it with type & src. Created element
     * append to Video container(take it from videoContainer parameter)
     * @method
     * @alias createNewSources
     * @memberOf module:Playlist.Playlist
     * @param {$OrderedList} source takes from properties.sources[itemIndex].src - 
     * source of playlist element
     * @param {$OrderedList} videoContainer container where source should be added
     */
    Playlist.prototype.createNewSources = function(source, videoContainer) {
        var newSource;

        var sourceBuilder = function(path) {
            var newSource = $("<source>"),
                type;

            if (path.match(/\.(mp4)$/)) {
                type = "video/mp4";
            } else if (path.match(/\.(webm)$/)) {
                type = "video/webm";
            } else if (path.match(/\.(ogv)$/)) {
                type = "video/ogg";
            } else {
                type = "video/youtube";
            }

            newSource.attr({
                type: type,
                src: path
            });

            return newSource;
        };

        if (source instanceof Array) {
            for (var i = 0; i < source.length; i++) {
                newSource = sourceBuilder(source[i]);
                videoContainer.find("video").append(newSource);
            }
        } else {
            newSource = sourceBuilder(source);
            videoContainer.find("video").append(newSource);
        }
    };
    /**
     * Replace source of each video element under play list component.
     * After replacing it reinitialized video component by calling 
     * [XA.component.video.initVideoFromPlaylist]{@link module:Video.initVideoFromPlaylist}
     * @name replaceSource
     * @function
     * @param {number} itemIndex position of active video in video list
     * @param {boolean} loadFromEvent
     * @memberOf module:Playlist.Playlist
     */

    Playlist.prototype.replaceSource = function(itemIndex, loadFromEvent) {
        var inst = this,
            videoContainer,
            videoClone,
            newSrc = inst.properties.sources[itemIndex].src,
            sources,
            videoId,
            videoContainerHeight = 0;

        $(inst.properties.playlistId).each(function() {
            videoContainer = $(this);
            videoId = inst.properties.playlistId;

            if (videoContainer.is(videoId) && newSrc.length) {
                videoContainer.addClass("show");

                sources = videoContainer.find("source");
                sources.remove();
                inst.createNewSources(newSrc, videoContainer);
                videoContainer
                    .find("video")
                    .attr({
                        src: ""
                    })
                    .show();

                var autoplayVideo = false;
                if (loadFromEvent) {
                    if (inst.properties.autoPlaySelected) {
                        autoplayVideo = true;
                    }
                } else {
                    if (inst.properties.autoPlay) {
                        autoplayVideo = true;
                    }
                }

                if (autoplayVideo) {
                    videoContainer.find("video").attr({
                        autoplay: ""
                    });
                }

                videoClone = videoContainer.find("video").clone();
                videoContainerHeight = videoContainer.height();
                videoContainer.css({
                    height: videoContainerHeight
                });

                var id = videoContainer.find(".mejs-container").attr("id");
                if (id) {
                    $("#" + id).remove();
                    delete mejs.players[id];
                    videoContainer
                        .find(".component-content")
                        .append(videoClone);
                }

                XA.component.video.initVideoFromPlaylist(
                    videoContainer,
                    inst.playlist
                );
                videoContainer.css({
                    height: "auto"
                });
            }
        });
    };

    /**
     * Inititalise videos under component. On call and on
     * [change-video]{@link module:Playlist.Playlist#change-video} event
     * calls [replaceSource]{@link module:Playlist.Playlist.replaceSource}
     * @name loadPlaylistVideo
     * @function
     * @listeners module:Playlist.Playlist#change-video
     * @memberOf module:Playlist.Playlist
     */

    Playlist.prototype.loadPlaylistVideo = function() {
        var inst = this,
            playlistItems = $(inst.playlist).find(".playlist-item"),
            activeListItem;

        inst.playlistItems = playlistItems.length;
        var loadVideoFromPlaylist = function(loadFromEvent) {
            inst.replaceSource(inst.activeVideo, loadFromEvent);
            activeListItem = playlistItems.eq(inst.activeVideo);
            activeListItem.addClass("active");
            activeListItem.siblings().removeClass("active");
        };

        loadVideoFromPlaylist();
        $(inst.playlist).on("change-video", function(event, properties) {
            var loadNewVideo = false;

            if (properties) {
                if (properties.hasOwnProperty("back")) {
                    inst.activeVideo--;
                    if (inst.activeVideo < 0) {
                        inst.activeVideo = 0;
                    } else {
                        loadNewVideo = true;
                    }
                } else {
                    inst.activeVideo++;
                    if (inst.activeVideo === inst.playlistItems) {
                        if (inst.properties.repeatAfterAll) {
                            inst.activeVideo = 0;
                            loadNewVideo = true;
                        } else {
                            inst.activeVideo = inst.playlistItems - 1;
                        }
                    } else {
                        loadNewVideo = true;
                    }
                }
            } else {
                if (inst.properties.playNext) {
                    if (inst.activeVideo + 1 <= inst.playlistItems) {
                        inst.activeVideo++;

                        if (inst.activeVideo === inst.playlistItems) {
                            inst.activeVideo = 0;

                            if (inst.properties.repeatAfterAll) {
                                loadNewVideo = true;
                            }
                        } else {
                            inst.actiVideo--;
                            loadNewVideo = true;
                        }
                    }
                }
            }

            if (loadNewVideo) {
                loadVideoFromPlaylist(true);
            }
        });
    };
    /**
     * Attach events to playlist-section and nav items
     * @name attachEvents
     * @function
     * @fires module:Playlist.Playlist#change-video
     * @memberOf module:Playlist.Playlist
     */
    Playlist.prototype.attachEvents = function() {
        var inst = this,
            link = $(inst.playlist).find(".playlist-section"),
            navItems = $(inst.playlist).find(".playlist-nav a"),
            playlistItem;
        link.on("click", function(event) {
            event.preventDefault();

            playlistItem = $(this).parents(".playlist-item");
            var itemIndex = playlistItem.index();

            if (itemIndex !== inst.activeVideo) {
                playlistItem.addClass("active");
                playlistItem.siblings().removeClass("active");
                inst.replaceSource(itemIndex, true);
                inst.activeVideo = itemIndex;
            }
        });

        navItems.on("click", function(event) {
            event.preventDefault();
            var properties = {};

            if (
                $(this)
                    .parent()
                    .hasClass("playlist-prev")
            ) {
                properties.back = true;
            }
            /**
             * Change video event fires when user click
             * on a ".playlist-nav a" element.
             *
             * @event module:Playlist.Playlist#change-video
             * @type {object}
             * @property {Object} properties - Indicates whether playlist has back button
             */
            $(inst.playlist).trigger("change-video", properties);
        });
    };
    /**
     * For each playlist component create new instance of
     * ["Playlist"]{@link  module:Playlist.Playlist} and call
     * ["loadPlaylistVideo"]{@link  module:Playlist.Playlist.loadPlaylistVideo},
     * ["attachEvents"]{@link  module:Playlist.Playlist.attachEvents}
     * methods
     * @memberOf module:Playlist
     * @method
     * @param {jQuery} component Root DOM element of playlist component wrapped by jQuery
     * @param {Object} prop Properties setted in data attribute
     *  of playlist component
     * @alias module:Playlist.initInstance
     */
    api.initInstance = function(component, prop) {
        var playlist;
        $(prop.playlistId).addClass("initialized"); //prevent video init in component-video.js
        if (prop.sources.length) {
            playlist = new Playlist(component, prop);
            playlist.loadPlaylistVideo();
            playlist.attachEvents();
        }
    };
    /**
     * Find all not initialized yet
     * Playlist components and in a loop for each of them
     * run ["initInstance"]{@link module:Playlist.initInstance}
     * method.
     * @memberOf module:Playlist
     * @alias module:Playlist.init
     */
    api.init = function() {
        var playlists = $(".playlist.component:not(.initialized)"),
            properties;

        playlists.each(function() {
            properties = $(this).data("properties");
            api.initInstance(this, properties);
            $(this).addClass("initialized");
        });
    };

    return api;
})(jQuery, document);

XA.register("playlist", XA.component.playlist);
