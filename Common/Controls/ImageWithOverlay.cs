namespace SpeakImageOverlay.Common.Controls
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Web.UI;
    using Sitecore;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Globalization;
    using Sitecore.Resources.Media;
    using Sitecore.Shell.Applications.ContentEditor;
    using Sitecore.Text;
    using Sitecore.Web;
    using Sitecore.Web.UI.Sheer;
    using Convert = System.Convert;

    public class ImageWithOverlay : Image
    {
        /// <summary>The overlay message.</summary>
        private const string OverlayMessage = "overlay";

        /// <summary>The overlay application location.</summary>
        private const string OverlayAppLocation = "/sitecore/client/Your Apps/OverlaySelector";

        /// <summary>Handles the message</summary>
        /// <param name="message">Message to handle</param>
        public override void HandleMessage(Message message)
        {
            if (message["id"] != ID)
            {
                return;
            }

            string[] command = message.Name.Split(':');
            Assert.IsTrue(command.Length > 1, "Expected message format is control:message");

            if (command[1] == OverlayMessage)
            {
                Sitecore.Context.ClientPage.Start(this, "Overlay");
                return;
            }

            base.HandleMessage(message);
        }

        /// <summary>The overlay message.</summary>
        /// <param name="args">The arguments.</param>
        public void Overlay(ClientPipelineArgs args)
        {
            if (args.IsPostBack)
            {
                if (!args.HasResult)
                {
                    return;
                }

                XmlValue.SetAttribute(Common.Constants.CoordinatesAttribute, args.Result);
                Update();
                SetModified();
                SheerResponse.Refresh(this);
            }
            else
            {
                UrlString urlString = new UrlString(OverlayAppLocation);

                Item selectedImage = GetMediaItem();
                if (selectedImage != null)
                {
                    urlString["fo"] = selectedImage.Uri.ToString();
                }

                string coords = XmlValue.GetAttribute(Common.Constants.CoordinatesAttribute);
                if (!string.IsNullOrEmpty(coords))
                {
                    urlString["coords"] = coords;
                }

                SheerResponse.ShowModalDialog(new ModalDialogOptions(urlString.ToString()) { Width = "800px", Height = "700px", Response = true, ForceDialogSize = true });
                args.WaitForPostBack();
            }
        }

        /// <summary>Render the item.</summary>
        /// <param name="output">The output.</param>
        protected override void DoRender(HtmlTextWriter output)
        {
            Assert.ArgumentNotNull(output, "output");
            Item mediaItem = GetMediaItem();
            string src = GetSrc();
            string str1 = " src=\"" + src + "\"";
            string str2 = " id=\"" + ID + "_image\"";
            string str3 = " alt=\"" + (mediaItem != null ? WebUtil.HtmlEncode(mediaItem["Alt"]) : string.Empty) + "\"";
            string coordinates = XmlValue.GetAttribute(Common.Constants.CoordinatesAttribute);
            int width = Convert.ToInt32(mediaItem["Width"]);
            int height = Convert.ToInt32(mediaItem["Height"]);
            double scale = 128.0 / height;
            if (string.IsNullOrEmpty(coordinates))
            {
                coordinates = Common.Constants.OverlayDefaultCoordinates;
            }

            int[] coords = coordinates.Split(',').Select(int.Parse).ToArray();

            // base.DoRender(output);
            output.Write("<div id=\"" + ID + "_pane\" class=\"scContentControlImagePane\" style=\"position:relative\">");
            output.Write("<div class=\"scContentControlImageImage\">");
            output.Write("<img src=\"/Content/images/overlay.png?w=49\" style=\"position:absolute;left:" + Math.Round((coords[0] / 10.0) + 12) + "px;top:" + Math.Round((coords[1] / 10.0) + 8) + "px;width:49px;\">");
            output.Write("<iframe" + str2 + str1 + str3 + " frameborder=\"0\" marginwidth=\"0\" marginheight=\"0\" width=\"" + Math.Round(scale * width) + "px\" height=\"" + Math.Round(scale * height) + "px\" allowtransparency=\"allowtransparency\"></iframe>");
            output.Write("</div>");
            output.Write("<div id=\"" + ID + "_details\" class=\"scContentControlImageDetails\">");
            string details = GetDetails();
            output.Write(details);
            output.Write("</div>");
            output.Write("</div>");
        }

        /// <summary>Get the media item (if selected) - taken from decompiled Image (base class).</summary>
        /// <returns>The selected media <see cref="Item"/>.</returns>
        private Item GetMediaItem()
        {
            string attribute = XmlValue.GetAttribute("mediaid");
            if (attribute.Length <= 0)
            {
                return null;
            }

            Language language = Language.Parse(ItemLanguage);
            return Client.ContentDatabase.GetItem(attribute, language);
        }

        /// <summary>Get the source - taken from decompiled Image (base class).</summary>
        /// <param name="src">The source.</param>
        private string GetSrc()
        {
            string src = string.Empty;
            MediaItem mediaItem = GetMediaItem();
            if (mediaItem != null)
            {
                MediaUrlOptions thumbnailOptions = MediaUrlOptions.GetThumbnailOptions(mediaItem);
                int result;
                if (!int.TryParse(mediaItem.InnerItem["Height"], out result))
                {
                    result = 128;
                }
                thumbnailOptions.Height = Math.Min(128, result);
                thumbnailOptions.MaxWidth = 640;
                thumbnailOptions.UseDefaultIcon = true;
                src = MediaManager.GetMediaUrl(mediaItem, thumbnailOptions);
            }

            return src;
        }

        /// <summary>Get the details - taken from decompiled Image (base class).</summary>
        /// <returns>The details.</returns>
        private string GetDetails()
        {
            string str1 = string.Empty;
            MediaItem mediaItem = GetMediaItem();
            if (mediaItem != null)
            {
                Item innerItem = mediaItem.InnerItem;
                StringBuilder stringBuilder = new StringBuilder();
                XmlValue xmlValue = XmlValue;
                stringBuilder.Append("<div>");
                string str2 = innerItem["Dimensions"];
                string str3 = WebUtil.HtmlEncode(xmlValue.GetAttribute("width"));
                string str4 = WebUtil.HtmlEncode(xmlValue.GetAttribute("height"));
                if (!string.IsNullOrEmpty(str3) || !string.IsNullOrEmpty(str4))
                {
                    stringBuilder.Append(Translate.Text("Dimensions: {0} x {1} (Original: {2})", str3, str4, str2));
                }
                else
                {
                    stringBuilder.Append(Translate.Text("Dimensions: {0}", str2));
                }

                stringBuilder.Append("</div>");
                stringBuilder.Append("<div style=\"padding:2px 0px 0px 0px\">");
                string str5 = WebUtil.HtmlEncode(innerItem["Alt"]);
                string str6 = WebUtil.HtmlEncode(xmlValue.GetAttribute("alt"));
                if (!string.IsNullOrEmpty(str6) && !string.IsNullOrEmpty(str5))
                {
                    stringBuilder.Append(Translate.Text("Alternate Text: \"{0}\" (Default Alternate Text: \"{1}\")", str6, str5));
                }
                else if (!string.IsNullOrEmpty(str6))
                {
                    stringBuilder.Append(Translate.Text("Alternate Text: \"{0}\"", str6));
                }
                else if (!string.IsNullOrEmpty(str5))
                {
                    stringBuilder.Append(Translate.Text("Default Alternate Text: \"{0}\"", str5));
                }
                else
                {
                    stringBuilder.Append(Translate.Text("Warning: Alternate Text is missing."));
                }

                stringBuilder.Append("</div>");
                stringBuilder.Append("<div style=\"padding:2px 0px 0px 0px\">");
                string str7 = WebUtil.HtmlEncode(xmlValue.GetAttribute(Common.Constants.CoordinatesAttribute));
                stringBuilder.Append(!string.IsNullOrEmpty(str7) ? Translate.Text("Overlay coordinates: {0}", str7) : Translate.Text("Overlay coordinates: No coordinates set, using 100,100."));

                stringBuilder.Append("</div>");
                str1 = stringBuilder.ToString();
            }

            if (str1.Length == 0)
            {
                str1 = Translate.Text("This media item has no details.");
            }

            return str1;
        }
    }
}