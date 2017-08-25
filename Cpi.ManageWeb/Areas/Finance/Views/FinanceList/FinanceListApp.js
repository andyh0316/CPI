var app = angular.module('FinanceListApp', ['AngularBaseModule', 'ui.router']);

app.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.when("", "/FinanceList");

    var listScopeData = {
        filter: {
            Loads: 0, SortObjects: [{ ColumnName: 'CreatedDate', IsDescending: true }]
        },
        httpRequest: { method: 'POST', url: '/Finance/FinanceList/GetFinanceList' }
    };

    $stateProvider
        .state('FinanceList', {
            url: '/FinanceList',
            templateUrl: '/Areas/Finance/Views/FinanceList/List.html',
            controller: 'FinanceListController',
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

app.controller('FinanceListController', ['$scope', '$controller', '$state', 'baseBo', 'model', 'scopeData', function ($scope, $controller, $state, baseBo, model, scopeData) {
    angular.extend(this, $controller('ListBaseController', { $scope: $scope }));

    $scope.scopeData = scopeData;
    $scope.model = model.Object;
}]);