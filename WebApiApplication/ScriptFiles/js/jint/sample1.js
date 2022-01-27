var message = 'Hello, World! By Typescript';
var Shape = /** @class */ (function () {
    function Shape(name) {
        this.name = name;
    }
    Shape.prototype.draw = function (x, y) {
        message = message + ' ' + ("Draw " + this.name + " at " + x + ", " + y);
    };
    return Shape;
}());
var shape = new Shape("MyCircle");
var result = shape.draw(100, 100);
