// -----------------------------------------------------------------------
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

(function (root, $, factory) {
  root.AddressEditor = factory;
}(this, jQuery, function (element) {
  "use strict";
  var component = new Component(element);

  component.InExperienceEditorMode = function () {
    component.Visual.Disable();
  };

  component.Init = function () {
    if (CXAApplication.IsExperienceEditorMode() === false) {
      component.Visual.Enable();
      AjaxService.Post("/api/cxa/AccountAddress/AddressEditorList", {}, function (data, success, sender) {
        var root = $(component.RootElement);
        var addressId = root.data("address-id");
        var accountPageUrl = root.data("page-url");
        component.model = new AddressEditorViewModel(data, accountPageUrl, addressId, component.RootElement);
        if (success && data) {
          ko.applyBindingsWithValidation(component.model, component.RootElement);
          component.model.showLoader(false);
        }
      });
    }
  };

  return component;
}));