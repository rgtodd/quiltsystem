var QUILT;
if (!QUILT) {
    QUILT = {};
}

QUILT.kit = {
    btnColor: null,
    Specification_FabricStyle_Sku: null,
    isDirty: false,
    saveActionDone: "DONE",
    saveActionHeader: "HEADER",
    saveAction: null
};

function kit_initialize() {
    $("#btnDone").on("click", null, null, _kit_onDoneClick);
    $("#btnUndo").on("click", null, null, _kit_onUndoClick);
    $("#btnBackingColor").on("click", _kit_onBackingColorClick);
    $("#btnBindingColor").on("click", _kit_onBindingColorClick);
    $("#btnBorderColor").on("click", _kit_onBorderColorClick);
    $("#frmMain").on("change", ".kit_update", null, _kit_onFormChange);
    $("#frmMain").on("input propertychange", ".kit_defer_update", null, _kit_onFormUpdate);
    $('#Specification_BindingWidth').on("change", _kit_onBindingWidthChange);
    $('#Specification_BorderWidth').on("change", _kit_onBorderWidthChange);
    $('#Specification_HasBacking').on("change", _kit_onHasBackingChange);
    $('#Specification_Size').on("change", _kit_onSizeChange);

    header_registerConfirmCallback(_kit_headerConfirmCallback);
    header_registerSaveCallback(_kit_headerSaveCallback);

    _kit_updateSizeControls($('#Specification_Size').val());
    _kit_updateBorderControls($('#Specification_BorderWidth').val());
    _kit_updateBindingControls($('#Specification_BindingWidth').val());
    _kit_updateBackingControls($('#Specification_HasBacking').is(":checked"));

    _kit_setDirty(false);

    var jsonError = $("input[name=JsonError]").attr("value");
    error_refresh(jsonError);
}

// ******************************
//
// Private methods.
//
// ******************************

function _kit_gotoKitList() {
    var url = $("#btnDone").attr("data-url");
    window.location.href = url;
}

function _kit_setBusy( /* Boolean */ isBusy) {
    if (isBusy) {
        // $("#iconRefresh").addClass("design-container__toolbar-item-spin-icon--active");
    }
    else {
        //$("#iconRefresh").removeClass("design-container__toolbar-item-spin-icon--active");
    }
}

function _kit_setDirty( /* Boolean*/ isDirty) {
    QUILT.kit.isDirty = isDirty;
    if (isDirty) {
        $("#btnUndo").removeClass("header__button--disabled");
    }
    else {
        $("#btnUndo").addClass("header__button--disabled");
    }
}

function _kit_updateSizeControls(value) {
    if (value === "CUSTOM") {
        $(".kit-size-custom").show();
    }
    else {
        $(".kit-size-custom").hide();
    }
}

function _kit_updateBorderControls(value) {
    if (value === "CUSTOM") {
        $(".kit-customBorderWidth").show();
    }
    else {
        $(".kit-customBorderWidth").hide();
    }
    if (value !== "NONE") {
        $(".kit-hasBorder").show();
    }
    else {
        $(".kit-hasBorder").hide();
    }
}

function _kit_updateBindingControls(value) {
    if (value === "CUSTOM") {
        $(".kit-customBindingWidth").show();
    }
    else {
        $(".kit-customBindingWidth").hide();
    }
    if (value !== "NONE") {
        $(".kit-hasBinding").show();
    }
    else {
        $(".kit-hasBinding").hide();
    }
}

function _kit_updateBackingControls(value) {
    if (value) {
        $(".kit-hasBacking").show();
    }
    else {
        $(".kit-hasBacking").hide();
    }
}

// ******************************
//
// Ajax methods.
//
// ******************************

function _kit_refreshAsync() {

    _kit_setBusy(true);

    $("#frmMain").ajaxSubmit({
        data: {
            "action~refresh": "Refresh"
        },
        success: _kit_onRefreshComplete,
        error: _kit_onRefreshError
    });
}

function _kit_deferRefreshAsync() {
    // If it's the propertychange event, make sure it's the value that changed.
    if (window.event && event.type === "propertychange" && event.propertyName !== "value")
        return;

    // Clear any previously set timer before setting a fresh one
    window.clearTimeout($(this).data("timeout"));
    $(this).data("timeout", setTimeout(function () {
        _kit_refreshAsync();
    }, 1000));
}

function _kit_saveAsync(saveAction) {

    QUILT.kit.saveAction = saveAction;

    _kit_setBusy(true);

    $("#frmMain").ajaxSubmit({
        data: {
            "action~saveAsync": "Save Async"
        },
        success: _kit_onSaveComplete,
        error: _kit_onSaveError
    });
}

// ******************************
//
// Ajax event handlers.
//
// ******************************

function _kit_onRefreshComplete(response) {

    _kit_setBusy(false);

    var responseHtml = $($.parseHTML(response));

    var jsonError = responseHtml.find("input[name=JsonError]").attr("value");
    error_refresh(jsonError);

    var imgPreview = responseHtml.find("#imgPreview");
    $("#imgPreview").replaceWith(imgPreview);

    var divKitDetail = responseHtml.find("#divKitDetail");
    $("#divKitDetail").replaceWith(divKitDetail);
}

function _kit_onRefreshError() {

    _kit_setBusy(false);
}

function _kit_onSaveComplete() {

    _kit_setBusy(false);

    if (QUILT.kit.saveAction === QUILT.kit.saveActionDone) {
        _kit_gotoKitList();
    }
    else if (QUILT.kit.saveAction === QUILT.kit.saveActionHeader) {
        header_saveComplete();
    }
    else {
        _kit_setDirty(false);
    }
}

function _kit_onSaveError() {

    _kit_setBusy(false);
}

// ******************************
//
// UI event handlers.
//
// ******************************

// User has click the Save button.
//
function _kit_onDoneClick() { // jQuery click event handler
    if (QUILT.kit.isDirty) {
        _kit_saveAsync(QUILT.kit.saveActionDone);
    }
    else {
        _kit_gotoKitList();
    }
}

// User has click the Undo button.
//
function _kit_onUndoClick() { // jQuery click event handler
    var url = $("#btnUndo").attr("data-url");
    window.location.href = url;
}

function _kit_onBackingColorClick(event) {
    event.preventDefault();

    QUILT.kit.btnColor = "#btnBackingColor";
    QUILT.kit.Specification_FabricStyle_Sku = "#Specification_BackingFabricStyle_Sku";

    colorList_open(_kit_colorDialogCallback, null, $(QUILT.kit.Specification_FabricStyle_Sku).val());

    $("#dlgSelectColor").dialog("open");
}

function _kit_onBindingColorClick(event) {
    event.preventDefault();

    QUILT.kit.btnColor = "#btnBindingColor";
    QUILT.kit.Specification_FabricStyle_Sku = "#Specification_BindingFabricStyle_Sku";

    colorList_open(_kit_colorDialogCallback, null, $(QUILT.kit.Specification_FabricStyle_Sku).val());

    $("#dlgSelectColor").dialog("open");
}

function _kit_onBorderColorClick(event) {
    event.preventDefault();

    QUILT.kit.btnColor = "#btnBorderColor";
    QUILT.kit.Specification_FabricStyle_Sku = "#Specification_BorderFabricStyle_Sku";

    colorList_open(_kit_colorDialogCallback, null, $(QUILT.kit.Specification_FabricStyle_Sku).val());

    $("#dlgSelectColor").dialog("open");
}

function _kit_onFormChange() {
    _kit_setDirty(true);
    _kit_refreshAsync();
}

function _kit_onFormUpdate() {
    _kit_setDirty(true);
    _kit_deferRefreshAsync();
}

function _kit_onBindingWidthChange(event) {
    var value = $(this).val();
    _kit_updateBindingControls(value);
}

function _kit_onBorderWidthChange(event) {
    var value = $(this).val();
    _kit_updateBorderControls(value);
}

function _kit_onHasBackingChange(event) {
    var value = $(this).is(":checked");
    _kit_updateBackingControls(value);
}

function _kit_onSizeChange(event) {
    var value = $(this).val();
    _kit_updateSizeControls(value);
}

// ******************************
//
// Callback event handlers.
//
// ******************************

// Called by color dialog (color-list) when a new color has been selected.
//
function _kit_colorDialogCallback(callbackData, color, sku) {

    _kit_setDirty(true);

    $(QUILT.kit.btnColor).css("background-color", color);
    $(QUILT.kit.Specification_FabricStyle_Sku).val(sku);

    $("#dlgSelectColor").dialog("close");

    _kit_refreshAsync();
}

// Called when a navigation link is selected.  Indicates if a confirmation dialog should be shown before continuing.
//
function _kit_headerConfirmCallback() { // header_registerConfirmCallback callback
    return QUILT.kit.isDirty;
}

// Called during navigation when the user has indicated that their work should be saved.  When saving is complete, the
// header_saveComplete method should be called to allow navigation to proceed.
//
function _kit_headerSaveCallback() { // header_registerSaveCallback callback
    _kit_saveAsync(QUILT.kit.saveActionHeader);
}