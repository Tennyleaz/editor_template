using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Controls;

namespace editor_template
{
    /// <summary>
    /// Client class
    /// </summary>
    public class MyTcpClient : IDisposable
    {
        /// <summary>
        /// 連線至主機
        /// </summary>
        private TcpClient tcpClient;
        private NetworkStream nStream;
        public bool isConnected = false;

        public async Task Connect(IPAddress ipa, int port)
        {
            Task task;
            var timeOut = TimeSpan.FromSeconds(5);
            var cancellationCompletionSource = new TaskCompletionSource<bool>();
            try
            {
                using(var cts = new CancellationTokenSource(timeOut))
                {
                    using(tcpClient = new TcpClient())
                    {
                        task = tcpClient.ConnectAsync(ipa, port);

                        using(cts.Token.Register(() => cancellationCompletionSource.TrySetResult(true)))
                        {
                            if (task != await Task.WhenAny(task, cancellationCompletionSource.Task))
                            {
                                throw new OperationCanceledException(cts.Token);
                            }
                        }
                        nStream = tcpClient.GetStream();
                        nStream.ReadTimeout = 5000;
                        isConnected = true;
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine(ex);
                nStream = null;
                isConnected = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                nStream = null;
                isConnected = false;
            }
        }
        public void Close()
        {
            tcpClient?.Close();
            tcpClient = null;
            nStream = null;
            isConnected = false;
        }

        public void Send(byte[] databuffer, int index, int length)
        {
            if (nStream != null)
            {
                nStream.Write(databuffer, index, length);
            }
        }

        public int Receive(byte[] datareceive, int offset, int length)
        {
            int received_length = 0;
            try
            {
                Array.Clear(datareceive, offset, length);
                received_length = nStream.Read(datareceive, offset, length);
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return received_length;
        }

        public void Dispose()
        {
            tcpClient?.Close();
            tcpClient?.Dispose();
            nStream?.Close(0);
            nStream?.Dispose();
            isConnected = false;
        }
    }
}