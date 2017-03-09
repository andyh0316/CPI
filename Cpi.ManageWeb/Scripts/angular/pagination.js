angular.module('PaginationModule', [])
.directive('pagination', function () {
    return {
        restrict: 'E',
        templateUrl: '/Views/Shared/_Pagination.html',
        scope: {
            pagination: '=',
            hideResultCount: '@',
        },
        controller: ['$scope', function ($scope) {
            $scope.hideResultCount = ($scope.hideResultCount === 'true') ? true : false;

            $scope.$watch('pagination.Page', function (newVal, oldVal) {
                if (newVal && oldVal && newVal != oldVal) {
                    $scope.$emit('paginationPageChangeEvent', { page: newVal });
                }
            });

            $scope.getPageButtons = function (pages, page) {
                var maxPages = 12; // must be even number
                //if (maxPages % 2 == 1) { return null };
                //maxPages = maxPages - 1;

                var startPage = page - Math.floor(maxPages / 2);
                var endPage = page + Math.ceil(maxPages / 2);
                if (startPage < 1) {
                    endPage = endPage + (1 - startPage);
                    startPage = 1;
                }

                if (endPage > pages) {
                    startPage = startPage - (endPage - pages);
                    if (startPage < 1) {
                        startPage = 1;
                    }
                    endPage = pages;
                }
                
                var pageButtons = [];
                if (startPage >= 2) {
                    pageButtons.push(1);
                    pageButtons.push('...');
                    startPage = startPage + 2;
                }

                if (endPage <= pages - 1) {
                    var flag1 = true;
                    endPage = endPage - 2;
                }

                
                for (var i = startPage; i <= endPage; i++) {
                    pageButtons.push(i);
                }

                if (flag1) {
                    pageButtons.push('...');
                    pageButtons.push(pages);
                }

                return pageButtons;
            }
        }]
    }
});