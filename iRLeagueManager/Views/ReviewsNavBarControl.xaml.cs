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

namespace iRLeagueManager.Views
{
    /// <summary>
    /// Interaktionslogik für ReviewsNavBarControl.xaml
    /// </summary>
    public partial class ReviewsNavBarControl : UserControl
    {
        public event SelectionChangedEventHandler SelectedReviewChanged;
        public ReviewsNavBarControl()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedReviewChanged?.Invoke(sender, e);
        }
    }
}
