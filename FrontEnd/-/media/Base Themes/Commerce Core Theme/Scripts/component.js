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
    define('CXA/Common/Component', ['exports'], factory);
  } else if (typeof exports === 'object') {
    // to support CommonJS
    factory(exports);
  }
  // browser global variable
  root.Component = factory;
}(this, function (element, model) {
  'use strict';
  var component = {};

  component.InExperienceEditorMode = function () {
    // To be overridden by actual component
  };

  component.Init = function () {
    // To be overridden by actual component
  };

  // public methods available to all components
  component.PublishError = function (message, data) {
    MessageContext.PublishError(component, message, data);
  };
  component.PublishInfo = function (message, data) {
    MessageContext.PublishInfo(component, message, data);
  };
  component.PublishWarning = function (message, data) {
    MessageContext.PublishWarning(component, message, data);
  };

  component.Visual = {
    EnableInDesignEditing: function () {
      var $inDesignButtons = $(component.RootElement).find(".disabled-in-design");

      $inDesignButtons.removeAttr("disabled");

      $inDesignButtons.on("click", function (e) {
        e.preventDefault();
        return false;
      });
    },
    Appear: function () {
      $(component.RootElement).fadeIn(100);
    },
    DisAppear: function () {
      $(component.RootElement).hide();
    },
    DisableLinks: function () {
      $(component.RootElement).find('a')
        .attr('href', 'javascript:void(0)');
    },
    Disable: function () {
      $(component.RootElement).find('input')
        .attr('disabled', 'disabled');
      $(component.RootElement).find('select')
        .attr('disabled', 'disabled');
      $(component.RootElement).find('textarea')
        .attr('disabled', 'disabled');
      $(component.RootElement).find('button')
        .attr('disabled', 'disabled');
      $(component.RootElement).find('a')
        .attr('disabled', 'disabled');
      component.Visual.DisableLinks();

      if (CXAApplication.IsExperienceEditorMode()) {
        this.EnableInDesignEditing();
      }
    },
    Enable: function () {
      $(component.RootElement).find('input')
        .removeAttr('disabled');
      $(component.RootElement).find('button')
        .removeAttr('disabled');
    }
  };

  component.LoadData = function (url, data, callback) {
    AjaxService.Post(url, data, function (data, success, sender) { if (success) callback(data); }, component);
  };

  // Load the component in the CXA Application instance
  CXAApplication.LoadComponent(component, element, model);

  return component;
}));