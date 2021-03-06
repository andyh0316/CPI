﻿var app = angular.module('CommodityApp', ['AngularBaseModule', 'ui.router']);

app.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.when("", "/List");

    var listScopeData = {
        filter: { Loads: 0, SortObjects: [{ ColumnName: 'CreatedDate', IsDescending: true }]},
        httpRequest: { method: 'POST', url: '/Manage/Commodity/GetList' }
    };

    $stateProvider
        .state('List', {
            url: '/List',
            templateUrl: '/Areas/Manage/Views/Commodity/List.html',
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

app.controller('ListController', ['$scope', '$controller', '$state', 'baseBo', 'model', 'scopeData', function ($scope, $controller, $state, baseBo, model, scopeData) {
    angular.extend(this, $controller('ListBaseController', { $scope: $scope }));

    $scope.scopeData = scopeData;
    $scope.model = model.Object;

    $scope.listItems = $scope.model.ListItems;
    $scope.listLoadCalculator = $scope.model.ListLoadCalculator;

    //$scope.create = function () {
    //    var newItem = {
    //        touched: true,
    //        LocationId: 1,
    //        Quantity: 1
    //    };
    //    $scope.listItems.unshift(newItem);
    //};

    $scope.save = function () {
        var savingListItems = $scope.listItems.filter(function (item) { return item.touched === true });
        baseBo.httpRequest('POST', '/Manage/Commodity/SaveList', savingListItems)
            .then(function (result) {
                if (result.ModelState)
                {
                    $scope.modelState = result.ModelState;
                }
                else
                {
                    $scope.getList('savedList');
                }
            });
    };
}]);