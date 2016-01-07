using LibFaolan.Config;
using LibFaolan.Data;
using LibFaolan.Network;

namespace UniverseAgent
{
    partial class UniverseAgentListener
    {
        private static readonly byte[] Sender = {0x0d, 0xa0, 0xdb, 0x4d, 0x60, 0x10, 0x01};
        private static readonly byte[] Receiver = {0x0d, 0x54, 0x40, 0x38, 0x0c, 0x10, 0xdc, 0xc8, 0x49};
        private static readonly byte[] Receiver2 = {0x0d, 0x54, 0x40, 0x38, 0x0c, 0x10, 0xec, 0xeb, 0x80, 0xde, 0x03};

        public static void InitAuth(NetworkClient client, Account account)
        {
            new PacketStream()
                .WriteHeader(Sender, Receiver, null, RespondseOpcodes.InitiateAuthentication)
                .WriteString(Account.AuthChallenge)
                .Send(client);
        }

        public static void SetRegionState(NetworkClient client, Account account)
        {
            new PacketStream()
                .WriteHeader(Sender, Receiver2, null, RespondseOpcodes.SetRegion)
                .WriteArray
                (
                    0x01, 0x01, 0x01, 0x01,
                    0x3f, 0x80,
                    0x00, 0x00,
                    0x3f, 0x80,
                    0x00, 0x00, 0x01, 0x01,
                    0x00
                )
                .Send(client);
        }

        public static void SendPlayerAgentRealm(NetworkClient client, Account account)
        {
            new PacketStream()
                .WriteHeader(Sender, Receiver2, null, RespondseOpcodes.SendPlayerAgentRealm)
                .WriteUInt32(0x01) // authstatus
                .WriteUInt64(account.LongId)
                .WriteString(Settings.PlayerAgentAddress + ":" + Settings.PlayerAgentPort)
                .WriteUInt32(0x310cec57)
                .WriteUInt32(0x00000000)
                .Send(client);
        }
    }
}