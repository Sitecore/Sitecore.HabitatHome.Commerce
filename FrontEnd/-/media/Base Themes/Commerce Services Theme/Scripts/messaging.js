// -----------------------------------------------------------------------
// <copyright file="cxa.common.messaging.js" company="Sitecore Corporation">
// Copyright (c) Sitecore Corporation 1999-2018
// </copyright>
// <summary>Provides a common tool</summary>
// -----------------------------------------------------------------------
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
  var instance;

  if (typeof define === 'function' && define.amd) {
    // use AMD define funtion to support AMD modules if in use
    define('CXA/Common/MessageContext', ['exports'], factory);
  } else if (typeof exports === 'object') {
    // to support CommonJS
    factory(exports);
  }

  function createContext() {
    var context = {
      Handlers: {}, // repsoitory to store message handlers
      LastHandlerId: -1, // last used handler Id, initialized to -1
      StandardChannels: { Error: "standard.error", Warning: "standard.warning", Info: "standard.info", Command: "standard.command" },
      StandardCommands: { ClearAll: "command.clearAll", ClearErrors: "command.clearErrors", ClearWarnings: "command.clearWarnings", ClearInfo: "command.clearInfo" }
    };
    return context;
  }

  function getContext() {
    if (!instance) {
      instance = createContext();
    }
    return instance;
  }
  // browser global variable
  var MessageContext = getContext();
  root.MessageContext = MessageContext;
  factory(MessageContext);
}(this, function (MessageContext) {
  'use strict';

  function channelExists(channel) {
    return MessageContext.Handlers.hasOwnProperty(channel);
  }

  function registerChannel(channel) {
    if (!MessageContext.Handlers.hasOwnProperty(channel)) {
      MessageContext.Handlers[channel] = {};
    }
  }

  function hasHandlers(channel) {
    if (!channelExists) { return false; }
    var handlers = MessageContext.Handlers[channel];
    var handlerId;
    for (handlerId in handlers) {
      if (handlers.hasOwnProperty(handlerId)) {
        return true;
      }
    }
    return false;
  }

  function notifyHandler(source, handler, message) {
    handler(source, message);
  }

  function dispatch(source, channel, message) {
    var handlers = MessageContext.Handlers[channel];
    if (channelExists(channel)) {
      for (var handlerId in handlers) {
        if (handlers.hasOwnProperty(handlerId)) {
          notifyHandler(source, handlers[handlerId], message);
        }
      }
    }
  }

  // -------------------- Service Public Functions ---------------------------------------------------------------------

  MessageContext.PublishMessage = function (source, channel, message) {
    if (!hasHandlers(channel)) { return false; }
    var dispatchFunction = dispatch(source, channel, message);
    // run asyncronously
    setTimeout(dispatchFunction, 0);
    return true;
  };

  MessageContext.PublishCommand = function (source, command) {
    if (!hasHandlers(MessageContext.StandardChannels.Command)) { return false; }
    var dispatchFunction = dispatch(source, MessageContext.StandardChannels.Command, command);
    // run asyncronously
    setTimeout(dispatchFunction, 0);
    return true;
  };

  MessageContext.PublishError = function (source, message, data) {
    if (!hasHandlers(MessageContext.StandardChannels.Error)) { return false; }
    var dispatchFunction = dispatch(source, MessageContext.StandardChannels.Error, message);
    // run asyncronously
    setTimeout(dispatchFunction, 0);
    return true;
  };

  MessageContext.PublishWarning = function (source, message, data) {
    if (!hasHandlers(MessageContext.StandardChannels.Warning)) { return false; }
    var dispatchFunction = dispatch(source, MessageContext.StandardChannels.Warning, message, data);
    // run asyncronously
    setTimeout(dispatchFunction, 0);
    return true;
  };

  MessageContext.PublishInfo = function (source, message, data) {
    if (!hasHandlers(MessageContext.StandardChannels.Info)) { return false; }
    var dispatchFunction = dispatch(source, MessageContext.StandardChannels.Info, message, data);
    // run asyncronously
    setTimeout(dispatchFunction, 0);
    return true;
  };

  MessageContext.SubscribeMessageHandler = function (channel, handler) {
    if (typeof handler !== 'function') {
      return false;
    }

    registerChannel(channel);
    var newHandlerId = 'h_' + String(++MessageContext.LastHandlerId);
    MessageContext.Handlers[channel][newHandlerId] = handler;

    return newHandlerId;
  };

  MessageContext.SubscribeErrorHandler = function (handler) {
    if (typeof handler !== 'function') {
      return false;
    }
    var channel = MessageContext.StandardChannels.Error;
    registerChannel(channel);
    var newHandlerId = 'h_' + String(++MessageContext.LastHandlerId);
    MessageContext.Handlers[channel][newHandlerId] = handler;

    return newHandlerId;
  };

  MessageContext.SubscribeInfoHandler = function (handler) {
    if (typeof handler !== 'function') {
      return false;
    }
    var channel = MessageContext.StandardChannels.Info;
    registerChannel(channel);
    var newHandlerId = 'h_' + String(++MessageContext.LastHandlerId);
    MessageContext.Handlers[channel][newHandlerId] = handler;

    return newHandlerId;
  };

  MessageContext.SubscribeWarningHandler = function (handler) {
    if (typeof handler !== 'function') {
      return false;
    }
    var channel = MessageContext.StandardChannels.Warning;
    registerChannel(channel);
    var newHandlerId = 'h_' + String(++MessageContext.LastHandlerId);
    MessageContext.Handlers[channel][newHandlerId] = handler;

    return newHandlerId;
  };

  MessageContext.SubscribeCommandHandler = function (handler) {
    if (typeof handler !== 'function') {
      return false;
    }
    var channel = MessageContext.StandardChannels.Command;
    registerChannel(channel);
    var newHandlerId = 'h_' + String(++MessageContext.LastHandlerId);
    MessageContext.Handlers[channel][newHandlerId] = handler;

    return newHandlerId;
  };

  MessageContext.UnSubscribeHandler = function (handlerId) {
    for (var channel in MessageContext.Handlers) {
      if (MessageContext.Handlers.hasOwnProperty(channel)) {
        var channelHandlers = MessageContext.Handlers[channel];
        if (channelHandlers[handlerId]) {
          delete channelHandlers[handlerId];
          return handlerId;
        }
      }
    }

    return null;
  };

  // sends a clearAll command to all components that have subscribed to MessageContext.StandardChannels.Command channel
  MessageContext.ClearAllMessages = function () {
    MessageContext.PublishCommand(this, MessageContext.StandardCommands.ClearAll);
  };
}));