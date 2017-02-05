using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data.Items;

namespace Helpfulcore.Audit.Formatters
{
    public class FieldChangesFormatter
    {
        public FieldChangesFormatter()
        {
            this.IgnoreFieldTypes = new List<string>();
            this.IgnoreFields = new List<string>();
        }

        public bool IgnoreSystemFields { get; protected set; }
        public List<string> IgnoreFieldTypes { get; protected set; }
        public List<string> IgnoreFields { get; protected set; }

        public string Format(ItemChanges itemChanges)
        {
            var changes = new StringBuilder();

            changes.AppendLine();
            changes.AppendLine("Item changes:");

            var propOrFieldChangesLogged = false;
            foreach (var prop in itemChanges.Properties)
            {
                changes.AppendLine($"propertyName : '{prop.Value.Name}', change: [new value : '{prop.Value.Value}', old value: '{prop.Value.OriginalValue}']");
                propOrFieldChangesLogged = true;
            }

            foreach (FieldChange field in itemChanges.FieldChanges)
            {
                if (this.IgnoreSystemFields && this.IsSystemField(field)) continue;
                if (this.IsIgnoredField(field) || this.IsIgnoredFieldType(field)) continue;

                changes.AppendLine($"field name : '{field.Definition.Name}', change: [new value: '{field.Value}', old value: '{field.OriginalValue}']");
                propOrFieldChangesLogged = true;
            }

            if (!propOrFieldChangesLogged)
            {
                changes.AppendLine("Field changes may have been ignored to log in the configration.");
            }

            return changes.ToString();
        }

        private bool IsIgnoredFieldType(FieldChange field)
        {
            return this.IgnoreFieldTypes.Any(fieldName => field.Definition.Type.ToLower().Equals(fieldName, StringComparison.CurrentCultureIgnoreCase));
        }

        private bool IsIgnoredField(FieldChange field)
        {
            return this.IgnoreFields.Any(fieldName => field.Definition.Name.ToLower().Equals(fieldName, StringComparison.CurrentCultureIgnoreCase));
        }

        private bool IsSystemField(FieldChange field)
        {
            return field.Definition.Name.StartsWith("__");
        }
    }
}

