var app = angular.module('ExpenseApp', ['AngularBaseModule', 'ui.router']);

app.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.when("", "/List");

    var listScopeData = {
        filter: { Loads: 0, SortObjects: [{ ColumnName: 'CreatedDate', IsDescending: true }]},
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

    $scope.create = function () {
        var newItem = {
            isEditing: true,
            LocationId: 1,
            Quantity: 1
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

    $scope.showDailyTotalExpense = function (index) {
        if (!$scope.isEditingAny() && $scope.scopeData.filter.SortObjects.length >= 1 && $scope.scopeData.filter.SortObjects[0].ColumnName === 'CreatedDate') {
            // determing if record at this index is the last of day
            var recordDate = $scope.model.Records[index].CreatedDate;
            recordDate = new Date(recordDate);
            recordDate = new Date(recordDate.getFullYear(), recordDate.getMonth(), recordDate.getDate());

            var nextRecordDate = null;
            if (index + 1 < $scope.model.Records.length) {
                nextRecordDate = $scope.model.Records[index + 1].CreatedDate;
                nextRecordDate = new Date(nextRecordDate);
                nextRecordDate = new Date(nextRecordDate.getFullYear(), nextRecordDate.getMonth(), nextRecordDate.getDate());
                return !(recordDate.getTime() === nextRecordDate.getTime());
            }
        }
            return false;
    };

    $scope.getDailyTotalExpense = function (index) {
        var record = $scope.model.Records[index];
        var recordDate = record.CreatedDate;
        console.log(index);
        recordDate = new Date(recordDate);
        recordDate = new Date(recordDate.getFullYear(), recordDate.getMonth(), recordDate.getDate());

        var totalExpense = record.Expense * record.Quantity;

        while (true) {
            index--;

            if (index < 0)
            {
                break;
            }

            var currentRecord = $scope.model.Records[index];
            var currentRecordDate = currentRecord.CreatedDate;
            currentRecordDate = new Date(currentRecordDate);
            currentRecordDate = new Date(currentRecordDate.getFullYear(), currentRecordDate.getMonth(), currentRecordDate.getDate());

            if (currentRecordDate.getTime() !== recordDate.getTime())
            {
                break;
            }

            totalExpense = totalExpense + currentRecord.Expense * currentRecord.Quantity;
        }

        return totalExpense;
    };
}]);