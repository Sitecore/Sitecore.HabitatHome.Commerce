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

function WishListLinesViewModel(data) {
    var self = this;
    self.wishList = ko.observable(new WishListViewModel(data));
    self.updateModel = function (data) {
        self.wishList(new WishListViewModel(data));

    };
    self.removeItem = function (item, event) {
        $(event.currentTarget).find(".glyphicon").removeClass("glyphicon-remove-circle");
        $(event.currentTarget).find(".glyphicon").addClass("glyphicon-refresh");
        $(event.currentTarget).find(".glyphicon").addClass("glyphicon-refresh-animate");
        var lineItemId = item.externalCartLineId;
        var sender = event.currentTarget;
        AjaxService.Post("/api/cxa/wishilistlines/RemoveWishListLines", { lineIds: lineItemId }, function (data, success, sender) {
            if (success && data.Success) {
                CartContext.TriggerCartUpdateEvent();
            }
        });
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
}



function WishListViewModel(data, userAddresses) {
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
