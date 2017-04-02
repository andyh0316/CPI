var app = angular.module('InvoiceApp', ['AngularBaseModule', 'ui.router', 'ngAnimate']);

app.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.when("", "/List");

    $stateProvider
        .state('List', {
            url: '/List',
            templateUrl: '/Areas/Invoice/Views/Invoice/List.html',
            controller: 'ListController',
            resolve: {
                jsonResult: ['$stateParams', 'baseBo', function ($stateParams, baseBo) {
                    return baseBo.httpRequest('POST', '/Invoice/Invoice/GetList', { });
                }]
            }
        })
        .state('List.View', {
            url: '/Import/',
            templateUrl: '/Areas/Invoice/Views/Invoice/Import.html',
            controller: 'ImportController'
        })
}]);

app.controller('ListController', ['$scope', '$controller', '$state', 'baseBo', 'jsonResult', function ($scope, $controller, $state, baseBo, jsonResult) {
    angular.extend(this, $controller('ListBaseController', { $scope: $scope }));

    $scope.model = jsonResult.Object;

    // match resolve dependency params
    //$scope.sortColumn = 'LastName';
    //$scope.sortDesc = false;
    $scope.page = 1;

    $scope.import = function () {
        $state.go('List.Import');
    };


    //$scope.create = function () {
    //    $state.go('List.Staff', { 'mode': 'Create', 'id': 0 });
    //};

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

    $scope.getList = function (loadMore) {
        $scope.filter.Loads = (loadMore) ? $scope.filter.Loads + 1 : 0;

        baseBo.httpRequest('POST', '/Invoice/Invoice/GetList', $scope.filter)
            .then(function (result) {
                $scope.model.Records = (loadMore) ? $scope.model.Records.concat(result.Object.Records) : result.Object.Records;
                $scope.model.ListLoadCalculator = result.Object.ListLoadCalculator;
            });
    };

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