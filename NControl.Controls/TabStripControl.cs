﻿using System;
using NControl.Abstractions;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NControl.Controls
{
	/// <summary>
	/// Tab strip control.
	/// </summary>
	public class TabStripControl:NControlView
	{
		/// <summary>
		/// Tab strip location.
		/// </summary>
		public enum TabLocation
		{
			Top,
			Bottom
		}

		#region Private Members

		/// <summary>
		/// The list of views that can be displayed.
		/// </summary>
		private readonly ObservableCollection<TabChild> _children = 
			new ObservableCollection<TabChild> ();

		/// <summary>
		/// The tab control.
		/// </summary>
		private readonly NControlView _tabControl;

		/// <summary>
		/// The content view.
		/// </summary>
		private readonly Grid _contentView;

		/// <summary>
		/// The button stack.
		/// </summary>
		private readonly StackLayout _buttonStack;

		/// <summary>
		/// The indicator.
		/// </summary>
		private readonly TabBarIndicator _indicator;

		/// <summary>
		/// The layout.
		/// </summary>
		private readonly RelativeLayout _mainLayout;

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="NControl.Controls.TabStripControl"/> class.
		/// </summary>
		public TabStripControl ()
		{
			_mainLayout = new RelativeLayout ();
			Content = _mainLayout;

			// Create tab control
			_buttonStack = new StackLayoutEx {
				Orientation = StackOrientation.Horizontal,
				Padding = 0,
				Spacing = 0,
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions= LayoutOptions.FillAndExpand,
			};

			_indicator = new TabBarIndicator {
				VerticalOptions = Location == TabLocation.Top ? LayoutOptions.End : LayoutOptions.Start,
				HorizontalOptions = LayoutOptions.Start,
				HeightRequest = 6,
				WidthRequest = 0,
			};

			_tabControl = new NControlView{
				BackgroundColor = TabBackColor,
				Content = new Grid{
					Padding = 0,
					ColumnSpacing = 0,
					RowSpacing=0,
					Children = {
						_buttonStack,
						_indicator,
					}
				}
			};

			_mainLayout.Children.Add (_tabControl, () => new Rectangle (
				0, Location == TabLocation.Top ? 0 : _mainLayout.Height-TabHeight,
				_mainLayout.Width, TabHeight)
			);

			// Create content control
			_contentView = new Grid {
				ColumnSpacing = 0,
				RowSpacing = 0,
				Padding = 0,
				BackgroundColor = Color.Transparent,
			};

			_mainLayout.Children.Add (_contentView, () => new Rectangle (
				0, Location == TabLocation.Top ? TabHeight : 0,
				_mainLayout.Width, _mainLayout.Height-TabHeight)
			);

			_children.CollectionChanged += (sender, e) => {

				_contentView.Children.Clear();
				_buttonStack.Children.Clear();

				foreach(var tabChild in Children)
				{
					var tabItemControl = new TabBarButton(null, tabChild.Title);
					tabItemControl.Font = TabFont;
					tabItemControl.SelectedColor = TabIndicatorColor;						
					_buttonStack.Children.Add(tabItemControl);
				}

				if(Children.Any())
					Activate(Children.First(), false);
			};

			//Border
			var border = new NControlView {
				DrawingFunction = (canvas, rect) => {

					canvas.DrawPath (new NGraphics.PathOp[]{
						new NGraphics.MoveTo(0, 0),
						new NGraphics.LineTo(rect.Width, 0)
					}, NGraphics.Pens.Gray, null);
				},
			};

			_mainLayout.Children.Add (border, () => new Rectangle(
				0, Location == TabLocation.Top ? TabHeight : 0, 
				_mainLayout.Width, Location == TabLocation.Top ? TabHeight : 0));

			// Shadow
			var shadow = new NControlView {
				DrawingFunction = (canvas, rect)=> {

					canvas.DrawRectangle(rect, null, new NGraphics.LinearGradientBrush(
						new NGraphics.Point(0.5, 0.0), new NGraphics.Point(0.5, 1.0),
						Color.Black.MultiplyAlpha(0.3).ToNColor(), NGraphics.Colors.Clear, 
						NGraphics.Colors.Clear));
				}
			};

			_mainLayout.Children.Add (shadow, () => new Rectangle(
				0, Location == TabLocation.Top ? TabHeight : 0, 
				_mainLayout.Width, 6));
		}
			
		/// <summary>
		/// Initializes a new instance of the <see cref="NControl.Controls.TabStripControl"/> class.
		/// </summary>
		/// <param name="view">View.</param>
		public void Activate (TabChild tabChild, bool animate)
		{
			var existingChild = Children.FirstOrDefault (t => t.View == _contentView.Children.FirstOrDefault ());
			if (existingChild == tabChild)
				return;
			
			var idxOfExisting = existingChild != null ? Children.IndexOf (existingChild) : -1;
			var idxOfNew = Children.IndexOf (tabChild);

			if (idxOfExisting > -1 && animate) 
			{
				// Animate
				var translation = idxOfExisting < idxOfNew ? 
					_contentView.Width : - _contentView.Width;

				_contentView.Children.Add(tabChild.View);			

				var newElementWidth = _buttonStack.Children.ElementAt (idxOfNew).Width;
				var newElementLeft = _buttonStack.Children.ElementAt (idxOfNew).X;

				var animation = new Animation ();
				var existingViewOutAnimation = new Animation ((d) => existingChild.View.TranslationX = d,
					0, -translation, Easing.CubicInOut, () => _contentView.Children.Remove (existingChild.View));

				var newViewInAnimation = new Animation ((d) => tabChild.View.TranslationX = d,
                     translation, 0, Easing.CubicInOut);

				var existingTranslation = _indicator.TranslationX;
				var itemWidth = newElementWidth;
				var indicatorTranslation = newElementLeft;
				var indicatorViewAnimation = new Animation ((d) => _indicator.TranslationX = d,
					existingTranslation, indicatorTranslation, Easing.CubicInOut);

				var startWidth = _indicator.Width;
				var indicatorSizeAnimation = new Animation ((d) => _indicator.WidthRequest = d,
					startWidth, newElementWidth, Easing.CubicInOut);

				animation.Add (0.0, 1.0, existingViewOutAnimation);
				animation.Add (0.0, 1.0, newViewInAnimation);
				animation.Add (0.0, 1.0, indicatorViewAnimation);
				animation.Add (0.0, 1.0, indicatorSizeAnimation);
				animation.Commit (this, "TabAnimation");
			} 
			else 
			{
				// Just set first view
				_contentView.Children.Clear();
				_contentView.Children.Add(tabChild.View);
			}

			foreach (var tabBtn in _buttonStack.Children)
				((TabBarButton)tabBtn).IsSelected = _buttonStack.Children.IndexOf(tabBtn) == idxOfNew;
		}

		/// <summary>
		/// Toucheses the began.
		/// </summary>
		/// <param name="points">Points.</param>
		public override bool TouchesBegan (IEnumerable<NGraphics.Point> points)
		{
			base.TouchesBegan(points);

			// Find selected item based on click
			var p = points.First();
			foreach (var child in _buttonStack.Children) {
				if (p.X >= child.X && p.X <= child.X + child.Width) {
					var idx = _buttonStack.Children.IndexOf (child);
					Activate (Children [idx], true);
					break;
				}
			}

			return true;
		}

		/// <summary>
		/// Positions and sizes the children of a Layout.
		/// </summary>
		/// <remarks>Implementors wishing to change the default behavior of a Layout should override this method. It is suggested to
		/// still call the base method and modify its calculated results.</remarks>
		protected override void LayoutChildren (double x, double y, double width, double height)
		{
			base.LayoutChildren (x, y, width, height);

			if (_indicator.WidthRequest == 0 && width > 0) {
				var existingChild = Children.FirstOrDefault (t => t.View == _contentView.Children.FirstOrDefault ());
				var idxOfExisting = existingChild != null ? Children.IndexOf (existingChild) : -1;

				_indicator.WidthRequest = _buttonStack.Children.ElementAt (idxOfExisting).Width;
			}
		}

		/// <summary>
		/// Gets the views.
		/// </summary>
		/// <value>The views.</value>
		public IList<TabChild> Children
		{
			get{ return _children;}
		}

		/// <summary>
		/// The TabFont property.
		/// </summary>
		public static BindableProperty TabFontProperty = 
			BindableProperty.Create<TabStripControl, Font> (p => p.TabFont, Font.Default,
				propertyChanged: (bindable, oldValue, newValue) => {
					var ctrl = (TabStripControl)bindable;
					ctrl.TabFont = newValue;
				});

		/// <summary>
		/// Gets or sets the TabFont of the TabStripControl instance.
		/// </summary>
		/// <value>The color of the buton.</value>
		public Font TabFont {
			get{ return (Font)GetValue (TabFontProperty); }
			set {
				SetValue (TabFontProperty, value);
				foreach (var tabBtn in _buttonStack.Children)
					((TabBarButton)tabBtn).Font = value;
			}
		}
		/// <summary>
		/// The TabLocation property.
		/// </summary>
		public static BindableProperty TabLocationProperty = 
			BindableProperty.Create<TabStripControl, TabLocation> (p => p.Location, TabLocation.Top,
				propertyChanged: (bindable, oldValue, newValue) => {
					var ctrl = (TabStripControl)bindable;
					ctrl.Location = newValue;
				});

		/// <summary>
		/// Gets or sets the TabLocation of the TabStripControl instance.
		/// </summary>
		/// <value>The color of the buton.</value>
		public TabLocation Location {
			get{ return (TabLocation)GetValue (TabLocationProperty); }
			set {
				SetValue (TabLocationProperty, value);
				_indicator.VerticalOptions = Location == TabLocation.Top ? 
					LayoutOptions.End : LayoutOptions.Start;
				_mainLayout.ForceLayout ();
			}
		}

		/// <summary>
		/// The TabIndicatorColor property.
		/// </summary>
		public static BindableProperty TabIndicatorColorProperty = 
			BindableProperty.Create<TabStripControl, Color> (p => p.TabIndicatorColor, Color.Accent,
				propertyChanged: (bindable, oldValue, newValue) => {
					var ctrl = (TabStripControl)bindable;
					ctrl.TabIndicatorColor = newValue;
				});

		/// <summary>
		/// Gets or sets the TabIndicatorColor of the TabStripControl instance.
		/// </summary>
		/// <value>The color of the buton.</value>
		public Color TabIndicatorColor {
			get{ return (Color)GetValue (TabIndicatorColorProperty); }
			set {
				SetValue (TabIndicatorColorProperty, value);
				_indicator.BackgroundColor = value;
			}
		}

		/// <summary>
		/// The TabHeight property.
		/// </summary>
		public static BindableProperty TabHeightProperty = 
			BindableProperty.Create<TabStripControl, double> (p => p.TabHeight, 40,
				propertyChanged: (bindable, oldValue, newValue) => {
					var ctrl = (TabStripControl)bindable;
					ctrl.TabHeight = newValue;
				});

		/// <summary>
		/// Gets or sets the TabHeight of the TabStripControl instance.
		/// </summary>
		/// <value>The color of the buton.</value>
		public double TabHeight {
			get{ return (double)GetValue (TabHeightProperty); }
			set {
				SetValue (TabHeightProperty, value);
			}
		}

		/// <summary>
		/// The TabBackColor property.
		/// </summary>
		public static BindableProperty TabBackColorProperty = 
			BindableProperty.Create<TabStripControl, Color> (p => p.TabBackColor, Color.White,
				propertyChanged: (bindable, oldValue, newValue) => {
					var ctrl = (TabStripControl)bindable;
					ctrl.TabBackColor = newValue;
				});

		/// <summary>
		/// Gets or sets the TabBackColor of the TabStripControl instance.
		/// </summary>
		/// <value>The color of the buton.</value>
		public Color TabBackColor {
			get{ return (Color)GetValue (TabBackColorProperty); }
			set {
				SetValue (TabBackColorProperty, value);
				_tabControl.BackgroundColor = value;
			}
		}			
	}

	/// <summary>
	/// Tab child.
	/// </summary>
	public class TabChild
	{
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Title { get; set; }

		/// <summary>
		/// Gets the view.
		/// </summary>
		/// <value>The view.</value>
		public View View { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="NControl.Controls.TabChild"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="view">View.</param>
		public TabChild(string title, View view)
		{
			Title = title;
			View = view;
		}
	}

	/// <summary>
	/// Tab bar indicator.
	/// </summary>
	public class TabBarIndicator: View
	{
		       
	}

	/// <summary>
	/// Tab bar button.
	/// </summary>
	public class TabBarButton: NControlView
	{
		private readonly FontAwesomeLabel _imageLabel;
		private readonly FontAwesomeLabel _selectedImageLabel;
		private readonly Label _label;

		public Color DarkTextColor = Color.Black;
		public Color AccentColor = Color.Accent;

		/// <summary>
		/// Initializes a new instance of the <see cref="Clooger.FormsApp.UserControls.TabBarButton"/> class.
		/// </summary>
		public TabBarButton(string imageName, string buttonText)
		{
			if (!string.IsNullOrWhiteSpace (imageName)) {
				_imageLabel = new FontAwesomeLabel {
					Text = imageName,
					XAlign = TextAlignment.Center,
					YAlign = TextAlignment.Center,                
					TextColor = DarkTextColor,
					IsVisible = true
				};			

				_selectedImageLabel = new FontAwesomeLabel {
					Text = imageName,
					XAlign = TextAlignment.Center,
					YAlign = TextAlignment.Center,                
					TextColor = AccentColor,
					IsVisible = false,
				};
			}

			_label = new Label {
				Text = buttonText,
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,                
				FontSize = 12,
				LineBreakMode = LineBreakMode.TailTruncation,					
				TextColor = DarkTextColor,
			};

			Content = new StackLayout{
				Orientation = StackOrientation.Vertical,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.Center,
				Padding = 10,
			};

			if (!string.IsNullOrWhiteSpace (imageName))
				(Content as StackLayout).Children.Add (new Grid { ColumnSpacing = 0, RowSpacing = 0, Padding = 0, 
					HeightRequest = 38,
					Children = { _imageLabel, _selectedImageLabel }
				});

			(Content as StackLayout).Children.Add (_label);
		}

		/// <summary>
		/// The SelectedColor property.
		/// </summary>
		public static BindableProperty SelectedColorProperty = 
			BindableProperty.Create<TabBarButton, Color> (p => p.SelectedColor, Color.Accent,
				propertyChanged: (bindable, oldValue, newValue) => {
					var ctrl = (TabBarButton)bindable;
					ctrl.SelectedColor = newValue;
				});

		/// <summary>
		/// Gets or sets the SelectedColor of the TabBarButton instance.
		/// </summary>
		/// <value>The color of the buton.</value>
		public Color SelectedColor {
			get{ return (Color)GetValue (SelectedColorProperty); }
			set {
				SetValue (SelectedColorProperty, value);
				AccentColor = value;

			}
		}
		/// <summary>
		/// The Font property.
		/// </summary>
		public static BindableProperty FontProperty = 
			BindableProperty.Create<TabBarButton, Font> (p => p.Font, Font.Default,
				propertyChanged: (bindable, oldValue, newValue) => {
					var ctrl = (TabBarButton)bindable;
					ctrl.Font = newValue;
				});

		/// <summary>
		/// Gets or sets the Font of the TabBarButton instance.
		/// </summary>
		/// <value>The color of the buton.</value>
		public Font Font {
			get{ return (Font)GetValue (FontProperty); }
			set {
				SetValue (FontProperty, value);

				_label.FontFamily = value.FontFamily;
				_label.FontSize = value.FontSize;
				_label.FontAttributes = value.FontAttributes;
			}
		}

		/// <summary>
		/// Draw the specified canvas.
		/// </summary>
		/// <param name="canvas">Canvas.</param>
		/// <param name="rect">Rect.</param>
		public override void Draw (NGraphics.ICanvas canvas, NGraphics.Rect rect)
		{
			base.Draw (canvas, rect);

//			canvas.DrawRectangle (new NGraphics.Rect (rect.Width, 0, rect.Width, rect.Height),
//				NGraphics.Pens.Gray, null);
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is selected.
		/// </summary>
		/// <value><c>true</c> if this instance is selected; otherwise, <c>false</c>.</value>
		public bool IsSelected
		{
			get
			{
				return _label.TextColor == AccentColor;
			}
			set
			{
				if (value)
					_label.TextColor = AccentColor;
				else
					_label.TextColor = DarkTextColor;

				if(_selectedImageLabel != null)
					_selectedImageLabel.IsVisible = value;

				if(_imageLabel != null)
					_imageLabel.IsVisible = !value;
			}
		}	

		/// <summary>
		/// Toucheses the began.
		/// </summary>
		/// <param name="points">Points.</param>
		public override bool TouchesBegan(System.Collections.Generic.IEnumerable<NGraphics.Point> points)
		{
			base.TouchesBegan(points);
			_label.TextColor = AccentColor;

			return true;
		}
	}

	/// <summary>
	/// Stack layout ex.
	/// </summary>
	internal class StackLayoutEx: StackLayout
	{
		/// <summary>
		/// Make sure we lay out so that we only use as much (or little) space as necessary for 
		/// each item
		/// </summary>
		/// <remarks>Implementors wishing to change the default behavior of a Layout should override this method. It is suggested to
		/// still call the base method and modify its calculated results.</remarks>
		protected override void LayoutChildren (double x, double y, double width, double height)
		{
			base.LayoutChildren (x, y, width, height);

			var total = Children.Sum (t => t.Width);
			var parentWidth = (Parent as View).Width;

			if (total < parentWidth) {

				// We need more space
				var diff = (parentWidth - total)/Children.Count;

				var xoffset = 0.0;
				foreach (var child in Children) {
					child.Layout (new Rectangle (child.X + xoffset, child.Y, child.Width + diff, child.Height));
					xoffset += diff;
				}
			}
		}
	}
}
