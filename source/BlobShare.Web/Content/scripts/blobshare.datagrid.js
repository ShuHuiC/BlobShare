var defaultPageSize = 10;
var currentPage = 0;

$(document).ready(function () {
    loadContent();
});

function loadContent() {
    $.ajax({
        contentType: "application/json",
        url: url,
        cache: false,
        dataType: 'json',
        success: function (data) {
            bindContent(data, pageIdentifier);
        },
        error: function (XMLHttpInvitation, textStatus, errorThrown) {
            alert("Error refreshing invitation content.\nPlease try again.\n" + textStatus);
        }
    });
}

function bindContent(data, pageIdentifier) { }

String.prototype.format = function () {
    var args = arguments;
    return this.replace(/{(\d+)}/g, function (match, number) {
        return typeof args[number] != 'undefined'
      ? args[number]
      : '{' + number + '}'
    ;
    });
};

function getFirstProperty(obj) {
    for (var property in obj) if (obj.hasOwnProperty(property)) return property;
}

/* grid functions */
function createDataGrid(container, emptyResultsContainer, pagingContainer, searchBox, schema, data, pageSize, pageIdentifier, callback) {
    var grid = new DataGrid(container, schema, [], true);
    grid.tableId = pageIdentifier;

    var paging = new Paginator(pagingContainer);
    var sortProperty, sortType, sortDirection, sortColIx;

    var pageAndSortData = function (data, pageSize, currentPage) {
        var totalPages = Math.ceil(data.length / pageSize);

        if (!sortProperty) { sortProperty = getFirstProperty(data[0]); }
        if (!sortColIx) { sortColIx = 0; }
        if (!sortDirection) { sortDirection = 1; }

        data = sortData(data, sortProperty, sortType, sortDirection);

        var startIx = (currentPage - 1) * pageSize;
        var endIx = startIx + pageSize;
        pageData = data.slice(startIx, endIx);

        grid.model = pageData;
        grid.refresh();
        grid.setSort(sortColIx, sortDirection);

        paging.setPages(totalPages, currentPage);

        if (data.length == 0) {
            $(emptyResultsContainer).show();
            $(grid.container).hide();
        } else {
            $(emptyResultsContainer).hide();
            $(grid.container).show();
        }
    };

    grid.events.bind("sort_request", function (e, args) {
        var propType = schema[args.colIndex].type;
        var propName = schema[args.colIndex].propertyName;
        sortProperty = propName;
        sortType = propType;
        sortDirection = args.sortDirection;
        sortColIx = args.colIndex;

        pageAndSortData(filterData(data, schema, searchBox.value), pageSize, 1);
    });

    paging.events.bind("change", function (e, args) {
        pageAndSortData(filterData(data, schema, searchBox.value), pageSize, args.page);
    });

    var filterGrid = function () {
        pageAndSortData(filterData(data, schema, searchBox.value), pageSize, 1);
    }

    var searchTimeout;

    $(searchBox).bind("keyup", function (e) {
        if (searchTimeout) clearTimeout(searchTimeout);
        searchTimeout = setTimeout(filterGrid, 200);
    });

    // first time
    grid.init();
    pageAndSortData(filterData(data, schema, searchBox.value), pageSize, 1);
    if (callback) callback();
    return grid;
}

function filterData(data, schema, filterString) {
    var filterString = filterString.toLowerCase();
    if (filterString.length == 0) {
        return data;
    }

    var filtered = [];
    for (var i = 0; i < data.length; i++) {
        for (var p = 0; p < schema.length; p++) {
            var prop = schema[p].propertyName;
            if (prop) {
                var propValue = data[i][prop];
                if (propValue != null && propValue.toString().toLowerCase().indexOf(filterString) > -1) {
                    filtered.push(data[i]);
                    break;
                }
            }
        }
    }

    return filtered;
}

function sortData(data, propName, propType, sortDirection) {
    var sortFunction;
    if (propType == "date" || propType == "number") {
        if (sortDirection == 1) {
            sortFunction = function (a, b) { return a[propName] - b[propName]; }
        } else {
            sortFunction = function (a, b) { return b[propName] - a[propName]; }
        }
    } else {
        if (sortDirection == 1) {
            sortFunction = function (a, b) {
                if (a[propName] < b[propName]) { return -1; }
                if (a[propName] > b[propName]) { return 1; }
                return 0;
            }
        } else {
            sortFunction = function (a, b) {
                if (b[propName] < a[propName]) { return -1; }
                if (b[propName] > a[propName]) { return 1; }
                return 0;
            }
        }
    }

    return data.sort(sortFunction)
}