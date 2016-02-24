namespace SpeakImageOverlay.Models
{
    using Glass.Mapper.Sc.Configuration.Attributes;

    [SitecoreType(TemplateId = Constants.Templates.Page)]
    public interface IPage
    {
        [SitecoreField(FieldId = Constants.Fields.PageImage)]
        ImageWithOverlay PageImage { get; set; }
    }
}
