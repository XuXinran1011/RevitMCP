using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RevitMCP.Shared.Tools
{
    /// <summary>
    /// 命令Schema定义，描述一个族类型的所有参数及其元数据。
    /// </summary>
    public class CommandSchema
    {
        /// <summary>元素类型（如Wall、Door等）</summary>
        public string ElementType { get; }
        /// <summary>族名称（可选）</summary>
        public string FamilyName { get; }
        /// <summary>参数定义表</summary>
        public IReadOnlyDictionary<string, ParameterDefinition> Parameters { get; }
        /// <summary>最后更新时间</summary>
        public DateTime LastUpdated { get; }

        public CommandSchema(string elementType, string familyName, IDictionary<string, ParameterDefinition> parameters, DateTime lastUpdated)
        {
            ElementType = elementType;
            FamilyName = familyName;
            Parameters = new ReadOnlyDictionary<string, ParameterDefinition>(parameters);
            LastUpdated = lastUpdated;
        }
    }

    /// <summary>
    /// 参数定义，描述单个参数的类型、单位、必填性等元数据。
    /// </summary>
    public class ParameterDefinition
    {
        /// <summary>参数名称</summary>
        public string Name { get; }
        /// <summary>参数类型（如number、string等）</summary>
        public string Type { get; }
        /// <summary>单位（如mm）</summary>
        public string Unit { get; }
        /// <summary>是否必填</summary>
        public bool Required { get; }
        /// <summary>参数描述</summary>
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

    /// <summary>
    /// 命令Schema导出格式。
    /// </summary>
    public enum SchemaExportFormat
    {
        Json,
        Markdown
    }

    /// <summary>
    /// 命令Schema导出服务接口，定义自动导出/同步命令字典的契约。
    /// </summary>
    public interface ICommandSchemaExportService
    {
        /// <summary>
        /// 获取所有族类型的命令Schema集合。
        /// </summary>
        Task<IEnumerable<CommandSchema>> GetAllSchemasAsync();

        /// <summary>
        /// 导出所有命令Schema到指定文件，支持多种格式。
        /// </summary>
        Task ExportSchemasAsync(string outputPath, SchemaExportFormat format);
    }

    /// <summary>
    /// JSON格式命令Schema导出实现。
    /// </summary>
    public class JsonCommandSchemaExporter : ICommandSchemaExportService
    {
        private readonly Func<Task<IEnumerable<CommandSchema>>> _schemaProvider;

        /// <param name="schemaProvider">用于获取所有命令Schema的委托（可由族库管理模块注入）</param>
        public JsonCommandSchemaExporter(Func<Task<IEnumerable<CommandSchema>>> schemaProvider)
        {
            _schemaProvider = schemaProvider;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CommandSchema>> GetAllSchemasAsync()
        {
            return await _schemaProvider();
        }

        /// <inheritdoc />
        public async Task ExportSchemasAsync(string outputPath, SchemaExportFormat format)
        {
            var schemas = await GetAllSchemasAsync();
            if (format == SchemaExportFormat.Json)
            {
                var json = JsonConvert.SerializeObject(schemas, Formatting.Indented);
                File.WriteAllText(outputPath, json);
            }
            // TODO: 支持Markdown等其他格式
        }
    }
} 