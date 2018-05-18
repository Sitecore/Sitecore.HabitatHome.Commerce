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

    //ProductSelectionContext Handlers
    component.ProductSelectionChangedHandler = function (source, catalogName, productId, variantId, data) {
        $(component.RootElement).find('#addtocart_catalogname').val(catalogName);
        $(component.RootElement).find('#addtocart_productid').val(productId);
        if (variantId) {
            $(component.RootElement).find('#addtocart_variantid').val(variantId);
        }
        else {
            $(component.RootElement).find('#addtocart_variantid').val("");
        }
    }

    ProductSelectionContext.SelectedProductValid = function (source, data) {
        $(component.RootElement).find('.add-to-cart-btn').removeAttr('disabled');
    };

    ProductSelectionContext.SelectedProductInvalid = function (source, data) {
        if (!$(component.RootElement).find('.add-to-cart-btn')[0].hasAttribute('disabled')) {
            $(component.RootElement).find('.add-to-cart-btn').attr('disabled', 'disabled');
        }
    };

    component.StartListening = function () {
        component.SelectionChangeHandlerId = ProductSelectionContext.SubscribeHandler(component.ProductSelectionChangedHandler);
        component.SelectedProductValidHandlerId = ProductSelectionContext.SubscribeHandler(component.ProductSelectionChangedHandler);
        component.SelectedProductInvalidHandlerId = ProductSelectionContext.SubscribeHandler(component.ProductSelectionChangedHandler);
    }
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
    }

    return component;

}));

$('form#AddBundleForm').submit(function (e) {
    var variantIds = "";
    $('input:checkbox.product-bundle-item').each(function () {
        if (this.checked) {
            var optSelect = $('.variant-option-select-' + $(this).attr('id'));
            console.log($(this).attr('id'));
            console.log($(optSelect));
            console.log($(optSelect).val());
            var baseProductId = $(this).val();
            if (optSelect.val() != null && optSelect.val() != 'unidentified')
                baseProductId = $(this).val() + "|" + $(optSelect).val();
            variantIds += baseProductId + ',';
        }        
    });
    $('input#addtocart_relatedvariantids').val(variantIds);
    console.log($('input#addtocart_relatedvariantids').val());
})

//Find & initialize all message summary components on the page
$(document).ready(function () {
    $("." + ProductSelection_ComponentClass).each(function () {
        var componet = new ProductSelection(this);
        componet.StartListening();
    });
});

