// ----- VariantDefinition -----

var BundledItemVariantValue = function (propertyName, propertyValue) {
    self = this;

    self.propertyName = propertyName;
    self.propertyValue = propertyValue;
}

var BundledItemVariantProperty = function (name, displayName) {
    self = this;

    self.name = name;
    self.displayName = displayName;
}

var BundledItemVariantCombo = function (variantId, variantValueList, savingsMessage, listPrice, adjustedPrice, isOnSale) {
    self = this;

    self.variantId = variantId;
    self.variantValueList = variantValueList;
    self.savingsMessage = savingsMessage;
    self.listPrice = listPrice;
    self.adjustedPrice = adjustedPrice;
    self.isOnSale = (isOnSale === "true");
}

var BundledItemSelectBox = function (name, displayName, valueList, bundleGroup) {
    self = this;

    self.name = ko.observable(name);
    self.bundleGroup = bundleGroup;
    self.displayName = displayName;
    self.id = bundleGroup.id + '_variant' + self.name();
    self.valueList = ko.observableArray(valueList);
    self.selectedValue = ko.observable();

    self.selectedValue.subscribe(function (selectedValue) {
        self = this;
        self.bundleGroup.variantSelectionChange(this, selectedValue);
    }.bind(this));
}

var BundleVariantDefinition = function (bundleGroup) {
    self = this;

    self.bundleGroup = bundleGroup;
    self.variantPropertyList = [];
    self.variantComboList = [];

    self.selectBoxList = ko.observableArray();

    self.readVariantInformation = function (variants) {
        var hasExtractedVariantProperties = false;
        var outerSelf = this;
        var variantId = "";

        $.each(variants, function () {
            var variantValueList = [];
            variantId = $(this).attr('id');
            var listPrice = $(this).attr('listprice');
            var adjustedPrice = $(this).attr('adjustedPrice');
            var isOnSale = $(this).attr('isonsale');

            $(this).find('.bundle-variant-value').find('input').each(function () {
                var propertyName = $(this).attr('id');
                var propertyValue = $(this).val();
                var propertyDisplayName = $(this).attr('displayName');

                if (propertyValue.length > 0) {
                    variantValueList.push(new BundledItemVariantValue(propertyName, propertyValue));

                    if (!hasExtractedVariantProperties) {
                        outerSelf.variantPropertyList.push(new BundledItemVariantProperty(propertyName, propertyDisplayName));
                    }
                }
            });

            hasExtractedVariantProperties = true;

            var savingsMessage = null;
            var savingsMessageElement = $(this).find('#savings-message');
            if ($(savingsMessageElement).length) {
                savingsMessage = $(savingsMessageElement).val();
            }

            if (variantValueList.length > 0) {
                outerSelf.variantComboList.push(new BundledItemVariantCombo(variantId, variantValueList, savingsMessage, listPrice, adjustedPrice, isOnSale));
            }
            else {
                // This indicates a product with a single empty variant.  We need to keep track of the variant id
                // as it is needed when the initial product selection is signaled.
                outerSelf.bundleGroup.SetSingleVariantProduct(variantId);
            }
        })

        $.each(outerSelf.variantPropertyList, function () {
            var name = this.name.toString();
            var displayName = this.displayName;
            var valueList = [];

            $.each(outerSelf.variantComboList, function () {
                $.each(this.variantValueList, function () {
                    if (this.propertyName === name) {
                        valueList.push(this.propertyValue);
                    }
                });
            });

            $.unique(valueList);

            var selectbox = new BundledItemSelectBox(name, displayName, valueList, outerSelf.bundleGroup);
            outerSelf.selectBoxList().push(selectbox);
        });
    };

    self.FindBasedOnCurrentSelection = function (bundleGroup) {
        self = this;

        var variantValues = {};

        $.each(self.selectBoxList(), function () {
            var propertyName = this.name();
            var value = this.selectedValue();

            variantValues[propertyName] = value;
        });

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

var BundleGroup = function (viewModel, component, bundleGroupElement, id) {
    self = this;

    self.id = id;
    self.component = component;
    self.viewModel = viewModel;
    self.bundleGroupElement = bundleGroupElement;
    self.variantDefinition = "";
    self.displayName = ko.observable("");
    self.isSelectionValid = ko.observable(false);
    self.productId = $(bundleGroupElement).attr('bundle-product-id');
    self.productLink = $(bundleGroupElement).attr('bundle-product-link');
    self.quantity = $(bundleGroupElement).attr('bundle-product-quantity');
    self.variantId = "";
    self.hasSingleSelectionsOnly = ko.observable(false);
    self.setArrowClass = function () {
        return this.hasSingleSelectionsOnly() ? "" : "collapse-arrow";
    }

    self.readVariantInformation = function () {
        outerSelf = this;

        var dislpayNameElement = $(self.bundleGroupElement).find("[class='bundle-displayname']");
        if ($(dislpayNameElement).length == 1) {
            outerSelf.displayName($(dislpayNameElement).val());
        }

        var variants = $(self.bundleGroupElement).find("[class='bundle-variant-data']");
        if ($(variants).length) {
            outerSelf.variantDefinition = new BundleVariantDefinition(outerSelf);
            outerSelf.variantDefinition.readVariantInformation(variants);

            if (outerSelf.variantDefinition.selectBoxList().length > 0) {
                var hasOnlySingleSelection = true;
                $.each(outerSelf.variantDefinition.selectBoxList(), function () {
                    hasOnlySingleSelection = this.valueList().length == 1;
                    if (!hasOnlySingleSelection) {
                        return false;
                    }
                });

                outerSelf.hasSingleSelectionsOnly(hasOnlySingleSelection);
            }
        }
        else {
            outerSelf.SetNoVariantProduct();
        }
    }

    self.SetSingleVariantProduct = function (variantId) {
        this.variantId = variantId;
        this.isSelectionValid(true);
    }

    self.SetNoVariantProduct = function () {
        this.variantId = "";
        this.isSelectionValid(true);
    }

    self.variantSelectionChange = function (selectBox, selectedValue) {
        currentBundleGroup = this;

        MessageContext.ClearAllMessages();

        if (currentBundleGroup.variantDefinition != null) {
            var foundVariant = currentBundleGroup.variantDefinition.FindBasedOnCurrentSelection(currentBundleGroup);
            if (foundVariant != null) {
                currentBundleGroup.isSelectionValid(true);
                currentBundleGroup.variantId = foundVariant.variantId;

                currentBundleGroup.viewModel.checkIfBundleIsReady();
            }
            else {
                currentBundleGroup.isSelectionValid(false);
                currentBundleGroup.variantId = "";

                var message = $('.invalid-bundle').text();
                MessageContext.PublishError("productinformation", message);
                ProductSelectionContext.SelectedProductInvalid(this, null);
            }
        }
    }

    self.toggleClicked = function (bundleGroup, event) {

        if ($(event.target).is('a')) {
            return true;
        }

        if (this.hasSingleSelectionsOnly()) {
            return;
        }

        $('.bundle-group-body-container').slideUp(30);
        var bundleLine = $(event.currentTarget).closest('.bundle-group');
        var hasNoVariants = $(bundleLine).hasClass('noVariants');
      
        var isActive = $(bundleLine).hasClass('active');
        if (isActive) {
            $(bundleLine).removeClass('active').find('.bundle-group-body-container').slideUp(200);
        }
        else {
            if (!hasNoVariants) {
                $(bundleLine).find('.bundle-group-body-container').slideToggle(200);
                $(bundleLine).addClass('active');
            }
            $(bundleLine).siblings('.bundle-group').removeClass('active').find('.bundle-group-body-container').slideUp(200);
        }
    }
}

function ProductBundleViewModel(component) {
    myself = this;

    myself.component = component;
    myself.bundleGroupList = ko.observableArray();

    myself.checkIfBundleIsReady = function () {
        var outerSelf = this;
        var isValid = true;
        if (myself.bundleGroupList().length) {
            $(myself.bundleGroupList()).each(function () {
                var bundleGroup = this;

                if (!bundleGroup.isSelectionValid()) {
                    isValid = false;
                    return false;
                }
            });

            if (isValid) {
                var bundleSelection = new BundleSelection();

                bundleSelection.catalogName = $('#bundle-component-product-catalog').val();
                bundleSelection.productId = $('#bundle-component-product-id').val();

                $(myself.bundleGroupList()).each(function () {
                    var bundledItem = new BundleItemSelection();

                    bundledItem.catalogName = bundleSelection.catalogName;
                    bundledItem.productId = this.productId;
                    bundledItem.variantId = this.variantId;

                    bundleSelection.addBundleItemSelection(bundledItem);
                });

                ProductSelectionContext.SelectedProductValid(this, null);
                ProductSelectionContext.SelectedBundleProduct(this, bundleSelection, null);
            }
        }
    }

    {
        if ($(component.RootElement).length) {

            var outerSelf = myself;
            var variantCount = 1;

            var bundleGroupList = $(component.RootElement).find(".component-content .valid-bundle-variant-combo [class*='bundle-group']");
            if ($(bundleGroupList).length) {
                $(bundleGroupList).each(function () {
                    var bundleGroup = new BundleGroup(outerSelf, component, this, this.className);
                    bundleGroup.readVariantInformation();

                    outerSelf.bundleGroupList.push(bundleGroup);
                });

                outerSelf.checkIfBundleIsReady();

                var data = {};
                data.productId = $(outerSelf.component.RootElement).find('#bundle-component-product-id').val();
                data.sourceProductVarianElement = outerSelf.component.RootElement;

                var listPrice = $(component.RootElement).find("#bundle-component-product-listprice").val();
                var adjustedPrice = $(component.RootElement).find("#bundle-component-product-adjustedprice").val();
                var isOnSale = $(component.RootElement).find("#bundle-component-product-isonsale").val();
                var savingsMessage = $(component.RootElement).find("#bundle-component-product-savingsmessage").val();

                ProductPriceContext.SetPrice(this, listPrice, adjustedPrice, isOnSale === "true", savingsMessage, data);
            }
        }
    }
}
