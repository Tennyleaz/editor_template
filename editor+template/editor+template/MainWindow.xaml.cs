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

namespace editor_template
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        List<DeviceUnit> DeviceUnitList;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnAdd_OnClick(object sender, RoutedEventArgs e)
        {
            DeviceUnit DeviceUnit = new DeviceUnit();
            myPanel.Children.Add(DeviceUnit);
        }

        private void BtnClear_OnClick(object sender, RoutedEventArgs e)
        {
            myPanel.Children.Clear();
        }
    }
}
