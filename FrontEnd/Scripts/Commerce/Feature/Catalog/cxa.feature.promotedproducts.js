
(function (root, factory) {
    if (typeof define === 'function' && define.amd) {
        // use AMD define funtion to support AMD modules if in use
        define('CXA/Feature/PromotedProducts', ['exports'], factory);

    } else if (typeof exports === 'object') {
        // to support CommonJS
        factory(exports);
    }
    // browser global variable
    root.PromotedProducts = factory;
    root.PromotedProducts_ComponentClass = "cxa-promoted-products-component";

}(this,
    function (element) {
        var component = new Component(element);
        var productListsRawValue = $(component.RootElement).find("[name = productListsRawValue]").val();
        var relationshipTitles = $(component.RootElement).find("[name = relationshipTitles]").val();
        var currentItemId = $(component.RootElement).find("[name = currentItemId]").val();
        var currentCatalogItemId = $(component.RootElement).find("[name = currentCatalogItemId]").val();
        var maxPageSize = $(component.RootElement).find("[name = maxPageSize]").val();
        var useLazyLoading = $(component.RootElement).find("[name = useLazyLoading]").val();

        component.model = new PromotedProductsViewModel();
        component.model.productListsRawValue(productListsRawValue);
        component.model.relationshipTitles(relationshipTitles);
        component.model.currentItemId(currentItemId);
        component.model.currentCatalogItemId(currentCatalogItemId);
        component.model.maxPageSize(maxPageSize);
        component.model.useLazyLoading(useLazyLoading);
        component.Name = "CXA/Feature/PromotedProducts";

        component.InExperienceEditorMode = function() {
        }
        component.Init = function() {
            component.model.loadPromotedProducts();
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

