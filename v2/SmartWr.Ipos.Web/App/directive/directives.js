(function () {
    window.app.directive("convertToNumber", function () {
        return {
            restrict: "A",
            require: "ngModel",
            link: function(scope, element, attrs, ngModel) {
                ngModel.$parsers.push(function (val) {
                    return val ? parseInt(val, 10) : 0;
                });
                ngModel.$formatters.push(function (val) {
                    return val ? "" + val : "";
                });
            }
        };
    }).directive("numberOnly", function () {
        return {
            scope: {
                allowDot: "="
            },
            restrict: "A",
            link: function (s, e) {
                e.on("keydown", function (ev) {
                    var charCode = (ev.which) ? ev.which : ev.keyCode;
                    var allowed = false;
                    if (s.allowDot)
                        if (charCode === 190)
                            allowed = true;
                    if ((charCode >= 48 && charCode <= 57) || (charCode >= 96 && charCode <= 105) || charCode === 8 || charCode === 9)
                        allowed = true;

                    return allowed;
                });
            }
        };
    }).directive("fileModel", ["$parse", function (parse) {
        return {

            restrict: "A",
            link: function (scope, element, attr) {
                var model = parse(attr.fileModel),
                    modelSetter = model.assign;

                element.bind("change", function () {
                    scope.$apply(function () {
                        modelSetter(scope, element[0].files[0]);
                    });
                });
            }
        };
    }]);
})()