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

function ProductListViewModel() {
  var self = this;
  self.products = ko.observableArray();
  self.currentCatalogItemId = ko.observable();
  self.useLazyLoading = ko.observable(false);
  self.maxPageSize = ko.observable();
  self.currentItemId = ko.observable();
  self.pageNumber = ko.observable(0);
  self.isShowMoreProducts = ko.observable(false);
  self.isPageReady = ko.observable(false);
  self.canLoadMoreProducts = ko.observable(true);

    self.loadProducts = function (component) {
    var params = self.loadingParameters();
    AjaxService.Post('/api/cxa/Catalog/GetProductList', params, function(
      data,
      success,
      sender
    ) {
      if (success && data && data.Success) {
        addMoreProducts(data);
      }
        else if (component.InExperienceEditorMode) {
            $(".product-list").html("<p>Product List</p>");
        }
    });
  };
  self.loadMoreProducts = function() {
    self.isShowMoreProducts(true);
    self.loadProducts();
  };
  self.loadingParameters = function() {
    var params = {};

    // SearchKeyword
    params.q = getQueryStringParamValue('q');

    // Page number
    params.pg = getPageNumber();

    // Facets
    params.f = getQueryStringParamValue('f');

    // Sort
    params.s = getQueryStringParamValue('s');

    // Page size
    params.ps = getQueryStringParamValue('ps') || self.maxPageSize() || 12;

    // Sort direction
    params.sd = getQueryStringParamValue('sd') || 'Asc';

    // Current Catalog Item Id
    params.cci = self.currentCatalogItemId();

    // Current Item Id
    params.ci = self.currentItemId();

    return params;
  };

  function addMoreProducts(data) {
    $(data.ChildProducts).each(function() {
      self.products.push(this);
    });

    var currentPageSize =
      getQueryStringParamValue('ps') || self.maxPageSize() || 12;
    self.canLoadMoreProducts(
      data.ChildProducts && data.ChildProducts.length >= currentPageSize
    );
  }

  function getPageNumber() {
    var pageNumber = 0;

    if (self.isShowMoreProducts()) {
      self.pageNumber(self.pageNumber() + 1);
      pageNumber = self.pageNumber();
      self.isShowMoreProducts(false);
    }
    if (getQueryStringParamValue('pg')) {
      pageNumber = Number(getQueryStringParamValue('pg'));
      pageNumber =
        pageNumber >= self.pageNumber() ? pageNumber : self.pageNumber();
      self.pageNumber(pageNumber);
    }

    return pageNumber;
  }

  function getQueryStringParamValue(queryPam) {
    var url = window.location.href;
    var queryPamValue = new Uri(window.location.href).getQueryParamValue(
      queryPam
    );

    return queryPamValue;
  }
}
