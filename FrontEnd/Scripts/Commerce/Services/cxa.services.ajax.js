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

    // browser global variable
    var AjaxService = {};
    root.AjaxService = AjaxService;
    factory(AjaxService);

}(this, function (AjaxService) {
    'use strict';

    //Hack >>
    //When debugging is on, Sitecore is returning extra information, we need to strip it from the JSON string
    var jsonParseRef = JSON.parse;

    JSON.parse = function (data) {
        var debugIndex = data.indexOf("<div title=\"Click here to view the profile.\"");

        if (debugIndex > 0) {
            data = data.substring(debugIndex, 0, debugIndex);
        }

        return jsonParseRef(data);
    }
    //Hack <<

    function InitializeService() {
        //bind error handler to all ajax events on the page
        $(document).ajaxSuccess(function (event, xhr, options, data) {
            handleSuccess(data);
        });
        $(document).ajaxError(function (event, xhr, options, error) {
            handleFailure(event, xhr, options, error);
        });
        $(document).ajaxSend(function (event, xhr, options, data) {
            var virtualFolder = document.getElementsByName("_SiteVirtualFolder");
            if (virtualFolder.length > 0) {
                var url = options.url;
                var virtualFolderString = virtualFolder[0].value;
                if (virtualFolderString.length > 1) {
                    virtualFolderString = virtualFolderString.replace("/", "").replace("/", "");
                    url = $("<a href='" + url + "'>").prop("pathname");
                    url = url.replace(virtualFolderString, "");
                    url = "/" + virtualFolderString + "/" + url;
                    url = url.replace("//", "/").replace("//", "/");
                    options.url = url;
                }
            }
        });
    }

    function retrieveAntiForgeryToken() {
        return $('#_CRSFform input[name=__RequestVerificationToken]').val();
    }

    function addAntiForgeryToken(data) {
        if (typeof data !== 'string')
            data.__RequestVerificationToken = retrieveAntiForgeryToken();
        return data;
    };

    function handleSuccess(data) {
        //check if there are any errors/warnings/infos at the data and publish them if any 
        if (data) {
            if (data.HasErrors) {
                $(data.Errors).each(function () {
                    MessageContext.PublishError(null, "Error: " + this, data);
                });
            }

            if (data.HasInfo) {
                $(data.Info).each(function () {
                    MessageContext.PublishInfo(null, "Info: " + this, data);
                });
            }

            if (data.HasWarnings) {
                $(data.Warnings).each(function () {
                    MessageContext.PublishWarning(null, "Warning: " + this, data);
                });
            }
        }
    }

    function handleFailure(event, xhr, options, error) {
        //if ajax request fails, publish an error message
        MessageContext.PublishError(event, error, xhr);
    }

    function ajaxCall(callType, url, data, callback, sender) {
        if (!data)
            data = {};
        $.ajax({
            global: true,
            type: callType,
            url: url,
            headers: { "__RequestVerificationToken": retrieveAntiForgeryToken() },
            cache: false,
            data: addAntiForgeryToken(data),
            datatype: "json",
            success: function (data) {
                if (callback != null) {
                    callback(data, true, sender);
                }
            },
            error: function (xhr, status, message) {
                console.log(xhr);
                if (callback != null) {
                    callback(null, false, sender);
                }
            }
        });
    }

    InitializeService();

    AjaxService.Get = function (url, data, callback, sender) {
        var virtualFolder = document.getElementsByName("_SiteVirtualFolder");
        if (virtualFolder.length > 0) {
            var virtualFolderString = virtualFolder[0].value;
            if (virtualFolderString.length > 1) {
                virtualFolderString = virtualFolderString.replace("/", "").replace("/", "");
                url = url.replace("/", "");
                url = "/" + virtualFolderString + "/" + url;
            }
        }

        ajaxCall("GET", url, data, callback, sender);
    };

    AjaxService.Post = function (url, data, callback, sender) {
        var virtualFolder = document.getElementsByName("_SiteVirtualFolder");
        if (virtualFolder.length > 0) {
            var virtualFolderString = virtualFolder[0].value;
            if (virtualFolderString.length > 1) {
                virtualFolderString = virtualFolderString.replace("/", "").replace("/", "");
                url = url.replace("/", "");
                url = "/" + virtualFolderString + "/" + url;
            }
        }

        ajaxCall("POST", url, data, callback, sender);
    }
}));