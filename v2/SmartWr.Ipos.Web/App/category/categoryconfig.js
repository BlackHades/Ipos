/// <reference path="../app.js" />

'use strict'
window.app.config(function ($stateProvider, $urlRouterProvider) {

    $stateProvider.state('list', {

        url: '/categories',
        //url: '/Existing_Categories',
        templateUrl: "listcategory/" + (new Date()).getTime(),
        controller: 'categoryCtrl'
    })
     .state('create', {
         url: '/createcategory',
         templateUrl: 'categorycreate',
         controller: 'categoryCtrl'
     })

    .state('viewdetail', {
        url: '/categorydetails/:id',
        templateUrl: 'categoryviewdetail',
        controller: 'categoryCtrl'
    })

    .state('edit', {
        url:'/editcategory/:id',
        templateUrl: 'categoryedit',
        controller: 'categoryCtrl'
    })
    $urlRouterProvider.otherwise('/categories');
})