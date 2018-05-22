function InitializeGiftCardWidget() {
    $('.get-balance-btn').click(function () {
        $('.gift-card-balance').show();        
    });
}


$(document).ready(function () {
    InitializeGiftCardWidget();
});