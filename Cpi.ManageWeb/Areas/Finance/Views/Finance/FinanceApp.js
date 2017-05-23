var app = angular.module('FinanceApp', ['AngularBaseModule', 'chart.js', 'ui.router']);

app.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.when("", "/Finance");

    var financeScopeData = {
        filter: { ReportDateId: 1 },
        httpRequest: { method: 'POST', url: '/Finance/Finance/GetFinance' }
    };

    var listScopeData = {
        filter: { Loads: 0, SortColumn: "CreatedDate", SortDesc: true },
        httpRequest: { method: 'POST', url: '/Finance/Finance/GetFinanceList' }
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
        .state('Finance.List', {
            url: '/List',
            templateUrl: '/Areas/Finance/Views/Finance/List.html',
            controller: 'ListController',
            resolve: {
                model: ['$stateParams', 'baseBo', function ($stateParams, baseBo) {
                    return baseBo.httpRequest(listScopeData.httpRequest.method, listScopeData.httpRequest.url, listScopeData.filter);
                }],
                scopeData: function () {
                    return listScopeData;
                }
            }
        })
}]);

app.controller('FinanceController', ['$scope', '$controller', '$state', 'baseBo', 'model', 'scopeData', 'modelData', function ($scope, $controller, $state, baseBo, model, scopeData, modelData) {
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

    $scope.goListView = function () {
        $state.go('Finance.List');
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

app.controller('ListController', ['$scope', '$controller', '$state', 'baseBo', 'model', 'scopeData', function ($scope, $controller, $state, baseBo, model, scopeData) {
    angular.extend(this, $controller('ListBaseController', { $scope: $scope }));

    $scope.scopeData = scopeData;
    $scope.model = model.Object;
}]);