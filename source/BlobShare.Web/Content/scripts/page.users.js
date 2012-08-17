/// <reference path="jquery-1.4.4.min.js" />
/// <reference path="BlobShare.datagrid.js" />

function bindContent(data, pageIdentifier) {
    var resultsContainer = $("#" + pageIdentifier + "Results");
    var searchBox = $("#" + pageIdentifier + "Search");
    var emptyResult = $("#" + pageIdentifier + "Empty");
    var resultsPager = $("#" + pageIdentifier + "Pager");

    var schema = [
        {
            className: "name-column",
            propertyName: "Name",
            headerText: "Name",
            type: "custom",
            customContent: function (dataItem) {
                return '<a href="' + detailsUrl + '/' + dataItem.UserId + '" title="' + dataItem.Name + ' Details"><span>' + dataItem.Name + '</span></a>';
            },
            sortable: true
        },
        {
            className: "email-column",
            propertyName: "Email",
            headerText: "Email",
            type: "string",
            sortable: true
        },
        {
            className: "status-column",
            propertyName: "Status",
            headerText: "Status",
            type: "string",
            sortable: true
        },
        {
            className: "roles-column",
            propertyName: "Roles",
            headerText: "Roles",
            type: "custom",
            customContent: function (dataItem) {
                return dataItem.Roles.join(",");
            },
            sortable: true
        },
    ];

        createDataGrid(resultsContainer[0], emptyResult[0], resultsPager[0], searchBox.find("input")[0], schema, data, defaultPageSize, pageIdentifier, function () { searchBox.show(); });
}