(function (root, $, factory) {

    root.AddressEditor = factory;

}(this, jQuery, function (element) {
    "use strict";
    var component = new Component(element);

    component.InExperienceEditorMode = function () {
        component.Visual.Disable();
    };

    component.Init = function () {

        if (CXAApplication.IsExperienceEditorMode() === false) {
            component.Visual.Enable();
            AjaxService.Post("/api/cxa/AccountAddress/AddressEditorList", {}, function (data, success, sender) {
                var root = $(component.RootElement);
                var addressId = root.data("address-id");
                var accountPageUrl = root.data("page-url");
                component.model = new AddressEditorViewModel(data, accountPageUrl, addressId, component.RootElement);
                if (success && data) {
                    ko.applyBindingsWithValidation(component.model, component.RootElement);
                    component.model.showLoader(false);
                }
            });
        }
    };

    return component;
}));