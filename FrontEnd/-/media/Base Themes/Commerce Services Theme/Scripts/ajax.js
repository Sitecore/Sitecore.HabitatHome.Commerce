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

/**
 * Http Queue Class
 * It queue the Http calls and execute them as a batch
 * It remove the duplicated calls and execute it once and share the results for all the callers
 */
var HttpCallsQueue = (function() {
  /**
   * Keep the list of queue items
   */
  let list = {};

  /**
   * Keep the current executed batch
   */
  let currentExecutedBatch;

  /**
   * Execution timeout between each batch
   */
  const EXECUTION_TIMEOUT = 10;

  return (function() {
    /**
     * Enqueue the http call
     * It creates the queue item key from the url and it's data to make sure the call is unique
     * For each new unique call, it creates a object propery and add the call details to an array
     * Any subsequent calls that has a same data & URL, will add to the same array otherwise
     * the new object property will create
     * @param httpAction is the callback will happen when the execute the queue
     * @param callType is the call type
     * @param url is the http url
     * @param data is the post data
     * @param callback is the callback
     * @param sender is the sender
     * @example
     * {
     *      ..,
     *      UNIQUE_KEY: [{callType, url, data, callback, sender}, {callType, url, data, callback, sender},..]
     * }
     */
    function enqueueCall(httpAction, callType, url, data, callback, sender) {
      clearTimeout(currentExecutedBatch);
      updateList(callType, url, data, callback, sender);
      executeQueue(httpAction);
    }

    /**
     * Update the list item
     */
    function updateList(callType, url, data, callback, sender) {
      const urlKey = createObjectKey(url, data);
      let listItem = list[urlKey];
      listItem = !Array.isArray(listItem) ? [] : listItem;
      listItem.push({
        callType: callType,
        url: url,
        data: data,
        callback: callback,
        sender: sender
      });
      list[urlKey] = listItem;
    }

    /**
     * Dequeue the item
     * @param urlKey is the uniqueue key to find the queue item
     */
    function dequeueCall(urlKey) {
      delete list[urlKey];
    }

    /**
     * Create a key from the url and it's data
     * It needs to make sure the key is uniqueue
     * @param url is the http call url
     * @param data is the http post data
     */
    function createObjectKey(url, data) {
      const dataJson = JSON.stringify(data);

      return url
        ? filterCharacters(url) + filterCharacters(JSON.stringify(dataJson))
        : url;
    }

    function filterCharacters(dataString) {
      return dataString.replace(/[^\w\s]/gi, '');
    }

    /**
     * Run through the queue and do a http call for each queue item
     * Each queue item has a collection of same/duplicated calls
     * We only run the call once and pass the array of callbacks to the specified action
     * When we run the action, the call will dequeue
     * @param action is the action that is run when a call need to happen. e.g http post call
     */
    function executeQueue(action) {
      if (list) {
        currentExecutedBatch = setTimeout(function() {
          Object.keys(list).forEach(function(key) {
            const calls = list[key];
            let callbacks = [];
            let firstCall = calls[0];

            calls.forEach(function(singleCall) {
              callbacks.push(singleCall.callback);
            });
            action(
              firstCall.callType,
              firstCall.url,
              firstCall.data,
              callbacks,
              firstCall.sender
            );
            dequeueCall(key);
          });
        }, EXECUTION_TIMEOUT);
      }
    }

    /**
     * Export the functions for external usage
     */
    return {
      enqueueCall: enqueueCall
    };
  })();
})(
  (function(root, factory) {
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
  })(this, function(AjaxService) {
    'use strict';

    //Hack >>
    //When debugging is on, Sitecore is returning extra information, we need to strip it from the JSON string
    var jsonParseRef = JSON.parse;

    JSON.parse = function(data) {
      var debugIndex = data.indexOf(
        '<div title="Click here to view the profile."'
      );

      if (debugIndex > 0) {
        data = data.substring(debugIndex, 0, debugIndex);
      }

      return jsonParseRef(data);
    };

    function injectVirtualFolder(url) {
      var virtualFolder = document.getElementsByName('_SiteVirtualFolder');

      if (virtualFolder.length <= 0) {
        return url;
      }

      var virtualFolderString = virtualFolder[0].value;
      if (virtualFolderString.length <= 1) {
        return url;
      }

      var $url = $("<a href='" + url + "'>");
      var host = $url.prop('host');

      // do not modify if link is external
      if (host != location.host) {
        return url;
      }

      virtualFolderString = virtualFolderString
        .replace('/', '')
        .replace('/', '');
      var pathname = $url.prop('pathname');
      pathname = pathname.replace(virtualFolderString, '');
      pathname = '/' + virtualFolderString + '/' + pathname;
      pathname = pathname.replace('//', '/').replace('//', '/');
      $url.prop('pathname', pathname);

      return $url.prop('href');
    }

    function InitializeService() {
      //bind error handler to all ajax events on the page
      $(document).ajaxSuccess(function(event, xhr, options, data) {
        handleSuccess(data);
      });
      $(document).ajaxError(function(event, xhr, options, error) {
        handleFailure(event, xhr, options, error);
      });
    }

    function retrieveAntiForgeryToken() {
      return $('#_CRSFform input[name=__RequestVerificationToken]').val();
    }

    function addAntiForgeryToken(data) {
      if (typeof data !== 'string')
        data.__RequestVerificationToken = retrieveAntiForgeryToken();
      return data;
    }

    function handleSuccess(data) {
      //check if there are any errors/warnings/infos at the data and publish them if any
      if (data) {
        if (data.HasErrors) {
          $(data.Errors).each(function() {
            MessageContext.PublishError(null, 'Error: ' + this, data);
          });
        }

        if (data.HasInfo) {
          $(data.Info).each(function() {
            MessageContext.PublishInfo(null, 'Info: ' + this, data);
          });
        }

        if (data.HasWarnings) {
          $(data.Warnings).each(function() {
            MessageContext.PublishWarning(null, 'Warning: ' + this, data);
          });
        }
      }
    }

    function handleFailure(event, xhr, options, error) {
      //if ajax request fails, publish an error message
      MessageContext.PublishError(event, error, xhr);
    }

    function ajaxCall(callType, url, data, callback, sender) {
      HttpCallsQueue.enqueueCall(
        httpAction,
        callType,
        url,
        data,
        callback,
        sender
      );
    }

    function httpAction(callType, url, data, callbacks, sender) {
      data = !data ? {} : data;

      $.ajax({
        global: true,
        type: callType,
        url: url,
        headers: { __RequestVerificationToken: retrieveAntiForgeryToken() },
        cache: false,
        data: addAntiForgeryToken(data),
        datatype: 'json',
        success: function(data) {
          if (data) {
            callbacks.forEach(function(callback) {
              callback(data, true, sender);
            });
          }
        },
        error: function(xhr, status, message) {
          if (data) {
            callbacks.forEach(function(callback) {
              callback(null, false, sender);
            });
          }
        }
      });
    }

    function getCurrentPageExtension() {
      var pageExtensiponWithQueryString = window.location.href.split('.').pop();
      if (
        pageExtensiponWithQueryString &&
        pageExtensiponWithQueryString.length >= 4
      ) {
        var pageExtension = pageExtensiponWithQueryString.substr(0, 4);
        return pageExtension;
      }
      return '';
    }
    InitializeService();

    var isCreativeExchangeVersion = getCurrentPageExtension() === 'html';

    AjaxService.Get = function(url, data, callback, sender) {
      if (!isCreativeExchangeVersion) {
        url = injectVirtualFolder(url);
        ajaxCall('GET', url, data, callback, sender);
      }
    };

    AjaxService.Post = function(url, data, callback, sender) {
      if (!isCreativeExchangeVersion) {
        url = injectVirtualFolder(url);
        ajaxCall('POST', url, data, callback, sender);
      }
    };
  })
);
