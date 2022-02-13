class Shape {
    name: string;

    constructor(name: string) {
        this.name = name;
    }

    draw(x: number, y: number): string {
        return `Draw ${this.name} at ${x}, ${y}`;
    }
}
let shape = new Shape("MyCircle");
let result = shape.draw(100, 100);