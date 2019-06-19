var ProductRelatedContentViewModel = function () {
    var self = this;
    self.ErrorMessage = ko.observable("");
    self.ProductId = ko.observable(""); 
    self.ProductListId = ko.observable(""); 
    self.RelatedProductsList = ko.observable([]);
    self.CrossSellProductsList = ko.observable([]);
    self.UpSellProductsList = ko.observable([]);
    self.ProductDocumentsList = ko.observable([]);

    self.InitializeViewModel = function (productID, productListID) {         
        self.ProductId = productID;
        self.ProductListId = productListID;
        self.GetRelatedProducts();    
        self.GetProductDocuments();
    }

    self.GetRelatedProducts = function () {
        var ServiceRequest = new Object();       
        ServiceRequest.ProductId = self.ProductId;  
        ServiceRequest.ProductListId = self.ProductListId;  
        var relatedPromise = GetRelatedProducts(ServiceRequest).done(function (result) {
            if (result.Errors != null & result.Errors != "undefined") {
                self.ErrorMessage(data.Errors[0]);
            }
            else {
                var productsList = [];
                if (result.Data.length > 0) {
                    $('.related-content-tabs').show();
             
                    $.each(result.Data, function (index, value) {
                        var newRelatedProduct = new AssociatedProduct(value);
                       
                        productsList.push(newRelatedProduct);
                    });
                    self.RelatedProductsList(productsList);
                    //setEqualHeight($(".related-content-product"));
                }

                var crossSellPromise = GetCrossSellProducts(ServiceRequest).done(function (result) {
                    if (result.Errors != null & result.Errors != "undefined") {
                        self.ErrorMessage(data.Errors[0]);
                    }
                    else {
                        var productsList = [];
                        if (result.Data.length > 0) {
                            $('.related-content-tabs').show();
                       
                            $.each(result.Data, function (index, value) {
                                var newRelatedProduct = new AssociatedProduct(value);

                                productsList.push(newRelatedProduct);
                            });
                            self.CrossSellProductsList(productsList);
                            //setEqualHeight($(".related-content-product-cross"));
                        }

                        var upSellPromise = GetUpSellProducts(ServiceRequest).done(function (result) {
                            if (result.Errors != null & result.Errors != "undefined") {
                                self.ErrorMessage(data.Errors[0]);
                            }
                            else {
                                var productsList = [];
                                if (result.Data.length > 0) {
                                    $('.related-content-tabs').show();
                               
                                    $.each(result.Data, function (index, value) {
                                        var newRelatedProduct = new AssociatedProduct(value);

                                        productsList.push(newRelatedProduct);
                                    });
                                    self.UpSellProductsList(productsList);
                                    //setEqualHeight($(".related-content-product-up"));
                                }
                            }
                        });
                    }
                });
            }
        });
        
        
    }    

    self.GetProductDocuments = function () {
        var ServiceRequest = new Object();
        ServiceRequest.ProductId = self.ProductId;

        var promise = GetProductDocuments(ServiceRequest).done(function (result) {
            if (result.Errors != null & result.Errors != "undefined") {
                self.ErrorMessage(data.Errors[0]);
            }
            else {
                documentsList = [];
                if (result.Data.length > 0) {
                    $('.related-content-tabs').show();
                    $.each(result.Data, function (index, value) {
                        var newProductDocument = new AssociatedDocument(value);
                        documentsList.push(newProductDocument);
                    });
                    self.ProductDocumentsList(documentsList);
                    
                }
            }
        });
    }    
}

function AssociatedProduct(data) {
    self = this;
    self.DisplayName = data['ProductName'];
    self.ImageUrl = data['ImageSrc'];
    self.ProductUrl = data['ProductUrl'];
    self.Price = data["ListPrice"];
}
function AssociatedDocument(data) {
    self = this;
    self.DocumentName = data['DocumentName'];
    self.ImageUrl = data['ImageUrl'];
    self.DocumentUrl = data['DocumentUrl'];
    self.Description = data["Description"];
    self.DocumentType = data["DocumentType"];
}
//Promise Functions
function GetRelatedProducts(ServiceRequest) {
    var url = "/api/cxa/ProductRelatedContent/GetRelatedProducts?pid=" + ServiceRequest.ProductId + "&plid=" + ServiceRequest.ProductListId;   
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

function GetCrossSellProducts(ServiceRequest) {
    var url = "/api/cxa/ProductRelatedContent/GetCrossSellProducts?pid=" + ServiceRequest.ProductId;
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

function GetUpSellProducts(ServiceRequest) {
    var url = "/api/cxa/ProductRelatedContent/GetUpSellProducts?pid=" + ServiceRequest.ProductId;
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

function GetProductDocuments(ServiceRequest) {
    var url = "/api/cxa/ProductRelatedContent/GetProductDocuments?pid=" + ServiceRequest.ProductId;
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

function InitializeProductRelatedContentWidget() {
    
    ProductRelatedContentController = new ProductRelatedContentViewModel();
    if ($("#divProductRelatedContent").length > 0) {
        ko.applyBindings(ProductRelatedContentController, document.getElementById("divProductRelatedContent"));  
        var productID = $("#variant-component-product-id").val();
        var productListID = $("#personalized-product-list").val();
        ProductRelatedContentController.InitializeViewModel(productID, productListID);        
    }    
}
function setEqualHeight(columns) {
    var tallestcolumn = 0;
    columns.each(function () {
        currentHeight = $(this).height();
        if (currentHeight > tallestcolumn) {
            tallestcolumn = currentHeight;
        }
    });
    columns.height(tallestcolumn);
    columns.css('display', 'table');
}
$(document).ready(function () {    
    InitializeProductRelatedContentWidget();
});

