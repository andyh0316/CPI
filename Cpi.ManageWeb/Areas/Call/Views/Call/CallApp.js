var app = angular.module('CallApp', ['AngularBaseModule', 'ui.router']);

app.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.when("", "/List");

    var listScopeData = {
        filter: { Loads: 0, SortColumn: "CreatedDate", SortDesc: true },
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
        .state('List.View', {
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

    $scope.import = function () {
        $state.go('List.Import');
    };

    $scope.create = function () {
        var newItem = {
            isEditing: true,
            DeliveryDate: new Date(),
            AddressId: 0,
            Address: {}
        };
        $scope.model.Records.unshift(newItem);
    };

    $scope.save = function () {
        var savingRecords = $scope.model.Records.filter(function (item) { return item.isEditing === true });
        baseBo.httpRequest('POST', '/Call/Call/SaveList', savingRecords)
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

    //$scope.view = function (id) {
    //    $state.go('List.Staff', { 'mode': 'View', 'id': id });
    //};

    //$scope.edit = function (id) {
    //    $state.go('List.Staff', { 'mode': 'Edit', 'id': id });
    //};

    //$scope.selectAllStaffIds = function () {

    //    baseBo.httpRequest('POST', '/Staff/StaffInfo/SelectAllStaffIds/', { searchString: $scope.simpleSearchString, advancedSearch: $scope.advancedSearch })
    //        .then(function (result) {
    //            $scope.tryAddToSelected(result.model.StaffIds);


    //        });
    //};

    //$scope.createStaffServices = function () {
    //    $state.go('List.CreateStaffServices');
    //};


    //$scope.getListData = function () {
    //    baseBo.httpRequest('GET', '/Staff/StaffInfo/GetStaffListData')
    //           .then(function (result) { $scope.data = result.model; });
    //};

    //// the select lists are taking a while to load so we will just get them in the background
    //$scope.getListData();
}]);

app.controller('ImportController', ['$scope', '$controller', '$state', 'baseBo', function ($scope, $controller, $state, baseBo) {
    angular.extend(this, $controller('BaseController', { $scope: $scope }));


}]);