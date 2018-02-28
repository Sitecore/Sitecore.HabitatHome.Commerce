XA.component.disqus = (function($, document) {

    var api = {};

    function initDisqus(prop) {
        var dsq = document.createElement('script');
        dsq.type = 'text/javascript';
        dsq.async = true;
        dsq.src = '//' + prop.disqus_shortname + '.disqus.com/embed.js';
        (document.getElementsByTagName('head')[0] ||
            document.getElementsByTagName('body')[0]).appendChild(dsq);
    }


    api.init = function() {
        var disqus = $('.disqus:not(.initialized)');

        disqus.each(function() {
            var properties = $(this).data('properties');

            window.disqus_config = function() {
                this.page.url = properties.disqus_url;
                this.page.identifier = properties.disqus_identifier;
                this.page.title = properties.disqus_title;
                this.page.category_id = properties.disqus_category_id;
            };

            if ($(this).find("#disqus_thread").length > 0) {
                initDisqus(properties);
            }

            $(this).addClass('initialized');
        });
    };

    return api;
}(jQuery, document));

XA.register('disqus', XA.component.disqus);