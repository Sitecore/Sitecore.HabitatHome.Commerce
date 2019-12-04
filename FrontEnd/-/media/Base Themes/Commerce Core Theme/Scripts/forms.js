(function (root, $, factory) {
  factory($);
}(window, window.jQuery, function ($) {
  $(document).ready(function () {
    var components = $('.component-content');
    var forms = components.find('form');
    $(forms).each(function () {
      var form = this;
      $(this).on('submit', function () {
        if ($(form).valid()) {
          window.MessageContext.ClearAllMessages();

          var submitButton = $(this).find(':submit');
          if (!$(submitButton).hasClass('search-button')) {
            $(submitButton).button('loading');
          }

        }

      });
      if (!CXAApplication.IsExperienceEditorMode()) {
        $(document).on('ajaxComplete', function () {
          var submitButton = $(forms).find(':submit');
          $(submitButton).button('reset');
        });
        $(document).on('ajaxError', function () {
          var submitButton = $(forms).find(':submit');
          $(submitButton).button('reset');
        });
      }
    });
  });
}));
