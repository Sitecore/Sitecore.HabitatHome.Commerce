//-----------------------------------------------------------------------
// Copyright 2016 Sitecore Corporation A/S
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file 
// except in compliance with the License. You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the 
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
// either express or implied. See the License for the specific language governing permissions 
// and limitations under the License.
// -------------------------------------------------------------------------------------------

function CartLinesViewModel(data) {
    var self = this;
    self.cart = ko.observable(new ShoppingCartViewModel(data));

    self.updateModel = function (data) {
        self.cart(new ShoppingCartViewModel(data));

    };
    self.removeItem = function (item, event) {
        $(event.currentTarget).find(".glyphicon").removeClass("glyphicon-remove-circle");
        $(event.currentTarget).find(".glyphicon").addClass("glyphicon-refresh");
        $(event.currentTarget).find(".glyphicon").addClass("glyphicon-refresh-animate");
        var lineItemId = item.externalCartLineId;
        var sender = event.currentTarget;
        AjaxService.Post("/api/cxa/cart/RemoveShoppingCartLine", { lineNumber: lineItemId }, function (data, success, sender) {
            if (success && data.Success) {
                CartContext.TriggerCartUpdateEvent();
            }
        });
    };
    self.addItemToWishList = function (item, event) {
        $(event.currentTarget).find(".fa").addClass("fa-spinner");
        $(event.currentTarget).find(".fa").addClass("fa-spin");
        var lineItemId = item.externalCartLineId;
        var sender = event.currentTarget;
                                        
        AjaxService.Post("api/cxa/ShoppingCartLines/GetCurrentCartLines", {}, function (data, success, sender) {
            if (success) {
                $.each(data.Data, function () {
                    var exId; var prodId; var varId;
                    $(this).map(function (i, b) {
                        if ((b['Key'] == 'ExternalCartLineId'))
                            exId = b['Value'];
                        if ((b['Key'] == 'ProductId'))
                            prodId = b['Value'];
                        if ((b['Key'] == 'VariantId'))
                            varId = b['Value'];
                    })
                    if (lineItemId === exId) {
                        AjaxService.Post("/api/cxa/wishlistlines/AddWishListLine", { productId: prodId, variantId: varId, quantity: item.quantity }, function (data, success, sender) {
                            if (success && data.Success) {
                                self.removeItem(item, event);
                            }
                        });
                    }
                });
            }
        })
    }
};

self.increaseQuantity = function (item) {
    item.quantity = Number(item.quantity) + 1;
    self.updateQuantity(item);
}
self.decreaseQuantity = function (item) {
    item.quantity = Number(item.quantity) - 1;
    self.updateQuantity(item);
}

self.quntityUpdating = ko.observable(false || (CXAApplication.RunningMode === RunningModes.ExperienceEditor));


self.updateQuantity = function (item) {
    if (CXAApplication.RunningMode === RunningModes.Normal) {
        self.quntityUpdating(true);
        var model = self;
        AjaxService.Post("/api/cxa/cart/UpdateCartLineQuantity", { quantity: item.quantity, lineNumber: item.externalCartLineId }, function (data, success, self) {
            if (success && data.Success) {
                CartContext.TriggerCartUpdateEvent();
                model.quntityUpdating(false);
            }

        });
    }

};                       

function ShoppingCartLineItemData(cartData, line, cart) {
    var self = this;

    self.cart = cart;
    // lines //
    self.image = line.Image;
    self.displayName = line.DisplayName;

    // TODO: Schema specific
    self.colorInformation = line.ColorInformation;
    self.sizeInformation = line.SizeInformation;
    self.styleInformation = line.StyleInformation;
    self.giftCardAmountInformation = line.GiftCardAmountInformation;
    self.lineItemDiscount = line.LineDiscount;
    self.quantity = line.Quantity;
    self.linePrice = line.LinePrice;
    self.lineTotal = line.LineTotal;
    self.externalCartLineId = line.ExternalCartLineId;
    self.productUrl = line.ProductUrl;
    self.discountOfferNames = line.DiscountOfferNames;
    self.shouldShowSavings = ko.observable(self.lineItemDiscount !== "$0.00" ? true : false);
    self.shouldShowDiscountOffers = ko.observable(self.discountOfferNames.length > 0 ? true : false);

    // shipping //
    self.selectedShippingOption = ko.observable('0');

    self.isLineShipAll = ko.observable(cartData.Shipments != null && cartData.Shipments.length > 1);
    self.isLineShipToEmail = ko.observable(false);
    self.showShipOptionContent = ko.observable(false);
    self.selectedShippingOptionName = ko.observable($('#SelectDeliveryFirstMessage').val());

    self.relatedKitProducts = ko.observable("");
    self.relatedBundleProducts = ko.observable("");

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
                                    self.shippingMethods.push(new method(v.Description, v.ExternalId));
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

function ShoppingCartViewModel(data, userAddresses) {
    var self = this;

    self.cartLines = ko.observableArray();
    self.userAddresses = ko.observableArray();
    if (userAddresses != null) {
        self.userAddresses(userAddresses);
    }

    self.getUniqueAddressId = function () {

        var newId = 10;
        for (i = 10; i < 1000; i++) {
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
            var lineItem = new ShoppingCartLineItemData(data, this, self);

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
            if (this.IsKit == true) {
                var relatedProductList = [];
                if (this.RelatedKitProducts.length > 0) {
                    $.each(this.RelatedKitProducts, function (index, value) {
                        var newProduct = new RelatedPackageProduct(value);
                        relatedProductList.push(newProduct);
                    });
                    lineItem.relatedKitProducts(relatedProductList);
                }
            }
            if (this.IsBundle == true) {
                var relatedProductList = [];
                if (this.RelatedBundleProducts.length > 0) {
                    $.each(this.RelatedBundleProducts, function (index, value) {
                        var newProduct = new RelatedPackageProduct(value);
                        relatedProductList.push(newProduct);
                    });
                    lineItem.relatedBundleProducts(relatedProductList);
                }
            }

            self.cartLines.push(lineItem);
        });
    }
}

function RelatedPackageProduct(data) {
    self = this;
    $(data).map(function (i, b) {
        if ((b['Key'] == 'ProductId'))
            self.ProductId = b['Value'];
        if ((b['Key'] == 'DisplayName'))
            self.DisplayName = b['Value'];
        if ((b['Key'] == 'ProductPrice'))
            self.ProductPrice = b['Value'];
    })
}