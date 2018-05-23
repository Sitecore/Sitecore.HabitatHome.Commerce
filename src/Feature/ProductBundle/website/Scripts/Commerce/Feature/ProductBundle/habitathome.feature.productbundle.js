var ProductBundleViewModel = function () {
    var self = this;
    self.ErrorMessage = ko.observable("");
    self.ProductId = ko.observable("");    
    self.RelatedProductsList = ko.observable([]);

    self.InitializeViewModel = function (productID) {          
        self.ProductId = productID;
        self.GetRelatedProducts();       
    }

    self.GetRelatedProducts = function () {
        var ServiceRequest = new Object();        
        ServiceRequest.ProductId = self.ProductId;        
        var promise = GetBundleRelatedProducts(ServiceRequest).done(function (result) {
            if (result.Errors != null & result.Errors != "undefined") {
                self.ErrorMessage(data.Errors[0]);
            }
            else {
                var productsList = [];
                if (result.Data.length > 0) {
                    console.log(result.Data);
                    $.each(result.Data, function (index, value) {
                        var newRelatedProduct = new RelatedBundleProduct(value);
                       
                        productsList.push(newRelatedProduct);
                    });
                    self.RelatedProductsList(productsList);
                }
            }
        });
    }    


}

function RelatedBundleProduct(data) {
    self = this;  
    self.DisplayName = data['ProductName'];
    self.Description = data['Description'];
    self.ImageUrl = data['Image'];
    self.ProductUrl = data['ProductUrl'];
    self.ProductId = data['ProductId'];
    self.Quantity = data['Quantity'];
    self.ProductVariants = data['ProductVariants'];
    self.VariantOptions = data['VariantOptions'];
}
//Promise Functions
function GetBundleRelatedProducts(ServiceRequest) {
    var url = "/api/cxa/ProductBundle/GetRelatedProducts?pid=" + ServiceRequest.ProductId;
    var requestData = JSON.stringify(ServiceRequest);
    $('.loader').show();
    var ajaxRequest = $.ajax({
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        url: url,
        success: function (data) {
            $('.loader').hide();
        },
        error: function (x, y, z) {
        }
    });
    return ajaxRequest;
}

function InitializeProductBundleWidget() {
    
    ProductBundleVMController = new ProductBundleViewModel();
    if ($("#divProductBundle").length > 0) {
        ko.applyBindings(ProductBundleVMController, document.getElementById("divProductBundle"));        
        var productID = $("#variant-component-product-id").val();        
        ProductBundleVMController.InitializeViewModel(productID);        
    }    
}
$(document).ready(function () {    
    InitializeProductBundleWidget();
});