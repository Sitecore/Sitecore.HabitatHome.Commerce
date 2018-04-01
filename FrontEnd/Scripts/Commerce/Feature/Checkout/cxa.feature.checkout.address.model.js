function AddressViewModel(address) {
    var self = this;

    var populate = address != null;

    self.externalId = populate ? ko.observable(address.ExternalId) : ko.observable();
    self.partyId = populate ? ko.observable(address.ExternalId) : ko.observable();
    self.name = populate ? ko.validatedObservable(address.Name).extend({ required: true }) : ko.validatedObservable().extend({ required: true });
    self.address1 = populate ? ko.validatedObservable(address.Address1).extend({ required: true }) : ko.validatedObservable().extend({ required: true });
    self.city = populate ? ko.validatedObservable(address.City).extend({ required: true }) : ko.validatedObservable().extend({ required: true });
    self.state = populate ? ko.validatedObservable(address.RegionCode).extend({ required: true }) : ko.validatedObservable().extend({ required: true });
    self.zipPostalCode = populate ? ko.validatedObservable(address.ZipPostalCode).extend({ required: true }) : ko.validatedObservable().extend({ required: true });
    self.country = populate ? ko.validatedObservable(address.CountryCode) : ko.validatedObservable();
    self.isPrimary = populate ? ko.observable(address.IsPrimary) : ko.observable();
    self.fullAddress = populate ? ko.observable(address.FullAddress) : ko.observable();
    self.detailsUrl = populate ? ko.observable(address.DetailsUrl) : ko.observable();

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
    }
}
