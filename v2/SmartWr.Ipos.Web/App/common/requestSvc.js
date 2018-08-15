window.app.service('requestSvc', ['$http', '$q',

    function requestSvc($http, $q) {

        function performget(url) {
            var config = {
                url: url,
                method: 'GET'
            };

            var deferred = $q.defer();

            $http(config)
              .success(function (data, status) {

                  if (status >= 200 && status <= 202) {

                      deferred.resolve(data, status);
                      return;
                  }

                  deferred.reject(data);

              }).error(function (status) {
                  deferred.reject(status);
              });

            return deferred.promise;
        }

        function performpost(url, data, headers, transformer) {

            var deferred = $q.defer();

            var config = {
                url: url,
                method: 'POST',
                data: data,
                timeout: deferred.promise,
                headers: headers || {},
                transformRequest: transformer || $http.defaults.transformRequest
            };

            $http(config)
              .success(function (data, status) {

                  if (status >= 200 && status <= 202) {

                      deferred.resolve(data, status);
                      return;
                  }

                  deferred.reject(data);

              }).error(function (status) {
                  deferred.reject(status);
              });

            return deferred.promise;
        }

        this.post = performpost;
        this.get = performget;
    }]);
