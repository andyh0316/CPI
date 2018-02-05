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
    $scope.back = function () {
        $state.go('^');
    }

    $scope.viewMode = 'View';
    $scope.createMode = 'Create';
    $scope.editMode = 'Edit';

    $scope.isViewMode = function () {
        return $scope.mode === $scope.viewMode;
    };

    $scope.isCreateMode = function () {
        return $scope.mode === $scope.createMode;
    };

    $scope.isEditMode = function () {
        return $scope.mode === $scope.editMode;
    };

    $scope.setMode = function (mode) {
        $scope.mode = mode;
    };
}]);


baseModule.run(['$rootScope', '$state', function ($rootScope, $state) {
    $rootScope.state = $state;
    $rootScope.isRoleLaozi = gIsRoleLaozi;

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

    $rootScope.generalChartOptions = {
        scales: {
            yAxes: [{
                ticks: {
                    beginAtZero: true
                }
            }]
        }
    };

    $rootScope.smartPrefixes = gSmartPrefixes;
    $rootScope.metFonePrefixes = gMetFonePrefixes;
    $rootScope.cellCardPrefixes = gCellCardPrefixes;
}]);


/* provides server/backend sorting, paging and searching. For client side sorting and paging */
baseModule.controller('ListBaseController', ['$scope', '$controller', 'baseBo', function ($scope, $controller, baseBo) {
    angular.extend(this, $controller('BaseController', { $scope: $scope }));

    $scope.scopeData = {
        filter: {
            SortObjects: [],
            AdvancedSearch: {}
        }
    };

    $scope.listItems = [];
    $scope.listLoadCalculator = {};

    // this is expected to be inherited in the child list controllers
    // We don't want to ever directly assign a new list to $scope.listItems because that would change the referece, instead we manually empty the list, maintaining the reference
    // default: empty old items, set load to 0, concat with new items
    // sorting: empty old items, set load to 0, concat with new items
    // savedList: empty old items, maintain load, concat with new items
    // loadMore: maintain old items, concat with new items
    $scope.getList = function (mode) {
        if (!$scope.scopeData.filter) {
            $scope.scopeData.filter = {};
        }

        if (!$scope.scopeData.filter.Loads) // if loads was not assigned
        {
            $scope.scopeData.filter.Loads = 0;
        }

        $scope.scopeData.filter.LoadMore = false;
        $scope.scopeData.filter.scrollTop = false;

        if (!mode) {
            $scope.scopeData.filter.Loads = 0;
            $scope.scopeData.filter.scrollTop = true;
        }

        if (mode === 'sorting') {
            $scope.scopeData.filter.Loads = 0;
            $scope.scopeData.filter.scrollTop = true;
        }

        if (mode === 'loadMore') {
            $scope.scopeData.filter.Loads++;
            $scope.scopeData.filter.LoadMore = true;
        }

        baseBo.httpRequest($scope.scopeData.httpRequest.method, $scope.scopeData.httpRequest.url, $scope.scopeData.filter)
            .then(function (result) {
                // remove records not being touched
                if (!mode || mode === 'sorting') {
                    for (var i = $scope.listItems.length - 1; i >= 0; i--) {
                        if (!$scope.listItems[i].touched) {
                            $scope.listItems.splice(i, 1);
                        }
                    }
                }

                if (mode === 'savedList') // if we just finished saving the list, we want to also remove all old list items (including touched), and replace with new ones
                {
                    for (var i = $scope.listItems.length - 1; i >= 0; i--) {
                        $scope.listItems.splice(i, 1);
                    }
                }

                // append new list: excluding the ones who are already being touched
                for (var i in result.Object.ListItems) {
                    var recordExists = false;
                    for (var j in $scope.listItems) {
                        if (result.Object.ListItems[i].Id === $scope.listItems[j].Id) {
                            recordExists = true;
                            break;
                        }
                    }

                    if (!recordExists) {
                        $scope.listItems.push(result.Object.ListItems[i]);
                    }
                }

                $scope.listLoadCalculator = result.Object.ListLoadCalculator;
            });
    };

    $scope.createListItem = function (item) {
        if (item) {
            newItem = item;
        }
        else {
            newItem = {};
        }

        if (!newItem.Id) {
            newItem.Id = 0;
        }

        newItem.touched = true;

        $scope.listItems.unshift(newItem);
        $scope.modelState = null;
    };

    $scope.editListItem = function (item) {
        item.touched = true;
        item.originalObject = angular.copy(item);
    };

    $scope.deleteListItem = function (item) {
        item.touched = true;
        item.Deleted = true;
    }

    $scope.isAnyListItemTouched = function () {
        for (var i in $scope.listItems)
        {
            if ($scope.listItems[i].touched) {
                return true;
            }
        }

        return false;
    };

    $scope.cancelListItem = function (item) {
        var list = $scope.listItems;

        for (var i in list) {
            if (item === list[i]) {
                if (item.Id > 0) {
                    if (item.originalObject) {
                        var originalObject = angular.copy(item.originalObject);
                        list[i] = originalObject;
                    }
                    list[i].Deleted = false;
                    list[i].touched = false;
                }
                else // if it is creating new, just take it out of the list
                {
                    list.splice(i, 1);
                }
            }
        }

        $scope.modelState = null;
    };

    $scope.cancelListItems = function () {
        if (confirm("Are you sure you want to cancel? Press OK to continue.")) {
            for (var i = $scope.listItems.length - 1; i >= 0; i--) {
                var item = $scope.listItems[i];

                if (item.touched) {
                    $scope.cancelListItem(item);
                }
            }

            $scope.modelState = null;
        }
    };

    $scope.getTouchedListItemIndex = function (item) {
        var index = 0;
        for (var i in $scope.listItems)
        {
            if ($scope.listItems[i].touched)
            {
                if ($scope.listItems[i] === item) {
                    return index;
                }
                index++;
            }
        }
    }

    $scope.showListItemInput = function (item) {
        return (item.touched && !item.Deleted);
    };
}]);

baseModule.factory('baseBo', ['$http', '$window', '$q', function ($http, $window, $q) {
    var instance = {};

    gCurrentRequests = 0;

    // Caution: all posts that attempt to save should preferrably pass in a whole model object instead of objects of different models
    // (ex. httpPost(url, $scope.model) instead of httpPost(url, { model: $scope.model })
    // this is because so that ModelState will not prefix keys with parameter name so our angular apps
    // can properly bind to fields to display validation errors generated by MVC
    instance.httpRequest = function (httpMethod, url, params, opts) {
        var noLoadIcon = false;
        if (opts) {
            if (opts.noLoadIcon === true) {
                noLoadIcon = true;
            }
        }

        if (!noLoadIcon) {
            $('#wait-mask').show();
        }

        gCurrentRequests++;

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

        return instance.httpBase(httpVar, noLoadIcon);
    };

    instance.httpBase = function (httpVar, noLoadIcon) {
        var deferred = $q.defer();

        $http(httpVar)
            .success(function (result) {

                if (result.IsSessionExpired) {
                    location.href = '/Public/';
                    return;
                };

                // tell javascript how much time is remaining for session timeout so we can warn the user
                // to extend his session
                setSessionWarning(result.SessionTimeLeft);

                // need to check for if its session timeout
                //successFunc(result);
                deferred.resolve(result);

                gCurrentRequests--;
                if (gCurrentRequests <= 0) {
                    gCurrentRequests = 0;
                    $('#wait-mask').hide();
                }
            })
            .error(function (data, status, headers, config) {
                gCurrentRequests--;
                if (gCurrentRequests <= 0) {
                    gCurrentRequests = 0;
                    $('#wait-mask').hide();
                }

                if (!noLoadIcon)
                {
                    alert("Oops! The application has encountered an unexpected error. Please sign out and sign in again. If this the problem persists, please report to us.");
                }
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
        return angularDateFilter(date, 'dd.MM.yyyy hh:mm a');
    }
}]);

//baseModule.filter('localDateTime', ['$filter', function ($filter) {
//    return function (date) {
//        if (!date)
//        {
//            return;
//        }

//        var date = new Date(date); // just by doing this javascript will automatically convert the time to local;
//        return utcDate;
//    }
//}]);

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

baseModule.filter('objectCommodities', function () {
    return function (objectCommodities) {
        if (!objectCommodities)
        {
            return null;
        }

        var returnString = '';

        for (var i = 0; i < objectCommodities.length; i++)
        {
            if (objectCommodities[i].Id > 0)
            {
                returnString = returnString + objectCommodities[i].Commodity.Name;
                if (objectCommodities[i].Quantity > 1)
                {
                    returnString = returnString + ' ' + '(' + objectCommodities[i].Quantity + ')'
                }

                if (objectCommodities.length > 1 && i !== objectCommodities.length - 1)
                {
                    returnString = returnString + ', ';
                }
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
            ngModelController.$parsers.push(function (modelValue) {
                // take the date MM/dd/yyyy (ex. 30/1/2017) and parse it into arrays of 3
                if (modelValue.split("/").length === 3)
                {
                    dateArray = modelValue.split("/");
                }
                else
                {
                    return modelValue;
                }

                // reverse the order of month and day (ex. 1/30/2017). If you use period instead of / the serializer will understand it as well but no need
                return dateArray[1] + '/' + dateArray[0] + '/' + dateArray[2]; 
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

baseModule.directive('modelCheckbox', ['$rootScope', function ($rootScope) {
    return {
        restrict: 'A',
        transclude: true,
        scope: {
            ngModel: '=',
            ngDisabled: '=',
            ngClick: '&',
        },

        template:
        '<input type="checkbox" ng-model="ngModel" ng-disabled="ngDisabled" ng-click="ngClick"> \
          <label> \
            <ng-transclude></ng-transclude> \
          </label>',

        link: {
            pre: function ($scope, $element, $attrs) {
                // prevent click behavior on disabled
                $element.bind('click', function (e) {
                    if ($scope.ngDisabled == true) {
                        e.stopImmediatePropagation();
                        e.preventDefault();
                    } else {
                        $scope.ngModel = !$scope.ngModel;
                        //$rootScope.setFormDirty($scope);
                        $scope.$apply();
                    }
                });
            }
        }
    };
}]);

baseModule.directive('modelCheckboxReversed', ['$rootScope', function ($rootScope) {
    return {
        restrict: 'A',
        transclude: true,
        scope: {
            ngModel: '=',
            ngDisabled: '=',
            ngClick: '&',
        },

        template:
        '<input type="checkbox" ng-model="ngModel" ng-disabled="ngDisabled" ng-click="ngClick" ng-true-value="false" ng-false-value="true"> \
          <label> \
            <ng-transclude></ng-transclude> \
          </label>',

        link: {
            pre: function ($scope, $element, $attrs) {
                // prevent click behavior on disabled
                $element.bind('click', function (e) {
                    if ($scope.ngDisabled == true) {
                        e.stopImmediatePropagation();
                        e.preventDefault();
                    } else {
                        $scope.ngModel = !$scope.ngModel;
                        //$rootScope.setFormDirty($scope);
                        $scope.$apply();
                    }
                });
            }
        }
    };
}]);

baseModule.directive('checkbox', ['$rootScope', function ($rootScope) {
    return {
        transclude: true,
        scope: {
            ngChecked: '=',
            ngDisabled: '='
        },

        template: '' +
        '<input type="checkbox" ng-checked="ngChecked" ng-disabled="ngDisabled"> \
          <label> \
            <ng-transclude></ng-transclude> \
          </label>',

        link: {
            pre: function ($scope, $element, $attrs) {
                // prevent click behavior on disabled
                $element.bind('click', function (e) {
                    if ($scope.ngDisabled == true) {
                        e.stopImmediatePropagation();
                        e.preventDefault();
                    } else {
                        //$rootScope.setFormDirty($scope);
                    }
                });
            }
        }
    };
}]);

baseModule.directive('modelRadio', ['$rootScope', function ($rootScope) {
    return {
        restrict: 'A',
        transclude: true,
        scope: {
            ngModel: '=',
            ngValue: '=',
            ngDisabled: '=',
        },

        template: '' +
        '<input type="radio" ng-model="ngModel" ng-value="ngValue" ng-disabled="ngDisabled"> \
          <label> \
            <ng-transclude></ng-transclude> \
          </label>',

        link: {
            pre: function ($scope, $element, $attrs) {
                // prevent click behavior on disabled
                $element.bind('click', function (e) {
                    if ($scope.ngDisabled != true) {
                        $scope.ngModel = $scope.ngValue;
                        //$rootScope.setFormDirty($scope)
                        $scope.$apply();
                    }
                });
            }
        }
    };
}]);

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
            
            if ($scope.searchObject) {
                for (var i in $scope.searchObject)
                {
                    if ($scope.inputKey === $scope.searchObject[i].Id)
                    {
                        $scope.searchString = $scope.searchObject[i].Name;
                        break;
                    }
                }
                
            } else {
                $scope.searchString = $scope.inputString;
            }

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

                    if (!$scope.searchString || newResult.Name.toLowerCase().includes($scope.searchString.toLowerCase())) {
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
                '<div ng-show="results.length > 0" class="result-row" ng-repeat="result in results" ng-mousedown="assign(result)" ng-mouseover="hover($index)" ng-class="{hovered: isHovered($index)}">' +
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
            commodities: '=',
            ngChange: '&'
        },
        link: function ($scope, $element, $attrs) {
            $scope.ngModel = ($scope.ngModel) ? $scope.ngModel : [];
            $scope.activeCommodities = $scope.commodities.filter(function (item) { return !item.Inactive });

            $scope.$watch('ngModel', function (newVal, oldVal) {
                if (newVal !== oldVal)
                {
                    $scope.ngChange();
                }
            }, true);

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
                var newObjectCommodity = {
                    CommodityId: item.Id,
                    Quantity: 1
                }
                $scope.ngModel.push(newObjectCommodity);
            };

            $scope.subtractQuantity = function (item) {
                // find the item in the ngModel
                for (var i in $scope.ngModel) {
                    if ($scope.ngModel[i].CommodityId === item.Id) 
                    {
                        if ($scope.ngModel[i].Quantity > 0) // subtract only if quantity is > 0
                        {
                            $scope.ngModel[i].Quantity--;
                        }

                        if ($scope.ngModel[i].Quantity === 0 && !$scope.ngModel[i].Id) // if quantity is 0 and the item was just created, take it out of the array
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
                '<span ng-if="item.Quantity > 0" ng-repeat="item in ngModel">' +
                    '{{getCommodityName(item)}}' + 
                    '<span ng-show="item.Quantity > 1"> ({{item.Quantity}})</span>' +
                    '<span ng-show="ngModel.length > 1 && $index != ngModel.length - 1">, </span>' +
                '</span>' +
            '</div>' +
            '<input/>' + // this input is always invisible: it helps to include this editor in tab order and when focused through tab it will show edit container 
            '<div ng-show="showEditContainer" class="edit-container">' +
                '<div ng-repeat="item in activeCommodities" class="edit-row">' +
                    '<span class="item-name">{{item.Name}}</span>' +
                    '<span class="minus" ng-click="subtractQuantity(item)"></span>' +
                    '<span class="quantity">{{getCommodityQuantityFor(item)}}</span>' +
                    '<span class="plus" ng-click="addQuantity(item)"></span>' +
                '</div>' +
            '</div>' +
        '</div>'
    };
});

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

listBodyDirectiveFunction = ['$timeout', function ($timeout) {
    return {
        restrict: 'AEC',
        transclude: true,
        link: function ($scope, $element, $attrs) {
            var raw = $element[0];
            $scope.hasMoreListItems = false;
            $scope.empty = false;

            $scope.$watch('listLoadCalculator.LoadGuid', function (newVal, oldVal) { // watch for when a getList is finished
                var listItemCount = ($scope.listItems) ? $scope.listItems.filter(function (item) { return item.Id > 0 }).length : 0; // excluding creating new
                if ($scope.listLoadCalculator && listItemCount < $scope.listLoadCalculator.Total) {
                    $scope.hasMoreListItems = true;
                } else {
                    $scope.hasMoreListItems = false;
                }

                if ($scope.scopeData.filter.scrollTop) {
                    $element.scrollTop(0);
                }
            });

            $element.bind('scroll', function () {
                if ($scope.getList) {
                    if ($scope.hasMoreListItems && raw.scrollTop + raw.offsetHeight >= raw.scrollHeight) { //at the bottom
                        $scope.getList('loadMore');
                        console.log(2);
                    }
                }
            });

            $scope.hoverForMoreData = function () {
                if ($scope.getList) {
                    if ($scope.hasMoreListItems) { //at the bottom
                        $scope.getList('loadMore');
                    }
                }
            };
        },
        template:
        '<ng-transclude></ng-transclude> \
        <div ng-show="hasMoreListItems" ng-click="hoverForMoreData()" class="has-more-items">Scroll or Click for More Data</div>'
    }
}];

baseModule.directive('tbody', listBodyDirectiveFunction);

baseModule.directive('fieldValidationError', function () {
    return {
        restrict: 'A',
        scope: {
            ngShow: '=',
        },
        link: function ($scope, $element, $attrs) {
        },
        template: '' +
        '{{ngShow}}'
    };
});

baseModule.directive('advancedSearch', ['$timeout', '$compile', '$rootScope', function ($timeout, $compile, $rootScope) {
    return {
        restrict: 'A',
        scope: true,
        transclude: true,
        link: function ($scope, $element, $attrs, $ctrl, $transclude) {
            $scope.autoTrigger = ($attrs.autoTrigger === 'true') ? true : false;

            /**** ADVANCED SEARCH ****/
            $scope.advancedSearchGo = function () {
                $scope.advancedSearchPrevious = angular.copy($scope.scopeData.filter.AdvancedSearch);
                $scope.getList();
                $scope.showAdvancedSearch = false;
            };

            $scope.advancedSearchReset = function () {
                $scope.advancedSearchPrevious = {};
                $scope.scopeData.filter.AdvancedSearch = {};
                $scope.getList();
            };

            $scope.advancedSearchUndo = function () {
                $scope.scopeData.filter.AdvancedSearch = angular.copy($scope.advancedSearchPrevious);
            };

            $scope.getAdvancedSearchCount = function () {
                if ($scope.scopeData.filter.AdvancedSearch) {
                    var count = 0;
                    var object = $scope.scopeData.filter.AdvancedSearch;
                    return $scope.getAdvancedSearchCountRecursionFunction(object);
                }
            };

            $scope.getAdvancedSearchCountRecursionFunction = function (object) {
                var count = 0;
                for (var property in object) {
                    if (object.hasOwnProperty(property)) {
                        if (object[property]) {
                            if (typeof object[property] === 'object') {
                                count = count + $scope.getAdvancedSearchCountRecursionFunction(object[property]);
                            }
                            else {
                                count++;
                            }
                        }
                    }
                }
                return count;
            };

            $scope.$watch('showAdvancedSearch', function () {
                if ($scope.showAdvancedSearch === true) {
                    $scope.scopeData.filter.AdvancedSearch = ($scope.scopeData.filter.AdvancedSearch) ? $scope.scopeData.filter.AdvancedSearch : {};
                    $scope.advancedSearchPrevious = angular.copy($scope.scopeData.filter.AdvancedSearch);
                }
            });

            $scope.$watch('scopeData.filter.AdvancedSearch', function (newVal, oldVal) {
                if (newVal !== oldVal) {
                    if ($scope.autoTrigger) {
                        $scope.searchDelay = 0;
                        if ($scope.delaySearch) {
                            $scope.delaySearch = false;
                            $scope.searchDelay = 1000;
                        }

                        $timeout.cancel($scope.timeoutId);
                        $scope.timeoutId = $timeout(function () {
                            $scope.getList();
                        }, $scope.searchDelay);
                    }
                }
            }, true);

            //$(document).on('mousedown', function (e) {
            //    $criteriaContainer = $element.find('.criteria-container');
            //    if ($scope.showAdvancedSearch === true && !$criteriaContainer.is(e.target) && $criteriaContainer.has(e.target).length === 0) {
            //        $scope.advancedSearchUndo();
            //        $scope.showAdvancedSearch = false;
            //        $scope.$apply();
            //    }
            //});
        },
        //compile: function ($element, $attr, $transclude) {
        //    $transclude().find('[ng-model]').attr('ignore-dirty', '');
        //    $element.find('.controls-container').append($transclude());
        //    var fn = $compile($element.html());
        //    return function ($scope) {
        //        fn($scope);
        //    };
        //},
        template:
        '<ng-form name="form"> \
            <button ng-show="!showAdvancedSearch && !autoTrigger" ng-click="showAdvancedSearch = true" class="button"> \
                Advanced \
                <span ng-show="getAdvancedSearchCount() > 0"> ({{getAdvancedSearchCount()}})</span> \
            </button> \
            <div ng-show="(showAdvancedSearch && !autoTrigger) || autoTrigger" class="criteria-container" ng-class="{autoTrigger: autoTrigger, manualTrigger: !autoTrigger}"> \
                <ng-transclude></ng-transclude> \
                <div ng-show="!autoTrigger" class="buttons-container"> \
                    <button class="button main" type="submit" ng-click="advancedSearchGo()">Search</button> \
                    <button class="button main" ng-click="advancedSearchReset()">Reset</button> \
                    <button class="button" ng-click="showAdvancedSearch = false; advancedSearchUndo();">Close</button> \
                </div> \
            </div> \
        </ng-form>'
    };
}]);

baseModule.directive('simpleSearch', function () {
    return {
        restrict: 'A',
        scope: true,
        link: function ($scope, $element, $attrs) {
            $element.on('keydown', function (e) {
                if (e.keyCode == '13') // enter
                {
                    $scope.getList();
                }
            });

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
                }, 500); // wait for user to finish typing input: how many ms to wait
            });
        },
        template: '' +
        '<input ng-model="scopeData.filter.SearchString" placeholder="Search">'
    };
});

baseModule.directive('ngSort', function () {
    return {
        restrict: 'A',
        scope: {
            ngSort: '@'
        },
        transclude: true,
        link: function ($scope, $element, $attrs) {
            /**** SORT ****/
            $scope.sort = function (sortColumn) {
                $scope.$parent.scopeData.filter.SortObjects = ($scope.$parent.scopeData.filter.SortObjects) ? $scope.$parent.scopeData.filter.SortObjects : [];
                var sortObjects = $scope.$parent.scopeData.filter.SortObjects;

                var columnExistsInSortObjects = false;
                for (var i in sortObjects) {
                    var sortObject = sortObjects[i];
                    if (sortObject.ColumnName === sortColumn) {
                        sortObjects.splice(i, 1);

                        if (!sortObject.IsDescending) {
                            sortObject.IsDescending = true;
                            sortObjects.push(sortObject);
                        }

                        columnExistsInSortObjects = true;
                        break;
                    }
                }

                if (!columnExistsInSortObjects) {
                    sortObjects.push({ ColumnName: sortColumn, IsDescending: false });
                }

                // this will make the sort singular: if there's 2 or more sortObjects, pop until it has only one
                for (var i = sortObjects.length - 2; i >= 0; i--)
                {
                    sortObjects.splice(i, 1);
                }

                $scope.$parent.getList('sorting');
            };

            $scope.getSortDirection = function (sortColumn) {
                if ($scope.$parent.scopeData.filter.SortObjects) {
                    var sortObjects = $scope.$parent.scopeData.filter.SortObjects;
                    for (var i in sortObjects) {
                        if (sortObjects[i].ColumnName === sortColumn) {
                            return (sortObjects[i].IsDescending) ? "sorted-desc" : "sorted-asc";
                        }
                    }
                }
            };

            $scope.getSortColumnOrder = function (sortColumn) {
                if ($scope.$parent.scopeData.filter.SortObjects) {
                    var sortObjects = $scope.$parent.scopeData.filter.SortObjects;
                    for (var i in sortObjects) {
                        if (sortObjects[i].ColumnName === sortColumn) {
                            return parseInt(i) + 1;
                        }
                    }
                }
            };

            $scope.getSortColumnCount = function () {
                return ($scope.$parent.scopeData.filter.SortObjects) ? $scope.$parent.scopeData.filter.SortObjects.length : 0;
            };

            $element.bind('click', function (e) {
                $scope.sort($scope.ngSort);

                $scope.$apply();

                //if ($scope.ngSort && !$scope.$parent.isEditingAny()) {
                //    $scope.$parent.sort($scope.ngSort);

                //    $scope.$apply();
                //}
            });
        },
        template:
        '<ng-transclude></ng-transclude> \
         <span ng-show="ngSort && !$parent.isEditingAny()"> \
             <span class="sort-asc-icon" ng-class="getSortDirection(ngSort)"></span> \
             <span class="sort-desc-icon" ng-class="getSortDirection(ngSort)"></span> \
             <span ng-show="getSortColumnCount() > 1" class="sort-column-order"> \
                <span class="inner">{{getSortColumnOrder(ngSort)}}</span> \
             </span> \
         </span>'
    };
});

baseModule.directive('phoneCompanyTag', ['$rootScope', function ($rootScope) {
    return {
        restrict: 'A',
        scope: {
            ngModel: '='
        },
        link: function ($scope, $element, $attrs) {
            $scope.phoneCompany = 'Other!';
            $scope.$watch('ngModel', function (newVal, oldVal) {
                if (newVal && newVal.length >= 2) {
                    var phonePrefix = newVal.substring(0, 2);

                    for (var i in $rootScope.smartPrefixes) {
                        if (phonePrefix === $rootScope.smartPrefixes[i])
                        {
                            $scope.phoneCompany = 'Smart';
                            return;
                        }
                    }

                    for (var i in $rootScope.metFonePrefixes) {
                        if (phonePrefix === $rootScope.metFonePrefixes[i]) {
                            $scope.phoneCompany = 'metfone';
                            return;
                        }
                    }

                    for (var i in $rootScope.cellCardPrefixes) {
                        if (phonePrefix === $rootScope.cellCardPrefixes[i]) {
                            $scope.phoneCompany = 'cellcard';
                            return;
                        }
                    }

                    $scope.phoneCompany = 'Other!';
                }
            });
        },
        template:
        '<span ng-class="{smart: phoneCompany === \'Smart\', metFone: phoneCompany === \'metfone\', cellCard: phoneCompany === \'cellcard\'}" class="inner">{{phoneCompany}}<span>'
    };
}]);

baseModule.directive('dd', function () {
    return {
        restrict: 'E',
        link: function ($scope, $element, $attrs) {
            $scope.$watch(function () { // watch for any scope changes
                if (!$element.find('input, select, textarea').length && !$element.text().trim().length) {
                    $element.addClass('empty');
                }
                else {
                    $element.removeClass('empty');
                }
            });
        }
    }
});
