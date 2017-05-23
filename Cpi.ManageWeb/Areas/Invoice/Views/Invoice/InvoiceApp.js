var app = angular.module('InvoiceApp', ['AngularBaseModule', 'ui.router']);

app.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.when("", "/List");

    var listScopeData = {
        filter: { Loads: 0, SortColumn: "CreatedDate", SortDesc: true },
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

app.controller('ListController', ['$scope', '$controller', '$state', 'baseBo', 'model', 'scopeData', 'modelData', function ($scope, $controller, $state, baseBo, model, scopeData, modelData) {
    angular.extend(this, $controller('ListBaseController', { $scope: $scope }));

    $scope.scopeData = scopeData;
    $scope.modelData = modelData.Object;
    $scope.model = model.Object;

    $scope.create = function (phoneNumber) {
        var newItem = {
            isEditing: true,
            CustomerPhone: phoneNumber
        };
        $scope.model.Records.unshift(newItem);
    };

    $scope.save = function () {
        var savingRecords = $scope.model.Records.filter(function (item) { return item.isEditing === true });
        baseBo.httpRequest('POST', '/Invoice/Invoice/SaveList', savingRecords)
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

    $scope.$watch('scopeData.filter.AdvancedSearch.CreatedTodayOnly', function (newVal, oldVal) {
        if (newVal) {
            $scope.scopeData.filter.AdvancedSearch.CreatedDateFrom = null;
            $scope.scopeData.filter.AdvancedSearch.CreatedDateTo = null;
        }
    });
}]);