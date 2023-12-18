using Lidgren.Network;
using NAudio.Wave;

namespace VerseVox.Scripts
{
    public partial class VoiceModule
    {
        private WaveOut WaveOut;
        private WaveIn WaveIn;
        private BufferedWaveProvider WaveProvider;

        private NetPeerConfiguration ClientConfiguration;
        private NetClient Client;

        public void ConfigureModule()
        {
            ClientConfiguration = new NetPeerConfiguration("Voice");
            Client = new NetClient(ClientConfiguration);
            Client.Start();

            WaveOut = new WaveOut();
            WaveIn = new WaveIn();
            WaveIn.WaveFormat = new WaveFormat(44100, 16, 2);
            WaveProvider = new BufferedWaveProvider(WaveIn.WaveFormat);
            WaveIn.DataAvailable += WaveAvailable;
            WaveOut.Init(WaveProvider);
            Client.RegisterReceivedCallback(MessageReceived);
        }
        public void ExitProtoVoiceChat()
        {
            ConfigureModule();
            Client.Disconnect("Bye!");
            NonStaticVariables.isConnected = false;
        }
        public void EnterProtoVoiceChat()
        {
            ConfigureModule();
            Client.Connect(GlobalVariables.ServerAddress, 27015);
            WaveIn.StartRecording();
            WaveOut.Play();
            NonStaticVariables.isConnected = true;
        }

        private void WaveAvailable(object Sender, WaveInEventArgs Args)
        {
            NetOutgoingMessage Broadcast = Client.CreateMessage();
            Broadcast.Write(NonStaticVariables.clientChannel); // Include channel information
            Broadcast.Write(Args.Buffer);
            Client.SendMessage(Broadcast, NetDeliveryMethod.ReliableOrdered);
            Client.FlushSendQueue();
        }

        public void ReceiveBuffer(byte[] Buffer)
        {
            WaveProvider.AddSamples(Buffer, 0, Buffer.Length);
        }

        private void MessageReceived(object Peer)
        {
            NetIncomingMessage Message = Client.ReadMessage();
            if (Message != null)
            {
                if (Message.MessageType == NetIncomingMessageType.Data)
                {
                    // Read channel information first
                    int receivedChannel = Message.ReadInt32();
                    int audioDataLength = Message.LengthBytes - sizeof(int); // Subtract the size of the channel information
                    byte[] audioData = Message.ReadBytes(audioDataLength);

                    if (receivedChannel == NonStaticVariables.clientChannel)
                    {
                        this.WaveProvider.AddSamples(audioData, 0, audioData.Length);
                    }
                    else
                    {
                        // Handle messages from other channels if needed
                        //MessageBox.Show($"Received message from channel {receivedChannel}, ignoring");
                    }
                }
            }
        }
    }
}
