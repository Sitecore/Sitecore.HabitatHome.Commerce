jQuery(document).ready(function() {
    jQuery('.toggle-nav').click(function(e) {
        jQuery(this).toggleClass('active');
        jQuery('.category-navigation-list').toggleClass('active');
 
        e.preventDefault();
    });
});