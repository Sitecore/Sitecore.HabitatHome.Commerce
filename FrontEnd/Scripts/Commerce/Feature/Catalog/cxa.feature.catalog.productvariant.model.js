// ----- VariantDefinition -----

var VariantValue = function (propertyName, propertyValue) {
    self = this;

    self.propertyName = propertyName;
    self.propertyValue = propertyValue;
}

var VariantCombo = function (variantId, variantValueList, savingsMessage, listPrice, adjustedPrice, isOnSale) {
    self = this;

    self.variantId = variantId;
    self.variantValueList = variantValueList;
    self.savingsMessage = savingsMessage;
    self.listPrice = listPrice;
    self.adjustedPrice = adjustedPrice;
    self.isOnSale = (isOnSale === "true");
}

var VariantDefinition = function () {
    self = this;

    self.variantPropertyList = [];
    self.variantComboList = [];

    self.readVariantInformation = function (variants) {
        $.each(variants, function () {
            var variantName = this.className;
            variantName = variantName.replace("product-variant-name ", "").substr(8);

            self.variantPropertyList.push(variantName);
        })

        var outerSelf = self;

        var variantData = $('.valid-variant-combo').find('.variant-data');
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
        if (self.variantPropertyList.length) {
            self.variantPropertyList.forEach(function (variantProperty) {
                var i = 0;
                var value = null;
                if ($('#variant' + variantProperty).length) {
                    value = $('#variant' + variantProperty).val();
                }
                else {
                    return null;
                }

                variantValues[variantProperty] = value;
            });
        }

        var selectedItems = [];
        var comboList = self.variantComboList;
        for (var key in variantValues) {
            for (var comboIndex in comboList) {
                for (var variantValueIndex in comboList[comboIndex].variantValueList) {
                    if (comboList[comboIndex].variantValueList[variantValueIndex].propertyName == key) {
                        if (comboList[comboIndex].variantValueList[variantValueIndex].propertyValue == variantValues[key]) {
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
}

var varianDefinition = null;
