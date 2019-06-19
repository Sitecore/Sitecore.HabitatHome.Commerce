// Copyright 2017 Sitecore Corporation A/S
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
    'use strict';
    if (typeof define === 'function' && define.amd) {
        // use AMD define funtion to support AMD modules if in use
        define(['exports'], factory);

    } else if (typeof exports === 'object') {
        // to support CommonJS
        factory(exports);

    }

    // browser global variable
    root.CheckoutDelivery = factory;
    root.CheckoutDelivery_ComponentClass = "cxa-checkoutdelivery-component";

}(this, function (element) {
    'use strict';
    var component = new Component(element);
    component.Name = "CXA/Feature/CheckoutDelivery";
    component.deliveryVM = null;
    component.Element = element;

    component.InExperienceEditorMode = function () {
       // component.Visual.Disable();
    }

    component.StartListening = function () {
    }
    component.StopListening = function () {
    }

    component.Init = function () {
        component.deliveryVM = new DeliveryDataViewModel();
        component.deliveryVM.load();
        ko.applyBindingsWithValidation(component.deliveryVM, component.Element);

        component.StartListening();
        component.SetupShippingPage();
    }

    component.initObservables = function () {
        method = function (description, id, name) {
            this.description = description;
            this.id = id;
            this.name = name;
        }
    }

    component.SetupShippingPage = function () {
        $("#orderGetShippingMethods").click(function () {
            component.deliveryVM.getShippingMethods();
        });

        $("body").on('click', '.lineGetShippingMethods', function () {
            var lineId = $(this).attr('id').replace('lineGetShippingMethods-', '');
            var line = ko.utils.arrayFirst(component.deliveryVM.cart().cartLines(), function (l) {
                return l.externalCartLineId === lineId;
            });

            if (line && line.shippingAddress() && line.shippingAddress.errors().length === 0) {
                $("#lineGetShippingMethods-" + lineId).button('loading');
                var party = ko.toJS(line.shippingAddress());
                var lines = [{ "ExternalCartLineId": lineId, "ShippingPreferenceType": line.selectedShippingOption() }];
                var data = { ShippingAddress: party, ShippingPreferenceType: component.deliveryVM.selectedShippingOption(), Lines: lines };
                AjaxService.Post("/api/cxa/checkout/GetShippingMethods", data, function (data, success, sender) {
                    var lineId = sender.attr('id').replace('lineGetShippingMethods-', '');
                    if (data.Success && success && component.deliveryVM != null) {
                        var match = ko.utils.arrayFirst(component.deliveryVM.cart().cartLines(), function (item) {
                            return item.externalCartLineId === lineId;
                        });

                        match.shippingMethods.removeAll();
                        $.each(data.LineShippingMethods[0].ShippingMethods, function (i, v) {
                            match.shippingMethods.push(new method(v.Description, v.ExternalId, v.Name));
                        });
                    }

                    $("#lineGetShippingMethods-" + lineId).button('reset');
                }, $(this));
            } else {
                line.shippingAddress.errors.showAllMessages();
            }
        });

        component.initObservables();
    }
}));

$(document).ready(function () {
    $("." + CheckoutDelivery_ComponentClass).each(function () {
        var component = new CheckoutDelivery(this);
    });
});
