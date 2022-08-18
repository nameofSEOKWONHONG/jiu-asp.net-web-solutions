using Application.Base;
using Application.Infrastructure.Message;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace MailSendWorkerService;

/// <summary>
/// TODO : Attachment 파일에 대한 Anti-virus 처리 구현해보자.
/// </summary>
public class MailSendWorker : ParallelBackgroundServiceBase<MailFileDto>
{
    private readonly MailFormReader _mailFormReader;
    private readonly INotifyMessageProvider _notifyMessageProvider;
    public MailSendWorker(ILogger logger, 
        IConfiguration configuration, 
        IServiceScopeFactory serviceScopeFactory,
        MailFormReader reader,
        MessageProviderResolver messageProviderResolver,
        int interval = 60, 
        int maxDegreeOfParallelism = 20) : base(logger, configuration, serviceScopeFactory, interval, maxDegreeOfParallelism)
    {
        _mailFormReader = reader;
        _notifyMessageProvider = messageProviderResolver(ENUM_NOTIFY_MESSAGE_TYPE.EMAIL);
    }

    protected override async Task<IEnumerable<MailFileDto>> OnProducerAsync(CancellationToken stopingToken)
    {
        return await Task.Factory.StartNew(() => _mailFormReader.GetMailFiles());
    }

    protected override async Task OnConsumerAsync(MailFileDto item, CancellationToken stopingToken)
    {
        await _notifyMessageProvider.SendMessageAsync(new EmailNotifyMessageRequest(
            item.MailFormItem.Receivers,
            item.MailFormItem.Title,
            item.MailFormItem.Contents,
            item.MailFormItem.Files));
    }

    protected override Task OnFinishAsync(IEnumerable<MailFileDto> items, CancellationToken stopingToken)
    {
        _mailFormReader.RemoveMailFiles(items);
        return Task.CompletedTask;
    }
}