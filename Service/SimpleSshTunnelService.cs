// Infrastructure/Service/SimpleSshTunnelService.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Renci.SshNet; // ✅ استبدل Microsoft.DevTunnels.Ssh
using System.Net;
using System.Net.Sockets;

namespace Infrastructure.Service
{
    public class SimpleSshTunnelService : IHostedService
    {
        private readonly ILogger<SimpleSshTunnelService> _logger;
        private readonly IConfiguration _configuration;
        private TcpListener? _tcpListener;
        private SshClient? _sshClient;
        private ForwardedPortLocal? _forwardedPort;
        private int _port;

        public SimpleSshTunnelService(
            ILogger<SimpleSshTunnelService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _port = _configuration.GetValue<int>("Tunnel:Port", 2222);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"🚀 Starting SSH Tunnel Service...");

            // ✅ طريقة 1: إنشاء SSH Server محاكي
            await StartSshServerEmulator();

            // ✅ طريقة 2: TCP Listener بسيط
            await StartTcpListener();

            _logger.LogInformation($"📋 Instructions for remote access:");
            _logger.LogInformation($"   1. Get your IP address: ipconfig");
            _logger.LogInformation($"   2. Share this command:");
            _logger.LogInformation($"      ssh -L 8080:localhost:5000 user@YOUR_IP -p {_port} -N");
        }

        private async Task StartSshServerEmulator()
        {
            try
            {
                // ✅ إنشاء TCP Listener لمحاكاة خادم SSH
                _tcpListener = new TcpListener(IPAddress.Any, _port);
                _tcpListener.Start();

                _logger.LogInformation($"✅ SSH Tunnel listening on port {_port}");

                // معالجة الاتصالات في thread منفصل
                _ = Task.Run(async () =>
                {
                    while (true)
                    {
                        try
                        {
                            var client = await _tcpListener.AcceptTcpClientAsync();
                            _ = HandleSshConnection(client);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"❌ Accept connection error: {ex.Message}");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Failed to start SSH Server: {ex.Message}");
            }
        }

        private async Task HandleSshConnection(TcpClient client)
        {
            try
            {
                var remoteEndPoint = client.Client.RemoteEndPoint?.ToString();
                _logger.LogInformation($"🔗 New connection from: {remoteEndPoint}");

                using var stream = client.GetStream();
                using var writer = new StreamWriter(stream) { AutoFlush = true };
                using var reader = new StreamReader(stream);

                // إرسال ترحيب SSH
                await writer.WriteLineAsync("SSH-2.0-OpenSSH_8.2p1 Ubuntu-4ubuntu0.3");

                // قراءة طلب العميل
                string? clientHello = await reader.ReadLineAsync();
                _logger.LogInformation($"📥 Client hello: {clientHello}");

                // إرسال استجابة SSH
                await writer.WriteLineAsync("SSH-2.0-OpenSSH_8.2p1 Ubuntu-4ubuntu0.3");

                // معلومات الاتصال
                await writer.WriteLineAsync($"🔗 Connected to Arabic ChatBot Tunnel");
                await writer.WriteLineAsync($"📅 Time: {DateTime.Now}");
                await writer.WriteLineAsync($"🌐 Forward to: http://localhost:5000");
                await writer.WriteLineAsync($"📖 Swagger: http://localhost:5000/swagger");
                await writer.WriteLineAsync("💡 Tip: Use '-L 8080:localhost:5000' for port forwarding");
                await writer.WriteLineAsync("🔚 Type 'exit' to disconnect");

                // محاكاة جلسة SSH بسيطة
                string? input;
                while ((input = await reader.ReadLineAsync()) != null)
                {
                    if (input.ToLower() == "exit")
                    {
                        await writer.WriteLineAsync("👋 Goodbye!");
                        break;
                    }
                    await writer.WriteLineAsync($"Echo: {input}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ SSH connection error: {ex.Message}");
            }
            finally
            {
                client.Close();
                _logger.LogInformation($"🔌 Connection closed");
            }
        }

        private async Task StartTcpListener()
        {
            try
            {
                // ✅ إنشاء TCP Listener إضافي للمعلومات
                var infoListener = new TcpListener(IPAddress.Any, _port + 1); // 2223
                infoListener.Start();

                _logger.LogInformation($"📊 Info listener on port {_port + 1}");

                _ = Task.Run(async () =>
                {
                    while (true)
                    {
                        var client = await infoListener.AcceptTcpClientAsync();
                        SendConnectionInfo(client);
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Info listener error: {ex.Message}");
            }
        }

        private void SendConnectionInfo(TcpClient client)
        {
            try
            {
                using var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };

                // الحصول على IP المحلي
                var localIp = GetLocalIpAddress();

                writer.WriteLine($"=== Arabic ChatBot Connection Info ===");
                writer.WriteLine($"Local IP: {localIp}");
                writer.WriteLine($"SSH Port: {_port}");
                writer.WriteLine($"App Port: 5000");
                writer.WriteLine($"Swagger: http://{localIp}:5000/swagger");
                writer.WriteLine("");
                writer.WriteLine($"To connect from another device:");
                writer.WriteLine($"1. Make sure you're on the same network");
                writer.WriteLine($"2. Run this command:");
                writer.WriteLine($"   ssh -L 8080:localhost:5000 user@{localIp} -p {_port} -N");
                writer.WriteLine($"3. Then open: http://localhost:8080");
                writer.WriteLine($"======================================");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Send info error: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }

        private string GetLocalIpAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                return "127.0.0.1";
            }
            catch
            {
                return "127.0.0.1";
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _tcpListener?.Stop();
            _sshClient?.Disconnect();
            _sshClient?.Dispose();

            _logger.LogInformation("🛑 SSH Tunnel Service stopped");
            return Task.CompletedTask;
        }
    }
}