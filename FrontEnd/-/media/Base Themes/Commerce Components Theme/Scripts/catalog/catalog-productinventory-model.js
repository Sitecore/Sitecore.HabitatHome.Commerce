// -----------------------------------------------------------------------
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

// ----- Stock Information View Model -----
function StockInfoViewModel(info) {
  var self = this;

  self.productId = info ? ko.observable(info.ProductId) : ko.observable();
  self.variantId = info ? ko.observable(info.VariantId) : ko.observable();
  self.status = info ? ko.observable(info.Status) : ko.observable();
  self.count = info ? ko.observable(info.Count) : ko.observable();
  self.availabilityDate = info ? ko.observable(info.AvailabilityDate) : ko.observable();
  self.showSingleLabel = info ? ko.observable(info.Count === 1) : ko.observable(false);
  self.isOutOfStock = info ? ko.observable(info.Status && info.Status.toUpperCase() === "OUT-OF-STOCK") : ko.observable(false);
  self.canShowSignupForNotification = info ? ko.observable(info.CanShowSignupForNotification) : ko.observable(false);

  self.showSignUpForNotification = self.canShowSignupForNotification() && self.isOutOfStock();
}
function StockInfoListViewModel() {
  var self = this;

  self.stockInfos = ko.observableArray();
  self.statuses = ko.observableArray();
  self.hasInfo = ko.observable(false);
  self.selectedStockInfo = ko.observable(new StockInfoViewModel());
  self.selectedBundle = null;
  self.load = function () {

    if (CXAApplication.RunningMode === RunningModes.ExperienceEditor) {
      return;
    }

    var data = "{\"productId\":\"" + $('#product-id').val() + "\"}";

    AjaxService.Post("/api/cxa/catalog/GetCurrentProductStockInfo", JSON.parse(data), function (data, success, sender) {
      if (success && data && data.Success) {
        $.each(data.StockInformationList, function () {
          self.stockInfos.push(new StockInfoViewModel(this));
        });

        if (self.selectedBundle) {
          self.switchBundleStatus(self.selectedBundle);
        } else {

          self.selectedStockInfo(new StockInfoViewModel(data.StockInformationList[0]));
          self.statuses(data.Statuses);
          self.hasInfo(data.StockInformationList.length > 0);

          if (self.selectedStockInfo().isOutOfStock()) {
            ProductSelectionContext.SelectedProductInvalid(this);
          }
        }
      } else if (data && !data.Success) {
        if (CXAApplication.RunningMode === RunningModes.ExperienceEditor) {
          return;
        }
        $(data.Errors).each(function () {
          MessageContext.PublishError("productinformation", this);
        });
      }
    });
  };

  self.switchInfo = function (catalogName, productId, variantId) {
    variantId = variantId === null ? "" : variantId;
    var item = self.findProduct(productId, variantId);

    if (item === null) {
      if (self.stockInfos().length > 0) {
        self.selectedStockInfo(self.stockInfos()[0]);
      }
    } else {
      self.selectedStockInfo(item);
    }

    if (self.selectedStockInfo().length > 0 && self.selectedStockInfo().isOutOfStock()) {
      ProductSelectionContext.SelectedProductInvalid(this);
    } else {
      ProductSelectionContext.SelectedProductValid(this);
    }
  };

  self.switchBundleStatus = function (selectedBundle) {

    var viewModel = this;

    viewModel.selectedBundle = selectedBundle;

    if (viewModel.stockInfos().length === 0) {
      return;
    }

    var selectedBundledItemList = [];
    $.each(selectedBundle.bundledItemList, function () {
      var bundledItem = this;

      var item = viewModel.findProduct(bundledItem.productId, bundledItem.variantId);
      if (item) {
        selectedBundledItemList.push(item);
      }
    });

    if (selectedBundledItemList.length > 0) {

      var outOfStockItem = ko.utils.arrayFirst(selectedBundledItemList, function (si) {
        if (si.isOutOfStock()) {
          return si;
        }

        return null;

      });

      if (outOfStockItem) {
        self.selectedStockInfo(outOfStockItem);
        ProductSelectionContext.SelectedProductInvalid(this);
      } else {
        selectedBundledItemList.sort(function (a, b) { return a.count() - b.count(); });
        self.selectedStockInfo(selectedBundledItemList[0]);
      }
    }
  };

  self.findProduct = function (productId, variantId) {
    variantId = variantId === null ? "" : variantId;
    return ko.utils.arrayFirst(this.stockInfos(), function (si) {
      if (si.productId() === productId && si.variantId() === variantId) {
        return si;
      }

      return null;
    });
  };
}