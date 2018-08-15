/// <reference path="../app.auth.js" />


'use strict'

window.app.config(function ($stateProvider, $urlRouterProvider) {

    $stateProvider.state('app', {
        url: '/useraccounts',
        templateUrl: "userlist/" + (new Date()).getTime(),
        controller: 'loginCtrl',
    })
        .state('createUserAccount', {
            url: '/createuseraccount',
            templateUrl: 'createuseraccount/' + (new Date()).getTime(),
            controller: 'loginCtrl',
        })
        .state('editUserAccount', {
            url: '/edituseraccount/?:accountId',
            templateUrl: 'useraccountedit/' + (new Date()).getTime(),
            controller: 'loginCtrl',
        })
        .state('changeUserPassword', {
            url: '/changeuserpassword/?:accountId',
            templateUrl: 'userpasswordchange/' + (new Date()).getTime(),
            controller: 'loginCtrl',
        })
      
    .state('viewUserAccount', {
        url: '/accountdetails/?:accountId',
        templateUrl: 'useraccountview/' + (new Date()).getTime(),
        controller: 'loginCtrl',
    })
    $urlRouterProvider.otherwise('/users');
});