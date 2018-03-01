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

(function (root, factory) {

    root.AddressList = factory;

}(this, function (element) {


    var $element = $(element);

    var maxRecords = $element.data("max-records");
    var addressPageUrl = $element.data("page-url");

    var component = new Component(element);
    component.Name = "CXA/Feature/AddressList";

    component.InExperienceEditorMode = function () {
    };

    var _BindModelData = function(addressPageUrl, maxRecords)
    {
        ko.applyBindingsWithValidation(component.model, component.RootElement);
        component.model.showLoader(false);
    }

    var _GetMockData = function (addressPageUrl, maxRecords) {
        var mockData = { Addresses: [] };

        for (var addressNumber = 1; addressNumber <= maxRecords; addressNumber++) {
            mockData.Addresses.push({ DetailsUrl: addressPageUrl, Address1: "Lorem ipsum dolor sit", Address2: "consectetur adipiscing elit", City: "Nunc", Country: "Proin", State: "Amet", IsPrimary: (addressNumber === 1) });
        }

        return mockData;
    }

    component.Init = function () {

        if (CXAApplication.IsExperienceEditorMode()) {
            var mockData = _GetMockData(addressPageUrl, maxRecords);
            component.model = new AddressListViewModel(mockData, addressPageUrl);
            _BindModelData();
        }
        else {
            AjaxService.Post("/api/cxa/Account/AddressList", { maxRecordsToShow: maxRecords }, function (data, success, sender) {
                console.log(data);
                console.log(success);

                if (success && data) {
                    component.model = new AddressListViewModel(data, addressPageUrl);
                    _BindModelData();
                }
            });
        }
    };

    return component;
}));
