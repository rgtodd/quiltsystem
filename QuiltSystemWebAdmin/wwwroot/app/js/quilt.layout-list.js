var QUILT;
if (!QUILT) {
    QUILT = {};
}

QUILT.layoutList = {
    layoutInfos: null, // Design_LayoutInfoData[]
    selectedLayoutName: null,
    selectedDiv: null,
    callback: null,
    callbackData: null,
    canvasSize: 50,
    previewSize: 10000
};

function layoutList_initialize() {
    $("#divLayouts").on("mouseenter", ".layout-list__item", null, _layoutList_onMouseEnter);
    $("#divLayouts").on("mouseleave", ".layout-list__item", null, _layoutList_onMouseLeave);
    $("#divLayouts").on("click", ".layout-list__item", null, _layoutList_onClick);
}

function layoutList_open(callback, callbackData, layoutName) {
    QUILT.layoutList.callback = callback;
    QUILT.layoutList.callbackData = callbackData;

    _layoutList_setSelection(layoutName);
}

function layoutList_refreshListAsync(rowCount, columnCount, size) {
    $.ajax({
        url: "/api/layouts?rowCount=" + rowCount + "&columnCount=" + columnCount + "&size=" + QUILT.layoutList.previewSize,
        dataType: "json"
    }).done(_layoutList_onRefreshListComplete);
}

// ******************************
//
// Ajax event handlers.
//
// ******************************

function _layoutList_onRefreshListComplete( /* Design_LayoutInfoData[] */ response) {
    QUILT.layoutList.layoutInfos = response;
    _layoutList_populateList();
    //_blockList_setSelection(QUILT.blockList.selectedBlockName);
}

// ******************************
//
// UI event handlers.
//
// ******************************

function _layoutList_onClick(event) {

    var layoutName = $(event.currentTarget).attr("data-layout-name");

    _layoutList_setSelection(layoutName);

    if (QUILT.layoutList.callback !== null) {
        var layoutInfo = _layoutList_lookupInfo(layoutName);
        if (layoutInfo !== null) {
            QUILT.layoutList.callback(QUILT.layoutList.selection, layoutInfo);
        }
    }
}

function _layoutList_onMouseEnter(event) {
    $(this).addClass("tooltray__item--hover");
}

function _layoutList_onMouseLeave(event) {
    $(this).removeClass("tooltray__item--hover");
}

// ******************************
//
// Private methods.
//
// ******************************

function _layoutList_lookupInfo(layoutName) {
    for (var idx = 0; idx < QUILT.layoutList.layoutInfos.length; ++idx) {
        var layoutInfo = QUILT.layoutList.layoutInfos[idx];
        if (layoutInfo.layoutName === layoutName) {
            return layoutInfo;
        }
    }

    return null;
}

function _layoutList_setSelection(layoutName) {

    if (QUILT.layoutList.selectedDiv !== null) {
        QUILT.layoutList.selectedDiv.removeClass("tooltray__item--selected");
    }

    if (layoutName === null) {
        QUILT.layoutList.selectedLayoutName = null;
        QUILT.layoutList.selectedDiv = null;
    }
    else {
        var div = $(".layout-list__item[data-layout-name='" + layoutName + "']");
        QUILT.layoutList.selectedLayoutName = layoutName;
        QUILT.layoutList.selectedDiv = div;
        QUILT.layoutList.selectedDiv.addClass("tooltray__item--selected");
    }
}

function _layoutList_populateList() {
    $("#divLayouts").empty();

    currentSection = $("<div class='tooltray__section'></div>");
    $("#divLayouts").append(currentSection);
    for (var idx = 0; idx < QUILT.layoutList.layoutInfos.length; ++idx) {
        var layoutInfo = QUILT.layoutList.layoutInfos[idx];

        var divId = "layoutDiv_" + layoutInfo.id;
        var canvasId = "layoutCanvas_" + layoutInfo.id;

        var maxLayoutSize = layoutInfo.preview.width;
        if (maxLayoutSize < layoutInfo.preview.height) {
            maxLayoutSize = layoutInfo.preview.height;
        }

        var canvasWidth = Math.round(QUILT.layoutList.canvasSize * layoutInfo.preview.width / maxLayoutSize);
        var canvasHeight = Math.round(QUILT.layoutList.canvasSize * layoutInfo.preview.height / maxLayoutSize);

        currentSection.append(
            "<div id='" + divId + "' data-layout-name='" + layoutInfo.layoutName + "' class='layout-list__item tooltray__item'>" +
            "<canvas id='" + canvasId + "' class='tooltray__item-canvas' width='" + canvasWidth + "' height='" + canvasHeight + "'></canvas></div>");

        var canvas = document.getElementById(canvasId);
        var context = canvas.getContext("2d");

        draw_layoutSiteArrayImmediate(context, layoutInfo.preview.layoutSites, (QUILT.layoutList.canvasSize - 1) / QUILT.layoutList.previewSize);
    }
}