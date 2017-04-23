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
                scopeData: function () {
                    return financeScopeData;
                }
            }
        })
}]);

app.controller('FinanceController', ['$scope', '$controller', '$state', 'baseBo', 'model', 'scopeData', function ($scope, $controller, $state, baseBo, model, scopeData) {
    angular.extend(this, $controller('BaseController', { $scope: $scope }));
    
    $scope.scopeData = scopeData;
    //$scope.modelData = modelData.Object;
    $scope.model = model.Object;
}]);