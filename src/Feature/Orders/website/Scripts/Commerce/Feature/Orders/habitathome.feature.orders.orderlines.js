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
        define("CXA/Feature/OrderLines", ["exports"], factory);

    } else if (typeof exports === "object") {
        // to support CommonJS
        factory(exports);

    }
    // browser global variable
    root.OrderLines = factory;

}(this, function (element) {
    "use strict";

    var component = new Component(element);
    component.Name = "CXA/Feature/OrderLines";
    component.InExperienceEditorMode = function () {
        component.Visual.Disable();
        component.Model = new OrderLinesViewModel({});
        ko.applyBindings(component.Model, component.RootElement);

    };

    component.OnCartUpdated = function (data) {
        $(component.RootElement).find(".initial-label").show();
        $(component.RootElement).find(".loading-label").hide();
    };

    component.StartListening = function () {
        CartContext.SubscribeHandler(CartContext.CartEvents.CartUpdate, component.OnCartUpdated)
    };

    component.Init = function () {

        if (CXAApplication.RunningMode === RunningModes.Normal) {
            component.StartListening();

            component.model = new OrderLinesViewModel();
            ko.applyBindings(component.model, component.RootElement);

        }
    };

    return component;
}));