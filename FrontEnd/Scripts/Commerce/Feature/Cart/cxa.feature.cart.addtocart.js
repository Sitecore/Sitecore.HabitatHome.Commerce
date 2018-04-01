(function (root, factory) {
    var AddToCartForm = {}
    root.AddToCartForm = AddToCartForm;
    factory(root.CartContext);
}(this, function (currentCart) {
    AddToCartForm.OnSuccess = function (data) {
        if (data.Success) {
            currentCart.TriggerCartUpdateEvent();
        }
    }
    AddToCartForm.Init = function (element) {
        var form = new CXAForm(element);
        form.Init(AddToCartForm);
        if (CXAApplication.IsExperienceEditorMode() === false) {
            form.Enable();
        }
        else {
            form.EnableInDesignEditing();
        }
    }
}));