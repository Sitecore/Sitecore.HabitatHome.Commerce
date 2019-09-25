// ----- Price Information View Model -----
function PriceInfoViewModel(rootElement) {
  var self = this;
  self.rootElement = rootElement;

  self.priceBefore = ko.observable();
  self.priceNow = ko.observable();
  self.savingsMessage = ko.observable();

  self.switchInfo = function (data) {
    var $rootElement = $(self.rootElement);
    var currentProductId = $rootElement.find('.price-info').attr('productid');

    if (data) {
      if (data.productId && currentProductId === data.productId) {
        self = this;
        self.priceNow(data.adjustedPrice);
        self.priceBefore(data.listPrice);
        self.savingsMessage(data.savingsMessage);
        if (!data.isOnSale) {
          $rootElement.find(".price-now-before").hide();
          $rootElement.find(".price-only").show();
        } else {
          $rootElement.find(".price-now-before").show();
          $rootElement.find(".price-only").hide();
        }
      }
    }
  };
}
