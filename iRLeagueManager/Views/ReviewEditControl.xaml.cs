﻿using iRLeagueManager.Models.Members;
using iRLeagueManager.ViewModels;
using System;
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
    /// Interaktionslogik für ReviewEditControl.xaml
    /// </summary>
    public partial class ReviewEditControl : UserControl
    {
        public IncidentReviewViewModel IncidentReview => DataContext as IncidentReviewViewModel;

        public ReviewEditControl()
        {
            InitializeComponent();
        }

        private void MoveLeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && IncidentReview != null)
            {
                var selectedMembers = MemberSelect.SelectedItems.Cast<LeagueMember>();

                if (selectedMembers != null && selectedMembers.Count() > 0)
                {
                    foreach (var selectedMember in selectedMembers.ToList())
                    {
                        if (IncidentReview.InvolvedMembers.Contains(selectedMember) == false)
                            IncidentReview.InvolvedMembers.Add(selectedMember);
                    }
                }
            }
        }

        private void MoveRightButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && IncidentReview != null)
            {
                var selectedMembers = InvolvedMembers.SelectedItems.Cast<LeagueMember>();

                if (selectedMembers != null && selectedMembers.Count() > 0)
                {
                    foreach (var selectedMember in selectedMembers.ToList())
                    {
                        if (IncidentReview.InvolvedMembers.Contains(selectedMember))
                            IncidentReview.InvolvedMembers.Remove(selectedMember);
                    }
                }
            }
        }
    }
}