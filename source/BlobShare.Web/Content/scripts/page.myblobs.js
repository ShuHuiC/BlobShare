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
                if (dataItem.IsBlob) {
                    return '<a href="' + viewBlobUrl + '/' + dataItem.BlobId + '" title="View Blob"><span>' + dataItem.Name + '</span></a>';
                } else {
                    return '<a href="' + viewBlobSetUrl + '/' + dataItem.BlobSetId + '" title="View Blob Set"><span>' + dataItem.Name + '</span></a>';
                }
            },
            sortable: true
        },
        {
            className: "description-column",
            propertyName: "Description",
            headerText: "Description",
            type: "string",
            sortable: true
        },
        {
            className: "actions-column",
            propertyName: "BlobId",
            headerText: "",
            type: "custom",
            customContent: function (dataItem) {
                if (dataItem.IsBlob) {
                    return '<a href="' + downloadBlobUrl + '/' + dataItem.BlobId + '" title="Download Blob"><span>Download</span></a>';
                }
                else {
                    return '';
                }
            },
            sortable: false
        }
    ];

        createDataGrid(resultsContainer[0], emptyResult[0], resultsPager[0], searchBox.find("input")[0], schema, data, defaultPageSize, pageIdentifier, function () { searchBox.show(); });
}