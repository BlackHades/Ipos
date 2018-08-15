(function () {

    window.app.config(function ($stateProvider, $urlRouterProvider, $locationProvider) {

        $locationProvider.html5Mode(false);
        $locationProvider.hashPrefix('!');

        $stateProvider.state('boughtitemslist', {
            url: '/boughtitemslist',
            templateUrl: 'home/recentboughtitems',
            controller: 'dashCtrl'
        })
            .state('boughtitemdetails', {
                url: '/boughtitemdetails/:itemid',
                templateUrl: 'home/RecentBoughtItemDetails',
                controller: 'orderDetailCtrl',
                resolve: {
                    orderDetails: function ($stateParams, $q, requestSvc) {

                        var deferred = $q.defer();

                        requestSvc.get('/api/IPosReportApi/GetDashboardSales?ordt=' + $stateParams.itemid).then(function (r) {

                            deferred.resolve(r);
                        }, function (e) {

                            deferred.reject(e);
                        });

                        return deferred.promise;
                    }
                }
            })


        $urlRouterProvider.otherwise('/boughtitemslist');

    });

    window.app.controller('orderDetailCtrl', ['$scope', 'orderDetails', '$state', function (vm, orderDetails, $state) {

        vm.items = orderDetails;

        vm.goBack = function () {

            $state.go('boughtitemslist');
        };
    }])
    window.app.controller('dashCtrl', ['$scope', 'requestSvc', '$timeout', '$stateParams', function (vm, rq, timeout, $stateParams) {

        vm.faultyItems = [];
        vm.restockItems = [];
        vm.boughtItems = [];
        vm.transactions = {
            totalTodaySales: 0,
            totalWeekSales: 0,
            totalTransactionCount: 0
        };

        vm.getBoughtItems = function (url) {

            url = url + '?ordUid=' + $stateParams.itemid;
            rq.get(url);

        };

        vm.recallbtn = function () {

            requestSvc(url, vm.checks).then(function (data) {
                if (data.errorStatus) {

                    alert(data.errorMessage);
                    return;
                }
                for (var i = 0; i < vm.checks; i++) {
                    vm.checks.remove(i);
                }
            });
        }

        vm.getFaultyItems = function (url) {

            url = url + '?spoilId=' + $stateParams.itemid;
            rq.get(url);

        };

        vm.getTopFaultyItems = function (url) {
            rq.get(url).then(function (data) {

                if (data.errorStatus) {

                    alert(data.errorMessage);
                    return;
                }

                vm.faultyItems = data.result;

            }, function (error) {

                alert(error);
            });
        }

        vm.getRestockItems = function (url) {
            rq.get(url).then(function (data) {

                if (data.errorStatus) {

                    alert(data.errorMessage);
                    return;
                }

                vm.restockItems = data.result;

            }, function (error) {

                alert(error);
            });
        }

        var iso8601RegEx = /(19|20|21)\d\d([-/.])(0[1-9]|1[012])\2(0[1-9]|[12][0-9]|3[01])T(\d\d)([:/.])(\d\d)([:/.])(\d\d)/;

        vm.fnConverDate = function (input) {
            if (typeof input !== "object") return input;

            for (var key in input) {
                if (!input.hasOwnProperty(key)) continue;

                var value = input[key];
                var type = typeof value;
                var match;
                if (type == 'string' && (match = value.match(iso8601RegEx))) {
                    input[key] = new Date(value)
                }
                else if (type === "object") {
                    fnConverDate(value);
                }
            }
        };

        vm.getTopItemsBought = function (url) {

            rq.get(url).then(function (data) {

                if (data.errorStatus) {

                    alert(data.errorMessage);
                    return;
                }

                vm.boughtItems = data.result;

            }, function (error) {

                alert(error);
            });

        };
        vm.initSalesTransaction = function (url) {

            getTransactionAggregates(url);
        }

        var getTransactionAggregates = function (url) {

            rq.get(url).then(function (data) {


                if (data.errorStatus) {
                    return;
                }

                vm.transactions = data.result;

                timeout(function () {
                    $('.counter').counterUp({
                        delay: 100,
                        time: 1200
                    });

                })

            }, function (dataError) {

            });
        }
        vm.getItemDetails = function (url) {

            //url = url + '?ordUid=' + $stateParams.itemid;

            rq.get(url).then(function (data) {

                if (data.errorStatus) {

                    alert(data.errorMessage);
                    return;
                }

                vm.itemDetails = data.result;
                // showModalDialog(url, data)
                $('#myModal4').modal('show');

            }, function (error) {

                alert(error);
            });



        };
        vm.ignoreVal = function (url) {


            //   url = url + '?prdUid=' + $stateParams.itemid;
            rq.get(url).then(function (data) {

                if (data.errorStatus) {

                    alert(data.errorMessage);
                    return;
                }

                vm.restockItems = data.result;

            }, function (error) {

                alert(error);
            });



        };

        vm.getItemDetailsForRecall = function (url) {

            //url = url + '?ordUid=' + $stateParams.itemid;

            rq.get(url).then(function (data) {

                if (data.errorStatus) {

                    alert(data.errorMessage);
                    return;
                }

                vm.itemDetails = data.result;
                // showModalDialog(url, data)
                $('#myModal5').modal('show');

            }, function (error) {

                alert(error);
            });
        };

        vm.formatUserName = function (username) {

            if (username && username.indexOf('@') !== -1)
                username = username.substr(0, username.indexOf('@'));

            return username;
        };
    }]);
})();