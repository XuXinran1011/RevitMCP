using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitMCP.Plugin.Domain.Models;
using RevitMCP.Shared.Interfaces;

namespace RevitMCP.Plugin.Domain.Services
{
    /// <summary>
    /// Revit元素服务
    /// </summary>
    public class RevitElementService
    {
        private readonly Document _document;
        
        /// <summary>
        /// 初始化Revit元素服务
        /// </summary>
        /// <param name="document">Revit文档</param>
        public RevitElementService(Document document)
        {
            _document = document ?? throw new ArgumentNullException(nameof(document));
        }
        
        /// <summary>
        /// 获取元素
        /// </summary>
        /// <param name="elementId">元素ID</param>
        /// <returns>Revit元素</returns>
        public IRevitElement GetElement(int elementId)
        {
            Element element = _document.GetElement(new ElementId(elementId));
            if (element == null)
            {
                return null;
            }
            
            return new RevitElement(element);
        }
        
        /// <summary>
        /// 获取所有元素
        /// </summary>
        /// <returns>Revit元素列表</returns>
        public IEnumerable<IRevitElement> GetAllElements()
        {
            FilteredElementCollector collector = new FilteredElementCollector(_document);
            collector.WhereElementIsNotElementType();
            
            return collector.ToElements().Select(e => new RevitElement(e));
        }
        
        /// <summary>
        /// 按类别获取元素
        /// </summary>
        /// <param name="categoryName">类别名称</param>
        /// <returns>Revit元素列表</returns>
        public IEnumerable<IRevitElement> GetElementsByCategory(string categoryName)
        {
            // 查找类别
            Category category = null;
            foreach (Category cat in _document.Settings.Categories)
            {
                if (cat.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase))
                {
                    category = cat;
                    break;
                }
            }
            
            if (category == null)
            {
                return Enumerable.Empty<IRevitElement>();
            }
            
            // 创建类别过滤器
            ElementCategoryFilter filter = new ElementCategoryFilter(category.Id);
            
            // 获取元素
            FilteredElementCollector collector = new FilteredElementCollector(_document);
            collector.WhereElementIsNotElementType().WherePasses(filter);
            
            return collector.ToElements().Select(e => new RevitElement(e));
        }
        
        /// <summary>
        /// 按参数查询元素
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="parameterValue">参数值</param>
        /// <returns>Revit元素列表</returns>
        public IEnumerable<IRevitElement> GetElementsByParameter(string parameterName, object parameterValue)
        {
            // 获取所有元素
            FilteredElementCollector collector = new FilteredElementCollector(_document);
            collector.WhereElementIsNotElementType();
            
            // 筛选元素
            List<IRevitElement> result = new List<IRevitElement>();
            foreach (Element element in collector.ToElements())
            {
                Parameter parameter = element.LookupParameter(parameterName);
                if (parameter != null && parameter.HasValue)
                {
                    bool match = false;
                    
                    switch (parameter.StorageType)
                    {
                        case StorageType.Double:
                            double doubleValue = parameter.AsDouble();
                            match = Math.Abs(doubleValue - Convert.ToDouble(parameterValue)) < 1e-6;
                            break;
                        case StorageType.Integer:
                            int intValue = parameter.AsInteger();
                            match = intValue == Convert.ToInt32(parameterValue);
                            break;
                        case StorageType.String:
                            string stringValue = parameter.AsString();
                            match = stringValue.Equals(parameterValue.ToString(), StringComparison.OrdinalIgnoreCase);
                            break;
                        case StorageType.ElementId:
                            int idValue = parameter.AsElementId().IntegerValue;
                            match = idValue == Convert.ToInt32(parameterValue);
                            break;
                    }
                    
                    if (match)
                    {
                        result.Add(new RevitElement(element));
                    }
                }
            }
            
            return result;
        }
    }
}
