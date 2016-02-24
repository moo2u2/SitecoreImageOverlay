namespace SpeakImageOverlay.Models
{
    public class ImageWithOverlay : Glass.Mapper.Sc.Fields.Image
    {
        public virtual string OverlayCoordinates { get; set; }
    }
}