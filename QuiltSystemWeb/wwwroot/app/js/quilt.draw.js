function draw_layoutSiteArrayImmediate(context, layoutSiteArray, scale) {
    for (var idxLayoutSite = 0; idxLayoutSite < layoutSiteArray.length; ++idxLayoutSite) {
        var layoutSite = layoutSiteArray[idxLayoutSite];
        _draw_layoutSite(context, layoutSite, scale);
    }
}

function draw_shapeArrayImmediate(context, shapeArray, scale) {
    if (shapeArray !== null) {
        for (var idxShape = 0; idxShape < shapeArray.length; ++idxShape) {
            var shape = shapeArray[idxShape];
            _draw_shape(context, shape, scale);
        }
    }
}

function _draw_shape(context, shape, scale) {
    context.fillStyle = shape.color.webColor;
    context.strokeStyle = "Black";
    context.lineWidth = 1;
    context.lineJoin = 'round';
    _draw_path(context, shape.path, scale);
    context.fill();
    context.stroke();
}

function _draw_layoutSite(context, layoutSite, scale) {
    if (layoutSite.color !== null) {
        context.fillStyle = layoutSite.color.webColor;
        context.strokeStyle = "Black";
        context.lineWidth = 1;
        context.lineJoin = 'round';
        _draw_path(context, layoutSite.path, scale);
        context.fill();
        context.stroke();
    }
}

function _draw_path(context, path, scale) {
    context.beginPath();
    for (var idxPoint = 0; idxPoint < path.points.length; ++idxPoint) {
        var point = path.points[idxPoint];
        if (idxPoint === 0) {
            context.moveTo(Math.round(point.x * scale) + 0.5, Math.round(point.y * scale) + 0.5);
        }
        else {
            context.lineTo(Math.round(point.x * scale) + 0.5, Math.round(point.y * scale) + 0.5);
        }
    }
    context.closePath();
}