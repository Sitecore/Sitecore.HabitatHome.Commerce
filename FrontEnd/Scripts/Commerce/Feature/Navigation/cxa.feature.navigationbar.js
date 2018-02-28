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
(function (root, factory) {
    if (typeof define === 'function' && define.amd) {
        define('CXA/Feature/Navigation', ['exports'], factory);

    } else if (typeof exports === 'object') {
        factory(exports);
    }

    // browser global variable
    root.Navigation = factory;
}(this, function (element) {
    var component = new Component(element);

    component.Visual.DisableLinks = function () {
        return false;
    }

    component.InExperienceEditorMode = function () {
        component.Visual.Disable();
    }

    return component;
}));