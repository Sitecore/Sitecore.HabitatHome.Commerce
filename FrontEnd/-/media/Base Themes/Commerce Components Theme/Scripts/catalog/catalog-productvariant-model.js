// ----- VariantDefinition -----

var ProductVariantViewModel = function (rootElement) {
    var self = this;
    self.rootElement = rootElement;
    self.variantPropertyList = [];
    self.variantComboList = [];

    self.VariantSelectionChanged = function () {
        ProductSelectionContext.SelectedProductValid(this, null);

        MessageContext.ClearAllMessages();

        var foundVariant = self.FindBasedOnCurrentSelection();
        if (foundVariant != null) {
            var productId = $(self.rootElement).find('#variant-component-product-id').val();
            var data = {};
            data.productId = productId;
            data.sourceProductVarianElement = self.rootElement;

            ProductPriceContext.SetPrice(this, foundVariant.listPrice, foundVariant.adjustedPrice, foundVariant.isOnSale, foundVariant.savingsMessage, data);

            var catalogName = $(self.rootElement).find('#variant-component-product-catalog').val();
            data.selectedVariantsValues = self.GetSelectedVariantsVaues();

            ProductSelectionContext.SelectedProduct(this, catalogName, productId, foundVariant.variantId, data);
        }
        else {
            var message = $(self.rootElement).find('.invalid-variant').text();
            MessageContext.PublishError("productinformation", message);
            ProductSelectionContext.SelectedProductInvalid(this, null);
        }
    }

    self.Initialize = function () {

        // If variants exist on the page, let's load their definition.
        var variants = $(self.rootElement).find(".component-content [class*='Variant_']");
        if ($(variants).length) {
            self.ReadVariantInformation(variants);
            self.VariantSelectionChanged();
            return;
        }
        else {
            // Handles catalogs with empty variant values.  Required when a variant ID is always required.
            var variantsII = $(self.rootElement).find('.valid-variant-combo [class^="variant-value"]');
            if ($(variantsII).length) {
                self.ReadVariantInformation(variants);
                self.VariantSelectionChanged();
                return;
            }
        }

        // if the 
        var savingmessageid = $(self.rootElement).find('#savingsmessage');
        if ($(savingmessageid).length) {
            var listprice = $(self.rootElement).find('#listprice');
            var adjustedprice = $(self.rootElement).find('#asjustedprice');
            var isonsale = $(self.rootElement).find('#isonsale');
            ProductPriceContext.SetPrice(this, listprice.val(), adjustedprice.val(), isonsale.val(), savingmessageid.val());
        }

        var catalogName = $(self.rootElement).find('#variant-component-product-catalog').val();
        var productId = $(self.rootElement).find('#variant-component-product-id').val();

        ProductSelectionContext.SelectedProduct(this, catalogName, productId, null, null);

    }

    self.ReadVariantInformation = function (variants) {
        $.each(variants,
            function () {
                var variantName = this.className;
                variantName = variantName.replace("product-variant-name ", "").substr(8);

                self.variantPropertyList.push(variantName);
            });

        var outerSelf = self;

        var variantData = $(self.rootElement).find('.valid-variant-combo').find('.variant-data');
        if ($(variantData).length) {
            $(variantData).each(function () {
                var variantValueList = [];
                var variantId = $(this).attr('id');
                var listPrice = $(this).attr('listprice');
                var adjustedPrice = $(this).attr('adjustedPrice');
                var isOnSale = $(this).attr('isonsale');
                $(this).find('.variant-value').find('input').each(function () {
                    var propertyName = $(this).attr('id');
                    var propertyValue = $(this).val();

                    variantValueList.push(new VariantValue(propertyName, propertyValue));
                });

                var savingsMessage = null;
                var savingsMessageElement = $(this).find('#savings-message');
                if ($(savingsMessageElement).length) {
                    savingsMessage = $(savingsMessageElement).val();
                }
                outerSelf.variantComboList.push(new VariantCombo(variantId, variantValueList, savingsMessage, listPrice, adjustedPrice, isOnSale));
            });
        }
    };

    self.FindBasedOnCurrentSelection = function () {
        self = this;

        var variantValues = {};
        variantValues = self.GetSelectedVariantsVaues();

        var selectedItems = [];
        var comboList = self.variantComboList;
        for (var key in variantValues) {
            for (var comboIndex in comboList) {
                for (var variantValueIndex in comboList[comboIndex].variantValueList) {
                    if (comboList[comboIndex].variantValueList[variantValueIndex].propertyName === key) {
                        if (comboList[comboIndex].variantValueList[variantValueIndex].propertyValue === variantValues[key]) {
                            selectedItems.push(comboList[comboIndex]);
                            break;
                        }
                    }
                }
            }
            comboList = selectedItems.slice();
            selectedItems = [];
        }

        if (comboList.length === 1) {
            return comboList[0];
        }

        return null;
    };

    self.GetSelectedVariantsVaues = function () {
        var variantValues = {};

        if (self.variantPropertyList.length) {
            self.variantPropertyList.forEach(function (variantProperty) {
                var i = 0;
                var value = null;
                if ($(self.rootElement).find('#variant' + variantProperty).length) {
                    value = $(self.rootElement).find('#variant' + variantProperty).val();
                    variantValues[variantProperty] = value;
                }
            });
        }

        return variantValues;
    }
}

var VariantValue = function (propertyName, propertyValue) {
    self = this;

    self.propertyName = propertyName;
    self.propertyValue = propertyValue;
}

function VariantCombo(variantId, variantValueList, savingsMessage, listPrice, adjustedPrice, isOnSale) {
    self = this;

    self.variantId = variantId;
    self.variantValueList = variantValueList;
    self.savingsMessage = savingsMessage;
    self.listPrice = listPrice;
    self.adjustedPrice = adjustedPrice;
    self.isOnSale = (isOnSale === "true");
}


