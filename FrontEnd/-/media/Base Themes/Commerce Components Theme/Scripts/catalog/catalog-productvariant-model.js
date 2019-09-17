// ----- VariantDefinition -----
var VariantValue = function (propertyName, propertyValue) {
  self = this;

  self.propertyName = propertyName;
  self.propertyValue = propertyValue;
};

function VariantCombo(variantInfo) {
  self = this;

  self.variantId = variantInfo.variantId;
  self.variantValueList = variantInfo.variantValueList;
  self.savingsMessage = variantInfo.savingsMessage;
  self.listPrice = variantInfo.listPrice;
  self.adjustedPrice = variantInfo.adjustedPrice;
  self.isOnSale = variantInfo.isOnSale === 'true';
}

var ProductVariantViewModel = function (rootElement) {
  var self = this;
  self.rootElement = rootElement;
  self.variantPropertyList = [];
  self.variantComboList = [];

  self.VariantSelectionChanged = function () {
    ProductSelectionContext.SelectedProductValid(this, null);

    MessageContext.ClearAllMessages();

    var foundVariant = self.FindBasedOnCurrentSelection();
    if (foundVariant) {
      var productId = $(self.rootElement).find('#variant-component-product-id')
        .val();
      ProductSelectionContext.CurrentProductId = productId;
      ProductSelectionContext.CurrentVariantId = foundVariant.variantId;

      var data = {};
      data.productId = productId;
      data.sourceProductVariantElement = self.rootElement;
      data.listPrice = foundVariant.listPrice;
      data.adjustedPrice = foundVariant.adjustedPrice;
      data.isOnSale = foundVariant.isOnSale;
      data.savingsMessage = foundVariant.savingsMessage;

      ProductPriceContext.SetPrice(this, data);

      var catalogName = $(self.rootElement).find('#variant-component-product-catalog')
        .val();
      ProductSelectionContext.CurrentCatalogName = catalogName;
      data.selectedVariantsValues = self.GetSelectedVariantsValues();

      ProductSelectionContext.SelectedProduct(this);
    } else {
      var message = $(self.rootElement).find('.invalid-variant')
        .text();
      MessageContext.PublishError("productinformation", message);
      ProductSelectionContext.SelectedProductInvalid(this);
    }
  };

  self.Initialize = function () {

    // If variants exist on the page, let's load their definition.
    var variants = $(self.rootElement).find(".component-content [class*='Variant_']");
    if ($(variants).length) {
      self.ReadVariantInformation(variants);
      self.VariantSelectionChanged();
      return;
    }

    // Handles catalogs with empty variant values.  Required when a variant ID is always required.
    var variantsII = $(self.rootElement).find('.valid-variant-combo [class^="variant-value"]');
    if ($(variantsII).length) {
      self.ReadVariantInformation(variants);
      self.VariantSelectionChanged();
      return;
    }

    var catalogName = $(self.rootElement).find('#variant-component-product-catalog')
        .val();
    ProductSelectionContext.CurrentCatalogName = catalogName;

    ProductSelectionContext.SelectedProduct(this);
  };

  self.ReadVariantInformation = function (variants) {
    $.each(variants,
      function () {
        var variantName = this.className;
        variantName = variantName.replace("product-variant-name ", "").substr(8);

        self.variantPropertyList.push(variantName);
      });

    var outerSelf = self;

    var variantData = $(self.rootElement).find('.valid-variant-combo')
      .find('.variant-data');
    if ($(variantData).length) {
      $(variantData).each(function () {
        var variantValueList = [];
        var variantId = $(this).attr('id');
        var listPrice = $(this).attr('listprice');
        var adjustedPrice = $(this).attr('adjustedPrice');
        var isOnSale = $(this).attr('isonsale');
        $(this).find('.variant-value')
          .find('input')
          .each(function () {
            var propertyName = $(this).attr('id');
            var propertyValue = $(this).val();

            variantValueList.push(new VariantValue(propertyName, propertyValue));
          });

        var savingsMessage = null;
        var savingsMessageElement = $(this).find('#savings-message');
        if ($(savingsMessageElement).length) {
          savingsMessage = $(savingsMessageElement).val();
        }

        var variantInfo = { variantId: variantId, variantValueList: variantValueList, savingsMessage: savingsMessage, listPrice: listPrice, adjustedPrice: adjustedPrice, isOnSale: isOnSale };
        outerSelf.variantComboList.push(new VariantCombo(variantInfo));
      });
    }
  };

  self.FindBasedOnCurrentSelection = function () {
    self = this;

    var variantValues = {};
    variantValues = self.GetSelectedVariantsValues();

    var selectedItems = [];
    var comboList = self.variantComboList;
    for (var key in variantValues) {
      for (var comboIndex in comboList) {
        for (var variantValueIndex in comboList[comboIndex].variantValueList) {
          if (comboList[comboIndex].variantValueList[variantValueIndex].propertyName === key &&
            comboList[comboIndex].variantValueList[variantValueIndex].propertyValue === variantValues[key]) {
            selectedItems.push(comboList[comboIndex]);
            break;
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

  self.GetSelectedVariantsValues = function () {
    var variantValues = {};

    if (self.variantPropertyList.length) {
      self.variantPropertyList.forEach(function (variantProperty) {
        var i = 0;
        var value = null;
        if ($(self.rootElement).find('#variant' + variantProperty).length) {
          value = $(self.rootElement).find('#variant' + variantProperty)
            .val();
          variantValues[variantProperty] = value;
        }
      });
    }

    return variantValues;
  };
};


