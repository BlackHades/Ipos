(function (window, angular, $) {
    'use strict'

    var angularApp = angular.module('iposApp', ['ui.router', 'angular-loading-bar', 'blockUI', 'angularUtils.directives.dirPagination']);

    Array.prototype.remove = function (from, to) {
        var rest = this.slice((to || from) + 1 || this.length);
        this.length = from < 0 ? this.length + from : from;
        return this.push.apply(this, rest);
    };

    angularApp.config(function ($locationProvider, $httpProvider, blockUIConfig, paginationTemplateProvider) {
        $locationProvider.html5Mode(false);
        $locationProvider.hashPrefix('!');
        $httpProvider.defaults.headers.common['X-Requested-With'] = 'XMLHttpRequest';

        blockUIConfig.message = 'Working...';
        blockUIConfig.delay = 0;
        paginationTemplateProvider.setPath(window.app.appUrl + 'scripts/dirpagination.tpl.html');
    });

    angularApp.value('formClear', function (event) {
        typeof (event.currentTarget.form) === 'undefined' ? angular.element(event.currentTarget).get(0).reset() : angular.element(event.currentTarget.form)[0].reset();
    })
       .value('notifier', function (type, header, message) {
           $.Notification.notify(type, 'right top', header, message);
       });

    angular.extend(window.app, angularApp)
})(window, angular, jQuery);