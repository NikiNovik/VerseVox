using Lidgren.Network;
using NAudio.Wave;
using System.Windows;
using System.Windows.Input;

namespace VerseVox.Frames
{
    /// <summary>
    /// Логика взаимодействия для VoiceChat.xaml
    /// </summary>
    public partial class VoiceChat : Window
    {
        private WaveOut WaveOut;
        private WaveIn WaveIn;
        private BufferedWaveProvider WaveProvider;

        private bool Connected;

        private NetPeerConfiguration ClientConfiguration;
        private NetClient Client;
        public VoiceChat()
        {
            InitializeComponent();

            ClientConfiguration = new NetPeerConfiguration("Voice");
            Client = new NetClient(ClientConfiguration);
            Client.Start();

            Connected = false;

            WaveOut = new WaveOut();
            WaveIn = new WaveIn();
            WaveIn.WaveFormat = new WaveFormat(44100, 16, 2);
            WaveProvider = new BufferedWaveProvider(WaveIn.WaveFormat);


            WaveIn.DataAvailable += WaveAvailable;

            WaveOut.Init(WaveProvider);

            Client.RegisterReceivedCallback(MessageReceived);
        }

        private void EnterProtoVoiceChat(object sender, MouseButtonEventArgs e)
        {
            if (!Connected)
            {
                ConnectButton.Visibility = Visibility.Hidden;
                ConnectInteractiveButton.Content = "Отсоединиться";
                DisconnectButton.Visibility = Visibility.Visible;
                Client.Connect("176.210.10.35", 27015);
                WaveIn.StartRecording();
                WaveOut.Play();
            }
            else
            {
                DisconnectButton.Visibility = Visibility.Hidden;
                ConnectInteractiveButton.Content = "Присоединиться";
                ConnectButton.Visibility = Visibility.Visible;
                Client.Disconnect("Bye!");
            }

            Connected = !Connected;
        }

        private void WaveAvailable(object Sender, WaveInEventArgs Args)
        {
            NetOutgoingMessage Broadcast = Client.CreateMessage();
            Broadcast.Write(Args.Buffer);
            Client.SendMessage(Broadcast, NetDeliveryMethod.ReliableOrdered);
            Client.FlushSendQueue();
        }

        private void ReceiveBuffer(byte[] Buffer)
        {
            WaveProvider.AddSamples(Buffer, 0, Buffer.Length);
        }

        private void MessageReceived(object Peer)
        {
            NetIncomingMessage Message = Client.ReadMessage();

            if (Message.MessageType == NetIncomingMessageType.Data)
            {
                WaveProvider.AddSamples(Message.ReadBytes(Message.LengthBytes), 0, Message.LengthBytes);
            }
        }
    }
}
