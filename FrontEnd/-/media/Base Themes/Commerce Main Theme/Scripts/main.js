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
$(document).ready(function () {
  CXAApplication.Initialize();
});

function formatCurrency(amount, currencyInfo) {
  var options = {
    precision: currencyInfo.precision || 2,
    decimalSeparator: currencyInfo.decimalSeparator || ',',
    groupSeparator: currencyInfo.groupSeparator || ' ',
    symbolPosition: typeof currencyInfo.symbolPosition === 'undefined' ? 3 : currencyInfo.symbolPosition
  };

  var currencyValue = amount.__ko_proto__ === ko.dependentObservable || amount.__ko_proto__ === ko.observable ? amount() : amount;

  var formatted = parseFloat(currencyValue, 10).toFixed(options.precision);

  var regex = new RegExp('^(\\d+)[^\\d](\\d{' + options.precision + '})$');
  formatted = formatted.replace(regex, '$1' + options.decimalSeparator + '$2');
  formatted = formatted.replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1' + options.groupSeparator);

  if (currencyInfo.symbol && currencyInfo.symbol.length > 0) {
    // Currency Symbol Position - [0: $n] [1: n$] [2: $ n] [3: n $]
    switch (options.symbolPosition) {
    case 0:
      formatted = currencyInfo.symbol + formatted;
      break;
    case 1:
      formatted = formatted + currencyInfo.symbol;
      break;
    case 2:
      formatted = currencyInfo.symbol + ' ' + formatted;
      break;
    case 3:
    default:
      formatted = formatted + ' ' + currencyInfo.symbol;
      break;
    }
  }

  return formatted;
}

if (!String.prototype.format) {
  String.prototype.format = function () {
    var args = arguments;
    return this.replace(/{(\d+)}/g, function (match, number) {
      return typeof args[number] !== 'undefined'
        ? args[number]
        : match
      ;
    });
  };
}

function getCurrentPageExtension() {
  var pageExtensiponWithQueryString = window.location.href.split('.').pop();
  if (pageExtensiponWithQueryString && pageExtensiponWithQueryString.length >= 4) {
    var pageExtension = pageExtensiponWithQueryString.substr(0, 4);
    return pageExtension;
  }
  return '';
}

function getCurrentTenantAndSiteName() {
  var currentTenantAndSite = { Tenant: '', Site: '' };
  var siteRootPath = $("input[name='_SiteRootPath'").val();
  var siteNodes = siteRootPath.split("/");

  currentTenantAndSite.Site = siteNodes[siteNodes.length - 1];
  currentTenantAndSite.Tenant = siteNodes[siteNodes.length - 2];
  return currentTenantAndSite;
}