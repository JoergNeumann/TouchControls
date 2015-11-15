using Windows.UI.Xaml;

namespace Neumann.TouchControls
{
    public class ColumnViewSection : HeaderedContentControl
    {

        #region Constructors

        public ColumnViewSection()
        {
            this.DefaultStyleKey = typeof(ColumnViewSection);
        }

        #endregion

        #region Properties

        #region IsExpanded

        public bool IsExpanded { get { return (bool)GetValue(IsExpandedProperty); } set { SetValue(IsExpandedProperty, value); } }
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ColumnViewSection),
            new PropertyMetadata(false));

        #endregion

        #endregion

    }
}
