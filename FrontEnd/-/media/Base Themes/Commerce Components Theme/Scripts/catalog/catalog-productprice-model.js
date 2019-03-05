// ----- Price Information View Model -----
function PriceInfoViewModel(rootElement) {
    var self = this;
    self.rootElement = rootElement;

    self.priceBefore = ko.observable();
    self.priceNow = ko.observable();
    self.savingsMessage = ko.observable();

    self.switchInfo = function (priceBefore, priceNow, isOnSale, savingsMessage, data) {
        var currentProductId = $(self.rootElement).find('#productprice_productid').val();

        if (data) {
            if (data.productId && currentProductId === data.productId) {
                self = this;
                self.priceNow(priceNow);
                self.priceBefore(priceBefore);
                self.savingsMessage(savingsMessage);
                if (!isOnSale) {
                    $(self.rootElement).find(".price-now-before").hide();
                    $(self.rootElement).find(".price-only").show();
                } else {
                    $(self.rootElement).find(".price-now-before").show();
                    $(self.rootElement).find(".price-only").hide();
                }
            }
        }
    };
}
