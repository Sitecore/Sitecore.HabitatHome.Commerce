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

(function (root, factory) {
  'use strict';
  if (typeof define === 'function' && define.amd) {
    // use AMD define funtion to support AMD modules if in use
    define(['CXA/Application', 'exports'], factory);
  } else if (typeof exports === 'object') {
    // to support CommonJS
    factory(exports);
  }

  // browser global variable
  root.RunningModes = {
    Normal: "Normal",
    ExperienceEditor: "Experience Editor"
  };

  root.ComponentType = {
    Form: "form",
    Component: "component"
  };

  var CXAApplication = {
    CXAComponentIdEnumerator: -1,
    CXAComponentIdPrefix: "cxa_component_",
    GenerateComponentId: function () {
      return this.CXAComponentIdPrefix + ++this.CXAComponentIdEnumerator;
    }
  };

  root.CXAApplication = CXAApplication;
  factory(CXAApplication);
}(this, function (CXAApplication) {
  'use strict';
  CXAApplication.RunningMode = RunningModes.Normal;
  CXAApplication.LoadedComponents = [];

  var _CreateAndInitialize = function ($object, callInitMethod) {
    // Had to set both data and attr as filteration is done on the basis of the attribute for performance.
    $object.data("cxa-component-initialized", "true");
    $object.attr("data-cxa-component-initialized", "true");

    var $componentClassName = $object.data("cxa-component-class");
    var $componentModelName = $object.data("cxa-component-model");

    var $componentType = $object.data("cxa-component-type");

    switch ($componentType) {
    case ComponentType.Component:
      var model;

      if ($componentModelName) {
        model = eval("new " + $componentModelName + "({})");
      }

      var componentInstance = eval($componentClassName)($object.get(0), model);

      if (callInitMethod) {
        // Initialize the Experience Editor specific display
        if (CXAApplication.IsExperienceEditorMode() && componentInstance.InExperienceEditorMode) {
          componentInstance.InExperienceEditorMode();
        }

        // Initialize the component
        if (componentInstance.Init) {
          componentInstance.Init();
        }
      }

      break;
    case ComponentType.Form:
      var $form = eval($componentClassName);

      if ($form.Init) {
        $form.Init($object.get(0));
      }
      // else {
      //     console.error("TODO: ComponentType.Form - " + $componentClassName + ".Init() not defined.");
      // }

      break;

    default:
      break;
    }
  };

  CXAApplication.CreateComponentInstance = function (cssSelector, callInitMethod, loadAll) {
    var $selectedObjects;

    $selectedObjects = $(cssSelector);

    if (!loadAll) {
      $selectedObjects = $selectedObjects.not("[data-cxa-component-initialized='true']");
    }

    if ($selectedObjects.length === 0) {
      return;
    }

    $selectedObjects.each(function () {
      _CreateAndInitialize($(this), callInitMethod);
    });
  };

  CXAApplication.IsExperienceEditorMode = function () {
    return CXAApplication.RunningMode === RunningModes.ExperienceEditor || typeof Sitecore !== "undefined" && typeof Sitecore.WebEditSettings !== "undefined" && Sitecore.WebEditSettings.editing;
  };

  CXAApplication.InitializeComponents = function () {
    CXAApplication.CreateComponentInstance("[data-cxa-component-class]", false, true);

    $(CXAApplication.LoadedComponents).each(function () {
      try {
        this.Init();
      } catch (err) {
        MessageContext.PublishError(CXAApplication, "component initialization failed : " + err);
        throw err;
      }
    });
  };

  CXAApplication.Initialize = function () {
    var pageExtension = getCurrentPageExtension();

    if (typeof Sitecore !== "undefined" && typeof Sitecore.WebEditSettings !== "undefined" && Sitecore.WebEditSettings.editing ||
            pageExtension === "html") {
      CXAApplication.RunningMode = RunningModes.ExperienceEditor;
    }

    CXAApplication.InitializeComponents();

    if (CXAApplication.IsExperienceEditorMode()) {
      $(CXAApplication.LoadedComponents).each(function () {
        this.InExperienceEditorMode();
      });
    }

    // If in Experience Editor mode, we need to observe DOM so that newly added components can be initialized...
    if (CXAApplication.IsExperienceEditorMode()) {
      if (typeof DomObserver === "undefined") {
        MessageContext.PublishError(CXAApplication, "DomObserver Service not Initialized...");
      } else {
        DomObserver.ObserveDomChanges(function () {
          CXAApplication.CreateComponentInstance("[data-cxa-component-class]", true);
        }, $("#wrapper").get(0));
      }
    }
  };

  CXAApplication.LoadComponent = function (component, element, model) {
    try {
      // setting components name and id
      component.Id = CXAApplication.GenerateComponentId();
      component.RootElement = element;
      // if there's a model bind it to the root element of component
      if (model) {
        model.Id = ko.observable(component.Id);
        component.Model = model;
        // bind the root element to the knockout model
        ko.applyBindings(model, element);
      }

      CXAApplication.LoadedComponents.push(component);
    } catch (err) {
      MessageContext.PublishError(CXAApplication, "component loading failed : " + err);
    }
  };

  CXAApplication.Goto = function (url) {
    var virtualFolder = document.getElementsByName("_SiteVirtualFolder");
    if (virtualFolder.length > 0) {
      var virtualFolderString = virtualFolder[0].value;
      if (virtualFolderString.length > 1) {
        virtualFolderString = virtualFolderString.replace("/", "").replace("/", "");
        url = url.replace("/", "");
        url = "/" + virtualFolderString + "/" + url;
      }
    }
    window.location.href = url;
  };
}));