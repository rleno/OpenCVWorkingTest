﻿#pragma checksum "..\..\CameraControl.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "74AE295C5AD17FA31BB4BCA85679B902CB9B1747D342F578A678B362C18136E0"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Camera_EMGU_WPFSample;
using Emgu.CV.UI;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Camera_EMGU_WPFSample {
    
    
    /// <summary>
    /// CameraControl
    /// </summary>
    public partial class CameraControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 20 "..\..\CameraControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox Combo;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\CameraControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TextBlockFx;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\CameraControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TextBlockFy;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\CameraControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TextBlockFz;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\CameraControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TextBlockDx;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\CameraControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TextBlockDy;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\CameraControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TextBlockDz;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\CameraControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Emgu.CV.UI.ImageBox CapturedImageBox;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Camera-EMGU-WPFSample;component/cameracontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\CameraControl.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 10 "..\..\CameraControl.xaml"
            ((Camera_EMGU_WPFSample.CameraControl)(target)).Loaded += new System.Windows.RoutedEventHandler(this.UserControl_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Combo = ((System.Windows.Controls.ComboBox)(target));
            
            #line 22 "..\..\CameraControl.xaml"
            this.Combo.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.Combo_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.TextBlockFx = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.TextBlockFy = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.TextBlockFz = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.TextBlockDx = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 7:
            this.TextBlockDy = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.TextBlockDz = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 9:
            this.CapturedImageBox = ((Emgu.CV.UI.ImageBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
