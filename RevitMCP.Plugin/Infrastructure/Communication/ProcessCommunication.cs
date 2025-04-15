using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RevitMCP.Shared.Communication;

namespace RevitMCP.Plugin.Infrastructure.Communication
{
    /// <summary>
    /// 负责与Server进程进行Stdio通信的基础类。
    /// </summary>
    public class ProcessCommunication
    {
        private readonly Process _serverProcess;
        private readonly StreamWriter _writer;
        private readonly StreamReader _reader;

        public ProcessCommunication(Process serverProcess)
        {
            _serverProcess = serverProcess;
            _writer = serverProcess.StandardInput;
            _reader = serverProcess.StandardOutput;
        }

        /// <summary>
        /// 发送QueryMessage并等待ResponseMessage。
        /// </summary>
        public async Task<ResponseMessage?> SendQueryAsync(QueryMessage query)
        {
            string json = JsonSerializer.Serialize(query);
            await _writer.WriteLineAsync(json);
            await _writer.FlushAsync();

            string? responseLine = await _reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(responseLine))
                return null;
            var response = JsonSerializer.Deserialize<ResponseMessage>(responseLine);
            return response;
        }
    }
} 