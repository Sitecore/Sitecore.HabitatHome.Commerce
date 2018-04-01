function GetGiftCardBalance(ServiceRequest) {

    var url = "/api/cxa/GiftCardBalance/GetBalance?cardId=" + ServiceRequest.CardID;
    var requestData = JSON.stringify(ServiceRequest);
    $('.loader').show();
    var ajaxRequest = $.ajax({
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        url: url,
        //data: { 'userLatitude': ServiceRequest.Latitude, 'userLongitude': ServiceRequest.Longitude },
        success: function (data) {
            $('.loader').hide();
        },
        error: function (x, y, z) {
        }
    });
    return ajaxRequest;
}

function InitializeGiftCardWidget() {
    $('.get-balance-btn').click(function () { 
        $('.gift-card-balance').remove();
        var ServiceRequest = new Object();
        ServiceRequest.CardID = $('#giftCardPayment_PaymentMethodID').val();
        var promise = GetGiftCardBalance(ServiceRequest).done(function (result) {
            if (result.Errors != null & result.Errors != "undefined") {
               
                console.log(result.Errors[0]);
            }
            else {
                var balanceHtml = '<div class="gift-card-balance"><div class="payment-total">Balance: <span>' + result.Data + '</span></div></div>';
                $(balanceHtml).insertAfter('.apply-gif-card-balance');
            }
        })
    });
}


$(document).ready(function () {
    InitializeGiftCardWidget();
});