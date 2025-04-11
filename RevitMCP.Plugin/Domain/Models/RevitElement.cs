using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitMCP.Shared.Interfaces;

namespace RevitMCP.Plugin.Domain.Models
{
    /// <summary>
    /// Revit元素领域模型
    /// </summary>
    public class RevitElement : IRevitElement
    {
        private readonly Element _element;
        
        /// <summary>
        /// 元素ID
        /// </summary>
        public int Id => _element.Id.IntegerValue;
        
        /// <summary>
        /// 元素名称
        /// </summary>
        public string Name => _element.Name;
        
        /// <summary>
        /// 元素类别
        /// </summary>
        public string Category => _element.Category?.Name ?? "未分类";
        
        /// <summary>
        /// 初始化Revit元素
        /// </summary>
        /// <param name="element">Revit元素</param>
        public RevitElement(Element element)
        {
            _element = element ?? throw new ArgumentNullException(nameof(element));
        }
        
        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数值</returns>
        public object GetParameterValue(string parameterName)
        {
            Parameter parameter = _element.LookupParameter(parameterName);
            if (parameter == null)
            {
                return null;
            }
            
            switch (parameter.StorageType)
            {
                case StorageType.Double:
                    return parameter.AsDouble();
                case StorageType.Integer:
                    return parameter.AsInteger();
                case StorageType.String:
                    return parameter.AsString();
                case StorageType.ElementId:
                    return parameter.AsElementId().IntegerValue;
                default:
                    return null;
            }
        }
        
        /// <summary>
        /// 设置参数值
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="value">参数值</param>
        /// <returns>是否设置成功</returns>
        public bool SetParameterValue(string parameterName, object value)
        {
            Parameter parameter = _element.LookupParameter(parameterName);
            if (parameter == null || parameter.IsReadOnly)
            {
                return false;
            }
            
            try
            {
                switch (parameter.StorageType)
                {
                    case StorageType.Double:
                        return parameter.Set(Convert.ToDouble(value));
                    case StorageType.Integer:
                        return parameter.Set(Convert.ToInt32(value));
                    case StorageType.String:
                        return parameter.Set(value.ToString());
                    case StorageType.ElementId:
                        return parameter.Set(new ElementId(Convert.ToInt32(value)));
                    default:
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// 获取所有参数
        /// </summary>
        /// <returns>参数字典</returns>
        public Dictionary<string, object> GetParameters()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            
            foreach (Parameter parameter in _element.Parameters)
            {
                if (parameter.HasValue)
                {
                    string name = parameter.Definition.Name;
                    object value = null;
                    
                    switch (parameter.StorageType)
                    {
                        case StorageType.Double:
                            value = parameter.AsDouble();
                            break;
                        case StorageType.Integer:
                            value = parameter.AsInteger();
                            break;
                        case StorageType.String:
                            value = parameter.AsString();
                            break;
                        case StorageType.ElementId:
                            value = parameter.AsElementId().IntegerValue;
                            break;
                    }
                    
                    if (value != null)
                    {
                        parameters[name] = value;
                    }
                }
            }
            
            return parameters;
        }
    }
}
