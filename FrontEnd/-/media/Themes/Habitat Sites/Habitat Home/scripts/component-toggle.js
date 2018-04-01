XA.component.toggle = (function($) {
    var api = {

    };

    var properties, instance;

    var getFlipList = function() {
        return instance.find('.component.flip');
    }

    var openToggle = function() {
        var flipList = getFlipList();
        instance.find('details').attr('open', 'open');
        if (flipList.lengt) {
            try {
                XA.component.flip.equalSideHeightInToggle(instance)
            } catch (e) {
                /* eslint-disable no-console */
                console.warn('Error during calculation height of Flip list in toggle'); // jshint ignore:line
                /* eslint-enable no-console */

            }
        }

    }
    var closeToggle = function() {
        instance.find('details').removeAttr('open');
    }

    var setAnimation = function() {
        instance.find('details summary~.component>.component-content').css({
            'animation-name': properties.easing,
            'animation-duration': (properties.speed || 500) + 'ms'
        })
    }
    var isExpandOnHover = function() {
        return properties.expandOnHover;
    }
    var isExpanded = function() {
        return properties.expandedByDefault;
    }
    var bindEvents = function() {
        var summary = instance.find('summary');
        if (isExpandOnHover()) {
            summary.on('mouseenter', function() {
                openToggle();
            })
        }
        if (isExpanded()) {
            openToggle()
        }

        summary.on('click', function(event) {
            event.preventDefault();
            var details = $(this).closest('details')
            if (details.attr('open')) {
                closeToggle();
            } else {
                openToggle();
            }

        })

    }

    var initToggle = function(component) {
        bindEvents(component)
        setAnimation(component);
    };

    api.init = function() {
        $('.toggle:not(.initialized)').each(function() {
            instance = $(this);
            properties = instance.data('properties');
            initToggle(this);
            instance.addClass('initialized');
        });
    };

    return api;
})(jQuery);

XA.register('component-toggle', XA.component.toggle);