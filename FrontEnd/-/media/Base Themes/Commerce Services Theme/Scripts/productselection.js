//-----------------------------------------------------------------------
// <copyright file="cxa.common.productselection.js" company="Sitecore Corporation">
// Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>Provides a common tool</summary>
//-----------------------------------------------------------------------
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
    var instance;

    if (typeof define === 'function' && define.amd) {
        // use AMD define funtion to support AMD modules if in use
        define('CXA/Common/ProductSelection', ['exports'], factory);

    } else if (typeof exports === 'object') {
        // to support CommonJS
        factory(exports);
    }

    function createContext() {
        var context = {
            Handlers: {}, //repsoitory to store handlers
            LastHandlerId: -1, //last used handler Id, initialized to -1
            CurrentCatalogName: "",
            CurrentProductId: "",
            CurrentVariantId: ""
        };
        return context;
    }

    function getContext() {
        if (!instance) {
            instance = createContext();
        }
        return instance;
    }
    // browser global variable
    var ProductSelectionContext = getContext();
    root.ProductSelectionContext = ProductSelectionContext;
    factory(ProductSelectionContext);

}(this, function (ProductSelectionContext) {
    'use strict';

    function hasHandlers(eventtype) {
        var handlers = ProductSelectionContext.Handlers[eventtype];
        var handlerId;
        for (handlerId in handlers) {
            if (handlers.hasOwnProperty(handlerId)) {
                return true;
            }
        }
    }

    function notifyHandler(handler, source, catalogName, productId, variantId, data) {
        handler(source, catalogName, productId, variantId, data);
    }

    function notifyHadlerSelectedBundleProduct(handler, source, bundleSelection, data) {
        handler(source, bundleSelection, data);
    }

    function dispatch(eventtype, source, catalogName, productId, variantId, data) {
        var handlers = ProductSelectionContext.Handlers[eventtype];

        var handlerId;
        for (handlerId in handlers) {
            if (handlers.hasOwnProperty(handlerId)) {
                notifyHandler(handlers[handlerId], source, catalogName, productId, variantId, data);
            }
        }
    }

    function dispatchSelectedBundleProduct(eventtype, source, bundleSelection, data) {
        var handlers = ProductSelectionContext.Handlers[eventtype];

        var handlerId;
        for (handlerId in handlers) {
            if (handlers.hasOwnProperty(handlerId)) {
                notifyHadlerSelectedBundleProduct(handlers[handlerId], source, bundleSelection, data);
            }
        }
    }

    //-------------------- Service Public Functions ---------------------------------------------------------------------

    ProductSelectionContext.Events = {
        SelectedProduct: "selectedProduct",
        SelectedBundleProduct: "selectedBundleProduct",
        SelectedProductValid: "selectedProductValid",
        SelectedProductInvalid: "selectedProductInvalid"
    };

    ProductSelectionContext.SelectedProduct = function (source, catalogName, productId, variantId, data) {
        if (!hasHandlers(ProductSelectionContext.Events.SelectedProduct)) { return false; }
        var dispatchFunction = dispatch(ProductSelectionContext.Events.SelectedProduct, source, catalogName, productId, variantId, data)
        //run asyncronously
        setTimeout(dispatchFunction, 0);
        return true;
    };

    ProductSelectionContext.SelectedBundleProduct = function (source, bundleSelection, data) {
        if (!hasHandlers(ProductSelectionContext.Events.SelectedBundleProduct)) { return false; }
        var dispatchFunction = dispatchSelectedBundleProduct(ProductSelectionContext.Events.SelectedBundleProduct, source, bundleSelection, data)
        //run asyncronously
        setTimeout(dispatchFunction, 0);
        return true;
    };

    ProductSelectionContext.SelectedProductValid = function (source, data) {
        if (!hasHandlers(ProductSelectionContext.Events.SelectedProductValid)) { return false; }
        var dispatchFunction = dispatch(ProductSelectionContext.Events.SelectedProductValid, source, data);
        //run asyncronously
        setTimeout(dispatchFunction, 0);
        return true;
    };

    ProductSelectionContext.SelectedProductInvalid = function (source, data) {
        if (!hasHandlers(ProductSelectionContext.Events.SelectedProductInvalid)) { return false; }
        var dispatchFunction = dispatch(ProductSelectionContext.Events.SelectedProductInvalid, source, data);
        //run asyncronously
        setTimeout(dispatchFunction, 0);
        return true;
    };

    ProductSelectionContext.SubscribeHandler = function (eventtype, handler) {
        if (typeof handler !== 'function') {
            return false;
        }

        if (!ProductSelectionContext.Handlers.hasOwnProperty(eventtype)) {
            ProductSelectionContext.Handlers[eventtype] = {};
        }

        var newHandlerId = 'h_' + String(++ProductSelectionContext.LastHandlerId);
        ProductSelectionContext.Handlers[eventtype][newHandlerId] = handler;

        return newHandlerId;
    };

    ProductSelectionContext.UnSubscribeHandler = function (handlerId) {
        if (ProductSelectionContext.Handlers[handlerId]) {
            delete ProductSelectionContext.Handlers[handlerId];
            return handlerId;
        }

        return null;
    };
}));

