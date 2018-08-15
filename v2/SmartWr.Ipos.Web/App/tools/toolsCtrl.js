(function () {
    app.controller('toolsCtrl', function ($scope, requestSvc, filterFilter, $stateParams, $state, notifier, formClear, blockUI, $timeout) {
        var myBlockUI = '';
        var vm = this, query = {}, cachedUrl, $selectize;
        vm.total = 0;
        vm.wastes = [];
        vm.audits = [];
        vm.waste = {};
        vm.itemsOnPage = 50;
        vm.pageIndex = 1;
        vm.email = {};
        vm.sms = {};

        function connectionIsAvailable() {
            if (!window.navigator.onLine) {
                notifier('warning', 'offline', 'email functionality is not available. Please connect to the internet.');
                return false;
            }
            return true;
        };

        if ($state.$current.name == 'email.compose') {
            var REGEX_EMAIL = '([a-z0-9!#$%&\'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&\'*+/=?^_`{|}~-]+)*@' +
                  '(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)';

            $selectize = $("#receipients").selectize({
                delimiter: ',',
                persist: false,
                create: function (input) {
                    if ((new RegExp('^' + REGEX_EMAIL + '$', 'i')).test(input)) {
                        return { value: input, text: input };
                    } else {
                        notifier('warning', 'Opps!', 'Invalid email address.');
                        return false;
                    }
                },
                createFilter: function (input) {
                    var match, regex;

                    regex = new RegExp('^' + REGEX_EMAIL + '$', 'i');
                    match = input.match(regex);
                    if (match) return !this.options.hasOwnProperty(match[0]);
                    return false;
                }
            });
        }

        vm.sendSms = function (event, url) {
            if (!$(event.target).valid()) {
                notifier('warning', 'Opps!', 'Invalid inputs. Please check and try again.');
                return;
            }

            var formData = new FormData();
            //formData.append('file', vm.sms.receipientList);

            Object.getOwnPropertyNames(vm.sms).forEach(function (item, idx, obj) {
                formData.append(item, vm.sms[item]);
            });

            requestSvc.post(url, formData, { 'Content-Type': undefined }, angular.identity)
                .then(function (response) {
                    if (response.errorStatus) {
                        notifier('error', 'Error!', response.errorMessage);
                        return;
                    } else {

                        notifier('success', 'Done!', response.message);
                        formClear(event);
                    }
                }, ApiErrorCallbk)
        }

        vm.sendMail = function (event, url) {
            var msg = $('#message').val(),
                receipients = $('#receipients').val();

            if (!$(event.target).valid() || !msg) {
                notifier('warning', 'Opps!', 'Invalid inputs. Please check and try again.');
                return;
            }

            if (!connectionIsAvailable())
                return;
            

            vm.email.message = msg;
            vm.email.receipients = receipients;

            requestSvc.post(url, vm.email).then(function (response) {

                if (response.errorStatus) {
                    notifier('error', 'Error!', response.errorMessage);
                    return;
                }
                else {
                    notifier('success', 'Done!', response.message);
                    formClear(event);
                    vm.email = {};
                    $selectize[0].selectize.clear();
                }
            }, ApiErrorCallbk);
        }

        vm.getWastes = function (url) {
            collateSearchParams(url);
        };

        vm.setItemPerPage = function (count) {
            vm.itemsOnPage = count;
        };

        vm.getSerialNo = function (index) {
            return (vm.itemsOnPage * (vm.pageIndex - 1)) + index;
        };

        vm.editWaste = function (index, selector) {
            var waste = vm.wastes[index];

            if (jQuery.isEmptyObject(waste)) {
                notifier('warning', 'No!', 'Please confirm your selection.');
                return;
            }
            angular.copy(waste, vm.waste);
            jQuery(selector).modal('show');
        };

        vm.submitEditedWasteItem = function (url) {

            myBlockUI = blockUI.instances.get('toolsAuditBlockUI');
            myBlockUI.start();

            requestSvc.post(url, vm.waste).then(function (response) {
                removeBlock();
                if (response.errorStatus) {
                    notifier('error', 'Error', response.errorMessage);
                    return;
                }
                else {
                    notifier('success', 'Done', response.message);
                    result = response.result,
                     waste = findWaste(result.spoilId);
                    waste.quantity = result.quantity;
                }

                hideModal();
            }, ApiErrorCallbk)
        };

        vm.pageChanged = function (page, formId) {
            //if (vm.pageIndex === page)
            //    return;
            vm.pageIndex = page;

            collateSearchParams(cachedUrl, formId);
        };

        vm.searchPage = function (formId) {
            vm.pageIndex = 1;
            collateSearchParams(cachedUrl, formId);
        };

        function collateSearchParams(url, formId) {
            if (!url)
                notifier('error', 'Error', 'request is invalid');

            cachedUrl = url;
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
            myBlockUI = blockUI.instances.get('toolsAuditBlockUI');
            myBlockUI.start();

            requestSvc.post(data.url, data.data).then(function (response) {

                removeBlock();
                if (response.errorStatus) {
                    notifier('error', 'Error', response.errorMessage)
                    return;
                }

                if ($state.$current.name === 'audit')
                    vm.audits = response.result;
                else
                    vm.wastes = response.result;

                vm.total = response.additionalResult || 0;
            }, ApiErrorCallbk);
        };

        function ApiErrorCallbk(response) {
            if (typeof (response) === 'object') {
                if (response.message)
                    notifier('error', 'Error', response.message);
            }
            else
                notifier('error', 'Error', response);
        }

        function hideModal() {
            $('.modal').modal('hide');
        };

        function removeBlock() {

            if (myBlockUI != null && (myBlockUI.stop && typeof (myBlockUI.stop) == 'function')) {
                $timeout(function () {
                    // Stop the block after some async operation.
                    myBlockUI.stop();
                }, 1000);
            }
        }

        function findWaste(id) {
            var currentitem;
            vm.wastes.some(function (item, index) {
                if (item.spoilId === id) {
                    currentitem = item;
                    return true;
                }
            });
            return currentitem;
        };

    });
})();