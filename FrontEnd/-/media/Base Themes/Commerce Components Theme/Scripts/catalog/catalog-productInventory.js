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
    root.ProductInventory = factory;

}(this, function (element) {
    var component = new Component(element);
    component.Name = "CXA/Feature/ProductInventory";
    component.stockInfoVM = null;

    component.InExperienceEditorMode = function () {
        component.Visual.Disable();
    }

    component.ProductSelectionChangedHandler = function (source, catalogName, productId, variantId, data) {
        component.stockInfoVM.switchInfo(catalogName, productId, variantId);
    }

    component.ProductBundleSelectionChangedHandler = function (source, bundleSelection, data) {
        component.stockInfoVM.switchBundleStatus(bundleSelection);
    }

    component.StartListening = function () {
        component.HandlerId = ProductSelectionContext.SubscribeHandler(ProductSelectionContext.Events.SelectedProduct, component.ProductSelectionChangedHandler);
        component.SelectedBundleProductHandlerId = ProductSelectionContext.SubscribeHandler(ProductSelectionContext.Events.SelectedBundleProduct, component.ProductBundleSelectionChangedHandler);
    }
    component.StopListening = function () {
        if (component.HandlerId) {
            ProductSelectionContext.UnSubscribeHandler(component.HandlerId);
        }

        if (component.SelectedBundleProductHandlerId) {
            ProductSelectionContext.UnSubscribeHandler(component.SelectedBundleProductHandlerId);
        }
    }

    component.Init = function () {
        var stockInfo = $(component.RootElement).find('.stock-info');
        if (stockInfo.length > 0) {
            component.stockInfoVM = new StockInfoListViewModel();
            component.stockInfoVM.load();
            ko.applyBindingsWithValidation(component.stockInfoVM, $(component.RootElement).find('.stock-info')[0]);
        }

        if (CXAApplication.RunningMode == RunningModes.ExperienceEditor) {
            var data = { "Statuses": ["In-Stock"] };
            component.stockInfoVM.statuses(data);

            data.Success = "True";
            data.StockInformationList = { "ProductId": "1234", "VariantId": "1", "Count": 74, "Status": "In-Stock" };
            component.stockInfoVM.stockInfos.push(new StockInfoViewModel(data.StockInformationList));
            component.stockInfoVM.selectedStockInfo(new StockInfoViewModel(data.StockInformationList));
            component.stockInfoVM.hasInfo(true);
        }

        component.StartListening();
    }

    return component;
}));
