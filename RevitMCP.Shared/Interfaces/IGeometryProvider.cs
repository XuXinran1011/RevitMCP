namespace RevitMCP.Shared.Interfaces
{
    /// <summary>
    /// 提供几何数据访问的基础接口，适用于跨端几何信息传递。
    /// </summary>
    public interface IGeometryProvider
    {
        /// <summary>
        /// 获取几何类型（如点、线、面、体等）。
        /// </summary>
        string GeometryType { get; }

        /// <summary>
        /// 获取几何数据的序列化表示（如JSON、WKT等）。
        /// </summary>
        string SerializedGeometry { get; }

        /// <summary>
        /// 获取几何的空间参考信息（如坐标系，可选）。
        /// </summary>
        string? SpatialReference { get; }
    }
} 