var QUILT;
if (!QUILT) {
    QUILT = {};
}

QUILT.select = {
    selection: null,
    selectionName: null,
    selectionId: null,
    callbacks: {}
};

function select_initialize() {
    $(".select").on("click", ".select__item", null, _select_onclick);
    $(".select").on("mouseenter", ".select__item", null, _select_onMouseEnter);
    $(".select").on("mouseleave", ".select__item", null, _select_onMouseLeave);
}

function select_addCallback(name, callback) {
    QUILT.select.callbacks[name] = callback;
}

function select_refresh() {
    if (QUILT.select.selectionName !== null) {
        var selection;
        if (QUILT.select.selectionId !== null) {
            selection = $(".select__item[data-select-name='" + QUILT.select.selectionName + "'][data-select-id='" + QUILT.select.selectionId + "']");
        }
        else {
            selection = $(".select__item[data-select-name='" + QUILT.select.selectionName + "']");
        }
        selection.addClass("select__item--selected");
        QUILT.select.selection = selection.get(0);
    }
}

function _select_onMouseEnter() {
    $(this).addClass("select__item--hover");
}

function _select_onMouseLeave() {
    $(this).removeClass("select__item--hover");
}

function _select_invokeCallback(name, value) {
    var callback = QUILT.select.callbacks[name];
    if (callback) {
        callback(value);
    }
}

function _select_onclick() {

    var wrappedThis = $(this);
    var selectionName = wrappedThis.attr("data-select-name");
    var selectionId = wrappedThis.attr("data-select-id");
    if (selectionId === undefined) {
        selectionId = null;
    }

    if (QUILT.select.selection === null) {
        QUILT.select.selection = this;
        QUILT.select.selectionName = selectionName;
        QUILT.select.selectionId = selectionId;
        $(QUILT.select.selection).addClass("select__item--selected");
        _select_invokeCallback(QUILT.select.selectionName, this);
    }
    else {
        if (QUILT.select.selection === this) {
            $(QUILT.select.selection).removeClass("select__item--selected");
            _select_invokeCallback(QUILT.select.selectionName, null);

            QUILT.select.selection = null;
            QUILT.select.selectionName = null;
            QUILT.select.selectionId = null;
        }
        else {
            if (QUILT.select.selectionName === selectionName) {
                $(QUILT.select.selection).removeClass("select__item--selected");

                QUILT.select.selection = this;
                QUILT.select.selectionId = selectionId;
                $(QUILT.select.selection).addClass("select__item--selected");
                _select_invokeCallback(QUILT.select.selectionName, this);
            }
            else {
                $(QUILT.select.selection).removeClass("select__item--selected");
                _select_invokeCallback(QUILT.select.selectionName, null);

                QUILT.select.selection = this;
                QUILT.select.selectionName = selectionName;
                QUILT.select.selectionId = selectionId;
                $(QUILT.select.selection).addClass("select__item--selected");
                _select_invokeCallback(QUILT.select.selectionName, this);
            }
        }
    }
}
