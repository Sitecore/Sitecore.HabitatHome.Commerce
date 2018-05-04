var ProductKitViewModel = function () {
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
        var promise = GetRelatedProducts(ServiceRequest).done(function (result) {
            if (result.Errors != null & result.Errors != "undefined") {
                self.ErrorMessage(data.Errors[0]);
            }
            else {
                var productsList = [];
                if (result.Data.length > 0) {
                    console.log(result.Data);
                    $.each(result.Data, function (index, value) {
                        var newRelatedProduct = new RelatedProduct(value);
                       
                        productsList.push(newRelatedProduct);
                    });
                    self.RelatedProductsList(productsList);
                    console.log(self.RelatedProductsList());
                }
            }
        });
    }    


}

function RelatedProduct(data) {
    self = this;
    console.log(data['ProductName']);
    self.DisplayName = data['ProductName'];
    self.Description = data['Description'];
    self.ImageUrl = data['Image'];
    self.ProductUrl = data['ProductUrl'];
}
//Promise Functions
function GetRelatedProducts(ServiceRequest) {
    var url = "/api/cxa/ProductKit/GetRelatedProducts?pid=" + ServiceRequest.ProductId;
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

function InitializeProductKitWidget() {
    
    ProductKitVMController = new ProductKitViewModel();
    if ($("#divProductKit").length > 0) {
        ko.applyBindings(ProductKitVMController, document.getElementById("divProductKit"));        
        var productID = $("#variant-component-product-id").val();        
        ProductKitVMController.InitializeViewModel(productID);        
    }    
}
$(document).ready(function () {    
    InitializeProductKitWidget();
});