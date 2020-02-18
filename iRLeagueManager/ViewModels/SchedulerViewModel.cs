﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using System.IO;
using Microsoft.Win32;

using iRLeagueManager;
using iRLeagueManager.Models;
using iRLeagueManager.Models.Results;
using iRLeagueManager.Models.Sessions;
using iRLeagueManager.Data;
using iRLeagueManager.Services;
using System.Collections;

namespace iRLeagueManager.ViewModels
{
    public class SchedulerViewModel : ViewModelBase, INotifyPropertyChanged//, INotifyCollectionChanged, IEnumerable<ScheduleViewModel>
    {
        private LeagueContext LeagueContext => GlobalSettings.LeagueContext;

        private ObservableModelCollection<ScheduleViewModel, ScheduleModel> schedules;
        public ObservableModelCollection<ScheduleViewModel, ScheduleModel> Schedules
        {
            get => schedules;
            protected set
            {
                if (SetValue(ref schedules, value, (t, v) => t.GetSource().Equals(v.GetSource())))
                {
                    OnPropertyChanged(null);
                }
            }
        }

        private SeasonModel season;
        public SeasonModel Season { get => season; protected set => SetValue(ref season, value); }

        private ResultModel currentResult;
        public ResultModel CurrentResult { get => currentResult; set => SetValue(ref currentResult, value); }

        public ICommand UploadFileCmd { get; protected set; }
        public ICommand CreateScheduleCmd { get; protected set; }

        //public event NotifyCollectionChangedEventHandler CollectionChanged
        //{
        //    add
        //    {
        //        ((INotifyCollectionChanged)Schedules).CollectionChanged += value;
        //    }

        //    remove
        //    {
        //        ((INotifyCollectionChanged)Schedules).CollectionChanged -= value;
        //    }
        //}

        public SchedulerViewModel()
        {
            Schedules = new ObservableModelCollection<ScheduleViewModel, ScheduleModel>(new ScheduleModel[] { ScheduleModel.GetTemplate() });
            CreateScheduleCmd = new RelayCommand(o => CreateSchedule(), o => Season != null);

            UploadFileCmd = new RelayCommand(o =>
            {

            }, o => CurrentResult?.Session != null);
            //UploadFileCmd = new RelayCommand(o => { }, o => false);
        }

        public async void CreateSchedule()
        {
            var newSchedule = new ScheduleModel()
            {
                Name = "New Schedule"
            };

            Season.Schedules.Add(newSchedule);
            await LeagueContext.UpdateModelAsync(Season);
            //Load(Season);
        }

        public async void Load(SeasonModel season)
        {
            var loadedModels = new List<ScheduleModel>();
            Season = season;
            if (season == null || season.Schedules.Count == 0)
            {
                Schedules.UpdateSource(new ScheduleModel[] { ScheduleModel.GetTemplate() });
                return;
            }
            else
            {
                loadedModels = Schedules.Select(x => x.GetSource()).Where(x => season.Schedules.Select(y => y.ScheduleId).Contains(x.ScheduleId)).ToList();
            }

            var newIds = season.Schedules.Select(x => x.ScheduleId.Value).Except(loadedModels.Select(x => x.ScheduleId.Value)).ToList();

            List<ScheduleModel> schedules = new List<ScheduleModel>();

            if (loadedModels.Count() > 0)
            {
                schedules.AddRange(await LeagueContext.UpdateModelsAsync(loadedModels));
            }
            if (newIds.Count() > 0)
            {
                schedules.AddRange(await LeagueContext.GetModelsAsync<ScheduleModel>(newIds));
            }

            //var scheduleIds = season.Schedules.Select(x => x.ScheduleId.Value);
            //schedules = await LeagueContext.GetModelsAsync<ScheduleModel>(scheduleIds);
            schedules = schedules.OrderBy(x => x.ScheduleId).ToList();

            Schedules.UpdateSource(schedules);
            OnPropertyChanged(null);
        }

        public async void DeleteSchedule(ScheduleModel schedule)
        {
            if (schedule == null)
                return;

            try
            {
                Season.Schedules.Remove(Season.Schedules.SingleOrDefault(x => x.ScheduleId == schedule.ScheduleId));
                IsLoading = true;
                await LeagueContext.UpdateModelAsync(Season);
                IsLoading = false;
                Load(Season);
            }
            catch (Exception e)
            { 
                GlobalSettings.LogError(e);
            }
            finally
            {
                IsLoading = false;
            }
        }

        //public IEnumerator<ScheduleViewModel> GetEnumerator()
        //{
        //    return ((IEnumerable<ScheduleViewModel>)Schedules).GetEnumerator();
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return ((IEnumerable<ScheduleViewModel>)Schedules).GetEnumerator();
        //}

        public override void Refresh(string propertyName = "")
        {
            Load(Season);
            base.Refresh(propertyName);
        }
    }
}