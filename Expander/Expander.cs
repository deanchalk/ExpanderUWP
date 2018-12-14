using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;

namespace Expander
{
    [TemplateVisualState(Name = StringConstants.StateContentExpanded, GroupName = StringConstants.GroupContent)]
    [TemplateVisualState(Name = StringConstants.StateContentCollapsed, GroupName = StringConstants.GroupContent)]
    [TemplatePart(Name = StringConstants.ExpanderToggleButtonPart, Type = typeof(ToggleButton))]
    [TemplatePart(Name = StringConstants.HeaderButtonPart, Type = typeof(ButtonBase))]
    [ContentProperty(Name = "Content")]
    public sealed class Expander : Control
    {
        internal static readonly DependencyProperty ExpanderToggleButtonStyleProperty =
            DependencyProperty.Register("ExpanderToggleButtonStyle", typeof(Style), typeof(Expander),
                new PropertyMetadata(null));

        internal static readonly DependencyProperty HeaderButtonStyleProperty =
            DependencyProperty.Register("HeaderButtonStyle", typeof(Style), typeof(Expander),
                new PropertyMetadata(null));

        internal static readonly DependencyProperty HeaderButtonContentProperty =
            DependencyProperty.Register("HeaderButtonContent", typeof(object), typeof(Expander),
                new PropertyMetadata(null));

        internal static readonly DependencyProperty HeaderButtonCommandProperty =
            DependencyProperty.Register("HeaderButtonCommand", typeof(ICommand), typeof(Expander),
                new PropertyMetadata(null));

        internal static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(Expander), new PropertyMetadata(null));


        internal static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(Expander),
                new PropertyMetadata(false, OnIsExpandedPropertyChanged));


        private ToggleButton expanderButton;
        private ButtonBase headerButton;
        private RowDefinition mainContentRow;

        public Expander()
        {
            DefaultStyleKey = typeof(Expander);
        }

        public bool IsExpanded
        {
            get => (bool) GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public ICommand HeaderButtonCommand
        {
            get => (ICommand) GetValue(HeaderButtonCommandProperty);
            set => SetValue(HeaderButtonCommandProperty, value);
        }

        public object HeaderButtonContent
        {
            get => GetValue(HeaderButtonContentProperty);
            set => SetValue(HeaderButtonContentProperty, value);
        }

        public Style HeaderButtonStyle
        {
            get => (Style) GetValue(HeaderButtonStyleProperty);
            set => SetValue(HeaderButtonStyleProperty, value);
        }

        public Style ExpanderToggleButtonStyle
        {
            get => (Style) GetValue(ExpanderToggleButtonStyleProperty);
            set => SetValue(ExpanderToggleButtonStyleProperty, value);
        }

        private static void OnIsExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var expander = d as Expander;
            if (expander == null || expander.expanderButton == null)
                return;
            var isExpanded = (bool) e.NewValue;
            expander.expanderButton.IsChecked = isExpanded;
            if (isExpanded)
                expander.ExpandControl();
            else
                expander.CollapseControl();
        }

        protected override void OnApplyTemplate()
        {
            expanderButton = GetTemplateChild(StringConstants.ExpanderToggleButtonPart) as ToggleButton;
            headerButton = GetTemplateChild(StringConstants.HeaderButtonPart) as ButtonBase;
            mainContentRow = GetTemplateChild(StringConstants.MainContentRowPart) as RowDefinition;

            if (expanderButton != null)
            {
                expanderButton.Checked += OnExpanderButtonChecked;
                expanderButton.Unchecked += OnExpanderButtonUnChecked;
                expanderButton.IsChecked = IsExpanded;
                if (IsExpanded)
                    ExpandControl();
                else
                    CollapseControl();
            }

            if (headerButton != null) headerButton.Click += OnHeaderButtonClick;
        }

        private void OnHeaderButtonClick(object sender, RoutedEventArgs e)
        {
            HeaderButtonCommand?.Execute(null);
        }

        private void ExpandControl()
        {
            if (mainContentRow == null || !mainContentRow.Height.Value.Equals(0d))
                return;
            VisualStateManager.GoToState(this, StringConstants.StateContentExpanded, true);
            mainContentRow.Height = new GridLength(1, GridUnitType.Auto);
        }

        private void CollapseControl()
        {
            if (mainContentRow == null || mainContentRow.Height.Value.Equals(0d))
                return;
            VisualStateManager.GoToState(this, StringConstants.StateContentCollapsed, true);
            mainContentRow.Height = new GridLength(0d);
        }

        private void OnExpanderButtonUnChecked(object sender, RoutedEventArgs e)
        {
            IsExpanded = false;
            CollapseControl();
        }

        private void OnExpanderButtonChecked(object sender, RoutedEventArgs e)
        {
            IsExpanded = true;
            ExpandControl();
        }
    }
}