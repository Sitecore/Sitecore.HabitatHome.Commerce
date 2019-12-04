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
  var ProfileEditorForm = {};
  root.ProfileEditorForm = ProfileEditorForm;
  factory(ProfileEditorForm);
}(this, function (ProfileEditorForm) {
  var GetSuccessRedirectUrl = function () {
    redirectUrl = $(ProfileEditorForm.RootElement).data("save-redirect-link");
    return redirectUrl;
  };

  ProfileEditorForm.OnSuccess = function (data) {
    if (data && data.Success) {
      var redirectUrl = GetSuccessRedirectUrl();

      if (redirectUrl) {
        window.location.href = redirectUrl;
      }
    }
  };

  ProfileEditorForm.Init = function (element) {
    var form = new CXAForm(element);
    form.Init(ProfileEditorForm);

    // Only enable form if we are not in experience editor mode
    if (CXAApplication.IsExperienceEditorMode() === false) {
      form.Enable();
    }
  };
}));