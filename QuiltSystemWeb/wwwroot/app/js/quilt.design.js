var QUILT;
if (!QUILT) {
    QUILT = {};
}

QUILT.design = {
    activeDesign: null, // XDesign_Design
    designInfo: null, // XDesign_DesignInfo
    bodyHeight: null,
    bodyWidth: null,
    canvasPadding: 20,
    canvasSize: 50,
    previewSize: 10000,
    isDirty: false,
    saveActionDone: "DONE",
    saveActionHeader: "HEADER",
    saveActionKit: "KIT",
    saveAction: null
};

function design_initialize() {
    $(window).resize(_design_onResize);
    _design_onResize();

    select_addCallback("layout", _design_onLayoutSelected);
    select_addCallback("block", _design_onBlockSelected);
    select_addCallback("swatch", _design_onSwatchSelected);

    header_registerConfirmCallback(_design_headerConfirmCallback);
    header_registerSaveCallback(_design_headerSaveCallback);

    $("#lstLayoutRows").on("change", null, null, _design_onLayoutRowsChange);
    $("#lstLayoutColumns").on("change", null, null, _design_onLayoutColumnsChange);
    $("#btnDone").on("click", null, null, _design_onDoneClick);
    $("#btnKit").on("click", null, null, _design_onKitClick);
    $("#btnUndo").on("click", null, null, _design_onUndoClick);
}

function design_loadDesign(designId) {
    _design_loadDesignAsync(designId);
}

// ******************************
//
// Private methods.
//
// ******************************

// Create XDesign_DesignBlock object from a XDesign_Block object.
//
function _design_createDesignBlock( /* XDesign_Block */ block) /* XDesign_DesignBlock */ {
    var designBlock = {
        className: "XDesign_DesignBlock",
        blockCategory : block.blockCategory,
        blockName: block.blockName,
        fabricStyles: []
    };

    var idx;
    for (idx = 0; idx < block.fabricStyles.length; ++idx) {
        var fabricStyle = block.fabricStyles[idx];
        designBlock.fabricStyles[idx] = {
            className: "XDesign_FabricStyle",
            color: {
                webColor: fabricStyle.color.webColor
            }
        };
    }

    return designBlock;
}

// Create XDesign_DesignLayout object from a XDesign_Layout object.
//
function _design_createDesignLayout( /* XDesign_Layout */ layout) /* XDesign_DesignLayout */ {
    var designLayout = {
        className: "XDesign_DesignLayout",
        layoutName: layout.layoutName,
        fabricStyles: [],
        rowCount: layout.rowCount,
        columnCount: layout.columnCount,
        blockCount: layout.blockCount
    };

    var idx;
    for (idx = 0; idx < layout.fabricStyles.length; ++idx) {
        var fabricStyle = layout.fabricStyles[idx];
        designLayout.fabricStyles[idx] = {
            className: "XDesign_FabricStyle",
            color: {
                webColor: fabricStyle.color.webColor
            }
        };
    }

    return designLayout;
}

// Adds the specified block to the toolbar.
//
function _design_createToolbarBlock(index, /* XDesign_Block */ block) {
    $("#divProjectBlocks").append(
        "<div class='design-container__toolbar-item-block' data-block-index='" + index + "'>" +
        "<canvas class='design-container__toolbar-item-block-canvas select__item' data-select-name='block' data-select-id='" + index + "' width='" + QUILT.design.canvasSize + "' height='" + QUILT.design.canvasSize + "'></canvas>" +
        "<div class='design-container__toolbar-item-block-palette'></div></div>");

    var canvas = $(".design-container__toolbar-item-block[data-block-index='" + index + "'] .design-container__toolbar-item-block-canvas").get(0);
    var context = canvas.getContext("2d");

    if (block !== null) {
        var designBlock = _design_createDesignBlock(block);
        draw_shapeArrayImmediate(context, block.preview.shapes, (QUILT.design.canvasSize - 1) / QUILT.design.previewSize);

        var divColors = $(".design-container__toolbar-item-block[data-block-index='" + index + "'] .design-container__toolbar-item-block-palette");
        divColors.html("");
        var idx = 0;
        for (idx = 0; idx < block.fabricStyles.length; ++idx) {
            var webColor = designBlock.fabricStyles[idx].color.webColor;
            divColors.append(
                "<div class='design-container__toolbar-item-block-swatch select__item' data-select-name='swatch' data-select-id='" + index + "-" + idx + "'  data-color='" + webColor + "' data-swatch-index='" + idx + "' style='background: " + webColor + ";'></div>");
        }
    }
    else {
        context.font = '30px Courier New';
        context.textBaseline = 'middle';
        context.textAlign = 'center';
        context.rect(0, 0, QUILT.design.canvasSize, QUILT.design.canvasSize);
        context.stroke();
        context.fillText('+', QUILT.design.canvasSize / 2, QUILT.design.canvasSize / 2);
    }
}

// Adds the specified layout to the toolbar.
//
function _design_createToolbarLayout(/* Design_LayoutInfoData */ layoutInfo) {
    var idProjectLayoutDiv = "divProjectLayout";
    var idProjectLayoutCanvas = "canvasProjectLayout";

    var maxLayoutSize = layoutInfo.preview.width;
    if (maxLayoutSize < layoutInfo.preview.height) {
        maxLayoutSize = layoutInfo.preview.height;
    }

    var canvasWidth = Math.round(QUILT.design.canvasSize * layoutInfo.preview.width / maxLayoutSize);
    var canvasHeight = Math.round(QUILT.design.canvasSize * layoutInfo.preview.height / maxLayoutSize);

    $("#divProjectLayouts").append(
        "<div class='design-container__toolbar-item-layout'>" +
        "<canvas id='" + idProjectLayoutCanvas + "' class='select__item' data-select-name='layout' width='" + canvasWidth + "' height='" + canvasHeight + "'></canvas>");

    if (layoutInfo !== null) {
        var canvas = document.getElementById(idProjectLayoutCanvas);
        var context = canvas.getContext("2d");
        draw_layoutSiteArrayImmediate(context, layoutInfo.preview.layoutSites, (QUILT.design.canvasSize - 1) / QUILT.design.previewSize);
    }
}

// Render design preview.
//
function _design_drawPreview() {
    var designInfo = QUILT.design.designInfo;
    if (designInfo === null) {
        return;
    }

    var canvas = document.getElementsByClassName("design_container__preview-canvas").item(0);
    var context = canvas.getContext("2d");

    var previewWidth = QUILT.design.bodyWidth * 0.5 - QUILT.design.canvasPadding * 2;
    var previewHeight = QUILT.design.bodyHeight - QUILT.design.canvasPadding * 2;

    // Determine optimal sizing for preview.
    //
    var blockWidth = previewWidth / designInfo.layout.columnCount;
    var blockHeight = previewHeight / designInfo.layout.rowCount;

    var blockSize = blockWidth;
    if (blockSize > blockHeight) {
        blockSize = blockHeight;
    }

    var quiltWidth = blockSize * designInfo.layout.columnCount;
    var quiltHeight = blockSize * designInfo.layout.rowCount;

    var quiltSize = quiltWidth;
    if (quiltSize < quiltHeight) {
        quiltSize = quiltHeight;
    }

    $(".design_container__preview-canvas").css({
        width: quiltWidth + "px",
        height: quiltHeight + "px"
    }).attr("width", quiltWidth).attr("height", quiltHeight);

    context.clearRect(0, 0, quiltWidth, quiltHeight);
    draw_shapeArrayImmediate(context, designInfo.preview.shapes, (quiltSize - 1) / QUILT.design.previewSize);
}

// Navigate to the kit editor.
//
function _design_gotoCreateKit() {
    var url = $("#btnKit").attr("data-url").replace("GUID", QUILT.design.activeDesign.designId);
    window.location.href = url;
}

function _design_gotoDesignList() {
    var url = $("#btnDone").attr("data-url");
    window.location.href = url;
}

function _design_setBusy( /* Boolean */ isBusy) {
    if (isBusy) {
        $("#iconRefresh").addClass("design-container__toolbar-item-spin-icon--active");
    }
    else {
        $("#iconRefresh").removeClass("design-container__toolbar-item-spin-icon--active");
    }
}

// Indicate if user has modified the current design.
//
function _design_setDirty( /* Boolean*/ isDirty) {
    QUILT.design.isDirty = isDirty;
    if (isDirty) {
        $("#btnUndo").removeClass("header__button--disabled");
    }
    else {
        $("#btnUndo").addClass("header__button--disabled");
    }
}

// ******************************
//
// Ajax methods.
//
// ******************************

// Load design from server.
//
function _design_loadDesignAsync(designId) {

    _design_setBusy(true);

    $.ajax({
        url: "/api/designs/" + designId
    }).done(_design_onLoadDesignComplete).fail(_design_onLoadDesignError);
}

// Send current design to server and refresh design container.
//
function _design_refreshAsync() {

    _design_setBusy(true);

    $.ajax({
        url: "/api/preview?designSize=" + QUILT.design.previewSize + "&layoutSize=" + QUILT.design.previewSize + "&blockSize=" + QUILT.design.previewSize,
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify(QUILT.design.activeDesign),
        dataType: "json"
    }).done(_design_onRefreshComplete).fail(_design_onRefreshError);
}

// Save current design to server.
//
function _design_saveAsync(saveAction) {

    QUILT.design.saveAction = saveAction;

    _design_setBusy(true);

    $.ajax({
        url: "/api/save",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify(QUILT.design.activeDesign),
        dataType: "json"
    }).done(_design_onSaveComplete).fail(_design_onSaveError);
}

// ******************************
//
// Ajax event handlers.
//
// ******************************

function _design_onLoadDesignComplete( /* XDesign_Design */ response) { // design_loadDesign done handler

    _design_setBusy(false);

    QUILT.design.activeDesign = response;

    _design_setDirty(false);

    var rowCount = QUILT.design.activeDesign.layout.rowCount;
    var columnCount = QUILT.design.activeDesign.layout.columnCount;

    $("#lstLayoutRows option[value='" + rowCount + "']").attr("selected", "selected");
    $("#lstLayoutColumns option[value='" + columnCount + "']").attr("selected", "selected");

    layoutList_refreshListAsync(rowCount, columnCount);
    blockList_refreshListAsync();
    _design_refreshAsync();
}

function _design_onLoadDesignError() {

    _design_setBusy(false);

    $("#dlgError").dialog("open");
}

function _design_onRefreshComplete( /* XDesign_DesignInfo */ response) { // design_refresh done handler
    var idx,
        blockInfo;

    _design_setBusy(false);

    QUILT.design.designInfo = response;

    _design_drawPreview();

    var designInfo = QUILT.design.designInfo;

    $(".design-container__toolbar-item-layout").remove();
    $(".design-container__toolbar-item-block").remove();

    if (designInfo.layout !== null) {
        _design_createToolbarLayout(designInfo.layout);

        for (idx = 0; idx < designInfo.layout.blockCount; ++idx) {
            if (idx < designInfo.blocks.length) {
                blockInfo = designInfo.blocks[idx];
            }
            else {
                blockInfo = null;
            }
            _design_createToolbarBlock(idx, blockInfo);
        }
    }

    select_refresh();

    _design_onResize();
}

function _design_onRefreshError() {

    _design_setBusy(false);

    $("#dlgError").dialog("open");
}

function _design_onSaveComplete( /* Guid */ response) { // design_save done handler

    _design_setBusy(false);

    QUILT.design.activeDesign.designId = response;

    if (QUILT.design.saveAction === QUILT.design.saveActionDone) {
        _design_gotoDesignList();
    }
    else if (QUILT.design.saveAction === QUILT.design.saveActionHeader) {
        header_saveComplete();
    }
    else if (QUILT.design.saveAction === QUILT.design.saveActionKit) {
        _design_gotoCreateKit();
    }
    else {
        _design_setDirty(false);
    }
}

function _design_onSaveError() {

    _design_setBusy(false);

    $("#dlgError").dialog("open");
}

// ******************************
//
// UI event handlers.
//
// ******************************

// User has selected a block in the toolbar.  Open the block tooltray and allow the user to select a new block.
//
function _design_onBlockSelected(element) { // select_addCallback callback

    if (element === null) {
        tooltray_hide();
        $(".design_container__preview").removeClass("design_container__preview--tooltray-active");
    }
    else {
        var blockName = null;
        var index = $(element).attr("data-select-id");
        if (index < QUILT.design.activeDesign.blocks.length) {
            var currentBlock = QUILT.design.activeDesign.blocks[index];
            if (currentBlock !== null) {
                blockName = currentBlock.blockName;
            }
        }

        blockList_open(_design_onBlockListSelection, $(element), blockName);
        $(".design_container__preview").addClass("design_container__preview--tooltray-active");
        tooltray_show("divBlocks");
    }
}

// User has selected a new column count in the toolbar.  Update and refresh design.  Update layout list.
//
function _design_onLayoutColumnsChange() { // jQuery change event handler

    _design_setDirty(true);

    QUILT.design.activeDesign.layout.columnCount = new Number($(this).val());

    var rowCount = QUILT.design.activeDesign.layout.rowCount;
    var columnCount = QUILT.design.activeDesign.layout.columnCount;
    layoutList_refreshListAsync(rowCount, columnCount);

    _design_refreshAsync();
}

// User has selected a new row count in the toolbar.  Update and refresh design.  Update layout list.
//
function _design_onLayoutRowsChange() { // jQuery change event handler

    _design_setDirty(true);

    QUILT.design.activeDesign.layout.rowCount = new Number($(this).val());

    var rowCount = QUILT.design.activeDesign.layout.rowCount;
    var columnCount = QUILT.design.activeDesign.layout.columnCount;
    layoutList_refreshListAsync(rowCount, columnCount);

    _design_refreshAsync();
}

// User has selected a layout in the toolbar.  Open the layout tooltray and allow the user to select a new layout.
//
function _design_onLayoutSelected(element) { // select_addCallback callback

    if (element === null) {
        tooltray_hide();
        $(".design_container__preview").removeClass("design_container__preview--tooltray-active");
    }
    else {
        var layoutName = QUILT.design.activeDesign.layout.layoutName;

        layoutList_open(_design_onLayoutListSelection, $(element), layoutName);
        $(".design_container__preview").addClass("design_container__preview--tooltray-active");
        tooltray_show("divLayouts");
    }
}

// User has resized the window.
//
function _design_onResize() { // jQuery resize event handler

    var previewTop = $(".design_container__preview").position().top;

    var footerHeight = $(".fixed-bottom").outerHeight();

    var height =
        $(window).height() - (
            previewTop +
            footerHeight);

    QUILT.design.bodyHeight = height;
    QUILT.design.bodyWidth = $(".design-container").width();

    $(".design-container__tooltrays").css({
        height: height + "px"
    });

    _design_drawPreview();
}

// User has click the Save button.
//
function _design_onDoneClick() { // jQuery click event handler

    if (QUILT.design.isDirty) {
        _design_saveAsync(QUILT.design.saveActionDone);
    }
    else {
        _design_gotoDesignList();
    }
}

// User has click the Undo button.
//
function _design_onUndoClick() { // jQuery click event handler

    _design_loadDesignAsync(QUILT.design.activeDesign.designId);
}

// User has click the Kit button.
//
function _design_onKitClick() { // jQuery click event handler

    if (QUILT.design.isDirty) {
        _design_saveAsync(QUILT.design.saveActionKit);
    }
    else {
        _design_gotoCreateKit();
    }
}

// User has selected a swatch in the toolbar.  Open the swatch tooltray and allow the user to select a new swatch.
//
function _design_onSwatchSelected(element) { // select_addCallback callback

    if (element === null) {
        tooltray_hide();
        $(".design_container__preview").removeClass("design_container__preview--tooltray-active");
    }
    else {
        var w = $(element);
        var blockIndex = Number.parseInt(w.parent().parent().attr("data-block-index"));
        var swatchIndex = Number.parseInt(w.attr("data-swatch-index"));
        var fabricStyle = QUILT.design.activeDesign.blocks[blockIndex].fabricStyles[swatchIndex];

        colorList_open(_design_onSwatchListSelection, $(element), fabricStyle.sku);
        $(".design_container__preview").addClass("design_container__preview--tooltray-active");
        tooltray_show("divColors");
    }
}

// ******************************
//
// Callback event handlers.
//
// ******************************

// Called when a navigation link is selected.  Indicates if a confirmation dialog should be shown before continuing.
//
function _design_headerConfirmCallback() { // header_registerConfirmCallback callback

    return QUILT.design.isDirty;
}

// Called during navigation when the user has indicated that their work should be saved.  When saving is complete, the
// header_saveComplete method should be called to allow navigation to proceed.
//
function _design_headerSaveCallback() { // header_registerSaveCallback callback

    _design_saveAsync(QUILT.design.saveActionHeader);
}

// User has selected a new block.  Update and refresh design.
//
function _design_onBlockListSelection(selection, block /* XDesign_Block */) { // blockList_open callback

    _design_setDirty(true);

    var designBlock = _design_createDesignBlock(block);

    var index = $(selection).attr("data-select-id");

    if (index < QUILT.design.activeDesign.blocks.length) {
        var currentBlock = QUILT.design.activeDesign.blocks[index];
        if (currentBlock !== null) {
            for (var idx = 0; idx < currentBlock.fabricStyles.length && idx < designBlock.fabricStyles.length; ++idx) {
                designBlock.fabricStyles[idx] = currentBlock.fabricStyles[idx];
            }
        }
    }

    QUILT.design.activeDesign.blocks[index] = designBlock;

    _design_refreshAsync();
}

// User has selected a new layout.  Update and refresh design.
//
function _design_onLayoutListSelection(selection, layout /* XDesign_Layout */) { // layoutList_open callback

    _design_setDirty(true);

    var designLayout = _design_createDesignLayout(layout);

    QUILT.design.activeDesign.layout = designLayout;

    _design_refreshAsync();
}

// User has selected a new swatch.  Update and refresh design.
//
function _design_onSwatchListSelection(selection, webColor, sku) { // colorList_open callback

    _design_setDirty(true);

    selection.css("background", webColor);

    var blockIndex = Number.parseInt(selection.parent().parent().attr("data-block-index"));
    var swatchIndex = Number.parseInt(selection.attr("data-swatch-index"));
    var fabricStyle = QUILT.design.activeDesign.blocks[blockIndex].fabricStyles[swatchIndex];

    fabricStyle.sku = sku;
    fabricStyle.color.webColor = webColor;

    _design_refreshAsync();
}