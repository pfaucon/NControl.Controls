using Ncontrol.Controls.UWP;
using NControl.Controls.UWP;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(Button), typeof(FontAwareButtonRenderer))]
namespace Ncontrol.Controls.UWP
{
    /// <summary>
    /// Custom font label renderer.
    /// </summary>
    public class FontAwareButtonRenderer : ButtonRenderer
    {
        /// <summary>
        /// Raises the element changed event.
        /// </summary>
        /// <param name="e">E.</param>
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
                UpdateFont();

            if (Control != null)
            {
                Control.Padding = new Windows.UI.Xaml.Thickness(0);
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Button.FontFamilyProperty.PropertyName ||
                e.PropertyName == Button.FontSizeProperty.PropertyName ||
                e.PropertyName == Button.FontAttributesProperty.PropertyName ||
                e.PropertyName == Button.TextProperty.PropertyName)
                UpdateFont();
        }

        private void UpdateFont()
        {
            var fontName = Element.FontFamily;
            if (string.IsNullOrWhiteSpace(fontName))
                return;

            if (NControls.Typefaces.ContainsKey(fontName))
                Control.FontFamily = NControls.Typefaces[fontName];
        }
    }
}
