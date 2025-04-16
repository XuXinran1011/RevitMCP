using RevitMCP.Shared.Models.QuantityTakeoff;
using System.Collections.Generic;

namespace RevitMCP.Shared.Interfaces.QuantityTakeoff
{
    /// <summary>
    /// 工程量导出提供者接口，定义工程量数据导出的基础契约。
    /// </summary>
    public interface IQuantityExportProvider
    {
        /// <summary>
        /// 导出工程量数据到指定格式。
        /// </summary>
        /// <param name="quantities">工程量信息集合</param>
        /// <param name="format">导出格式（如Excel、CSV等）</param>
        /// <param name="options">导出选项（可选）</param>
        /// <returns>导出文件路径或内容</returns>
        string Export(IEnumerable<QuantityInfo> quantities, string format, object? options = null);
    }
} 