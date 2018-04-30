using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using NControl.Controls.UWP;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(Label), typeof(FontAwareLabelRenderer))]
namespace NControl.Controls.UWP
{
    /// <summary>
    /// Custom font label renderer.
    /// </summary>
    public class FontAwareLabelRenderer : LabelRenderer
    {
        /// <summary>
        /// Raises the element changed event.
        /// </summary>
        /// <param name="e">E.</param>
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Label> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
                UpdateFont();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Label.FontFamilyProperty.PropertyName)
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