angular.module('umbraco')
    .controller('auditController',
        function ($scope,
            $routeParams,
            auditResource,
            notificationsService,
            dialogService) {

            $scope.nodeId = $routeParams.id;
            $scope.loading = true;
            $scope.error = false;
            $scope.sortType = "ChangeDateTime";
            $scope.hideZeroChanges = false;
            $scope.sortReverse = true;

            $scope.fetchData = function() {
                auditResource.getChanges(
                        $scope.nodeId)
                    .error(function(response) {
                        $scope.error = true;
                        $scope.loading = false;
                    })
                    .then(function(response) {
                        $scope.changes = response.data;
                        $scope.loading = false;
                        $scope.error = false;
                    });
            };

            $scope.order = function(sortType) {
                if ($scope.sortType != sortType) {
                    $scope.sortType = sortType;
                    $scope.sortReverse = false;
                } else {
                    $scope.sortReverse = !$scope.sortReverse;
                }
            };
            
            $scope.fetchData();
        });