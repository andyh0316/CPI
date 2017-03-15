var baseModule = angular.module('AngularBaseModule', ['PaginationModule']);

baseModule.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.defaults.headers.common['Cache-Control'] = 'no-cache, no-store, must-revalidate';
    $httpProvider.defaults.headers.common['Pragma'] = 'no-cache';
    $httpProvider.defaults.headers.common['Expires'] = '0';
    $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
}]);


baseModule.controller('BaseController', ['$scope', '$state', function ($scope, $state) {
    // show only the most nested buttons
    //var levels = 0;
    //var deepest;

    //$('.secondary-menu-buttons').hide();
    //$('body').find('.secondary-menu-buttons').each(function () {
    //    if (!this.firstChild || this.firstChild.nodeType !== 1) {
    //        var levelsFromThis = $(this).parentsUntil('body').length;
    //        if (levelsFromThis > levels) {
    //            levels = levelsFromThis;
    //            deepest = this;
    //        }
    //    }
    //});

    //$(deepest).show();
}]);

/* provides server/backend sorting, paging and searching. For client side sorting and paging */
baseModule.controller('ListBaseController', ['$scope', '$controller', function ($scope, $controller) {
    angular.extend(this, $controller('BaseController', { $scope: $scope }));

    $scope.getList = null; // this is expected to be defined in the child list controllers IF they dont set return funtions for sort and search and etc

    // this is called when the child wants to reload the parent list.. for example, when done saving a student, the list will get the updated
    // entries
    $scope.$on('reloadListEvent', function () {
        $scope.getList();
    });

    /**** PAGE ****/
    $scope.$on('paginationPageChangeEvent', function (event, args) {
        $scope.page = args.page;
        $scope.getList();
        event.stopPropagation(); // prevent the $emit from propagating: only let the pagination send to its immediate parent
    });

    /**** SORT ****/
    $scope.sort = function (sortColumn) {
        if ($scope.sortColumn == sortColumn) {
            $scope.sortDesc = !$scope.sortDesc;
        } else {
            $scope.sortDesc = false;
        }

        $scope.sortColumn = sortColumn;
        $scope.page = 1;

        if ($scope.sortReturn) {
            $scope.sortReturn();
        } else {
            $scope.getList();
        }
    };

    $scope.setSortReturn = function (returnFunction) {
        $scope.sortReturn = returnFunction;
    };

    $scope.getSortOrder = function (sortColumn) {
        if ($scope.sortColumn == sortColumn) {
            return ($scope.sortDesc) ? 'sorted-desc' : 'sorted-asc';
        }

        return null;
    };

    /**** SELECTION ****/
    $scope.selectedIds = [];
    $scope.maxSelectedIds = 500;
    $scope.updateSelectedId = function (id) {
        var index = $scope.getIndexOfSelectedId(id);
        if (index == -1) {
            if ($scope.isMaxSelectedIds([id])) {
                return;
            };

            $scope.selectedIds.push(id);
        } else {
            $scope.selectedIds.splice(index, 1);
        }
    };

    $scope.clearSelectedIds = function () {
        $scope.selectedIds = [];
    };

    $scope.isIdSelected = function (id) {
        if ($scope.getIndexOfSelectedId(id) == -1) {
            return false;
        } else {
            return true;
        }
    };

    $scope.updateSelectAllOnPage = function (items) {
        if ($scope.areSelectedAllOnPage(items)) {
            items.forEach(function (item) {
                var index = $scope.getIndexOfSelectedId(item.Id);
                if (index > -1) {
                    $scope.selectedIds.splice(index, 1);
                }
            });
        } else {
            if ($scope.isMaxSelectedIds(items.map(function (item) { item.Id }))) {
                return;
            }

            items.forEach(function (item) {
                if (!$scope.isIdSelected(item.Id)) {
                    $scope.selectedIds.push(item.Id);
                }
            });
        }
    };

    $scope.areSelectedAllOnPage = function (items) {
        if (!items || items.length == 0) {
            return false;
        };

        var flag = true;
        for (var i = 0; i < items.length; i++) {
            if (!$scope.isIdSelected(items[i].Id)) {
                flag = false;
                break;
            }
        }

        return flag;
    };

    $scope.getIndexOfSelectedId = function (id) {
        var index = -1;
        for (var i = 0; i < $scope.selectedIds.length; i++) {
            if (id == $scope.selectedIds[i]) {
                index = i;
                return index;
            }
        }

        return index;
    };

    $scope.tryAddToSelected = function (ids) {
        if ($scope.isMaxSelectedIds(ids)) {
            return;
        }

        for (var i in ids) {
            if (!$scope.isIdSelected(ids[i])) {
                $scope.selectedIds.push(ids[i]);
            }
        }
    };

    // first check if adding the ids will exceed maximum, and prevent it
    $scope.isMaxSelectedIds = function (ids) {
        var finalSelectedIdsLength = $scope.selectedIds.length;
        for (var i in ids) {
            if (!$scope.isIdSelected(ids[i])) {
                finalSelectedIdsLength++;
            }
        }

        if (finalSelectedIdsLength > $scope.maxSelectedIds) {
            $scope.setNotification('warning', 'You cannot select more than ' + $scope.maxSelectedIds + ' results.');
            return true;
        }

        return false;
    };

    /**** SIMPLE SEARCH ****/
    $scope.simpleSearchTimeout = null;
    $scope.simpleSearchString = null;
    $scope.$watch('simpleSearchString', function (newVal, oldVal) {
        if (!newVal && !oldVal) {
            return;
        }

        //$scope.advancedSearch = null;
        //$scope.showAdvancedSearch = false;

        clearTimeout($scope.simpleSearchTimeout);
        $scope.simpleSearchTimeout = setTimeout(function () {
            if ($scope.doNotSimpleSearch) {
                $scope.doNotSimpleSearch = false;
                return;
            }

            $scope.page = 1;
            $scope.getList();
        }, 500); // wait for user to finish typing input: how many ms to wait
    });

    $scope.simpleSearchGo = function () {
        $scope.doNotSimpleSearch = true;
        $scope.getList();
    };

    /**** ADVANCED SEARCH ****/
    if (typeof gShowAdvancedSearch !== 'undefined') {
        $scope.showAdvancedSearch = gShowAdvancedSearch;
    }

    $scope.advancedSearch = null;
    $scope.doNotAdvancedSearch = false;
    $scope.advancedSearchTimeout = null;
    $scope.$watch('advancedSearch', function (newVal, oldVal) {
        if ($scope.doNotAdvancedSearch) {
            $scope.doNotAdvancedSearch = false;
            return;
        }

        if (newVal == oldVal)
            return;

        var advancedSearchDelay = 0;
        if ($scope.delayAdvancedSearch) {
            $scope.delayAdvancedSearch = false;
            advancedSearchDelay = 700;
        }

        $scope.page = 1;

        clearTimeout($scope.advancedSearchTimeout);
        $scope.advancedSearchTimeout = setTimeout(function () {
            $scope.getList();
        }, advancedSearchDelay);
    }, true);

}]);

baseModule.run(['$rootScope', '$state', function ($rootScope, $state) {
    $rootScope.state = $state;

    $rootScope.back = function () {
        $state.go('^');
    };

    $rootScope.checkPermission = function (permission, action) {
        for (var i in $rootScope.permissions) {
            var permissionDto = $rootScope.permissions[i];
            if (permissionDto.Name === permission) {
                if (action == "View") {
                    if (permissionDto.View) {
                        return true;
                    }
                }
                else if (action == "Create") {
                    if (permissionDto.Create) {
                        return true;
                    }
                }
                else if (action == "Edit") {
                    if (permissionDto.Edit) {
                        return true;
                    }
                }
                else if (action == "Delete") {
                    if (permissionDto.Delete) {
                        return true;
                    }
                }

                return false;
            }
        }

        return false;
    };

    $rootScope.setNotification = function (type, msg) {
        var notificationSelector = $('#notification-message');
        var notificationMsgSelector = $('#notification-message .content');
        if (type == 'warning') {
            notificationSelector.attr("class", "warning");
            notificationMsgSelector.html(msg);
        }
        else if (type == 'error') {
            notificationSelector.attr("class", "error");
            if (msg) {
                notificationMsgSelector.html(msg);
            } else {
                notificationMsgSelector.html('An Error Has Occurred');
            }
        }
        else {
            notificationSelector.attr("class", "");
            if (msg) {
                notificationMsgSelector.html(msg);
            } else {
                notificationMsgSelector.html('Action Successful');
            }

            notificationSelector.stop().hide().fadeIn(75).delay(2000).fadeOut(500);
            return;
        }

        notificationSelector.stop().hide().fadeIn(75);
    }

    $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {

    });

    $rootScope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {

    });

    $rootScope.$on('$viewContentLoaded', function (event, view) {
        // show only the most nested buttons
        //debugger;
        //var levels = 0;
        //var deepest;

        //$('.secondary-menu-buttons').hide();
        //$('body').find('.secondary-menu-buttons').each(function () {
        //    if (!this.firstChild || this.firstChild.nodeType !== 1) {
        //        var levelsFromThis = $(this).parentsUntil('body').length;
        //        if (levelsFromThis > levels) {
        //            levels = levelsFromThis;
        //            deepest = this;
        //        }
        //    }
        //});

        //$(deepest).show();
    });
}]);

baseModule.factory('baseBo', ['$http', '$window', '$q', function ($http, $window, $q) {
    var instance = {};

    var currentRequests = 0;

    // Caution: all posts that attempt to save should preferrably pass in a whole model object instead of objects of different models
    // (ex. httpPost(url, $scope.model) instead of httpPost(url, { model: $scope.model })
    // this is because so that ModelState will not prefix keys with parameter name so our angular apps
    // can properly bind to fields to display validation errors generated by MVC
    instance.httpRequest = function (httpMethod, url, params, opts) {
        if (opts && opts.noLoadIcon === true) {
        }
        else {
            $('#wait-mask').show();
            currentRequests++;
        }

        if (httpMethod == 'GET') {
            var httpVar = { method: 'GET', url: url, params: params };
        } else if (httpMethod == 'POST') {
            var httpVar = { method: 'POST', url: url, data: params };
        } else if (httpMethod == 'UPLOAD') {
            var httpVar = {
                method: 'POST', url: url, data: params,
                withCredentials: true, headers: { 'Content-Type': undefined }, transformRequest: angular.identity
            };
        }

        return instance.httpBase(httpVar);
    };

    instance.httpBase = function (httpVar) {
        var deferred = $q.defer();

        $http(httpVar)
            .success(function (result) {

                if (result.IsSessionExpired) {
                    location.href = '/Public/Login/';
                    return;
                };

                // tell javascript how much time is remaining for session timeout so we can warn the user
                // to extend his session
                //setSessionWarning(result.SessionTimeLeft);

                // need to check for if its session timeout
                //successFunc(result);
                deferred.resolve(result);

                currentRequests--;
                if (currentRequests <= 0) {
                    currentRequests = 0;
                    $('#wait-mask').hide();
                }
            })
            .error(function (data, status, headers, config) {
                currentRequests--;
                if (currentRequests <= 0) {
                    currentRequests = 0;
                    $('#wait-mask').hide();
                }

                alert("Oops! The application has encountered an unexpected error. Please sign out and sign in again. If this the problem persists, please report to us.");
            });

        return deferred.promise;
    };

    return instance;
}]);

baseModule.filter('shortDate', ['$filter', function ($filter) {
    var angularDateFilter = $filter('date');
    return function (date) {

        return angularDateFilter(date, 'MM/dd/yyyy');
    }
}]);

baseModule.filter('shortDateWithTime', ['$filter', function ($filter) {
    var angularDateFilter = $filter('date');
    return function (date) {

        return angularDateFilter(date, 'MM/dd/yyyy hh:mm a');
    }
}]);

baseModule.filter('bool', function () {
    return function (value) {

        if (value == true) {
            return "Yes";
        } else if (value == false) {
            return "No";
        }

        return null;
    }
});

baseModule.filter('range', function () {
    return function (input, start, end) {
        for (var i = start; i <= end; i++) {
            input.push(i);
        }

        return input;
    };
});

baseModule.directive('selectableText', ['$window', '$timeout', function ($window, $timeout) {
    var i = 0;
    return {
        restrict: 'A',
        priority: 1,
        compile: function (tElem, tAttrs) {
            var fn = '$$clickOnNoSelect' + i++,
                _ngClick = tAttrs.ngClick;

            tAttrs.ngClick = fn + '($event)';

            return function (scope) {
                var lastAnchorOffset, lastFocusOffset, timer;

                scope[fn] = function (event) {
                    var selection = $window.getSelection(),
                        anchorOffset = selection.anchorOffset,
                        focusOffset = selection.focusOffset;

                    if (focusOffset - anchorOffset !== 0) {
                        if (!(lastAnchorOffset === anchorOffset && lastFocusOffset === focusOffset)) {
                            lastAnchorOffset = anchorOffset;
                            lastFocusOffset = focusOffset;
                            if (timer) {
                                $timeout.cancel(timer);
                                timer = null;
                            }
                            return;
                        }
                    }
                    lastAnchorOffset = null;
                    lastFocusOffset = null;
                    scope.$eval(_ngClick, { $event: event });

                    // delay invoking click so as to watch for user double-clicking 
                    // to select words
                    //timer = $timeout(function() {
                    //    scope.$eval(_ngClick, {$event: event});  
                    //    timer = null;
                    //}, 250);
                };
            };
        }
    };
}]);

baseModule.directive('dateInput', ['$filter', function ($filter) {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ngModelController) {

            var dateFormat = 'MM/dd/yyyy';

            ngModelController.$formatters.push(function (modelValue) {
                return $filter('date')(modelValue, dateFormat);

                //var currentDate = new Date(modelValue + "+00:00");
                //currentDate = new Date((currentDate.getTime() + (currentDate.getTimezoneOffset() * 60 * 1000)));

                //if (isNaN(currentDate) === false) {
                //    return $filter('date')(currentDate, dateFormat);
                //}
                //return undefined;
            });

            element.datepicker({
                dateFormat: 'mm/dd/yy',
                yearRange: '1950:2020',
                changeMonth: true,
                changeYear: true,
                onSelect: function (dateText) {
                    scope.$apply(function (scope) {
                        // call $parsers pipeline then update $modelValue
                        ngModelController.$setViewValue(dateText);
                        // update the local view
                        ngModelController.$render();
                    });
                    element.blur();
                }
            });

            //Check the attributes for a 'yearRange'                
            if (attrs && attrs.yearrange) {
                element.datepicker("option", "yearRange", attrs.yearrange);
            }
        }
    };
}]);

baseModule.directive('tbody', function () {
    return {
        restrict: 'E',
        scope: true, // pass in the entire scope
        link: function (scope, element, attrs) {
            scope.$watch(function () { // watch for any scope changes
                if (element.find('tr').length == 0) {
                    element.addClass('empty');
                } else {
                    element.removeClass('empty');
                }
            });
        }
    }
});

baseModule.directive('listPanelsContainer', function () {
    return {
        restrict: 'AEC',
        scope: true, // pass in the entire scope
        link: function (scope, element, attrs) {
            scope.$watch(function () { // watch for any scope changes
                if (element.find('.list-panel').length == 0) {
                    element.addClass('empty');
                } else {
                    element.removeClass('empty');
                }
            });
        }
    }
});

baseModule.directive('modelCheckbox', function () {
    return {
        restrict: 'A',
        scope: {
            ngModel: '=',
            ngLabel: '=',
            ngDisabled: '=',
            ngClick: '&',
        },

        template: '' +
          '<input type="checkbox" ng-model="ngModel" ng-disabled="ngDisabled" ng-click="ngClick">' +
          '<label>{{ngLabel}}</label>',

        link: {
            pre: function (scope, element, attrs) {
                // prevent click behavior on disabled
                element.bind('click', function (e) {
                    if (scope.ngDisabled == true) {
                        e.stopImmediatePropagation();
                        e.preventDefault();
                    } else {
                        scope.ngModel = !scope.ngModel;
                        if (scope.$parent.form) {
                            scope.$parent.form.$setDirty();
                        }
                        scope.$apply();
                    }
                });
            }
        }
    };
});

baseModule.directive('modelCheckboxReversed', function () {
    return {
        restrict: 'A',
        scope: {
            ngModel: '=',
            ngLabel: '=',
            ngDisabled: '=',
            ngClick: '&',
        },

        template: '' +
          '<input type="checkbox" ng-model="ngModel" ng-disabled="ngDisabled" ng-click="ngClick" ng-true-value="false" ng-false-value="true">' +
          '<label>{{ngLabel}}</label>',

        link: {
            pre: function (scope, element, attrs) {
                // prevent click behavior on disabled
                element.bind('click', function (e) {
                    if (scope.ngDisabled == true) {
                        e.stopImmediatePropagation();
                        e.preventDefault();
                    } else {
                        scope.ngModel = !scope.ngModel;
                        if (scope.$parent.form) {
                            scope.$parent.form.$setDirty();
                        }
                        scope.$apply();
                    }
                });
            }
        }
    };
});

baseModule.directive('checkbox', function () {
    return {
        scope: {
            ngChecked: '=',
            ngLabel: '=',
            ngDisabled: '='
        },

        template: '' +
          '<input type="checkbox" ng-checked="ngChecked" ng-disabled="ngDisabled">' +
          '<label>{{ngLabel}}</label>',

        link: {
            pre: function (scope, element, attrs) {
                // prevent click behavior on disabled
                element.bind('click', function (e) {
                    if (scope.ngDisabled == true) {
                        e.stopImmediatePropagation();
                        e.preventDefault();
                    } else {
                        if (scope.$parent.form) {
                            scope.$parent.form.$setDirty();
                        }
                    }
                });
            }
        }
    };
});

baseModule.directive('modelRadio', function () {
    return {
        restrict: 'A',
        scope: {
            ngModel: '=',
            ngValue: '=',
            ngLabel: '=',
            ngDisabled: '=',
        },

        template: '' +
          '<input type="radio" ng-model="ngModel" ng-value="ngValue" ng-disabled="ngDisabled">' +
          '<label>{{ngLabel}}</label>',

        link: {
            pre: function (scope, element, attrs) {
                // prevent click behavior on disabled
                element.bind('click', function (e) {
                    if (scope.ngDisabled != true) {
                        scope.ngModel = scope.ngValue;
                        if (scope.$parent.form) {
                            scope.$parent.form.$setDirty();
                        }
                        scope.$apply();
                    }
                });
            }
        }
    };
});

baseModule.directive('searchDropDown', ['baseBo', '$rootScope', function (baseBo, $rootScope) {
    return {
        restrict: 'A',
        scope: {
            inputString: '@',
            inputKey: '=',
            searchUrl: '@',
            searchObject: '=',
            resultString: '@',
            resultDescription: '@',
            noResultText: '@',
            noResultFunction: '&',
            assignMethod: '@',
            noValidate: '@',
            placeholder: '@',
            ngDisabled: '=',
            clearInputString: '@',
            multiSelect: '@',
            inputKeys: '=',
            onChange: '&',
            searchStringBindTo: '=',
            debugMode: '@'
        },
        link: function ($scope, $element, $attrs) {
            $scope.results = [];
            $scope.selectedIndex = 0;
            $scope.showContainer = false;
            $scope.multiSelect = ($scope.multiSelect === 'true') ? true : false;
            $scope.searchString = $scope.inputString;
            $scope.formatString = $rootScope.formatString;
            $scope.debugMode = ($scope.debugMode === 'true') ? true : false;

            if ($scope.debugMode) {

            }

            $scope.$watch('searchString', function (newVal, oldVal) {
                if (oldVal == newVal)
                    return;

                if ($scope.doNotSearch) {
                    $scope.doNotSearch = false;
                    return;
                }

                $scope.searchStringBindTo = $scope.searchString;
                $scope.search();
            });

            $scope.$watch('searchObject', function () {
                $scope.results = $scope.getResultsFromSearchObject();
            });

            $scope.$watch('inputKey', function (newVal, oldVal) {
                //
                if (oldVal == newVal)
                    return;

                if (newVal == null) {
                    $scope.doNotSearch = true;
                    $scope.searchString = null;
                }
            });

            $element.find('input').focus(function () {
                $scope.search();
                $scope.$apply();
            });

            $scope.timeOut = null;
            $scope.search = function () {
                if ($scope.searchUrl) {
                    clearTimeout($scope.timeOut);
                    $scope.timeOut = setTimeout(function () {
                        baseBo.httpRequest('GET', $scope.searchUrl, { searchString: $scope.searchString }).then(function (result) {
                            $scope.results = [];
                            for (var i in result.model) {
                                var newResult = {
                                    Id: result.model[i].Id,
                                    String: result.model[i][$scope.resultString],
                                    Object: result.model[i],
                                };

                                if ($scope.resultDescription) {
                                    newResult.Description = result.model[i][$scope.resultDescription];
                                }

                                $scope.results.push(newResult);
                            }

                            $scope.showContainer = true;
                        });
                    }, 500);
                } else {
                    $scope.results = $scope.getResultsFromSearchObject();
                    $scope.showContainer = true;
                }
            };

            $scope.getResultsFromSearchObject = function () {
                var results = [];

                for (var i in $scope.searchObject) {
                    var newResult = {
                        Id: $scope.searchObject[i].Id,
                        String: $scope.searchObject[i][$scope.resultString],
                        Object: $scope.searchObject[i]
                    };

                    if ($scope.resultDescription) {
                        newResult.Description = $scope.searchObject[i][$scope.resultDescription];
                    };

                    if (!$scope.searchString || newResult.String.startsWith($scope.searchString)) {
                        results.push(newResult);
                    }
                }

                return results;
            };

            $scope.assign = function (result) {
                if (!$scope.multiSelect) {
                    if ($scope.assignMethod === "string") {
                        $scope.inputKey = result.String;
                    } else if ($scope.assignMethod === "object") {
                        $scope.inputKey = angular.copy(result.Object);
                    } else {
                        $scope.inputKey = result.Id; // normal Id
                    }

                    $scope.doNotSearch = true;

                    if ($scope.clearInputString == "true") {
                        $scope.doNotSearch = true;
                        $scope.searchString = null;
                    } else {
                        $scope.searchString = result.String;
                    }
                } else {
                    $scope.inputKeys = (!$scope.inputKeys) ? [] : $scope.inputKeys;
                    var index = $scope.getIndexOfKeyInInputKeys(result.Id);
                    if (index) {
                        $scope.inputKeys.splice(index, 1);
                    } else {
                        $scope.inputKeys.push(result.Id);
                    }
                }

                $scope.showContainer = false;

                if ($scope.$parent.form) {
                    $scope.$parent.form.$setDirty();
                }

                $scope.onChange();
            };

            $scope.getIndexOfKeyInInputKeys = function (key) {
                for (var i in $scope.inputKeys) {
                    if ($scope.inputKeys[i] === key) {
                        return i;
                    }
                }

                return null;
            };

            $scope.getStringById = function (id) {
                for (var i in $scope.results) {
                    if ($scope.results[i].Id == id) {
                        return $scope.results[i].String;
                    }
                }
            };

            $scope.hover = function (resultIndex) {
                $scope.selectedIndex = resultIndex;
            };

            $scope.isHovered = function (resultIndex) {
                return $scope.selectedIndex === resultIndex;
            };

            $scope.unfocus = function () {
                var matched = false;

                // check if there's an exact match and use it
                if ($scope.results.length > 0) {
                    for (var i in $scope.results) {
                        if ($scope.results[i].String === $scope.searchString) {
                            $scope.assign($scope.results[i]);
                            matched = true;
                            break;
                        }
                    }
                }

                if ($scope.noValidate == 'true' && $scope.assignMethod == "string") {
                    $scope.inputKey = $scope.searchString;
                } else {
                    if (!matched) {
                        $scope.inputKey = null;
                        $scope.doNotSearch = true;
                        $scope.searchString = null;
                    }
                }

                $scope.showContainer = false;
                $scope.$apply();
            };

            // on click away
            $(document).mousedown(function (e) {
                if ($scope.showContainer && !$element.is(e.target) && $element.has(e.target).length === 0) {
                    $scope.unfocus();
                }
            });

            $element.on('keydown', 'input:focus', function (e) {
                //e.stopPropagation();

                if (e.keyCode == '38' || e.keyCode == '40') {
                    if (e.keyCode == '38') // up
                    {
                        e.preventDefault();
                        $scope.selectedIndex = ($scope.selectedIndex > 0) ? $scope.selectedIndex - 1 : $scope.selectedIndex;
                    }

                    if (e.keyCode == '40') // down
                    {
                        e.preventDefault();
                        $scope.selectedIndex = ($scope.selectedIndex < $scope.results.length - 1) ? $scope.selectedIndex + 1 : $scope.selectedIndex;
                    }

                    $scope.$apply();

                    var containerTop = $element.find('.results-container').offset().top;
                    var containerHeight = $element.find('.results-container').height();
                    var selectedResultTop = $element.find('.result-row').eq($scope.selectedIndex).offset().top;
                    var selectedResultHeight = $element.find('.result-row').eq($scope.selectedIndex).outerHeight();

                    if (selectedResultTop >= containerTop + containerHeight) {
                        $element.find('.results-container').scrollTop($element.find('.results-container').scrollTop() + selectedResultHeight);
                    }

                    if (selectedResultTop < containerTop) {
                        $element.find('.results-container').scrollTop($element.find('.results-container').scrollTop() - selectedResultHeight);
                    }
                }

                if (e.keyCode == '13') // enter
                {
                    $scope.assign($scope.results[$scope.selectedIndex]);
                    $scope.$apply();
                    e.preventDefault();
                }
            });

            $(document).on('keydown', function (e) {
                if (e.keyCode == '27') {
                    $scope.unfocus();
                }
            });
        },
        template:
            '' +
            '<input ng-show="!multiSelect" type="text" ng-model="searchString" placeholder={{placeholder}} ng-disabled="ngDisabled" />' +

            '<div ng-show="multiSelect" class="input-container" ng-class="{counter: inputKeys.length > 0}" ng-click="search()">' +
                '<div ng-show="inputKeys.length > 0" ng-click="inputKeys = []; $event.stopPropagation();" class="counter">' +
                    '<span class="number">{{inputKeys.length}}</span>' +
                    '<span class="clear">X</span>' +
                '</div>' +
                '<div ng-show="!inputKeys || inputKeys.length == 0" class="placeholder">{{placeholder}}</div>' +
                '<div class="inner-container">' +
                    '<span ng-repeat="key in inputKeys" class="tag">{{getStringById(key)}}</span>' +
                '</div>' +
            '</div>' +

            '<div class="results-container" ng-show="showContainer">' +
                '<div ng-show="results.length > 0" class="result-row" ng-repeat="result in results" ng-click="assign(result)" ng-mouseover="hover($index)" ng-class="{hovered: isHovered($index)}">' +
                    '<div>' +
                        '<span checkbox ng-show="multiSelect" ng-checked="getIndexOfKeyInInputKeys(result.Id)" style="position: relative; top: -1px;"></span>' +
                        '<span>{{formatString(result.String)}}</span>' +
                    '</div>' +
                    '<div class="description">{{result.Description}}</div>' +
                '</div>' +
                '<div ng-show="results.length == 0" class="no-result">' +
                    'No Results. ' + '<span ng-show="noResultText" class="link" ng-click="noResultFunction()">{{noResultText}}</span>' +
                '</div>' +
            '</div>'
    };
}]);

baseModule.directive('fileUploader', function () {
    return {
        restrict: 'A',
        scope: {
            accept: '@',
            multiple: '@',
            onChange: '&'
        },
        link: function ($scope, $element, $attrs) {
            $scope.filesCount = 0;
            $scope.multiple = ($scope.multiple == 'true') ? true : false;

            $element.find('.file-uploader-input').change(function () {
                if (this.files && this.files.length > 0) {
                    $scope.fileName = this.files[0].name;
                    $scope.filesCount = this.files.length;
                } else {
                    $scope.filesCount = 0;
                }

                if ($scope.onChange) {
                    $scope.onChange();
                }

                $scope.$apply();
            });
        },

        template: '' +
          '<div class="file-uploader-container" ng-class="{active: filesCount > 0}">' +
              '<div ng-show="filesCount == 0" class="default-text" ng-show="true">' +
                  'Drop file<span ng-show="mutliple">(s)</span> here or click to browse' +
              '</div>' +
              '<div ng-show="filesCount > 0" class="uploading-files">' +
                   '<div class="material-icons">description</div>' +
                   '<div ng-show="filesCount == 1" class="ellipsis">{{fileName}}</div>' +
                   '<div ng-show="filesCount > 1">{{filesCount}} files selected</div>' +
              '</div>' +
              '<input class="file-uploader-input" name="file" type="file" accept="{{accept}}" ng-multiple="multiple" />' +
          '</div>',
    };
});