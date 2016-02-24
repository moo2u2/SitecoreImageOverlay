namespace SpeakImageOverlay.Common.Fields
{
    using Common;
    using Sitecore.Data.Fields;

    public class ImageWithOverlayField : ImageField
    {
        public ImageWithOverlayField(Field innerField)
            : base(innerField)
        {
        }
        
        public ImageWithOverlayField(Field innerField, string runtimeValue)
            : base(innerField, runtimeValue)
        {
        }
        
        public string OverlayCoordinates
        {
            get
            {
                return GetAttribute(Constants.CoordinatesAttribute) ?? Constants.OverlayDefaultCoordinates;
            }

            set
            {
                SetAttribute(Constants.CoordinatesAttribute, value ?? Constants.OverlayDefaultCoordinates);
            }
        }
        
        public static implicit operator ImageWithOverlayField(Field field)
        {
            return field == null ? null : new ImageWithOverlayField(field);
        }
    }
}