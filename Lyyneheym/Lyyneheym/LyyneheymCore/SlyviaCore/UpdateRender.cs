﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Lyyneheym.LyyneheymCore.Utils;
using Lyyneheym.LyyneheymCore.ILPackage;

namespace Lyyneheym.LyyneheymCore.SlyviaCore
{
    /// <summary>
    /// 负责将场景动作转化为前端事物的类
    /// </summary>
    public class UpdateRender
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public UpdateRender()
        {
            // 初始化鼠标键位
            UpdateRender.KS_MOUSE_Dict.Add(MouseButton.Left, MouseButtonState.Released);
            UpdateRender.KS_MOUSE_Dict.Add(MouseButton.Middle, MouseButtonState.Released);
            UpdateRender.KS_MOUSE_Dict.Add(MouseButton.Right, MouseButtonState.Released);
            UpdateRender.KS_MOUSE_Dict.Add(MouseButton.XButton1, MouseButtonState.Released);
            UpdateRender.KS_MOUSE_Dict.Add(MouseButton.XButton2, MouseButtonState.Released);
        }
        
        /// <summary>
        /// 设置主窗体引用
        /// </summary>
        /// <param name="mw">主窗体引用</param>
        public void SetPlatformReference(MainWindow mw)
        {
            this.view = mw;
        }

        /// <summary>
        /// 设置运行时环境引用
        /// </summary>
        /// <param name="rm">运行时环境</param>
        public void SetRuntimeManagerReference(RuntimeManager rm)
        {
            this.runMana = rm;
        }

        /// <summary>
        /// 调用运行时环境计算表达式
        /// </summary>
        /// <param name="polish">逆波兰式</param>
        /// <returns>表达式的值</returns>
        public object CalculatePolish(string polish)
        {
            return this.runMana.CalculatePolish(polish);
        }

        /// <summary>
        /// 接受一个场景动作并演绎她
        /// </summary>
        /// <param name="action">场景动作实例</param>
        public void Accept(SceneAction action)
        {
            switch (action.aType)
            {
                case SActionType.act_bgm:
                    this.Bgm(action.argsDict["filename"], (float)this.CalculatePolish(action.argsDict["vol"]));
                    break;
                case SActionType.act_stopbgm:
                    this.Stopbgm();
                    break;
            }
        }

        public KeyStates GetKeyboardState(Key key)
        {
            if (UpdateRender.KS_KEY_Dict.ContainsKey(key) == false)
            {
                UpdateRender.KS_KEY_Dict.Add(key, KeyStates.None);
                return KeyStates.None;
            }
            return UpdateRender.KS_KEY_Dict[key];
        }

        public void SetKeyboardState(Key key, KeyStates state)
        {
            UpdateRender.KS_KEY_Dict[key] = state;
        }

        public MouseButtonState GetMouseButtonState(MouseButton key)
        {
            return UpdateRender.KS_MOUSE_Dict[key];
        }

        public void SetMouseButtonState(MouseButton key, MouseButtonState state)
        {
            UpdateRender.KS_MOUSE_Dict[key] = state;
        }

        public int GetMouseWheelDelta()
        {
            return UpdateRender.KS_MOUSE_WHEEL_DELTA;
        }

        public void SetMouseWheelDelta(int delta)
        {
            UpdateRender.KS_MOUSE_WHEEL_DELTA = delta;
        }

        /// <summary>
        /// 处理游戏窗体的鼠标按下信息
        /// </summary>
        /// <param name="e"></param>
        public void WMouseDownEventHandler(MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (this.view.BO_MessageBoxLayer.Visibility == System.Windows.Visibility.Hidden)
                {
                    this.view.BO_MainName.Visibility = this.view.BO_MainText.Visibility = this.view.BO_MsgTria.Visibility =
                        this.view.BO_MessageBoxLayer.Visibility = System.Windows.Visibility.Visible;

                }
                else
                {
                    this.view.BO_MainName.Visibility = this.view.BO_MainText.Visibility = this.view.BO_MsgTria.Visibility =
                        this.view.BO_MessageBoxLayer.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            
        }

        public void WMouseUpEventHandler(MouseButtonEventArgs e)
        {

        }

        #region 文字层相关
        /// <summary>
        /// 隐藏小三角
        /// </summary>
        public void HideMessageTria()
        {
            this.view.BO_MsgTria.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// 显示小三角
        /// </summary>
        /// <param name="opacity">透明度</param>
        public void ShowMessageTria(double opacity = 1.0f)
        {
            this.view.BO_MsgTria.Opacity = opacity;
            this.view.BO_MsgTria.Visibility = Visibility.Visible;
        }

        public void BeginMessageTriaUpDownAnimation()
        {

        }

        public void EndMessageTriaUpDownAnimation()
        {

        }

        /// <summary>
        /// 设置小三角位置
        /// </summary>
        /// <param name="pos"></param>
        public void SetMessageTriaPosition(Point pos)
        {
            Canvas.SetLeft(this.view.BO_MsgTria, pos.X);
            Canvas.SetTop(this.view.BO_MsgTria, pos.Y);
        }
        #endregion

        /// <summary>
        /// 为更新器设置作用窗体
        /// </summary>
        /// <param name="mw">窗体引用</param>
        public void SetMainWindow(MainWindow mw)
        {
            this.view = mw;
        }

        /// <summary>
        /// 主窗体引用
        /// </summary>
        private MainWindow view = null;

        /// <summary>
        /// 运行时环境引用
        /// </summary>
        private RuntimeManager runMana = null;

        /// <summary>
        /// 音乐引擎
        /// </summary>
        private Musician musician = Musician.GetInstance();

        /// <summary>
        /// 资源管理器
        /// </summary>
        private ResourceManager resMana = ResourceManager.GetInstance();

        private ScreenManager scrMana = ScreenManager.GetInstance();
        private ViewManager viewMana = ViewManager.GetInstance();

        #region 演绎函数
        /// <summary>
        /// 结束程序
        /// </summary>
        public void Shutdown()
        {
            DebugUtils.ConsoleLine("Shutdown is called", "UpdateRender", OutputStyle.Important);
            this.view.Close();
        }

        /// <summary>
        /// 跳过所有动画
        /// </summary>
        public void SkipAllAnimation()
        {
            SpriteAnimation.SkipAllAnimation();
        }

        private void Dialog()
        {

        }

        private void DialogTerminator()
        {

        }

        private void A()
        {

        }

        /// <summary>
        /// 显示背景
        /// </summary>
        /// <param name="id"></param>
        /// <param name="filename"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="opacity"></param>
        /// <param name="xscale"></param>
        /// <param name="yscale"></param>
        /// <param name="ro"></param>
        /// <param name="anchor"></param>
        /// <param name="cut"></param>
        private void Background(int id, string filename, double x, double y, double opacity, double xscale, double yscale, double ro, SpriteAnchorType anchor, Int32Rect cut)
        {
            this.scrMana.AddBackground(id, filename, x, y, id, ro, opacity, anchor, cut);
            this.viewMana.Draw(id, ResourceType.Background);
        }

        /// <summary>
        /// 显示图片
        /// </summary>
        /// <param name="id"></param>
        /// <param name="filename"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="opacity"></param>
        /// <param name="xscale"></param>
        /// <param name="yscale"></param>
        /// <param name="ro"></param>
        private void Picture(int id, string filename, double x, double y, double opacity, double xscale, double yscale, double ro, SpriteAnchorType anchor, Int32Rect cut)
        {
            this.scrMana.AddPicture(id, filename, x, y, id, ro, opacity, anchor, cut);
            this.viewMana.Draw(id, ResourceType.Pictures);
        }

        /// <summary>
        /// 移动图片
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rType"></param>
        /// <param name="property"></param>
        /// <param name="fromValue"></param>
        /// <param name="toValue"></param>
        /// <param name="acc"></param>
        /// <param name="duration"></param>
        private void Move(int id, ResourceType rType, string property, double fromValue, double toValue, double acc, Duration duration)
        {
            MySprite actionSprite = this.viewMana.GetSprite(id, rType);
            SpriteDescriptor descriptor = this.scrMana.GetDescriptor(id, rType);
            if (actionSprite == null)
            {
                DebugUtils.ConsoleLine(String.Format("Ignored move (sprite is null): {0}, {1}", rType.ToString(), id),
                    "UpdateRender", OutputStyle.Warning);
                return;
            }
            switch (property)
            {
                case "x":
                    SpriteAnimation.XYMoveAnimation(actionSprite, duration, fromValue, toValue, actionSprite.displayY, actionSprite.displayY, acc, 0);
                    break;
                case "y":
                    SpriteAnimation.XYMoveAnimation(actionSprite, duration, actionSprite.displayX, actionSprite.displayX, fromValue, toValue, 0, acc);
                    break;
                case "o":
                case "opacity":
                    SpriteAnimation.OpacityAnimation(actionSprite, duration, fromValue, toValue, acc);
                    break;
                case "a":
                case "angle":
                    SpriteAnimation.RotateAnimation(actionSprite, duration, fromValue, toValue, acc);
                    break;
                case "s":
                case "scale":
                    SpriteAnimation.ScaleAnimation(actionSprite, duration, fromValue, toValue, fromValue, toValue, acc, acc);
                    break;
                case "sx":
                case "scalex":
                    SpriteAnimation.ScaleAnimation(actionSprite, duration, fromValue, toValue, descriptor.ScaleY, descriptor.ScaleY, acc, 0);
                    break;
                case "sy":
                case "scaley":
                    SpriteAnimation.ScaleAnimation(actionSprite, duration, descriptor.ScaleX, descriptor.ScaleX, fromValue, toValue, 0, acc);
                    break;
                default:
                    DebugUtils.ConsoleLine(String.Format("Move instruction without valid parameters: {0}", property),
                        "UpdateRender", OutputStyle.Warning);
                    break;
            }
        }

        /// <summary>
        /// 移除图片
        /// </summary>
        private void Deletepicture(int id, ResourceType rType)
        {
            this.viewMana.RemoveSprite(id, rType);
        }

        /// <summary>
        /// 显示立绘
        /// </summary>
        /// <param name="id"></param>
        /// <param name="filename"></param>
        /// <param name="locationStr"></param>
        /// <param name="opacity"></param>
        /// <param name="xscale"></param>
        /// <param name="yscale"></param>
        /// <param name="ro"></param>
        /// <param name="anchor"></param>
        /// <param name="cut"></param>
        private void Cstand(int id, string filename, string locationStr, double opacity, double xscale, double yscale, double ro, SpriteAnchorType anchor, Int32Rect cut)
        {
            CharacterStandType cst;
            switch (locationStr)
            {
                case "l":
                case "left":
                    cst = CharacterStandType.Left;
                    break;
                case "r":
                case "right":
                    cst = CharacterStandType.Right;
                    break;
                case "ml":
                case "midleft":
                    cst = CharacterStandType.MidLeft;
                    break;
                case "mr":
                case "midright":
                    cst = CharacterStandType.MidRight;
                    break;
                default:
                    cst = CharacterStandType.Mid;
                    break;
            }
            this.scrMana.AddCharacterStand(id, filename, cst, id, ro, opacity, anchor, cut);
            this.viewMana.Draw(id, ResourceType.Stand);
        }

        /// <summary>
        /// 移除立绘
        /// </summary>
        private void Deletecstand(CharacterStandType cst)
        {
            this.viewMana.RemoveSprite(Convert.ToInt32(cst), ResourceType.Stand);
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="volume"></param>
        private void Se(string resourceName, float volume)
        {
            var seKVP = this.resMana.GetSE(resourceName);
            this.musician.PlaySE(seKVP.Key, seKVP.Value, volume);
        }

        /// <summary>
        /// 演绎函数：播放BGM，如果是同一个文件将不会重新播放
        /// </summary>
        /// <param name="resourceName">要播放的BGM名字</param>
        /// <param name="volume">音量</param>
        private void Bgm(string resourceName, float volume)
        {
            // 如果当前BGM就是此BGM就只调整音量
            if (this.musician.currentBGM != resourceName)
            {
                var bgmKVP = this.resMana.GetBGM(resourceName);
                this.musician.PlayBGM(resourceName, bgmKVP.Key, bgmKVP.Value, volume);
            }
            else
            {
                this.musician.SetBGMVolume(volume);
            }
        }

        /// <summary>
        /// 演绎函数：停止BGM
        /// </summary>
        private void Stopbgm()
        {
            this.musician.StopAndReleaseBGM();
        }

        private void Vocal()
        {

        }

        private void Stopvocal()
        {

        }

        private void Title()
        {

        }

        private void Menu()
        {

        }

        private void Save()
        {

        }

        private void Load()
        {

        }

        private void Lable()
        {

        }

        private void Jump()
        {

        }

        private void For()
        {

        }

        private void Endfor()
        {

        }

        private void If()
        {

        }

        private void Function()
        {

        }

        private void Endfunction()
        {

        }

        private void Var()
        {

        }

        private void Break()
        {

        }

        private void Wait()
        {

        }

        private void Branch()
        {

        }

        private void Call()
        {

        }

        private void Titlepoint()
        {

        }

        private void MsgLayer()
        {

        }

        private void MsgLayerOpt()
        {

        }
        #endregion

        #region 键位按钮状态
        public static int KS_MOUSE_WHEEL_DELTA = 0;
        private static Dictionary<MouseButton, MouseButtonState> KS_MOUSE_Dict = new Dictionary<MouseButton, MouseButtonState>();
        private static Dictionary<Key, KeyStates> KS_KEY_Dict = new Dictionary<Key, KeyStates>();
        #endregion
    }
}
