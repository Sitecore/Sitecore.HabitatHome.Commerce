$(document).ready(function () {
  let $topbar = $('.cxa-topbarlinks-component');
  let $topbarItems = $topbar.find('ul');
  let $topbarContent = $topbar.find('.component-content');
  let $logoutLink = $('a[href*="/api/cxa/account/logoff"]');
  let hoverTimeout;

  function showContent() {
    clearTimeout(hoverTimeout);
    $topbarItems
      .stop()
      .fadeTo('fast', 1)
      .show();
  }

  function hideContent() {
    hoverTimeout = setTimeout(function () {
      $topbarItems.fadeTo('slow', 0).hide();
    }, 200);
  }

  $logoutLink.on('click', function (event) {
    CartContext.DeleteCachedCartCount();
  });

  if (isMobile()) {
    let targetExitedBoundary = function (target) {
      const isTopBar = $topbarContent.is($(target));
      const isTopBarDescendant = $.contains($topbar.get(0), target);
      return !(isTopBar || isTopBarDescendant);
    };

    $(window)
      .off('touchend.topbar')
      .on('touchend.topbar', function (e) {
        if (targetExitedBoundary(e.target)) {
          $topbarItems.hide();
        }
      });

    $topbarContent
      .off('touchend')
      .on('touchend', 'a', function (e) {
        e.stopPropagation();
      })
      .on('touchend', function () {
        if ($topbarItems.css('display') === 'none') {
          showContent();
        } else {
          hideContent();
        }
      });
  } else {
    $topbarContent.hover(showContent, hideContent);
  }

  $topbarItems.hide();
});
