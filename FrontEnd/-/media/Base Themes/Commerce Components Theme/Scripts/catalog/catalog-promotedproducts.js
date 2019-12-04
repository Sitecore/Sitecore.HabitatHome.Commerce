// Copyright 2017-2018 Sitecore Corporation A/S
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
    // use AMD define funtion to support AMD modules if in use
    define('CXA/Feature/PromotedProducts', ['exports'], factory);
  } else if (typeof exports === 'object') {
    // to support CommonJS
    factory(exports);
  }
  // browser global variable
  root.PromotedProducts = factory;
  root.PromotedProducts_ComponentClass = "cxa-promoted-products-component";
}(this,
  function (element) {
    var component = new Component(element);
    var productListsRawValue = $(component.RootElement).find("[name = productListsRawValue]")
      .val();
    var relationshipId = $(component.RootElement).find("[name = relationshipId]")
      .val();
    var currentItemId = $(component.RootElement).find("[name = currentItemId]")
      .val();
    var currentCatalogItemId = $(component.RootElement).find("[name = currentCatalogItemId]")
      .val();
    var maxPageSize = $(component.RootElement).find("[name = maxPageSize]")
      .val();
    var useLazyLoading = $(component.RootElement).find("[name = useLazyLoading]")
      .val();

    component.model = new PromotedProductsViewModel();
    component.model.productListsRawValue(productListsRawValue);
    component.model.relationshipId(relationshipId);
    component.model.currentItemId(currentItemId);
    component.model.currentCatalogItemId(currentCatalogItemId);
    component.model.maxPageSize(maxPageSize);
    component.model.useLazyLoading(useLazyLoading);
    component.Name = "CXA/Feature/PromotedProducts";

    component.InExperienceEditorMode = function () {
    };

    component.Init = function () {
      component.model.loadProducts();
      ko.applyBindings(component.model, component.RootElement);
    };
    return component;
  }));

function setEqualHeight(columns) {
  var tallestcolumn = 0;
  columns.each(function () {
    currentHeight = $(this).height();
    if (currentHeight > tallestcolumn) {
      tallestcolumn = currentHeight;
    }
  });
  columns.height(tallestcolumn);
}

$(window).on("load", function () {
  setEqualHeight($(".product-list div.col-sm-4"));
});