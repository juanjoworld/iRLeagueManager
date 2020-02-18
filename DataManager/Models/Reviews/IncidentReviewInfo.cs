﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRLeagueManager.Models.Members;
using iRLeagueManager.Models;
using iRLeagueManager.Interfaces;

namespace iRLeagueManager.Models.Reviews
{
    public class IncidentReviewInfo : ModelBase, IReviewInfo, IHierarchicalModel
    {
        private int? reviewId;
        public int? ReviewId { get => reviewId; internal set { reviewId = value; OnPropertyChanged(); } }

        private LeagueMember author;
        public LeagueMember Author { get => author; internal set { author = value; OnPropertyChanged(); } }
        IAdmin IReviewInfo.Author => Author;

        private int onLap;
        public int OnLap { get => onLap; set { onLap = value; OnPropertyChanged(); } }

        private int corner;
        public int Corner { get => corner; set { corner = value; OnPropertyChanged(); } }

        string IHierarchicalModel.Description => "L" + OnLap.ToString() + " - C" + Corner.ToString();

        IEnumerable<object> IHierarchicalModel.Children => new object[0];
    }
}