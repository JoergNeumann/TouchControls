using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Neumann.TouchControls
{
    public class HeaderedContentControl : ContentControl
    {

        #region Constructors

        public HeaderedContentControl()
        {
            this.DefaultStyleKey = typeof(HeaderedContentControl);
        }

        #endregion

        #region Properties

        #region Header

        public object Header { get { return this.GetValue(HeaderProperty); } set { this.SetValue(HeaderProperty, value); } }
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(HeaderedContentControl),
            new PropertyMetadata(" "));

        #endregion

        #region HeaderTemplate

        public DataTemplate HeaderTemplate { get { return this.GetValue(HeaderTemplateProperty) as DataTemplate; } set { this.SetValue(HeaderTemplateProperty, value); } }
        public static readonly DependencyProperty HeaderTemplateProperty =
                DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(HeaderedContentControl),
                new PropertyMetadata(null));

        #endregion

        #endregion

    }
}
