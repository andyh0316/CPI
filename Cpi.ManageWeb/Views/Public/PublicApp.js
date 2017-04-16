﻿var app = angular.module('PublicApp', ['AngularBaseModule', 'ui.router', 'ngAnimate']);

app.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.when("", "/Login");

    var listScopeData = {
        filter: { Loads: 0, SortColumn: "CreatedDate", SortDesc: true },
        httpRequest: { method: 'POST', url: '/Public/Login' }
    };

    $stateProvider
        .state('Login', {
            url: '/Login',
            templateUrl: '/Views/Public/Login.html',
            controller: 'LoginController',
        })
        //.state('List.View', {
        //    url: '/Import/',
        //    templateUrl: '/Areas/Public/Views/Public/Import.html',
        //    controller: 'ImportController'
        //})
}]);

app.controller('LoginController', ['$scope', '$controller', '$state', 'baseBo', function ($scope, $controller, $state, baseBo) {
    $scope.model = {};

    $scope.login = function () {
        baseBo.httpRequest('POST', '/Public/Login', $scope.model)
            .then(function (result) {
                if (result.ModelState)
                {
                    $scope.modelState = result.ModelState;
                }
                else {
                    if (result.Object.IsUpdatingPassword)
                    {
                        $scope.model.IsUpdatingPassword = result.Object.IsUpdatingPassword;
                    }

                    if (result.Object.LoginSuccess)
                    {
                        location.href = '/Call/Call';
                    }
                }
            });
    };
}]);