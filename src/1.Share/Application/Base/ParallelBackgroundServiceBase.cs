﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using eXtensionSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Base;

public abstract class ParallelBackgroundServiceBase<TEntity> : BackgroundService
    where TEntity : class
{
    protected ILogger _logger;
    protected IConfiguration _configuration;
    protected IServiceScopeFactory _serviceScopeFactory;
    private int _interval;
    private int _maxDegreeOfParallelism;
    
    protected readonly TimeSpan _from = TimeSpan.Zero;
    protected readonly TimeSpan _to = TimeSpan.Zero;
    private readonly string _dateFormat = "yyyyMMddHHmmss";
    private readonly bool _useTimeZone = false;
    
    protected ParallelBackgroundServiceBase(ILogger logger,
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory,
        int interval = 60, 
        int maxDegreeOfParallelism = 20,
        string timeFrom = "",
        string timeTo = "")
    {
        this._logger = logger;
        this._configuration = configuration;
        this._serviceScopeFactory = serviceScopeFactory;
        this._interval = interval;
        this._maxDegreeOfParallelism = maxDegreeOfParallelism;
        
        _useTimeZone = timeFrom.xIsNotEmpty() && timeTo.xIsNotEmpty();
        if (_useTimeZone)
        {
            this._from = DateTime
                .ParseExact($"{DateTime.Now.ToString("YYMMdd")}{timeFrom}", _dateFormat, CultureInfo.InvariantCulture)
                .TimeOfDay;
            this._to = DateTime
                .ParseExact($"{DateTime.Now.ToString("YYMMdd")}{timeTo}", _dateFormat, CultureInfo.InvariantCulture)
                .TimeOfDay;               
        }
    }
    
    /// <summary>
    /// 실제 처리에 사용될 데이터를 사전에 조회 한다.
    /// </summary>
    /// <param name="stopingToken"></param>
    /// <returns></returns>
    protected abstract Task<IEnumerable<TEntity>> OnProducerAsync(CancellationToken stopingToken);
    
    /// <summary>
    /// 조회된 데이터를 기반으로 동작할 코드를 작성한다.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="stopingToken"></param>
    /// <returns></returns>
    protected abstract Task OnConsumerAsync(TEntity item, CancellationToken stopingToken);

    /// <summary>
    /// 모든 처리가 종료된 후 실행 됩니다.
    /// </summary>
    /// <param name="items"></param>
    /// <param name="stopingToken"></param>
    /// <returns></returns>
    protected abstract Task OnFinishAsync(IEnumerable<TEntity> items, CancellationToken stopingToken);

    /// <summary>
    /// 메인 실행 프로세스
    /// </summary>
    /// <param name="stoppingToken"></param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_useTimeZone)
                {
                    var nowTime = DateTime.Now.TimeOfDay;
                    if (nowTime.xBetween(_from, _to))
                    {
                        this._logger.LogInformation($"ExecuteProducerAsync start");
                        var items = await this.OnProducerAsync(stoppingToken);
                        this._logger.LogInformation($"ExecuteProducerAsync end");

                        this._logger.LogInformation($"Parallel.ForEachAsync start");
                        await Parallel.ForEachAsync(items,
                            new ParallelOptions() {MaxDegreeOfParallelism = _maxDegreeOfParallelism}, async (producer, token) =>
                            {
                                this._logger.LogInformation($"ExecuteConsumerAsync start");
                                await this.OnConsumerAsync(producer, token);
                                this._logger.LogInformation($"ExecuteConsumerAsync end");
                            });
                        this._logger.LogInformation($"Parallel.ForEachAsync end");

                        this._logger.LogInformation($"OnFinishAsync start");
                        await this.OnFinishAsync(items, stoppingToken);
                        this._logger.LogInformation($"OnFinishAsync end");
                    }
                    else
                    {
                        this._logger.LogTrace($"{_from.ToString()} {_to.ToString()} not execution time");
                    }
                }
            }
            catch (Exception e)
            {
                this._logger.LogError(e, e.Message);
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}