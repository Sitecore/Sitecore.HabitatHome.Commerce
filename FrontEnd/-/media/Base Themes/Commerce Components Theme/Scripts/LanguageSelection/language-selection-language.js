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

  // browser global variable
  root.LanguageSelector = factory;
}(this, function (element) {
  'use strict';
  var component = new Component(element);
  component.Name = "CXA/Feature/LanguageSelection";

  component.SwitchLangauge = function (selected, current) {
    AjaxService.Post(
      "api/cxa/languageselection/setcurrentlanguage",
      { Culture: selected },
      function (data, success, sender) {
        var currentLanguageUrlSegment = '/' + current;
        var selectedLanguageUrlSegment = '/' + selected;
        var url = new Uri(window.location.href);
        var urlPathSegments = url.path().replace('/', '')
          .split('/');
        var regex = new RegExp(currentLanguageUrlSegment, 'i');

        if (urlPathSegments[0].toLowerCase() === current.toLowerCase()) {
          var pathName = url.path().replace(regex, selectedLanguageUrlSegment);
          url.setPath(pathName);
        } else {
          url.setPath(selectedLanguageUrlSegment + url.path());
        }
        url.deleteQueryParam("sc_lang");
        window.location.href = url.toString();
      }
    );
  };

  component.Init = function () {
    $(component.RootElement).find(".language-item")
      .each(function () {
        var selected = $(this).data("itemLanguage");
        var current = $(this).data("currentLanguage");
        $(this).click(function () {
          component.SwitchLangauge(selected, current);
        });
      });

    component.Visual.Enable();
  };

  return component;
}));