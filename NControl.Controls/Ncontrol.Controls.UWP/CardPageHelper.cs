using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using NControl.Controls.UWP;
using NControl.Controls;
using Xamarin.Forms;
using static Xamarin.Forms.Application;

[assembly: Xamarin.Forms.Dependency(typeof(CardPageHelper))]

namespace NControl.Controls.UWP
{
    /// <summary>
    /// Card page renderer.
    /// </summary>
    public class CardPageHelper : ICardPageHelper
    {
        // we assume that only a single page can be shown at a time because of a bug in UWP implementation causing taps to occur twice.
        private bool isPageClosed = false;

        #region ICardPageHelper implementation

        /// <summary>
        /// Gets the size of the screen.
        /// </summary>
        /// <returns>The screen size.</returns>
        public Size GetScreenSize()
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            var size = new Size(bounds.Width * scaleFactor, bounds.Height * scaleFactor);
            return size;
        }

        /// <summary>
        /// Shows the card async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="page">Page.</param>
        public Task ShowAsync(CardPage page)
        {
            isPageClosed = false;
            return Current.MainPage.Navigation.PushModalAsync(page, false);
        }

        /// <summary>
        /// Closes the card async
        /// </summary>
        /// <returns>The aync.</returns>
        /// <param name="page">Page.</param>
        public Task CloseAsync(CardPage page)
        {
            if (isPageClosed) return Task.FromResult(true);

            isPageClosed = true;
            return Current.MainPage.Navigation.PopModalAsync(false);
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="CardPageHelper"/> control animates itself.
        /// </summary>
        /// <value><c>true</c> if control animates itself; otherwise, <c>false</c>.</value>
        public bool ControlAnimatesItself
        {
            get { return true; }
        }

        #endregion
    }

    /// <summary>
    /// Droid card page navigation page.
    /// </summary>
    public class UWPCardPageNavigationPage : NavigationPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DroidCardPageNavigationPage"/> class.
        /// </summary>
        public UWPCardPageNavigationPage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DroidCardPageNavigationPage"/> class.
        /// </summary>
        /// <param name="rootPage">Root page.</param>
        public UWPCardPageNavigationPage(Page rootPage) : base(rootPage)
        {
        }
    }
}