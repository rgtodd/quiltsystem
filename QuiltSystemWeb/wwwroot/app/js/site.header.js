var SITE;
if (!SITE) {
    SITE = {};
}

SITE.header = {
    confirmDialog: null,
    confirmNavigation: true,
    confirmCallback: null,
    saveCallback: null,
    target: null
};

function header_initialize() {

    SITE.header.confirmDialog = $("#dlgConfirmNavigation");

    $(".header__link").on("click", _header_onMenuClick);

    SITE.header.confirmDialog.dialog({
        autoOpen: false,
        closeText: "closeText",
        modal: true,
        buttons: [
            { text: "Save Changes", click: _header_onSave },
            { text: "Continue Without Saving", click: _header_onContinue },
            { text: "Cancel", click: _header_onCancel }
        ]
    });
}

function header_saveComplete() {
    window.location.href = SITE.header.target;
}

function header_saveCancelled() {
    SITE.header.confirmDialog.dialog("close");
}

function _header_onSave() {
    if (SITE.header.saveCallback !== null) {
        try {
            SITE.header.saveCallback();
        }
        catch (e) {
            SITE.header.confirmDialog.dialog("close");
            return;
        }
    }
    //window.location.href = SITE.header.target;
}

function _header_onCancel() {
    SITE.header.confirmDialog.dialog("close");
}

function _header_onContinue() {
    window.location.href = SITE.header.target;
}

function header_registerConfirmCallback(confirmCallback) {
    SITE.header.confirmCallback = confirmCallback;
}

function header_registerSaveCallback(saveCallback) {
    SITE.header.saveCallback = saveCallback;
}

function header_enableMenuConfirmation() {
    SITE.header.confirmNavigation = true;
}

function header_disableMenuConfirmation() {
    SITE.header.confirmNavigation = false;
}

function _header_onMenuClick(event) {
    if (SITE.header.confirmCallback !== null) {
        var confirm = SITE.header.confirmCallback();
        if (confirm) {
            SITE.header.target = $(this).attr("href");
            event.preventDefault();
            SITE.header.confirmDialog.dialog("open");
        }
    }
}