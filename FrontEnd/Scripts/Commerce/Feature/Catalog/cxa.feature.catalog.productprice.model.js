// ----- Price Information View Model -----
var PriceInfoViewModel = function () {
    self = this;

    self.priceBefore = ko.observable();
    self.priceNow = ko.observable();
    self.savingsMessage = ko.observable();

    self.switchInfo = function (priceBefore, priceNow, isOnSale, savingsMessage) {
        self = this;
        self.priceNow(priceNow);
        self.priceBefore(priceBefore);
        self.savingsMessage(savingsMessage);

        if (!isOnSale) {
            $(".price-now-before").hide()
            $(".price-only").show();
        }
        else {
            $(".price-now-before").show()
            $(".price-only").hide();
        }
    };
}
