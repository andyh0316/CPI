﻿var baseModule = angular.module('AngularBaseModule', []);

baseModule.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.defaults.headers.common['Cache-Control'] = 'no-cache, no-store, must-revalidate';
    $httpProvider.defaults.headers.common['Pragma'] = 'no-cache';
    $httpProvider.defaults.headers.common['Expires'] = '0';
    $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
}]);


baseModule.controller('BaseController', ['$scope', '$state', function ($scope, $state) {
}]);

baseModule.factory('baseBo', ['$http', '$window', '$q', function ($http, $window, $q) {
    var instance = {};

    var currentRequests = 0;

    // Caution: all posts that attempt to save should preferrably pass in a whole model object instead of objects of different models
    // (ex. httpPost(url, $scope.model) instead of httpPost(url, { model: $scope.model })
    // this is because so that ModelState will not prefix keys with parameter name so our angular apps
    // can properly bind to fields to display validation errors generated by MVC
    instance.httpRequest = function (httpMethod, url, params, opts) {
        if (opts && opts.noLoadIcon === true) {
        }
        else {
            $('#wait-mask').show();
            currentRequests++;
        }

        if (httpMethod == 'GET') {
            var httpVar = { method: 'GET', url: url, params: params };
        } else if (httpMethod == 'POST') {
            var httpVar = { method: 'POST', url: url, data: params };
        } else if (httpMethod == 'UPLOAD') {
            var httpVar = {
                method: 'POST', url: url, data: params,
                withCredentials: true, headers: { 'Content-Type': undefined }, transformRequest: angular.identity
            };
        }

        return instance.httpBase(httpVar);
    };

    instance.httpBase = function (httpVar) {
        var deferred = $q.defer();

        $http(httpVar)
            .success(function (result) {

                if (result.IsSessionExpired) {
                    location.href = '/Public/Login/';
                    return;
                };

                // tell javascript how much time is remaining for session timeout so we can warn the user
                // to extend his session
                //setSessionWarning(result.SessionTimeLeft);

                // need to check for if its session timeout
                //successFunc(result);
                deferred.resolve(result);

                currentRequests--;
                if (currentRequests <= 0) {
                    currentRequests = 0;
                    $('#wait-mask').hide();
                }
            })
            .error(function (data, status, headers, config) {
                currentRequests--;
                if (currentRequests <= 0) {
                    currentRequests = 0;
                    $('#wait-mask').hide();
                }

                alert("Oops! The application has encountered an unexpected error. Please sign out and sign in again. If this the problem persists, please report to us.");
            });

        return deferred.promise;
    };

    return instance;
}]);
