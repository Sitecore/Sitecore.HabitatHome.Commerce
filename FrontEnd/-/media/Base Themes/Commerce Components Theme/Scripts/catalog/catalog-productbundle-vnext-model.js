var BundleVariant = function(key, value) {
  var variant = this;
  variant.key = key;
  variant.value = value;
};

var BundleVariantCombination = function(variantCombinationEL) {
  var variantCombination = this;
  var $variantCombination = $(variantCombinationEL);

  variantCombination.variantId = $variantCombination.attr('data-variant-id');
  variantCombination.variants = ko.observableArray();

  function setup() {
    $variantCombination.find('.bundle-variant-combination-value').each(function() {
      var key = this.getAttribute('data-variant-key');
      var value = this.getAttribute('data-variant-value');
      variantCombination.variants.push(new BundleVariant(key, value));
    });
  }

  setup();
};

BundleVariantCombination.prototype = {
  /**
   * Verify if the variant combination is empty.
   *
   * @returns {boolean} true if variant combination is a empty, false otherwise
   */
  isEmptyVariant: function() {
    return this.variants().length === 0;
  },

  /**
   * Case insensitive comparison for both values.
   *
   * @param {string} value1 - the first value to be compared
   * @param {string} value2 - the second value to be compared
   * @returns {boolean} true if both values matches, false otherwise
   */
  isSameValue: function(value1, value2) {
    return value1.toUpperCase() === value2.toUpperCase();
  },

  /**
   * Verify if the variants provided is a valid combination by comparing against the existing
   * variant combinations.
   *
   * @param {Object[]} variants - the variant combinations to be verify
   * @returns {boolean} true if variants provided is a valid combinations, false otherwise
   */
  isValidVariants: function(variants) {
    var variantCombination = this;
    return variantCombination.variants().every(function(validVariant) {
      return variants.some(function(variant) {
        return (
          variantCombination.isSameValue(variant.key, validVariant.key) &&
          variantCombination.isSameValue(variant.value, validVariant.value)
        );
      });
    });
  }
};

var BundleGroupVNext = function(vm, bundleGroupEL) {
  var bundleGroup = this;

  bundleGroup.vm = vm;
  bundleGroup.bundleGroupEL = bundleGroupEL;
  bundleGroup.$bundleGroup = $(bundleGroupEL);
  bundleGroup.bundleLineItemId = ko.observable(bundleGroup.$bundleGroup.attr('data-bundle-line-item-id'));
  bundleGroup.bundleLineItemName = ko.observable(bundleGroup.$bundleGroup.attr('data-bundle-line-item-name'));
  bundleGroup.variantCombinations = [];
  bundleGroup.isNoVariant = ko.observable(false);
  bundleGroup.isFixedVariant = ko.observable(false);
  bundleGroup.isMultiVariant = ko.pureComputed(function() {
    return !(bundleGroup.isNoVariant() || bundleGroup.isFixedVariant());
  });
  bundleGroup.isValidSelection = ko.observable(true);
  bundleGroup.selectedVariantsId = ko.observable('');
  bundleGroup.selectedVariants = ko.observable('');
  bundleGroup.setArrowClass = ko.pureComputed(function() {
    return bundleGroup.isMultiVariant() ? 'collapse-arrow' : '';
  });

  function setup() {
    bundleGroup.createBundleVariantCombination();
    bundleGroup.prepareVariantCombination();
  }

  setup();
};

BundleGroupVNext.prototype = {
  /**
   * Create bundle variant combination data from markup
   */
  createBundleVariantCombination: function() {
    var bundleGroup = this;
    bundleGroup.$bundleGroup.find('.bundle-variant-combination').each(function() {
      bundleGroup.variantCombinations.push(new BundleVariantCombination(this));
    });
  },

  /**
   * Update variant type indicators and assign default variant to bundle group.
   */
  prepareVariantCombination: function() {
    var defaultVariantCombination = this.variantCombinations[0];
    if (this.variantCombinations.length === 1) {
      if (defaultVariantCombination.isEmptyVariant()) {
        this.isNoVariant(true);
      } else {
        this.isFixedVariant(true);
      }
    }
    if (defaultVariantCombination) {
      this.selectedVariantsId(defaultVariantCombination.variantId);
      this.selectedVariants(defaultVariantCombination.variants);
    }
  },

  /**
   * Toggle the bundle group body, show if it is hidden, hide if it is shown.
   *
   * @param {object} bundleGroupHeaderEL - DOM element that is triggering the event
   * @param {object} e - event triggered
   */
  toggleBundleGroupBody: function(bundleGroupHeaderEL, e) {
    if (this.isMultiVariant()) {
      var $bundleGroup = $(e.currentTarget).closest('.bundle-group');
      var bundleGroupBodyIsVisible = $bundleGroup.hasClass('active');
      if (bundleGroupBodyIsVisible) {
        this.hideBundleGroupBody($bundleGroup);
      } else {
        this.showBundleGroupBody($bundleGroup);
      }
    }
    return true;
  },

  /**
   * Hide the target bundle group body.
   *
   * @param {object} $bundleGroup - targeted bundle group jQuery object
   */
  hideBundleGroupBody: function($bundleGroup) {
    $bundleGroup
      .removeClass('active')
      .find('.bundle-group-body-container')
      .slideUp(200);
  },

  /**
   * Display the target bundle group body and hide any opened bundle group body.
   *
   * @param {object} $bundleGroup - targeted bundle group jQuery object
   */
  showBundleGroupBody: function($bundleGroup) {
    $bundleGroup
      .siblings('.bundle-group')
      .removeClass('active')
      .find('.bundle-group-body-container')
      .slideUp(200);

    $bundleGroup
      .addClass('active')
      .find('.bundle-group-body-container')
      .slideToggle(200);
  },

  /**
   * Event handler for variant selection changes. Manages the bundle group validation flag
   * and selected variants information.
   */
  onVariantSelectionChanged: function() {
    MessageContext.ClearAllMessages();

    var bundleGroup = this;
    var variantSelections = bundleGroup.getVariantSelections();
    var selectedVariantId = bundleGroup.getVariantId(variantSelections);

    if (selectedVariantId) {
      bundleGroup.isValidSelection(true);
      bundleGroup.selectedVariantsId(selectedVariantId);
      bundleGroup.selectedVariants(variantSelections);

      bundleGroup.vm.prepareBundleSelection();
    } else {
      bundleGroup.isValidSelection(false);
      bundleGroup.selectedVariantsId('');
      bundleGroup.selectedVariants([]);

      MessageContext.PublishError('productinformation', bundleGroup.vm.bundleErrorMessage);
      ProductSelectionContext.SelectedProductInvalid(bundleGroup, null);
    }
  },

  /**
   * Generate a collection of variants selected from the DOM element.
   *
   * @returns {Object[]} array of key-value pair objects from the variants selected
   */
  getVariantSelections: function() {
    var variantSelections = [];
    this.$bundleGroup.find('.group-variant-section .group-variant-selection').each(function() {
      var $selection = $(this).find('.group-variant-select');
      variantSelections.push({
        key: $selection.attr('data-variant-key'),
        value: $selection.val()
      });
    });
    return variantSelections;
  },

  /**
   * Returns the identifier of the selected variants if exist.
   *
   * @param {Object []} variants - the selected variants
   */
  getVariantId: function(variants) {
    var variantId = null;
    this.variantCombinations.some(function(variantCombination) {
      var isValidVariant = variantCombination.isValidVariants(variants);
      if (isValidVariant) {
        variantId = variantCombination.variantId;
      }
      return isValidVariant;
    });
    return variantId;
  }
};

function ProductBundleVNextViewModel(component) {
  var vm = this;

  vm.component = component;
  vm.$componentRoot = $(component.RootElement);
  vm.$bundleHeader = vm.$componentRoot.find('.bundle-header');
  vm.catalogName = vm.$bundleHeader.attr('data-catalog-name');
  vm.bundleId = vm.$bundleHeader.attr('data-bundle-id');
  vm.bundleErrorMessage = vm.$bundleHeader.attr('data-bundle-error-message');
  vm.bundleGroups = ko.observableArray();

  function setup() {
    if (vm.$componentRoot.length) {
      vm.prepareBundle();
    }
  }

  setup();
}

ProductBundleVNextViewModel.prototype = {
  /**
   * Prepare the data needed for product bundle
   */
  prepareBundle: function() {
    var $bundleGroups = this.$componentRoot.find('.bundle-group');
    if ($bundleGroups.length) {
      this.createBundleGroup($bundleGroups);
      this.prepareBundleSelection();
      this.prepareBundlePrice();
    }
  },

  /**
   * Create bundle group data from target markup.
   *
   * @param {object} $bundleGroups - the target bundle group markup
   */
  createBundleGroup: function($bundleGroups) {
    var vm = this;
    $bundleGroups.each(function() {
      vm.bundleGroups.push(new BundleGroupVNext(vm, this));
    });
  },

  /**
   * Prepare the bundle data needed for adding into cart.
   */
  prepareBundleSelection: function() {
    if (this.isValidProductBundle()) {
      var bundleSelection = new BundleSelection();

      bundleSelection.catalogName = this.catalogName;
      bundleSelection.productId = this.bundleId;

      this.bundleGroups().forEach(function(bundleGroup) {
        var bundledItem = new BundleItemSelection();

        bundledItem.catalogName = bundleSelection.catalogName;
        bundledItem.productId = bundleGroup.bundleLineItemId();
        bundledItem.variantId = bundleGroup.selectedVariantsId();

        bundleSelection.addBundleItemSelection(bundledItem);
      });

      ProductSelectionContext.SelectedProductValid(this, null);
      ProductSelectionContext.SelectedBundleProduct(this, bundleSelection, null);
    }
  },

  /**
   * Returns the validation state of product bundle.
   *
   * @returns {boolean} true if bundle is valid, false otherwise
   */
  isValidProductBundle: function() {
    var bundleGroups = this.bundleGroups();
    return (
      bundleGroups.length &&
      bundleGroups.every(function(bundleGroup) {
        return bundleGroup.isValidSelection();
      })
    );
  },

  /**
   * Prepare the pricing data to be display.
   */
  prepareBundlePrice: function() {
    var listPrice = this.$bundleHeader.attr('data-bundle-list-price');
    var adjustedPrice = this.$bundleHeader.attr('data-bundle-adjusted-price');
    var isOnSale = this.$bundleHeader.attr('data-bundle-is-on-sale');
    var savingsMessage = this.$bundleHeader.attr('data-bundle-savings-percentage');
    var data = {
      productId: this.bundleId,
      sourceProductVarianElement: this.component.RootElement
    };

    ProductPriceContext.SetPrice(this, listPrice, adjustedPrice, isOnSale === 'true', savingsMessage, data);
  }
};
