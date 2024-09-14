using System.Net;
using System.Text;
using Sirenix.OdinInspector;
using YuoTools;
using YuoTools.Extend.Helper;
using YuoTools.Extend.Helper.Network;
using YuoTools.Main.Ecs;

namespace ET
{
    [AutoAddToMain]
    public class NetworkTest : YuoComponentGet<NetworkTest>
    {
        [ShowInInspector] public AService AService { get; set; }

        public void OnAccept(long channelId, IPEndPoint ipEndPoint)
        {
            $"OnAccept:{channelId} {ipEndPoint}".Log();
        }

        public void OnRead(long channelId, MemoryBuffer bytes)
        {
            var message = Encoding.UTF8.GetString(bytes.ToArray());
            $"OnRead:{channelId}__Message__{message}".Log();
        }

        public void OnError(long channelId, int e)
        {
            $"OnError:{channelId} {e}".Log();
        }
    }

    public class NetworkTestAwakeSystem : YuoSystem<NetworkTest>, IAwake, IExitGame
    {
        protected override void Run(NetworkTest self)
        {
            if (RunType == SystemTagType.Awake)
            {
                self.AService = new KService(NetworkHelper.ToIPEndPoint("127.0.0.1", 921), NetworkProtocol.UDP,
                    ServiceType.Inner);
                self.AService.AcceptCallback = self.OnAccept;
                self.AService.ReadCallback = self.OnRead;
                self.AService.ErrorCallback = self.OnError;
            }

            if (RunType == SystemTagType.ExitGame)
            {
                self.AService.Dispose();
            }
        }
    }

    public class NetworkTestUpdateSystem : YuoSystem<NetworkTest>, IUpdate
    {
        protected override void Run(NetworkTest self)
        {
            self.AService.Update();
        }
    }
}