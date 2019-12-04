// -----------------------------------------------------------------------
// <copyright file="cxa.common.productselection.js" company="Sitecore Corporation">
// Copyright (c) Sitecore Corporation 1999-2018
// </copyright>
// <summary>Provides a common tool</summary>
// -----------------------------------------------------------------------
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
      Handlers: {}, // repository to store handlers
      LastHandlerId: -1, // last used handler Id, initialized to -1
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

  function hasHandlers(eventType) {
    var handlers = ProductSelectionContext.Handlers[eventType];
    var handlerId;
    for (handlerId in handlers) {
      if (handlers.hasOwnProperty(handlerId)) {
        return true;
      }
    }
    return false;
  }

  function notifyHandler(handler, source) {
    handler(source);
  }

  function notifyHandlerSelectedBundleProduct(handler, source, bundleSelection) {
    handler(source, bundleSelection);
  }

  function dispatch(eventType, source) {
    var handlers = ProductSelectionContext.Handlers[eventType];

    var handlerId;
    for (handlerId in handlers) {
      if (handlers.hasOwnProperty(handlerId)) {
        notifyHandler(handlers[handlerId], source);
      }
    }
  }

  function dispatchSelectedBundleProduct(eventType, source, bundleSelection) {
    var handlers = ProductSelectionContext.Handlers[eventType];

    var handlerId;
    for (handlerId in handlers) {
      if (handlers.hasOwnProperty(handlerId)) {
        notifyHandlerSelectedBundleProduct(handlers[handlerId], source, bundleSelection);
      }
    }
  }

  // -------------------- Service Public Functions ---------------------------------------------------------------------

  ProductSelectionContext.Events = {
    SelectedProduct: "selectedProduct",
    SelectedBundleProduct: "selectedBundleProduct",
    SelectedProductValid: "selectedProductValid",
    SelectedProductInvalid: "selectedProductInvalid"
  };

  ProductSelectionContext.SelectedProduct = function (source) {
    if (!hasHandlers(ProductSelectionContext.Events.SelectedProduct)) { return false; }
    var dispatchFunction = dispatch(ProductSelectionContext.Events.SelectedProduct, source);
    // run asyncronously
    setTimeout(dispatchFunction, 0);
    return true;
  };

  ProductSelectionContext.SelectedBundleProduct = function (source, bundleSelection) {
    if (!hasHandlers(ProductSelectionContext.Events.SelectedBundleProduct)) { return false; }
    var dispatchFunction = dispatchSelectedBundleProduct(ProductSelectionContext.Events.SelectedBundleProduct, source, bundleSelection);
    // run asyncronously
    setTimeout(dispatchFunction, 0);
    return true;
  };

  ProductSelectionContext.SelectedProductValid = function (source) {
    if (!hasHandlers(ProductSelectionContext.Events.SelectedProductValid)) { return false; }
    var dispatchFunction = dispatch(ProductSelectionContext.Events.SelectedProductValid, source);
    // run asyncronously
    setTimeout(dispatchFunction, 0);
    return true;
  };

  ProductSelectionContext.SelectedProductInvalid = function (source) {
    if (!hasHandlers(ProductSelectionContext.Events.SelectedProductInvalid)) { return false; }
    var dispatchFunction = dispatch(ProductSelectionContext.Events.SelectedProductInvalid, source);
    // run asyncronously
    setTimeout(dispatchFunction, 0);
    return true;
  };

  ProductSelectionContext.SubscribeHandler = function (eventType, handler) {
    if (typeof handler !== 'function') {
      return false;
    }

    if (!ProductSelectionContext.Handlers.hasOwnProperty(eventType)) {
      ProductSelectionContext.Handlers[eventType] = {};
    }

    var newHandlerId = 'h_' + String(++ProductSelectionContext.LastHandlerId);
    ProductSelectionContext.Handlers[eventType][newHandlerId] = handler;

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