namespace RevitMCP.Shared.Interfaces.QuantityTakeoff
{
    /// <summary>
    /// 共享工程量计算器接口，定义跨端工程量计算的基础契约。
    /// </summary>
    public interface ISharedQuantityCalculator
    {
        /// <summary>
        /// 计算指定元素的工程量。
        /// </summary>
        /// <param name="element">待计算的元素</param>
        /// <param name="method">计算方法（可选）</param>
        /// <returns>工程量信息对象</returns>
        QuantityInfo CalculateQuantity(IRevitElement element, string? method = null);

        /// <summary>
        /// 批量计算多个元素的工程量。
        /// </summary>
        /// <param name="elements">元素集合</param>
        /// <param name="method">计算方法（可选）</param>
        /// <returns>工程量信息集合</returns>
        IEnumerable<QuantityInfo> CalculateQuantities(IEnumerable<IRevitElement> elements, string? method = null);
    }
} 