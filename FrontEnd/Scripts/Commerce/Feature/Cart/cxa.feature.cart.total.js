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

    if (typeof define === "function" && define.amd) {
        // use AMD define funtion to support AMD modules if in use
        define("CXA/Feature/CartTotal", ["exports"], factory);

    } else if (typeof exports === "object") {
        // to support CommonJS
        factory(exports);

    }
    // browser global variable
    root.CartTotal = factory;

}(this, function (element) {
    "use strict";

    var component = new Component(element);
    component.Name = "CXA/Feature/CartTotal";
    component.InExperienceEditorMode = function () {
    };

    component.OnCartUpdated = function (data) {
        AjaxService.Post("/api/cxa/Cart/GetShoppingCartTotal", {}, function (data, success, sender) {
            if (success && data && data.Success) {
                component.model.updateModel(data);
            }
        });
    };

    component.StartListening = function () {
        CartContext.SubscribeHandler(CartContext.CartEvents.CartUpdate, component.OnCartUpdated)
    };

    component.Init = function () {
        component.StartListening();
        AjaxService.Post("/api/cxa/Cart/GetShoppingCartTotal", {}, function (data, success, sender) {
            if (success && data && data.Success) {
                component.model = new CartTotalViewModel(data);
                ko.applyBindings(component.model, component.RootElement);
            }
        });
    };

    return component;

}));
