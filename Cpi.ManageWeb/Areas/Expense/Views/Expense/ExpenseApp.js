var app = angular.module('ExpenseApp', ['AngularBaseModule', 'ui.router']);

app.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.when("", "/List");

    var listScopeData = {
        filter: { Loads: 0, SortColumn: "CreatedDate", SortDesc: true },
        httpRequest: { method: 'POST', url: '/Expense/Expense/GetList' }
    };

    $stateProvider
        .state('List', {
            url: '/List',
            templateUrl: '/Areas/Expense/Views/Expense/List.html',
            controller: 'ListController',
            resolve: {
                model: ['$stateParams', 'baseBo', function ($stateParams, baseBo) {
                    return baseBo.httpRequest(listScopeData.httpRequest.method, listScopeData.httpRequest.url, listScopeData.filter);
                }],
                modelData: ['$stateParams', 'baseBo', function ($stateParams, baseBo) {
                    return baseBo.httpRequest('GET', '/Expense/Expense/GetListData');
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

    $scope.import = function () {
        $state.go('List.Import');
    };

    $scope.create = function (phoneNumber, statusId) {
        var newItem = {
            isEditing: true,
            CustomerPhone: phoneNumber,
            StatusId: statusId
        };
        $scope.model.Records.unshift(newItem);
    };

    $scope.save = function () {
        var savingRecords = $scope.model.Records.filter(function (item) { return item.isEditing === true });
        baseBo.httpRequest('POST', '/Expense/Expense/SaveList', savingRecords)
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

    $scope.$watch('scopeData.filter.AdvancedSearch.CreatedTodayOnly', function (newVal, oldVal) {
        if (newVal)
        {
            $scope.scopeData.filter.AdvancedSearch.CreatedDateFrom = null;
            $scope.scopeData.filter.AdvancedSearch.CreatedDateTo = null;
        }
    });

    $scope.getTotalExpense = function () {
        var total = 0;
        for (var i in $scope.model.Records)
        {
            var record = $scope.model.Records[i];
            if (record.Expense)
            {
                total = total + record.Expense;
            }
        }

        return total;
    };
}]);