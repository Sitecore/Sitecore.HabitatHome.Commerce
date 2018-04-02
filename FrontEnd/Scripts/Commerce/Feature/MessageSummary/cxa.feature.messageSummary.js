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
        define('CXA/Feature/MessageSummary', ['exports'], factory);

    } else if (typeof exports === 'object') {
        // to support CommonJS
        factory(exports);

    }
    // browser global variable
    root.MessageSummary = factory;

}(this, function (element,model) {
    'use strict';

    var component = new Component(element, model);
    component.Name = "CXA/Feature/MessageSummary";
    component.InExperienceEditorMode = function()
    {
        model.errorMessages.push(new Message("error: Lorem ipsum dolor sit amet, consectetur adipiscing.", null));
        model.infoMessages.push(new Message("info: Lorem ipsum dolor sit amet, consectetur adipiscing.", null));
        model.warningMessages.push(new Message("warning: Lorem ipsum dolor sit amet, consectetur adipiscing.", null));
    }
    //Message Handlers
    component.ErrorMessageHandler = function (source, message, data) {
        var item = new Message(message, data);
        if (item.message) {
            model.errorMessages.push(item);
        }
    }

    component.WarningMessageHandler = function (source, message, data) {
        var item = new Message(message, data);
        if (item.message) {
            model.warningMessages.push(item);
        }
    }

    component.InfoMessageHandler = function (source, message, data) {
        var item = new Message(message, data);
        if (item.message) {
            model.infoMessages.push(item);
        }
    }

    component.CommandMessageHandler = function (source, command, data) {
        if (command == MessageContext.StandardCommands.ClearAll)
        {
            component.Model.clear();
        }
    }

    component.StartListening = function () {
        component.ErrorHandlerId = MessageContext.SubscribeMessageHandler(MessageContext.StandardChannels.Error, component.ErrorMessageHandler);
        component.WarningHandlerId = MessageContext.SubscribeMessageHandler(MessageContext.StandardChannels.Warning, component.WarningMessageHandler);
        component.InfoHandlerId = MessageContext.SubscribeMessageHandler(MessageContext.StandardChannels.Info, component.InfoMessageHandler);
        component.CommandHandlerId = MessageContext.SubscribeMessageHandler(MessageContext.StandardChannels.Command, component.CommandMessageHandler);
    }
    component.StopListening = function ()
    {
        if (component.ErrorHandlerId)
            MessageContext.UnSubscribeHandler(component.ErrorHandlerId);
        if (component.WarningHandlerId)
            MessageContext.UnSubscribeHandler(component.WarningHandlerId);
        if (component.InfoHandlerId)
            MessageContext.UnSubscribeHandler(component.InfoHandlerId);
    }

    component.Init = function () {
        component.StartListening();
    };
   
    return component;
}));