//-----------------------------------------------------------------------
// <copyright file="cxa.services.dom.observer.js" company="Sitecore Corporation">
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
        define('CXA/Common/DomObserver', ['exports'], factory);

    } else if (typeof exports === 'object') {
        // to support CommonJS
        factory(exports);

    }

    function createDomObserver() {
        var domObserver = {};

        return domObserver;
    }

    function getDomObserver() {
        if (!instance) {
            instance = createDomObserver();
        }
        return instance;
    }
    // browser global variable
    var DomObserver = getDomObserver();
    root.DomObserver = DomObserver;
    factory(DomObserver);

}(this, function (DomObserver) {
    'use strict';

    //https://dom.spec.whatwg.org/#mutation-observers
    //http://stackoverflow.com/questions/2844565/is-there-a-javascript-jquery-dom-change-listener
    //http://stackoverflow.com/questions/3219758/detect-changes-in-the-dom

    //-------------------- Service Public Functions ---------------------------------------------------------------------
    DomObserver.ObserveDomChanges = function (callback, targetNode, options) {

        if (!targetNode) {
            targetNode = document;
        }

        if (!options) {
            options = {
                subtree: true,
                attributes: false,
                childList: true
            };
        }

        var MutationObserver = window.MutationObserver || window.WebKitMutationObserver;

        var observer = new MutationObserver(function (mutations, observer) {
            console.log("SERVICE --> new MutationObserver callback...");

            if (callback) {
                callback();
            }
        });

        // define what element should be observed by the observer
        // and what types of mutations trigger the callback
        observer.observe(targetNode, options);

        return true;
    };
}));