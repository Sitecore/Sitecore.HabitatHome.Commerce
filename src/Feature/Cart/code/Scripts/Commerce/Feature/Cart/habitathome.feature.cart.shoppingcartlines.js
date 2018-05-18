
(function (root, factory) {

    if (typeof define === "function" && define.amd) {
        // use AMD define funtion to support AMD modules if in use
        define("CXA/Feature/CartLines", ["exports"], factory);

    } else if (typeof exports === "object") {
        // to support CommonJS
        factory(exports);

    }
    // browser global variable
    root.CartLines = factory;

}(this, function (element) {
    "use strict";

    var component = new Component(element);
    component.Name = "CXA/Feature/CartLines";
    component.InExperienceEditorMode = function () {
        component.Visual.Disable();
        component.Model = new CartLinesViewModel({});
        ko.applyBindings(component.Model, component.RootElement);

        for (var i = 0; i < 5; i++) {
            component.Model.cart().cartLines.push({
                displayName: "Lorem ipsum dolor sit amet, id dicant",
                colorInformation: "Soleat",
                sizeInformation: "dolor",
                styleInformation: "sit",
                giftCardAmountInformation: "$25",
                productUrl: "javascript:vid(0)",
                discountOfferNames: ["mediocritatem no mei(25%)"],
                quantity: "1",
                linePrice: "0.00 USD",
                lineTotal: "0.00 USD",
                lineItemDiscount: "0.00 USD",
                externalCartLineId: i,
                image: "/sitecore/shell/-/media/Feature/Experience%20Accelerator/Commerce/Catalog/72x72.png?h=72&w=72"
            });
        }

    };

    component.OnCartUpdated = function (data) {
        $(component.RootElement).find(".glyphicon").removeClass("glyphicon-refresh");
        $(component.RootElement).find(".glyphicon").removeClass("glyphicon-refresh-animate");

        $(component.RootElement).find(".glyphicon").removeClass("glyphicon-remove-circle");
        $(component.RootElement).find(".glyphicon").addClass("glyphicon-remove-circle");

        AjaxService.Post("/api/cxa/Cart/GetShoppingCartLines", {}, function (data, success, sender) {
            if (success && data && data.Success) {
                component.model.updateModel(data);
            }
        });
    };

    component.StartListening = function () {
        CartContext.SubscribeHandler(CartContext.CartEvents.CartUpdate, component.OnCartUpdated)
    };

    component.Init = function () {
      
        if (CXAApplication.RunningMode === RunningModes.Normal) {
            component.StartListening();
            AjaxService.Post("/api/cxa/ShoppingCartLines/GetShoppingCartLines", {}, function (data, success, sender) {
                if (success && data && data.Success) {
                    component.model = new CartLinesViewModel(data);
                    ko.applyBindings(component.model, component.RootElement);
                }
            });
        }
    };

    return component;
}));