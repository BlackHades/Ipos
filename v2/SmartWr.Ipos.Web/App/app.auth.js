(function (window, angular, $) {

    window.app = angular.module('iposApp', ['angular-loading-bar', 'blockUI', 'ui.router'])
        .value('notifier', function (type, header, message) {
            $.Notification.notify(type, 'right top', header, message);
        })
    .value('formClear', function (event) {
        typeof (event.currentTarget.form) === 'undefined' ? angular.element(event.currentTarget).get(0).reset() : angular.element(event.currentTarget.form)[0].reset();
    });

    app.config(function (blockUIConfig, $locationProvider, $httpProvider) {
        $locationProvider.html5Mode(false);
        $locationProvider.hashPrefix('!');
        $httpProvider.defaults.headers.common['X-Requested-With'] = 'XMLHttpRequest';

        blockUIConfig.message = 'Working...';
    });
})(window, angular, jQuery);