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

function CartPromotionViewModel(data) {
    var self = this;
    self.cart = ko.observable(new CartViewModel(data));
    self.updateModel = function (data) {
        self.cart(new CartViewModel(data));
    };
    self.removePromotionCode = function (promoCode, event) {
        if (CXAApplication.RunningMode === RunningModes.Normal) {
            $(event.currentTarget).find(".glyphicon").removeClass("glyphicon-remove-circle");
            $(event.currentTarget).find(".glyphicon").addClass("glyphicon-refresh");
            $(event.currentTarget).find(".glyphicon").addClass("glyphicon-refresh-animate");
            AjaxService.Post("/api/cxa/cart/RemoveDiscount",
                { promotionCode: promoCode },
                function(data, success, self) {
                    if (success && data.Success) {
                        CartContext.TriggerCartUpdateEvent();
                    }
                });
        }
        else {
            self.cart().promoCodes.remove(promoCode);
        }
    };
    self.addPromotionCode = function () {
        if (CXAApplication.RunningMode === RunningModes.Normal) {
            AjaxService.Post("/api/cxa/cart/ApplyDiscount",
                { promotionCode: self.cart().promoCode() },
                function(data, success, self) {
                    if (success && data.Success) {
                        CartContext.TriggerCartUpdateEvent();
                    }

                });
        }
        else {
            self.cart().promoCodes.push(self.cart().promoCode());
        }
       
    };
}