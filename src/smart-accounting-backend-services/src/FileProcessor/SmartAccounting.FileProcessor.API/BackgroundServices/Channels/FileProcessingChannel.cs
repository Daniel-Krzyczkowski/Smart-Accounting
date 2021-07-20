using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SmartAccounting.FileProcessor.API.BackgroundServices.Channels
{
    public record FileProcessingChannelElement
    {
        public string FilePath { get; init; }
        public string UserId { get; init; }
    }

    public class FileProcessingChannel
    {
        private const int MaxMessagesInChannel = 100;

        private readonly Channel<FileProcessingChannelElement> _channel;
        private readonly ILogger<FileProcessingChannel> _logger;

        public FileProcessingChannel(ILogger<FileProcessingChannel> logger)
        {
            var options = new BoundedChannelOptions(MaxMessagesInChannel)
            {
                SingleWriter = false,
                SingleReader = true
            };

            _channel = Channel.CreateBounded<FileProcessingChannelElement>(options);

            _logger = logger;
        }

        public async Task<bool> AddFileAsync(FileProcessingChannelElement fileProcessingChannelElement, CancellationToken ct = default)
        {
            while (await _channel.Writer.WaitToWriteAsync(ct) && !ct.IsCancellationRequested)
            {
                if (_channel.Writer.TryWrite(fileProcessingChannelElement))
                {
                    Log.ChannelMessageWritten(_logger, fileProcessingChannelElement.FilePath);

                    return true;
                }
            }

            return false;
        }

        public IAsyncEnumerable<FileProcessingChannelElement> ReadAllAsync(CancellationToken ct = default) =>
            _channel.Reader.ReadAllAsync(ct);

        public bool TryCompleteWriter(Exception ex = null) => _channel.Writer.TryComplete(ex);

        internal static class EventIds
        {
            public static readonly EventId ChannelMessageWritten = new EventId(100, "ChannelMessageWritten");
        }

        private static class Log
        {
            private static readonly Action<ILogger, string, Exception> _channelMessageWritten = LoggerMessage.Define<string>(
                LogLevel.Debug,
                EventIds.ChannelMessageWritten,
                "Filename {FileName} was written to the channel.");

            public static void ChannelMessageWritten(ILogger logger, string fileName)
            {
                _channelMessageWritten(logger, fileName, null);
            }
        }
    }
}
