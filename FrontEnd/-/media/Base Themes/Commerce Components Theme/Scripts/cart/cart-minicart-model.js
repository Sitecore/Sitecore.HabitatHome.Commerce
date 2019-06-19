//-----------------------------------------------------------------------
// Copyright 2016 Sitecore Corporation A/S
// Licensed under the Apache License, Version 2.0 (the 'License"); you may not use this file
// except in compliance with the License. You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
// either express or implied. See the License for the specific language governing permissions
// and limitations under the License.
// -------------------------------------------------------------------------------------------

const MINICART = {
    URL: {
        GET: '/api/cxa/cart'
    },
    HOVER_STATE: {
        ACTIVE: 'show-minicart',
        INACTIVE: 'hide-minicart'
    },
    FAKE_CART_COUNT: 5
};

function MinicartViewModel() {
    /**
     * Wether the fetch request is in progress
     */
    let isFetchInProgress;

    /**
     * Keep the cart information
     */
    let cart;

    /**
     * Keep the number of cart items
     */
    let cartCount;

    /**
     * Keep the current class name of minicart
     */
    let minicartClassName;

    /**
     * Wether the cart data is changed
     */
    let isCartItemsChanged = true;

    let isNormalMode = false;

    let hoverTimeout = false;

    /**
     * Model constructor
     */
    (function constructor() {
        setDefaults();
        updateCartCount();
    })();

    /**
     * Set default values and initialize the variables
     */
    function setDefaults() {
        isNormalMode = CXAApplication.RunningMode === RunningModes.Normal;
        isFetchInProgress = ko.observable(false);
        cart = ko.observable(new CartViewModel());
        cartCount = ko.observable(0);
        minicartClassName = ko.observable(MINICART.HOVER_STATE.INACTIVE);
    }

    /**
     * Update the cart items count
     */
    function updateCartCount() {
        isCartItemsChanged = true;

        if (isNormalMode) {
            AjaxService.Post(MINICART.URL.GET + '/GetCartLinesCount', {}, function (
                    data
                ) {
                    if (data) {
                        const count = parseInt(data.LinesCount);
                        cartCount(count);
                    }
                });
        } else {
            cartCount(MINICART.FAKE_CART_COUNT);
        }
    }

    /**
     * Fires when mouse hover or touch start happen
     * It fetch the cart data
     */
    function hoverMinicart() {
        clearTimeout(hoverTimeout);
        if (!hoverTimeout) {
            minicartClassName(MINICART.HOVER_STATE.ACTIVE);
            isNormalMode ? getCart() : getFakeCart();
        }
    }

    /**
     * Fetch the cart data
     * It only runs when there is a new changes
     */
    function getCart() {
        if (shouldFetchCart()) {
            isFetchInProgress(true);
            AjaxService.Post(MINICART.URL.GET + '/GetMinicart', {}, function (data) {
                    if (data) {
                        refreshCartData(data);
                    }
                    isFetchInProgress(false);
                    isCartItemsChanged = false;
                });
        }
    }

    /**
     * Get the fake cart data for EE mode
     */
    function getFakeCart() {
        let items = [];
        const mockProductImage = getMiniProductMockImage();

        if (shouldFetchCart()) {
            cart().total('0.00 USD');
            for (let i = 0; i < 5; i++) {
                items.push({
                    displayName: 'Lorem ipsum dolor sit amet, id dicant',
                    colorInformation: 'Soleat',
                    productUrl: 'javascript:vid(0)',
                    discountOfferNames: ['mediocritatem no mei(25%)'],
                    quantity: 1,
                    linePrice: '0.00 USD',
                    lineTotal: '0.00 USD',
                    lineItemDiscount: '0.00 USD',
                    externalCartLineId: i,
                    image: mockProductImage
                });
            }
            cart().cartLines(items);
        }
    }

    /**
     * Wehther should http cart call happen
     */
    function shouldFetchCart() {
        return !isFetchInProgress() && isCartItemsChanged;
    }

    /**
     * Update the cart data
     * @param {*} data
     */
    function refreshCartData(data) {
        const cartLinesCount = data.Lines ? data.Lines.length : 0;

        cartCount(cartLinesCount);
        cart(new CartViewModel(data));
    }

    /**
     * Remove a cart item
     * @param {*} item
     * @param {*} event
     */
    function removeItem(item, event) {
        const icon = $(event.currentTarget).find('.glyphicon');
        let lineItemId = item.externalCartLineId;
        let sender = event.currentTarget;
        isFetchInProgress(true);

        $(icon).removeClass('glyphicon-remove-circle');
        $(icon)
            .find('.glyphicon')
            .addClass('glyphicon-refresh');
        $(icon)
            .find('.glyphicon')
            .addClass('glyphicon-refresh-animate');
        AjaxService.Post(
            MINICART.URL.GET + '/RemoveMinicartLine',
            { lineNumber: lineItemId },
            function (data, success, sender) {
                isCartItemsChanged = true;

                if (data) {
                    isFetchInProgress(false);
                    isNormalMode ? getCart() : getFakeCart();
                }
            },
            sender
        );
    }

    function leaveMinicart() {
        hoverTimeout = setTimeout(function() {
                minicartClassName(MINICART.HOVER_STATE.INACTIVE);
                hoverTimeout = false;
        }, 200);
    }

    function toggleMinicart() {
        MINICART.HOVER_STATE.INACTIVE === minicartClassName()
            ? hoverMinicart()
            : leaveMinicart();
    }

    function getMiniProductMockImage() {
        let imageSrc;
        const pageExtension = getCurrentPageExtension();
        const tenantAndSiteNames = getCurrentTenantAndSiteName();

        if (pageExtension === 'html') {
            return (
                '-/media/Project/' +
                    tenantAndSiteNames.Tenant +
                    '/' +
                    tenantAndSiteNames.Site +
                    '/Placeholder Images/72x72.png'
            );
        } else {
            return '/sitecore/shell/-/media/Feature/Experience%20Accelerator/Commerce/Catalog/72x72.png?h=72&w=72';
        }
    }

    /**
     * Export the functions for external use
     */
    return {
        refreshCartData: refreshCartData,
        removeItem: removeItem,
        hoverMinicart: hoverMinicart,
        leaveMinicart: leaveMinicart,
        toggleMinicart: toggleMinicart,
        cart: cart,
        isFetchInProgress: isFetchInProgress,
        minicartClassName: minicartClassName,
        cartCount: cartCount,
        updateCartCount: updateCartCount
    };
}