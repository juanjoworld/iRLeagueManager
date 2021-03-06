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
using System.Data;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using iRLeagueManager.Interfaces;
using iRLeagueManager.Enums;
using iRLeagueManager.Models.Sessions;
using iRLeagueManager.Models.Results;
using iRLeagueManager.Models.Reviews;
using iRLeagueManager.Models.Statistics;

namespace iRLeagueManager.Models
{
    public class SeasonModel : SeasonInfo, IHierarchicalModel //, ISeason
    {        
        private ObservableCollection<ScheduleInfo> schedules;
        public ObservableCollection<ScheduleInfo> Schedules
        {
            get => schedules;
            internal set => SetNotifyCollection(ref schedules, value);
        }
        //ReadOnlyObservableCollection<IScheduleInfo> ISeason.Schedules => new ReadOnlyObservableCollection<IScheduleInfo>(Schedules);

        private ObservableCollection<ScoringModel> scorings;
        public ObservableCollection<ScoringModel> Scorings { get => scorings; internal set => SetNotifyCollection(ref scorings, value); }
        //ReadOnlyObservableCollection<IScoringInfo> ISeason.Scorings => new ReadOnlyObservableCollection<IScoringInfo>(Scorings);

        private ObservableCollection<ScoringTableModel> scoringTables;
        public ObservableCollection<ScoringTableModel> ScoringTables { get => scoringTables; internal set => SetNotifyCollection(ref scoringTables, value); }

        private ObservableCollection<StatisticSetInfo> seasonStatisticSets;
        public ObservableCollection<StatisticSetInfo> SeasonStatisticSets { get => seasonStatisticSets; set => SetNotifyCollection(ref seasonStatisticSets, value); }

        private ScoringModel mainScoring;
        public ScoringModel MainScoring
        {
            get => mainScoring;
            set => SetValue(ref mainScoring, value);
        }

        //private ObservableCollection<IncidentReviewInfo> reviews;
        //public ObservableCollection<IncidentReviewInfo> Reviews { get => reviews; internal set => SetNotifyCollection(ref reviews, value); }
        //ReadOnlyObservableCollection<IncidentReview> ISeason.Reviews => new ReadOnlyObservableCollection<IReviewInfo>(Reviews);

        //private ObservableCollection<ResultInfo> results;
        //public ObservableCollection<ResultInfo> Results { get => results; internal set => SetNotifyCollection(ref results, value); }

        string IHierarchicalModel.Description => SeasonName;

        private DateTime seasonStart;
        public DateTime SeasonStart { get => seasonStart; internal set => SetValue(ref seasonStart, value); }

        private DateTime seasonEnd;
        public DateTime SeasonEnd { get => seasonEnd; internal set => SetValue(ref seasonEnd, value); }
        
        private ObservableCollection<VoteCategoryModel> voteCategories;
        public ObservableCollection<VoteCategoryModel> VoteCategories { get => voteCategories; set => SetNotifyCollection(ref voteCategories, value); }

        private ObservableCollection<CustomIncidentModel> customIncidents;
        public ObservableCollection<CustomIncidentModel> CustomIncidents { get => customIncidents; set => SetNotifyCollection(ref customIncidents, value); }

        private bool hideCommentsBeforeVoted;
        public bool HideCommentsBeforeVoted { get => hideCommentsBeforeVoted; set => SetValue(ref hideCommentsBeforeVoted, value); }

        IEnumerable<object> IHierarchicalModel.Children => new List<IEnumerable<object>> { Schedules.Cast<object>() };

        public IEnumerable<ResultModel> GetResults()
        {
            return null;
        }
        // client.Results.Where(x => Sessions.Select(y => y.SessionId).Contains(x.SessionId));

        //public IEnumerable<ISession> Sessions => Schedules.Select(x => x.Sessions.AsEnumerable()).Aggregate((x, y) => x.Concat(y));

        //public int SessionCount => Sessions.Count();

        //public ISessionInfo LastSession => Sessions.OrderBy(x => x.Date).Last();

        //public IRaceInfo LastRace => Sessions.Where(x => x.SessionType == SessionType.Race).OrderBy(x => x.Date).Last() as IRaceInfo;

        public SeasonModel()
        {
            Schedules = new ObservableCollection<ScheduleInfo>();
            Scorings = new ObservableCollection<ScoringModel>();
            VoteCategories = new ObservableCollection<VoteCategoryModel>();
            CustomIncidents = new ObservableCollection<CustomIncidentModel>();
            ScoringTables = new ObservableCollection<ScoringTableModel>();
        }

        public SeasonModel(long? seasonId) : this()
        {
            SeasonId = seasonId;
        }

        //public IEnumerable<SessionInfo> GetSessions()
        //{
        //    return Schedules.Select(x => x.Sessions.AsEnumerable()).Aggregate((x, y) => x.Concat(y));
        //}

        //public int GetSessionCount()
        //{
        //    return GetSessions().Count();
        //}

        //public SessionInfo GetLastSession()
        //{
        //    return GetSessions().OrderBy(x => x.Date).Last();
        //}

        //internal override void InitReset()
        //{
        //    foreach (var schedule in Schedules)
        //    {
        //        schedule.Season = this;
        //        schedule.InitReset();
        //    }

        //    foreach (var result in Results)
        //    {
        //        result.InitReset();
        //    }

        //    foreach (var review in Reviews)
        //    {
        //        review.InitReset();
        //    }
        //    base.InitReset();
        //}

        internal override void InitializeModel()
        {
            if (!isInitialized)
            {
                foreach (var schedule in Schedules)
                {
                    //schedule.Season = this;
                    schedule.InitializeModel();
                }
                foreach (var scoring in Scorings)
                {
                    scoring.Season = this;
                    scoring.InitializeModel();
                }
                foreach (var scoringTable in ScoringTables)
                {
                    //for (int i = 0; i < scoringTable.Scorings.Count(); i++)
                    //{
                    //    var scoring = Scorings.SingleOrDefault(x => x.ScoringId == scoringTable.Scorings.ElementAt(i).Key.ScoringId);
                    //    if (scoring != null)
                    //    {
                    //        scoringTable.Scorings.ElementAt(i).Key = scoring;
                    //    }
                    //}
                    scoringTable.InitializeModel();
                }

                //foreach (var result in Results)
                //{
                //    result.InitializeModel();
                //}

                //foreach (var review in Reviews)
                //{
                //    review.InitializeModel();
                //}
            }
            base.InitializeModel();
        }

        public static SeasonModel GetTemplate()
        {
            return new SeasonModel();
        }

        public static SeasonModel GetTemplate(SeasonModel seasonInfo)
        {
            return new SeasonModel(seasonInfo.SeasonId)
            {
                SeasonName = seasonInfo.SeasonName
            };
        }

        //public IRaceSession GetLastRace()
        //{
        //    return GetSessions().Where(x => x.SessionType == SessionType.Race).OrderBy(x => x.Date).Last() as IRaceSession;
        //}

        //internal void SetLeagueClient(IRLeagueClient leagueClient)
        //{
        //    client = leagueClient;
        //    // Error handling für nicht vorhandene Id
        //    Schedules = Schedules.Select(x => client.Schedules.Single(y => y.ScheduleId == x.ScheduleId)).ToList();
        //    Scorings.ForEach(x => ((Scoring)x).SetLeagueClient(client));
        //}

        //public IResult AddNewResult()
        //{
        //    Result result = new Result(client.GetNewResultId());
        //    result.SetLeagueClient(client);
        //    //result.Session = Sessions.SingleOrDefault(x => x.SessionId == result.SessionId).GetSourceObject() as SessionBase;
        //    client.Results.Add(result);
        //    return result;
        //}

        //public ISchedule AddNewSchedule()
        //{
        //    return client.AddSchedule();
        //}

        //public IScoring AddNewScoring()
        //{
        //    throw new NotImplementedException();
        //}

        //public ISession AddNewSession()
        //{
        //    throw new NotImplementedException();
        //}

        //public void AddSession(ISession session)
        //{
        //    throw new NotImplementedException();
        //}

        //public void RemoveSession(ISession session)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
