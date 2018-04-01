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

function MinicartViewModel(data) {
    var self = this;
    this.cart = ko.observable(new CartViewModel(data));

    self.updateModel = function (data) {
        self.cart(new CartViewModel(data));

    };
    self.slideDown = function (element) {
        $(".minicart").slideDown(500);
        return false;
    };
    self.slideUp = function (element) {
        $(element).slideUp(500);
        return false;
    };
    self.removeItem = function (item, event) {
        $(event.currentTarget).find(".glyphicon").removeClass("glyphicon-remove-circle");
        $(event.currentTarget).find(".glyphicon").addClass("glyphicon-refresh");
        $(event.currentTarget).find(".glyphicon").addClass("glyphicon-refresh-animate");
        var lineItemId = item.externalCartLineId;
        var sender = event.currentTarget;
        AjaxService.Post("/api/cxa/cart/RemoveMinicartLine", { lineNumber: lineItemId }, function (data, success, sender) {
            if (success && data.Success) {
                CartContext.TriggerCartUpdateEvent();
            }
        }, sender);
    };
}