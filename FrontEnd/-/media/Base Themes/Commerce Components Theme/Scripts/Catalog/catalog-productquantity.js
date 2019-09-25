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

(function (root, $, factory) {
  factory($);
}(this, window.jQuery, function ($) {
  var parent = $('.quantity-input');
  if (parent.length > 0) {
    var increaseButton = parent.find('.increase')[0];
    var decreaseButton = parent.find('.decrease')[0];
    var quantityInput = parent.find('.add-to-cart-qty-input')[0];
    var max = parseInt($(quantityInput).attr('max'));
    var min = parseInt($(quantityInput).attr('min'));

    var setButtonState = function (currentValue) {
      $(increaseButton).removeAttr('disabled');
      $(decreaseButton).removeAttr('disabled');
      if (currentValue === max) {
        $(increaseButton).attr('disabled', 'disabled');
      }
      if (currentValue === min) {
        $(decreaseButton).attr('disabled', 'disabled');
      }
    };

    $(quantityInput).on('change keyup paste keypress set input', function () {
      var currentValue = parseInt(quantityInput.value);
      setButtonState(currentValue);
    });

    $(increaseButton).on('click', function (e) {
      e.preventDefault();
      var currentValue = parseInt(quantityInput.value);
      if (currentValue < max) {
        currentValue = currentValue + 1;
        $(quantityInput).attr('value', currentValue);
        setButtonState(currentValue);
      }
    });

    $(decreaseButton).on('click', function (e) {
      e.preventDefault();
      var currentValue = parseInt(quantityInput.value);
      if (currentValue > min) {
        currentValue = currentValue - 1;
        $(quantityInput).attr('value', currentValue);
        setButtonState(currentValue);
      }
    });

    $(document).ready(function () {
      var currentValue = parseInt(quantityInput.value);
      setButtonState(currentValue);
    });
  }
}));