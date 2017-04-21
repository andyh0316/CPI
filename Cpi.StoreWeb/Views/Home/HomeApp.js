var app = angular.module('HomeApp', ['AngularBaseModule', 'ui.router']);

app.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.when("", "/Home");

    $stateProvider
        .state('Home', {
            url: '/Home',
            templateUrl: '/Views/Home/Home.html',
            controller: 'HomeController',
            //resolve: {
            //    model: ['$stateParams', 'baseBo', function ($stateParams, baseBo) {
            //        return baseBo.httpRequest(listScopeData.httpRequest.method, listScopeData.httpRequest.url, listScopeData.filter);
            //    }],
            //    modelData: ['$stateParams', 'baseBo', function ($stateParams, baseBo) {
            //        return baseBo.httpRequest('GET', '/Call/Call/GetListData');
            //    }],
            //    scopeData: function () {
            //        return listScopeData;
            //    }
            //}
        })
        //.state('List.Import', {
        //    url: '/Import/',
        //    templateUrl: '/Areas/Call/Views/Call/Import.html',
        //    controller: 'ImportController'
        //})
}]);

app.controller('HomeController', ['$scope', '$controller', '$state', 'baseBo', function ($scope, $controller, $state, baseBo) {

}]);