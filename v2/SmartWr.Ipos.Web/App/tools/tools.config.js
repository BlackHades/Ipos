/// <reference path="../app.js" />

'use strict'

window.app.config(function ($stateProvider, $urlRouterProvider) {
    $stateProvider
        .state('main', {
            url: '/wasteditems',
            templateUrl: "wasteditems/" + (new Date()).getTime(),
            controller: 'toolsCtrl',
            controllerAs: 'vm'
        })
        .state('audit', {
            url: '/auditlog',
            templateUrl: 'audittrail/' + (new Date()).getTime(),
            controller: 'toolsCtrl',
            controllerAs: 'vm'
        })
        .state('sms', {
            url: '/sms',
            templateUrl: 'smsapp/' + (new Date()).getTime(),
            controller: 'toolsCtrl',
            controllerAs: 'vm'
        })
        .state('email', {
            url: '/emailapp',
            templateUrl: 'emailapp/' + (new Date()).getTime(),
            abstract: true,
        })
        .state('email.compose', {
            url: '/compose',
            templateUrl: 'composeemail/' + (new Date()).getTime(),
            controller: 'toolsCtrl',
            controllerAs: 'vm',
        })
    .state('email.sent', {
        url: '/sent',
        templateUrl: 'sentemails/' + (new Date()).getTime(),
        controller: 'toolsCtrl',
        controllerAs: 'vm',
    });

    $urlRouterProvider.otherwise('/wasteditems');
})