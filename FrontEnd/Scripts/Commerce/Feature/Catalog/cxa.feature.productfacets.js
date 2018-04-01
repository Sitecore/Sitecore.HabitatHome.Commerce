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
    if (typeof define === 'function' && define.amd) {
        // use AMD define funtion to support AMD modules if in use
        define(['exports'], factory);

    } else if (typeof exports === 'object') {
        // to support CommonJS
        factory(exports);
    }
    var CatalogFacetComponent = {}
    root.CatalogFacetComponent = CatalogFacetComponent;

    factory(CatalogFacetComponent);

}(this, function (catalogFacetComponent) {
    var FireFacetApplied = function(facetName, facetValue, isApplied, url) {
        isApplied = (isApplied == 1 || isApplied) ? true : false;

        var facet = facetName + "=" + facetValue;

        apiUrl = "/api/cxa/catalog/facetapplied";
        data = { "facetValue": facet, "isApplied": isApplied };

        AjaxService.Post(apiUrl, data, function (data, success, sender) {
            window.location.href = url;
        });
    };

    catalogFacetComponent.Init = function ($element) {
        $element = $($element);

        $element.find(".product-facets ul li a").click(function (index) {
            $onclick = $(this).data("onclick");

            if ($onclick) {
                eval($onclick);
            }
        });
    }
}));
