(function (root, $, factory) {
  factory($)
}(this, window.jQuery, function ($) {
  var parent = $('.quantity-input')
  if (parent.length > 0) {
    var increaseButton = parent.find('.increase')[0]
    var decreaseButton = parent.find('.decrease')[0]
    var quantityInput = parent.find('.add-to-cart-qty-input')[0]
    var max = parseInt($(quantityInput).attr('max'))
    var min = parseInt($(quantityInput).attr('min'))
    var currentValue = parseInt(quantityInput.value)

    $(quantityInput).on('change keyup paste keypress set input', function () {
      var currentValue = parseInt(quantityInput.value)
      $(increaseButton).removeAttr('disabled')
      $(decreaseButton).removeAttr('disabled')
      if (currentValue === max) {
        $(increaseButton).attr('disabled', 'disabled')
      }
      if (currentValue === min) {
        $(decreaseButton).attr('disabled', 'disabled')
      }
    });

    $(increaseButton).on('click', function (e) {
      e.preventDefault()
      var currentValue = parseInt(quantityInput.value)
      if (currentValue < max) {
        currentValue = currentValue + 1
        $(quantityInput).attr('value', currentValue)
        $(increaseButton).removeAttr('disabled')
        $(decreaseButton).removeAttr('disabled')
        if (currentValue === max) {
          $(increaseButton).attr('disabled', 'disabled')
        }
        if (currentValue === min) {
          $(decreaseButton).attr('disabled', 'disabled')
        }
      }
    });

    $(decreaseButton).on('click', function (e) {
      e.preventDefault()
      var currentValue = parseInt(quantityInput.value)
      if (currentValue > min) {
        currentValue = currentValue - 1
        $(quantityInput).attr('value', currentValue)
        $(increaseButton).removeAttr('disabled')
        $(decreaseButton).removeAttr('disabled')
        if (currentValue === max) {
          $(increaseButton).attr('disabled', 'disabled')
        }
        if (currentValue === min) {
          $(decreaseButton).attr('disabled', 'disabled')
        }
      }
    });
    $(document).ready(function () {
      $(increaseButton).removeAttr('disabled')
      $(decreaseButton).removeAttr('disabled')
      if (currentValue === max) {
        $(increaseButton).attr('disabled', 'disabled')
      }
      if (currentValue === min) {
        $(decreaseButton).attr('disabled', 'disabled')
      }
    })
  }
}));
