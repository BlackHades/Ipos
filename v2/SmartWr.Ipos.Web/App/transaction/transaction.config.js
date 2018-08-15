/// <reference path="../app.js" />
'use strict'

window.app.config(function ($stateProvider, $urlRouterProvider) {

    $stateProvider.state('main', {
        url: '/transactions',
        templateUrl: "saleshistory/" + (new Date()).getTime(),
        controller: 'transactionCtrl',
        controllerAs: 'transaction'
    })
        .state('viewOrderDetail', {
            url: '/transactiondetails/:id',
            templateUrl: "transactiondetails/" + (new Date()).getTime(),
            controller: 'transactionCtrl',
            controllerAs: 'vm'
        })
        .state('pendingpost', {
            url: '/pendingpost',
            templateUrl: "pendingpost/" + (new Date()).getTime(),
            controller: 'transactionCtrl',
            controllerAs: 'vm'
        });
    $urlRouterProvider.otherwise('/transactions');
});