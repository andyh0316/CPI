﻿var app = angular.module('UserApp', ['AngularBaseModule', 'ui.router']);

app.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.when("", "/List");

    var listScopeData = {
        filter: { Loads: 0, SortObjects: [{ ColumnName: 'Fullname', IsDescending: false }]},
        httpRequest: { method: 'POST', url: '/Manage/User/GetList' }
    };

    $stateProvider
        .state('List', {
            url: '/List',
            templateUrl: '/Areas/Manage/Views/User/List.html',
            controller: 'ListController',
            resolve: {
                model: ['$stateParams', 'baseBo', function ($stateParams, baseBo) {
                    return baseBo.httpRequest(listScopeData.httpRequest.method, listScopeData.httpRequest.url, listScopeData.filter);
                }],
                modelData: ['$stateParams', 'baseBo', function ($stateParams, baseBo) {
                    return baseBo.httpRequest('GET', '/Manage/User/GetListData');
                }],
                scopeData: function () {
                    return listScopeData;
                }
            }
        })
        .state('List.User', {
            url: '/User/:mode/:userId/',
            templateUrl: '/Areas/Manage/Views/User/User.html',
            controller: 'UserController',
            resolve: {
                model: ['$stateParams', 'baseBo', function ($stateParams, baseBo) {
                    return baseBo.httpRequest('GET', '/Manage/User/GetUser', { id: $stateParams.userId });
                }],
                modelData: ['$stateParams', 'baseBo', function ($stateParams, baseBo) {
                    return baseBo.httpRequest('GET', '/Manage/User/GetUserData');
                }],
                mode: ['$stateParams', function ($stateParams) {
                    return $stateParams.mode;
                }],
            }
        })
}]);

app.controller('ListController', ['$scope', '$controller', '$state', 'baseBo', 'model', 'scopeData', 'modelData', function ($scope, $controller, $state, baseBo, model, scopeData, modelData) {
    angular.extend(this, $controller('ListBaseController', { $scope: $scope }));

    $scope.scopeData = scopeData;
    $scope.modelData = modelData.Object;
    $scope.model = model.Object;

    $scope.createUser = function () {
        $state.go('List.User', { 'mode': $scope.createMode, 'userId': 0 });
    };

    $scope.viewUser = function (userId) {
        $state.go('List.User', { 'mode': $scope.viewMode, 'userId': userId });
    };
}]);

app.controller('UserController', ['$scope', '$controller', '$state', 'baseBo', 'mode', 'model', 'modelData', function ($scope, $controller, $state, baseBo, mode, model, modelData) {
    angular.extend(this, $controller('BaseController', { $scope: $scope }));

    $scope.modelData = modelData.Object;
    $scope.model = model.Object;
    $scope.setMode(mode);

    $scope.remove = function () {
        if (confirm(gConfirmDeleteMsg)) {
            baseBo.httpRequest('GET', '/Manage/User/DeleteUser', { id: $scope.model.Id })
                .then(function (result) {
                    $scope.$emit('reloadListEvent', {});
                    $scope.back();
                    $scope.setNotification();
                });
        }
    };

    $scope.save = function () {
        baseBo.httpRequest('POST', '/Manage/User/SaveUser/', $scope.model).then(function (result) {
            if (result.ModelState) {
                $scope.modelState = result.ModelState;
            }
            else
            {
                $scope.$emit('reloadListEvent', {});
                $scope.back();
                $scope.setNotification();
            }
        });
    };
}]);

