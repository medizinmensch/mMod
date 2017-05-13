/**
 * Created by jakob on 30.09.16.
 */

function BezierControl(points, container) {
    container = $("#bezier-container");
    var cvs = container.find("canvas").get(0);
    // curve_list holds the Bezier-objects forming the curve
    var curve_list = [];

    // the drawing object is responsible for everything concerning the appearance of the curve on the canvas.
    // redraw() should be called whenever elements of curve_list change or are added/removed
    // get_scale() returns the factor used to translate points on the curve to points on the canvas (used by interaction)

    var drawing = function () {
        var ctx = cvs.getContext("2d");
        cvs.width  = container.width();
        cvs.height = container.height();
        var styles = {
            curve_style: {
                strokeStyle: "white",
                fillStyle: "#696969",
                lineWidth: 2
            },
            endpoint_style: {
                strokeStyle: "blue",
                fillStyle: "blue",
                lineWidth: 0.1
            },
            midpoint_style: {
                strokeStyle: "green",
                fillStyle: "green",
                lineWidth: 0.1
            },
            line_style: {
                strokeStyle: "grey",
                lineWidth: 1.2
            }
        };

        // --- public functions --- //
        function redraw() {
            reset();
            var scale = get_scale();
            drawCurves(scale);
            drawSkeleton(scale);
        }

        function get_scale() {
            return Math.min(cvs.height, cvs.width);
            
        }

        // --- helper functions ---//

        function setDrawStyle(style) {
            for (var attr in style) {
                ctx[attr] = style[attr];
            }
        }

        function reset() {
            ctx.clearRect(0, 0, cvs.width, cvs.height);
        }

        function drawCurves(scale, offset) {
            scale = scale || 1;
            offset = offset || {x: 0, y: 0};
            setDrawStyle(styles.curve_style);
            ctx.beginPath();
            for (var i = 0; i < curve_list.length; i++) {
                var pts = curve_list[i].points.map(function (p) {
                    return {x: p.x * scale + offset.x, y: p.y * scale + offset.y}
                });
                if (i == 0) {
                    ctx.moveTo(pts[0].x, pts[0].y);
                }
                ctx.bezierCurveTo(
                    pts[1].x, pts[1].y,
                    pts[2].x, pts[2].y,
                    pts[3].x, pts[3].y
                );
            }
            ctx.stroke();
            ctx.closePath();
            ctx.fill();
        }

        function drawSkeleton(scale, offset) {
            scale = scale || 1;
            offset = offset || {x: 0, y: 0};
            for (var i = 0; i < curve_list.length; i++) {
                var pts = curve_list[i].points.map(function (p) {
                    return {x: p.x * scale + offset.x, y: p.y * scale + offset.y}
                });
                setDrawStyle(styles.line_style);
                drawLine(pts[0], pts[1]);
                drawLine(pts[2], pts[3]);
                drawPoints(pts);
            }
        }

        function drawLine(p1, p2) {
            ctx.beginPath();
            ctx.moveTo(p1.x, p1.y);
            ctx.lineTo(p2.x, p2.y);
            ctx.stroke();
        }

        function drawPoints(points) {
            for (var i = 0; i < points.length; i++) {
                setDrawStyle(i == 0 || i == 3 ? styles.endpoint_style : styles.midpoint_style);
                drawSquare(points[i], 3);
            }
        }

        function drawSquare(p, r) {
            ctx.beginPath();
            ctx.rect(p.x - r, p.y - r, 2 * r, 2 * r);
            ctx.stroke();
            ctx.fill();
        }

        return {redraw: redraw, get_scale: get_scale, styles: styles}
    }();

    // Redraw canvas after pixelgrid loaded
    $('#pixelgrid-container').ready(function(){
        if ( $(window).width() < 720){
                cvs.height = $('#pixelgrid-container').height();
            } else {
                cvs.height = 240;
        }
        drawing.redraw();
    });

    $(window).resize(function(){
        cvs.width = container.width();
        if ( $(window).width()  < 720){
            cvs.height = $('#pixelgrid-container').height();
        } else {
            cvs.height = 240;
        }
        console.log(container.width());
        drawing.redraw();
    });

    // The interaction object is responsible for handling interaction with the user to transform the curve
    // It adds event listeners for the canvas and binds functions to the add/remove and buttons
    // It also exposes a subscribe() function to externally register for certain events (update, mousedrop, point add ...)
    // The fire_event() function can also be called from other components of BezierControl to signal that a certain event happened
    // The update_points() function should be called whenever elements are added/removed from curve_list
    var interaction = function () {
        var observers = [];
        var points = [];
        var cvs = container.find("canvas").get(0);
        var moving = false, add_enabled = false, remove_enabled = false, mx = 0, my = 0, ox = 0, oy = 0, cx, cy, mp = {};
        var add_button = container.find(".add_button");
        var remove_button = container.find(".remove_button");

        // A dragpoint is a Point with a reference to its partners (the surrounding points of an endpoint)
        // When the endpoint is moved, the surrounding points are moved with it
        function DragPoint(coords, is_endpoint, partners, id) {
            is_endpoint = is_endpoint || false;
            partners = partners || [];
            this.is_endpoint = is_endpoint;
            this.partners = partners;
            this.coords = coords;
            this.id = id;

            this.move_to = function (x, y) {
                var ox = x - coords.x;
                var oy = y - coords.y;
                move(this, ox, oy);
                if (this.is_endpoint) {
                    this.partners.forEach(function (p) {
                        move(p, ox, oy)
                    })
                }
            };

            function move(p, ox, oy) {
                p.coords.x += ox;
                p.coords.y += oy;
            }
        }


        cvs.addEventListener("mousedown", function (evt) {
            if (add_enabled || remove_enabled) {
                return;
            }

            fix(evt);
            mx = evt.offsetX;
            my = evt.offsetY;
            // don't include first and last point, since it should not be dragged
            points.slice(1, points.length - 1).forEach(function (point) {
                var p = point.coords;
                if (Math.abs(mx - p.x * drawing.get_scale()) < 10 && Math.abs(my - p.y * drawing.get_scale()) < 10) {
                    moving = true;
                    mp = point;
                    cx = p.x;
                    cy = p.y;
                }
            });
        });

        // Here the possibility to drag the points of the curve is implemented
        cvs.addEventListener("mousemove", function (evt) {
            fix(evt);

            var selected_point;
            // don't include first and last point, since it should not be dragged
            points.slice(1, points.length - 1).forEach(function (point) {
                var p = point.coords;
                var mx = evt.offsetX;
                var my = evt.offsetY;
                if (Math.abs(mx - p.x * drawing.get_scale()) < 10 && Math.abs(my - p.y * drawing.get_scale()) < 10) {
                    selected_point = point;
                }
            });

            // Style of pointer is dependent on add/remove_enabled and on is_endpoint (first and last endpoint is not draggable)
            cvs.style.cursor = selected_point && !add_enabled && !remove_enabled || (remove_enabled && selected_point.is_endpoint && selected_point.id != 0 && selected_point.id != points.length - 1) ? " pointer" : "default";

            if (!moving) return;

            ox = evt.offsetX - mx;
            oy = evt.offsetY - my;
            //mp.x = cx + ox / drawing.get_scale();
            //mp.y = cy + oy / drawing.get_scale();
            mp.move_to(cx + ox / drawing.get_scale(), cy + oy / drawing.get_scale());

            curve_list.forEach(function (curve) {
                curve.update()
            });

            drawing.redraw();
            fire_event("update");
        });

        cvs.addEventListener("mouseup", function (evt) {
            if (add_enabled || remove_enabled || !moving) return;
            moving = false;
            mp = {};

            // delay callbacks a little bit so canvas can update first
            setTimeout(function () {
                fire_event("mousedrop")
            }, 100);
        });

        // clicking on the canvas is relevant for adding and removing points
        cvs.addEventListener("click", function (evt) {
            fix(evt);
            var mx = evt.offsetX / drawing.get_scale();
            var my = evt.offsetY / drawing.get_scale();

            if (add_enabled) {
                add_point(mx, my);
                update_points();
            }

            if (remove_enabled) {
                remove_point(mx, my);
                update_points();
            }
        });

        add_button.click(function () {
            if (remove_enabled) {
                toggle_remove()
            }
            toggle_add()
        });

        remove_button.click(function () {
            if (add_enabled) {
                toggle_add()
            }
            toggle_remove()
        });

        // --- public functions --- //
        function update_points() {
            var pts = [];
            for (var i = 0; i < curve_list.length; i++) {
                for (var j = 0; j < curve_list[i].points.length; j++) {
                    // to avoid duplicates in the list, dont push the fist point of the curve, unless its the very first
                    if (i == 0 || j > 0) {
                        pts.push(new DragPoint(curve_list[i].points[j]))
                    }
                }
            }

            // register the surrounding midpoints of an endpoint as his partners, so they can be dragged if the endpoint is dragged
            for (i = 0; i < pts.length; i++) {
                pts[i].id = i;
                var partners = [];
                if (i % 3 == 0) {
                    pts[i].is_endpoint = true;
                    if (i != 0 && i != pts.length - 1) {
                        partners.push(pts[i - 1]);
                    }
                    if (i != 0) {
                        partners.push(pts[i + 1]);
                    }

                }

                pts[i].partners = partners;
            }
            points = pts;
        }

        function get_points() {
            return points.slice();
        }

        function subscribe(event, callback) {
            observers.push({event: event, callback: callback});
        }

        function fire_event(event) {
            observers.forEach(function (observer) {
                if (observer.event === event) {
                    observer.callback();
                }
            });
        }

        // --- helper functions --- //
        function fix(e) {
            e = e || window.event;
            var target = e.target || e.srcElement,
                rect = target.getBoundingClientRect();
            e.offsetX = e.clientX - rect.left;
            e.offsetY = e.clientY - rect.top;
        }

        function toggle_add() {
            add_enabled = !add_enabled;
            add_button.toggleClass("button-active");
        }

        function toggle_remove() {
            remove_enabled = !remove_enabled;
            remove_button.toggleClass("button-active")
        }

        return {update_points: update_points, get_points: get_points, subscribe: subscribe, fire_event: fire_event}
    }();

    // Initialize the curve with the given arguments
    // The number of arguments must be valid for cubic Bezier-Curves
    if ((points.length - 2) % 6 != 0 || points.length < 8) throw "Invalid number of Points: " + points.length + " (must be one of 4, 7, 10, 13 ...)";

    for (var i = 0; i < (points.length - 2) / 6; i++) {
        if (i == 0) {
            curve_list.push(new Bezier(points[0], points[1], points[2], points[3], points[4], points[5], points[6], points[7]))
        } else {
            var new_curve = new Bezier(0, 0, points[i * 8], points[i * 8 + 1], points[i * 8 + 2], points[i * 8 + 3], points[i * 8 + 4], points[i * 8 + 5]);
            new_curve.points[0] = curve_list[i - 1].points[3];
            new_curve.update()
            curve_list.push(new_curve);
        }
    }

    // refresh interaction and drawing
    interaction.update_points();
    drawing.redraw();

    // --- public functions --- //
    function add_point(x, y) {
        // add a point to the curve, not changing the geometry

        var delta = 0.05;
        var curve = undefined;
        var t = 0;
        var new_pos = 0;
        var min_dist;

        for (var i = 0; i < curve_list.length; i++) {
            var lut = curve_list[i].getLUT(100);
            for (var j = 0; j < lut.length; j++) {
                var dist = Math.sqrt(Math.pow(lut[j].x - x, 2) + Math.pow(lut[j].y - y, 2));
                if ((i == 0 && j == 0) || dist < min_dist) {
                    min_dist = dist;
                    curve = curve_list[i];
                    t = j / 100;
                    new_pos = i + 1;
                }
            }
        }

        if (min_dist > delta) {
            return;
        }

        // see http://stackoverflow.com/questions/2613788/
        var p0 = curve.points[0], p1 = curve.points[1], p2 = curve.points[2], p3 = curve.points[3];
        var p0_1 = {}, p1_2 = {}, p2_3 = {}, p01_12 = {}, p12_23 = {}, p0112_1223 = {};

        ["x", "y"].forEach(function (i) {
            p0_1[i] = (1 - t) * p0[i] + t * p1[i];
            p1_2[i] = (1 - t) * p1[i] + t * p2[i];
            p2_3[i] = (1 - t) * p2[i] + t * p3[i];

            p01_12[i] = (1 - t) * p0_1[i] + t * p1_2[i];
            p12_23[i] = (1 - t) * p1_2[i] + t * p2_3[i];

            p0112_1223[i] = (1 - t) * p01_12[i] + t * p12_23[i];

        });

        // update old curve
        curve.points[1] = p0_1;
        curve.points[2] = p01_12;
        curve.points[3] = p0112_1223;
        curve.update();

        // create new curve
        var new_curve = new Bezier(0, 0, 0, 0, 0, 0, 0, 0);
        new_curve.points = [p0112_1223, p12_23, p2_3, p3];
        new_curve.update();

        // insert new curve
        add_curve(new_curve, new_pos);
        // delay callbacks a little bit so canvas can update first
        setTimeout(function () {
            interaction.fire_event("point_added");
        }, 10);
    }

    function remove_point(x, y) {
        var delta = 0.05;

        for (var i = 1; i < curve_list.length; i++) {
            if (Math.sqrt(Math.pow(curve_list[i].points[0].x - x, 2) + Math.pow(curve_list[i].points[0].y - y, 2)) < delta) {
                curve_list[i - 1].points[3] = curve_list[i].points[3];
                remove_curve(i);
                // delay callbacks a little bit so canvas can update first
                setTimeout(function () {
                    interaction.fire_event("point_removed");
                }, 100);
                return;
            }
        }
    }

    function getLUT(steps) {
        // get Look Up Table consisting of points of all curves in curve_list
        // the total number of points is always steps, but if steps is not dividable by curve_list.length,
        // the first curve gets the remainder as additional points
        steps = steps || 100;
        var steps_each = Math.floor(steps / curve_list.length);
        var lut = curve_list[0].getLUT(steps_each + steps % curve_list.length);
        curve_list.slice(1).forEach(function (curve) {
            lut = lut.concat(curve.getLUT(steps_each).slice(1));
        });
        return lut;
    }

    function normal(t) {
        // returns the normal at the given t, where 0 is the first point of the first curve and 1 the last point of the last curve.
        // If t exactly between two curves, where two curves share a point, the normal of the first curve is used
        if (t == 1) {
            return curve_list[curve_list.length - 1].normal(1);
        }
        var i = Math.floor(curve_list.length * t);
        var t_i = curve_list.length * t - i;
        return curve_list[i].normal(t_i);
    }

    function get(t) {
        // returns the point at the given t, where 0 is the first point of the first curve and 1 the last point of the last curve.
        if (t == 1) {
            return curve_list[curve_list.length - 1].get(1);
        }
        var i = Math.floor(curve_list.length * t);
        var t_i = curve_list.length * t - i;
        return curve_list[i].get(t_i);
    }

    // --- helper functions --- //
    function add_curve(curve, pos) {
        pos = pos || 0;
        curve_list.splice(pos, 0, curve);
        interaction.update_points();
        drawing.redraw();
    }

    function remove_curve(pos) {
        curve_list.splice(pos, 1);
        interaction.update_points();
        drawing.redraw();
    }

    // expose public functions
    this.subscribe = interaction.subscribe;
    this.add_point = add_point;
    this.remove_point = remove_point;
    this.get_points = interaction.get_points;
    this.getLUT = getLUT;
    this.normal = normal;
    this.get = get;
    this.draw_styles = drawing.styles;
    //this.intersects = intersects
}


