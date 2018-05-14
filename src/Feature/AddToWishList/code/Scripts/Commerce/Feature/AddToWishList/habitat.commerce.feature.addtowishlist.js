var AddToWishListViewModel = function () {
    var self = this;
    self.ErrorMessage = ko.observable("");
    self.ProductId = ko.observable("");
    self.VariantId = ko.observable("");   

    self.InitializeViewModel = function (productID, variantID) {          
        self.ProductId = productID;
        self.VariantId = variantID;
        
    }

    self.AddItemToWishList = function (item, event) {        
        var itemQuantity = $('.add-to-cart-qty-input').val();
        var sender = event.currentTarget;
        $(event.currentTarget).attr('disabled', 'disabled');
        $(event.currentTarget).find('span').text('ADDING TO MY WISH LIST ...');

        AjaxService.Post("/api/cxa/wishlistlines/AddWishListLine", { productId: self.ProductId, variantId: self.VariantId, quantity: itemQuantity }, function (data, success, sender) {
            if (success && data.Success) {
                $(event.currentTarget).removeAttr('disabled');
                $(event.currentTarget).find('span').text('ADD TO MY WISH LIST');
            }
        });
    }
    self.UpdateVariant = function (variantId) {
        self.VariantId = variantId;        
    }
}

function InitializeAddToWishListButoon() {
    
    AddToWishListVMController = new AddToWishListViewModel();
    if ($("#AddToWishList").length > 0) {

        ko.applyBindings(AddToWishListVMController, document.getElementById("AddToWishList"));        
        var productID = $("#addtocart_productid").val();
        var variantID = ($("#addtocart_variantid").val() != "" ? $("#addtocart_variantid").val() : $(".valid-variant-combo").children(":first").attr("id"));

        AddToWishListVMController.InitializeViewModel(productID, variantID);
        $("#variantColor").on('change', function () {
            AddToWishListVMController.UpdateVariant($("#addtocart_variantid").val());           
        });
    }    
}
$(document).ready(function () {    
    InitializeAddToWishListButoon();
});