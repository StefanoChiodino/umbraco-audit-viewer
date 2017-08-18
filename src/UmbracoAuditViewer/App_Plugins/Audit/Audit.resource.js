angular.module("umbraco.resources")
    .factory("auditResource",
        function ($http) {
            return {
                getChanges: function (id, pageIndex, pageSize) {
                    return $http.get(
                        Umbraco.Sys.ServerVariables.AuditController.BaseUrl + "GetChanges"
                        + "?id=" + id);
                },
            }
        });