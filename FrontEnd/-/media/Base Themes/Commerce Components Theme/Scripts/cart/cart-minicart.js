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
    if (typeof define === 'function' && define.amd) {
        define('CXA/Feature/Minicart', ['exports'], factory);
    } else if (typeof exports === 'object') {
        factory(exports);
    }
    root.Minicart = factory;
    root.Minicart_ComponentClass = 'cxa-minicart-component';

}(this, function (element) {
    let component = new Component(element);
    component.Name = 'CXA/Feature/Minicart';
    component.model = new MinicartViewModel();

    component.InExperienceEditorMode = function () {
        ko.applyBindings(component.model, component.RootElement);
    };

    component.OnCartUpdated = function (data) {
        const icon = $(component.RootElement).find('.glyphicon');

        $(icon).removeClass('glyphicon-refresh');
        $(icon).removeClass('glyphicon-refresh-animate');
        $(icon).removeClass('glyphicon-remove-circle');
        $(icon).addClass('glyphicon-remove-circle');

        component.model.updateCartCount();
    };

    component.StartListening = function () {
        CartContext.SubscribeHandler(CartContext.CartEvents.CartUpdate, component.OnCartUpdated);
    };

    component.Init = function () {
        if (CXAApplication.RunningMode === RunningModes.Normal) {
            component.StartListening();
            ko.applyBindings(component.model, component.RootElement);
        }
    };

    return component;
}));

//handle taps outside of minicart
$(document).ready(function () {
    var $basket = $("." + Minicart_ComponentClass + " .basket");
    $(document).on("touchend", function (e) {
        $basket.each(function () {
            $this = $(this);
            if (!$this.is(e.target) && $this.has(e.target).length === 0) {
                var model = ko.dataFor(this);
                model.leaveMinicart();
            }
        });

    });
});