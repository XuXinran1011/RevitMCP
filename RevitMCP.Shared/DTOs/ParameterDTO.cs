namespace RevitMCP.Shared.DTOs
{
    /// <summary>
    /// 族参数传输对象（DTO），用于跨进程或网络传输族参数信息。
    /// </summary>
    public class ParameterDTO
    {
        /// <summary>参数名称</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>参数类型（如number、string等）</summary>
        public string Type { get; set; } = string.Empty;
        /// <summary>参数单位（如mm、m等）</summary>
        public string? Unit { get; set; }
        /// <summary>是否必填</summary>
        public bool Required { get; set; }
        /// <summary>参数描述</summary>
        public string? Description { get; set; }
        /// <summary>参数默认值（可选）</summary>
        public object? DefaultValue { get; set; }
    }
} 