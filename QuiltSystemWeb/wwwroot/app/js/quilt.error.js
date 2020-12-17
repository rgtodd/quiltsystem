var QUILT;
if (!QUILT) {
    QUILT = {};
}

QUILT.error = {
};

function error_refresh(jsonError) {
    error_removeAllPageErrors();

    if (jsonError === "") {
        error_removeAllFieldErrors();
    } else {
        var error = JSON.parse(jsonError);
        if (error !== null) {
            var idx;
            var fieldError;

            if (error.pageErrors !== null) {
                for (idx = 0; idx < error.pageErrors.length; ++idx) {
                    var pageError = error.pageErrors[idx];
                    $("h2:first").after("<div class='alert alert-danger page-error' role='alert'>" + pageError.message + "</div>");
                }
            }

            var fieldErrors = error_getErrorFields();
            for (idx = 0; idx < fieldErrors.length; ++idx) {
                fieldError = fieldErrors[idx];
                var inError = false;
                for (var idx2 = 0; idx2 < error.fieldErrors.length; ++idx2) {
                    if (fieldError === error.fieldErrors[idx2].fieldName) {
                        inError = true;
                        break;
                    }
                }

                if (!inError) {
                    error_removeFieldError(fieldError);
                }
            }

            if (error.fieldErrors !== null) {
                for (idx = 0; idx < error.fieldErrors.length; ++idx) {
                    fieldError = error.fieldErrors[idx];
                    error_addFieldError(fieldError.fieldName, fieldError.message);
                }
            }
        }
    }
}

function error_inError(fieldName) {
    var field = $("div[data-field='" + fieldName + "']");
    return field.hasClass("has-error");
}

function error_addFieldError(fieldName, message) {
    var field = $("div[data-field='" + fieldName + "']");
    if (field.hasClass("has-error")) {
        field.children("div").children("span.help-block").text(message);
    }
    else {
        field.addClass("has-error has-feedback");
        field.children("div").append("<span class='glyphicon glyphicon-remove form-control-feedback' aria-hidden='true'></span>"
                + "<span class='sr-only'>(error)</span>"
                + "<span class='help-block'>" + message + "</span>");
    }
}

function error_getErrorFields() {
    var errorFields = [];

    var fields = $("div.has-error").each(function () {
        errorFields.push($(this).attr("data-field"));
    });

    return errorFields;
}

function error_removeFieldError(fieldName) {
    var field = $("div[data-field='" + fieldName + "']");
    field.removeClass("has-error has-feedback");
    field.children("div").children("span").remove();
}

function error_removeAllFieldErrors() {
    var field = $("div[data-field]");
    field.removeClass("has-error has-feedback");
    field.children("div").children("span").remove();
}

function error_removeAllPageErrors() {
    $("div.page-error").remove();
}