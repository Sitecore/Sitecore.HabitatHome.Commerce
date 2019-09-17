/* global googleCalendarApiKey:true */
/**
 * Component Fullcalendar
 * @module Fullcalendar
 * @param  {jQuery} $ Instance of jQuery
 * @return {Object} List of archive methods
 */
XA.component.calendar = (function($) {
    /**
     * This class used by [Fullcalendar]{@link module:Fullcalendar} module for
     * storing events option
     * @class GetEvents
     * @memberOf module:Fullcalendar
     * @param {string} selector selector of event element
     * @param {Object} options options of component
     */
    function GetEvents(selector, options) {
        this.data = options.data;
        this.selector = selector;
        this.options = options;
        this.events = [];

        this.checkSource();
    }
    /**
     * checkSource method call ["getJson"]{@link module:Fullcalendar.getJson} 
     * if "options.dataType" equal "json"
     * and create instance of 
     * ["InitCalendar"]{@link module:Fullcalendar.InitCalendar} class 
     * @method
     * @alias checkSource
     * @memberOf module:Fullcalendar.GetEvents
     */
    GetEvents.prototype.checkSource = function() {
        var inst = this;

        switch (inst.options.dataType) {
            case "json":
                inst.getJson();
                new InitCalendar(inst.selector, inst.options, inst.events);
                break;
            case "gcalendar":
                new InitCalendar(inst.selector, inst.options, inst.events);
                break;
        }
    };
    /**
     * getJson method fill array of events with
     * new event
     * @method
     * @alias getJson
     * @memberOf module:Fullcalendar.GetEvents
     */
    GetEvents.prototype.getJson = function() {
        var inst = this,
            date,
            dateEnd,
            allDay = false,
            tempObj = [];

        $.each(inst.data, function() {
            date = new Date(this.eventStart);
            dateEnd = new Date(this.eventEnd);

            if (date === dateEnd) {
                allDay = true;
            }

            tempObj = {
                title: this.eventName,
                start: date,
                end: dateEnd,
                eventDescription: this.eventDescription,
                eventLink: this.eventLink,
                eventClass: this.eventClass
            };

            inst.events.push(tempObj);
        });
    };

    /**
     * This class used by
     * [GetEvents]{@link module:Fullcalendar.GetEvents} module and
     * initialize instances of calendar with predefined list of options
     * @class InitCalendar
     * @memberOf module:Fullcalendar
     * @param {string} selector DOM Root element of
     * @param {Object} options options of component
     * @param {Array} events options of event calendar ["event object"]{@link https://fullcalendar.io/docs/event-object}
     */
    function InitCalendar(selector, options, events) {
        var inst = this,
            prevNext = "",
            title = "",
            calendarTypes = "";

        if (options.dataType === "gcalendar") {
            googleCalendarApiKey = options.calendarApiKey;
            events = options.calendarId;
        } else {
            googleCalendarApiKey = null;
        }

        options.showPrevNext ? (prevNext = "prev, next") : "";
        options.showMonthCaptions ? (title = "title") : "";

        for (var i in options.calendarTypes) {
            if (options.calendarTypes[i] === "day") {
                options.calendarTypes[i] = "basicDay";
            } else if (options.calendarTypes[i] === "week") {
                options.calendarTypes[i] = "basicWeek";
            }
        }

        if (options.calendarTypes.length > 1) {
            calendarTypes = options.calendarTypes.join();
        }

        $(selector).fullCalendar({
            monthNames: options.localization.monthNames,
            monthNamesShort: options.localization.monthNamesShort,
            dayNames: options.localization.dayNames,
            dayNamesShort: options.localization.dayNamesShort,
            nextDayThreshold: "00:00",

            buttonText: {
                agendaDay: "agenda day",
                agendaWeek: "agenda week"
            },

            header: {
                left: prevNext,
                center: title,
                right: calendarTypes
            },

            googleCalendarApiKey: googleCalendarApiKey,
            events: events,
            renderEvent: false,
            eventRender: function(event, element) {
                if (options.compactView && options.dataType === "json") {
                    $(element).css("display", "none");
                } else {
                    if (options.dataType === "json") {
                        inst.attachTooltip(event, element, false);
                    }
                }

                element.addClass(event.eventClass);
            },
            eventAfterAllRender: function() {
                if (options.compactView && options.dataType === "json") {
                    inst.renderCompactCalendarEvents(selector, events);
                }
            }
        });
    }
    /**
     * attachTooltip method is create, positioning and 
     * animate tooltip for events
     * @method
     * @alias attachTooltip
     * @memberOf module:Fullcalendar.InitCalendar
     * @param {Object} event options of event calendar ["event object"]{@link https://fullcalendar.io/docs/event-object}
     * @param {DOM Element} element DOM element of an event
     * @param {boolean} compactCalendar  is calendar in compact mode
     */
    InitCalendar.prototype.attachTooltip = function(
        event,
        element,
        compactCalendar
    ) {
        var $tooltip, tooltipContent;

        $(element).on("mouseenter", function() {
            tooltipContent = "";
            $(".calendar-tooltip").fadeOut();
            $(".calendar-tooltip").remove();

            if (compactCalendar) {
                tooltipContent = "";
                $.each(event, function() {
                    tooltipContent +=
                        "<div class='compact-event'>" +
                        "<span class='title'>" +
                        this.title +
                        "</span>" +
                        "<span class='description'>" +
                        this.eventDescription +
                        "</span>" +
                        "<span class='link'><a href='" +
                        this.eventLink +
                        "'>Link</a></span></div>";
                });
            } else {
                tooltipContent =
                    "<span class='description'>" +
                    event.eventDescription +
                    "</span>" +
                    "<span class='link'>" +
                    event.eventLink +
                    "</span>";
            }
            $tooltip = $(
                "<div style='border-radius:5px;border:1px solid #000;position:absolute;z-index:999; paading:5px;background:#FFF' class='calendar-tooltip'><div class='arrow'>" +
                    "</div><div class='events'>" +
                    tooltipContent +
                    "</div></div>"
            );

            $tooltip.css({
                left: $(this).offset().left
            });
            $tooltip.css({
                top: $(this).offset().top - $(this).height()
            });
            $("body").append($tooltip);

            var timeout;
            $(this).unbind("mouseleave");
            $(this).on("mouseleave", function() {
                timeout = setTimeout(function() {
                    $tooltip.fadeOut(function() {
                        $(this).remove();
                    });
                }, 300);

                $tooltip.unbind("mouseenter");
                $tooltip.on("mouseenter", function() {
                    clearTimeout(timeout);
                });
            });

            $tooltip.unbind("mouseleave");
            $tooltip.on("mouseleave", function() {
                $(this).fadeOut(function() {
                    $(this).remove();
                });
            });
        });
    };

     /**
    * attach events for single days - compact calendar
     * @method 
     * @alias renderCompactCalendarEvents
     * @memberOf module:Fullcalendar.InitCalendar
     * @param {Array} events  ["event object"]{@link https://fullcalendar.io/docs/event-object}
     * @param {string} selector selector of an event in DOM
     */
    InitCalendar.prototype.renderCompactCalendarEvents = function(
        selector,
        events
    ) {
        var inst = this,
            currentDay,
            currentDate,
            currentEvent,
            dc,
            mc,
            yc,
            d,
            m,
            y,
            de,
            me,
            ye,
            he,
            startDate,
            endDate,
            dayEvents = [];

        $(selector)
            .find(".fc-day")
            .each(function() {
                currentDay = this;

                currentDate = new Date($(this).data("date"));
                dc = currentDate.getDate();
                mc = currentDate.getMonth();
                yc = currentDate.getFullYear();

                dayEvents = [];
                $.each(events, function() {
                    currentEvent = this;
                    startDate = new Date(this.start);
                    d = startDate.getDate();
                    m = startDate.getMonth();
                    y = startDate.getFullYear();

                    if (this.end) {
                        endDate = new Date(this.end);
                        de = endDate.getDate();
                        me = endDate.getMonth();
                        ye = endDate.getFullYear();
                        he = endDate.getHours();
                    }

                    if (
                        yc >= y &&
                        yc <= ye &&
                        mc >= m &&
                        mc <= me &&
                        dc >= d &&
                        dc <= de
                    ) {
                        if (yc == ye && mc == me && dc == de && he < 9) {
                            // If last day and hour < 9 do nothing
                        } else {
                            $(currentDay).addClass("selected-day");
                            dayEvents.push(currentEvent);
                        }
                    }
                });

                if (dayEvents.length) {
                    inst.attachTooltip(dayEvents, currentDay, true);
                }
            });
    };
    /**
     * resizeCalendar method run render of a calendar
     * @memberOf module:Fullcalendar
     * @param {string} selector  Selector of a calendar component
     * @private
     */
    function resizeCalendar(selector) {
        $(selector).fullCalendar("render");
    }
    /**
     * In this object stored all public api methods
     * @type {Object.<Methods>}
     * @memberOf module:Fullcalendar
     * */
    var api = {};
    /**
     * initInstance method create instance of
     * ["GetEvents"]{@link module:Fullcalendar.GetEvents} class and bind
     * ["resizeCalendar"]{@link module:Fullcalendar.resizeCalendar} to window resize
     * event
     * @memberOf module:Fullcalendar
     * @alias module:Fullcalendar.initInstance
     */
    api.initInstance = function(component, prop) {
        var selector = "#" + component.find(".event-calendar-inner").attr("id");
        if (prop.compactView && prop.dataType === "json") {
            $(this).addClass("compact-mode");
        }

        new GetEvents(selector, prop);

        $(window).resize(function() {
            resizeCalendar(selector);
        });
    };

    /**
     * init method calls in a loop for each
     * full calendar component on a page and run Full calendar's
     * ["initInstance"]{@link module:Fullcalendar.api.initInstance} method
     * @memberOf module:Fullcalendar
     * @alias module:Fullcalendar.init
     */
    api.init = function() {
        $(".event-calendar:not(.initialized)").each(function() {
            var properties = $(this).data("properties");
            api.initInstance($(this), properties);
            $(this).addClass("initialized");
        });
    };

    return api;
})(jQuery, document);

XA.register("calendar", XA.component.calendar);
