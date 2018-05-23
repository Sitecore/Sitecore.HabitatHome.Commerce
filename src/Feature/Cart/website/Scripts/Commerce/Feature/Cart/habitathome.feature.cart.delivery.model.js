
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

    self.isPickUp = ko.observable(false);
    self.pickUpStores = ko.observableArray();
    self.pickUpStoreId = ko.observable("");
    self.store = ko.observable(new NewPickupStore({ "ExternalId": "0" }));
    self.storeDeliveryMethod = ko.observable("");
   
    self.emailDeliveryMethod = ko.observable("");
    self.orderShippingOptions = ko.observableArray();

    self.selectedShippingOption = ko.observable('0');
    self.selectedShippingOption.subscribe(function (option) {
        self.isShipAll(option === 1);
        self.isShipToEmail(option === 3);
        self.isShipItems(option === 4);
        self.isPickUp(option === 2);
    }.bind(this));

    self.isAuthenticated = ko.observable(false);
    self.userEmail = "";

    self.userAddresses = ko.observableArray();
    self.userAddresses.push(new AddressViewModel({ "ExternalId": "UseShipping", "FullAddress": $("#billingAddressSelect").attr("title") }));
    self.userAddresses.push(new AddressViewModel({ "ExternalId": "UseOther", "FullAddress": $("#ShippingAddressSelect").attr("title2") }));

    self.shippingMethods = ko.observableArray();
    self.pickupMethod = ko.observableArray();
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
                    else if (this.isLinePickUp()) {    
                        var inpPickUpStore = $('input[name=pickUpOptions]:checked');
                        if (inpPickUpStore.length) {
                            var storeObject = ko.contextFor(inpPickUpStore.parents()[0]);
                            isValid.push(storeObject.$data.Address1 != null);
                        }
                    }
                    else {
                        isValid.push(false);
                    }
                });
                return isValid.every(isItemValid);
            }
            if (self.isPickUp()) {
                console.log("ispickuup");
                var inpPickUpStore = $('input[name=pickUpOptions]:checked');
                if (inpPickUpStore.length) {                    
                    var storeObject = ko.contextFor(inpPickUpStore.parents()[0]);
                    return (storeObject.$data.Address1 != null);
                }               
                
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

            if (v.Description == "Pickup From Store") {
                self.pickupMethod(new method(v.Description, v.ExternalId));
   
            }
            else {
                self.shippingMethods.push(new method(v.Description, v.ExternalId));
            }
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
                if (orderShippingPreference == '2') {
                    self.selectedShippingOption(2)
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


    var modelCartLines;
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

                self.cart(new DeliveryCartViewModel(data.Cart, self.userAddresses()));
                modelCartLines = self.cart().cartLines();

                if (data.OrderShippingOptions != null) {
                    $.each(data.OrderShippingOptions, function (index, value) {
                        self.orderShippingOptions.push(value);
                    });
                }

                AjaxService.Post("api/cxa/Delivery/GetCurrentCart", "", function (data, success, sender) {
                    if (success) {                        
                        $.each(data.Data, function () {
                            var exId;var prodId;var varId;
                            $(this).map(function (i, b) {
                                if ((b['Key'] == 'ExternalCartLineId'))
                                    exId = b['Value'];
                                if ((b['Key'] == 'ProductId'))
                                    prodId = b['Value'];
                                if ((b['Key'] == 'VariantId'))
                                    varId = b['Value'];
                            })
                            var match = ko.utils.arrayFirst(self.cart().cartLines(), function (a) {
                                if (a.externalCartLineId === exId) {
                                    if (getCookie("sxa_site_shops_stores") != "") {
                                        self.GetInventory(a, prodId, varId);
                                    }
                                    else {
                                        self.LoadNearestStores(a, prodId, varId);
                                    }
                                }
                            });
                        });

                    }
                })
                self.handleEditMode(data);
            }
        }
        )
        
    };

    self.GetInventory = function (cartLine, productId, variantId) {
        var ServiceRequest = new Object();
        ServiceRequest.ProductId = productId;
        ServiceRequest.VariantId = variantId;
        var promise = GetInventory(ServiceRequest).done(function (result) {
            if (result.Errors != null & result.Errors != "undefined") {
                self.ErrorMessage(data.Errors[0]);
            }
            else {
                var storesData = result.Data;
                var storesList = [];
                //self.NearestStoresList(storesData);
                $.each(result.Data, function (index, value) {
                    var newStore = new NewPickupStore(value);
                    storesList.push(newStore);
                });
                cartLine.linePickUpStores(storesList);
    
            }
        })
    }

    self.LoadNearestStores = function (cartLine, productId, variantId) {
        var ServiceRequest = new Object();
        ServiceRequest.Latitude = "";
        ServiceRequest.Longitude = "";
        ServiceRequest.ProductId = productId;
        ServiceRequest.VariantId = variantId;
        var promise = GetNearestStores(ServiceRequest).done(function (result) {
            if (result.Errors != null & result.Errors != "undefined") {
                self.ErrorMessage(data.Errors[0]);
            }
            else {

                var storesList = [];
                if (result.Data.length > 0) {
                    $.each(result.Data, function (index, value) {
                        var newStore = new NewPickupStore(value);
             
                        storesList.push(newStore);
                    });
                    cartLine.linePickUpStores(storesList);
                }
            }
        });
    }   

    self.setPickUpStore = function (model) {
                
        self.pickUpStoreId = model.InventoryStoreId;
        self.store(model);

    }
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
            var data = '{"DeliveryItemPath": "' + $("#DeliveryItemPath").val() + '", "OrderShippingPreferenceType": "' + orderShippingPreference + '", "ShippingMethods":' + JSON.stringify(shipping) + ', "ShippingAddresses":' + JSON.stringify(parties) + '}';
          
            AjaxService.Post("/api/cxa/checkout/SetShippingMethods", JSON.parse(data), function (data, success, sender) {
                if (success && data.Success) {
                    window.location.href = data.NextPageLink;
                }
                $("#ToBillingButton").button('reset');
            }, $(this));
        }
        
        else if (orderShippingPreference === 2) {
            var inpName = 'pickUpOptions';
            var inpPickUpStore = $('input[name=' + inpName + ']:checked');
            var storeObject = ko.contextFor(inpPickUpStore.parents()[0]);

            self.selectedShippingOption(2);

            if (self.store().InventoryStoreId == null) {

                self.setPickUpStore(storeObject.$data);

            }
            var storeshippingAddress = new Object();
            storeshippingAddress.externalId = 0;
            storeshippingAddress.partyId = 0;
            storeshippingAddress.name = self.store().InventoryStoreId;
            storeshippingAddress.address1 = self.store().Address1;
            storeshippingAddress.city = self.store().City;
            storeshippingAddress.state = self.store().State;
            storeshippingAddress.zipPostalCode = self.store().Zip;
            storeshippingAddress.country = self.store().Country;
            storeshippingAddress.isPrimary = false;
            storeshippingAddress.fullAddress = self.store().Address1 + " " + self.store().City + " " + self.store().State + " " + self.store().Zip + " " + self.store().Country;
            storeshippingAddress.detailsUrl = "";
            
            var party = ko.toJS(storeshippingAddress);
            
            var data = { ShippingAddress: party, ShippingPreferenceType: self.selectedShippingOption(), Lines: null };
            
            AjaxService.Post("/api/cxa/checkout/GetShippingMethods", data, function (data, success, sender) {
                if (data.Success && success) {                   
                    self.setShippingMethods(data.ShippingMethods);
                    var partyId = self.shippingAddress().externalId();
                    parties.push({
                        "Name": self.store().InventoryStoreId,
                        "Address1": self.store().Address1,
                        "Country": self.store().Country,
                        "City": self.store().City,
                        "State": self.store().State,
                        "ZipPostalCode": self.store().Zip,
                        "ExternalId": partyId,
                        "PartyId": partyId
                    });                   
                    shipping.push({
                        "ShippingMethodID": self.pickupMethod().id,
                        "ShippingMethodName": self.pickupMethod().description,
                        "ShippingPreferenceType": 1,
                        "PartyID": partyId
                    });
                   
                    var shipData = '{"DeliveryItemPath": "' + $("#DeliveryItemPath").val() + '", "OrderShippingPreferenceType": "' + orderShippingPreference + '", "ShippingMethods":' + JSON.stringify(shipping) + ', "ShippingAddresses":' + JSON.stringify(parties) + '}';
           
                    AjaxService.Post("/api/cxa/checkout/SetShippingMethods", JSON.parse(shipData), function (data, success, sender) {
                        if (success && data.Success) {
                            window.location.href = data.NextPageLink;
                        }
                        $("#ToBillingButton").button('reset');
                    }, $(this));

                }
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

                if (lineDeliveryPreference === 2) {

                    var partyId = this.shippingAddress().externalId();
                    var inpName = 'pickUpOptions-' + this.externalCartLineId;
                    var inpPickUpStore = $('input[name=' + inpName + ']:checked');
                    var storeObject = ko.contextFor(inpPickUpStore.parents()[0]);

                    if (storeObject == null) {
    
                        $('#ToBillingButton').attr('disabled');
                    }
             
                    var storeName = storeObject.$data.InventoryStoreId;
                    var address1 = storeObject.$data.Address1;
                    var city = storeObject.$data.City;
                    var state = storeObject.$data.State;
                    var zip = storeObject.$data.Zip;
                    var country = storeObject.$data.Country;

                    var storeshippingAddress = new Object();
                    storeshippingAddress.externalId = 0;
                    storeshippingAddress.partyId = 0;
                    storeshippingAddress.name = storeName;
                    storeshippingAddress.address1 = address1;
                    storeshippingAddress.city = city;
                    storeshippingAddress.state = state;
                    storeshippingAddress.zipPostalCode = zip;
                    storeshippingAddress.country = country;
                    storeshippingAddress.isPrimary = false;
                    storeshippingAddress.fullAddress = address1 + " " + city + " " + state + " " + zip + " " + country;
                    storeshippingAddress.detailsUrl = "";

                    var party = ko.toJS(storeshippingAddress);
                    var data = { ShippingAddress: party, ShippingPreferenceType: self.selectedShippingOption(), Lines: null };

                    AjaxService.Post("/api/cxa/checkout/GetShippingMethods", data, function (data, success, sender) {
                        if (data.Success && success) {
                            self.setShippingMethods(data.ShippingMethods);
                 
                            var inpName = 'pickUpOptions';
                            var inpPickUpStore = $('input[name=' + inpName + ']:checked');
                            var storeObject = ko.contextFor(inpPickUpStore.parents()[0]);

                            var partyId = self.shippingAddress().externalId();
                            parties.push({
                                "Name": storeName,
                                "Address1": address1,
                                "Country": country,
                                "City": city,
                                "State": state,
                                "ZipPostalCode": zip,
                                "ExternalId": partyId,
                                "PartyId": partyId
                            });

                            shipping.push({
                                "ShippingMethodID": self.pickupMethod().id,
                                "ShippingMethodName": self.pickupMethod().description,
                                "ShippingPreferenceType": 1,
                                "PartyID": partyId,
                                "LineIDs": [lineId]
                            });

                            var shipData = '{"DeliveryItemPath": "' + $("#DeliveryItemPath").val() + '", "OrderShippingPreferenceType": "' + orderShippingPreference + '", "ShippingMethods":' + JSON.stringify(shipping) + ', "ShippingAddresses":' + JSON.stringify(parties) + '}';
                            
                            AjaxService.Post("/api/cxa/checkout/SetShippingMethods", JSON.parse(shipData), function (data, success, sender) {
                                if (success && data.Success) {
                                    window.location.href = data.NextPageLink;
                                }
                                $("#ToBillingButton").button('reset');
                            }, $(this));

                        }
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

            var data = '{"DeliveryItemPath": "' + $("#DeliveryItemPath").val() + '", "OrderShippingPreferenceType": "' + orderShippingPreference + '", "ShippingMethods":' + JSON.stringify(shipping) + ', "ShippingAddresses":' + JSON.stringify(parties) + '}';
          
            AjaxService.Post("/api/cxa/checkout/SetShippingMethods", JSON.parse(data), function (data, success, sender) {
                if (success && data.Success) {
                    window.location.href = data.NextPageLink;
                }

                $("#ToBillingButton").button('reset');
            }, $(this));
        }

        if (orderShippingPreference != 2 && orderShippingPreference != 4) {
            var data = '{"DeliveryItemPath": "' + $("#DeliveryItemPath").val() + '", "OrderShippingPreferenceType": "' + orderShippingPreference + '", "ShippingMethods":' + JSON.stringify(shipping) + ', "ShippingAddresses":' + JSON.stringify(parties) + '}';

            AjaxService.Post("/api/cxa/checkout/SetShippingMethods", JSON.parse(data), function (data, success, sender) {
                if (success && data.Success) {
                    window.location.href = data.NextPageLink;
                }

                $("#ToBillingButton").button('reset');
            }, $(this));
        }
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
function GetNearestStores(ServiceRequest) {

    var url = "/api/cxa/NearestStore/GetStores?userLatitude=" + ServiceRequest.Latitude + "&userLongitude=" + ServiceRequest.Longitude + "&pid=" + ServiceRequest.ProductId + "-" + ServiceRequest.VariantId;
    var requestData = JSON.stringify(ServiceRequest);
    $('.loader').show();
    var ajaxRequest = $.ajax({
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        url: url,
        //data: { 'userLatitude': ServiceRequest.Latitude, 'userLongitude': ServiceRequest.Longitude },
        success: function (data) {
            $('.loader').hide();
        },
        error: function (x, y, z) {
        }
    });
    return ajaxRequest;
}

function GetInventory(ServiceRequest) {

    var url = "/api/cxa/NearestStore/GetInventory?pid=" + ServiceRequest.ProductId + "-" + ServiceRequest.VariantId;
    var requestData = JSON.stringify(ServiceRequest);
    $('.loader').show();
    var ajaxRequest = $.ajax({
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        url: url,
        //data: { 'userLatitude': ServiceRequest.Latitude, 'userLongitude': ServiceRequest.Longitude },
        success: function (data) {
            $('.loader').hide();
        },
        error: function (x, y, z) {
        }
    });
    return ajaxRequest;
}

function NewPickupStore(data) {
    self = this;    
    $(data).map(function (i, b) {   
        if ((b['Key'] == 'Id'))
            self.Id = b['Value'];
        if ((b['Key'] == 'ExternalId'))
            self.ExternalId = b['Value'];
        if ((b['Key'] == 'InventoryStoreId'))
            self.InventoryStoreId = b['Value'];
        if ((b['Key'] == 'InventoryStoreId'))
            self.Distance = b['Value'];
        if ((b['Key'] == 'ZeroInventory'))
            self.ZeroInventory = b['Value'];
        if ((b['Key'] == 'DisplayName'))
            self.DisplayName = b['Value'];
        if ((b['Key'] == 'Limited'))
            self.Limited = b['Value'];
        if ((b['Key'] == 'Quantity'))
            self.Quantity = b['Value'];
        if ((b['Key'] == 'InventoryAmount'))
            self.InventoryAmount = b['Value'];
        if ((b['Key'] == 'Address'))
            self.Address1 = b['Value'];
        if ((b['Key'] == 'City'))
            self.City = b["Value"];
        if ((b['Key'] == 'Zip'))
            self.Zip = b["Value"];
        if ((b['Key'] == 'State'))
            self.State = b["Value"];        
        if ((b['Key'] == 'Country'))
            self.Country = b["Value"];
    })
}







