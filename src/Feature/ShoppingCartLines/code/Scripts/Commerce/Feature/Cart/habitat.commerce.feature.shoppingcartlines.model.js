//-----------------------------------------------------------------------
// Copyright 2016 Sitecore Corporation A/S
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file 
// except in compliance with the License. You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the 
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
// either express or implied. See the License for the specific language governing permissions 
// and limitations under the License.
// -------------------------------------------------------------------------------------------

function CartLinesViewModel(data) {
    var self = this;
    self.cart = ko.observable(new CartViewModel(data));
    self.updateModel = function (data) {
        self.cart(new CartViewModel(data));

    };
    self.removeItem = function (item, event) {
        $(event.currentTarget).find(".glyphicon").removeClass("glyphicon-remove-circle");
        $(event.currentTarget).find(".glyphicon").addClass("glyphicon-refresh");
        $(event.currentTarget).find(".glyphicon").addClass("glyphicon-refresh-animate");
        var lineItemId = item.externalCartLineId;
        var sender = event.currentTarget;
        AjaxService.Post("/api/cxa/cart/RemoveShoppingCartLine", { lineNumber: lineItemId }, function (data, success, sender) {
            if (success && data.Success) {
                CartContext.TriggerCartUpdateEvent();
            }
        });
    };
    self.addItemToWishList = function (item, event) {    
        $(event.currentTarget).find(".fa").addClass("fa-spinner");
        $(event.currentTarget).find(".fa").addClass("fa-spin");
        var lineItemId = item.externalCartLineId;      
        var sender = event.currentTarget;

        var cartID = self.cart().id;
        AjaxService.Post("api/cxa/ShoppingCartLines/GetCurrentCartLines", cartID, function (data, success, sender) {
            if (success) {
                $.each(data.Data, function () {
                    var exId;var prodId;var varId;
                    $(this).map(function (i, b) {
                        if ((b['Key'] == 'ExternalCartLineId'))
                            exId = b['Value'];
                        if ((b['Key'] == 'ProductId'))
                            prodId = b['Value'];
                        if ((b['Key'] == 'VariantId'))
                            varId = b['Value'];
                    })        
                    if (lineItemId === exId) {
                        AjaxService.Post("/api/cxa/wishlistlines/AddWishListLine", { productId: prodId, variantId: varId, quantity: item.quantity }, function (data, success, sender) {
                            if (success && data.Success) {
                                self.removeItem(item, event);                                
                            }
                        });
                    }
                });
            }
        })        
    };
    self.increaseQuantity = function (item) {
        item.quantity = Number(item.quantity) + 1;
        self.updateQuantity(item);
    }
    self.decreaseQuantity = function (item) {
        item.quantity = Number(item.quantity) - 1;
        self.updateQuantity(item);
    }

    self.quntityUpdating = ko.observable(false || (CXAApplication.RunningMode === RunningModes.ExperienceEditor));


    self.updateQuantity = function (item) {
        if (CXAApplication.RunningMode === RunningModes.Normal) {
            self.quntityUpdating(true);
            var model = self;
            AjaxService.Post("/api/cxa/cart/UpdateCartLineQuantity", { quantity: item.quantity, lineNumber: item.externalCartLineId }, function (data, success, self) {
                if (success && data.Success) {
                    CartContext.TriggerCartUpdateEvent();
                    model.quntityUpdating(false);
                }

            });
        }

    };
}