//-----------------------------------------------------------------------
// <copyright file="cxa.common.productprice.js" company="Sitecore Corporation">
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
        define('CXA/Common/ProductPrice', ['exports'], factory);

    } else if (typeof exports === 'object') {
        // to support CommonJS
        factory(exports);
    }

    function createContext() {
        var context = {
            Handlers: {}, //repsoitory to store handlers
            LastHandlerId: -1, //last used handler Id, initialized to -1
            Price: "",
            SavingsMessage: ""
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
    var ProductPriceContext = getContext();
    root.ProductPriceContext = ProductPriceContext;
    factory(ProductPriceContext);

}(this, function (ProductPriceContext) {
    'use strict';

    function hasHandlers() {
        var handlers = ProductPriceContext.Handlers;
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

    function dispatch(source, catalogName, productId, variantId, data) {
        var handlers = ProductPriceContext.Handlers;

        var handlerId;
        for (handlerId in handlers) {
            if (handlers.hasOwnProperty(handlerId)) {
                notifyHandler(handlers[handlerId], source, catalogName, productId, variantId, data);
            }
        }
    }

    //-------------------- Service Public Functions ---------------------------------------------------------------------

    ProductPriceContext.SetPrice = function (source, priceBefore, price, isOnSale, savingsMessage, data) {
        if (!hasHandlers()) { return false; }
        var dispatchFunction = dispatch(source, priceBefore, price, isOnSale, savingsMessage, data)
        //run asyncronously
        setTimeout(dispatchFunction, 0);
        return true;
    };

    ProductPriceContext.SubscribeHandler = function (handler) {
        if (typeof handler !== 'function') {
            return false;
        }

        var newHandlerId = 'h_' + String(++ProductPriceContext.LastHandlerId);
        ProductPriceContext.Handlers[newHandlerId] = handler;

        return newHandlerId;
    };

    ProductPriceContext.UnSubscribeHandler = function (handlerId) {
        if (ProductPriceContext.Handlers[handlerId]) {
            delete ProductPriceContext.Handlers[handlerId];
            return handlerId;
        }

        return null;
    };
}));