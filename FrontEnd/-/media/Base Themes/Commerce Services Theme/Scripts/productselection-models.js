//-----------------------------------------------------------------------
// <copyright file="cxa.common.productselection.models.js" company="Sitecore Corporation">
// Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>Provides a common tool</summary>
//-----------------------------------------------------------------------
// Copyright 2017 Sitecore Corporation A/S
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file 
// except in compliance with the License. You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the 
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
// either express or implied. See the License for the specific language governing permissions 
// and limitations under the License.
// -------------------------------------------------------------------------------------------

function BundleItemSelection() {
    self = this;

    self.catalogName = "";
    self.productId = "";
    self.variantId = "";
}

function BundleSelection() {
    self = this;

    self.catalogName = "";
    self.productId = "";
    self.bundledItemList = [];
    self.quantity = "";

    self.addBundleItemSelection = function (bundledItem) {
        this.bundledItemList.push(bundledItem);
    }

    self.toJSONString = function () {

        return JSON.stringify(this);
    }
}