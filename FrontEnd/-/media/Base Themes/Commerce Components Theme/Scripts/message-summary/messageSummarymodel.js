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

var Message = function(message,data)
{
    return {
        message: message,
        data: data
    }
}

var MessageSummaryViewModel = function () {

    var self = this;

    self.errorMessages = ko.observableArray();
    self.infoMessages = ko.observableArray();
    self.warningMessages = ko.observableArray();

    self.showError = ko.computed(function () { return self.errorMessages().length > 0});
    self.showWarning = ko.computed(function () { return self.warningMessages().length > 0 });
    self.showInfo = ko.computed(function () { return self.infoMessages().length > 0 });

    self.showComponent = ko.computed(function () {
        bShow = (self.showError() || self.showWarning() || self.showInfo());
        return bShow;
    })

    self.clear = function () {
        self.errorMessages.removeAll();
        self.infoMessages.removeAll();
        self.warningMessages.removeAll();
    };
};





