$(document).ready(function () {
  (function loadFirstSlide() {
    var url = window.location.href.toString();
    var isIndexZero = url.includes('carousel=0');
    isIndexZero ? showFirstSlide() : false;
  })();

  function showFirstSlide() {
    setTimeout(function () {
      $('.slides')
        .find('li')
        .first()
        .show();
    }, 1000);
  }
});