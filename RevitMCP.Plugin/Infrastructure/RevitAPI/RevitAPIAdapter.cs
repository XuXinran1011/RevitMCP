using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitMCP.Plugin.Domain.Models;
using RevitMCP.Shared.Models;

namespace RevitMCP.Plugin.Infrastructure.RevitAPI
{
    /// <summary>
    /// Revit API适配器
    /// </summary>
    public class RevitAPIAdapter
    {
        private readonly UIApplication _uiApplication;
        
        /// <summary>
        /// 初始化Revit API适配器
        /// </summary>
        /// <param name="uiApplication">Revit UI应用程序</param>
        public RevitAPIAdapter(UIApplication uiApplication)
        {
            _uiApplication = uiApplication ?? throw new ArgumentNullException(nameof(uiApplication));
        }
        
        /// <summary>
        /// 获取当前文档
        /// </summary>
        /// <returns>当前文档</returns>
        public Document GetActiveDocument()
        {
            return _uiApplication.ActiveUIDocument?.Document;
        }
        
        /// <summary>
        /// 将Revit元素转换为元素信息
        /// </summary>
        /// <param name="element">Revit元素</param>
        /// <returns>元素信息</returns>
        public RevitElementInfo ConvertToElementInfo(Element element)
        {
            if (element == null)
            {
                return null;
            }
            
            RevitElementInfo info = new RevitElementInfo
            {
                Id = element.Id.IntegerValue,
                Name = element.Name,
                Category = element.Category?.Name ?? "未分类"
            };
            
            // 获取类型
            ElementId typeId = element.GetTypeId();
            if (typeId != ElementId.InvalidElementId)
            {
                Element typeElement = element.Document.GetElement(typeId);
                if (typeElement != null)
                {
                    info.TypeId = typeId.IntegerValue;
                    info.TypeName = typeElement.Name;
                }
            }
            
            // 获取参数
            foreach (Parameter parameter in element.Parameters)
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
                        info.Parameters[name] = value;
                    }
                }
            }
            
            return info;
        }
        
        /// <summary>
        /// 在事务中执行操作
        /// </summary>
        /// <param name="document">文档</param>
        /// <param name="transactionName">事务名称</param>
        /// <param name="action">操作</param>
        /// <returns>操作结果</returns>
        public bool ExecuteInTransaction(Document document, string transactionName, Action action)
        {
            if (document == null)
            {
                return false;
            }
            
            using (Transaction transaction = new Transaction(document, transactionName))
            {
                try
                {
                    transaction.Start();
                    action();
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.RollBack();
                    return false;
                }
            }
        }
    }
}
