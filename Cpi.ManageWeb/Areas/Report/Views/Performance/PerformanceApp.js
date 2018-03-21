var app = angular.module('PerformanceApp', ['AngularBaseModule', 'chart.js', 'ui.router']);

app.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.when("", "/Performance");

    var listScopeData = {
        filter: {
            ReportDateFilter: { ReportDateId: gReportDateId }
        },
        httpRequest: { method: 'POST', url: '/Report/Performance/GetPerformance' }
    };

    $stateProvider
        .state('Performance', {
            url: '/Performance',
            templateUrl: '/Areas/Report/Views/Performance/Performance.html',
            controller: 'PerformanceController',
            resolve: {
                model: ['$stateParams', 'baseBo', function ($stateParams, baseBo) {
                    return baseBo.httpRequest(listScopeData.httpRequest.method, listScopeData.httpRequest.url, listScopeData.filter);
                }],
                modelData: ['$stateParams', 'baseBo', function ($stateParams, baseBo) {
                    return baseBo.httpRequest('GET', '/Report/Performance/GetPerformanceData');
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
        $scope.performanceForOperatorNames = $scope.model.PerformanceForOperators.map(function (a) { return a.Item1; });
        $scope.performanceForOperatorCounts = $scope.model.PerformanceForOperators.map(function (a) { return a.Item2; });

        $scope.performanceForDeliveryStaffNames = $scope.model.PerformanceForDeliveryStaff.map(function (a) { return a.Item1; });
        $scope.performanceForDeliveryStaffCounts =
            [
                $scope.model.PerformanceForDeliveryStaff.map(function (a) { return a.Item2; }),
                $scope.model.PerformanceForDeliveryStaff.map(function (a) { return a.Item3; })
            ];

        $scope.callForWeekDaysNames = $scope.model.CallForWeekDays.map(function (a) { return a.Item1; });
        $scope.callForWeekDaysCounts = $scope.model.CallForWeekDays.map(function (a) { return a.Item2; });

        //$scope.callCounts =
        //    [
        //        $scope.model.Calls.map(function (a) { return a.Item2; }),
        //        $scope.model.Calls.map(function (a) { return a.Item3; }),
        //    ];
    };

    $scope.setGraphData();
}]);