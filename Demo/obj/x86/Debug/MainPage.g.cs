﻿#pragma checksum "D:\Projects\(Repros)\TouchControls\Demo\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "457FCF2DA5ED2BA0AD0352427EA1BAF9"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Demo
{
    partial class MainPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                {
                    this.SplitView = (global::Windows.UI.Xaml.Controls.SplitView)(target);
                }
                break;
            case 2:
                {
                    this.HamburgerMenuButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 116 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.HamburgerMenuButton).Click += this.HamburgerMenuButton_Click;
                    #line default
                }
                break;
            case 3:
                {
                    this.SplitViewPane = (global::Windows.UI.Xaml.Controls.Grid)(target);
                    #line 31 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Grid)this.SplitViewPane).ManipulationCompleted += this.SplitViewPane_ManipulationCompleted;
                    #line default
                }
                break;
            case 4:
                {
                    this.samplesListBox = (global::Windows.UI.Xaml.Controls.ListBox)(target);
                    #line 70 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ListBox)this.samplesListBox).SelectionChanged += this.ListView_SelectionChanged;
                    #line default
                }
                break;
            case 5:
                {
                    this.SplitViewFrame = (global::Windows.UI.Xaml.Controls.Frame)(target);
                }
                break;
            case 6:
                {
                    global::Windows.UI.Xaml.Controls.Grid element6 = (global::Windows.UI.Xaml.Controls.Grid)(target);
                    #line 95 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Grid)element6).ManipulationCompleted += this.SplitViewOpener_ManipulationCompleted;
                    #line default
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

