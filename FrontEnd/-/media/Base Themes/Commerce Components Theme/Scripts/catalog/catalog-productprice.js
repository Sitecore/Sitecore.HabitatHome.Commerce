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
    root.ProductPrice = factory;
    root.ProductPrice_ComponentClass = "cxa-productprice-component";

}(this, function (element) {
    'use strict';
    var component = new Component(element);
    component.Name = "CXA/Feature/ProductPrice";
    component.priceInfoVM = null;

    //ProductPriceContext Handlers
    component.ProductPriceSetPriceHandler = function (source, priceBefore, price, isOnSale, savingsMessage, data) {
        component.priceInfoVM.switchInfo(priceBefore, price, isOnSale, savingsMessage, data);
    };

    component.InExperienceEditorMode = function () {
        component.Visual.Disable();
    };

    component.StartListening = function () {
        component.HandlerId = ProductPriceContext.SubscribeHandler(component.ProductPriceSetPriceHandler);
    };

    component.StopListening = function () {
        if (component.HandlerId) {
            ProductPriceContext.UnSubscribeHandler(component.HandlerId);
        }
    };

    component.Init = function () {
        $(component.RootElement).find(".price-info").each(function () {
            component.priceInfoVM = new PriceInfoViewModel(component.RootElement);

            ko.applyBindingsWithValidation(component.priceInfoVM, this);
            component.Visual.Appear();
            component.StartListening();

            if (CXAApplication.IsExperienceEditorMode()) {
                var data = {};
                data.productId = $(component.RootElement).find('#productprice_productid').val();
                component.ProductPriceSetPriceHandler("MockData", "14.95 USD", "12.95 USD", "true", "- Save up to  13%", data);
            }
        });
    };

    return component;
}));