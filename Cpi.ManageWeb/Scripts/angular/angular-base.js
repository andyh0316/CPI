var baseModule = angular.module('AngularBaseModule', ['ngAnimate']);

baseModule.config(['$httpProvider', '$animateProvider', function ($httpProvider, $animateProvider) {
    $httpProvider.defaults.headers.common['Cache-Control'] = 'no-cache, no-store, must-revalidate';
    $httpProvider.defaults.headers.common['Pragma'] = 'no-cache';
    $httpProvider.defaults.headers.common['Expires'] = '0';
    $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';

    // when ngAnimate is referenced: it will cause slow down on all elements. 
    // To prevent this, we use this so that only the elements with certain class will be checked by angular for animation
    $animateProvider.classNameFilter(/ng-animate-enabled/);
}]);


baseModule.controller('BaseController', ['$scope', '$state', function ($scope, $state) {

}]);

/* provides server/backend sorting, paging and searching. For client side sorting and paging */
baseModule.controller('ListBaseController', ['$scope', '$controller', 'baseBo', function ($scope, $controller, baseBo) {
    angular.extend(this, $controller('BaseController', { $scope: $scope }));

    // this is expected to be inherited in the child list controllers
    $scope.getList = function (loadMore) {
        $scope.scopeData.filter.Loads = (loadMore) ? $scope.scopeData.filter.Loads + 1 : 0;

        console.time('listLoadTimer');
        baseBo.httpRequest($scope.scopeData.httpRequest.method, $scope.scopeData.httpRequest.url, $scope.scopeData.filter)
            .then(function (result) {
                console.timeEnd('listLoadTimer');
                if (loadMore)
                {
                    $scope.model.Records = $scope.model.Records.concat(result.Object.Records);
                }
                else // if we are not loading more but getting a new list instead (etc. sort), then we need to maintain the records from the old list that are being edited and put those on top
                {
                    // remove records from not being edited
                    for (var i = $scope.model.Records.length - 1; i >= 0; i--) 
                    {
                        if (!$scope.model.Records[i].isEditing)
                        {
                            $scope.model.Records.splice(i, 1);
                        }
                    }

                    // append new list: excluding the ones who are already being edited
                    for (var i in result.Object.Records)
                    {
                        var recordExists = false;
                        for (var j in $scope.model.Records)
                        {
                            if (result.Object.Records[i].Id === $scope.model.Records[j].Id)
                            {
                                recordExists = true;
                                break;
                            }
                        }

                        if (!recordExists)
                        {
                            $scope.model.Records.push(result.Object.Records[i]);
                        }
                    }
                }

                $scope.model.ListLoadCalculator = result.Object.ListLoadCalculator;
            });
    };

    $scope.edit = function (record) {
        record.isEditing = true;
        record.originalObject = angular.copy(record);
    };

    $scope.isEditingAny = function () {
        for (var i in $scope.model.Records)
        {
            if ($scope.model.Records[i].isEditing) {
                return true;
            }
        }

        return false;
    };

    $scope.cancel = function (record) {
        if (record.Id > 0) { // if existing record
            var originalObject = angular.copy(record.originalObject);
            record.originalObject = null;
            for (var i in originalObject) {
                record[i] = originalObject[i];
            }
            record.isEditing = false;
        }
        else { // if new record
            for (var i in $scope.model.Records) {
                if (record === $scope.model.Records[i]) {
                    $scope.model.Records.splice(i, 1);
                }
            }
        }
    };

    $scope.cancelAll = function () {
        for (var i = $scope.model.Records.length - 1; i >= 0; i--) {
            var record = $scope.model.Records[i];

            if (record.isEditing) {
                $scope.cancel(record);
            }
        }
    };

    $scope.getEditingRecordIndex = function (record) {
        var index = 0;
        for (var i in $scope.model.Records)
        {
            if ($scope.model.Records[i].isEditing)
            {
                if ($scope.model.Records[i] === record) {
                    return index;
                }
                index++;
            }
        }
    }

    $scope.$watch('model.Records', function (oldVal, newVal) {
        if (oldVal.filter(function (record) { return record.isEditing === true; }).length !== newVal.filter(function (record) { return record.isEditing === true; }).length)
        {
            $scope.modelState = null;
        }
    }, true);

    //$scope.filter = {
    //    Loads: 0,
    //    SortColumn: null, // needs to be defined in every child list controller
    //    SortDesc: false,
    //    SearchString: null,
    //    AdvancedSearch: {}
    //};

    // this is called when the child wants to reload the parent list.. for example, when done saving a student, the list will get the updated
    // entries
    $scope.$on('reloadListEvent', function () {
        $scope.getList();
    });

    /**** SORT ****/
    $scope.sort = function (sortColumn) {
        if ($scope.scopeData.filter.SortColumn == sortColumn) {
            $scope.scopeData.filter.SortDesc = !$scope.scopeData.filter.SortDesc;
        } else {
            $scope.scopeData.filter.SortDesc = false;
        }

        $scope.scopeData.filter.SortColumn = sortColumn;

        $scope.getList();
    };

    $scope.getSortOrder = function (sortColumn) {
        if ($scope.scopeData.filter.SortColumn == sortColumn) {
            return ($scope.scopeData.filter.SortDesc) ? 'sorted-desc' : 'sorted-asc';
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
    $scope.searchStringTimeout = null;
    $scope.$watch('scopeData.filter.SearchString', function (newVal, oldVal) {
        if (!newVal && !oldVal) {
            return;
        }

        clearTimeout($scope.searchStringTimeout);
        $scope.searchStringTimeout = setTimeout(function () {
            if ($scope.doNotTriggerSearchString) {
                $scope.doNotTriggerSearchString = false;
                return;
            }

            $scope.getList();
        }, gSearchInputDelay); // wait for user to finish typing input: how many ms to wait
    });

    $scope.searchStringGo = function () {
        $scope.doNotTriggerSearchString = true;
        $scope.getList();
    };

    /**** ADVANCED SEARCH ****/
    $scope.advancedSearchGo = function () {
        $scope.advancedSearchPrevious = angular.copy($scope.scopeData.filter.AdvancedSearch);
        $scope.getList();
        $scope.showAdvancedSearch = false;
    };

    $scope.advancedSearchReset = function () {

    };

    $scope.advancedSearchUndo = function () {
        $scope.scopeData.filter.AdvancedSearch = angular.copy($scope.advancedSearchPrevious);
    };

    $scope.$watch('showAdvancedSearch', function () {
        if ($scope.showAdvancedSearch === true) {
            $scope.scopeData.filter.AdvancedSearch = ($scope.scopeData.filter.AdvancedSearch) ? $scope.scopeData.filter.AdvancedSearch : {};
            $scope.advancedSearchPrevious = angular.copy($scope.scopeData.filter.AdvancedSearch);
        }
    })

    //if (typeof gShowAdvancedSearch !== 'undefined') {
    //    $scope.showAdvancedSearch = gShowAdvancedSearch;
    //} else {
    //    $scope.showAdvancedSearch = false;
    //}

    //$scope.doNotTriggerAdvancedSearch = false;
    //$scope.advancedSearchTimeout = null;
    //$scope.$watch('filter.AdvancedSearch', function (newVal, oldVal) {
    //    if ($scope.doNotTriggerAdvancedSearch) {
    //        $scope.doNotTriggerAdvancedSearch = false;
    //        return;
    //    }

    //    if (newVal == oldVal)
    //        return;

    //    var advancedSearchDelay = 0;
    //    if ($scope.delayAdvancedSearch) {
    //        $scope.delayAdvancedSearch = false;
    //        advancedSearchDelay = 700;
    //    }

    //    $scope.page = 1;

    //    clearTimeout($scope.advancedSearchTimeout);
    //    $scope.advancedSearchTimeout = setTimeout(function () {
    //        $scope.getList();
    //    }, advancedSearchDelay);
    //}, true);

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
        return angularDateFilter(date, 'dd/MM/yyyy');
    }
}]);

baseModule.filter('shortDateWithTime', ['$filter', function ($filter) {
    var angularDateFilter = $filter('date');
    return function (date) {
        return angularDateFilter(date, 'dd/MM/yyyy hh:mm a');
    }
}]);

baseModule.filter('localDateTime', ['$filter', function ($filter) {
    return function (date) {
        if (!date)
        {
            return;
        }

        var date = new Date(date); // just by doing this javascript will automatically convert the time to local
        //date.setHours(date.getHours() + 7);
        return date;
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

baseModule.filter('callCommodities', function () {
    return function (callCommodities) {
        if (!callCommodities)
        {
            return null;
        }

        var returnString = '';

        for (var i = 0; i < callCommodities.length; i++)
        {
            returnString = returnString + callCommodities[i].Commodity.Name;
            if (callCommodities[i].Quantity > 1)
            {
                returnString = returnString + ' ' + '(' + callCommodities[i].Quantity + ')'
            }

            if (callCommodities.length > 1 && i !== callCommodities.length - 1)
            {
                returnString = returnString + ', ';
            }
        }

        return returnString;
    }
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

            //var dateFormat = 'dd/MM/yyyy';

            //ngModelController.$formatters.push(function (modelValue) {
            //    return 'yomama';
            //    return $filter('date')(modelValue, dateFormat);

            //    //var currentDate = new Date(modelValue + "+00:00");
            //    //currentDate = new Date((currentDate.getTime() + (currentDate.getTimezoneOffset() * 60 * 1000)));

            //    //if (isNaN(currentDate) === false) {
            //    //    return $filter('date')(currentDate, dateFormat);
            //    //}
            //    //return undefined;
            //});

            ngModelController.$parsers.push(function (modelValue) {
                dateArray = modelValue.split("/");
                //var date = new Date(dateArray[1] + "/" + dateArray[0] + "/" + dateArray[2]);
                var date = new Date(dateArray[2], dateArray[1] - 1, dateArray[0]);

                if (Object.prototype.toString.call(date) === "[object Date]") {
                    if (isNaN(date.getTime())) {  // d.valueOf() could also work
                        // date is not valid
                        return modelValue;
                    }
                    else {
                        // date is valid
                        console.log(date);
                        return date;
                    }
                }

                return modelValue;
            });

            ngModelController.$formatters.push(function (modelValue) {
                return $filter('date')(modelValue, 'dd/MM/yyyy');
            });

            //element.datepicker({
            //    dateFormat: 'mm/dd/yy',
            //    yearRange: '1950:2020',
            //    changeMonth: true,
            //    changeYear: true,
            //    onSelect: function (dateText) {
            //        scope.$apply(function (scope) {
            //            // call $parsers pipeline then update $modelValue
            //            ngModelController.$setViewValue(dateText);
            //            // update the local view
            //            ngModelController.$render();
            //        });
            //        element.blur();
            //    }
            //});

            ////Check the attributes for a 'yearRange'                
            //if (attrs && attrs.yearrange) {
            //    element.datepicker("option", "yearRange", attrs.yearrange);
            //}
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
                            $scope.results = result.Object;
                            $scope.showContainer = true;
                        });
                    }, gSearchInputDelay);
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
                        Name: $scope.searchObject[i].Name,
                        Description: $scope.searchObject[i].Description,
                    };

                    if (!$scope.searchString || newResult.Name.startsWith($scope.searchString)) {
                        results.push(newResult);
                    }
                }

                return results;
            };

            $scope.assign = function (result) {
                if (!$scope.multiSelect) {
                    if ($scope.assignMethod === "string") {
                        $scope.inputKey = result.Name;
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
                        $scope.searchString = result.Name;
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
                        return $scope.results[i].Name;
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
                        if ($scope.results[i].Name === $scope.searchString) {
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

            $element.find('input').focusout(function () {
                setTimeout(function () {
                    $scope.unfocus();
                }, 100);
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
                        '<span>{{result.Name}}</span>' +
                    '</div>' +
                    '<div class="description">{{result.Description}}</div>' +
                '</div>' +
                '<div ng-show="results.length == 0" class="no-result">' +
                    'No Results. ' + '<span ng-show="noResultText" class="link" ng-click="noResultFunction()">{{noResultText}}</span>' +
                '</div>' +
            '</div>'
    };
}]);

baseModule.directive('commoditiesEdit', function () {
    return {
        restrict: 'A',
        scope: {
            ngModel: '=',
            commodities: '='
        },
        link: function ($scope, $element, $attrs) {
            $scope.ngModel = ($scope.ngModel) ? $scope.ngModel : [];

            $(document).mousedown(function (e) {
                if ($scope.showEditContainer && !$element.is(e.target) && $element.has(e.target).length === 0) {
                    $scope.justClickedInsideContainer = true;
                    $scope.showEditContainer = false;
                    $scope.$apply();
                }
            });

            $element.find('input').focus(function () {
                    $scope.showEditContainer = true;
                    $scope.$apply();
            });

            $element.find('input').focusout(function () {
                if (!$scope.justClickedInsideContainer)
                {
                    $scope.showEditContainer = false;
                    $scope.$apply();
                }

                $scope.justClickedInsideContainer = false;
            });

            $scope.getCommodityQuantityFor = function (item) {
                for (var i in $scope.ngModel)
                {
                    if ($scope.ngModel[i].CommodityId === item.Id)
                    {
                        return $scope.ngModel[i].Quantity;
                    }
                }

                return 0;
            };

            $scope.addQuantity = function (item) {
                // first try to find the item in the ngModel
                for (var i in $scope.ngModel)
                {
                    if ($scope.ngModel[i].CommodityId === item.Id) // if found .. add to the quantity
                    {
                        $scope.ngModel[i].Quantity++;
                        return;
                    }
                }

                // else create the object in ngModel
                var newCallCommodity = {
                    CommodityId: item.Id,
                    Quantity: 1
                }
                $scope.ngModel.push(newCallCommodity);
            };

            $scope.subtractQuantity = function (item) {
                // find the item in the ngModel
                for (var i in $scope.ngModel) {
                    if ($scope.ngModel[i].CommodityId === item.Id) // if found .. subtract quantity
                    {
                        $scope.ngModel[i].Quantity--;
                        if ($scope.ngModel[i].Quantity === 0) // is quantity is 0: take it out of the array
                        {
                            $scope.ngModel.splice(i, 1);
                        }
                        return;
                    }
                }
            };

            $scope.getCommodityName = function (item) {
                for (var i in $scope.commodities)
                {
                    if ($scope.commodities[i].Id === item.CommodityId)
                    {
                        return $scope.commodities[i].Name;
                    }
                }
            };
        },
        template:
        '' +
        '<div class="commodities-edit">' +
            '<div class="view-container input-container" ng-click="showEditContainer = true">' +
                '<span ng-repeat="item in ngModel">' +
                    '{{getCommodityName(item)}}' + 
                    '<span ng-show="item.Quantity > 1"> ({{item.Quantity}})</span>' +
                    '<span ng-show="ngModel.length > 1 && $index != ngModel.length - 1">, </span>' +
                '</span>' +
            '</div>' +
            '<input/>' + // this input is always invisible: it helps to include this editor in tab order and when focused through tab it will show edit container 
            '<div ng-show="showEditContainer" class="edit-container">' +
                '<div ng-repeat="item in commodities" class="edit-row">' +
                    '<span class="item-name">{{item.Name}}</span>' +
                    '<span class="minus" ng-click="subtractQuantity(item)"></span>' +
                    '<span class="quantity">{{getCommodityQuantityFor(item)}}</span>' +
                    '<span class="plus" ng-click="addQuantity(item)"></span>' +
                '</div>' +
            '</div>' +
        '</div>'
    };
});

//baseModule.directive('addressViewEdit', function () {
//    return {
//        restrict: 'A',
//        scope: {
//            ngModel: '=',
//            isEditing: '='
//        },
//        link: function ($scope, $element, $attrs) {
//            $scope.ngModel = ($scope.ngModel) ? $scope.ngModel : [];
//        },
//        template:
//        '' +
//        '<div class="address-view-edit">' +
//            '<div class="view-container" ng-click="showEditContainer = (isEditing) ? true : false" ng-class="{\'input-container\': isEditing}">' +
//            '</div>' +
//            '<div ng-show="showEditContainer" class="edit-container">' +
//                '<input ng-model="ngModel.Street" placeholder="Street" />' +
//                '<input ng-model="ngModel.Vilas" placeholder="Vilas" />' +
//                '<input ng-model="ngModel.Sonka" placeholder="Sonka" />' +
//                '<input ng-model="ngModel.District" placeholder="District" />' +
//                '<input ng-model="ngModel.City" placeholder="City" />' +
//            '</div>' +
//        '</div>'
//    };
//});

baseModule.directive('fileInput', function () {
    return {
        restrict: 'A',
        scope: {
            accept: '@',
            label: '@',
            name: '@',
            onChange: '&',
        },
        link: function ($scope, $element, $attrs) {
            $scope.filesCount = 0;
            $scope.multiple = ($scope.multiple == 'true') ? true : false;

            $scope.click = function () {
                $element.find('input').click();
            };

            $element.find('input').change(function () {
                if ($scope.onChange) {
                    $scope.onChange();
                }

                $scope.$apply();
            });
        },

        template: '' +
          '<span>' +
              '<input ng-show="false" name="{{name}}" type="file" accept="{{accept}}" />' +
              '<button class="main button" ng-click="click()">{{label}}</label>' +
          '</div>',
    };
});

baseModule.directive('tbody', function () {
    return {
        restrict: 'E',
        link: function ($scope, $element, $attrs) {
            var raw = $element[0];
            $element.bind('scroll', function () {
                //console.log(raw.scrollTop + raw.offsetHeight + ' ' + raw.scrollHeight);
                if (raw.scrollTop + raw.offsetHeight >= raw.scrollHeight) { //at the bottom
                    $scope.getList(true);
                }
            });
        }
    };
});

baseModule.directive('searchBox', function () {
    return {
        restrict: 'A',
        link: function ($scope, $element, $attrs) {
            $element.on('keydown', function (e) {
                if (e.keyCode == '13') // enter
                {
                    $scope.getList();
                }
            });
        }
    };
});

baseModule.directive('advancedSearch', function () {
    return {
        restrict: 'A',
        scope: {
            ngShow: '=',
            advancedSearchUndo: '&'
        },
        link: function ($scope, $element, $attrs) {
            $(document).on('mousedown', function (e) {
                if ($scope.ngShow === true && !$element.is(e.target) && $element.has(e.target).length === 0) {
                    $scope.advancedSearchUndo();
                    $scope.ngShow = false;
                    $scope.$apply();
                }
            });
        },
    };
});

//baseModule.directive('tr', ['$timeout', function ($timeout) {
//    return {
//        restrict: 'E',
//        link: function ($scope, $element, $attrs) {
//            if ($scope.$index < 50)
//            {
//                $element.css('opacity', 0);
//                $element.delay(15 * $scope.$index).fadeTo(200, 1);
//            }
//        }
//    };
//}]);