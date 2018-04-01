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

function PromotedProductsViewModel() {
    var self = this;
    self.promotedProducts = ko.observableArray();
    self.promotedProductsTitle = ko.observable();
    self.productsList = ko.observableArray();
    self.listTitle = ko.observable();
    self.useLazyLoading = ko.observable(false);
    self.maxPageSize = ko.observable();
    self.pageNumber = ko.observable(0);
    self.currentItemId = ko.observable();
    self.currentCatalogItemId = ko.observable();
    self.productListsRawValue = ko.observable();
    self.relationshipId = ko.observable();
    self.canLoadMoreProducts = ko.observable(false);

    self.loadProducts = function () {
        var params = self.loadingParameters();

        AjaxService.Post("/api/cxa/Catalog/GetPromotedProducts", params, function (data, success, sender) {
            if (success && data && data.Success) {
                addProductList(data);
            }
        });
    }
    self.loadingParameters = function () {

        var params = {};

        // Page number
        params.pg = getPageNumber();

        // Page size
        params.ps = self.maxPageSize() || 4;

        // Current Item Id
        params.ci = self.currentItemId();

        // Current catalog Item Id
        params.cci = self.currentCatalogItemId();

        // Promoted productLists rendering parameter value
        params.plrv = self.productListsRawValue();

        // Relationship type field Id rendering parameter value
        params.rt = self.relationshipId();

        return params;
    }

    function addProductList(data) {
        $(data.ProductsList).each(function () {
            self.productsList.push(this);
        });

        self.listTitle(data.ListTitle);
        self.canLoadMoreProducts(data.ProductsList && (data.ProductsList.length >= (self.maxPageSize() || 4)));
    };

    function getPageNumber() {
        var pageNumber = self.pageNumber();
        // Increment the page number so that the next call for loadMoreProducts will load the next page
        self.pageNumber(self.pageNumber() + 1);
        return pageNumber;
    }

}