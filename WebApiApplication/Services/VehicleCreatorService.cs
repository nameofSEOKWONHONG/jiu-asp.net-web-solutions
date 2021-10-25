using System;
using System.IO.Pipelines;
using Microsoft.Extensions.DependencyInjection;

namespace WebApiApplication.Services
{
    public interface IVehicleCreatorService
    {
        public string Create();
    }

    public class VehicleCreatorServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public VehicleCreatorServiceFactory(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }
        public IVehicleCreatorService CreateService<T>() where T : IVehicleCreatorService
        {
            return this._serviceProvider.GetService<T>();
        }
    }

    public class BoatCreatorService : IVehicleCreatorService
    {
        public string Create()
        {
            return "create boat";
        }
    }

    public class CarCreatorService : IVehicleCreatorService
    {
        public string Create()
        {
            return "create car";
        }        
    }

    public class BusCreatorService : IVehicleCreatorService
    {
        public string Create()
        {
            return "create bus";
        }        
    }
}