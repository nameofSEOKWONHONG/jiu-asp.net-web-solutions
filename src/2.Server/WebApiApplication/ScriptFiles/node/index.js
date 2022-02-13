var module1 = require("./module1");
module.exports = (callback, x, y) => {  // Module must export a function that takes a callback as its first parameter
    //var result = x + y; // Your javascript logicr
    var result = `${x.a} + ${x.b} + ${x.c} + ${y.a} + ${y.b} + ${y.c} `; 
    result += `and ${module1.HelloWorld} + ${module1.HelloWorldNodeJs} `;
    result += `and ${module1.YourName("jiu")}`;
    callback(null /* If an error occurred, provide an error object or message */, result); // Call the callback when you're done.
};