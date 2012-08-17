/// <reference path="jquery-1.4.4.min.js" />
/// <reference path="BlobShare.datagrid.js" />

function bindContent(data, pageIdentifier) {
    var resultsContainer = $("#" + pageIdentifier + "Results");
    var searchBox = $("#" + pageIdentifier + "Search");
    var emptyResult = $("#" + pageIdentifier + "Empty");
    var resultsPager = $("#" + pageIdentifier + "Pager");

    // normalize data
    for (var i = 0; i < data.length; i++) {
        var row = data[i];
        if (row.Created) row.Created = new Date(row.Created);
        if (row.Expiration) row.Expiration = new Date(row.Expiration);
    }

    var schema = [
        {
            propertyName: "Created",
            headerText: "Created",
            type: "date",
            sortable: true
        },
        {
            propertyName: "UserName",
            headerText: "User Name",
            type: "custom",
            customContent: function (dataItem) {
                return '<a href="' + detailsUrl + '/' + dataItem.UserId + '" title="' + dataItem.UserName + ' Details"><span>' + dataItem.UserName + '</span></a>';
            },
            sortable: true
        },
        {
            propertyName: "Email",
            headerText: "Email",
            type: "string",
            sortable: true
        },
        {
            propertyName: "Activated",
            headerText: "Activated",
            type: "string",
            sortable: true
        },
        {
            propertyName: "Expiration",
            headerText: "Expiration",
            type: "date",
            sortable: true
        },
    ];

        createDataGrid(resultsContainer[0], emptyResult[0], resultsPager[0], searchBox.find("input")[0], schema, data, defaultPageSize, pageIdentifier, function () { searchBox.show(); });
}