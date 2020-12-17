var QUILT;
if (!QUILT) {
    QUILT = {};
}

QUILT.tooltray = {
    id: null
};

function tooltray_initialize() {
    $(".tooltray").addClass("tooltray--hidden");
}

function tooltray_show(id) {
    QUILT.tooltray.id = id;
    $("#" + QUILT.tooltray.id + ".tooltray").removeClass("tooltray--hidden");
}


function tooltray_hide() {
    $("#" + QUILT.tooltray.id + ".tooltray").addClass("tooltray--hidden");
    QUILT.tooltray.id = null;
}