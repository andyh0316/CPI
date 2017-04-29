﻿var app = angular.module('FinanceApp', ['AngularBaseModule', 'chart.js', 'ui.router']);

app.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.when("", "/Finance");

    var financeScopeData = {
        filter: { ReportDateId: 1 },
        httpRequest: { method: 'POST', url: '/Finance/Finance/GetFinance' }
    };

    $stateProvider
        .state('Finance', {
            url: '/Finance',
            templateUrl: '/Areas/Finance/Views/Finance/Finance.html',
            controller: 'FinanceController',
            resolve: {
                model: ['$stateParams', 'baseBo', function ($stateParams, baseBo) {
                    return baseBo.httpRequest(financeScopeData.httpRequest.method, financeScopeData.httpRequest.url, financeScopeData.filter);
                }],
                modelData: ['$stateParams', 'baseBo', function ($stateParams, baseBo) {
                    return baseBo.httpRequest('GET', '/Finance/Finance/GetFinanceData');
                }],
                scopeData: function () {
                    return financeScopeData;
                }
            }
        })
}]);

app.controller('FinanceController', ['$scope', '$controller', '$state', 'baseBo', 'model', 'scopeData', 'modelData', function ($scope, $controller, $state, baseBo, model, scopeData, modelData) {
    angular.extend(this, $controller('BaseController', { $scope: $scope }));
    
    $scope.scopeData = scopeData;
    $scope.modelData = modelData.Object;
    $scope.model = model.Object;

    $scope.$watch('scopeData.filter', function (newVal, oldVal) {
        if (newVal !== oldVal)
        {
            baseBo.httpRequest($scope.scopeData.httpRequest.method, $scope.scopeData.httpRequest.url, $scope.scopeData.filter)
                .then(function (result) {
                    $scope.model = result.Object;
                    $scope.setGraphData();
                });
        }
    }, true);

    $scope.getReportDateName = function () {
        for (var i in $scope.modelData.ReportDates)
        {
            if ($scope.scopeData.filter.ReportDateId === $scope.modelData.ReportDates[i].Id)
            {
                return $scope.modelData.ReportDates[i].Name;
            }
        }
    };

    $scope.setGraphData = function () {
        if ($scope.model.Revenues)
        {
            $scope.revenueDates = $scope.model.Revenues.map(function (a) { return a.Item1; });
            $scope.revenueNumbers = $scope.model.Revenues.map(function (a) { return a.Item2; });
        }

        if ($scope.model.Calls)
        {
            $scope.callDates = $scope.model.Calls.map(function (a) { return a.Item1; });
            $scope.callCounts =
                [
                    $scope.model.Calls.map(function (a) { return a.Item2; }),
                    $scope.model.Calls.map(function (a) { return a.Item3; }),
                ];
        }
    }

    $scope.setGraphData();
}]);