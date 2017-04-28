var app = angular.module('FinanceApp', ['AngularBaseModule', 'ui.router']);

app.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.when("", "/Finance");

    var financeScopeData = {
        //filter: { Loads: 0, SortColumn: "CreatedDate", SortDesc: true },
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

    $scope.filters = {};

    $scope.$watch('filters', function () {
        baseBo.httpRequest('POST', '/Call/Call/SaveList', savingRecords)
            .then(function (result) {
                if (result.ModelState) {
                    $scope.modelState = result.ModelState;
                }
                else {
                    $scope.cancelAll();
                    $scope.getList();
                }
            });
    });
}]);