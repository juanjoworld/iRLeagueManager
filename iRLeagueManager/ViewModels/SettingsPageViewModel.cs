﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Data;
using System.ComponentModel;

using iRLeagueManager.Models;
using iRLeagueManager.Models.Results;
using iRLeagueManager.Models.Sessions;
using iRLeagueManager.ViewModels.Collections;

namespace iRLeagueManager.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private SeasonViewModel season;
        public SeasonViewModel Season { get => season; set => SetValue(ref season, value); }

        private readonly ObservableModelCollection<ScoringViewModel,ScoringModel> scorings;
        public ObservableModelCollection<ScoringViewModel, ScoringModel> Scorings
        {
            get
            {
                if (scorings.GetSource() != Season?.Scorings)
                    scorings.UpdateSource(Season?.Scorings);
                return scorings;
            }
        }

        private readonly ObservableModelCollection<ScoringTableViewModel, ScoringTableModel> scoringTables;
        public ObservableModelCollection<ScoringTableViewModel, ScoringTableModel> ScoringTables
        {
            get
            {
                if (scoringTables.GetSource() != Season?.ScoringTables)
                    scoringTables.UpdateSource(Season?.ScoringTables);
                return scoringTables;
            }
        }

        public ICommand AddScoringCmd { get; }
        public ICommand DeleteScoringCmd { get; }
        public ICommand AddScoringTableCmd { get; }
        public ICommand DeleteScoringTableCmd { get; }

        public ICommand SaveChangesCmd { get; }

        public SettingsPageViewModel() : base()
        {
            scorings = new ObservableModelCollection<ScoringViewModel, ScoringModel>(constructorAction: x => x.SetScoringsList(Scorings.GetSource()));
            scoringTables = new ObservableModelCollection<ScoringTableViewModel, ScoringTableModel>(x => x.SetScoringsList(Scorings));
            Season = new SeasonViewModel();
            AddScoringCmd = new RelayCommand(o => AddScoring(), o => Season != null);
            AddScoringTableCmd = new RelayCommand(o => AddScoringTable(), o => Season != null);
            DeleteScoringCmd = new RelayCommand(o => DeleteScoring((o as ScoringViewModel).Model), o => o != null);
            DeleteScoringTableCmd = new RelayCommand(o => DeleteScoringTable((o as ScoringTableViewModel).Model), o => o != null);
            SaveChangesCmd = new RelayCommand(o =>
            {
                foreach (var scoring in Scorings)
                {
                    scoring.SaveChanges();
                }
                foreach (var scoringTable in ScoringTables)
                {
                    scoringTable.SaveChanges();
                }
            });
        }

        public async Task Load(SeasonModel season)
        {
            if (season == null)
            {
                Season = null;
                StatusMsg = "No season loaded";
                OnPropertyChanged(null);
                return;
            }

            //Season = new SeasonViewModel(season);
            Season.UpdateSource(season);
            OnPropertyChanged(null);

            await LeagueContext.GetModelAsync<SeasonModel>(season.ModelId);
        }

        public override async Task Refresh()
        {
            LeagueContext.ModelManager.ForceExpireModels<SeasonModel>();
            LeagueContext.ModelManager.ForceExpireModels<ScoringModel>();
            LeagueContext.ModelManager.ForceExpireModels<ScoringTableModel>();
            await Load(Season.Model);
            await base.Refresh();
        }

        public async void AddScoring()
        {
            try
            {
                var scoring = new ScoringModel() { Season = this.Season.Model, Name = "New Scoring" };
                scoring = await LeagueContext.AddModelAsync(scoring);
                //Season.Scorings.Add(scoring);
                await LeagueContext.UpdateModelAsync(Season.Model);
                Scorings.UpdateCollection();
            }
            catch (Exception e)
            {
                GlobalSettings.LogError(e);
            }
            finally
            {

            }
        }

        public async void DeleteScoring(ScoringModel scoring)
        {
            if (scoring == null)
                return;

            try
            {
                await LeagueContext.DeleteModelsAsync(scoring);
                //Season.Scorings.Remove(scoring);
                await LeagueContext.UpdateModelAsync(Season.Model);
                Scorings.UpdateCollection();
            }
            catch (Exception e)
            {
                GlobalSettings.LogError(e);
            }
            finally
            {

            }
        }

        public async void AddScoringTable()
        {
            try
            {
                var scoringTable = new ScoringTableModel() { Name = "New Scoring Table" };
                //scoringTable = await LeagueContext.AddModelAsync(scoringTable);
                Season.ScoringTables.Add(scoringTable);
                await LeagueContext.UpdateModelAsync(Season.Model);
                ScoringTables.UpdateCollection();
            }
            catch (Exception e)
            {
                GlobalSettings.LogError(e);
            }
            finally
            {

            }
        }

        public async void DeleteScoringTable(ScoringTableModel scoringTable)
        {
            if (scoringTable == null)
                return;

            try
            {
                await LeagueContext.DeleteModelsAsync(scoringTable);
                //Season.ScoringTables.Remove(scoringTable);
                await LeagueContext.UpdateModelAsync(Season.Model);
                ScoringTables.UpdateCollection();
            }
            catch (Exception e)
            {
                GlobalSettings.LogError(e);
            }
            finally
            {

            }
        }

        protected override void Dispose(bool disposing)
        {
            season.Dispose();
            scorings.Dispose();
            scoringTables.Dispose();
            base.Dispose(disposing);
        }
    }
}
