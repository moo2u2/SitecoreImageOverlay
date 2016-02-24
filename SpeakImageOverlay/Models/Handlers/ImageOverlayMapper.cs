namespace SpeakImageOverlay.Models.Handlers
{
    using System;
    using Fields;
    using Glass.Mapper.Sc;
    using Glass.Mapper.Sc.Configuration;
    using Glass.Mapper.Sc.DataMappers;
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    
    public class ImageOverlayMapper : AbstractSitecoreFieldMapper
    {
        public ImageOverlayMapper()
            : base(typeof(ImageWithOverlay))
        {
        }
        
        public override object GetField(Field field, SitecoreFieldConfiguration config, SitecoreDataMappingContext context)
        {
            ImageWithOverlay img = new ImageWithOverlay();
            ImageWithOverlayField sitecoreImage = new ImageWithOverlayField(field);

            SitecoreFieldImageMapper.MapToImage(img, sitecoreImage);
            img.OverlayCoordinates = sitecoreImage.OverlayCoordinates;

            return img;
        }
        
        public override void SetField(Field field, object value, SitecoreFieldConfiguration config, SitecoreDataMappingContext context)
        {
            ImageWithOverlay img = value as ImageWithOverlay;
            if (field == null || img == null)
            {
                return;
            }

            var item = field.Item;

            ImageWithOverlayField sitecoreImage = new ImageWithOverlayField(field);

            SitecoreFieldImageMapper.MapToField(sitecoreImage, img, item);
            sitecoreImage.OverlayCoordinates = img.OverlayCoordinates;
        }
        
        public override string SetFieldValue(object value, SitecoreFieldConfiguration config, SitecoreDataMappingContext context)
        {
            throw new NotImplementedException();
        }
        
        public override object GetFieldValue(string fieldValue, SitecoreFieldConfiguration config, SitecoreDataMappingContext context)
        {
            Item item = context.Service.Database.GetItem(new ID(fieldValue));

            if (item == null)
            {
                return null;
            }

            MediaItem imageItem = new MediaItem(item);
            ImageWithOverlay image = new ImageWithOverlay();
            SitecoreFieldImageMapper.MapToImage(image, imageItem);
            image.OverlayCoordinates = Constants.OverlayDefaultCoordinates;
            return image;
        }
    }
}