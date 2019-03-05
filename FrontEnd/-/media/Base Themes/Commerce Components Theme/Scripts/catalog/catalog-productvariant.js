﻿//function VariantSelectionChanged() {
//    ProductSelectionContext.SelectedProductValid(this, null);

//    MessageContext.ClearAllMessages();

//    if (varianDefinition != null) {
//        var foundVariant = varianDefinition.FindBasedOnCurrentSelection();
//        if (foundVariant != null) {
//            ProductPriceContext.SetPrice(this, foundVariant.listPrice, foundVariant.adjustedPrice, foundVariant.isOnSale, foundVariant.savingsMessage);

//            var catalogName = $('#variant-component-product-catalog').val();
//            var productId = $('#variant-component-product-id').val();

//            ProductSelectionContext.SelectedProduct(this, catalogName, productId, foundVariant.variantId, null);
//        }
//        else {
//            var message = $('.invalid-variant').text();
//            MessageContext.PublishError("productinformation", message);
//            ProductSelectionContext.SelectedProductInvalid(this, null);
//        }
//    }
//}

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
    root.ProductVariants = factory;

}(this, function (element) {
    'use strict';
    var component = new Component(element);
    component.Name = "CXA/Feature/ProductVariants";
    component.model = new ProductVariantViewModel(component.RootElement);

    component.InExperienceEditorMode = function () {
        component.Visual.Disable();
    }

    component.Init = function () {
        if ($(component.RootElement).length) {
            component.model.Initialize();
            ko.applyBindings(component.model, component.RootElement);
        }
    }

    return component;
}));
