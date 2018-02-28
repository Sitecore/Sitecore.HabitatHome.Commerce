$(function(){
    var categoryFacet = $('.cxa-productfacets-component');
    categoryFacet.find(".facet-title").click(
        function(){
            var clicks = $(this).data('clicks');
            var $obj = $(this);
            var $list = $obj.next().filter("div[class$=-list]");
            if (!clicks)
            {
                $list.fadeTo("fast", 0);
                $list.hide();
                $(this).data('clicks',1);
            }
            else{
                $list.show();
                $list.fadeTo("fast", 1);
                $(this).removeData();
            }
        }
    );    
});