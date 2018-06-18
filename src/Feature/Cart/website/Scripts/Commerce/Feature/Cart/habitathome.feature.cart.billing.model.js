﻿var mustEqual = function (val, other) {

    if (val) {
        return val === other;
    }

    return true;
};

// -----[GIFT CARD PAYMENT MODEL]-----
function GiftCardPaymentViewModel(billingViewModel, card) {
    var self = this;
    var populate = card != null;

    self.billingViewModel = billingViewModel;

    self.isAdded = ko.observable(false);
    self.balance = ko.observable(0);
    self.formattedBalance = ko.observable("");
    self.enableAddCard = ko.observable(false);

    self.giftCardNumber = populate ? ko.validatedObservable(card.PaymentMethodID).extend({ required: true }) : ko.validatedObservable("").extend({ required: true });
    self.giftCardNumber.subscribe(function (cn) {
        self.balance(0);
        self.formattedBalance("");

        self.reEnableAddCard();
        //self.enableAddCard(cn.length > 0 && self.giftCardAmount().length > 0 && self.isAdded() === false);
    }.bind(this));

    self.reload = function (data) {
        self.giftCardNumber(data.ExternalId);
        self.formattedBalance(data.FormattedBalance);
        self.balance(data.Balance);
        //self.enableAddCard(false);
        self.reEnableAddCard();
    }

    self.reEnableAddCard = function (ca) {
        if (!ca) {
            ca = self.giftCardAmount();
        }

        self.enableAddCard(ca.length > 0 && self.giftCardNumber().length > 0 && self.isAdded() === false && self.giftCardAmount.isValid() && self.balance() > 0);
    }

    self.getBalance = function () {
        MessageContext.ClearAllMessages();
        var $btn = $('.git-card-payment-get-balance-btn').button('loading');
        var data = {};
        data.GiftCardId = self.giftCardNumber();
        AjaxService.Post('/api/cxa/checkout/getgiftcardbalance', data, function (data, success, sender) {
            if (success && data.Success) {
                self.reload(data);
            }

            $('.git-card-payment-get-balance-btn').button('reset');
        }, this);
    };

    self.applyBalance = function () {
        MessageContext.ClearAllMessages();
        var balance = self.balance();
        if (balance <= 0) {
            return;
        }

        var orderTotal = self.billingViewModel.cart().totalAmount();
        if (balance > orderTotal) {
            self.giftCardAmountRawValue(orderTotal);
        }
        else if (balance <= orderTotal) {
            self.giftCardAmountRawValue(balance);
        }

        self.enableAddCard(true);
    };

    self.addCard = function () {
        MessageContext.ClearAllMessages();
        self.isAdded(true);
        self.enableAddCard(false);
        self.revalueAmounts();
    };

    self.removeCard = function () {
        MessageContext.ClearAllMessages();
        self.isAdded(false);
        self.giftCardNumber("");
        self.formattedBalance("");
        self.giftCardAmountRawValue(0.00);
        self.balance(0.00);
    };

    self.revalueAmounts = function () {
        if (!self.isAdded()) {
            return;
        }

        var ccIsAdded = self.billingViewModel.creditCardPayment().isAdded();
        if (!ccIsAdded) {
            return;
        }

        var ccAmount = parseFloat(self.billingViewModel.creditCardPayment().creditCardAmount());
        var total = parseFloat(self.billingViewModel.cart().totalAmount());
        var aTotal = parseFloat(parseFloat(self.giftCardAmountRawValue()) + ccAmount);

        if (aTotal > total) {
            var count = 1;
            var diff = (aTotal - total) / count;
            ccAmount = ccIsAdded ? ccAmount - diff : 0;
            self.billingViewModel.creditCardPayment().creditCardAmount((ccAmount).toFixed(2));
        }
    };

    self.giftCardAmount = ko.validatedObservable(0.00).extend({ required: true, number: true });

    self.giftCardAmountRawValue = ko.computed({
        read: function () {
            value = self.giftCardAmount();

            if (self.billingViewModel) {
                value = value.toString();
                value = value.replace(self.billingViewModel.currencyDecimalSeparator(), ".");
            }

            return value;
        },
        write: function (value) {
            if (self.billingViewModel) {
                value = value.toString();
                value = value.replace(".", self.billingViewModel.currencyDecimalSeparator());
            }

            self.giftCardAmount(value);
        }
    });

    self.giftCardAmount.subscribe(function (ca) {
        self.reEnableAddCard(ca);
        self.revalueAmounts();
    }.bind(this));

    self.addGiftCard = function (cardData) {
        self.giftCardNumber(cardData.GiftCardNumber != null ? cardData.GiftCardNumber : "");
        self.giftCardAmountRawValue(cardData.Amount);
        self.isAdded(true);
    }
}

// -----[CREDITCARD PAYMENT MODEL]-----
function CreditCardPaymentViewModel(card, checkoutViewModel) {
    var self = this;
    var populate = card != null;

    self.checkoutViewModel = checkoutViewModel;

    self.isAdded = ko.observable(false);
    self.creditCardNumber = ko.validatedObservable().extend({ required: true, number: true });
    self.creditCardNumberMasked = ko.observable();
    self.creditCardNumber.subscribe(function (number) {
        self.creditCardNumberMasked("XXXX XXXX XXXX " + number.substr(number.length - 4));
    });

    self.description = ko.observable();

    self.existingPaymentAmount = ko.observable(populate ? card.Amount : 0);

    self.paymentMethodID = populate ? ko.validatedObservable(card.PaymentMethodID).extend({ required: true }) : ko.validatedObservable().extend({ required: true });
    self.paymentMethodID.subscribe(function (methodId) {
        if (methodId !== "0") {
            self.isAdded(true);

            var paymentMethod = ko.utils.arrayFirst(self.checkoutViewModel.paymentMethods(), function (a) {
                if (a.ExternalId === methodId) {
                    return a;
                }

                return null;
            });

            self.description(paymentMethod ? paymentMethod.Description : "");
            self.checkoutViewModel.creditCardEnable(true);
            self.checkoutViewModel.billingAddressEnable(true);
        } else {
            self.isAdded(false);
            self.checkoutViewModel.creditCardEnable(false);
            self.checkoutViewModel.billingAddressEnable(false);
        }
    });

    self.changePayment = ko.observable(false);
    self.displayExistingPaymentMessage = ko.computed({
        read: function () {
            if (self.checkoutViewModel.editMode() && self.existingPaymentAmount() == self.creditCardAmount() && !self.changePayment()) {
                return true;
            }
            else {
                return false;
            }
        },
        write: function () { }
    });

    self.mustRevalidatePaymentMessage = ko.observable(populate ? card.CartAmountDifferentThanPaymentMessage : "");
    self.mustRevalidatePayment = ko.computed({
        read: function () {
            if (self.checkoutViewModel.editMode() && self.existingPaymentAmount() != self.creditCardAmount()) {
                return true;
            }
            else {
                return false;
            }
        },
        write: function () { }
    });

    self.creditCardAmount = ko.computed({
        read: function () {
            if (!self.isAdded()) {
                return 0;
            }

            var total = self.checkoutViewModel.cart() ? self.checkoutViewModel.cart().totalAmount() : 0;
            var gcAmount = self.checkoutViewModel.giftCardPayment() ? self.checkoutViewModel.giftCardPayment().giftCardAmountRawValue() : 0;
            var ccAmount = (parseFloat(total) - parseFloat(gcAmount)).toFixed(2);

            if (ccAmount <= 0) {
                self.isAdded(false);
            }

            return ccAmount;
        },
        write: function () { }
    });

    self.originalExistingPaymentMessage = ko.observable(populate ? card.ExistingPaymentMessage : "");
    self.existingPaymentMessage = ko.computed({
        read: function () {
            if (self.creditCardAmount() != null) {
                return self.originalExistingPaymentMessage().format(self.creditCardAmount());
            }
            else {
                return "";
            }
        },
        write: function () { }
    });

    self.partyID = populate ? ko.observable(card.PartyID) : ko.observable();
}

// -----[BILLING DATA MODEL]-----
function BillingDataViewModel() {
    var self = this;

    var PaymentTypes = {
        GiftCard: 3,
        FederatedPayment: 4,
    };

    self.cart = ko.observable("");

    self.editMode = ko.observable(false);

    var Country = function (name, code) {
        this.country = name;
        this.code = code;
    };
    self.countries = ko.observableArray();

    self.isAuthenticated = false;
    self.userEmail = "";

    self.isShipAll = ko.observable(false);

    self.currencyDecimalSeparator = ko.observable();
    self.currencyGroupSeparator = ko.observable();

    self.userAddresses = ko.observableArray();
    self.hasDigitalProductsInCart = false;
    self.userAddresses.push(new AddressViewModel({ "ExternalId": "UseOther", "FullAddress": $("#OtherAddressLabel").val() }));

    self.billingEmail = ko.validatedObservable(self.userEmail).extend({ required: true, email: true });
    self.billingConfirmEmail = ko.validatedObservable(self.userEmail).extend({ required: true, validation: { validator: mustEqual, message: $("#EmailsMustMatchMessage").val(), params: self.billingEmail } });

    self.payFederatedPayment = ko.observable(false);
    self.payGiftCard = ko.observable(false);
    self.cardPaymentAcceptPageUrl = ko.observable('');
    self.cardPaymentResultAccessCode = "";
    self.cardPaymentAcceptCardPrefix = "";
    self.CARDPAYMENTACCEPTPAGEHEIGHT = "msax-cc-height";
    self.CARDPAYMENTACCEPTPAGEERROR = "msax-cc-error";
    self.CARDPAYMENTACCEPTPAGERESULT = "msax-cc-result";
    self.CARDPAYMENTACCEPTPAGESUBMIT = "msax-cc-submit";
    self.CARDPAYMENTACCEPTCARDPREFIX = "msax-cc-cardprefix";

    var PaymentMethod = function (externalId, description) {
        this.ExternalId = externalId;
        this.Description = description;
    };

    self.paymentMethods = ko.observableArray();
    self.paymentClientToken = ko.observable("");

    self.giftCardPayment = ko.validatedObservable(new GiftCardPaymentViewModel(self));
    self.creditCardPayment = ko.validatedObservable(new CreditCardPaymentViewModel(null, self));
    self.creditCardEnable = ko.observable(false);
    self.billingAddress = ko.validatedObservable(new AddressViewModel({ "ExternalId": "1" }));
    self.billingAddressEnable = ko.observable(false);
    self.shippingAddress = ko.observable();
    self.selectedBillingAddress = ko.observable("UseOther");
    self.selectedBillingAddress.subscribe(function (id) {
        if (id === "UseShipping") {
            self.billingAddressEnable(false);
            self.billingAddress(self.shippingAddress());
        } else {
            var match = self.getAddress(id);
            if (match != null) {
                self.billingAddressEnable(false);
                self.billingAddress(match);
            } else {
                self.billingAddressEnable(true);
                self.billingAddress(new AddressViewModel({ "ExternalId": "1" }));
            }
        }

        $(".select-billing-address").prop("disabled", false);
    });

    self.paymentTotal = ko.computed({
        read: function () {
            var ccIsAdded = self.creditCardPayment().isAdded();
            var gcIsAdded = self.giftCardPayment().isAdded();
            if (!ccIsAdded && !gcIsAdded) {
                return 0;
            }

            var ccAmount = ccIsAdded ? self.creditCardPayment().creditCardAmount() : 0;
            var gcAmount = gcIsAdded ? self.giftCardPayment().giftCardAmountRawValue() : 0;
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
            if (self.giftCardPayment().isAdded()) {
                paymentsAreValid = self.giftCardPayment.isValid();
            }

            if (self.creditCardPayment().isAdded()) {
                if (!self.payFederatedPayment()) {
                    paymentsAreValid = self.creditCardPayment.isValid() && self.billingAddress.isValid();
                }
                else {
                    paymentsAreValid = !self.creditCardPayment().mustRevalidatePayment() && self.billingAddress.isValid();
                }
            }

            if (paymentsAreValid) {
                ko.validation.group(self).showAllMessages(true);
            }

            return paymentsAreValid && self.billingEmail.isValid() && self.billingConfirmEmail.isValid();
        },
        write: function (value) {
            return Boolean(value);
        }
    });

    self.setPaymentMethods = function () {
        var data = "{";

        data = data + '"BillingItemPath":"' + $("#BillingItemPath").val() + '",'
        data = data + '"UserEmail":' + '"' + self.billingEmail() + '"';

        if (self.creditCardPayment().isAdded()) {
            var cc = self.creditCardPayment();
            if (self.payFederatedPayment) {
                var creditCard = {
                    "CardToken": self.cardPaymentResultAccessCode,
                    "Amount": cc.creditCardAmount(),
                    "CardPaymentAcceptCardPrefix": self.cardPaymentAcceptCardPrefix
                };

                if (data.length > 1) {
                    data += ",";
                }

                data += '"FederatedPayment":' + JSON.stringify(creditCard);
                if (self.cardPaymentAcceptCardPrefix === "paypal") {
                    var ba = self.billingAddress();
                    var billingAddress =
                        {
                            "Name": ba.name(),
                            "Address1": ba.address1(),
                            "Country": ba.country(),
                            "City": ba.city(),
                            "State": ba.state(),
                            "ZipPostalCode": ba.zipPostalCode(),
                            "ExternalId": ba.externalId(),
                            "PartyId": ba.externalId()
                        };

                    if (data.length > 1) {
                        data += ",";
                    }

                    data += '"BillingAddress":' + JSON.stringify(billingAddress);
                }
            }
        }

        if (self.giftCardPayment().isAdded()) {
            var giftCard = {
                "PaymentMethodID": self.giftCardPayment().giftCardNumber(),
                "Amount": self.giftCardPayment().giftCardAmountRawValue()
            };

            if (data.length > 1) {
                data += ",";
            }

            data += '"GiftCardPayment":' + JSON.stringify(giftCard);
        }

        data += "}";

        $(".to-confirm-button").button('loading');
        MessageContext.ClearAllMessages();

        AjaxService.Post("/api/cxa/checkout/SetPaymentMethods", JSON.parse(data), function (data, success, sender) {
            if (data.Success && success) {
                window.location.href = data.NextPageLink
            }
            $(".to-confirm-button").button('reset');
        }, $(this));
    }

    self.setupPage = function () {
        $('.existing-payment-reset-link').on('click', function (event) {
            self.creditCardPayment().changePayment(true);
        });
    }

    self.goToNextPageClick = function () {
        self.setPaymentMethods();
    }

    self.getAddress = function (id) {
        var match = ko.utils.arrayFirst(self.userAddresses(), function (a) {
            if (a.externalId() === id && id !== "UseOther") {
                return a;
            }

            return null;
        });

        return match;
    };

    self.load = function () {
        AjaxService.Post("/api/cxa/checkout/GetBillingData", null, function (data, success, sender) {
            if (success && data.Success) {
                console.log(data);
                if (data.Countries != null) {
                    $.each(data.Countries, function (index, value) {
                        self.countries.push(new Country(value, index));
                    });
                }

                self.cart(new CartViewModel(data.Cart));

                // Set the shipping address. Needed for same as shipping.
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

                self.paymentClientToken(data.PaymentClientToken);

                if (data.IsUserAuthenticated === true && data.UserAddresses.Addresses != null) {
                    $.each(data.UserAddresses.Addresses, function () {
                        self.userAddresses.push(new AddressViewModel(this));
                    });

                    self.isAuthenticated = true;
                }

                // Checking if the cart contains a digital product "HasDigitalProduct"
                // if true then the "Same as shipping address" will not be available 
                // else it will be added to nilling address list
                self.hasDigitalProductsInCart = data.HasDigitalProduct;
                if (!self.hasDigitalProductsInCart) {
                    self.userAddresses.push(new AddressViewModel({ "ExternalId": "UseShipping", "FullAddress": $(".select-billing-address").attr("title") }));
                }

                self.userEmail = data.UserEmail;
                self.billingEmail(data.UserEmail);
                self.billingConfirmEmail(data.UserEmail);

                self.currencyDecimalSeparator(data.CurrencyDecimalSeparator);
                self.currencyGroupSeparator(data.CurrencyGroupSeparator)

                //if (self.currencyDecimalSeparator() != ".") {
                //}

                currentNumberValidator = ko.validation.rules.number.validator;
                ko.validation.rules.number.validator = function (n, t) {
                    n = n.replace(self.currencyDecimalSeparator(), ".");

                    return currentNumberValidator(n, t) && $.isNumeric(n);
                }

                if (data.PaymentOptions != null) {
                    $.each(data.PaymentOptions, function (index, value) {
                        if (value.PaymentOptionTypeName === "PayFederatedPayment") {
                            self.payFederatedPayment(true);
                        }
                        if (value.PaymentOptionTypeName === "PayGiftCard") {
                            self.payGiftCard(true);
                        }
                    });
                }

                if (data.PaymentMethods != null) {
                    self.paymentMethods.push(new PaymentMethod("0", $("#PaymentMethods").attr("title")));
                    $.each(data.PaymentMethods, function (index, value) {
                        self.paymentMethods.push(new PaymentMethod(value.ExternalId, value.Description));
                    });
                }

                self.isShipAll((data.Cart != null && data.Cart.Shipments != null && data.Cart.Shipments.length == 1 && data.Cart.Shipments[0].ShippingMethodName != "Pickup From Store") ? true : false);

                if (self.paymentClientToken() != null) {
                    var clientToken = self.paymentClientToken();
                    if (clientToken.length > 0) {
                        braintree.setup(clientToken, 'dropin', {
                            container: 'dropin-container',
                            paymentMethodNonceReceived: function (event, nonce) {
                                if (nonce.length > 0) {
                                    self.cardPaymentResultAccessCode = nonce;
                                    self.cardPaymentAcceptCardPrefix = "paypal";

                                    if (self.creditCardPayment() != null && self.editMode()) {
                                        self.creditCardPayment().existingPaymentAmount(self.creditCardPayment().creditCardAmount());
                                        self.creditCardPayment().changePayment(false);
                                    }
                                }
                            }
                        });
                    }
                }

                if (data.Cart != null && data.Cart.AccountingParty != null) {
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

                if (data.Cart != null && data.Cart.Payments != null && data.Cart.Payments.length > 0) {
                    var giftCardAdded = false;
                    var fedPaymentAdded = false;
                    $.each(data.Cart.Payments, function (index, Value) {
                        if (Value.PaymentType == PaymentTypes.FederatedPayment) {
                            self.creditCardPayment(new CreditCardPaymentViewModel(Value, self));
                            self.payFederatedPayment(true);
                            self.cardPaymentResultAccessCode = Value.CardToken;
                            self.cardPaymentAcceptCardPrefix = "paypal";
                            fedPaymentAdded = true;
                        }

                        if (Value.PaymentType == PaymentTypes.GiftCard) {
                            self.giftCardPayment().addGiftCard(Value);
                            self.payGiftCard(true);
                            giftCardAdded = true;
                        }
                    });

                    self.editMode(fedPaymentAdded || giftCardAdded);

                    if (fedPaymentAdded) {
                        $(".ccpayment").trigger("click");
                    }

                    if (giftCardAdded) {
                        $(".giftCardPayment").trigger("click");
                    }
                }

                self.setupPage();
            }
        }
        )
    };
}