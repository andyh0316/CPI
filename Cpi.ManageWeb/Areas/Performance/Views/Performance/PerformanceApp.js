var app = angular.module('PerformanceApp', ['AngularBaseModule', 'chart.js', 'ui.router']);

app.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.when("", "/Performance");

    var listScopeData = {
        filter: { Loads: 0, SortColumn: "CreatedDate", SortDesc: true },
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

    $scope.PerformanceForOperatorNames = $scope.model.PerformanceForOperators.map(function (a) { return a.Nickname; });
    $scope.PerformanceForOperatorCount = $scope.model.PerformanceForOperators.map(function (a) { return a.PerformanceCount; });

    $scope.PerformanceForDeliveryStaffNames = $scope.model.PerformanceForDeliveryStaff.map(function (a) { return a.Nickname; });
    $scope.PerformanceForDeliveryStaffCount = $scope.model.PerformanceForDeliveryStaff.map(function (a) { return a.PerformanceCount; });

    $scope.create = function (phoneNumber) {
        var newItem = {
            isEditing: true,
            CustomerPhone: phoneNumber
        };
        $scope.model.Records.unshift(newItem);
    };

    $scope.save = function () {
        var savingRecords = $scope.model.Records.filter(function (item) { return item.isEditing === true });
        baseBo.httpRequest('POST', '/Performance/Performance/SaveList', savingRecords)
            .then(function (result) {
                if (result.ModelState)
                {
                    $scope.modelState = result.ModelState;
                }
                else
                {
                    $scope.cancelAll();
                    $scope.getList();
                }
            });
    };
}]);