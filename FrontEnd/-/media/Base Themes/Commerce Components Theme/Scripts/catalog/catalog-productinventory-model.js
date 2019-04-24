// ----- Stock Information View Model -----
function StockInfoViewModel(info) {
    var populate = info != null;
    var self = this;

    self.productId = populate ? ko.observable(info.ProductId) : ko.observable();
    self.variantId = populate ? ko.observable(info.VariantId) : ko.observable();
    self.status = populate ? ko.observable(info.Status) : ko.observable();
    self.count = populate ? ko.observable(info.Count) : ko.observable();
    self.availabilityDate = populate ? ko.observable(info.AvailabilityDate) : ko.observable();
    self.showSingleLabel = populate ? ko.observable(info.Count === 1) : ko.observable(false);
    self.isOutOfStock = populate ? ko.observable(info.Status != null && info.Status.toUpperCase() == "OUT-OF-STOCK") : ko.observable(false);
    self.canShowSignupForNotification = populate ? ko.observable(info.CanShowSignupForNotification) : ko.observable(false);

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

        if (CXAApplication.RunningMode == RunningModes.ExperienceEditor) {
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
                }
                else {

                    self.selectedStockInfo(new StockInfoViewModel(data.StockInformationList[0]));
                    self.statuses(data.Statuses);
                    self.hasInfo(data.StockInformationList.length > 0);

                    if (self.selectedStockInfo().isOutOfStock()) {
                        ProductSelectionContext.SelectedProductInvalid(this, null);
                    }
                }
            }
            else if (data && !data.Success) {
                if (CXAApplication.RunningMode == RunningModes.ExperienceEditor) {
                    return;
                }
                $(data.Errors).each(function () {
                    MessageContext.PublishError("productinformation", this);
                });
            }
        });
    };

    self.switchInfo = function (catalogName, productId, variantId) {
        variantId = variantId == null ? "" : variantId;
        var item = self.findProduct(productId, variantId);

        if (item == null) {
            if (self.stockInfos().length > 0) {
                self.selectedStockInfo(self.stockInfos()[0]);
            }
        } else {
            self.selectedStockInfo(item);
        }

        if (self.selectedStockInfo().length > 0 && self.selectedStockInfo().isOutOfStock()) {
            ProductSelectionContext.SelectedProductInvalid(this, null);
        } else {
            ProductSelectionContext.SelectedProductValid(this, null);
        }
    };

    self.switchBundleStatus = function (selectedBundle) {

        var viewModel = this;

        viewModel.selectedBundle = selectedBundle;

        if (viewModel.stockInfos().length == 0) {
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
                ProductSelectionContext.SelectedProductInvalid(this, null);
            }
            else {
                selectedBundledItemList.sort(function (a, b) { return a.count() - b.count() });
                self.selectedStockInfo(selectedBundledItemList[0]);
            }
        }
    };

    self.findProduct = function (productId, variantId) {
        variantId = variantId == null ? "" : variantId;
        return ko.utils.arrayFirst(this.stockInfos(), function (si) {
            if (si.productId() === productId && si.variantId() === variantId) {
                return si;
            }

            return null;
        });
    };
}