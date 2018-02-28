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
    'use strict';
    if (typeof define === 'function' && define.amd) {
        // use AMD define funtion to support AMD modules if in use
        define(['exports'], factory);

    } else if (typeof exports === 'object') {
        // to support CommonJS
        factory(exports);
    }

    root.CXAForm = factory;

}(this, function (element) {

    var form = {
        RootElement: element
    };
    form.FormRoot = $(form.RootElement).find('form');
    form.SubmitButton = $(form.FormRoot).find(':submit');

    form.Disable = function () {
        $(form.RootElement).find('input').attr('disabled', 'disabled');
        $(form.RootElement).find('button').attr('disabled', 'disabled');
        $(form.RootElement).find('a').attr('disabled', 'disabled');
    }

    form.Enable = function () {
        $(form.RootElement).find('input').removeAttr('disabled');
        $(form.RootElement).find('button').removeAttr('disabled');
        $(form.RootElement).find('a').removeAttr('disabled');
    }

    form.EnableInDesignEditing = function () {
        $inDesignButtons = $(form.RootElement).find(".disabled-in-design");

        $inDesignButtons.removeAttr("disabled");

        $inDesignButtons.on("click", function (e) {
            e.preventDefault();
            return false;
        });
    }

    form.OnBegin = function () {
        MessageContext.ClearAllMessages();
        $(form.SubmitButton).button('loading');
        form.Disable();
    }
    form.OnSuccess = function (data) { }

    form.OnComplete = function () {
        $(form.SubmitButton).button('reset');
        form.Enable();
    }

    form.Init = function (instance) {
        if (CXAApplication.RunningMode == RunningModes.ExperienceEditor) {
            form.Disable();
        }

        if (!instance.RootElement) { instance.RootElement = form.RootElement }
        if (!instance.OnBegin) { instance.OnBegin = form.OnBegin }
        if (!instance.OnComplete) { instance.OnComplete = form.OnComplete }
        if (!instance.OnSuccess) { instance.OnSuccess = form.OnSuccess }
    }

    return form;
}));


