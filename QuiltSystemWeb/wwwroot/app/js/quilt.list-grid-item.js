var QUILT;
if (!QUILT) {
    QUILT = {};
}

QUILT.listGridItem = {
    currentSelection: null
};

function listGridItem_initialize() {
    $(".list-grid-item").on("click", function () {
        if (QUILT.listGridItem.currentSelection !== null) {
            QUILT.listGridItem.currentSelection.removeClass("list-grid-item--selected");
        }

        var newSelection = $(this);
        if (newSelection.is(QUILT.listGridItem.currentSelection)) {
            QUILT.listGridItem.currentSelection = null;
        }
        else {
            QUILT.listGridItem.currentSelection = newSelection;
            QUILT.listGridItem.currentSelection.addClass("list-grid-item--selected");
        }
    });

    //$(".list-grid-item").on("mouseenter", function () {
    //    $(this).addClass("list-grid-item--selected");
    //});

    //$(".list-grid-item").on("mouseleave", function () {
    //    $(this).removeClass("list-grid-item--selected");
    //});
}