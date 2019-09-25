// -----------------------------------------------------------------------
// Copyright 2017-2018 Sitecore Corporation A/S
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
  'use strict';
  if (typeof define === 'function' && define.amd) {
    // use AMD define funtion to support AMD modules if in use
    define(['exports'], factory);
  } else if (typeof exports === 'object') {
    // to support CommonJS
    factory(exports);
  }
  var RegistrationForm = {};
  root.RegistrationForm = RegistrationForm;
  factory(RegistrationForm);
}(this, function (RegistrationForm) {
  RegistrationForm.OnSuccess = function (data) {
    if (data && data.Success) {
      if (data.IsSignupFlow) {
        var url = new Uri("/UserPendingActivation")
          .addQueryParam("isSignupFlow", data.IsSignupFlow)
          .addQueryParam("email", data.UserName)
          .toString();
        CXAApplication.Goto(url);
      } else {
        CXAApplication.Goto("/AccountManagement");
      }
    }
  };

  RegistrationForm.Init = function (element) {
    var form = new CXAForm(element);
    form.Init(RegistrationForm);

    // Only enable form if we are not in experience editor mode
    if (CXAApplication.IsExperienceEditorMode() === false) {
      form.Enable();
    } else {
      form.EnableInDesignEditing();
    }
  };
}));