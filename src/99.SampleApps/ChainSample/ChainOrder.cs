namespace ChainSample;

#region [chain interfaces]

public interface IAddOrderItem
{
    IAddOrShipItem AddItem(string code, int qty);
}

public interface IAddOrShipItem
{
    IAddOrShipItem AddItem(string code, int qty);
    IReadyToProcess AddShippingDetails(string address,
        string city,
        string postalcode);
}

public interface IReadyToProcess
{
    OrderDto Process();
}

#endregion

#region [data translate object]

public record OrderDto
{
    public int Id { get; set; }
    public OrderShippingDetailDto ShippingDetailDto { get; set; } = new OrderShippingDetailDto();
    public CustomerDetailDto CustomerDetailDto { get; set; } = new CustomerDetailDto();
    public List<ShippingItemDto> ShippingItemDto { get; set; } = new List<ShippingItemDto>();
}

public record OrderShippingDetailDto
{
    public string City { get; set; }
    public string PostalCode { get; set; }
}

public record CustomerDetailDto
{
    public string Name { get; set; }
}

public record ShippingItemDto
{
    public string Code { get; set; }
    public int Qty { get; set; }
}

#endregion

#region [implement chain methods]

public class Order:IAddOrderItem, IAddOrShipItem, IReadyToProcess
{
    private OrderDto _orderDto;
    private Order(string name)
    {
        _orderDto = new OrderDto()
        {
            Id = 1
        };
        _orderDto.CustomerDetailDto.Name = name;
    }

    public IReadyToProcess AddShippingDetails(string address,
        string city,
        string postalcode)
    {
        _orderDto.ShippingDetailDto.City = city;
        _orderDto.ShippingDetailDto.PostalCode = postalcode;
        return this;
    }
    
    public static IAddOrderItem AddCustomerDetails
        (string name)
    {
        return new Order(name);
    }

    public IAddOrShipItem AddItem(string code, int qty)
    {
        _orderDto.ShippingItemDto.Add(new ShippingItemDto() { Code = code, Qty = qty });
        return this;
    }
    
    public OrderDto Process()
    {
        return this._orderDto;
    }
}

#endregion

