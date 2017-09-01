var app = angular.module('FinanceOverviewApp', ['AngularBaseModule', 'chart.js', 'ui.router']);

app.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.when("", "/FinanceOverview");

    var financeScopeData = {
        filter: { LocationId: 1, ReportDateFilter: { ReportDateId: 1 }},
        httpRequest: { method: 'POST', url: '/Report/FinanceOverview/GetFinanceOverview' }
    };

    $stateProvider
        .state('FinanceOverview', {
            url: '/FinanceOverview',
            templateUrl: '/Areas/Report/Views/FinanceOverview/FinanceOverview.html',
            controller: 'FinanceOverviewController',
            resolve: {
                model: ['$stateParams', 'baseBo', function ($stateParams, baseBo) {
                    return baseBo.httpRequest(financeScopeData.httpRequest.method, financeScopeData.httpRequest.url, financeScopeData.filter);
                }],
                modelData: ['$stateParams', 'baseBo', function ($stateParams, baseBo) {
                    return baseBo.httpRequest('GET', '/Report/FinanceOverview/GetFinanceOverviewData');
                }],
                scopeData: function () {
                    return financeScopeData;
                }
            }
        })
}]);

app.controller('FinanceOverviewController', ['$scope', '$controller', '$state', 'baseBo', 'model', 'scopeData', 'modelData', function ($scope, $controller, $state, baseBo, model, scopeData, modelData) {
    angular.extend(this, $controller('BaseController', { $scope: $scope }));
    
    $scope.scopeData = scopeData;
    $scope.modelData = modelData.Object;
    $scope.model = model.Object;

    $scope.filterSearchTimeout = null;
    $scope.$watch('scopeData.filter', function (newVal, oldVal) {
        if (newVal !== oldVal)
        {
            var filterSearchDelay = 0;
            if ($scope.delayFilterSearch) {
                $scope.delayFilterSearch = false;
                filterSearchDelay = 1000;
            }

            clearTimeout($scope.filterSearchTimeout);
            $scope.filterSearchTimeout = setTimeout(function () {
                $scope.getFinance();
            }, filterSearchDelay);
        }
    }, true);

    $scope.getFinance = function () {
        baseBo.httpRequest($scope.scopeData.httpRequest.method, $scope.scopeData.httpRequest.url, $scope.scopeData.filter)
            .then(function (result) {
                $scope.model = result.Object;
                $scope.setGraphData();
            });
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

        if ($scope.model.Products)
        {
            $scope.productNames = $scope.model.Products.map(function (a) { return a.Item1; });
            $scope.productCounts =
                [
                    $scope.model.Products.map(function (a) { return a.Item2; }),
                    $scope.model.Products.map(function (a) { return a.Item3; }),
                ];
        }
    }

    $scope.setGraphData();
}]);