var app = angular.module('PublicApp', ['AngularBaseModule', 'ui.router', 'ngAnimate']);

app.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.when("", "/Login");

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

app.controller('LoginController', ['$scope', '$controller', '$state', '$timeout', 'baseBo', function ($scope, $controller, $state, $timeout, baseBo) {
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

    //$scope.clearModel = function () {
    //    $scope.model.Username = null;
    //    $scope.model.Password = null;
    //};

    //$timeout($scope.clearModel, 1000);
}]);