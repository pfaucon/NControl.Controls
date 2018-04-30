﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ncontrol.Controls.UWP;
using NControl.Controls;
using Xamarin.Forms.Platform.UWP;
using Xamarin.Forms.PlatformConfiguration;

[assembly: ExportRenderer(typeof(CardPage), typeof(CardPageRenderer))]
namespace Ncontrol.Controls.UWP
{
    /// <summary>
    /// Card page renderer.
    /// </summary>
    public class CardPageRenderer : PageRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CardPageRenderer"/> class.
        /// </summary>
        public CardPageRenderer()
        {

        }

        // TODO: other platforms set parent backgrounds to transparent, but we have no handle to our parent, and clicks on our modal view hit twice
        // it is probable that we will need to fix this somehow


        /// <Docs>This is called when the view is attached to a window.</Docs>
        /// <summary>
        /// Raises the attached to window event.
        /// </summary>
        //protected override void ()
        //{
        //    base.OnAttachedToWindow();

        //    Background = null;
        //    SetBackgroundColor(Android.Graphics.Color.Transparent);

        //    var parent = (Parent as Android.Views.ViewGroup);
        //    if (parent != null)
        //    {
        //        for (var i = 0; i < parent.ChildCount; i++)
        //            parent.GetChildAt(i).SetBackgroundColor(Android.Graphics.Color.Transparent);
        //    }
        //}
    }
}