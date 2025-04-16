namespace RevitMCP.Shared.Models.QuantityTakeoff
{
    /// <summary>
    /// 工程量信息模型，描述单个元素的工程量数据，支持序列化和跨端传输。
    /// </summary>
    public class QuantityInfo
    {
        /// <summary>
        /// 工程量唯一标识符。
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 关联的元素ID。
        /// </summary>
        public string ElementId { get; set; } = string.Empty;

        /// <summary>
        /// 工程量类型（如面积、体积、长度等）。
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// 工程量数值。
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// 工程量单位（如m2、m3、mm等）。
        /// </summary>
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// 工程量描述（可选）。
        /// </summary>
        public string? Description { get; set; }
    }
} 