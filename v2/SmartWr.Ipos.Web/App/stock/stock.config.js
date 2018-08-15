/// <reference path="../app.js" />

'use strict'

window.app.config(function ($stateProvider, $urlRouterProvider) {

    $stateProvider.state('list', {
        url: '/stocks',
        templateUrl: "listproducts/" + (new Date()).getTime(),
        controller: 'stockCtrl',
    })
    .state('create', {
        url: '/createstock',
        templateUrl: 'stockcreate/' + (new Date()).getTime(),
        controller: 'stockCtrl'
    })

    .state('edit', {
        url: '/editstock/:id',
        templateUrl: 'stockedit/' + (new Date()).getTime(),
        controller: 'stockCtrl'
    })

    .state('viewdetail', {
        url: '/stockdetails/:id',
        templateUrl: 'stockviewdetail/' + (new Date()).getTime(),
        controller: 'stockCtrl'
    })

    $urlRouterProvider.otherwise('/stocks');
});