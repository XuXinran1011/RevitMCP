using System;

namespace RevitMCP.Shared.Models
{
    /// <summary>
    /// 参数领域模型，值对象，包含参数名、类型、单位、是否必填、描述、默认值等。
    /// </summary>
    public class Parameter
    {
        public string Name { get; }
        public string Type { get; }
        public string Unit { get; }
        public bool Required { get; }
        public string Description { get; }
        public object? DefaultValue { get; }

        /// <summary>
        /// 构造函数，初始化所有参数属性。
        /// </summary>
        public Parameter(string name, string type, string unit, bool required, string description, object? defaultValue)
        {
            Name = name;
            Type = type;
            Unit = unit;
            Required = required;
            Description = description;
            DefaultValue = defaultValue;
        }
    }
} 