using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.ComponentModel.Design;
using eSSCParallon.Activities.Design.Designers;
using eSSCParallon.Activities.Design.Properties;

namespace eSSCParallon.Activities.Design
{
    public class DesignerMetadata : IRegisterMetadata
    {
        public void Register()
        {
            var builder = new AttributeTableBuilder();
            builder.ValidateTable();

            var categoryAttribute = new CategoryAttribute($"{Resources.Category}");

            builder.AddCustomAttributes(typeof(PALoadNTLM), categoryAttribute);
            builder.AddCustomAttributes(typeof(PALoadNTLM), new DesignerAttribute(typeof(PALoadNTLMDesigner)));
            builder.AddCustomAttributes(typeof(PALoadNTLM), new HelpKeywordAttribute(""));


            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}
