using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DGruppensuizidBot.AlphabetThread;
using Discord;
using Discord.WebSocket;

namespace BelegtesBrot.Channels
{
    internal class AlphabetMode:IServerMessageChannel
    {
        public ChannelType ChannelType { get; }
        public string Name { get; }
        public IGuildChannel Channel { get; }
        public void MessageReceived(IMessage message)
        {
            throw new NotImplementedException();
        }

        public void MessageUpdated(Cacheable<IMessage, ulong> previousMessage, IMessage currentMessage, ISocketMessageChannel channel)
        {
            throw new NotImplementedException();
        }

        public void MessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessage, ulong> message1)
        {
            throw new NotImplementedException();
        }

        public ulong Id { get; }
        public DateTimeOffset CreatedAt { get; }
        public IAsyncEnumerable<IReadOnlyCollection<IUser>> GetUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }


    }
}
