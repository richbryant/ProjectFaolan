using System;
using System.Windows.Forms;
using AgentServer;
using CSPlayerAgent;
using GameServer;
using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.Scs.Communication.Protocols;
using LibFaolan.Config;
using LibFaolan.Database;
using LibFaolan.Network;
using LibFaolan.Other;
using PlayerAgent;
using UniverseAgent;

namespace ProjectFaolan
{
    public partial class Form1 : Form
    {
#if DEBUG
        private const string ConfigPath = "../../other/faolan.ini";
#else
        private const string ConfigPath="faolan.ini";
#endif

        private readonly Logger _logger;
        private readonly ConsoleStream _consoleStream;

        public Form1()
        {
            InitializeComponent();

            _consoleStream = new ConsoleStream(richTextBox);
            _logger = GetLogger(null);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if(Statics.GameServer != null)
                Statics.GameServer.SaveAllCharacterData();

            base.OnFormClosing(e);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            _logger.WriteLine(LibFaolan.Other.Statics.BuildInfo);

            if (Settings.Load(ConfigPath))
                _logger.WriteLine("Loaded settings from: '" + ConfigPath + "'");
            else
            {
                _logger.WriteLine("Failed to load settings from: '" + ConfigPath + "'");
            }
        }

        private void databaseConnectButton_Click(object sender, EventArgs e)
        {
            if (Settings.UseMysql)
            {
                Statics.Database = new MySqlDatabase(Settings.MySqlAddress, Settings.MySqlPort, Settings.MySqlDatabase,
                    Settings.MySqlUsername, Settings.MySqlPassword);
            }
            else if(Settings.UseSqLite)
                Statics.Database = new SqLiteDatabase(Settings.SqLitePath);

            if (Statics.Database.Connect())
                _logger.WriteLine("Connected to database");
            else
                _logger.WriteLine("Failed to connect to database");
        }

        private Logger GetLogger(string tag) => new Logger(tag) {TextWriter = _consoleStream};

        private void universeAgentButton_Click(object sender, EventArgs e)
        {
            var logger = GetLogger("[UniverseAgent] ");
            Statics.UniverseAgent = new UniverseAgentListener(Settings.UniverseAgentPort, logger, Statics.Database);
            StartServer(Statics.UniverseAgent);
        }

        private void playerAgentButton_Click(object sender, EventArgs e)
        {
            var logger = GetLogger("[PlayerAgent] ");
            Statics.PlayerAgent = new PlayerAgentListener(Settings.PlayerAgentPort, logger, Statics.Database);
            StartServer(Statics.PlayerAgent);
        }

        private void csPlayerAgentButton_Click(object sender, EventArgs e)
        {
            var logger = GetLogger("[CsPlayerAgent] ");
            Statics.CsplayerAgent = new CsPlayerAgentListener(Settings.CsPlayerAgentPort, logger, Statics.Database);
            StartServer(Statics.CsplayerAgent);
        }

        private void agentServerButton_Click(object sender, EventArgs e)
        {
            var logger = GetLogger("[AgentServer] ");
            Statics.AgentServer = new AgentServerListener(Settings.AgentServerPort, logger, Statics.Database);
            StartServer(Statics.AgentServer);
        }

        private void gameServerButton_Click(object sender, EventArgs e)
        {
            var logger = GetLogger("[GameServer] ");
            Statics.GameServer = new GameServerListener(Settings.GameServerPort, logger, Statics.Database,
                Statics.AgentServer);
            StartServer(Statics.GameServer);
        }

        private bool StartServer<TPacketType, TWireProtocolFactory>(Server<TPacketType, TWireProtocolFactory> server)
            where TPacketType : IScsMessage
            where TWireProtocolFactory : IScsWireProtocolFactory, new()
        {
            if (server.Start())
            {
                server.Logger.WriteLine("Started on port: " + server.Port);
                return true;
            }

            server.Logger.WriteLine("Failed to start on port: " + server.Port);
            return false;
        }
    }
}