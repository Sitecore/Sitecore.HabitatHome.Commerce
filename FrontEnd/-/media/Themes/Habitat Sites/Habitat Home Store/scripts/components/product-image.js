$(function(){
    var list = $('.product-images li');
    list.first().addClass('active');

    list.click(function(e){
        $('.product-images li').removeClass('active');
        e.preventDefault();
        $(this).addClass('active');
        
        if(list.length > 1){
            var imageSrc = $(this).find('a').attr('href');
            $(".product-image").find("img").attr('src', imageSrc);
        }
        
    });
});
