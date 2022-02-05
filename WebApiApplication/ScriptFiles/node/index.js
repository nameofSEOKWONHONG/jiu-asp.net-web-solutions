module.exports = (callback, x, y) => {  // Module must export a function that takes a callback as its first parameter
    //var result = x + y; // Your javascript logic
    debugger;
    var result = `${x.a} + ${x.b} + ${x.c} + ${y.a} + ${y.b} + ${y.c}`; 
    callback(null /* If an error occurred, provide an error object or message */, result); // Call the callback when you're done.
};