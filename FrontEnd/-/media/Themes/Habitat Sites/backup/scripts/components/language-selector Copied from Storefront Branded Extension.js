$(document).ready(function() {
  let $component = $('.cxa-languageselector-component');
  let $availableLanguages = $component.find('.available-languages');
  let $languageIcon = $component.find('.component-content');
  let hoverTimeout;

  function showContent() {
    clearTimeout(hoverTimeout);
    $availableLanguages
      .stop()
      .fadeTo('fast', 1)
      .show();
  }

  function hideContent() {
    hoverTimeout = setTimeout(function() {
      $availableLanguages.fadeTo('slow', 0).hide();
    }, 200);
  }

  if (isMobile()) {
    function targetExitedBoundary(target) {
      const isLanguageIcon = $languageIcon.is($(target));
      const isLanguageIconDescendant = $component
        .toArray()
        .some(function(component) {
          return $.contains(component, target);
        });
      return !(isLanguageIcon || isLanguageIconDescendant);
    }

    $(window)
      .off('touchend.languageSelector')
      .on('touchend.languageSelector', function(e) {
        if (targetExitedBoundary(e.target)) {
          $availableLanguages.hide();
        }
      });

    $languageIcon.off('touchend').on('touchend', function() {
      if ($availableLanguages.css('display') === 'none') {
        showContent();
      } else {
        hideContent();
      }
    });
  } else {
    $languageIcon.hover(showContent, hideContent);
  }

  $availableLanguages.hide();
});
