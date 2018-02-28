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
    'use strict';
    if (typeof define === 'function' && define.amd) {
        // use AMD define funtion to support AMD modules if in use
        define(['exports'], factory);

    } else if (typeof exports === 'object') {
        // to support CommonJS
        factory(exports);
    }
    root.StartUpManager = factory;
}(this, function (element,actions) {

    var component = new Component(element);

    component.StartupActions = actions;

    component.Run = function () {
        $(component.StartupActions).each(function () {
            try{
                this();
            }
            catch (err) {
                component.PublishError(err);
            }
            
        });
    }

    component.Init = function () {
        component.Run();
    }
}));

var StartupActionDefinitions = {

    InitializeSubmitButtons : function () {
        $(component.RootElement).on('submit', 'form', function () {
            $("[data-loading-text]").button('loading');
        });
    }
}

var component = new StartUpManager(document, [
    StartupActionDefinitions.InitializeSubmitButtons
]);

