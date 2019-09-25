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
  'use strict';
  if (typeof define === 'function' && define.amd) {
    // use AMD define funtion to support AMD modules if in use
    define('CXA/Feature/ProductSelection', ['exports'], factory);

  } else if (typeof exports === 'object') {
    // to support CommonJS
    factory(exports);

  }
  // browser global variable
  root.ProductSelection = factory;
  root.ProductSelection_ComponentClass = "cxa-addtocart-component";

}(this, function (element) {
  'use strict';

  var component = new Component(element);
  component.Name = "CXA/Feature/ProductSelection";

  component.bundleProductPayload = "";

  // ProductSelectionContext Handlers
  component.ProductSelectionChangedHandler = function (source) {
    $(component.RootElement).find('#addtocart_catalogname')
      .val(ProductSelectionContext.CurrentCatalogName);
    var currentProductId = $(component.RootElement).find('#addtocart_productid')
      .val();

    if (currentProductId === ProductSelectionContext.CurrentProductId) {
      $(component.RootElement).find('#addtocart_variantid')
        .val(ProductSelectionContext.CurrentVariantId ? ProductSelectionContext.CurrentVariantId : "");
    }
  };

  component.ProductBundleSelectionChangedHandler = function (source, bundleSelection) {
    component.bundleProductPayload = bundleSelection;
  };

  component.SelectedProductInvalid = function (source, data) {
    if (!$(component.RootElement).find('.add-to-cart-btn')[0].hasAttribute('disabled')) {
      $(component.RootElement).find('.add-to-cart-btn')
        .attr('disabled', 'disabled');
    }
  };

  component.SelectedProductValid = function (source, data) {
    $(component.RootElement).find('.add-to-cart-btn')
      .removeAttr('disabled');
  };

  component.StartListening = function () {
    component.SelectionChangeHandlerId = ProductSelectionContext.SubscribeHandler(ProductSelectionContext.Events.SelectedProduct, component.ProductSelectionChangedHandler);
    component.SelectedProductValidHandlerId = ProductSelectionContext.SubscribeHandler(ProductSelectionContext.Events.SelectedProductValid, component.SelectedProductValid);
    component.SelectedProductInvalidHandlerId = ProductSelectionContext.SubscribeHandler(ProductSelectionContext.Events.SelectedProductInvalid, component.SelectedProductInvalid);
    component.SelectedBundleProductHandlerId = ProductSelectionContext.SubscribeHandler(ProductSelectionContext.Events.SelectedBundleProduct, component.ProductBundleSelectionChangedHandler);
  };
  component.StopListening = function () {
    if (component.SelectionChangeHandlerId) {
      ProductSelectionContext.UnSubscribeHandler(component.SelectionChangeHandlerId);
    }

    if (component.SelectedProductValidHandlerId) {
      ProductSelectionContext.UnSubscribeHandler(component.SelectedProductValidHandlerId);
    }

    if (component.SelectedProductInvalidHandlerId) {
      ProductSelectionContext.UnSubscribeHandler(component.SelectedProductInvalidHandlerId);
    }

    if (component.SelectedBundleProductHandlerId) {
      ProductSelectionContext.UnSubscribeHandler(component.SelectedBundleProductHandlerId);
    }
  };

  {
    $(component.RootElement).find(".add-to-cart-btn")
      .click(function (event) {
        event.preventDefault();

        var action = "";
        var data = "";
        var isBundle = $(component.RootElement).find("#addtocart_isbundle")
          .val();
        var quantity = $(component.RootElement).find('.add-to-cart-qty-input')
          .val();

        if (isBundle === "1") {
          component.bundleProductPayload.quantity = quantity;
          data = component.bundleProductPayload.toJSONString();

          action = "AddBundleCartLine";
        } else {
          action = "AddCartLine";

          var catalogName = $(component.RootElement).find('#addtocart_catalogname')
            .val();
          var productId = $(component.RootElement).find('#addtocart_productid')
            .val();
          var variantId = $(component.RootElement).find('#addtocart_variantid')
            .val();

          data = "{";
          data = data + '"addtocart_catalogname":"' + catalogName + '",';
          data = data + '"addtocart_productid":"' + productId + '",';
          data = data + '"addtocart_variantid":"' + variantId + '",';
          data = data + '"quantity":"' + quantity + '"';
          data += "}";
        }

        if (action.length > 0) {
          MessageContext.ClearAllMessages();
          $(component.RootElement).find(".add-to-cart-btn")
            .button('loading');

          AjaxService.Post("/api/cxa/Cart/" + action, JSON.parse(data), function (data, success, sender) {
            if (data && data.Success && success) {
              AddToCartForm.OnSuccess(data);
            }
            $(component.RootElement).find(".add-to-cart-btn")
              .button('reset');
          }, $(this));
        }

        return false;
      });
  }

  return component;

}));

// Find & initialize all message summary components on the page
$(document).ready(function () {
  $("." + ProductSelection_ComponentClass).each(function () {
    var componet = new ProductSelection(this);
    componet.StartListening();
  });
});

