using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application.Base;

public abstract class ParallelBackgroundServiceBase<TEntity> : BackgroundServiceBase
    where TEntity : class
{
    private int _maxDegreeOfParallelism;
    
    protected ParallelBackgroundServiceBase(ILogger logger,
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory,
        int interval = 60, 
        int maxDegreeOfParallelism = 20) : base(logger, configuration, serviceScopeFactory, interval)
    {
        this._maxDegreeOfParallelism = maxDegreeOfParallelism;
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
            catch (Exception e)
            {
                this._logger.LogError(e, e.Message);
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return base.StopAsync(cancellationToken);
    }
}