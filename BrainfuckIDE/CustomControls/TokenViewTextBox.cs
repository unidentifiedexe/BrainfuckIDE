﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BrainfuckIDE.CustomControls
{
    /// <summary>
    /// このカスタム コントロールを XAML ファイルで使用するには、手順 1a または 1b の後、手順 2 に従います。
    ///
    /// 手順 1a) 現在のプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:BrainfuckIDE.CustomControls"
    ///
    ///
    /// 手順 1b) 異なるプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:BrainfuckIDE.Controls;assembly=BrainfuckIDE.CustomControls"
    ///
    /// また、XAML ファイルのあるプロジェクトからこのプロジェクトへのプロジェクト参照を追加し、
    /// リビルドして、コンパイル エラーを防ぐ必要があります:
    ///
    ///     ソリューション エクスプローラーで対象のプロジェクトを右クリックし、
    ///     [参照の追加] の [プロジェクト] を選択してから、このプロジェクトを参照し、選択します。
    ///
    ///
    /// 手順 2)
    /// コントロールを XAML ファイルで使用します。
    ///
    ///     <MyNamespace:TokenViewTextBox/>
    ///
    /// </summary>
    public class TokenViewTextBox : TextBox
    {
        static TokenViewTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TokenViewTextBox), new FrameworkPropertyMetadata(typeof(TokenViewTextBox)));
        }



        public bool IsEdditing
        {
            get { return (bool)GetValue(IsEdditingProperty); }
            private set { SetValue(IsEdditingPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey IsEdditingPropertyKey = DependencyProperty.RegisterReadOnly
            (nameof(IsEdditing), typeof(bool), typeof(TokenViewTextBox), new PropertyMetadata(false, OnIsEdditingChanged));

        // Using a DependencyProperty as the backing store for IsEdditing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEdditingProperty = IsEdditingPropertyKey.DependencyProperty;
        //DependencyProperty.Register(nameof(IsEdditing), typeof(bool), typeof(MemoryTokenView), new PropertyMetadata(false, OnIsEdditingChnaged));

        private static void OnIsEdditingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //if (!(d is TokenViewTextBox owner)) return;
            //if (!(e.NewValue is bool val)) return;
            //owner.SetMainTextVisible(val);
        }

    }
}
