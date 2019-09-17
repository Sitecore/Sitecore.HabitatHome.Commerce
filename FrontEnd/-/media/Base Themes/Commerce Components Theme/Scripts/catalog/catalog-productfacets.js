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
  if (typeof define === 'function' && define.amd) {
    // use AMD define funtion to support AMD modules if in use
    define(['exports'], factory);
  } else if (typeof exports === 'object') {
    // to support CommonJS
    factory(exports);
  }
  var CatalogFacetComponent = {};
  root.CatalogFacetComponent = CatalogFacetComponent;

  factory(CatalogFacetComponent);
})(this, function (catalogFacetComponent) {
  var toggleFacet = function (facetName, facetValue, isApplied, url) {
    var data = {
      facetValue: facetName + '=' + facetValue,
      isApplied: isApplied
    };

    AjaxService.Post('/api/cxa/catalog/facetapplied', data, function () {
      // reload page after toggling facets
      window.location.href = url;
    });
  };

  catalogFacetComponent.Init = function (element) {
    $(element)
      .find('.product-facets ul li a')
      .click(function () {
        var $facet = $(this);
        var facetName = $facet.data('facet-name');
        var facetValue = $facet.data('facet-value');
        var facetIsActive = $facet.data('facet-is-active');
        var facetUrl = $facet.data('facet-url');

        toggleFacet(facetName, facetValue, facetIsActive, facetUrl);
      });

    $(element)
      .find(".facet-title")
      .click(function () {
        var $obj = $(this);
        var clicks = $obj.data('clicks');
        var $list = $obj.next().filter("div[class$=-list]");
        if (!clicks) {
          $list.fadeTo("fast", 0).hide();
          $obj.data('clicks', 1);
        } else {
          $list.show().fadeTo("fast", 1);
          $obj.removeData();
        }
      });
  };
});
