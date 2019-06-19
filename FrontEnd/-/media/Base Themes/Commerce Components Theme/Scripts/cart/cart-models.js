function LineItemShippingMethod(id, description) {
    var self = this;

    self.id = id;
    self.description = description;
}

function ShipmentData(data) {
    self = this;

    self.partyId = data.PartyID;
    self.lineIds = data.LineIDs;
    self.shippingMethodId = data.ShippingMethodId;
    self.shippingMethodName = data.ShippingMethodName;
}

function LineItemData(cartData, line, cart) {
    var self = this;

    self.cart = cart;
    // lines //
    self.image = line.Image;
    self.displayName = line.DisplayName;
    self.sublines = ko.observableArray();
    if (line != null) {
        $(line.SubLines).each(function () {
            var lineItem = new LineItemData(cartData, this, null);
            self.sublines.push(lineItem);
        });
    }

    // TODO: Schema specific
    self.colorInformation = line.ColorInformation;
    self.sizeInformation = line.SizeInformation;
    self.styleInformation = line.StyleInformation;
    self.giftCardAmountInformation = line.GiftCardAmountInformation;
    self.properties = [];
    var props = line.Properties || {};
    Object.keys(props).forEach(function(key, index) {
        if (props[key]) {
            self.properties.push({
                label: key,
                value: props[key]
            });
        }
    });

    self.lineItemDiscount = line.LineDiscount;
    self.quantity = line.Quantity;
    self.linePrice = line.LinePrice;
    self.lineTotal = line.LineTotal;
    self.externalCartLineId = line.ExternalCartLineId;
    self.productUrl = line.ProductUrl;
    self.discountOfferNames = line.DiscountOfferNames;
    self.shippingMethodName = line.ShippingMethodName;
    self.address = line.Address;
    self.shouldShowSavings = ko.observable(self.lineItemDiscount !== "$0.00" ? true : false);
    self.shouldShowDiscountOffers = ko.observable(self.discountOfferNames.length > 0 ? true : false);

    // shipping //
    self.selectedShippingOption = ko.observable('0');

    self.isLineShipAll = ko.observable(cartData.Shipments != null && cartData.Shipments.length > 1);
    self.isLineShipToEmail = ko.observable(false);
    self.showShipOptionContent = ko.observable(false);
    self.selectedShippingOptionName = ko.observable($('#SelectDeliveryFirstMessage').val());
    self.toggleShipContent = function () {
        self.showShipOptionContent(!self.showShipOptionContent());
    };

    self.selectedShippingOption.subscribe(function (option) {
        self.isLineShipAll(option === 1);
        self.isLineShipToEmail(option === 3);
        self.showShipOptionContent(option !== 0);

        var match = ko.utils.arrayFirst(self.shippingOptions(), function (o) {
            return o.ShippingOptionType.Value === option;
        });

        if (match != null) {
            self.selectedShippingOptionName(match.Name);
        }
    }.bind(this));

    self.shippingMethods = ko.observableArray();
    self.shippingMethod = ko.validatedObservable().extend({ required: true });
    self.selectShippingMethod = function (shippingMethod) {
        if (shippingMethodsArray.indexOf(self.externalCartLineId) === -1) {
            shippingMethodsArray.push(self.externalCartLineId);
        }
    };

    self.shippingAddress = ko.validatedObservable(new AddressViewModel({ "ExternalId": self.externalCartLineId }));
    self.selectedShippingAddress = ko.observable("UseOther");
    self.selectedShippingAddress.subscribe(function (id) {
        var match = ko.utils.arrayFirst(self.cart.userAddresses(), function (a) {
            if (a.externalId() === id && id !== "UseOther") {
                return a;
            }

            return null;
        });

        self.shippingMethod("");
        self.shippingMethods.removeAll();
        if (match != null) {
            var newAddress = match.clone();
            var newId = self.cart.getUniqueAddressId().toString();
            newAddress.externalId(newId);
            newAddress.partyId(newId);
            self.shippingAddress(newAddress);
        } else {
            self.shippingAddress(new AddressViewModel({ "ExternalId": self.externalCartLineId }));
        }
    }.bind(this));
    self.shippingAddressFieldChanged = function () {
        var index = shippingMethodsArray.indexOf(self.externalCartLineId);
        if (index !== -1) {
            shippingMethodsArray.splice(index, 1);
        }
        self.shippingMethod("");
        self.shippingMethods.removeAll();
    };
    
    self.shippingEmail = ko.validatedObservable("").extend({ required: true, email: true });
    self.shippingEmail.subscribe(function (email) {
        var index = shippingMethodsArray.indexOf(self.externalCartLineId);
        if (email.trim().length > 0 && index === -1) {
            shippingMethodsArray.push(self.externalCartLineId);
        }
        else if (email.trim().length === 0 && index !== -1) {
            shippingMethodsArray.splice(index, 1);
        }

    }.bind(this));
    self.shippingEmailContent = ko.observable("");

    self.shippingOptions = ko.observableArray();
    if (line.ShippingOptions !== null) {
        $(line.ShippingOptions).each(function () {
            self.shippingOptions.push(this);
        });

        // Handle edit mode.
        if (line.ShippingOptions != null && line.ShippingOptions.length > 0) {
            var shippingOption = line.ShippingOptions[0];
            if (shippingOption != null) {
                if (shippingOption.ShippingOptionType != null) {
                    self.selectedShippingOption(shippingOption.ShippingOptionType.Value);
                }
            }
        }
    }

    self.handleEditMode = function (data) {
        if (data.Cart.Shipments != null && data.Cart.Shipments.length > 0) {
            selectedShippingOption = self.selectedShippingOption();

            $(data.Cart.Shipments).each(function () {
                var shipment = this;
                var foundShipment = null;
                if (shipment.LineIDs != null) {

                    foundLineId = ko.utils.arrayFirst(shipment.LineIDs, function (a) {
                        if (a === self.externalCartLineId) {
                            return a;
                        }

                        return null;
                    });

                    if (foundLineId != null) {

                        // ShipToAddress
                        if (selectedShippingOption == "1") {
                            if (shipment.ShipmentEditModel != null) {
                                var methods = "";
                                self.shippingMethods.removeAll();
                                $.each(shipment.ShipmentEditModel.ShippingMethods, function (i, v) {
                                    self.shippingMethods.push(new method(v.Description, v.ExternalId, v.Name));
                                });

                                var sm = ko.utils.arrayFirst(self.shippingMethods(), function (a) {
                                    if (a.description === shipment.ShippingMethodName) {
                                        return a;
                                    }

                                    return null;
                                });

                                if (sm != null) {
                                    self.shippingMethod(sm);
                                }
                            }
                        }

                        // Email
                        if (selectedShippingOption == '3') {
                            self.shippingEmail(shipment.Email);
                            self.shippingEmailContent(shipment.EmailContent);
                        }
                    }
                }
            });

            if (selectedShippingOption == "1") {
                self.isLineShipAll(true);
                self.isLineShipToEmail(false);
            }

            if (selectedShippingOption == '3') {
                self.isLineShipToEmail(true);
                self.isLineShipAll(false);
            }

            self.showShipOptionContent(true);
        }
    }
}

function CartViewModel(data, userAddresses) {
    var self = this;

    self.cartLines = ko.observableArray();
    self.userAddresses = ko.observableArray();
    if (userAddresses != null) {
        self.userAddresses(userAddresses);
    }

    self.getUniqueAddressId = function () {

        var newId = 10;
        for (i = 10 ; i < 1000 ; i++) {
            var match = ko.utils.arrayFirst(self.cartLines(), function (a) {
                if (a.shippingAddress() != null && a.shippingAddress().externalId() === i.toString()) {
                    return a;
                }

                return null;
            });

            if (match == null) {
                newId = i;
                break;
            }
        }

        return newId;
    }

    //existing promo codes
    self.promoCodes = ko.observableArray();
    if (data != null) {
        $(data.PromoCodes).each(function () {
            self.promoCodes.push(this);
        });
    }

    //promo code input
    self.promoCode = ko.observable();
    self.hasPromoCode = ko.computed(function () {
        return self.promoCode();
    });

    self.subTotal = ko.observable(data != null ? data.Subtotal : 0);
    self.taxTotal = ko.observable(data != null ? data.TaxTotal : 0);
    self.total = ko.observable(data != null ? data.Total : 0);
    self.totalAmount = ko.observable(data != null ? data.TotalAmount : 0);
    self.discount = ko.observable(data != null ? data.Discount : 0);
    self.shippingTotal = ko.observable(data != null ? data.ShippingTotal : 0);
    self.isLineShipAll = data != null && data.Shipments != null && data.Shipments.length > 1;

    self.shipments = [];
    if (data != null) {
        $(data.Shipments).each(function () {
            self.shipments.push(new ShipmentData(this));
        });
    }

    self.parties = [];
    if (data != null) {
        $(data.Parties).each(function () {
            self.parties.push(new AddressViewModel(this));
        });
    }

    if (data != null) {
        $(data.Lines).each(function () {
            var lineItem = new LineItemData(data, this, self);

            if (self.isLineShipAll) {
                var shipment = ko.utils.arrayFirst(self.shipments, function (a) {
                    if (a.lineIds != null && a.lineIds.length > 0) {
                        var found = ko.utils.arrayFirst(a.lineIds, function (l) {
                            if (l === lineItem.externalCartLineId) {
                                return l;
                            }
                        })

                        if (found != null) {
                            return this;
                        }

                        return null;
                    }

                    return null;
                });

                if (shipment != null) {
                    lineItem.shippingMethod(new LineItemShippingMethod(shipment.shippingMethodId, shipment.shippingMethodName))

                    var party = ko.utils.arrayFirst(self.parties, function (a) {
                        if (a.externalId() === shipment.partyId) {
                            return a;
                        }

                        return null;
                    });

                    if (party != null) {
                        lineItem.shippingAddress(party);
                    }
                }
            }

            self.cartLines.push(lineItem);
        });
    }
}
