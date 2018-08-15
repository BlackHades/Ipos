/// <reference path="C:\Users\Sampa\Source\Workspaces\Smartware Interactive POS 2\v2\SmartWr.Ipos.Web\Scripts/angular.js" />
(function () {

    var handleSelect2 = function () {

        // $.fn.select2.defaults.set("theme", "bootstrap");
        $('.select2').select2({
            width: '100%',
            allowClear: true,
        });
    };
    app.controller('transactionCtrl', function ($scope, requestSvc, filterFilter, $stateParams, formClear, notifier, blockUI, $timeout) {
        var vm = this, query = {}, cachedUrl, myBlockUI;
        vm.currentIndex = 1;
        vm.total = 0;
        vm.sumTotal = 0;
        vm.sales = [];
        vm.selection = [25, 40, 50, 100, 200];
        vm.itemsOnPage = vm.selection[2];
        vm.orderDetails = [];
        vm.itemsBought = [];
        vm.posts = [];
        vm.checks = [];
        vm.users = [];

        handleSelect2();

        vm.getTodaysales = function (url) {
            collateSearchParams(url);
        };

        vm.downloadReport = function (url, formId, type) {
            if (!url)
                notifier('error', 'Error', 'request is invalid.');

            query.url = url;
            query.data = {
                reportType: type
            };

            if (formId) {
                var formValues = angular.element(formId).serialize2JSON();

                Object.getOwnPropertyNames(formValues).forEach(function (val) {
                    query.data[val] = formValues[val] || null;
                });
            }

            $.fileDownload(query.url, {
                preparingMessageHtml: "Your report is now being generated, please wait...",
                failMessageHtml: "There was a problem generating your report, please try again.",
                httpMethod: "POST",
                data: query.data
            });
        }

        vm.getSerialNo = function (index) {
            return (vm.itemsOnPage * (vm.currentIndex - 1)) + index;
        };

        vm.getpendingPost = function (url) {
            myBlockUI = blockUI.instances.get('transactionPendingBlockUI');
            myBlockUI.start();

            requestSvc.get(url).then(function (response) {
                removeBlock();
                if (response.errorStatus) {
                    notifier('error', 'Error', response.errorMessage);
                    return;
                }
                vm.posts = response.result || [];
                vm.total = vm.posts.length;
            }, ApiErrorCallbk)
        };

        vm.searchSales = function (url, formId) {
            collateSearchParams(url, formId);
        };

        vm.getTransactionDetail = function (url) {
            if (!url)
                $.Notification.notify('error', 'right bottom', 'Error', 'request is invalid');

            myBlockUI = blockUI.instances.get('transactionDetailBlockUI');
            myBlockUI.start();

            requestSvc.get(url + $stateParams.id).then(function (response) {
                removeBlock();
                if (response.errorStatus) {
                    $.Notification.notify('error', 'right bottom', 'Error', response.errorMessage)
                    return;
                }
                vm.itemsBought = response.result;
                vm.orderDetails = response.additionalResult;
            }, ApiErrorCallbk)
        }
        $scope.isDisabled = false;
        vm.commitPendingPost = function (event, url) {
            $scope.isDisabled = true;
            if (!url) {
                notifier('error', 'Error', 'Incomplete request.');
            }

            requestSvc.post(url, { entryDate: vm.postDate, remarks: vm.remarks }).then(function (response) {

                if (response.errorStatus) {
                    notifier('error', 'Error', response.errorMessage);
                    $scope.isDisabled = false;
                }
                else {
                    notifier('success', 'Done', response.message);
                    $scope.isDisabled = false;
                    formClear(event);
                    hideModal();
                    vm.posts = [];
                    vm.total = 0;
                }
            }, ApiErrorCallbk)
        };

        vm.pageChanged = function (page, formId) {
            vm.currentIndex = page;
            collateSearchParams(cachedUrl, formId);
        };

        vm.deleteSingle = function (url, index) {
            var ids = [];
            var record = vm.posts[index].id;

            if (!record) {
                notifier("warning", "Huh!", "sorry! record not found");
                return;
            }

            ids.push(record);
            mainDelete(url, ids, false);
        };

        vm.deleteMultiple = function (url) {
            mainDelete(url, vm.checks, true);
        };

        /*Check box code starts here*/
        var updateSelected = function (action, id) {

            if (action === 'add' && $scope.vm.checks.indexOf(id) === -1) {
                $scope.vm.checks.push(id);
            }
            if (action === 'remove' && $scope.vm.checks.indexOf(id) !== -1) {
                $scope.vm.checks.splice($scope.vm.checks.indexOf(id), 1);
            }
        };

        $scope.updateSelection = function (event, id) {
            var checkbox = event.target;
            var action = (checkbox.checked ? 'add' : 'remove');
            updateSelected(action, id);
        };

        $scope.selectAll = function (event) {
            var checkbox = event.target;
            var action = (checkbox.checked ? 'add' : 'remove');

            for (var i = 0; i < $scope.vm.posts.length; i++) {

                var entity = $scope.vm.posts[i];
                updateSelected(action, entity.id);
            }
        };

        $scope.isSelected = function (id) {
            return $scope.vm.checks.indexOf(id) >= 0;
        };

        $scope.isSelectedAll = function () {
            return $scope.vm.checks.length === $scope.vm.posts.length;
        };
        /*check box code ends here*/

        vm.recallOrderItem = function (e, url) {

            e.preventDefault(false);

            if (!$(e.target).valid()) {
                notifier('warning', 'Opps!', 'Please validate your request')
                return;
            }

            var formData = $(e.target).serialize2JSON();
            formData.itemId = vm.transactionId;

            requestSvc.post(url, formData).then(function (data) {

                if (data.errorStatus) {

                    notifier('error', 'Error', data.errorMessage);
                    return;
                }
                else {

                    if (data.result) {

                        var item = findorderDetail(data.result.id);
                        item.total = data.result.total;
                        item.quantity = data.result.quantity;
                        vm.orderDetails.total = data.additionalResult;
                    }

                    notifier('success', 'Done', data.message);
                    formClear(e);
                    hideModal();
                }
            });
        };

        vm.openRecallModal = function (index, selector) {

            vm.transactionId = vm.itemsBought[index].id;
            vm.itemName = vm.itemsBought[index].productName;
            vm.ordtOldQty = vm.itemsBought[index].quantity;
            vm.ordtOldPrice = vm.itemsBought[index].sellPrice;

            if (!vm.transactionId) {
                notifier('warning', 'Opps!', 'Invalid selection')
                return;
            }

            $(selector).modal();
        };

        vm.getUsers = function (url) {
            requestSvc.get(url).then(function (response) {
                if (response.errorStatus) {
                    notifier('error', 'Error', response.errorMessage);
                    return;
                }
                vm.users = response.result;
            }, ApiErrorCallbk)
        };

        $scope.$watch('transaction.itemsOnPage', function (n, o) {
            vm.currentIndex = 1;
        });

        function findPostIndex(id) {
            var currentIndex;
            vm.posts.some(function (item, index) {
                if (item.id === id) {
                    currentIndex = index;
                    return true;
                }
            });
            return currentIndex;
        };

        function findorderDetail(id) {
            var currentitem;
            vm.itemsBought.some(function (item, index) {
                if (item.id === id) {
                    currentitem = item;
                    return true;
                }
            });
            return currentitem;
        };

        function mainDelete(url, ids) {
            if (ids.length <= 0) {
                notifier('warning', 'error', "confirm data to delete.")
                return;
            }

            if (!confirm('Are you sure want to delete the selected items ?'))
                return;

            //change post items to data.items for an object
            myBlockUI = blockUI.instances.get('transactionPendingBlockUI');
            myBlockUI.start();

            requestSvc.post(url, ids)
                .then(function (response, status) {

                    removeBlock();

                    if (!response.result)
                        return;

                    var deleteCount = 0;
                    angular.element(response.result).each(function (index, id) {
                        vm.posts.remove(findPostIndex(id));
                        deleteCount += 1;
                    });

                    var a = deleteCount > 1 ? deleteCount + "  records" : deleteCount + "  record";
                    notifier('success', 'Done', 'successfully deleted  ' + a);
                    vm.total -= deleteCount;
                    vm.checks.length = 0;
                },
               ApiErrorCallbk);
        };

        function collateSearchParams(url, formId) {

            if (!url)
                $.Notification.notify('error', 'right bottom', 'Error', 'request is invalid');

            cachedUrl = url;
            cachedFormId = formId;
            query.url = cachedUrl;

            query.data = {
                pageIndex: vm.currentIndex - 1,
                itemsOnPage: vm.itemsOnPage,
            };

            if (formId) {
                var formValues = angular.element(formId).serialize2JSON();

                Object.getOwnPropertyNames(formValues).forEach(function (val) {
                    query.data[val] = formValues[val] || null;
                });
            }

            mainSearch(query);
        };

        function mainSearch(data) {
            myBlockUI = blockUI.instances.get('transactionBlockUI');
            myBlockUI.start();
            requestSvc.post(data.url, data.data).then(function (response) {
                removeBlock()
                if (response.errorStatus) {
                    notifier('error', 'Error', response.errorMessage)
                    return;
                }
                if (response.result) {
                    vm.sales = response.result;
                    vm.total = response.additionalResult ? response.additionalResult.pageCount : 0;
                    vm.sumTotal = response.additionalResult ? response.additionalResult.sumTotal : 0;
                }
                else {
                    if (response.message)
                        notifier('warning', 'Opps!', response.message)
                }
            }, ApiErrorCallbk)
        };

        function ApiErrorCallbk(response) {
            if (typeof (response) === 'object') {
                if (response.errorMessage)
                    notifier('error', 'Error', response.errorMessage);
            }
            else
                notifier('error', 'Error', response);
        };

        function scrollToTop() {
            if ($(document).scrollTop() > 0) {
                $('html').animate({ scrollTop: 0 }, 'slow');
                $('body').animate({ scrollTop: 0 }, 'slow');
            }
        };

        function removeBlock() {
            if (myBlockUI != null && (myBlockUI.stop && typeof (myBlockUI.stop) == 'function')) {
                $timeout(function () {
                    // Stop the block after some async operation.
                    myBlockUI.stop();
                }, 1000);
            }
        }

        function hideModal() {
            $('.modal').modal('hide');
        };

        vm.reverseDiscount = function calculate(percent, currentPrice) {
            var _percent = 100;

            if (percent <= 0 && currentPrice || percent > _percent)
                return currentPrice || 0;

            percent = _percent - percent;
            // Hackish way to get floating point to measure up in JS.
            currentPrice = currentPrice * 10 * 10;
            return parseFloat(currentPrice) / percent;
        }

        vm.reverseDiscountAmount = function calculate(discountAmount, currentPrice) {
            return currentPrice + discountAmount;
        }
    })
})();