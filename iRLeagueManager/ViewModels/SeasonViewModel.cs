﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using iRLeagueManager.Models;
using iRLeagueManager.Models.Sessions;
using iRLeagueManager.Models.Results;
using iRLeagueManager.Models.Reviews;
using iRLeagueManager.Data;
using iRLeagueManager.Interfaces;
using System.Windows.Input;

namespace iRLeagueManager.ViewModels
{
    public class SeasonViewModel : ContainerModelBase<SeasonModel>
    {
        public SeasonModel Model
        {
            get => Source;
            private set
            {
                if (SetSource(value))
                {
                    OnPropertyChanged(null);
                }
            }
        }

        public string SeasonName { get => Model?.SeasonName; set => Model.SeasonName = value; }

        public ObservableCollection<ScheduleInfo> Schedules => Model?.Schedules;

        public ObservableCollection<ResultInfo> Results => Model?.Results;

        public ObservableCollection<IncidentReviewInfo> Reviews => Model?.Reviews;

        public DateTime? SeasonStart => Model?.SeasonStart;

        public DateTime? SeasonEnd => Model?.SeasonEnd;

        private LeagueContext LeagueContext => GlobalSettings.LeagueContext;

        public SeasonViewModel() : base()
        {
            SeasonModel.GetTemplate();
        }

        public SeasonViewModel(SeasonModel source) : base(source) { }

        public async void Load(int seasonId)
        {
            if (Model != null && Model.SeasonId == seasonId)
            {
                IsLoading = true;
                try
                {
                    await LeagueContext.UpdateModelAsync(Model);
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
            else
            {
                IsLoading = true;
                try
                {
                    Model = SeasonModel.GetTemplate();
                    Model = await LeagueContext.GetModelAsync<SeasonModel>(seasonId);
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
        }

        public async void Load(SeasonInfo season)
        {
            if (Model == null || Model?.SeasonId != season?.SeasonId)
            {
                IsLoading = true;
                try
                {
                    Model = Model = SeasonModel.GetTemplate(season);
                    Model = await LeagueContext.GetModelAsync<SeasonModel>(season.SeasonId.Value);
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
            else
            {
                Load(season.SeasonId.Value);
            }
        }
    }
}