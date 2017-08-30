﻿var app = angular.module('FinanceListApp', ['AngularBaseModule', 'ui.router']);

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
        .state('FinanceList.Finance', {
            url: '/Finance/:date/',
            templateUrl: '/Areas/Finance/Views/FinanceList/Finance.html',
            controller: 'FinanceController',
            resolve: {
                model: ['$stateParams', 'baseBo', function ($stateParams, baseBo) {
                    return baseBo.httpRequest('GET', '/Finance/FinanceList/GetFinance', { date: $stateParams.date });
                }],
            }
        })
}]);

app.controller('FinanceListController', ['$scope', '$controller', '$state', 'baseBo', 'model', 'scopeData', function ($scope, $controller, $state, baseBo, model, scopeData) {
    angular.extend(this, $controller('ListBaseController', { $scope: $scope }));

    $scope.scopeData = scopeData;
    $scope.model = model.Object;

    $scope.viewFinance = function (date) {
        $state.go('FinanceList.Finance', { 'date': date });
    };
}]);

app.controller('FinanceController', ['$scope', '$controller', '$state', 'baseBo', 'model', function ($scope, $controller, $state, baseBo, model) {
    angular.extend(this, $controller('BaseController', { $scope: $scope }));

    $scope.model = model.Object;

    $scope.getTotalExpense = function () {
        var sum = 0;
        for (var i in $scope.model.Expenses) {
            var expense = $scope.model.Expenses[i];
            sum = sum + expense.Expense * expense.Quantity;
        }

        return sum;
    };

    $scope.getTotalRevenue = function () {
        var sum = 0;
        for (var i in $scope.model.InvoiceSummaries) {
            var invoiceCommodity = $scope.model.InvoiceSummaries[i];
            sum = sum + invoiceCommodity.TotalPrice;
        }

        return sum;
    };
}]);

