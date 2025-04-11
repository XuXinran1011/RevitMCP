using System;
using System.Collections.Generic;

namespace RevitMCP.Shared.Interfaces
{
    /// <summary>
    /// Revit元素接口
    /// </summary>
    public interface IRevitElement
    {
        /// <summary>
        /// 元素ID
        /// </summary>
        int Id { get; }
        
        /// <summary>
        /// 元素名称
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// 元素类别
        /// </summary>
        string Category { get; }
        
        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数值</returns>
        object GetParameterValue(string parameterName);
        
        /// <summary>
        /// 设置参数值
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="value">参数值</param>
        /// <returns>是否设置成功</returns>
        bool SetParameterValue(string parameterName, object value);
        
        /// <summary>
        /// 获取所有参数
        /// </summary>
        /// <returns>参数字典</returns>
        Dictionary<string, object> GetParameters();
    }
}
