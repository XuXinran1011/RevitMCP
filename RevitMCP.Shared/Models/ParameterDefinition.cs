using System;

namespace RevitMCP.Shared.Models
{
    /// <summary>
    /// 参数定义，描述单个参数的类型、单位、必填性等元数据。
    /// </summary>
    public class ParameterDefinition
    {
        public string Name { get; }
        public string Type { get; }
        public string Unit { get; }
        public bool Required { get; }
        public string Description { get; }

        public ParameterDefinition(string name, string type, string unit, bool required, string description)
        {
            Name = name;
            Type = type;
            Unit = unit;
            Required = required;
            Description = description;
        }
    }
} 