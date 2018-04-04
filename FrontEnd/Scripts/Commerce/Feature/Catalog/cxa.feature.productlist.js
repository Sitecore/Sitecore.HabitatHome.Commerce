
(function (root, factory) {
    if (typeof define === 'function' && define.amd) {
        // use AMD define funtion to support AMD modules if in use
        define('CXA/Feature/ProductList', ['exports'], factory);

    } else if (typeof exports === 'object') {
        // to support CommonJS
        factory(exports);
    }
    // browser global variable
    root.ProductList = factory;
    root.ProductList_ComponentClass = "cxa-productlist-component";

}(this,
    function (element) {
        var component = new Component(element);
        var currentCatalogItemId = $(component.RootElement).find("[name = currentCatalogItemId]").val();
        var currentItemId = $(component.RootElement).find("[name = currentItemId]").val();
        var maxPageSize = $(component.RootElement).find("[name = maxPageSize]").val();
        var useLazyLoading = $(component.RootElement).find("[name = useLazyLoading]").val();

        component.model = new ProductListViewModel();
        component.model.currentCatalogItemId(currentCatalogItemId);
        component.model.currentItemId(currentItemId);
        component.model.maxPageSize(maxPageSize);
        component.model.useLazyLoading(useLazyLoading);
        component.Name = "CXA/Feature/ProductList";

        component.InExperienceEditorMode = function() {
        }
        component.Init = function() {
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

