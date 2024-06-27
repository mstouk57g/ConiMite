using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.ApplicationModel;
using Rect = Windows.Foundation.Rect;
using Microsoft.UI.Windowing; //�������
using Windows.Foundation.Collections;
using Microsoft.UI.Input;
using Microsoft.UI.Composition.SystemBackdrops;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FirstWinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private AppWindow m_AppWindow;
        private Window m_window;
        public MainWindow()
        {
            this.InitializeComponent();
            // ��ʼ��һ�����߰����
            m_AppWindow = this.AppWindow;
            m_AppWindow.Changed += AppWindow_Changed; // ���ϳ�ʼ��
            Activated += MainWindow_Activated;
            AppTitleBar.SizeChanged += AppTitleBar_SizeChanged;
            AppTitleBar.Loaded += AppTitleBar_Loaded;

            ExtendsContentIntoTitleBar = true;
            if (ExtendsContentIntoTitleBar == true)
            {
                m_AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
            }
            TitleBarTextBlock.Text = AppInfo.Current.DisplayInfo.DisplayName;
            ExtendsContentIntoTitleBar = true; // ��ҳ����չ���������У������˴�ͳ�ı�������
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            m_window = new AnotherWindow();
            m_window.Activate();
        }
        private void AppTitleBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (ExtendsContentIntoTitleBar == true)
            {
                // ��ҳ�滭����չ����������ʱ�򣬳�ʼ������������
                SetRegionsForCustomTitleBar();
            }
        }

        private void AppTitleBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ExtendsContentIntoTitleBar == true)
            {
                // ��ҳ�滭����չ����������ǰ���£������ڴ�С�仯ʱ���½�������
                SetRegionsForCustomTitleBar();
            }
        }

        private void SetRegionsForCustomTitleBar()
        {
            // ��������������򵽵��ж��
            // �������� �����Ǹ���ť�������Ǹ���Ҫ���û�����

            double scaleAdjustment = AppTitleBar.XamlRoot.RasterizationScale;

            RightPaddingColumn.Width = new GridLength(m_AppWindow.TitleBar.RightInset / scaleAdjustment);
            LeftPaddingColumn.Width = new GridLength(m_AppWindow.TitleBar.LeftInset / scaleAdjustment);

            // ��ȡ��ť���ε�����
            GeneralTransform transform = this.myButton.TransformToVisual(null);
            Rect bounds = transform.TransformBounds(new Rect(0, 0,
                                                             this.myButton.ActualWidth,
                                                             this.myButton.ActualHeight));
            Windows.Graphics.RectInt32 myButton = GetRect(bounds, scaleAdjustment);

            // ���������м��������ؼ�������5�о͸��Ƽ��Σ�Ȼ��ѱ������ĳɿؼ���Name�����Ū���������򰴵������Ǹ�var��߾��У�����������Ļ�

            var rectArray = new Windows.Graphics.RectInt32[] { myButton };

            InputNonClientPointerSource nonClientInputSrc =
                InputNonClientPointerSource.GetForWindowId(this.AppWindow.Id);
            nonClientInputSrc.SetRegionRects(NonClientRegionKind.Passthrough, rectArray);
        }

        private Windows.Graphics.RectInt32 GetRect(Rect bounds, double scale)
        {
            // Rect�¾�������
            return new Windows.Graphics.RectInt32(
                _X: (int)Math.Round(bounds.X * scale),
                _Y: (int)Math.Round(bounds.Y * scale),
                _Width: (int)Math.Round(bounds.Width * scale),
                _Height: (int)Math.Round(bounds.Height * scale)
            );
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            // �����ڲ��ʱŪ�ɻ�ɫ�ģ��ʱ�Ͳ�Ū�ɻ�ɫ��
            if (args.WindowActivationState == WindowActivationState.Deactivated)
            {
                TitleBarTextBlock.Foreground =
                    (SolidColorBrush)App.Current.Resources["WindowCaptionForegroundDisabled"];
            }
            else
            {
                TitleBarTextBlock.Foreground =
                    (SolidColorBrush)App.Current.Resources["WindowCaptionForeground"];
            }
        }
        private void AppWindow_Changed(AppWindow sender, AppWindowChangedEventArgs args)
        {
            if (args.DidPresenterChange)
            {
                switch (sender.Presenter.Kind)
                {
                    case AppWindowPresenterKind.CompactOverlay:
                        // ������ͼ�У������Զ���ĵı�����������ʹ��ϵͳ�ı�����
                        // ��Ϊ����ʱ���Զ���ı������ᱻ������ͨ�Ŀؼ����ź�
                        // ��Ȼ��������Ҳ�У� Ч��Ҳ��˶���
                        AppTitleBar.Visibility = Visibility.Collapsed; // �����Զ��������
                        sender.TitleBar.ResetToDefault(); // ʹ��ϵͳĬ�ϱ�����
                        break;

                    case AppWindowPresenterKind.FullScreen:
                        // ȫ����ʱ��Ҳ�����Զ���ı���������Ϊ�Զ���ı�����Ҳ�ᱻ������ͨ�Ŀؼ����ź�
                        AppTitleBar.Visibility = Visibility.Collapsed; // �����Զ��������
                        sender.TitleBar.ExtendsContentIntoTitleBar = true; // ����������չ����������
                        break;

                    case AppWindowPresenterKind.Overlapped:
                        // �ص���ʱ�򣨾��Ƿǳ���ͨ������������ӣ���ʹ�õ��������Լ��Զ���ı�����
                        AppTitleBar.Visibility = Visibility.Visible;
                        sender.TitleBar.ExtendsContentIntoTitleBar = true;
                        break;

                    default:
                        // ʹ��ϵͳĬ�ϱ�����
                        sender.TitleBar.ResetToDefault();
                        break;
                }
            }
        }

        private void SwitchPresenter(object sender, RoutedEventArgs e)
        {
            if (AppWindow != null)
            {
                AppWindowPresenterKind newPresenterKind;
                switch ((sender as Button).Name)
                {
                    case "CompactoverlaytBtn": // ����
                        newPresenterKind = AppWindowPresenterKind.CompactOverlay;
                        break;

                    case "FullscreenBtn": // ȫ��
                        newPresenterKind = AppWindowPresenterKind.FullScreen;
                        break;

                    case "OverlappedBtn": // �ص��������㲻������ʱ��
                        newPresenterKind = AppWindowPresenterKind.Overlapped;
                        break;

                    default: // ɶҲ���ǣ�ʹ��ϵͳĬ�ϣ��������ϵͳ������
                        newPresenterKind = AppWindowPresenterKind.Default;
                        break;
                }

                // ��������ģʽ���ְ������ģʽ�İ�ť���͹���Ĭ�ϵ�ģʽ��ϵͳ�����ģ�
                if (newPresenterKind == AppWindow.Presenter.Kind)
                {
                    AppWindow.SetPresenter(AppWindowPresenterKind.Default);
                }
                else
                {
                    // ������ǣ����л�
                    AppWindow.SetPresenter(newPresenterKind);
                }
            }
        }
        private void SetBackdrop(object sender, RoutedEventArgs e)
        {
            if (AppWindow != null)
            {
                switch ((sender as Button).Name)
                {
                    case "MicaBaseBtn": // ��ťMicaBase
                        SystemBackdrop = new MicaBackdrop()
                            { Kind = MicaKind.Base }; //���и���
                        break;
                    case "MicaBaseAltBtn": // ��ťMicaBaseAlt
                        SystemBackdrop = new MicaBackdrop()
                            { Kind = MicaKind.BaseAlt }; //���и���
                        break;
                    case "AcrylicBtn": // ��ťAcrylic
                        SystemBackdrop = new DesktopAcrylicBackdrop(); //���и���
                        break;
                }
            }
        }
    }
}
