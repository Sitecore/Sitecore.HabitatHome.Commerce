// -----------------------------------------------------------------------
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

var shippingMethodsArray = [];
var methodsViewModel = null;
var method = null;

function DeliveryDataViewModel() {
  var self = this;
  var Country = function (name, code) {
    this.country = name;
    this.code = code;
  };

  function isItemValid(element, index, array) {
    return element === true;
  }

  function getShippingMethodsMockData() {
    var mockData = {
      "ShippingMethods": [
        {
          "ExternalId": "65a9ae69-77f8-4530-b5ba-bde0615cbc28",
          "Description": "Next Day Air",
          "Name": "Next Day Air",
          "ShippingOptionId": null,
          "ShopName": null,
          "Errors": [],
          "Info": [],
          "Warnings": [],
          "HasErrors": false,
          "HasInfo": false,
          "HasWarnings": false,
          "Success": true,
          "Url": null,
          "ContentEncoding": null,
          "ContentType": null,
          "Data": null,
          "JsonRequestBehavior": 1,
          "MaxJsonLength": null,
          "RecursionLimit": null
        },
        {
          "ExternalId": "b146622d-dc86-48a3-b72a-05ee8ffd187a",
          "Description": "Ground",
          "Name": "Ground",
          "ShippingOptionId": null,
          "ShopName": null,
          "Errors": [],
          "Info": [],
          "Warnings": [],
          "HasErrors": false,
          "HasInfo": false,
          "HasWarnings": false,
          "Success": true,
          "Url": null,
          "ContentEncoding": null,
          "ContentType": null,
          "Data": null,
          "JsonRequestBehavior": 1,
          "MaxJsonLength": null,
          "RecursionLimit": null
        },
        {
          "ExternalId": "cf0af82a-e1b8-45c2-91db-7b9847af287c",
          "Description": "Standard",
          "Name": "Standard",
          "ShippingOptionId": null,
          "ShopName": null,
          "Errors": [],
          "Info": [],
          "Warnings": [],
          "HasErrors": false,
          "HasInfo": false,
          "HasWarnings": false,
          "Success": true,
          "Url": null,
          "ContentEncoding": null,
          "ContentType": null,
          "Data": null,
          "JsonRequestBehavior": 1,
          "MaxJsonLength": null,
          "RecursionLimit": null
        },
        {
          "ExternalId": "d5930340-c1dd-482c-b972-50a9250bb7a6",
          "Description": "Standard Overnight",
          "Name": "Standard Overnight",
          "ShippingOptionId": null,
          "ShopName": null,
          "Errors": [],
          "Info": [],
          "Warnings": [],
          "HasErrors": false,
          "HasInfo": false,
          "HasWarnings": false,
          "Success": true,
          "Url": null,
          "ContentEncoding": null,
          "ContentType": null,
          "Data": null,
          "JsonRequestBehavior": 1,
          "MaxJsonLength": null,
          "RecursionLimit": null
        }
      ],
      "LineShippingMethods": null,
      "Errors": [],
      "Info": [],
      "Warnings": [],
      "HasErrors": false,
      "HasInfo": false,
      "HasWarnings": false,
      "Success": true,
      "Url": null,
      "ContentEncoding": null,
      "ContentType": null,
      "Data": null,
      "JsonRequestBehavior": 1,
      "MaxJsonLength": null,
      "RecursionLimit": null
    };

    return mockData;
  }

  function getMiniProductMockImage() {
    var imageSrc = "";
    var pageExtension = getCurrentPageExtension();
    var tenantAndSiteNames = getCurrentTenantAndSiteName();
    if (pageExtension === "html") {
      imageSrc =
            "-/media/Project/" + tenantAndSiteNames.Tenant + "/" + tenantAndSiteNames.Site + "/Placeholder Images/72x72.png";
    } else {
      imageSrc =
            "/sitecore/shell/-/media/Feature/Experience%20Accelerator/Commerce/Catalog/72x72.png?h=72&w=72";
    }
    return imageSrc;
  }

  function getDeliveryMockData() {
    var productMockImage = getMiniProductMockImage();
    var mockData = {
      "OrderShippingOptions":
            [
              {
                "ExternalId": "3817f8d5-994b-4fbc-8bbe-4c342ec7553a",
                "Description": "Ship items",
                "Name": "Ship items",
                "ShippingOptionType": { "Value": 1, "Name": "ShipToAddress" },
                "ShopName": null,
                "Errors": [],
                "Info": [],
                "Warnings": [],
                "HasErrors": false,
                "HasInfo": false,
                "HasWarnings": false,
                "Success": true,
                "Url": null,
                "ContentEncoding": null,
                "ContentType": null,
                "Data": null,
                "JsonRequestBehavior": 1,
                "MaxJsonLength": null,
                "RecursionLimit": null
              },
              {
                "ExternalId": "3878b502-85ac-41b5-9203-e0f8712a854b",
                "Description": "Digital",
                "Name": "Digital",
                "ShippingOptionType": { "Value": 3, "Name": "ElectronicDelivery" },
                "ShopName": null,
                "Errors": [],
                "Info": [],
                "Warnings": [],
                "HasErrors": false,
                "HasInfo": false,
                "HasWarnings": false,
                "Success": true,
                "Url": null,
                "ContentEncoding": null,
                "ContentType": null,
                "Data": null,
                "JsonRequestBehavior": 1,
                "MaxJsonLength": null,
                "RecursionLimit": null
              },
              {
                "ExternalId": "2fb0128e-fbbd-4fce-9197-f000d8d39f5c",
                "Description": "Select delivery options by item",
                "Name": "Select delivery options by item",
                "ShippingOptionType": { "Value": 4, "Name": "DeliverItemsIndividually" },
                "ShopName": null,
                "Errors": [],
                "Info": [],
                "Warnings": [],
                "HasErrors": false,
                "HasInfo": false,
                "HasWarnings": false,
                "Success": true,
                "Url": null,
                "ContentEncoding": null,
                "ContentType": null,
                "Data": null,
                "JsonRequestBehavior": 1,
                "MaxJsonLength": null,
                "RecursionLimit": null
              }
            ],
      "LineShippingOptions":
            [
              {
                "LineId": "acd9474b824746f993c2258280718f19",
                "ShippingOptions": [
                  {
                    "ExternalId": "3817f8d5-994b-4fbc-8bbe-4c342ec7553a",
                    "Description": "Ship items",
                    "Name": "Ship items",
                    "ShippingOptionType": { "Value": 1, "Name": "ShipToAddress" },
                    "ShopName": null,
                    "Errors": [],
                    "Info": [],
                    "Warnings": [],
                    "HasErrors": false,
                    "HasInfo": false,
                    "HasWarnings": false,
                    "Success": true,
                    "Url": null,
                    "ContentEncoding": null,
                    "ContentType": null,
                    "Data": null,
                    "JsonRequestBehavior": 1,
                    "MaxJsonLength": null,
                    "RecursionLimit": null
                  }
                ],
                "Errors": [],
                "Info": [],
                "Warnings": [],
                "HasErrors": false,
                "HasInfo": false,
                "HasWarnings": false,
                "Success": true,
                "Url": null,
                "ContentEncoding": null,
                "ContentType": null,
                "Data": null,
                "JsonRequestBehavior": 1,
                "MaxJsonLength": null,
                "RecursionLimit": null
              },
              {
                "LineId": "726073d348b44edeb96f1eb1bff1890d",
                "ShippingOptions": [
                  {
                    "ExternalId": "3878b502-85ac-41b5-9203-e0f8712a854b",
                    "Description": "Digital",
                    "Name": "Digital",
                    "ShippingOptionType": {
                      "Value": 3,
                      "Name": "ElectronicDelivery"
                    },
                    "ShopName": null,
                    "Errors": [],
                    "Info": [],
                    "Warnings": [],
                    "HasErrors": false,
                    "HasInfo": false,
                    "HasWarnings": false,
                    "Success": true,
                    "Url": null,
                    "ContentEncoding": null,
                    "ContentType": null,
                    "Data": null,
                    "JsonRequestBehavior": 1,
                    "MaxJsonLength": null,
                    "RecursionLimit": null
                  }
                ],
                "Errors": [],
                "Info": [],
                "Warnings": [],
                "HasErrors": false,
                "HasInfo": false,
                "HasWarnings": false,
                "Success": true,
                "Url": null,
                "ContentEncoding": null,
                "ContentType": null,
                "Data": null,
                "JsonRequestBehavior": 1,
                "MaxJsonLength": null,
                "RecursionLimit": null
              }
            ],
      "EmailDeliveryMethod":
        {
          "ExternalId": "8a23234f-8163-4609-bd32-32d9dd6e32f5",
          "Description": "Email",
          "Name": "Email",
          "ShippingOptionId": null,
          "ShopName": null,
          "Errors": [],
          "Info": [],
          "Warnings": [],
          "HasErrors": false,
          "HasInfo": false,
          "HasWarnings": false,
          "Success": true,
          "Url": null,
          "ContentEncoding": null,
          "ContentType": null,
          "Data": null,
          "JsonRequestBehavior": 1,
          "MaxJsonLength": null,
          "RecursionLimit": null
        },
      "Cart": {
        "Lines": [
          {
            "ExternalCartLineId": "726073d348b44edeb96f1eb1bff1890d",
            "Image": productMockImage,
            "DisplayName": "Lorem ipsum dolor sit amet, consectetur",
            "ProductUrl": "/#",
            "ColorInformation": null,
            "SizeInformation": null,
            "StyleInformation": null,
            "GiftCardAmountInformation": "$0",
            "Quantity": "1",
            "LinePrice": "0.00 USD",
            "LineTotal": "0.00 USD",
            "Properties": { "Lorem": "ipsum" },
            "ShippingOptions": [
              {
                "ExternalId": "3878b502-85ac-41b5-9203-e0f8712a854b",
                "Description": "Digital",
                "Name": "Digital",
                "ShippingOptionType": { "Value": 3, "Name": "ElectronicDelivery" },
                "ShopName": null,
                "Errors": [],
                "Info": [],
                "Warnings": [],
                "HasErrors": false,
                "HasInfo": false,
                "HasWarnings": false,
                "Success": true,
                "Url": null,
                "ContentEncoding": null,
                "ContentType": null,
                "Data": null,
                "JsonRequestBehavior": 1,
                "MaxJsonLength": null,
                "RecursionLimit": null
              }
            ],
            "DiscountOfferNames": [],
            "LineDiscount": "0.00 USD",
            "ProductId": "0000000",
            "ShippingMethodName": "Etiam rhoncus",
            "Address": {
              "Address1": "Mauris eget lacus sed dolor viverra",
              "City": "Etiam",
              "State": "In gravida",
              "ZipPostalCode": "99999",
              "Country": "Nam pulvinar"
            },
            "Errors": [],
            "Info": [],
            "Warnings": [],
            "HasErrors": false,
            "HasInfo": false,
            "HasWarnings": false,
            "Success": true,
            "Url": null,
            "ContentEncoding": null,
            "ContentType": null,
            "Data": null,
            "JsonRequestBehavior": 1,
            "MaxJsonLength": null,
            "RecursionLimit": null
          },
          {
            "ExternalCartLineId": "e06202acdcdf487681a944183ebda795",
            "Image": productMockImage,
            "DisplayName": "Lorem ipsum dolor sit amet, consectetur",
            "ProductUrl":
                        "/#",
            "ColorInformation": "dolor",
            "SizeInformation": null,
            "StyleInformation": null,
            "GiftCardAmountInformation": null,
            "Quantity": "1",
            "LinePrice": "0.00 USD",
            "LineTotal": "0.00 USD",
            "Properties": { "Lorem": "ipsum" },
            "ShippingOptions": [
              {
                "ExternalId": "3817f8d5-994b-4fbc-8bbe-4c342ec7553a",
                "Description": "Ship items",
                "Name": "Ship items",
                "ShippingOptionType": { "Value": 1, "Name": "ShipToAddress" },
                "ShopName": null,
                "Errors": [],
                "Info": [],
                "Warnings": [],
                "HasErrors": false,
                "HasInfo": false,
                "HasWarnings": false,
                "Success": true,
                "Url": null,
                "ContentEncoding": null,
                "ContentType": null,
                "Data": null,
                "JsonRequestBehavior": 1,
                "MaxJsonLength": null,
                "RecursionLimit": null
              }
            ],
            "DiscountOfferNames": [],
            "LineDiscount": "0.00 USD",
            "ProductId": "0000000",
            "ShippingMethodName": "Etiam rhoncus",
            "Address": {
              "Address1": "Mauris eget lacus sed dolor viverra",
              "City": "Etiam",
              "State": "In gravida",
              "ZipPostalCode": "99999",
              "Country": "Nam pulvinar"
            },
            "Errors": [],
            "Info": [],
            "Warnings": [],
            "HasErrors": false,
            "HasInfo": false,
            "HasWarnings": false,
            "Success": true,
            "Url": null,
            "ContentEncoding": null,
            "ContentType": null,
            "Data": null,
            "JsonRequestBehavior": 1,
            "MaxJsonLength": null,
            "RecursionLimit": null
          }
        ],
        "Email": "",
        "Subtotal": "0.00 USD",
        "TaxTotal": "0.00 USD",
        "Total": "0.00 USD",
        "ShippingTotal": "0.00 USD",
        "TotalAmount": 0.00,
        "Shipments": [],
        "Payments": [],
        "Parties": [],
        "AccountingParty": null,
        "Discount": "0.00 USD",
        "PromoCodes": [],
        "Errors": [],
        "Info": [],
        "Warnings": [],
        "HasErrors": false,
        "HasInfo": false,
        "HasWarnings": false,
        "Success": true,
        "Url": null,
        "ContentEncoding": null,
        "ContentType": null,
        "Data": null,
        "JsonRequestBehavior": 1,
        "MaxJsonLength": null,
        "RecursionLimit": null
      },
      "CurrencyCode": "USD",
      "IsUserAuthenticated": false,
      "UserEmail": "",
      "Countries": { "CA": "Canada", "US": "United States" },
      "UserAddresses": null,
      "Errors": [],
      "Info": [],
      "Warnings": [],
      "HasErrors": false,
      "HasInfo": false,
      "HasWarnings": false,
      "Success": true,
      "Url": null,
      "ContentEncoding": null,
      "ContentType": null,
      "Data": null,
      "JsonRequestBehavior": 1,
      "MaxJsonLength": null,
      "RecursionLimit": null
    };

    return mockData;
  }

  function initializeDeliveryModel(data) {
    if (data) {
      self.emailDeliveryMethod(data.EmailDeliveryMethod);
      self.currencyCode = data.CurrencyCode;

      if (data.Countries) {
        $.each(data.Countries,
          function (index, value) {
            self.countries.push(new Country(value, index));
          });
      }

      if (data.IsUserAuthenticated === true && data.UserAddresses && data.UserAddresses.Addresses) {
        $.each(data.UserAddresses.Addresses,
          function () {
            self.userAddresses.push(new AddressViewModel(this));
          });

        self.isAuthenticated(true);
        self.userEmail = data.UserEmail;
      }

      self.cart(new CartViewModel(data.Cart, self.userAddresses()));

      if (data.OrderShippingOptions) {
        $.each(data.OrderShippingOptions,
          function (index, value) {
            self.orderShippingOptions.push(value);
          });
      }

      self.handleEditMode(data);
    }
  }

  self.cart = ko.observable("");
  self.currencyCode = ko.observable("");

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
    if (match) {
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
        return self.shippingMethod.isValid() && self.shippingAddress.isValid();
      }

      if (self.isShipItems()) {
        var isValid = [];
        $.each(self.cart().cartLines(), function () {
          if (this.isLineShipToEmail()) {
            isValid.push(this.shippingEmail() && this.shippingEmail.isValid());
          } else if (this.isLineShipAll()) {
            isValid.push(this.shippingMethod.isValid() && this.shippingAddress.isValid());
          } else {
            isValid.push(false);
          }
        });

        return isValid.every(isItemValid);
      }

      return null;
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
    if (CXAApplication.IsExperienceEditorMode()) {
      var mockData = getShippingMethodsMockData();
      self.setShippingMethods(mockData.ShippingMethods);
      $("#orderGetShippingMethods").button('reset');
    } else if (self.shippingAddress() && self.shippingAddress.errors().length === 0) {
      $("#orderGetShippingMethods").button('loading');
      var party = ko.toJS(self.shippingAddress());
      var data = { ShippingAddress: party, ShippingPreferenceType: self.selectedShippingOption(), Lines: null };

      AjaxService.Post("/api/cxa/checkout/GetShippingMethods",
        data,
        function (data, success, sender) {
          if (data.Success && success) {
            self.setShippingMethods(data.ShippingMethods);
          }

          $("#orderGetShippingMethods").button('reset');
        }, $(this));
    } else {
      component.deliveryVM.shippingAddress.errors.showAllMessages();
    }
  };

  self.setShippingMethods = function (shippingMethodDataList) {
    var methods = "";
    self.shippingMethods.removeAll();
    $.each(shippingMethodDataList, function (i, v) {
      self.shippingMethods.push(new method(v.Description, v.ExternalId, v.Name));
    });
  };

  self.handleEditModeShipAll = function (data) {
    self.isShipAll(true);

    var shipment = data.Cart.Shipments[0];
    if (shipment && shipment.EditModeShippingOptionType ) {
      self.selectedShippingOption(shipment.EditModeShippingOptionType.Value);
      var orderShippingPreference = self.selectedShippingOption();

      // ShipToAddress
      if (orderShippingPreference === 1) {
        var partyId = self.cart().shipments[0].partyId;
        var party = ko.utils.arrayFirst(self.cart().parties, function (a) {
          if (a.externalId() === partyId) {
            return a;
          }

          return null;
        });

        if (party) {
          self.shippingAddress(party);
        }
      }

      // Email
      if (orderShippingPreference === 3) {
        self.shippingEmail(shipment.Email);
        self.shippingEmailContent(shipment.EmailContent);
      }

      if (shipment.ShipmentEditModel && shipment.ShipmentEditModel.ShippingMethods) {
        self.setShippingMethods(shipment.ShipmentEditModel.ShippingMethods);

        var sm = ko.utils.arrayFirst(self.shippingMethods(), function (a) {
          if (a.name === shipment.ShippingMethodName) {
            return a;
          }

          return null;
        });

        if (sm) {
          self.shippingMethod(sm);
        }
      }
    }
  };

  self.handleEditModeShipIndividually = function (data) {
    self.isShipAll(false);
    self.isShipItems(true);
    self.selectedShippingOption(4);
    $.each(self.cart().cartLines(), function (index, value) {
      value.handleEditMode(data);
    });
  };

  self.handleEditMode = function (data) {
    if (self.cart().shipments && self.cart().shipments.length > 0) {
      if (self.cart().shipments.length === 1) {
        self.handleEditModeShipAll(data);
      } else {
        self.handleEditModeShipIndividually(data);
      }
    }
  };

  self.load = function () {
    if (CXAApplication.IsExperienceEditorMode()) {
      var data = getDeliveryMockData();
      initializeDeliveryModel(data);
    } else {
      AjaxService.Post("/api/cxa/checkout/GetDeliveryData", null, function (data, success, sender) {
        if (success && data.Success) {
          initializeDeliveryModel(data);
        }
      });
    }
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
        "ShippingMethodName": self.shippingMethod().name,
        "ShippingPreferenceType": orderShippingPreference,
        "PartyID": partyId
      });
    } else if (orderShippingPreference === 2) {
      var storeId = self.store().externalId();
      parties.push({
        "Name": self.store().name(),
        "Address1": self.store().address()
          .address1(),
        "Country": self.store().address()
          .country(),
        "City": self.store().address()
          .city(),
        "State": self.store().address()
          .state(),
        "ZipPostalCode": self.store().address()
          .zipPostalCode(),
        "ExternalId": storeId,
        "PartyId": storeId
      });

      shipping.push({
        "ShippingMethodID": self.shipToStoreDeliveryMethod().ExternalId,
        "ShippingMethodName": self.shipToStoreDeliveryMethod().Name,
        "ShippingPreferenceType": orderShippingPreference,
        "PartyID": storeId
      });
    } else if (orderShippingPreference === 4) {
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
            "ShippingMethodName": this.shippingMethod().name,
            "ShippingPreferenceType": lineDeliveryPreference,
            "PartyID": partyId,
            "LineIDs": [lineId]
          });
        }

        if (lineDeliveryPreference === 3) {
          shipping.push({
            "ShippingMethodID": self.emailDeliveryMethod().ExternalId,
            "ShippingMethodName": self.emailDeliveryMethod().Name,
            "ShippingPreferenceType": lineDeliveryPreference,
            "ElectronicDeliveryEmail": this.shippingEmail(),
            "ElectronicDeliveryEmailContent": this.shippingEmailContent(),
            "LineIDs": [lineId]
          });
        }
      });
    } else if (orderShippingPreference === 3) {
      shipping.push({
        "ShippingMethodID": self.emailDeliveryMethod().ExternalId,
        "ShippingMethodName": self.emailDeliveryMethod().Name,
        "ShippingPreferenceType": orderShippingPreference,
        "ElectronicDeliveryEmail": self.shippingEmail(),
        "ElectronicDeliveryEmailContent": self.shippingEmailContent()
      });
    }

    var data = '{"DeliveryItemPath": "' + $("#DeliveryItemPath").val() + '", "OrderShippingPreferenceType": "' + orderShippingPreference + '", "ShippingMethods":' + JSON.stringify(shipping) + ', "ShippingAddresses":' + JSON.stringify(parties) + '}';
    AjaxService.Post("/api/cxa/checkout/SetShippingMethods", JSON.parse(data), function (data, success, sender) {
      if (success && data.Success) {
        window.location.href = data.NextPageLink;
      }

      $("#ToBillingButton").button('reset');
    }, $(this));
    return false;
  };
}

/* eslint-disable max-params */
/* Cannot reduce the number of params as the init and update callback method definitions are defined by knockout.js */
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

    element.checked = checkedValue === meValue;
  }
};