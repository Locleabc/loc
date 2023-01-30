using log4net;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TopCom.Define;
using TopCom.MES;
using TopCom.Models;

namespace TopCom.Mqtt
{
    public class MqttClient : ILoggable
    {
        #region Properties
        public string Host { get; set; } = Config.Host;

        public uint Port { get; set; } = Config.Port;

        public string ClientID
        {
            get
            {
                CMESInfo mesInfo = TopCom.MES.CMESInfo.Load();
                if (string.IsNullOrEmpty(mesInfo.Head2.EQPName))
                {
                    return mesInfo.Head1.EQPName;
                }
                else
                {
                    return mesInfo.Head1.EQPName + " - " + mesInfo.Head2.EQPName;
                }
            }
        }

        public string Username { get; set; } = Config.Username;

        public string Password { get; set; } = Config.Password;

        public bool Tls { get; set; } = Config.Tls;

        public bool IsConnected
        {
            get
            {
                return mqttClient.IsConnected;
            }
        }

        public ILog Log { get; protected set; }
        #endregion

        #region Constructors
        public MqttClient()
        {
            Log = LogManager.GetLogger("MQTT");
            LogFactory.Configure();
        }

        ~MqttClient()
        {
        }

        public void Initialize()
        {
            mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(msg => OnMqttConnected(msg));
            mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(msg => OnClientDisconnected(msg));
            mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(msg => OnSubscriberMessageReceived(msg)); 

            var v = ConnectAsync(true);
        }

        public void Terminate()
        {
#if !SIMULATION
            var v = PublishAsync(Topics.EquipPing, new AliveMessage { Status = false });
#endif
            var t = DisconnectAsync();
        }
        #endregion

        private void OnClientDisconnected(MqttClientDisconnectedEventArgs msg)
        {
            Log.Debug($"Disconnected to host {Host}");
#if !SIMULATION
#endif
        }

        private void OnSubscriberMessageReceived(MqttApplicationMessageReceivedEventArgs msg)
        {

        }

        private void OnMqttConnected(MqttClientConnectedEventArgs msg)
        {
            // Log.Debug($"Connect successed! to host {Host}");
#if !SIMULATION
            var v = PublishAsync(Topics.EquipPing, new AliveMessage { Status = true });
#endif
        }

        public async Task ConnectAsync(bool forceReconnect = false)
        {
            var options = new MqttClientOptionsBuilder()
                        .WithClientId(ClientID)
                        .WithCredentials(Username, Password)
                        .WithTcpServer(Host, (int)Port)
                        .Build();

            if (Tls)
            {
                options = new MqttClientOptionsBuilder()
                    .WithClientId(ClientID)
                    .WithCredentials(Username, Password)
                    .WithTls()
                    .WithTcpServer(Host, (int)Port)
                    .Build();
            }

            if (IsConnected)
            {
                Log.Debug("Client connected already!");
                if (!forceReconnect)
                {
                    return;
                }
                await DisconnectAsync();
            }

            try
            {
                // Log.Debug($"Connectting to host {Host}");
                await mqttClient.ConnectAsync(options, CancellationToken.None); // Since 3.0.5 with CancellationToken
            }
            catch (Exception ex)
            {
                Log.Info($"CONNECT FAILED! {ex.Message}");
            }
        }

        public async Task PublishAsync(string topic, object content)
        {
            if (IsConnected == false)
            {
                await ConnectAsync();
            }

            if (IsConnected == false)
            {
                return;
            }

            string message = "";
            if (content.GetType() == typeof(string))
            {
                message = (string)content;
            }
            else
            {
                message = Newtonsoft.Json.JsonConvert.SerializeObject(content);
            }

            try
            {
                var msg = new MqttApplicationMessageBuilder()
                        .WithPayload(message)
                        .WithTopic(topic)
                        .Build();

                await mqttClient.PublishAsync(msg);
            }
            catch (Exception ex)
            {
                Log.Info(ex.Message);
            }
        }

        public async Task SubscribeAsync(string topic)
        {
            if (IsConnected == false)
            {
                await ConnectAsync();
            }

            if (IsConnected == false)
            {
                return;
            }

            try
            {
                await mqttClient.SubscribeAsync(topic);
            }
            catch (Exception ex)
            {
                Log.Info(ex.Message);
            }
        }

        public async Task UnsubscribeAsync(string topic)
        {
            if (IsConnected == false)
            {
                await ConnectAsync();
            }

            if (IsConnected == false)
            {
                return;
            }

            try
            {
                await mqttClient.UnsubscribeAsync(topic);
            }
            catch (Exception ex)
            {
                Log.Info(ex.Message);
            }
        }

        public async Task DisconnectAsync()
        {
            if (IsConnected == false)
            {
                Log.Debug("Client disconnected already!");
                return;
            }

            try
            {
                await mqttClient.DisconnectAsync();
                Log.Debug("Client disconnected!");
            }
            catch (Exception ex)
            {
                Log.Info(ex.Message);
            }
        }

        #region Privates
        static readonly MqttFactory factory = new MqttFactory();
        static readonly IMqttClient mqttClient = factory.CreateMqttClient();

        private string _ClientID = Config.ClientID;
        #endregion
    }
}
