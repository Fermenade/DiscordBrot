using Discord;
using System.Collections.Concurrent;

namespace DGruppensuizidBot.Discord
{
    internal class Delete
    {
        private bool _isProcessingQueue; // damit wenn noch in arbeit weiter gearbeitet wird
        private ConcurrentQueue<Cacheable<IMessage, ulong>> _messageQueue = new(); //Interfaces :)
        private async Task MessageDeleted(Cacheable<IMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> channel)
        {
            //if (channel.Id == _ThreadAlphabetBack)
            {
                // Enqueue the message
                _messageQueue.Enqueue(cachedMessage); //enqhene

                // Start processing if not already processing
                if (!_isProcessingQueue) //wenn nicht dann doch
                {
                    _isProcessingQueue = true;
                    await ProcessMessageQueue(channel); //asnyc
                    _isProcessingQueue = false;
                }
            }
        }

        public delegate void DeletedMessageProcessDelegate(IMessage message);
        public DeletedMessageProcessDelegate DeletedChannelProcess;
        private async Task ProcessMessageQueue(Cacheable<IMessageChannel, ulong> channel)
        {
            while (_messageQueue.TryDequeue(out var cachedMessage))
            {
                DeletedChannelProcess.Invoke(cachedMessage.Value);

                await Task.Delay(0); //await zerrrrrooooooo!
            }
        }
    }
}
