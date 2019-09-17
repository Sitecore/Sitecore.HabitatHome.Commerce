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

(function (root, factory) {
  if (typeof define === "function" && define.amd) {
    // use AMD define funtion to support AMD modules if in use
    define("CXA/Feature/CartPromotion", ["exports"], factory);
  } else if (typeof exports === "object") {
    // to support CommonJS
    factory(exports);
  }
  // browser global variable
  root.CartPromotion = factory;
}(this, function (element) {
  "use strict";

  var component = new Component(element);
  component.Name = "CXA/Feature/CartPromotion";
  component.InExperienceEditorMode = function () {
  };

  component.OnCartUpdated = function (data) {
    $(component.RootElement).find(".glyphicon")
      .removeClass("glyphicon-refresh");
    $(component.RootElement).find(".glyphicon")
      .removeClass("glyphicon-refresh-animate");

    $(component.RootElement).find(".glyphicon")
      .removeClass("glyphicon-remove-circle");
    $(component.RootElement).find(".glyphicon")
      .addClass("glyphicon-remove-circle");

    AjaxService.Post("/api/cxa/Cart/GetPromoCodes", {}, function (data, success, sender) {
      if (success && data && data.Success) {
        component.model.updateModel(data);

        let cartLinesCount = data.Lines ? data.Lines.length : 0;
        CartContext.UpdateCachedCartCount(cartLinesCount);
      }
    });
  };

  component.StartListening = function () {
    CartContext.SubscribeHandler(CartContext.CartEvents.CartUpdate, component.OnCartUpdated);
  };

  component.Init = function () {
    component.StartListening();
    AjaxService.Post("/api/cxa/Cart/GetPromoCodes", {}, function (data, success, sender) {
      if (success && data && data.Success) {
        component.model = new CartPromotionViewModel(data);
        ko.applyBindings(component.model, component.RootElement);

        let cartLinesCount = data.Lines ? data.Lines.length : 0;
        CartContext.UpdateCachedCartCount(cartLinesCount);
      }
    });
  };

  return component;
}));