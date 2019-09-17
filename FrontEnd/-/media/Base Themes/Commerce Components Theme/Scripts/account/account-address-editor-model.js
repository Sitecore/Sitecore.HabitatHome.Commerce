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

var defaultCountryCode = "USA";

function AddressEditorViewModel(data, accountPageUrl, addressId, rootElementRef) {
  var self = this;
  var states = function ($element, text) { $($element).button(text); };

  function CountryStateViewModel(name, code) {
    this.name = name;
    this.code = code;
  }

  function AddressEditorItemViewModel(address) {
    let self = this;

    self.externalId = address ? ko.observable(address.ExternalId) : ko.observable();
    self.partyId = address ? ko.observable(address.ExternalId) : ko.observable();
    self.name = address ? ko.validatedObservable(address.Name).extend({ required: true }) : ko.validatedObservable().extend({ required: true });
    self.address1 = address ? ko.validatedObservable(address.Address1).extend({ required: true }) : ko.validatedObservable().extend({ required: true });
    self.city = address ? ko.validatedObservable(address.City).extend({ required: true }) : ko.validatedObservable().extend({ required: true });
    self.state = address ? ko.validatedObservable(address.State).extend({ required: false }) : ko.validatedObservable().extend({ required: false });
    self.zipPostalCode = address ? ko.validatedObservable(address.ZipPostalCode).extend({ required: true }) : ko.validatedObservable().extend({ required: true });
    self.country = address ? ko.validatedObservable(address.Country).extend({ required: true }) : ko.validatedObservable().extend({ required: true });
    self.isPrimary = address ? ko.observable(address.IsPrimary) : ko.observable();
    self.fullAddress = address ? ko.observable(address.FullAddress) : ko.observable();

    self.states = ko.observableArray();

    self.country.subscribe(function (countryCode) {
      self.states.removeAll();
    });
  }

  var InitModel = function (data, reset) {
    if (!data) {
      return;
    }

    if (reset) {
      self.addresses.removeAll();
      self.countries.removeAll();
    }

    if (data.Addresses) {
      $.each(data.Addresses, function () {
        self.addresses.push(new AddressEditorItemViewModel(this));
      });
    }

    if (data.Countries) {
      $.each(data.Countries, function (code, name) {
        self.countries.push(new CountryStateViewModel(name, code));
      });
    }
  };

  var EnableButtons = function (enable) {
    self.enableDelete(enable);
    self.enableSave(enable);
    self.enableCancel(enable);
  };

  // //////////////////////////////////////////////////// INNER PRIVATE MODELS ////////////////////////////////////////////////////
  self = this;

  self.accountPageUrl = accountPageUrl;
  self.rootElement = $(rootElementRef);

  self.addresses = ko.observableArray();
  self.countries = ko.observableArray();

  InitModel(data);

  self.showLoader = ko.observable(true);
  self.enableDelete = ko.observable(false);
  self.enableSave = ko.observable(true);
  self.enableCancel = ko.observable(true);

  self.address = ko.validatedObservable(new AddressEditorItemViewModel());
  self.selectedAddress = ko.observable();

  self.selectedAddress.subscribe(function (externalId) {
    var address = ko.utils.arrayFirst(this.addresses(), function (a) {
      if (a.externalId() === externalId) {
        return a;
      }

      return null;
    });

    if (address) {
      self.address(address);
      self.enableDelete(true);
    } else {
      self.address(new AddressEditorItemViewModel());
      self.enableDelete(false);
    }
  }.bind(this));

  if (addressId) {
    self.selectedAddress(addressId);
  }

  self.saveAddress = function () {
    if (self.address.errors().length === 0) {
      states(self.rootElement.find("#saveAddress"), 'loading');
      EnableButtons(false);

      var address = JSON.parse(ko.toJSON(self.address));

      AjaxService.Post("/api/cxa/AccountAddress/AddressEditorModify", address, function (data, success, sender) {
        if (data.Success) {
          self.reload(data);
          states(self.rootElement.find("#saveAddress"), 'reset');
        } else {
          self.enableSave(true);
        }
      });
    } else {
      self.address.errors.showAllMessages();
      self.rootElement.find('#addressBook-Name').focus();
    }
  };

  self.deleteAddress = function () {
    states(self.rootElement.find("#deleteAddress"), "loading");
    EnableButtons(false);

    AjaxService.Post("/api/cxa/AccountAddress/AddressEditorDelete", { addressId: self.address().externalId() }, function (data, success, sender) {
      self.reload(data);
      states(self.rootElement.find("#deleteAddress"), "reset");
    });
  };

  self.reload = function (data) {
    InitModel(data, true);

    self.selectedAddress("");
    self.address(new AddressEditorItemViewModel());

    self.enableDelete(false);
    self.enableSave(true);
    self.enableCancel(true);
  };
}