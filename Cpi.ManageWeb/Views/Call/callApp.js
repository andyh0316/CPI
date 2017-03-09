var app = angular.module('CallApp', ['AngularBaseModule', 'ui.router']);

app.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.when("", "/List");

    $stateProvider
        .state('List', {
            url: '/List',
            templateUrl: '/Views/Call/List.cshtml',
            controller: 'ListController',
            resolve: {
                jsonResult: ['$stateParams', 'baseBo', function ($stateParams, baseBo) {
                    return baseBo.httpRequest('POST', '/Call/GetList', { page: 1 });
                }]
            }
        })
}]);

app.controller('ListController', ['$scope', '$controller', '$state', 'baseBo', 'jsonResult', function ($scope, $controller, $state, baseBo, jsonResult) {
    angular.extend(this, $controller('ListBaseController', { $scope: $scope }));

    $scope.model = jsonResult.model;

    // match resolve dependency params
    //$scope.sortColumn = 'LastName';
    //$scope.sortDesc = false;
    $scope.page = 1;

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

    $scope.getList = function () {
        baseBo.httpRequest('POST', '/Staff/StaffInfo/GetStaffList', { page: $scope.page })
              .then(function (result) { $scope.model = result.model; });
    };

    $scope.getListData = function () {
        baseBo.httpRequest('GET', '/Staff/StaffInfo/GetStaffListData')
               .then(function (result) { $scope.data = result.model; });
    };

    // the select lists are taking a while to load so we will just get them in the background
    $scope.getListData();
}]);