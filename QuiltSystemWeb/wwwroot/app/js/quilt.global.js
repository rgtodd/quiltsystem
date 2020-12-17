var QUILT;
if (!QUILT) {
    QUILT = {};
}

QUILT.TEST_QUILT = {
    pieces: [
        { path: [{ x: 0, y: 0 }, { x: 100, y: 0 }, { x: 0, y: 100 }], color: "Red" },
        { path: [{ x: 100, y: 0 }, { x: 100, y: 100 }, { x: 0, y: 100 }], color: "Blue" }
    ]
};

QUILT.RENDER_CANVAS = function () {
    var canvas = document.createElement("canvas");
    canvas.width = 500;
    canvas.height = 500;
    return canvas;
}();

QUILT.OFFSCREEN_CANVAS = function () {
    var canvas = document.createElement("canvas");
    canvas.width = 500;
    canvas.height = 500;
    return canvas;
}();

QUILT.SELECTED_PIECE = null;

QUILT.activeQuilt = null;



