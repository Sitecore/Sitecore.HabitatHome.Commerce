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

function ForgotPasswordViewModel(inExperienceEditorMode) {
    var self = this;

    if (!inExperienceEditorMode) {
        inExperienceEditorMode = false;
    }

    self.showConfirmationView = ko.observable(inExperienceEditorMode);
    self.showEmailEntryView = ko.observable(true);
    self.emailAddress = ko.observable("");

    self.displayEmailAddress = ko.computed(function () {
        if (self.emailAddress() === "") {
            return "...";
        }

        return self.emailAddress();
    });
}

(function (root, factory) {
    'use strict';
    if (typeof define === 'function' && define.amd) {
        // use AMD define funtion to support AMD modules if in use
        define(['exports'], factory);

    } else if (typeof exports === 'object') {
        // to support CommonJS
        factory(exports);
    }
    var ForgotPasswordForm = {}
    root.ForgotPasswordForm = ForgotPasswordForm;
    factory(ForgotPasswordForm);

}(this, function (ForgotPasswordForm) {
    var form;

    ForgotPasswordForm.OnSuccess = function (data) {
        console.log(data);

        if (data && data.Success) {
            form.model.showConfirmationView(true);
            form.model.showEmailEntryView(false);
        }
    }

    ForgotPasswordForm.Init = function (element) {
        form = new CXAForm(element);
        form.Init(ForgotPasswordForm);

        var isEEMode = CXAApplication.IsExperienceEditorMode();
        form.model = new ForgotPasswordViewModel(isEEMode);
        ko.applyBindings(form.model, form.RootElement);

        //Only enable form if we are not in experience editor mode
        if (isEEMode === false) {
            form.Enable();
        }
        else {
            form.EnableInDesignEditing();
        }
    }

    return form;
}));
