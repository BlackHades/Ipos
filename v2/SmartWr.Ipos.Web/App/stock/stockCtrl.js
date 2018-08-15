(function () {
    app.controller("stockCtrl", function ($scope, requestSvc, filterFilter, $state, $stateParams, formClear, notifier, blockUI, $timeout) {
        $scope.products = [];
        $scope.currentIndex = 1;
        $scope.total = 0;
        $scope.pageSize = 50;
        $scope.categories = [];
        //u set d ng-model on d view as {}
        $scope.productItemLog = {};
        $scope.waste = {};
        $scope.quantity = {};
        var cachedUrl,
            query = {},
            myBlockUI, initialQty;
        $scope.checks = [];
        $scope.postProducts = [];
        $scope.isDisabled = false;
        $scope.items = {
            itemCount: 0
        };

        var unWatch = $scope.$watch('productItemLog.lqty', function (n, o) {
            var newQty = parseInt(n) || 0;
            $scope.productItemLog.quantity = initialQty + newQty;
        });

        $scope.downloadProductReport = function (url) {

            if (!url)
                notifier("error", "Error", "request is invalid.");

            $.fileDownload(url, {
                preparingMessageHtml: "Available Stock Report is now being generated, please wait...",
                failMessageHtml: "There was a problem generating your report, please try again.",
                httpMethod: "GET"
            });
        };

        function ApiErrorCallbk(response) {

            removeBlock();

            if (typeof (response) === "object") {
                if (response.errorMessage)
                    notifier("error", "Error", response.errorMessage);
            } else
                notifier("error", "Error", response);
        }

        //Function to stop th block UI

        function removeBlock() {

            if (myBlockUI != null && (myBlockUI.stop && typeof (myBlockUI.stop) == "function")) {
                $timeout(function () {
                    // Stop the block after some async operation.
                    myBlockUI.stop();
                }, 1000);
            }
        }

        function mainSearch(data) {

            myBlockUI = blockUI.instances.get("stockListBlockUI");
            myBlockUI.start();

            requestSvc.post(data.url, data.data).then(function (response) {

                removeBlock();
                if (response.errorStatus) {
                    notifier("error", "Error", response.errorMessage);
                    return;
                }
                $scope.checks = [];
                $scope.products = response.result;
                $scope.total = response.additionalResult || 0;

            }, ApiErrorCallbk);
        }


        function collateSearchParams(url, formId) {
            if (!url)
                notifier("error", "Error", "request is invalid");

            cachedUrl = url;
            query.url = cachedUrl;
            query.data = {
                pageIndex: $scope.currentIndex,
                itemsOnPage: $scope.pageSize
            };

            if (formId) {
                var formValues = angular.element(formId).serialize2JSON();

                Object.getOwnPropertyNames(formValues).forEach(function (val) {
                    query.data[val] = formValues[val] || null;
                });
            }
            mainSearch(query);
        };


        var getItemAggregates = function (url) {
            $scope.get(url).then(function (data) {
                if (data.errorStatus) {
                    return;
                }

                $scope.items = data.result;

                timeout(function () {
                    $(".counter").counterUp({
                        delay: 100,
                        time: 1200
                    });
                });
            }, ApiErrorCallbk);
        };

        //This allows u to create a product by clicking save on the item and this is linked with d controller

        $scope.productItemsCount = function (url) {
            getItemAggregates(url);
        };

        $scope.CreateStock = function (e, url) {
            e.preventDefault(false);
            if (!$(e.target).valid())
                return;

            var formData = $(e.target).serialize2JSON();

            formData.canExpire = formData.canExpire === "on" ? true : false;
            formData.isDiscountable = formData.isDiscountable === "on" ? true : false;

            myBlockUI = blockUI.instances.get("stockCreateBlockUI");
            myBlockUI.start();

            requestSvc.post(url, formData).then(function (data) {

                removeBlock();
                if (data.errorStatus) {
                    notifier("error", "Error!", data.errorMessage);
                    return;
                } else {
                    notifier("success", "Done!", data.errorMessage);
                    formClear(e);
                    //$state.go('list');
                }
            }, ApiErrorCallbk);
        };

        //This gets the particular id u want to edit / view detail and it is inserted on the controller an ng in it
        $scope.getProdItem = function (url) {

            myBlockUI = blockUI.instances.get("stockEditBlockUI");
            myBlockUI.start();

            requestSvc.get(url + $stateParams.id).then(function (data) {

                removeBlock();
                if (data.errorStatus) {
                    notifier("error", "Error!", data.errorMessage);
                    return;
                }

                //This allows u to get d edited product to the view page from d db using ng-model on the view item
                $scope.productItemLog = data.result;
                initialQty = $scope.productItemLog.quantity;
            }, ApiErrorCallbk);
        };

        function findItemIndex(id) {
            var currentIndex;
            $scope.products.some(function (item, index) {
                if (item.productUId === id) {
                    currentIndex = index;
                    return true;
                }
                return false;
            });
            return currentIndex;
        };

        function findProduct(id) {
            var currentitem;
            $scope.products.some(function (item) {
                if (item.productUId === id) {
                    currentitem = item;
                    return true;
                }
                return false;
            });
            return currentitem;
        };

        function findProductById(id) {
            var currentitem;
            $scope.products.some(function (item) {
                if (item.id === id) {
                    currentitem = item;
                    return true;
                }
                return false;
            });
            return currentitem;
        };

        function mainDelete(url, ids) {
            if (ids.length <= 0) {
                notifier("warning", "Error", "confirm data to delete.");
                return;
            }

            if (!confirm("Are you sure want to delete the selected items ?"))
                return;

            myBlockUI = blockUI.instances.get("stockListBlockUI");
            myBlockUI.start();

            requestSvc.post(url, ids)
                .then(function (response) {
                    removeBlock();
                    if (response.errorStatus) {
                        notifier("error", "Error", response.errorMessage);
                    }

                    if (!response.result || response.result.length <= 0)
                        return;

                    var deleteCount = 0;
                    angular.element(response.result).each(function (index, id) {
                        $scope.products.remove(findItemIndex(id));
                        deleteCount += 1;
                    });

                    var a = deleteCount > 1 ? deleteCount + "  records" : deleteCount + "  record";
                    notifier("success", "Done", "successfully deleted " + a);
                    $scope.total -= deleteCount;
                    $scope.checks.length = 0;
                },
                    ApiErrorCallbk);
        };

        //This is d delete item

        $scope.removeProduct = function (url, index) {

            var formData = $scope.products[index].productUId;

            if (formData == null) {
                notifier("warning", "Opps!", "Item was not found in the list");
                return;
            }

            mainDelete(url, [formData]);
        };


        // The angular post to the server of waste item
        $scope.submitWasteItem = function (event, url) {

            event.preventDefault(false);

            var formData = $(event.target).serialize2JSON();

            if (!$(event.target).valid()) {
                //toastr.warning('please check and confirm your inputs')
                return;
            }
            $scope.isDisabled = true;
            requestSvc.post(url, formData).then(function (data) {
                if (data.errorStatus) {
                    notifier("error", "Error!", data.errorMessage);
                    $scope.isDisabled = false;
                    return;
                } else {
                    $scope.isDisabled = false;
                    var product = findProductById(data.result.id)
                    product.quantity = data.result.quantity;

                    notifier("success", "Done!", data.message);
                    $scope.hideModal();
                    formClear(event);
                    return;
                }
            }, ApiErrorCallbk);
        };

        //The angular to display the modal page to the screen

        $scope.createWasteItem = function (index, modalId) {
            var product = $scope.products[index];
            $scope.waste.name = product.name;
            $scope.waste.description = product.description;
            $scope.waste.id = product.id;
            if (jQuery.isEmptyObject($scope.waste)) {
                return;
            }
            $(modalId).modal();
        };
        //Angular to display the quantity modal

        $scope.addQuantityToStock = function (index, modalId) {
            var product = $scope.products[index];
            $scope.quantity.name = product.name;
            $scope.quantity.id = product.id;
            if (jQuery.isEmptyObject($scope.quantity)) {
                return;
            }
            $(modalId).modal();
        };

        //Posting of the quantity modal to the database

        $scope.submitItemQuantity = function (event, url) {

            event.preventDefault(false);

            var formData = $(event.target).serialize2JSON();

            if (!$(event.target).valid()) {
                // toastr.warning("please check and confirm your inputs")
                return;
            }
            $scope.isDisabled = true;
            requestSvc.post(url, formData).then(function (data) {

                if (data.errorStatus) {
                    notifier("error", "Opps!", data.errorMessage);
                } else {
                    var result = data.result,
                     product = findProductById(result.id);

                    console.log(product);

                    if (product)
                        product.quantity = result.quantity;
                    //// //  $scope.isDisabled = false;

                    notifier("success", "Done", data.message);
                    $scope.hideModal();
                    formClear(event);

                }
            }, ApiErrorCallbk);
            $scope.isDisabled = false;
        };

        //To hide modal

        $scope.hideModal = function () {
            $(".modal").modal("hide");
        };

        //The posting of the object that has been edited

        $scope.editStock = function (e, url) {
            e.preventDefault(false);

            if (!$(e.target).valid())
                return;

            var formData = $(e.target).serialize2JSON();
            formData.canExpire = formData.canExpire === "on" ? true : false;
            formData.isDiscountable = formData.isDiscountable === "on" ? true : false;

            myBlockUI = blockUI.instances.get("stockEditBlockUI");
            myBlockUI.start();

            requestSvc.post(url, formData).then(function (data) {
                removeBlock();
                if (!data.errorStatus) {

                    initialQty = $scope.productItemLog.quantity;
                    $scope.productItemLog.lqty = NaN;

                    notifier("success", "Done!", data.message);
                } else {
                    notifier("error", "Opps!", data.errorMessage);
                }
            }, ApiErrorCallbk);
        };

        //This is another way to use your angular js to validate instead of jquery validation

        $scope.init = function (id) {
            if (!id)
                return;

            $(id).validate({
                rules: {
                    name: {
                        required: true
                    },
                    description: {
                        required: true
                    },
                    quantity: {
                        required: true,
                        number: true
                    },
                    sellprice: {
                        required: true,
                        number: true
                    },
                    costprice: {
                        required: true,
                        number: true
                    },

                    category: {
                        required: true
                    }
                },
                messages: {
                    name: "Product name cannot be empty",
                    quantity: {
                        required: "Product quantity is required",
                        number: "Value entered, please supply a value greater than zero"
                    }
                }
            });
        };

        //This gets the productList to the screen and it has a controller function

        $scope.getProductList = function (url) {
            collateSearchParams(url);
        };

        $scope.getFilteredProductList = function (url, formId) {
            collateSearchParams(url, formId);
        };

        $scope.pageChanged = function (page, oldPage, formId) {
            $scope.currentIndex = page;
            collateSearchParams(cachedUrl, formId);
        };

        $scope.setItemPerPage = function (count) {
            $scope.pageSize = count;
            $scope.currentIndex = 1;
        };

        $scope.getSerialNo = function (index) {
            return ($scope.pageSize * ($scope.currentIndex - 1)) + index;
        };

        $scope.salesHistorypageChanged = function (page) {
            $scope.pageIndex = page;
            $scope.getItemHistory(cachedUrl);
        };

        $scope.getItemHistory = function (url) {
            cachedUrl = url;
            myBlockUI = blockUI.instances.get("productHistoryBlockUI");
            myBlockUI.start();
            query.id = $scope.productItemLog.id;
            query.pageIndex = $scope.pageIndex;
            query.itemsPerPage = $scope.itemsOnPage;

            requestSvc.post(cachedUrl, query).then(function (data) {

                $timeout(function () {
                    // Stop the block after some async operation.
                    myBlockUI.stop();
                }, 1000);

                if (data.errorStatus) {
                    notifier("error", "Error", data.errorMessage);
                    return;
                }
                $scope.salesHistory = data.result;
                $scope.total = data.additionalResult || 0;
            }, ApiErrorCallbk);
        };

        $scope.deleteMultiple = function (url) {
            mainDelete(url, $scope.checks);
        };


        /*To add quantity up automatically*/
        //$scope.numbers = [{
        //    quantity: 200,
        //}

        //];


        //$scope.add = function () {

        //};


        //$scope.update = function () {
        //    var result = 0;
        //    angular.forEach($scope.numbers, function (num) {
        //        result += ( num.quantity);
        //    });
        //    $scope.sum = result;
        //};

        /*Check box code starts here*/
        var updateSelected = function (action, id) {

            if (action === "add" && $scope.checks.indexOf(id) === -1) {
                $scope.checks.push(id);
            }
            if (action === "remove" && $scope.checks.indexOf(id) !== -1) {
                $scope.checks.splice($scope.checks.indexOf(id), 1);
            }
        };

        $scope.updateSelection = function (event, id) {
            var checkbox = event.target;
            var action = (checkbox.checked ? "add" : "remove");
            updateSelected(action, id);
        };

        $scope.selectAll = function (event) {
            var checkbox = event.target;
            var action = (checkbox.checked ? "add" : "remove");

            for (var i = 0; i < $scope.products.length; i++) {

                var entity = $scope.products[i];
                updateSelected(action, entity.productUId);
            }
        };

        $scope.isSelected = function (id) {
            return $scope.checks.indexOf(id) >= 0;
        };

        $scope.isSelectedAll = function () {
            return $scope.checks.length === $scope.products.length;
        };

        /*check box code ends here*/

        function findPostItemIndex(id) {
            var currentIndex;
            $scope.postProducts.some(function (item, index) {
                if (item.id === id) {
                    currentIndex = index;
                    return true;
                }
                return false;
            });
            return currentIndex;
        };

        $scope.openMultiModal = function (id) {

            $scope.checks.forEach(function (item) {
                var prod = findProduct(item);

                if (prod && findPostItemIndex(prod.id) === undefined) {
                    $scope.postProducts.push({
                        id: prod.id,
                        quantity: 1,
                        name: prod.name,
                        unitPrice: prod.sellPrice,
                        remarks: ""
                    });
                }
            });

            $(id).modal();
        };

        $scope.calculatePostTotal = function () {

            var total = 0;

            $scope.postProducts.forEach(function (item) {
                total += (item.unitPrice || 0) * (item.quantity || 0);
            });

            return total;
        };

        $scope.removePostProduct = function (index) {

            var item = $scope.postProducts[index];
            if (item) {
                $scope.postProducts.remove(index);
            }
        };

        $scope.comitMultiPost = function (event, url) {

            $scope.isDisabled = true;

            var postObj = { products: $scope.postProducts };

            requestSvc.post(url, postObj).then(function (data) {

                if (!data.errorStatus) {
                    $scope.hideModal();
                    $scope.postProducts.length = 0;
                    notifier("success", "Done", data.message);
                } else
                    notifier("error", "Error", data.errorMessage);
            }, ApiErrorCallbk);

            $scope.isDisabled = false;
        };

        $scope.clearCurrentPostItems = function () {
            $scope.postProducts.length = 0;
        }

        $scope.openSingleModal = function (index, id) {

            var prod = $scope.products[index];

            if (prod && findPostItemIndex(prod.id) === undefined) {
                $scope.postProducts.push({
                    id: prod.id,
                    quantity: 1,
                    name: prod.name,
                    unitPrice: prod.sellPrice,
                    remarks: ""
                });
            }
            $(id).modal();
        };
    });
})()