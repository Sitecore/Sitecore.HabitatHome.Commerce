$(function () {
  function getCurrentPageExtension() {
    var pageExtensionWithQueryString = window.location.href.split(".").pop();
    if (pageExtensionWithQueryString && pageExtensionWithQueryString.length >= 4) {
      var pageExtension = pageExtensionWithQueryString.substr(0, 4);
      return pageExtension;
    }
    return "";
  }

  var productImagesComponents = $('.cxa-productimages-component');
  productImagesComponents.each(function () {
    var component = $(this);
    var list = component.find('.product-images li');
    list.first().addClass('active');

    list.click(function (e) {
      list.removeClass('active');
      e.preventDefault();
      $(this).addClass('active');

      if (list.length > 1) {
        var imageSrc = $(this).find('a')
          .attr('href');
        var isCreativeExchangeVersion = getCurrentPageExtension() === 'html';

        if (isCreativeExchangeVersion) {
          var linkChunks = imageSrc.split('/');
          var imageNameWithPrefix = linkChunks.pop().split('.')[0];
          if (imageNameWithPrefix) {
            var imageNameParts = imageNameWithPrefix.split('-');
            if (imageNameParts.length >= 2) {
              var imageName = imageNameWithPrefix.split('-')[0];
              imageSrc = imageSrc.replace(imageNameWithPrefix, imageName);
            }
          }
        }

        component.find(".product-image").find("img")
          .attr('src', imageSrc);
      }
    });
  });
});
