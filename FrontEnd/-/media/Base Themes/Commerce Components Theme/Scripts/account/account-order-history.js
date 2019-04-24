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

(function (root, factory) {

    if (typeof define === "function" && define.amd) {
        // use AMD define funtion to support AMD modules if in use
        define("CXA/Feature/OrderHistory", ["exports"], factory);

    } else if (typeof exports === "object") {
        // to support CommonJS
        factory(exports);

    }
    // browser global variable
    root.OrderHistory = factory;

}(this, function (element, model) {
    "use strict";

    var component = new Component(element, model);

    component.Name = "CXA/Feature/OrderHistory";

    var AddMockData = function (component) {
        component.Model.orders.push(new OrderHeaderViewModel({
            OrderStatus: "lorem ipsum",
            OrderDate: "01/01/1970",
            OrderId: "UV3QSJDFL66P"
        }));
        component.Model.orders.push(new OrderHeaderViewModel({
            OrderStatus: "lorem ipsum",
            OrderDate: "01/01/1970",
            OrderId: "VBB4SJHJM06T"
        }));
        component.Model.orders.push(new OrderHeaderViewModel({
            OrderStatus: "lorem ipsum",
            OrderDate: "01/01/1970",
            OrderId: "CD7FJXXE02E"
        }));
        component.Model.orders.push(new OrderHeaderViewModel({
            OrderStatus: "lorem ipsum",
            OrderDate: "01/01/1970",
            OrderId: "TMO45P6XXDR"
        }));
        component.Model.orders.push(new OrderHeaderViewModel({
            OrderStatus: "lorem ipsum",
            OrderDate: "01/01/1970",
            OrderId: "FTP76DVP0X1"
        }));
    }

    component.InExperienceEditorMode = function() {
        AddMockData(component);
        component.Visual.Disable();
    };

    component.Init = function () {
        if (CXAApplication.RunningMode === RunningModes.Normal) {
            $(component.RootElement).find('.recent-orders-list').addClass("loading");
            AjaxService.Post("/api/cxa/Orders/GetOrderHistory", {}, function (data, success) {
                $(component.RootElement).find('.recent-orders-list').removeClass("loading");
                if (success && data && data.Success) {
                    component.Model.updateModel(data);
                }
            });
        }
    };

    return component;
}));


