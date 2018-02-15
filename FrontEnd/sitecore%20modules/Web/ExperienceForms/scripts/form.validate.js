(function($jq) {
    var adapters = $jq.validator.unobtrusive.adapters;
    adapters.fxbAddNumberVal = function (adapterName, attribute, ruleName) {
        attribute = attribute || "val";
        ruleName = ruleName || adapterName;
        this.add(adapterName, [attribute], function(options) {
                var attrVal = options.params[attribute];
                if ((attrVal || attrVal === 0) && !isNaN(attrVal)) {
                    options.rules[ruleName] = Number(attrVal);
                }
                if (options.message) {
                    options.messages[ruleName] = options.message;
                }
            });
    };

    adapters.fxbAddMinMax = function(adapterName, minRuleName, maxRuleName, minAttribute, maxAttribute) {
        minAttribute = minAttribute || "min";
        maxAttribute = maxAttribute || "max";
        this.add(adapterName, [minAttribute, maxAttribute], function(options) {
                if (options.params[minAttribute] && options.params[maxAttribute]) {
                    if (!options.rules.hasOwnProperty(minRuleName)) {
                        if (options.message) {
                            options.messages[minRuleName] = options.message;
                        }
                    }
                    if (!options.rules.hasOwnProperty(maxRuleName)) {
                        if (options.message) {
                            options.messages[maxRuleName] = options.message;
                        }
                    }
                }
            });
    };

    adapters.addBool("ischecked", "required");

    $jq.validator.addMethod(
        "daterange",
        function(value, element, params) {
            return this.optional(element) || (value >= params.min && value <= params.max);
        });

    adapters.add(
        "daterange",
        ["min", "max"],
        function(options) {
            var params = {
                min: options.params.min,
                max: options.params.max
            };
            options.rules["daterange"] = params;
            options.messages["daterange"] = options.message;
        });

    adapters.fxbAddNumberVal("min");
    adapters.fxbAddNumberVal("max");
    adapters.fxbAddNumberVal("step");

    adapters.fxbAddMinMax("range", "min", "max");
    adapters.fxbAddMinMax("length", "minlength", "maxlength");
    adapters.fxbAddMinMax("daterange", "min", "max");

})(jQuery);