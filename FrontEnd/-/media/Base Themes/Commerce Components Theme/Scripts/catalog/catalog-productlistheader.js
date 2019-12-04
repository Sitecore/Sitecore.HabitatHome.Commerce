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

var queryStringParamerterSort = "s";
var queryStringParamerterSortDirection = "sd";
var queryStringParamerterSortDirectionAsc = "asc";
var queryStringParamerterSortDirectionAscShort = "+";
var queryStringParamerterSortDirectionDesc = "desc";
var queryStringParamerterPage = "pg";
var queryStringParamerterPageSize = "ps";

function resetUrl() {
  var url = new Uri(window.location.href)
    .deleteQueryParam(queryStringParamerterSort)
    .deleteQueryParam(queryStringParamerterSortDirection)
    .deleteQueryParam(queryStringParamerterPage)
    .deleteQueryParam(queryStringParamerterPageSize)
    .deleteQueryParam(queryStringParameterSiteContentPage)
    .deleteQueryParam(queryStringParameterSiteContentPageSize)
    .toString();

  window.location.href = url;
}

$(document).ready(function () {
  $(".sort-dropdown", '.cxa-productlistheader-component').change(function () {
    var val = $(this).find("option:selected")
      .attr("value");

    if (val) {
      var fieldName = val.substr(0, val.length - 1);
      var direction = val.charAt(val.length - 1) === queryStringParamerterSortDirectionAscShort ? queryStringParamerterSortDirectionAsc : queryStringParamerterSortDirectionDesc;

      AjaxService.Post("/api/cxa/catalog/sortorderapplied", JSON.parse("{\"sortField\":\"" + fieldName + "\", \"sortDirection\":\"" + direction + "\"}"), function (data, success, sender) {
        var url = new Uri(window.location.href)
          .deleteQueryParam(queryStringParamerterSort)
          .deleteQueryParam(queryStringParamerterSortDirection)
          .addQueryParam(queryStringParamerterSort, fieldName)
          .addQueryParam(queryStringParamerterSortDirection, direction)
          .deleteQueryParam(queryStringParamerterPage)
          .toString();

        window.location.href = url;
      });
    } else {
      resetUrl();
    }
  });

  $(".change-pagesize", '.cxa-productlistheader-component').change(function () {
    var val = $(this).find("option:selected")
      .attr("value");

    if (val) {
      var url = new Uri(window.location.href)
        .deleteQueryParam(queryStringParamerterPageSize)
        .addQueryParam(queryStringParamerterPageSize, val)
        .deleteQueryParam(queryStringParamerterPage)
        .toString();

      window.location.href = url;
    } else {
      resetUrl();
    }
  });
});