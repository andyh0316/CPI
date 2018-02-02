var app = angular.module('ExpenseApp', ['AngularBaseModule', 'ui.router']);

app.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.when("", "/List");

    var listScopeData = {
        filter: { Loads: 0, SortObjects: [{ ColumnName: 'Date', IsDescending: true }]},
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

    $scope.listItems = $scope.model.ListItems;
    $scope.listLoadCalculator = $scope.model.ListLoadCalculator;

    $scope.import = function () {
        $state.go('List.Import');
    };

    $scope.save = function () {
        var savingListItems = $scope.listItems.filter(function (item) { return item.touched === true });
        baseBo.httpRequest('POST', '/Expense/Expense/SaveList', savingListItems)
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

    $scope.$watch('scopeData.filter.AdvancedSearch.TodayOnly', function (newVal, oldVal) {
        if (newVal)
        {
            $scope.scopeData.filter.AdvancedSearch.DateFrom = null;
            $scope.scopeData.filter.AdvancedSearch.DateTo = null;
        }
    });

    $scope.getTotalExpense = function () {
        var total = 0;
        for (var i in $scope.listItems)
        {
            var record = $scope.listItems[i];
            if (record.Expense)
            {
                total = total + record.Expense;
            }
        }

        return total;
    };

    $scope.showDailyTotalExpense = function (index) {
        if (!$scope.isAnyListItemTouched() && $scope.scopeData.filter.SortObjects.length >= 1 && $scope.scopeData.filter.SortObjects[0].ColumnName === 'Date') {
            // determing if record at this index is the last of day
            var recordDate = $scope.listItems[index].Date;
            recordDate = new Date(recordDate);
            recordDate = new Date(recordDate.getFullYear(), recordDate.getMonth(), recordDate.getDate());

            var nextRecordDate = null;
            if (index + 1 < $scope.listItems.length) {
                nextRecordDate = $scope.listItems[index + 1].Date;
                nextRecordDate = new Date(nextRecordDate);
                nextRecordDate = new Date(nextRecordDate.getFullYear(), nextRecordDate.getMonth(), nextRecordDate.getDate());
                return !(recordDate.getTime() === nextRecordDate.getTime());
            }

            if (index === $scope.listItems.length - 1)
            {
                return true;
            }
        }

        return false;
    };

    $scope.getDailyTotalExpense = function (index) {
        var record = $scope.listItems[index];
        var recordDate = record.Date;
        recordDate = new Date(recordDate);
        recordDate = new Date(recordDate.getFullYear(), recordDate.getMonth(), recordDate.getDate());

        var totalExpense = record.Expense;

        while (true) {
            index--;

            if (index < 0)
            {
                break;
            }

            var currentRecord = $scope.listItems[index];
            var currentRecordDate = currentRecord.Date;
            currentRecordDate = new Date(currentRecordDate);
            currentRecordDate = new Date(currentRecordDate.getFullYear(), currentRecordDate.getMonth(), currentRecordDate.getDate());

            if (currentRecordDate.getTime() !== recordDate.getTime())
            {
                break;
            }

            totalExpense = totalExpense + currentRecord.Expense;
        }

        return totalExpense;
    };
}]);