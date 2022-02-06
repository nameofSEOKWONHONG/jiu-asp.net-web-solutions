function printHelloWorld() {
    return "Hello World";
}

function printHelloWorldNodeJs() {
    return "Hello World NodeJS World";
}

function printYourName(name) {
    return "Hello " + name;
}

module.exports.HelloWorld = printHelloWorld();
module.exports.HelloWorldNodeJs = printHelloWorldNodeJs();
module.exports.YourName = printYourName;