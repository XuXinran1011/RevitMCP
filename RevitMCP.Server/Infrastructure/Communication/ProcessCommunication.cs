using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using RevitMCP.Shared.Communication;

namespace RevitMCP.Server.Infrastructure.Communication
{
    /// <summary>
    /// 负责监听标准输入，处理QueryMessage并返回ResponseMessage。
    /// </summary>
    public class ProcessCommunication
    {
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;

        public ProcessCommunication(Stream input, Stream output)
        {
            _reader = new StreamReader(input);
            _writer = new StreamWriter(output) { AutoFlush = true };
        }

        /// <summary>
        /// 启动监听循环，收到QueryMessage后返回ResponseMessage。
        /// </summary>
        public async Task ListenAsync()
        {
            while (true)
            {
                string line = await _reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line)) continue;
                QueryMessage? query = JsonSerializer.Deserialize<QueryMessage>(line);
                if (query == null)
                {
                    // TODO: 可记录日志或返回错误响应
                    continue;
                }
                var response = HandleQuery(query);
                string respJson = JsonSerializer.Serialize(response);
                await _writer.WriteLineAsync(respJson);
            }
        }

        /// <summary>
        /// 简单处理QueryMessage，Ping-Pong演示。
        /// </summary>
        private ResponseMessage HandleQuery(QueryMessage query)
        {
            if (query.QueryText?.Trim().ToLower() == "ping")
            {
                return new ResponseMessage
                {
                    MessageType = IPCProtocol.MessageTypeResponse,
                    RequestId = query.RequestId,
                    Success = true,
                    Message = "Pong",
                    Data = null
                };
            }
            // 其它命令可扩展
            return new ResponseMessage
            {
                MessageType = IPCProtocol.MessageTypeResponse,
                RequestId = query.RequestId,
                Success = false,
                Message = "未知命令",
                Data = null
            };
        }
    }
} 