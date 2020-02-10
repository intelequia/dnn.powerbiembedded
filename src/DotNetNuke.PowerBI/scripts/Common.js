var Common = (function () {

    var call = function (httpMethod, action, service, params, success, fail, always) {
        var jqxhr = $.ajax({
            url: service.baseUrl + service.controller + "/" + action,
            beforeSend: service.framework.setModuleHeaders,
            type: httpMethod,
            async: true,
            data: httpMethod === "GET" ? params : JSON.stringify(params),
            dataType: httpMethod === "GET" ? "" : "json",
            contentType: httpMethod === "GET" ? "" : "application/json; charset=UTF-8"
        }).done(function (data) {
            if (typeof (success) === "function") {
                success(data);
            }
        }).fail(function (error, exception) {
            if (typeof (fail) === "function") {
                fail(error, exception);
            }
        }).always(function () {
            if (typeof (always) === "function") {
                always();
            }
        });
    };


    return {
        Call: call
    }
})();


