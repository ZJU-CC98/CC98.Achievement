using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

using System.Xml.Linq;
using JetBrains.Annotations;

namespace CC98.Achievement.Documentation;

/// <summary>
/// 提供对枚举项目的说明。
/// </summary>
public class EnumTypesSchemaFilter : ISchemaFilter
{
	/// <summary>
	/// 枚举项目的一个或多个 XML 文档注释数据。
	/// </summary>
	private IEnumerable<XDocument> XmlComments { get; }

	/// <summary>
	/// 初始化 <see cref="EnumTypesSchemaFilter"/> 的新实例。
	/// </summary>
	/// <param name="xmlPaths">要加载的数据的 XML 路径。</param>
	[UsedImplicitly]
	public EnumTypesSchemaFilter(string[] xmlPaths)
	{
		XmlComments = xmlPaths.Select(XDocument.Load);
	}

	/// <inheritdoc />
	public void Apply(OpenApiSchema schema, SchemaFilterContext context)
	{
		if (schema.Enum is { Count: > 0 } &&
		    context.Type is { IsEnum: true })
		{
			schema.Description += "<p>Members:</p><ul>";

			var fullTypeName = context.Type.FullName;

			foreach (var enumMemberName in schema.Enum.OfType<OpenApiString>().
						 Select(v => v.Value))
			{
				var fullEnumMemberName = $"F:{fullTypeName}.{enumMemberName}";

				var enumMemberComments = XmlComments.Descendants("member")
					.FirstOrDefault(m => m.Attribute("name")!.Value.Equals
						(fullEnumMemberName, StringComparison.OrdinalIgnoreCase));

				if (enumMemberComments == null) continue;

				var summary = enumMemberComments.Descendants("summary").FirstOrDefault();

				if (summary == null) continue;

				schema.Description += $"<li><i>{enumMemberName}</i> - {summary.Value.Trim()}</li>";

			}

			schema.Description += "</ul>";
		}
	}
}