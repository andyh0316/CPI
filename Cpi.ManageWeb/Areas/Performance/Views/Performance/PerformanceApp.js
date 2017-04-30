var app = angular.module('PerformanceApp', ['AngularBaseModule', 'chart.js', 'ui.router']);

app.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.when("", "/Performance");

    var listScopeData = {
        filter: { ReportDateId: 1 },
        httpRequest: { method: 'GET', url: '/Performance/Performance/GetPerformance' }
    };

    $stateProvider
        .state('Performance', {
            url: '/Performance',
            templateUrl: '/Areas/Performance/Views/Performance/Performance.html',
            controller: 'PerformanceController',
            resolve: {
                model: ['$stateParams', 'baseBo', function ($stateParams, baseBo) {
                    return baseBo.httpRequest(listScopeData.httpRequest.method, listScopeData.httpRequest.url, listScopeData.filter);
                }],
                modelData: ['$stateParams', 'baseBo', function ($stateParams, baseBo) {
                    return baseBo.httpRequest('GET', '/Performance/Performance/GetPerformanceData');
                }],
                scopeData: function () {
                    return listScopeData;
                }
            }
        })
}]);

app.controller('PerformanceController', ['$scope', '$controller', '$state', 'baseBo', 'model', 'scopeData', 'modelData', function ($scope, $controller, $state, baseBo, model, scopeData, modelData) {
    angular.extend(this, $controller('BaseController', { $scope: $scope }));

    $scope.scopeData = scopeData;
    $scope.modelData = modelData.Object;
    $scope.model = model.Object;

    $scope.$watch('scopeData.filter', function (newVal, oldVal) {
        if (newVal !== oldVal) {
            var filterSearchDelay = 0;
            if ($scope.delayFilterSearch) {
                $scope.delayFilterSearch = false;
                filterSearchDelay = 1000;
            }

            clearTimeout($scope.filterSearchTimeout);
            $scope.filterSearchTimeout = setTimeout(function () {
                baseBo.httpRequest($scope.scopeData.httpRequest.method, $scope.scopeData.httpRequest.url, $scope.scopeData.filter)
                    .then(function (result) {
                        $scope.model = result.Object;
                        $scope.setGraphData();
                    });
            }, filterSearchDelay);
        }
    }, true);

    $scope.setGraphData = function () {
        $scope.performanceForOperatorNames = $scope.model.PerformanceForOperators.map(function (a) { return a.Nickname; });
        $scope.performanceForOperatorCounts = $scope.model.PerformanceForOperators.map(function (a) { return a.PerformanceCount; });

        $scope.performanceForDeliveryStaffNames = $scope.model.PerformanceForDeliveryStaff.map(function (a) { return a.Nickname; });
        $scope.performanceForDeliveryStaffCounts = $scope.model.PerformanceForDeliveryStaff.map(function (a) { return a.PerformanceCount; });
    };

    $scope.setGraphData();
}]);