
(function (root, factory) {
    if (typeof define === 'function' && define.amd) {
        // use AMD define funtion to support AMD modules if in use
        define('CXA/Feature/PurchasableProductList', ['exports'], factory);

    } else if (typeof exports === 'object') {
        // to support CommonJS
        factory(exports);
    }
    // browser global variable
    root.PurchasableProductList = factory;
    root.PurchasableProductList_ComponentClass = "cxa-productlist-component";

}(this,
    function (element) {
        var component = new Component(element);
        var currentCatalogItemId = $(component.RootElement).find("[name = currentCatalogItemId]").val();
        var currentItemId = $(component.RootElement).find("[name = currentItemId]").val();
        var maxPageSize = $(component.RootElement).find("[name = maxPageSize]").val();
        var useLazyLoading = $(component.RootElement).find("[name = useLazyLoading]").val();

        component.model = new PurchasableProductListViewModel();
        component.model.currentCatalogItemId(currentCatalogItemId);
        component.model.currentItemId(currentItemId);
        component.model.maxPageSize(maxPageSize);
        component.model.useLazyLoading(useLazyLoading);
        component.Name = "CXA/Feature/PurchasableProductList";

        component.InExperienceEditorMode = function () {
        }

        component.OnCartUpdated = function (data) {
            $(component.RootElement).find(".initial-label").show();
            $(component.RootElement).find(".loading-label").hide();
        };

        component.StartListening = function () {
            CartContext.SubscribeHandler(CartContext.CartEvents.CartUpdate, component.OnCartUpdated)
        };

        component.Init = function () {

            if (CXAApplication.RunningMode === RunningModes.Normal) {
                component.StartListening();
            }

            component.model.loadProducts();
            ko.applyBindings(component.model, component.RootElement);
        };
        return component;
    }));

function setEqualHeight(columns) {
    var tallestcolumn = 0;
    columns.each(function () {
        currentHeight = $(this).height();
        if (currentHeight > tallestcolumn) {
            tallestcolumn = currentHeight;
        }
    });
    columns.height(tallestcolumn);
}

$(window).on("load", function () {
    setEqualHeight($(".product-list div.col-sm-4"));
});

