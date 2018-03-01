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
    root.CheckoutConfirm = factory;
    root.CheckoutConfirm_ComponentClass = "cxa-checkoutconfirm-component";

}(this, function (element) {
    'use strict';
    var component = new Component(element);
    component.Name = "CXA/Feature/CheckoutConfirm";
    component.confirmVM = null;
    component.Element = element;

    component.InExperienceEditorMode = function () {
        component.Visual.Disable();
    }

    component.StartListening = function () {
    }
    component.StopListening = function () {
    }

    component.Init = function () {
        component.confirmVM = new ConfirmDataViewModel();

        if (!CXAApplication.IsExperienceEditorMode()) {
            component.confirmVM.load();
        }

        ko.applyBindingsWithValidation(component.confirmVM, component.Element);

        component.SetupConfirmPage();
    }

    component.SetupConfirmPage = function () {
    }
}));

$(document).ready(function () {
    $("." + CheckoutConfirm_ComponentClass).each(function () {
        var component = new CheckoutConfirm(this);
    });
});
