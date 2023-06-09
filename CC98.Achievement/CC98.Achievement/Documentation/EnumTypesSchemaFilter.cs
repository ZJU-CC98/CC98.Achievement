using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

using System.Xml.Linq;

namespace CC98.Achievement.Documentation;

public class EnumTypesSchemaFilter : ISchemaFilter
{
	public IEnumerable<XDocument> XmlComments { get; }

	public EnumTypesSchemaFilter(string[] xmlPaths)
	{
		XmlComments = xmlPaths.Select(XDocument.Load);
	}

	public void Apply(OpenApiSchema schema, SchemaFilterContext context)
	{
		if (schema.Enum != null && schema.Enum.Count > 0 &&
			context.Type != null && context.Type.IsEnum)
		{
			schema.Description += "<p>Members:</p><ul>";

			var fullTypeName = context.Type.FullName;

			foreach (var enumMemberName in schema.Enum.OfType<OpenApiString>().
						 Select(v => v.Value))
			{
				var fullEnumMemberName = $"F:{fullTypeName}.{enumMemberName}";

				var enumMemberComments = XmlComments.Descendants("member")
					.FirstOrDefault(m => m.Attribute("name").Value.Equals
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