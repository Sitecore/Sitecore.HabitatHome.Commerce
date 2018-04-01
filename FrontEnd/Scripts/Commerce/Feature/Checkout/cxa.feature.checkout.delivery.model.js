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


// TODO: Get rid of globals!
var shippingMethodsArray = [];
var methodsViewModel = null;
var method = null;

function DeliveryDataViewModel() {
    var self = this;

    self.cart = ko.observable("");

    self.currencyCode = ko.observable("");

    var Country = function (name, code) {
        this.country = name;
        this.code = code;
    };
    self.countries = ko.observableArray();
    self.states = ko.observableArray([]);

    self.isShipAll = ko.observable(false);
    self.isShipToEmail = ko.observable(false);
    self.isShipItems = ko.observable(false);
    self.emailDeliveryMethod = ko.observable("");
    self.orderShippingOptions = ko.observableArray();

    self.selectedShippingOption = ko.observable('0');
    self.selectedShippingOption.subscribe(function (option) {
        self.isShipAll(option === 1);
        self.isShipToEmail(option === 3);
        self.isShipItems(option === 4);
    }.bind(this));

    self.isAuthenticated = ko.observable(false);
    self.userEmail = "";

    self.userAddresses = ko.observableArray();
    self.userAddresses.push(new AddressViewModel({ "ExternalId": "UseShipping", "FullAddress": $("#billingAddressSelect").attr("title") }));
    self.userAddresses.push(new AddressViewModel({ "ExternalId": "UseOther", "FullAddress": $("#ShippingAddressSelect").attr("title2") }));

    self.shippingMethods = ko.observableArray();
    self.shippingMethod = ko.validatedObservable().extend({ required: true });

    self.shippingAddress = ko.validatedObservable(new AddressViewModel({ "ExternalId": "0" }));
    self.shippingAddressFieldChanged = function () {
        self.shippingMethod("");
        self.shippingMethods.removeAll();
    };

    self.selectedShippingAddress = ko.observable("UseOther");
    self.selectedShippingAddress.subscribe(function (id) {
        var match = self.getAddress(id);
        self.shippingMethod("");
        self.shippingMethods.removeAll();
        if (match != null) {
            self.shippingAddress(match);
        } else {
            self.shippingAddress(new AddressViewModel({ "ExternalId": "0" }));
        }
    }.bind(this));

    self.shippingEmail = ko.validatedObservable().extend({ required: true, email: true });
    self.shippingEmailContent = ko.observable("");
    self.setSendToMe = function (item, event) {
        var email = $(event.currentTarget).is(':checked') ? self.userEmail : "";
        item.shippingEmail(email);
    };

    self.enableToBilling = ko.computed({
        read: function () {
            if (self.isShipToEmail()) {
                return self.shippingEmail() && self.shippingEmail.isValid();
            }

            if (self.isShipAll()) {
                return self.shippingMethod.isValid() && self.shippingAddress.isValid()
            }

            if (self.isShipItems()) {
                var isValid = [];
                $.each(self.cart().cartLines(), function () {
                    if (this.isLineShipToEmail()) {
                        isValid.push(this.shippingEmail() && this.shippingEmail.isValid());
                    }
                    else if (this.isLineShipAll()) {
                        isValid.push(this.shippingMethod.isValid() && this.shippingAddress.isValid());
                    }
                    else {
                        isValid.push(false);
                    }
                });

                return isValid.every(isItemValid);
            }
        },
        write: function (value) {
            return Boolean(value);
        }
    });

    self.getAddress = function (id) {
        var match = ko.utils.arrayFirst(self.userAddresses(), function (a) {
            if (a.externalId() === id && id !== "UseOther") {
                return a;
            }

            return null;
        });

        return match;
    };

    self.getShippingMethods = function () {
        if (self.shippingAddress() && self.shippingAddress.errors().length === 0) {
            $("#orderGetShippingMethods").button('loading');
            var party = ko.toJS(self.shippingAddress());
            var data = { ShippingAddress: party, ShippingPreferenceType: self.selectedShippingOption(), Lines: null };
            AjaxService.Post("/api/cxa/checkout/GetShippingMethods", data, function (data, success, sender) {
                if (data.Success && success) {
                    self.setShippingMethods(data.ShippingMethods);
                }

                $("#orderGetShippingMethods").button('reset');
            }, $(this));
        }
        else {
            component.deliveryVM.shippingAddress.errors.showAllMessages();
        }
    };

    self.setShippingMethods = function (shippingMethodDataList) {
        var methods = "";
        self.shippingMethods.removeAll();
        $.each(shippingMethodDataList, function (i, v) {
            self.shippingMethods.push(new method(v.Description, v.ExternalId));
        });
    };

    self.handleEditModeShipAll = function (data) {
        self.isShipAll(true);

        var shipment = data.Cart.Shipments[0];
        if (shipment != null) {

            if (shipment.EditModeShippingOptionType != null) {
                self.selectedShippingOption(shipment.EditModeShippingOptionType.Value);
                var orderShippingPreference = self.selectedShippingOption();

                // ShipToAddress
                if (orderShippingPreference == '1') {
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

                // Email
                if (orderShippingPreference == '3') {
                    self.shippingEmail(shipment.Email);
                    self.shippingEmailContent(shipment.EmailContent);
                }

                if (shipment.ShipmentEditModel != null && shipment.ShipmentEditModel.ShippingMethods != null) {
                    self.setShippingMethods(shipment.ShipmentEditModel.ShippingMethods);

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
        }
    };

    self.handleEditModeShipIndividually = function (data) {
        self.isShipAll(false);
        self.isShipItems(true);
        self.selectedShippingOption('4');
        $.each(self.cart().cartLines(), function (index, value) {
            value.handleEditMode(data);
        });
    };

    self.handleEditMode = function (data) {
        if (self.cart().shipments != null && self.cart().shipments.length > 0) {
            if (self.cart().shipments.length == 1) {
                self.handleEditModeShipAll(data);
            }
            else {
                self.handleEditModeShipIndividually(data);
            }
        }
    };

    self.load = function () {
        AjaxService.Post("/api/cxa/checkout/GetDeliveryData", null, function (data, success, sender) {
            if (success && data.Success) {

                self.emailDeliveryMethod(data.EmailDeliveryMethod);
                self.currencyCode = data.CurrencyCode;

                if (data.Countries != null) {
                    $.each(data.Countries, function (index, value) {
                        self.countries.push(new Country(value, index));
                    });
                }

                if (data.IsUserAuthenticated === true && data.UserAddresses.Addresses != null) {
                    $.each(data.UserAddresses.Addresses, function () {
                        self.userAddresses.push(new AddressViewModel(this));
                    });

                    self.isAuthenticated(true);
                    self.userEmail = data.UserEmail;
                }

                self.cart(new CartViewModel(data.Cart, self.userAddresses()));

                if (data.OrderShippingOptions != null) {
                    $.each(data.OrderShippingOptions, function (index, value) {
                        self.orderShippingOptions.push(value);
                    });
                }

                self.handleEditMode(data);
            }
        }
    )
    };

    self.goToNextPageClick = function () {
        var parties = [];
        var shipping = [];
        var orderShippingPreference = self.selectedShippingOption();
        $("#deliveryMethodSet").val(false);

        $("#ToBillingButton").button('loading');
        //        $("#BackToBillingButton").button('loading');

        if (orderShippingPreference === 1) {
            var partyId = self.shippingAddress().externalId();
            parties.push({
                "Name": self.shippingAddress().name(),
                "Address1": self.shippingAddress().address1(),
                "Country": self.shippingAddress().country(),
                "City": self.shippingAddress().city(),
                "State": self.shippingAddress().state(),
                "ZipPostalCode": self.shippingAddress().zipPostalCode(),
                "ExternalId": partyId,
                "PartyId": partyId
            });

            shipping.push({
                "ShippingMethodID": self.shippingMethod().id,
                "ShippingMethodName": self.shippingMethod().description,
                "ShippingPreferenceType": orderShippingPreference,
                "PartyID": partyId
            });
        }
        else if (orderShippingPreference === 2) {
            var storeId = self.store().externalId();
            parties.push({
                "Name": self.store().name(),
                "Address1": self.store().address().address1(),
                "Country": self.store().address().country(),
                "City": self.store().address().city(),
                "State": self.store().address().state(),
                "ZipPostalCode": self.store().address().zipPostalCode(),
                "ExternalId": storeId,
                "PartyId": storeId
            });

            shipping.push({
                "ShippingMethodID": self.shipToStoreDeliveryMethod().ExternalId,
                "ShippingMethodName": self.shipToStoreDeliveryMethod().Description,
                "ShippingPreferenceType": orderShippingPreference,
                "PartyID": storeId
            });
        }
        else if (orderShippingPreference === 4) {
            $.each(self.cart().cartLines(), function () {
                var lineDeliveryPreference = this.selectedShippingOption();
                var lineId = this.externalCartLineId;

                if (lineDeliveryPreference === 1) {
                    var partyId = this.shippingAddress().externalId();
                    parties.push({
                        "Name": this.shippingAddress().name(),
                        "Address1": this.shippingAddress().address1(),
                        "Country": this.shippingAddress().country(),
                        "City": this.shippingAddress().city(),
                        "State": this.shippingAddress().state(),
                        "ZipPostalCode": this.shippingAddress().zipPostalCode(),
                        "ExternalId": partyId,
                        "PartyId": partyId
                    });

                    shipping.push({
                        "ShippingMethodID": this.shippingMethod().id,
                        "ShippingMethodName": this.shippingMethod().description,
                        "ShippingPreferenceType": lineDeliveryPreference,
                        "PartyID": partyId,
                        "LineIDs": [lineId]
                    });
                }

                if (lineDeliveryPreference === 3) {
                    shipping.push({
                        "ShippingMethodID": self.emailDeliveryMethod().ExternalId,
                        "ShippingMethodName": self.emailDeliveryMethod().Description,
                        "ShippingPreferenceType": lineDeliveryPreference,
                        "ElectronicDeliveryEmail": this.shippingEmail(),
                        "ElectronicDeliveryEmailContent": this.shippingEmailContent(),
                        "LineIDs": [lineId]
                    });
                }
            });
        }
        else if (orderShippingPreference === 3) {
            shipping.push({
                "ShippingMethodID": self.emailDeliveryMethod().ExternalId,
                "ShippingMethodName": self.emailDeliveryMethod().Description,
                "ShippingPreferenceType": orderShippingPreference,
                "ElectronicDeliveryEmail": self.shippingEmail(),
                "ElectronicDeliveryEmailContent": self.shippingEmailContent()
            });
        }

        var data = '{"DeliveryItemPath": "' + $("#DeliveryItemPath").val() + '", "OrderShippingPreferenceType": "' + orderShippingPreference +'", "ShippingMethods":' + JSON.stringify(shipping) + ', "ShippingAddresses":' + JSON.stringify(parties) + '}';
        AjaxService.Post("/api/cxa/checkout/SetShippingMethods", JSON.parse(data), function (data, success, sender) {
            if (success && data.Success) {
                window.location.href = data.NextPageLink;
            }

            $("#ToBillingButton").button('reset');
        }, $(this));
        return false;
    }
}

ko.bindingHandlers.checkMe = {
    init: function (element, valueAccessor, all, vm, bindingContext) {
        ko.utils.registerEventHandler(element, "click", function () {
            var checkedValue = valueAccessor(),
                meValue = bindingContext.$data,
                checked = element.checked;
            if (checked && ko.isObservable(checkedValue)) {
                checkedValue(meValue);
            }
        });
    },
    update: function (element, valueAccessor, all, vm, bindingContext) {
        var checkedValue = ko.utils.unwrapObservable(valueAccessor()),
            meValue = bindingContext.$data;

        element.checked = (checkedValue === meValue);
    }
};

function isItemValid(element, index, array) {
    return (element === true);
}