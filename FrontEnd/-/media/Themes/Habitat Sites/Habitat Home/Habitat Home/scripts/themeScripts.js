XA.component.themeScripts = (function ($) {
    var $megadrop = $('.megadrop');

    $('header .search-box').on("click", function (e) {
        var target = $(e.target);
        if (target.hasClass('search-box-input')) {
            return false;
        }
        $(this).toggleClass('open');
        $(".megadrop.open").removeClass("open");
        $(".language-selector.open").removeClass("open");
    });
    
    $megadrop.find('.megadrop-icon').on("click", function () {        
        var li = $megadrop.find('.megadrop-nav  nav > ul > li');
        $(this).closest(".megadrop").toggleClass("open");
        
        if (!li.hasClass("active") && $(window).width() > 768) {
            $(".megadrop .megadrop-nav ul li.first").addClass("active");
        }
        $(".search-box.open").removeClass("open");
        $(".language-selector.open").removeClass("open");
    });    

    /*
    Change button behavior for mobile screensizes
     */
    $(".megadrop .megadrop-navbar ul li div.megadrop-secondary-title-link, .megadrop .megadrop-secondary-title-link").on("mouseover", function () {
        if($(window).width() > 768) {
            $(this).closest('ul').find('li').removeClass('active');
            $(this).closest('li').addClass('active');
        }
    });

    $(".megadrop .megadrop-navbar ul li div.megadrop-secondary-title-link a, .megadrop .megadrop-secondary-title-link a").on("click", function (e) {        
        var $li = $(this).closest('li');

        if($(window).width() < 768) {
            if($li.hasClass('active')){
                $li.removeClass('active');
            }else {
                $(this).closest('ul').find('li').removeClass('active');        
                $li.addClass('active');
            }            
            e.preventDefault();
            return;
        }        

        $(this).closest('ul').find('li').removeClass('active');
        $li.addClass('active');
    });

    $(".megadrop .megadrop-navbar ul li div.megadrop-secondary-title-link a").on("touchstart", function (e) {
        e.preventDefault();
        $(this).closest('ul').find('li').removeClass('active');
        $(this).closest('li').addClass('active');
    });
    
    $('button.sidebar-closed[data-toggle="sidebar"]').on("click", function () {
        var btn = $(this);
        $('html').addClass('show-sidebar-right');
        btn.hide();
        btn.siblings('button').show();
    });

    $('button.sidebar-opened[data-toggle="sidebar"]').on('click', function (e) {
        e.preventDefault();
        var selector = $(this).parent('div.btn-group-vertical');
        selector.find('button').hide();
        selector.find('button.sidebar-closed[data-toggle="sidebar"]').show();
        $('html').removeClass('show-sidebar-right');
    });
    /*
    $('input[type="radio"]').on('change', function(e) {
        $('div.content-finder form label').removeClass("selected");
        $('div.content-finder form input[type="radio"]:checked').parent('label').addClass("selected");
    })*/
}(jQuery, document));

