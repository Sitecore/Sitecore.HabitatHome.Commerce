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
    self.relatedProducts = ko.observableArray();
    self.useLazyLoading = ko.observable(false);
    self.maxPageSize = ko.observable();
    self.pageNumber = ko.observable(0);
    self.currentItemId = ko.observable();
    self.currentCatalogItemId = ko.observable();
    self.productListsRawValue = ko.observable();
    self.relationshipTitles = ko.observable();

    self.canLoadMorePromotedProducts = ko.observable(true);
    self.canLoadMoreRelatedProducts = ko.observable(true);

    self.loadPromotedProducts = function () {
        var params = self.loadingParameters();

        AjaxService.Post("/api/cxa/Catalog/GetPromotedProducts", params, function (data, success, sender) {
            if (success && data && data.Success) {
                addPromotedProducts(data);
                addRelatedProducts(data);
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

        // Related products  relationship titles rendering parameter value
        params.rt = self.relationshipTitles();

        return params;
    }

    function addPromotedProducts(data) {
        $(data.PromotedProducts).each(function () {
            self.promotedProducts.push(this);
        });
        self.promotedProductsTitle(data.PromotedProductsTitle);
        self.canLoadMorePromotedProducts(data.PromotedProducts && (data.PromotedProducts.length >= (self.maxPageSize() || 4)));
    };

    function addRelatedProducts(data) {
        $(data.RelatedProducts).each(function () {
            self.relatedProducts.push(this);
        });
        self.canLoadMoreRelatedProducts(data.RelatedProducts && (data.RelatedProducts.length >= (self.maxPageSize() || 12)));
    };

    function getPageNumber() {
        var pageNumber = self.pageNumber();
        // Increment the page number so that the next call for loadMoreProducts will load the next page
        self.pageNumber(self.pageNumber() +1 );
        return pageNumber;
    }

}