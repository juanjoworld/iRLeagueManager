﻿// MIT License

// Copyright (c) 2020 Simon Schulze

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
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
using iRLeagueManager.Models.Reviews;
using System.Collections.ObjectModel;
using iRLeagueManager.Models.Statistics;

namespace iRLeagueManager.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase, ISeasonPageViewModel
    {
        private SeasonViewModel season;
        public SeasonViewModel Season { get => season; set => SetValue(ref season, value); }

        private readonly ObservableViewModelCollection<ScoringViewModel,ScoringModel> scorings;
        //public ObservableModelCollection<ScoringViewModel, ScoringModel> Scorings
        //{
        //    get
        //    {
        //        if (scorings.GetSource() != Season?.Scorings)
        //            scorings.UpdateSource(Season?.Scorings);
        //        return scorings;
        //    }
        //}
        public ICollectionView Scorings
        {
            get
            {
                if (scorings.GetSource() != Season?.Scorings)
                    scorings.UpdateSource(Season?.Scorings);
                return scorings.CollectionView;
            }
        }

        private readonly ObservableViewModelCollection<ScoringTableViewModel, ScoringTableModel> scoringTables;
        //public ObservableModelCollection<ScoringTableViewModel, ScoringTableModel> ScoringTables
        //{
        //    get
        //    {
        //        if (scoringTables.GetSource() != Season?.ScoringTables)
        //            scoringTables.UpdateSource(Season?.ScoringTables);
        //        return scoringTables;
        //    }
        //}
        public ICollectionView ScoringTables
        {
            get
            {
                if (scoringTables.GetSource() != Season?.ScoringTables)
                    scoringTables.UpdateSource(Season?.ScoringTables);
                return scoringTables.CollectionView;
            }
        }

        private ObservableCollection<VoteCategoryModel> voteCategoriesCollection;
        private ICollectionView voteCategories;
        public ICollectionView VoteCategories { get => voteCategories; set => SetValue(ref voteCategories, value); }

        private ObservableCollection<CustomIncidentModel> incidentKindsCollection;
        private ICollectionView incidentKinds;
        public ICollectionView IncidentKinds { get => incidentKinds; set => SetValue(ref incidentKinds, value); }

        public ICommand AddScoringCmd { get; }
        public ICommand DeleteScoringCmd { get; }
        public ICommand AddScoringTableCmd { get; }
        public ICommand DeleteScoringTableCmd { get; }

        public ICommand AddIncidentKindCmd { get; }
        public ICommand RemoveIncidentKindCmd { get; }
        public ICommand AddVoteCategoryCmd { get; }
        public ICommand RemoveVoteCategoryCmd { get; }

        public ICommand SaveChangesCmd { get; }

        public SettingsPageViewModel() : base()
        {
            scorings = new ObservableViewModelCollection<ScoringViewModel, ScoringModel>(constructorAction: x => x.SetScoringsList(scorings.GetSource()));
            scoringTables = new ObservableViewModelCollection<ScoringTableViewModel, ScoringTableModel>(x => x.SetScoringsList(scorings));
            Season = new SeasonViewModel();
            AddScoringCmd = new RelayCommand(o => AddScoring(), o => Season != null);
            AddScoringTableCmd = new RelayCommand(o => AddScoringTable(), o => Season != null);
            DeleteScoringCmd = new RelayCommand(o => DeleteScoring((o as ScoringViewModel).Model), o => o != null);
            DeleteScoringTableCmd = new RelayCommand(o => DeleteScoringTable((o as ScoringTableViewModel).Model), o => o != null);
            SaveChangesCmd = new RelayCommand(async o => await SaveChanges(), o => CanSaveChanges());
            AddIncidentKindCmd = new RelayCommand(async o => await AddIncidentKind());
            RemoveIncidentKindCmd = new RelayCommand(async o => await RemoveIncidentKind(o as CustomIncidentModel), o => o != null && o is CustomIncidentModel);
            AddVoteCategoryCmd = new RelayCommand(async o => await AddVoteCategory());
            RemoveVoteCategoryCmd = new RelayCommand(async o => await RemoveVoteCategory(o as VoteCategoryModel), o => o != null && o is VoteCategoryModel);
        }

        public async Task Load(SeasonModel season)
        {
            try
            {
                IsLoading = true;
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
                var incidentKinds = await LeagueContext.GetModelsAsync<CustomIncidentModel>();
                incidentKindsCollection = new ObservableCollection<CustomIncidentModel>(incidentKinds);
                SetIncidentKindsView(incidentKindsCollection);
                var voteCategories = await LeagueContext.GetModelsAsync<VoteCategoryModel>();
                voteCategoriesCollection = new ObservableCollection<VoteCategoryModel>(voteCategories);
                SetVoteCategoriesView(voteCategoriesCollection);
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

        private void SetVoteCategoriesView(object source)
        {
            VoteCategories = CollectionViewSource.GetDefaultView(source);
            VoteCategories.SortDescriptions.Add(new SortDescription(nameof(VoteCategoryModel.Index), ListSortDirection.Ascending));
        }

        private void SetIncidentKindsView(object source)
        {
            IncidentKinds = CollectionViewSource.GetDefaultView(source);
            IncidentKinds.SortDescriptions.Add(new SortDescription(nameof(CustomIncidentModel.Index), ListSortDirection.Ascending));
        }

        private async Task AddIncidentKind()
        {
            try
            {
                IsLoading = true;
                var newIncident = await LeagueContext.AddModelAsync(new CustomIncidentModel() { Index = incidentKindsCollection.Count > 0 ? incidentKindsCollection.Max(x => x.Index) + 1 : 0 });
                incidentKindsCollection.Add(newIncident);
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

        private async Task RemoveIncidentKind(CustomIncidentModel incidentKind)
        {
            if (incidentKind == null)
            {
                return;
            }

            try
            {
                IsLoading = true;
                await LeagueContext.DeleteModelAsync<CustomIncidentModel>(incidentKind.ModelId);
                incidentKindsCollection.Remove(incidentKind);
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

        private async Task AddVoteCategory()
        {
            try
            {
                IsLoading = true;
                var newCategory = await LeagueContext.AddModelAsync(new VoteCategoryModel() { Index = voteCategoriesCollection.Count > 0 ? voteCategoriesCollection.Max(x => x.Index) + 1 : 0 });
                voteCategoriesCollection.Add(newCategory);
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

        private async Task RemoveVoteCategory(VoteCategoryModel voteCategory)
        {
            if (voteCategory == null)
            {
                return;
            }

            try
            {
                IsLoading = true;
                await LeagueContext.DeleteModelAsync<VoteCategoryModel>(voteCategory.ModelId);
                voteCategoriesCollection.Remove(voteCategory);
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

        public override async Task Refresh()
        {
            try
            {
                IsLoading = true;
                LeagueContext.ModelManager.ForceExpireModels<SeasonModel>();
                LeagueContext.ModelManager.ForceExpireModels<ScoringModel>();
                LeagueContext.ModelManager.ForceExpireModels<ScoringTableModel>();
                LeagueContext.ModelManager.ForceExpireModels<CustomIncidentModel>();
                LeagueContext.ModelManager.ForceExpireModels<VoteCategoryModel>();
                await Load(Season.Model);
                await base.Refresh();
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

        public async void AddScoring()
        {
            try
            {
                IsLoading = true;
                var scoring = new ScoringModel() { Season = this.Season.Model, Name = "New Scoring" };
                scoring = await LeagueContext.AddModelAsync(scoring);
                Season.Scorings.Add(scoring);
                await LeagueContext.UpdateModelAsync(Season.Model);
                scorings.UpdateCollection();
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

        public async void DeleteScoring(ScoringModel scoring)
        {
            if (scoring == null)
                return;

            try
            {
                IsLoading = true;
                await LeagueContext.DeleteModelsAsync(scoring);
                Season.Scorings.Remove(scoring);
                await LeagueContext.UpdateModelAsync(Season.Model);
                scorings.UpdateCollection();
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

        public async void AddScoringTable()
        {
            try
            {
                IsLoading = true;
                var scoringTable = new ScoringTableModel() { Name = "New Scoring Table" };
                //scoringTable = await LeagueContext.AddModelAsync(scoringTable);
                Season.ScoringTables.Add(scoringTable);
                await LeagueContext.UpdateModelAsync(Season.Model);
                scoringTables.UpdateCollection();
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

        public async void DeleteScoringTable(ScoringTableModel scoringTable)
        {
            if (scoringTable == null)
                return;

            try
            {
                IsLoading = true;
                await LeagueContext.DeleteModelsAsync(scoringTable);
                Season.ScoringTables.Remove(scoringTable);
                await LeagueContext.UpdateModelAsync(Season.Model);
                scoringTables.UpdateCollection();
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

        public async Task SaveChanges()
        {
            try
            {
                IsLoading = true;
                if (Season.Model.ContainsChanges)
                {
                    await Season.SaveChanges();
                }

                foreach (var scoring in scorings)
                {
                    await scoring.SaveChanges();
                }
                foreach (var scoringTable in scoringTables)
                {
                    await scoringTable.SaveChanges();
                }

                var updateIncidenKinds = incidentKindsCollection.Where(x => x.ContainsChanges);
                if (updateIncidenKinds.Count() > 0)
                {
                    await LeagueContext.UpdateModelsAsync(updateIncidenKinds);
                }
                var updateVoteCategories = voteCategoriesCollection.Where(x => x.ContainsChanges);
                if (updateVoteCategories.Count() > 0)
                {
                    await LeagueContext.UpdateModelsAsync(updateVoteCategories);
                }
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

        public bool CanSaveChanges()
        {
            if (Season == null)
            {
                return false;
            }

            var hasChanges = Season.Model.ContainsChanges;
            foreach (var scoring in scorings)
            {
                hasChanges |= scoring.Model.ContainsChanges;
            }
            foreach (var scoringTable in scoringTables)
            {
                hasChanges |= scoringTable.Model.ContainsChanges;
            }
            if (incidentKindsCollection != null)
            {
                hasChanges |= incidentKindsCollection.Any(x => x.ContainsChanges);
            }
            if (voteCategoriesCollection != null)
            {
                hasChanges |= voteCategoriesCollection.Any(x => x.ContainsChanges);
            }

            return hasChanges;
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
