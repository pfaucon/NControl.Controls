using Ncontrol.Controls.UWP;
using NControl.Controls;
using NControl.Controls.UWP;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(ExtendedEntry), typeof(ExtendedEntryRenderer))]
namespace Ncontrol.Controls.UWP
{
    public class ExtendedEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            //Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
            Control.Padding = new Windows.UI.Xaml.Thickness(10, 0, 0, 0);

            Control.HorizontalAlignment = ConvertXFAlignmentToWindows(Element.HorizontalTextAlignment);
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == Entry.HorizontalTextAlignmentProperty.PropertyName)
                Control.HorizontalAlignment = ConvertXFAlignmentToWindows(Element.HorizontalTextAlignment);
            else if (e.PropertyName == Entry.FontFamilyProperty.PropertyName)
                UpdateFont();
        }

        /// <summary>
        /// Updates the font.
        /// </summary>
        private void UpdateFont()
        {
            var fontName = Element.FontFamily;
            if (string.IsNullOrWhiteSpace(fontName))
                return;

            if (NControls.Typefaces.ContainsKey(fontName))
                Control.FontFamily = NControls.Typefaces[fontName];
        }

        /// <summary>
        /// Updates the text alignment.
        /// </summary>
        private Windows.UI.Xaml.HorizontalAlignment ConvertXFAlignmentToWindows(TextAlignment alignment)
        {
            switch (alignment)
            {
                case TextAlignment.Start:
                    return Windows.UI.Xaml.HorizontalAlignment.Left;
                case TextAlignment.Center:
                    return Windows.UI.Xaml.HorizontalAlignment.Center;
                case TextAlignment.End:
                    return Windows.UI.Xaml.HorizontalAlignment.Right;
                default:
                    return Windows.UI.Xaml.HorizontalAlignment.Center;
            }
        }
    }
}
