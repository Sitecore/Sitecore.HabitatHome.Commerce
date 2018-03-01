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
    root.CheckoutBilling = factory;
    root.CheckoutBilling_ComponentClass = "cxa-checkoutbilling-component";

}(this, function (element) {
    'use strict';
    var component = new Component(element);
    component.Name = "CXA/Feature/CheckoutBilling";
    component.billingVM = null;
    component.Element = element;

    component.InExperienceEditorMode = function () {
        component.Visual.Disable();
    }

    component.StartListening = function () {
    }
    component.StopListening = function () {
    }

    component.Init = function () {
        component.billingVM = new BillingDataViewModel();
        component.billingVM.load();
        ko.applyBindingsWithValidation(component.billingVM, component.Element);

        component.StartListening();
        component.SetupBillingPage();
    }
   
    component.SetupBillingPage = function () {
        $('.apply-credit-card-toggle').on('click', function (event) {
            event.preventDefault();

            // create accordion variables
            var accordion = $(this);
            var accordionContent = $(component.RootElement).find('.credit-card-payment-section');
            //var accordionToggleIcon = $(this).children('.toggle-icon');

            // toggle accordion link open class
            accordion.toggleClass("open");

            // toggle accordion content
            accordionContent.slideToggle(250);

            // change plus/minus icon
            if (accordion.hasClass("open")) {
                //accordionToggleIcon.html("<span class='glyphicon glyphicon-minus-sign'></span>");
                if ($(this).hasClass("ccpayment")) {
                    component.billingVM.creditCardPayment().isAdded(true);
                    if (component.billingVM.paymentClientToken() != null) {
                        component.billingVM.creditCardEnable(true);
                        component.billingVM.billingAddressEnable(true);
                    }
                }                
            } 
        });

        $('.apply-gift-card-toggle').on('click', function (event) {
            event.preventDefault();

            // create accordion variables
            var accordion = $(this);
            var accordionContent = $(component.RootElement).find('.apply-gift-card-section');
            //var accordionToggleIcon = $(this).children('.toggle-icon');

            // toggle accordion link open class
            accordion.toggleClass("open");

            // toggle accordion content
            accordionContent.slideToggle(250);
            
        });
    }
}));

$(document).ready(function () {
    $("." + CheckoutBilling_ComponentClass).each(function () {
        var component = new CheckoutBilling(this);
    });
});
