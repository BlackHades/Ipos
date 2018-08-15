(function () {
    app.controller('categoryCtrl', function ($scope, requestSvc, $state, $stateParams, notifier, filterFilter, blockUI, $timeout, formClear) {
        $scope.total = 0;
        $scope.pageIndex = 1;
        $scope.itemsPerPage = 50;
        $scope.category = [];
        $scope.checks = [];
        categoryItemLog = {};
        var filterData = {}, cachedUrl,
            keyword = {};
        var myBlockUI;

        //To get / fetch the category list

        function getCategoryList(url, formrequestData) {
            cachedUrl = url;

            myBlockUI = blockUI.instances.get('categoryBlockUI');
            myBlockUI.start();

            requestSvc.post(url, formrequestData).then(function (data) {

                removeBlock();

                if (data.errorStatus) {
                    notifier('error', 'Error', data.errorMessage);
                    return;
                }
                $scope.category = data.result;
                $scope.total = data.additionalResult;

            }, ApiErrorCallbk);


        }

        function ApiErrorCallbk(response) {

            removeBlock();

            if (typeof (response) === 'object') {
                if (response.errorMessage)
                    notifier('error', 'Error', response.errorMessage);
            }
            else
                notifier('error', 'Error', response);
        }

        //$scope.$watch('q', function (n, o) {

        //    $scope.total = filterFilter($scope.category, n).length;
        //}, true);

        //To create a category

        $scope.createcategory = function (e, url) {
            e.preventDefault = (false);

            if (!$(e.target).valid())
                return;
            var formData = $(e.target).serialize2JSON();

            myBlockUI = blockUI.instances.get('categoryCreatBlockUI');
            myBlockUI.start();
            requestSvc.post(url, formData).then(function (data) {

                removeBlock();
                if (data.errorStatus) {
                    notifier('error', 'Error!', data.errorMessage);
                    return;
                }

                if (!data.errorStatus) {
                    notifier('success', 'Task completed!', data.message);

                    formClear(e);
                }
            });
        }

        //To get the items on the row clicked to the screen

        $scope.getCatItem = function (url) {

            myBlockUI = blockUI.instances.get('categoryEditBlockUI');
            myBlockUI.start();

            requestSvc.get(url + $stateParams.id).then(function (data) {

                removeBlock();

                if (data.errorStatus) {
                    notifier('error', 'An error occured while retrieving the record', data.errorMessage);
                    return;
                }

                //This allows u to bind the item on the database to the view for either edit or viewdetail

                $scope.categoryItemLog = data.result;
            }, ApiErrorCallbk);

        };

        //To post item back to the server on edit post

        $scope.editcategories = function (e, url) {

            e.preventDefault = (false);

            if (!$(e.target).valid())
                return;
            var formData = $(e.target).serialize2JSON();

            myBlockUI = blockUI.instances.get('categoryEditBlockUI');
            myBlockUI.start();
            requestSvc.post(url, formData).then(function (data) {
                
                removeBlock();

                if (data.errorStatus) {
                    notifier('error', 'Opps!', data.errorMessage);
                    return;
                } else {
                    notifier('success', 'Done', data.message);
                    $state.go('list');
                    return;
                }
            }, ApiErrorCallbk);
        }


        //Search of category according to what is typed in the search box result

        $scope.searchForCategory = function (url, formId) {

            if (!url)
                notifier('error', 'Error', 'request is invalid');

            cachedUrl = url;
            keyword.url = cachedUrl;
            keyword.data = {
                pageIndex: $scope.pageIndex,
                itemsOnPage: $scope.itemsPerPage,
            };

            if (formId) {
                var formValues = angular.element(formId).serialize2JSON();

                Object.getOwnPropertyNames(formValues).forEach(function (val) {
                    keyword.data[val] = formValues[val] || null;
                });
            }
            mainSearch(keyword);
        };

        function mainSearch(data) {

            myBlockUI = blockUI.instances.get('categoryBlockUI');
            myBlockUI.start();

            requestSvc.post(data.url, data.data).then(function (response) {

                removeBlock();

                if (response.errorStatus) {
                    notifier('error', 'Error', response.errorMessage)
                    return;
                }

                $scope.category = response.result;
                $scope.total = response.additionalResult || 0;

            }, ApiErrorCallbk);
        }

        function removeBlock() {

            if (myBlockUI != null && (myBlockUI.stop && typeof (myBlockUI.stop) == 'function')) {
                $timeout(function () {
                    // Stop the block after some async operation.
                    myBlockUI.stop();
                }, 1000);
            }
        }

        //To do pagination for a page

        $scope.pageChanged = function (page, oldPage, formId) {
            $scope.pageIndex = page,
            $scope.searchForCategory(cachedUrl, formId);
        };

        $scope.setItemPerPage = function (count) {
            $scope.itemsPerPage = count;
        };


        $scope.removecategory = function (url, index) {

            var formData = $scope.category[index];

            if (formData == null) {
                notifier('warning', 'Opps!', 'Item was not found in the list')
                return;
            }
            if (formData.productCount > 0) {



























































































































































































                notifier('warning', 'Sorry!', 'This category could not be deleted because it contains stocks. ')
                return;
            }

            mainDelete(url, [formData.categoryUId]);
        };

        function mainDelete(url, ids) {
            if (ids.length <= 0) {
                notifier('warning', 'error', "confirm data to delete.")
                return;
            }

            if (!confirm('Are you sure want to delete the selected items ?'))
                return;

            myBlockUI = blockUI.instances.get('categoryDeleteBlockUI');
            myBlockUI.start();

            requestSvc.post(url, ids)
                .then(function (response, status) {
                    removeBlock();
                    if (response.errorStatus) {
                        notifier('error', 'error', response.errorMessage);
                    }

                    if (!response.result || response.result.length <= 0)
                        return;

                    var deleteCount = 0;
                    angular.element(response.result).each(function (index, id) {
                        $scope.category.remove(findItemIndex(id));
                        deleteCount += 1;
                    });

                    var a = deleteCount > 1 ? deleteCount + "  records" : deleteCount + "  record";
                    notifier('success', 'Done', 'successfully deleted  ' + a);
                    $scope.total -= deleteCount;
                    $scope.checks.length = 0;
                },
               ApiErrorCallbk);
        };

        function findItemIndex(id) {
            var currentIndex;
            $scope.category.some(function (item, index) {
                if (item.categoryUId === id) {
                    currentIndex = index;
                    return true;
                }
            });
            return currentIndex;
        };

        $scope.deleteMultiple = function (url) {
            mainDelete(url, $scope.checks);
        };

        /*Check box code starts here*/
        var updateSelected = function (action, id) {

            if (action === 'add' && $scope.checks.indexOf(id) === -1) {
                $scope.checks.push(id);
            }
            if (action === 'remove' && $scope.checks.indexOf(id) !== -1) {
                $scope.checks.splice($scope.checks.indexOf(id), 1);
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

            for (var i = 0; i < $scope.category.length; i++) {

                var entity = $scope.category[i];
                updateSelected(action, entity.categoryUId);
            }
        };

        $scope.isSelected = function (id) {
            return $scope.checks.indexOf(id) >= 0;
        };

        $scope.isSelectedAll = function () {
            return $scope.checks.length === $scope.category.length;
        };
        /*check box code ends here*/

    })
})();