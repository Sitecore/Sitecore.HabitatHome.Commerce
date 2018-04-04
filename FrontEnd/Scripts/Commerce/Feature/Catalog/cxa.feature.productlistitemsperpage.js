var queryStringParamerterSort = "s";
var queryStringParamerterSortDirection = "sd";
var queryStringParamerterSortDirectionAsc = "asc";
var queryStringParamerterSortDirectionAscShort = "+";
var queryStringParamerterSortDirectionDesc = "desc";
var queryStringParamerterPage = "pg";
var queryStringParamerterPageSize = "ps";

$(document).ready(function () {
    $(".change-pagesize", '.cxa-productlistitemsperpage-component').change(function () {
        var val = $(this).find("option:selected").attr("value");

        if (val != null && val != "") {
            var url = new Uri(window.location.href)
                .deleteQueryParam(queryStringParamerterPageSize)
                .addQueryParam(queryStringParamerterPageSize, val)
                .deleteQueryParam(queryStringParamerterPage)
                .toString();

            window.location.href = url;
        }
        else {
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
    });

});
