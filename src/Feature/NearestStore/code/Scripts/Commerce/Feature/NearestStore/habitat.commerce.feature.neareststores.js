var NearestStoreViewModel = function () {
    var self = this;
    self.ErrorMessage = ko.observable("");
    self.ProductId = ko.observable("");
    self.VariantId = ko.observable("");
    self.UserLat = ko.observable("");
    self.UserLong = ko.observable("");
    self.NearestStoresList = ko.observable([]);


    self.InitializeViewModel = function (productID, variantID) {          
        self.ProductId = productID;
        self.VariantId = variantID;
        if (getCookie("sxa_site_shops_stores") != "") {
            self.GetInventory();
        }
        else {
            self.LoadNearestStores();
        }        
    }

    self.LoadNearestStores = function () {
        var ServiceRequest = new Object();        
        ServiceRequest.ProductId = self.ProductId;
        ServiceRequest.VariantId = self.VariantId;
        var promise = GetNearestStores(ServiceRequest).done(function (result) {
            if (result.Errors != null & result.Errors != "undefined") {
                self.ErrorMessage(data.Errors[0]);
            }
            else {

                var storesList = [];
                if (result.Data.length > 0) {
                    $.each(result.Data, function (index, value) {
                        var newStore = new NewStore(value);
                        storesList.push(newStore);
                    });
                    self.NearestStoresList(storesList);
                }
            }
        });
    }    

    self.GetInventory = function () {
        var ServiceRequest = new Object();        
        ServiceRequest.ProductId = self.ProductId;
        ServiceRequest.VariantId = self.VariantId;
        var promise = GetInventory(ServiceRequest).done(function (result) {
            if (result.Errors != null & result.Errors != "undefined") {
                self.ErrorMessage(data.Errors[0]);
            }
            else {
                var storesData = result.Data;
                var storesList = [];
                //self.NearestStoresList(storesData);
                $.each(result.Data, function (index, value) {
                    var newStore = new NewStore(value);
                    storesList.push(newStore);
                });
                self.NearestStoresList(storesList);
            }
        })
    }

    self.UpdateVariant = function (variantId) {
        self.VariantId = variantId;
        var ServiceRequest = new Object();
        ServiceRequest.ProductId = self.ProductId;
        ServiceRequest.VariantId = self.VariantId;
        var promise = GetInventory(ServiceRequest).done(function (result) {
            if (result.Errors != null & result.Errors != "undefined") {
                self.ErrorMessage(data.Errors[0]);
            }
            else {
                var storesData = result.Data;
                var storesList = [];
                //self.NearestStoresList(storesData);
                if (result.Data.length > 0) {
                    $.each(result.Data, function (index, value) {
                        var newStore = new NewStore(value);
                        storesList.push(newStore);
                    });
                    self.NearestStoresList(storesList);
                }
            }
        })
    }
}

function NewStore(data) {
    self = this;
    $(data).map(function (i, b) {
        if ((b['Key'] == 'Id'))
            self.Id = b['Value'];
        if ((b['Key'] == 'Distance'))
            self.Distance = b['Value'];
        if ((b['Key'] == 'ZeroInventory'))
            self.ZeroInventory = b['Value'];
        if ((b['Key'] == 'DisplayName'))
            self.DisplayName = b['Value'];
        if ((b['Key'] == 'Limited'))
            self.Limited = b['Value'];
        if ((b['Key'] == 'Quantity'))
            self.Quantity = b['Value'];
        if ((b['Key'] == 'InventoryAmount'))
            self.InventoryAmount = b['Value'];
    })
}
//Promise Functions
function GetNearestStores(ServiceRequest) {

    var url = "/api/cxa/NearestStore/GetStores?pid=" + ServiceRequest.ProductId + "-" + ServiceRequest.VariantId;
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

function GetInventory(ServiceRequest) {

    var url = "/api/cxa/NearestStore/GetInventory?pid=" + ServiceRequest.ProductId + "-" + ServiceRequest.VariantId;
    var requestData = JSON.stringify(ServiceRequest);
    $('.loader').show();
    var ajaxRequest = $.ajax({
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        url: url,
        //data: { 'userLatitude': ServiceRequest.Latitude, 'userLongitude': ServiceRequest.Longitude },
        success: function (data) {
            $('.loader').hide();
        },
        error: function (x, y, z) {
        }
    });
    return ajaxRequest;
}

//General Functions
function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

function InitializeNearestStoreWidget() {
    
    NearestStoreVMController = new NearestStoreViewModel();
    if ($("#divNearestStores").length > 0) {

        ko.applyBindings(NearestStoreVMController, document.getElementById("divNearestStores"));        
        var productID = $("#addtocart_productid").val();
        var variantID = ($("#addtocart_variantid").val() != "" ? $("#addtocart_variantid").val() : $(".valid-variant-combo").children(":first").attr("id"));

        NearestStoreVMController.InitializeViewModel(productID, variantID);
        $("#variantColor").on('change', function () {
            NearestStoreVMController.UpdateVariant($("#addtocart_variantid").val());           
        });
    }    
}
$(document).ready(function () {    
    InitializeNearestStoreWidget();
});