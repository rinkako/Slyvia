﻿using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Yuri.PlatformCore;
using Yuri.PageView;
using Yuri.Utils;

namespace Yuri
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 导演类的引用
        /// </summary>
        private readonly Director core = Director.GetInstance();

        /// <summary>
        /// Alt键正在被按下的标记
        /// </summary>
        private static bool altDown = false;
        
        /// <summary>
        /// 构造器
        /// </summary>
        public MainWindow()
        {
            CommonUtils.ConsoleLine("MWnd Initialization begin", "MainWindow", OutputStyle.Important);
            InitializeComponent();
            ViewManager.SetWindowReference(this);
            this.Title = GlobalDataContext.GAME_TITLE_NAME;
            this.Width = GlobalDataContext.GAME_WINDOW_WIDTH;
            this.Height = GlobalDataContext.GAME_WINDOW_ACTUALHEIGHT;
            this.mainCanvas.Width = GlobalDataContext.GAME_WINDOW_WIDTH;
            this.mainCanvas.Height = GlobalDataContext.GAME_WINDOW_HEIGHT;
            this.ResizeMode = GlobalDataContext.GAME_WINDOW_RESIZEABLE ? ResizeMode.CanResize : ResizeMode.NoResize;
            this.core.SetStagePageReference(new StagePage());
            //this.mainFrame.Content = new PageView.Stage3D();
            this.mainFrame.Width = GlobalDataContext.GAME_WINDOW_WIDTH;
            this.mainFrame.Height = GlobalDataContext.GAME_WINDOW_HEIGHT;
            this.mainFrame.Content = ViewPageManager.RetrievePage(GlobalDataContext.FirstViewPage);
            //this.upperFrame.Content = new PageView.SLPage(false);
            // 预注册保存和读取页面
            ViewPageManager.RegisterPage("SavePage", new SLPage(isSave: true));
            ViewPageManager.RegisterPage("LoadPage", new SLPage(isSave: false));
            CommonUtils.ConsoleLine("MWnd Initialization finish", "MainWindow", OutputStyle.Important);
        }
        
        #region 窗体监听事件
        /// <summary>
        /// 事件：窗体关闭
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            PersistenceContext.SaveToSteadyMemory();
            this.core.DisposeResource();
        }

        /// <summary>
        /// 事件：键盘即将按下按钮
        /// </summary>
        private void window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                MainWindow.altDown = true;
            }
            else if (e.SystemKey == Key.F4 && MainWindow.altDown)
            {
                this.core.GetMainRender().Shutdown();
            }
            else if (e.SystemKey == Key.Enter && MainWindow.altDown)
            {
                if (Director.FullScreen == true)
                {
                    this.WindowScreenTransform();
                }
                else
                {
                    this.FullScreenTransform();
                }
                Director.FullScreen = !Director.FullScreen;
            }
        }

        /// <summary>
        /// 事件：键盘即将松开按钮
        /// </summary>
        private void window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                MainWindow.altDown = false;
            }
        }
        
        /// <summary>
        /// 事件：窗体大小改变
        /// </summary>
        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Width = e.NewSize.Width;
            this.Height = (double)GlobalDataContext.GAME_WINDOW_ACTUALHEIGHT * this.Width / (double)GlobalDataContext.GAME_WINDOW_WIDTH;
        }

        /// <summary>
        /// 事件：窗口尺寸模式改变
        /// </summary>
        private void window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.FullScreenTransform();
            }
        }
        #endregion

        #region 辅助函数
        /// <summary>
        /// 切换到全屏模式
        /// </summary>
        public void FullScreenTransform()
        {
            this.WindowState = WindowState.Normal;
            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;
            this.Topmost = true;
            this.Left = 0.0;
            this.Top = 0.0;
            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Height = SystemParameters.PrimaryScreenHeight;
        }

        /// <summary>
        /// 切换到窗口模式
        /// </summary>
        public void WindowScreenTransform()
        {
            this.WindowState = WindowState.Normal;
            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.ResizeMode = ResizeMode.CanResize;
            this.Topmost = false;
            this.Left = 0.0;
            this.Top = 0.0;
            this.Width = GlobalDataContext.GAME_WINDOW_WIDTH;
            this.Height = GlobalDataContext.GAME_WINDOW_ACTUALHEIGHT;
        }
        #endregion
    }
}
