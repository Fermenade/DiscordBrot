using Discord;
using Discord.WebSocket;
using System.Runtime.InteropServices;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BelegtesBrot
{
    internal interface IServer : IBaseCom
    {
        public IGuild Guild { get; }
        public List<IServerMessageChannel> MessageChannels { get; }


    }

    internal class Server : IServer
    {
        public IGuild Guild => guild;

        private readonly IGuild guild;
        public List<IServerMessageChannel> MessageChannels => new();

        public Server(IGuild guild)
        {
            this.guild = guild;

        }

        private void ReadConnectedChannels()
        {
            string serverGuildFile = $"{Guild.Id}.json";
            if (!File.Exists(serverGuildFile)) return;
            string e = File.ReadAllText(serverGuildFile);
            List<LinkedChannels> x = JsonSerializer.Deserialize<List<LinkedChannels>>(e);

            foreach (LinkedChannels linkedChannel in x)
            {
                Type loadedType = Type.GetType(linkedChannel.Channel, throwOnError: true);
                if (loadedType == null)
                {
                    throw new Exception($"Unknown type: {linkedChannel.Channel}");
                }

                // Using a constructor with arguments
                IServerMessageChannel instance = (IServerMessageChannel)Activator.CreateInstance(
                    loadedType,
                    new { linkedChannel.ChannelId }); // arguments must match a ctor signature
                MessageChannels.Add(instance);
            }

        }

        void AddCannel(SocketTextChannel channel, IServerMessageChannel messageChannel) //TODO: link this logic with a register command
        {
            LinkedChannels linkedChannels = new LinkedChannels(channel, messageChannel);
        }

        public Task MessageReceived(IMessage message)
        {
            foreach (IServerMessageChannel VARIABLE in MessageChannels)
            {
                if (VARIABLE.Channel.Id == message.Channel.Id)
                {
                    return VARIABLE.MessageReceived(message);
                }
            }

            return Task.CompletedTask;
        }

        public static T ByteToType<T>(BinaryReader reader) // TODO: Look at me
        {
            byte[] bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));

            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return theStructure;
        }

        static T ReadStructFromBinaryFile<T>(string filePath) where T : struct // TODO: Look at me
        {
            byte[] bytes = File.ReadAllBytes(filePath);

            IntPtr ptr = Marshal.AllocHGlobal(bytes.Length);
            try
            {
                Marshal.Copy(bytes, 0, ptr, bytes.Length);
                return Marshal.PtrToStructure<T>(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        static void WriteStructToBinaryFile<T>(T structure, string filePath) where T : struct // TODO: Look at me
        {
            int size = Marshal.SizeOf(structure);
            byte[] bytes = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structure, ptr, true);
                Marshal.Copy(ptr, bytes, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            File.WriteAllBytes(filePath, bytes);
        }

        public Task MessageUpdated(Cacheable<IMessage, ulong> previousMessage, IMessage currentMessage, ISocketMessageChannel channel)
        {
            foreach (IServerMessageChannel VARIABLE in MessageChannels)
            {
                if (VARIABLE.Channel.Id == channel.Id)
                {
                    return VARIABLE.MessageUpdated(previousMessage, currentMessage, channel);
                }
            }
            return Task.CompletedTask;
        }
        public Task MessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
        {
            foreach (IServerMessageChannel VARIABLE in MessageChannels)
            {
                if (VARIABLE.Channel.Id == channel.Value.Id)
                {
                    return VARIABLE.MessageDeleted(message, channel);
                }
            }
            return Task.CompletedTask;
        }
    }
}
