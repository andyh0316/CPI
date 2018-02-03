var app = angular.module('InvoiceApp', ['AngularBaseModule', 'ui.router']);

app.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.when("", "/List");

    var listScopeData = {
        filter: { Loads: 0, SortObjects: [{ ColumnName: 'Date', IsDescending: true }]},
        httpRequest: { method: 'POST', url: '/Invoice/Invoice/GetList' }
    };

    $stateProvider
        .state('List', {
            url: '/List',
            templateUrl: '/Areas/Invoice/Views/Invoice/List.html',
            controller: 'ListController',
            resolve: {
                model: ['$stateParams', 'baseBo', function ($stateParams, baseBo) {
                    return baseBo.httpRequest(listScopeData.httpRequest.method, listScopeData.httpRequest.url, listScopeData.filter);
                }],
                modelData: ['$stateParams', 'baseBo', function ($stateParams, baseBo) {
                    return baseBo.httpRequest('GET', '/Invoice/Invoice/GetListData');
                }],
                scopeData: function () {
                    return listScopeData;
                }
            }
        })
}]);

app.controller('ListController', ['$scope', '$controller', '$state', '$timeout', 'baseBo', 'model', 'scopeData', 'modelData', function ($scope, $controller, $state, $timeout, baseBo, model, scopeData, modelData) {
    angular.extend(this, $controller('ListBaseController', { $scope: $scope }));

    $scope.scopeData = scopeData;
    $scope.modelData = modelData.Object;
    $scope.model = model.Object;

    $scope.listItems = $scope.model.ListItems;
    $scope.listLoadCalculator = $scope.model.ListLoadCalculator;

    $scope.save = function () {
        $scope.newListItemCount = 0;

        var savingListItems = $scope.listItems.filter(function (item) { return item.touched === true });
        baseBo.httpRequest('POST', '/Invoice/Invoice/SaveList', savingListItems)
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

    $scope.newListItemCount = 0;
    $scope.checkNewListItems = function () {
        if (gCurrentRequests === 0 && !$scope.isAnyListItemTouched())
        {
            baseBo.httpRequest(scopeData.httpRequest.method, scopeData.httpRequest.url, scopeData.filter, {noLoadIcon: true})
                .then(function (result) {
                    if ($scope.listLoadCalculator.Total < result.Object.ListLoadCalculator.Total)
                    {
                        $scope.newListItemCount = result.Object.ListLoadCalculator.Total - $scope.listLoadCalculator.Total;
                    }
                });
        }

        $timeout($scope.checkNewListItems, 30000);
    }

    $timeout($scope.checkNewListItems, 3000);

    $scope.invoiceCommodityChange = function (record) {
        var total = 0;
        for (var i in record.InvoiceCommodities)
        {
            var invoiceCommodity = record.InvoiceCommodities[i];
            var commodityPrice = $scope.modelData.Commodities.filter(function (item) { return item.Id === invoiceCommodity.CommodityId })[0].Price;
            total = total + invoiceCommodity.Quantity * commodityPrice;
        }
        record.TotalPrice = total;
    };

    $scope.$watch('scopeData.filter.AdvancedSearch.TodayOnly', function (newVal, oldVal) {
        if (newVal) {
            $scope.scopeData.filter.AdvancedSearch.DateFrom = null;
            $scope.scopeData.filter.AdvancedSearch.DateTo = null;
        }
    });
}]);