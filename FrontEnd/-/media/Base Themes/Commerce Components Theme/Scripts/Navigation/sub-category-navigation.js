$(function () {
  var categoryNav = $('.component.cxa-subcategorynavigation-component');
  var categoryNavList = categoryNav.find("ul");

  categoryNav.find(".subcategories-title").click(
    function () {
      var clicks = $(this).data('clicks');
      if (!clicks) {
        categoryNavList.fadeTo("fast", 0);
        categoryNavList.hide();
        $(this).data('clicks', 1);
      } else {
        categoryNavList.show();
        categoryNavList.fadeTo("fast", 1);
        $(this).removeData();
      }
    }
  );
});