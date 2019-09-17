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

function AddressViewModel(address) {
  var self = this;

  self.externalId = address ? ko.observable(address.ExternalId) : ko.observable();
  self.partyId = address ? ko.observable(address.ExternalId) : ko.observable();
  self.name = address ? ko.validatedObservable(address.Name).extend({ required: true }) : ko.validatedObservable().extend({ required: true });
  self.address1 = address ? ko.validatedObservable(address.Address1).extend({ required: true }) : ko.validatedObservable().extend({ required: true });
  self.city = address ? ko.validatedObservable(address.City).extend({ required: true }) : ko.validatedObservable().extend({ required: true });
  self.state = address ? ko.validatedObservable(address.RegionCode).extend({ required: true }) : ko.validatedObservable().extend({ required: true });
  self.zipPostalCode = address ? ko.validatedObservable(address.ZipPostalCode).extend({ required: true }) : ko.validatedObservable().extend({ required: true });
  self.country = address ? ko.validatedObservable(address.CountryCode) : ko.validatedObservable();
  self.isPrimary = address ? ko.observable(address.IsPrimary) : ko.observable();
  self.fullAddress = address ? ko.observable(address.FullAddress) : ko.observable();
  self.detailsUrl = address ? ko.observable(address.DetailsUrl) : ko.observable();

  self.states = ko.observableArray();
  self.country.subscribe(function (countryCode) {
    self.states.removeAll();
  });

  self.clone = function () {
    var newAddress = new AddressViewModel();

    newAddress.externalId(self.externalId());
    newAddress.partyId(self.partyId());
    newAddress.name(self.name());
    newAddress.address1(self.address1());
    newAddress.city(self.city());
    newAddress.state(self.state());
    newAddress.zipPostalCode(self.zipPostalCode());
    newAddress.country(self.country());
    newAddress.isPrimary(self.isPrimary());
    newAddress.fullAddress(self.fullAddress());
    newAddress.detailsUrl(self.detailsUrl());
    newAddress.states(self.states());

    return newAddress;
  };
}