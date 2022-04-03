// See https://aka.ms/new-console-template for more information
using ChainSample;

var order = Order
    .AddCustomerDetails("test")
    .AddItem("11011", 10)
    .AddShippingDetails("seoul", "hwagok", "111-11")
    .Process();
    
    Console.WriteLine(order);