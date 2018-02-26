$(document).ready(function () {
    //var basket;
    //var miniCart;

    var $miniCartComponent = $('.cxa-minicart-component'),
        $button = $miniCartComponent.find('.top-text');    

    $button.click(function () {        
        if($miniCartComponent.hasClass('open')){
            $miniCartComponent.parent().find('.open').removeClass('open');
        }else {
            $miniCartComponent.parent().find('.open').removeClass('open');
            $miniCartComponent.addClass('open');
        }
    });

});