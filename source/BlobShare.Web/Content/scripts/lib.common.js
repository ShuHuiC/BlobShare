// redirect function scope
Function.prototype.setScope = function (obj) {
    var method = this, temp = function () {
        return method.apply(obj, arguments);
    };

    return temp;
}

// log function (firefox, webkit, etc)
function log(o) {
    if (window.console && console.log) console.log(o);
}

// enum helpers
EnumHelper = {};
EnumHelper.getValue = function(enumeration, label) {
    for(var p in enumeration) {
        if(p == label) return enumeration[p];
    }

    return null;
};

EnumHelper.getLabel = function(enumeration, value) {
    for(var p in enumeration) {
        if (enumeration[p] == value) return p;
    }

    return null;
};

// read MS Ajax dates
function readMsAjaxDate(v) {
    try {
        return new Date(parseFloat(v.slice(6, 19)));
    } catch(err) {
        return null;
    }
}

// TODO: Generic error handling for XHR
function ajaxErrorHandling(xhr, err) {
    alert(xhr.status + ": " + xhr.statusText + "\n" + xhr.responseText);
}

// enums
BuildStatusEnum = {
    Unknown: 0,
    Queued: 1,
    Running: 2,
    Success: 3,
    Failed: 4,
    Canceled: 5
};