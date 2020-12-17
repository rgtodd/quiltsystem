var QUILT;
if (!QUILT) {
    QUILT = {};
}

QUILT.blockList = {
    blocks: null, // XDesign_Block[]
    selectedBlockName: null,
    selectedDiv: null,
    callback: null,
    callbackData: null,
    canvasSize: 50,
    previewSize: 10000
};

function blockList_initialize() {
    $("#divBlocks").on("mouseenter", ".block-list__item", null, _blockList_onMouseEnter);
    $("#divBlocks").on("mouseleave", ".block-list__item", null, _blockList_onMouseLeave);
    $("#divBlocks").on("click", ".block-list__item", null, _blockList_onClick);
}

function blockList_open(callback, callbackData, blockName) {
    QUILT.blockList.callback = callback;
    QUILT.blockList.callbackData = callbackData;

    _blockList_setSelection(blockName);
}

function blockList_refreshListAsync() {
    $.ajax({
        url: "/api/blocks?size=" + QUILT.blockList.previewSize,
        dataType: "json"
    }).done(_blockList_onRefreshListComplete);
}

// ******************************
//
// Ajax event handlers.
//
// ******************************

function _blockList_onRefreshListComplete( /* XDesign_Block[] */ response) {
    QUILT.blockList.blocks = response;
    _blockList_populateList();
    _blockList_setSelection(QUILT.blockList.selectedBlockName);
}

// ******************************
//
// UI event handlers.
//
// ******************************

function _blockList_onClick(event) {

    var blockName = $(event.currentTarget).attr("data-block-name");

    _blockList_setSelection(blockName);

    if (QUILT.blockList.callback !== null) {
        var blockInfo = _blockList_lookupInfo(blockName);
        if (blockInfo !== null) {
            QUILT.blockList.callback(QUILT.blockList.callbackData, blockInfo);
        }
    }
}

function _blockList_onMouseEnter(event) {
    $(this).addClass("tooltray__item--hover");
}

function _blockList_onMouseLeave(event) {
    $(this).removeClass("tooltray__item--hover");
}

// ******************************
//
// Private methods.
//
// ******************************

function _blockList_lookupInfo(blockName) {
    for (var idx = 0; idx < QUILT.blockList.blocks.length; ++idx) {
        var blockInfo = QUILT.blockList.blocks[idx];
        if (blockInfo.blockName === blockName) {
            return blockInfo;
        }
    }

    return null;
}

function _blockList_setSelection(blockName) {

    if (QUILT.blockList.selectedDiv !== null) {
        QUILT.blockList.selectedDiv.removeClass("tooltray__item--selected");
    }

    if (blockName === null) {
        QUILT.blockList.selectedBlockName = null;
        QUILT.blockList.selectedDiv = null;
    }
    else {
        var div = $(".block-list__item[data-block-name='" + blockName + "']");
        QUILT.blockList.selectedBlockName = blockName;
        QUILT.blockList.selectedDiv = div;
        QUILT.blockList.selectedDiv.addClass("tooltray__item--selected");
    }
}

function _blockList_populateList() {
    $("#divBlocks").empty();

    var currentGroup = "";
    var currentSection = null;
    for (var idx = 0; idx < QUILT.blockList.blocks.length; ++idx) {
        var blockInfo = QUILT.blockList.blocks[idx];

        var divId = "blockDiv_" + blockInfo.id;
        var canvasId = "blockCanvas_" + blockInfo.id;

        if (blockInfo.blockGroup !== currentGroup) {
            currentGroup = blockInfo.blockGroup;
            $("#divBlocks").append("<div class='tooltray__heading'>" + currentGroup + "</div>");

            currentSection = $("<div class='tooltray__section'></div>");
            $("#divBlocks").append(currentSection);
        }

        currentSection.append(
            "<div id='" + divId + "' data-block-name='" + blockInfo.blockName + "' class='block-list__item tooltray__item'>" +
            "<canvas id='" + canvasId + "' class='tooltray__item-canvas' width='" + QUILT.blockList.canvasSize + "' height='" + QUILT.blockList.canvasSize + "'></canvas></div>");

        var canvas = document.getElementById(canvasId);
        var context = canvas.getContext("2d");

        draw_shapeArrayImmediate(context, blockInfo.preview.shapes, (QUILT.blockList.canvasSize - 1) / QUILT.blockList.previewSize);
    }
}