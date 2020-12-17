var QUILT;
if (!QUILT) {
    QUILT = {};
}

QUILT.colorList = {
    catalog: null, // Design_FabricStyleCatalogData[]
    selectedSku: null,
    selectedDiv: null,
    callback: null,
    callbackData: null
};

function colorList_initialize() {
    $("#divColorsBody").on("mouseenter", ".color-list__item", null, _colorList_onMouseEnter);
    $("#divColorsBody").on("mouseleave", ".color-list__item", null, _colorList_onMouseLeave);
    $("#divColorsBody").on("click", ".color-list__item", null, _colorList_onClick);
    $("#selectManufacturer").on("click", _colorList_onManufacturerChange);
    $("#selectCollection").on("click", _colorList_onCollectionChange);
}

function colorList_open(callback, callbackData, sku) {
    QUILT.colorList.callback = callback;
    QUILT.colorList.callbackData = callbackData;

    if (QUILT.colorList.catalog === null) {
        _colorList_retrieveCatalogAsync(sku);
    }
    else {
        _colorList_setSelection(sku);
    }
}

// ******************************
//
// Private methods.
//
// ******************************

function _colorList_populateManufacturers() {
    var jSelectManufacturer = $("#selectManufacturer").empty();
    for (var idxManufacturer = 0; idxManufacturer < QUILT.colorList.catalog.manufacturers.length; ++idxManufacturer) {
        var manufacturer = QUILT.colorList.catalog.manufacturers[idxManufacturer];
        jSelectManufacturer.append($("<option />").val(manufacturer.manufacturerName).text(manufacturer.manufacturerName));
    }
}

function _colorList_populateCollections() {
    var manufacturerName = _colorList_getSelectedManufacturerName();
    for (var idxManufacturer = 0; idxManufacturer < QUILT.colorList.catalog.manufacturers.length; ++idxManufacturer) {
        var manufacturer = QUILT.colorList.catalog.manufacturers[idxManufacturer];
        if (manufacturer.manufacturerName === manufacturerName) {
            var jSelectCollection = $("#selectCollection").empty();
            for (var idxCollection = 0; idxCollection < manufacturer.collections.length; ++idxCollection) {
                var collection = manufacturer.collections[idxCollection];
                jSelectCollection.append($("<option />").val(collection.collectionName).text(collection.collectionName));
            }
            return;
        }
    }
}

function _colorList_populateFabrics(collection) {

    var jDivColors = $("#divColorsBody");
    jDivColors.empty();

    var jDivCurrentSection = $("<div class='tooltray__section' />");
    jDivColors.append(jDivCurrentSection);

    if (collection === null) {
        collection = _colorList_lookupSelectedCollection();
    }

    for (var index = 0; index < collection.fabricStyles.length; ++index) {
        var fabricStyle = collection.fabricStyles[index];
        jDivCurrentSection.append(
            "<div class='color-list__item tooltray__item' data-color='" +
            fabricStyle.color.webColor +
            "' data-sku='" +
            fabricStyle.sku +
            "' style='background: " +
            fabricStyle.color.webColor +
            ";'></div>");
    }

    _colorList_highlightSelectedDiv();
}

function _colorList_setSelection(sku) {

    var jSelectManufacturer = $("#selectManufacturer");
    var jSelectCollection = $("#selectCollection");

    var skuInfo = _colorList_lookupSkuInfo(sku);
    if (skuInfo === null) {
        _colorList_populateCollections();
        _colorList_populateFabrics(null);
    }
    else if (jSelectManufacturer.val() !== skuInfo.manufacturer.manufacturerName) {
        jSelectManufacturer.val(skuInfo.manufacturer.manufacturerName);
        _colorList_populateCollections();
        jSelectCollection.val(skuInfo.collection.collectionName);
        _colorList_populateFabrics(skuInfo.collection);
    }
    else if (jSelectCollection.val() !== skuInfo.collection.collectionName) {
        _colorList_populateCollections();
        jSelectCollection.val(skuInfo.collection.collectionName);
        _colorList_populateFabrics(skuInfo.collection);
    }

    if (QUILT.colorList.selectedDiv !== null) {
        QUILT.colorList.selectedDiv.removeClass("tooltray__item--selected");
        QUILT.colorList.selectedDiv = null;
    }

    if (sku === null) {
        QUILT.colorList.selectedSku = null;
    }
    else {
        QUILT.colorList.selectedSku = sku;
        _colorList_highlightSelectedDiv();
    }
}

function _colorList_highlightSelectedDiv() {
    if (QUILT.colorList.selectedSku !== null) {
        var jDiv = $(".color-list__item[data-sku='" + QUILT.colorList.selectedSku + "']");
        QUILT.colorList.selectedDiv = jDiv;
        QUILT.colorList.selectedDiv.addClass("tooltray__item--selected");
    }
}

function _colorList_getSelectedManufacturerName() {
    var manufacturerName = $("#selectManufacturer").val();
    return manufacturerName;
}

function _colorList_getSelectedCollectionName() {
    var collectionName = $("#selectCollection").val();
    return collectionName;
}

function _colorList_lookupSelectedCollection() {
    var manufacturerName = _colorList_getSelectedManufacturerName();
    var collectionName = _colorList_getSelectedCollectionName();

    for (var idxManufacturer = 0; idxManufacturer < QUILT.colorList.catalog.manufacturers.length; ++idxManufacturer) {
        var manufacturer = QUILT.colorList.catalog.manufacturers[idxManufacturer];
        if (manufacturer.manufacturerName === manufacturerName) {
            for (var idxCollection = 0; idxCollection < manufacturer.collections.length; ++idxCollection) {
                var collection = manufacturer.collections[idxCollection];
                if (collection.collectionName === collectionName) {
                    return collection;
                }
            }
        }
    }

    return null;
}

function _colorList_lookupSkuInfo(sku) {
    for (var idxManufacturer = 0; idxManufacturer < QUILT.colorList.catalog.manufacturers.length; ++idxManufacturer) {
        var manufacturer = QUILT.colorList.catalog.manufacturers[idxManufacturer];
        for (var idxCollection = 0; idxCollection < manufacturer.collections.length; ++idxCollection) {
            var collection = manufacturer.collections[idxCollection];
            for (var index = 0; index < collection.fabricStyles.length; ++index) {
                var fabricStyle = collection.fabricStyles[index];
                if (fabricStyle.sku === sku) {
                    return {
                        manufacturer: manufacturer,
                        collection: collection,
                        fabricStyle: fabricStyle
                    };
                }
            }
        }
    }
    return null;
}

// ******************************
//
// Ajax methods.
//
// ******************************

function _colorList_retrieveCatalogAsync(sku) {
    $.ajax({
        url: "/api/fabricStyles",
        dataType: "json"
    }).done(function ( /* Design_FabricStyleCatalogData */ response) { _colorList_onRetrieveCatalogComplete(response, sku); });
}

// ******************************
//
// Ajax event handlers.
//
// ******************************

function _colorList_onRetrieveCatalogComplete( /* Design_FabricStyleCatalogData */ response, sku) {
    QUILT.colorList.catalog = response;
    _colorList_populateManufacturers();
    _colorList_setSelection(sku);
}

// ******************************
//
// UI event handlers.
//
// ******************************

function _colorList_onClick(event) {

    var sku = $(event.currentTarget).attr("data-sku");
    var color = $(event.currentTarget).attr("data-color");

    _colorList_setSelection(sku);

    if (QUILT.colorList.callback !== null) {
        QUILT.colorList.callback(QUILT.colorList.callbackData, color, sku);
    }
}

function _colorList_onMouseEnter(event) {
    $(this).addClass("tooltray__item--hover");
}

function _colorList_onMouseLeave(event) {
    $(this).removeClass("tooltray__item--hover");
}

function _colorList_onManufacturerChange() {
    _colorList_populateCollections();
    _colorList_populateFabrics(null);
}

function _colorList_onCollectionChange() {
    _colorList_populateFabrics(null);
}

//
// OBSOLETE
//

function _colorList_X_lookupManufacturer(manufacturerName) {
    for (var idxManufacturer = 0; idxManufacturer < QUILT.colorList.catalog.manufacturers.length; ++idxManufacturer) {
        var manufacturer = QUILT.colorList.catalog.manufacturers[idxManufacturer];
        if (manufacturer.manufacturerName === manufacturerName) {
            return manufacturer;
        }
    }

    return null;
}

function _colorList_X_populateList() {

    var divColors = $("#divColorsBody");
    divColors.empty();

    currentSection = $("<div class='tooltray__section' />");
    divColors.append(currentSection);

    for (var idxManufacturer = 0; idxManufacturer < QUILT.colorList.catalog.manufacturers.length; ++idxManufacturer) {
        var manufacturer = QUILT.colorList.catalog.manufacturers[idxManufacturer];
        currentSection.append("<h2>" + manufacturer.manufacturerName + "<h2>");

        for (var idxCollection = 0; idxCollection < manufacturer.collections.length; ++idxCollection) {
            var collection = manufacturer.collections[idxCollection];
            currentSection.append("<h3>" + collection.collectionName + "<h3>");

            for (var index = 0; index < collection.fabricStyles.length; ++index) {
                var fabricStyle = collection.fabricStyles[index];
                currentSection.append(
                    "<div class='color-list__item tooltray__item' data-color='" +
                    fabricStyle.color.webColor +
                    "' data-sku='" +
                    fabricStyle.sku +
                    "' style='background: " +
                    fabricStyle.color.webColor +
                    ";'></div>");
            }
        }
    }
}