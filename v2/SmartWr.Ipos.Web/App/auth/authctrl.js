(function () {

    window.app.controller('loginCtrl', ['$scope', 'requestSvc', '$timeout', 'notifier', 'formClear', '$stateParams', 'blockUI', '$timeout', function (vm, rq, t, notifier, formClear, sp, blockUI, $timeout) {
        var query = {}, cachedUrl;
        vm.total = 0;
        vm.checks = [];
        vm.users = [];
        vm.itemsOnPage = 50;
         vm.pageIndex = 1;
        vm.user = {};

        var myBlockUI;  

        vm.user.id = sp.accountId || 0;

        vm.getUserAccount = function (url, id) {
            if (!id || id === 0) {
                notifier('warning', 'Opps!', "sorry! record not found.");
            }

            myBlockUI = blockUI.instances.get('userCreateBlockUI');
            myBlockUI.start();

            rq.get(url + id).then(function (response) {
                removeBlock();
                if (response.errorStatus) {
                    notifier('error', 'Error!', response.errorMessage);
                    return;
                }

                vm.user = response.result;
                ///Select2
                $timeout(function () {
                    $('.select2').val(vm.user.role).trigger("change");
                }, 0, false);
                ///
            }, ApiErrorCallbk);
        };

        vm.showAlertBox = false;

        vm.login = function (event, url) {

            event.preventDefault(false);
            var formData = $(event.target).serialize2JSON();
            console.log(formData);

            postLoginData(url, formData);
        };

        var postLoginData = function (url, data) {

            rq.post(url, data).then(function (response) {

                if (response.errorStatus) {

                    vm.showAlertBox = true;
                    vm.showErrorMsg = response.errorMessage;

                    t(function () {

                        vm.showAlertBox = false;
                    }, 6000);
                    console.log('Here');
                    return;
                }

                window.location.href = appUrl ;
            }, ApiErrorCallbk);
        }

        vm.getUsers = function (url) {
            collateSearchParams(url);
        };

        vm.createUserAccount = function (url, event) {
            if (!$(event.currentTarget).valid()) {
                notifier('warning', 'sorry!', 'Cannot create account with incomplete fields.');
                return;
            }

            CreateUser(url, vm.user, event);
        };

        vm.editUserAccount = function (url, event) {
            if (!$(event.currentTarget).valid()) {
                notifier('warning', 'sorry!', 'Cannot update account with incomplete fields.');
                return;
            }

            CreateUser(url, vm.user);
        }

        vm.changePassword = function (url, event) {

            if (!$(event.currentTarget).valid()) {
                notifier('warning', 'sorry!', 'Cannot update password with incomplete fields.');
                return;
            }

            CreateUser(url, vm.user);
        }

        vm.deleteSingle = function (id, url) {
            var ids = [];
            ids.push(id);
            mainDelete(url, ids, false);
        };

        vm.deleteMultiple = function (url) {
            mainDelete(url, vm.checks, true);
        };

        vm.getSerialNo = function (index) {
            return (vm.itemsOnPage * (vm.pageIndex - 1)) + index;
        };

        vm.setItemPerPage = function (count) {
            vm.itemsOnPage = count;
        };

        vm.searchPage = function (formId) {
            vm.pageIndex = 1;
            collateSearchParams(cachedUrl, formId);
        };

        function CreateUser(url, model, event) {

            myBlockUI = blockUI.instances.get('userCreateBlockUI');
            myBlockUI.start();

            request = rq.post(url, model);
            request.then(function (response) {

                removeBlock()

                if (response.errorStatus) {
                    notifier('error', 'Opps!', response.errorMessage);
                    return;
                }
                else 
                    notifier('success', 'Done!', response.message);

                if (event) {
                    formClear(event);
                    $('.select2').val(null).trigger("change");
                    vm.user = {};
                }

            }, ApiErrorCallbk);
        }

        function collateSearchParams(url, formId) {
            if (!url)
                notifier('error', 'Error', 'request is invalid');

            cachedUrl = url;
            cachedFormId = formId;
            query.url = cachedUrl;
            query.data = {
                pageIndex: vm.pageIndex,
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

            myBlockUI = blockUI.instances.get('userCreateBlockUI');
            myBlockUI.start();

            rq.post(data.url, data.data).then(function (response) {
                removeBlock();

                if (response.errorStatus) {
                    notifier('error', 'Error', response.errorMessage)
                    return;
                }
                if (response.result) {
                    vm.users = response.result;
                    vm.total = response.additionalResult || 0;
                }
                else {
                    if (response.message)
                        notifier('warning', 'Opps!', response.message)
                }
            }, ApiErrorCallbk)
        };

        function mainDelete(url, ids) {
            if (ids.length <= 0) {
                notifier('warning', 'error', "confirm data to delete.")
                return;
            }

            if (!confirm('Are you sure want to delete the selected items ?'))
                return;

            myBlockUI = blockUI.instances.get('userCreateBlockUI');
            myBlockUI.start();

            rq.post(url, ids)
                .then(function (response, status) {
                    removeBlock();

                    if (response.errorStatus) {
                        notifier('error', 'error', response.errorMessage);
                    }

                    if (!response.result || response.result.length <= 0)
                        return;

                    var deleteCount = 0;
                    angular.element(response.result).each(function (index, id) {
                        vm.users.remove(findUserIndex(id));
                        deleteCount += 1;
                    });

                    var a = deleteCount > 1 ? deleteCount + "  records" : deleteCount + "  record";
                    notifier('success', 'Done', 'successfully deleted  ' + a);
                    vm.total -= deleteCount;
                    vm.checks.length = 0;
                },
               ApiErrorCallbk);
        };

        function removeBlock() {

            if (myBlockUI != null && (myBlockUI.stop && typeof (myBlockUI.stop) == 'function')) {
                $timeout(function () {
                    // Stop the block after some async operation.
                    myBlockUI.stop();
                }, 1000);
            }
        }

        function ApiErrorCallbk(response) {
            if (typeof (response) === 'object') {
                if (response.errorMessage)
                    notifier('error', 'Error', response.errorMessage);
            }
            else
                notifier('error', 'Error', response);
        };

        function findUserIndex(id) {
            var currentIndex;
            vm.users.some(function (item, index) {
                if (item.id === id) {
                    currentIndex = index;
                    return true;
                }
            });
            return currentIndex;
        };
    }]);
})();