﻿using Application.Base;
using Application.Infrastructure.Message;
using Domain.Enums;
using eXtensionSharp;
using Infrastructure.Abstract;
using LibreHardwareMonitor.Hardware;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApiApplication.Services.Abstract;

namespace Infrastructure.BackgroundServices;

public class HardwareMonitorBackgroundService : MessageNotifyBackgroundServiceBase
{
    private const int WARNING_VALUE = 90;
    private const int WARNING_CNT = 10;
    
    private readonly Computer _computer;
    
    public HardwareMonitorBackgroundService(ILogger<HardwareMonitorBackgroundService> logger,
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory,
        MessageProviderResolver messageProviderResolver) : base(logger, configuration, serviceScopeFactory, messageProviderResolver)
    {
        this._notifyMessageProvider = messageProviderResolver(ENUM_NOTIFY_MESSAGE_TYPE.EMAIL);
        this._serviceScopeFactory = serviceScopeFactory;
        _computer = new Computer()
        {
            IsCpuEnabled = true,
            IsMemoryEnabled = true,
            IsNetworkEnabled = true,
            IsStorageEnabled = true
        };
    }

    protected override async Task OnRunAsync(CancellationToken stoppingToken)
    {
        var warningCnt = 0;
        
        _computer.Open();
        _computer.Accept(new UpdateVisitor());

        foreach (IHardware hardware in this._computer.Hardware)
        {
            // _logger.LogInformation("Hardware: {0}", hardware.Name);
            // foreach (IHardware subhardware in hardware.SubHardware)
            // {
            //     _logger.LogInformation("\tSubhardware: {0}", subhardware.Name);
            //
            //     foreach (ISensor sensor in subhardware.Sensors)
            //     {
            //         _logger.LogInformation("\t\tSensor: {0}, value: {1}", sensor.Name, sensor.Value);
            //     }
            // }
            //
            // foreach (ISensor sensor in hardware.Sensors)
            // {
            //     _logger.LogInformation("\tSensor: {0}, value: {1}", sensor.Name, sensor.Value);
            // }

            var cpuSencer = hardware.Sensors.FirstOrDefault(m => m.Name == "CPU Total");
            if (cpuSencer.xIsNotEmpty())
            {
                if (cpuSencer.Value > WARNING_VALUE)
                {
                    warningCnt += 1;
                    if (warningCnt > WARNING_CNT)
                    {
                        using var scope = _serviceScopeFactory.CreateScope();
                        var service = scope.ServiceProvider.GetRequiredService<IUserService>();
                        var users = await service.FindAllUserByRoleAsync(new[]{ENUM_ROLE_TYPE.ADMIN, ENUM_ROLE_TYPE.SUPER});
                        
                        await _notifyMessageProvider.SendMessageAsync(
                            new EmailNotifyMessageRequest(
                                users.Select(m => m.EMAIL).ToArray(),
                                "waning cpu usage rate",
                                "waning cpu usage rate. monitoring necessary.",
                                null));

                        warningCnt = 0;
                    }
                }
            }
        }
        
        _computer.Close();
    }
}

public class UpdateVisitor : IVisitor
{
    public void VisitComputer(IComputer computer)
    {
        computer.Traverse(this);
    }

    public void VisitHardware(IHardware hardware)
    {
        hardware.Update();
        foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
    }

    public void VisitSensor(ISensor sensor)
    {
    }

    public void VisitParameter(IParameter parameter)
    {
    }
}