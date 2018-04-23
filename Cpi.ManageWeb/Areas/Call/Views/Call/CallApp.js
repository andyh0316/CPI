var app = angular.module('CallApp', ['AngularBaseModule', 'ui.router']);

app.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.when("", "/List");

    var listScopeData = {
        filter: {
            Loads: 0,
            SortObjects: [{ ColumnName: 'Date', IsDescending: true }],
            AdvancedSearch: {
                ReportDateFilter: {
                    ReportDateId: gReportDateId
                }
            }
        },
        httpRequest: { method: 'POST', url: '/Call/Call/GetList' }
    };

    $stateProvider
        .state('List', {
            url: '/List',
            templateUrl: '/Areas/Call/Views/Call/List.html',
            controller: 'ListController',
            resolve: {
                model: ['$stateParams', 'baseBo', function ($stateParams, baseBo) {
                    return baseBo.httpRequest(listScopeData.httpRequest.method, listScopeData.httpRequest.url, listScopeData.filter);
                }],
                modelData: ['$stateParams', 'baseBo', function ($stateParams, baseBo) {
                    return baseBo.httpRequest('GET', '/Call/Call/GetListData');
                }],
                scopeData: function () {
                    return listScopeData;
                }
            }
        })
        .state('List.Import', {
            url: '/Import/',
            templateUrl: '/Areas/Call/Views/Call/Import.html',
            controller: 'ImportController'
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
        baseBo.httpRequest('POST', '/Call/Call/SaveList', savingListItems)
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
}]);

app.controller('ImportController', ['$scope', '$controller', '$state', 'baseBo', function ($scope, $controller, $state, baseBo) {
    angular.extend(this, $controller('BaseController', { $scope: $scope }));

    $scope.organizePhoneNumbers = function () {
        baseBo.httpRequest('POST', '/Call/Call/OrganizePhoneNumbers', { phoneNumbers: $scope.phoneNumbers })
            .then(function (result) {
                $scope.parsedPhoneNumbersModel = result.Object;
            });
    };

    $scope.createPhoneNumbers = function () {
        baseBo.httpRequest('POST', '/Call/Call/ParsePhoneNumbers', { phoneNumbers: $scope.phoneNumbers })
            .then(function (result) {
                for (var i in result.Object.PhoneNumbers)
                {
                    var newItem = {
                        touched: true,
                        CustomerPhone: result.Object.PhoneNumbers[i].Item1,
                        Date: (result.Object.PhoneNumbers[i].Item2) ? result.Object.PhoneNumbers[i].Item2 : $scope.modelData.TodayDate,
                        StatusId: $scope.modelData.CallStatusIdEnums.SentToCallCenter,
                    };

                    $scope.$parent.createListItem(newItem);
                }

                $scope.back();
            });
    };
}]);