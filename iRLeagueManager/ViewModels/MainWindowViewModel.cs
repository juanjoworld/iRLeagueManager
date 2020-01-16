﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using iRLeagueManager.Data;
using iRLeagueManager.Enums;
using iRLeagueManager.Interfaces;
using iRLeagueManager.Models;
using iRLeagueManager.ViewModels;
using iRLeagueManager.Models.Database;
using iRLeagueManager.Models.Sessions;
using iRLeagueManager.User;

namespace iRLeagueManager.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        //private LeagueDatabase _leagueDb;
        //public LeagueDatabase LeagueDb { get => _leagueDb; set { _leagueDb = value; NotifyPropertyChanged(); } }
        private LeagueContext LeagueContext => GlobalSettings.LeagueContext;
        //public LeagueContext LeagueContext { get => leagueContext; private set { leagueContext = value; OnPropertyChanged(); } }

        private UserContext UserContext => GlobalSettings.UserContext;

        private LoginViewModel userLogin;
        public LoginViewModel UserLogin { get => userLogin; set => SetValue(ref userLogin, value); }

        public UserModel CurrentUser => UserContext?.CurrentUser;

        private DatabaseStatusModel dbStatus;
        public DatabaseStatusModel DbStatus { get => dbStatus; set => SetValue(ref dbStatus, value); }

        private ViewModelBase contentViewModel;
        public ViewModelBase ContentViewModel { get => contentViewModel; set => SetValue(ref contentViewModel, value); }

        private ObservableCollection<SeasonInfo> seasonList;
        public ObservableCollection<SeasonInfo> SeasonList { get => seasonList; set => SetValue(ref seasonList, value); }

        public ICommand SchedulesButtonCmd { get; }

        private SeasonInfo selectedSeason;
        public SeasonInfo SelectedSeason
        {
            get => selectedSeason;
            set
            {
                if (SetValue(ref selectedSeason, value) && selectedSeason?.SeasonId != null)
                {
                    CurrentSeason.Load(selectedSeason);
                }
            }
        }

        public SeasonViewModel CurrentSeason { get; }

        public MainWindowViewModel()
        {
            DbStatus = new DatabaseStatusModel();
            SeasonList = new ObservableCollection<SeasonInfo>(new List<SeasonInfo>() { new SeasonInfo() { SeasonName = "Loading..." } });
            SelectedSeason = SeasonList.First();
            CurrentSeason = new SeasonViewModel();
        }

        public async void Load()
        {
            if (LeagueContext == null)
            {
                GlobalSettings.SetGlobalLeagueContext(new LeagueContext());
            }

            LeagueContext.AddStatusItem(DbStatus);

            //LeagueContext.AddStatusItem(DbStatus);
            SeasonList = new ObservableCollection<SeasonInfo>(await LeagueContext.GetSeasonListAsync());
            SelectedSeason = SeasonList.FirstOrDefault();
        }

        public void LogInOut()
        {
            if (CurrentUser != null)
            {
                UserContext.UserLogout();
            }
            else
            {
                UserLogin.Open();
            }
            OnPropertyChanged(null);
        }

        public void Dispose()
        {
            if (LeagueContext != null)
            {
                LeagueContext.RemoveStatusItem(DbStatus);
            }
        }
    }
}
