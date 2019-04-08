﻿function ConfirmShippingMethod(id, description) {
    var self = this;

    self.id = id;
    self.description = description;
}

function ConfirmCreditCardData(data, confirmViewModel) {
    var self = this;

    self.confirmViewModel = confirmViewModel;
    self.isAdded = ko.observable(data != null ? true : false);
    self.creditCardAmount = ko.observable((data != null ? parseFloat(data.Amount) : 0));
    self.customerNameOnPayment = ko.observable("");
    self.description = ko.observable("");
    self.creditCardNumberMasked = ko.observable("");
    self.expirationMonth = ko.observable("");
    self.expirationYear = ko.observable("");
}

function ConfirmGiftCardData(data, confirmViewModel) {
    var self = this;

    self.confirmViewModel = confirmViewModel;
    self.isAdded = ko.observable(data != null ? true : false);
    self.giftCardAmount = ko.observable((data != null ? parseFloat(data.Amount) : 0));
    self.giftCardNumber = ko.observable(data != null ? data.GiftCardNumber : "");
}


// -----[CONFIRM DATA MODEL]-----
function ConfirmDataViewModel() {
    var self = this;

    self.cart = ko.observable("");
    self.benefitsData = ko.observable("");
    self.isShipAll = ko.observable(false);
    self.creditCardPayment = ko.validatedObservable(new ConfirmCreditCardData(null));
    self.giftCardPayment = ko.validatedObservable(new GiftCardPaymentViewModel(null));
    self.shippingAddress = ko.observable();
    self.shippingMethod = ko.observable();
    self.billingAddress = ko.observable();
    self.currencyCode = ko.observable();

    self.goToNextPageClick = function () {
        MessageContext.ClearAllMessages();

        $("#PlaceOrderButton").button('loading');

        var data = '{"ConfirmItemPath": "' + $("#ConfirmItemPath").val() + '"}';
        AjaxService.Post("/api/cxa/checkout/SubmitOrder", JSON.parse(data), function (data, success, sender) {
            if (data.Success && success) {
                window.location.href = data.NextPageLink;
            }

            $("#PlaceOrderButton").button('reset');
        }, $(this));
    }

    self.paymentTotal = ko.computed({
        read: function () {
            var ccIsAdded = self.creditCardPayment().isAdded();
            var gcIsAdded = self.giftCardPayment().isAdded();
            if (!ccIsAdded && !gcIsAdded) {
                return 0;
            }

            var ccAmount = ccIsAdded ? self.creditCardPayment().creditCardAmount() : 0;
            var gcAmount = gcIsAdded ? self.giftCardPayment().giftCardAmount() : 0;
            return (parseFloat(ccAmount) + parseFloat(gcAmount)).toFixed(2);
        },
        write: function () { }
    });

    self.enableToConfirm = ko.computed({
        read: function () {

            if (self.cart().length == 0) {
                return false;
            }

            var paymentTotalIsValid = parseFloat(self.paymentTotal()) === parseFloat(self.cart().totalAmount());
            if (!paymentTotalIsValid) {
                return false;
            }

            var paymentsAreValid = false;
            var giftCardValid = true;
            if (self.giftCardPayment().isAdded()) {
                giftCardValid = paymentsAreValid = self.giftCardPayment.isValid();
            }

            if (giftCardValid && self.creditCardPayment().isAdded()) {
                paymentsAreValid = true;
            }

            return paymentsAreValid;
        },
        write: function (value) {
            return Boolean(value);
        }
    });

    self.load = function () {
        $('.shoppingcart-delete').find('.btn').hide();
        AjaxService.Post("/api/cxa/orderreview/GetReviewData", null, function (data, success, sender) {
         
            if (success && data.Success) {

                self.cart(new CartViewModel(data.Cart));
                self.benefitsData(data.BenefitsData);
                self.currencyCode(data.CurrencyCode);

                var isShipAll = (data.Cart != null && data.Cart.Shipments != null && data.Cart.Shipments.length == 1) && !data.HasDigitalProduct ? true : false;

                if (self.cart().shipments.length == 1) {
                    var partyId = self.cart().shipments[0].partyId;
                    var party = ko.utils.arrayFirst(self.cart().parties, function (a) {
                        if (a.externalId() === partyId) {
                            return a;
                        }

                        return null;
                    });

                    if (party != null) {
                        self.shippingAddress(party);
                    }
                }

                if (data.Cart != null && isShipAll) {
                    var shipment = data.Cart.Shipments[0];
                    var shippingMethodId = shipment.ShippingMethodId;
                    var shippingMethodName = shipment.ShippingMethodName;

                    self.shippingMethod(new ConfirmShippingMethod(shippingMethodId, shippingMethodName));
                }

                self.isShipAll(isShipAll);

                if (data.Cart != null) {
                    if (data.Cart.Payments != null) {
                        $.each(data.Cart.Payments, function (index, value) {
                            if (value.PaymentType === 4) {
                                self.creditCardPayment(new ConfirmCreditCardData(value, self));
                            }
                            else if (value.PaymentType === 3) {
                                self.giftCardPayment(new ConfirmGiftCardData(value, self));
                            }
                        });
                    }

                    if (data.Cart.AccountingParty != null && data.Cart.AccountingParty.PartyID != null) {
                        var partyId = data.Cart.AccountingParty.PartyID;
                        var party = ko.utils.arrayFirst(self.cart().parties, function (a) {
                            if (a.externalId() === partyId) {
                                return a;
                            }

                            return null;
                        });

                        if (party != null) {
                            self.billingAddress(party);
                        }
                    }
                }
            }
        }
        );
    };
}