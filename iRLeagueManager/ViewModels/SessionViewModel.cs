﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Win32;

using iRLeagueManager.Models.Sessions;
using iRLeagueManager.Models;
using iRLeagueManager.Interfaces;
using iRLeagueManager.Services;
using iRLeagueManager.Models.Results;
using iRLeagueManager.Enums;
using iRLeagueManager.Locations;
using iRLeagueManager.Timing;
using iRLeagueManager.Models.Members;
using System.Runtime.CompilerServices;

namespace iRLeagueManager.ViewModels
{
    public class SessionViewModel : LeagueContainerModel<SessionModel>
    {
        public SessionModel Model
        {
            get => Source;
            protected set
            {
                if (SetSource(value))
                {
                    OnPropertyChanged(null);
                }
            }
        }

        public ICommand UploadFileCmd { get; }

        private LocationCollection Locations { get; } = new LocationCollection();

        private ScheduleViewModel schedule;
        public ScheduleViewModel Schedule { get => schedule; set => SetValue(ref schedule, value); }

        public int? SessionId => Model.SessionId;

        public int? SessionNumber => Schedule?.Sessions.IndexOf(this) + 1;

        public SessionType SessionType { get => Model.SessionType; set => Model.SessionType = value; }
        public DateTime FullDate { get => Model.Date; set => Model.Date = value; }
        public DateTime Date { get => Model.Date.Date; set => Model.Date = value.Date.Add(Model.Date.TimeOfDay); }
        public TimeSpan SessionStart { get => Model.Date.TimeOfDay; set => Model.Date = Date.Date.Add(value); }
        public TimeSpan SessionEnd => SessionStart.Add(Duration);
        public TimeComponentVector TimeOfDayComponents { get; }

        public TimeSpan Duration { get => Model.Duration; set => Model.Duration = value; }
        public TimeComponentVector DurationComponents { get; }

        //public RaceTrack Track { get => Model.Location?.GetTrackInfo(); set => Model.Location = new Location(value.Configs.First()); }
        //public IEnumerable<TrackConfig> TrackConfigs { get => Track?.Configs; }
        //public TrackConfig Config { get => Model.Location?.GetConfigInfo(); set => Model.Location = new Location(value); }

        public int TrackId { get => Model.TrackId; set => Model.TrackId = value; }
        public int TrackIndex { get => TrackId - 1; set => TrackId = value + 1; }
        public int ConfigId { get => Model.ConfigId; set => Model.ConfigId = value; }
        public int ConfigIndex { get => ConfigId - 1; set => ConfigId = value + 1; }

        public RaceTrack Track => Locations.FirstOrDefault(x => x.GetTrackInfo().TrackId == TrackId)?.GetTrackInfo();
        public IEnumerable<TrackConfig> TrackConfigs => Track?.Configs;
        public TrackConfig Config => Track?.Configs.SingleOrDefault(x => x.ConfigId == ConfigId);
        public Location Location => Locations.FirstOrDefault(x => x.LocationId == Model.LocationId);

        public int Laps { get => ((Model as RaceSessionModel)?.Laps).GetValueOrDefault(); set { if (Model is RaceSessionModel race) { race.Laps = value; } } }
        public string IrResultLink { get => (Model as RaceSessionModel)?.IrResultLink; set { if (Model is RaceSessionModel race) { race.IrResultLink = value; } } }
        public string IrSessionId { get => (Model as RaceSessionModel)?.IrSessionId; set { if (Model is RaceSessionModel race) { race.IrSessionId = value; } } }
        public TimeSpan PracticeLength { get => ((Model as RaceSessionModel)?.PracticeLength).GetValueOrDefault(); set { if (Model is RaceSessionModel race) { race.PracticeLength = value; } } }
        public TimeComponentVector PracticeLenghtComponents { get; }
        public TimeSpan PracticeStart => SessionStart;
        public TimeSpan PracticeEnd => SessionStart.Add(PracticeLength);
        public TimeSpan QualyLength { get => ((Model as RaceSessionModel)?.QualyLength).GetValueOrDefault(); set { if (Model is RaceSessionModel race) { race.QualyLength = value; } } }
        public TimeComponentVector QualyLengthComponents { get; }
        public TimeSpan QualyStart => PracticeStart.Add(PracticeLength);
        public TimeSpan QualyEnd => QualyStart.Add(QualyLength);
        public TimeSpan RaceLength { get => ((Model as RaceSessionModel)?.RaceLength).GetValueOrDefault(); set { if (Model is RaceSessionModel race) { race.RaceLength = value; } } }
        public TimeComponentVector RaceLengthComponents { get; }
        public TimeSpan RaceStart => QualyStart.Add(QualyLength);
        public TimeSpan RaceEnd => RaceStart.Add(RaceLength);
        public bool QualyAttached { get => ((Model as RaceSessionModel)?.QualyAttached).GetValueOrDefault(); set { if (Model is RaceSessionModel race) { race.QualyAttached = value; } } }
        public bool PracticeAttached { get => ((Model as RaceSessionModel)?.PracticeAttached).GetValueOrDefault(); set { if (Model is RaceSessionModel race) { race.PracticeAttached = value; } } }

        public int? RaceId => (Model as RaceSessionModel)?.RaceId;

        public SessionViewModel() : base()
        {
            SetSource(RaceSessionModel.GetTemplate());
            TimeOfDayComponents = new TimeComponentVector(() => SessionStart, x => SessionStart = x);
            DurationComponents = new TimeComponentVector(() => Duration, x => Duration = x);
            PracticeLenghtComponents = new TimeComponentVector(() => PracticeLength, x => PracticeLength = x);
            QualyLengthComponents = new TimeComponentVector(() => QualyLength, x => QualyLength = x);
            RaceLengthComponents = new TimeComponentVector(() => RaceLength, x => RaceLength = x);
            UploadFileCmd = new RelayCommand(o => UploadFile(Model), o => true);
        }

        public async void UploadFile(SessionModel session)
        {
            OpenFileDialog openDialog = new OpenFileDialog
            {
                Filter = "CSV Dateien (*.csv)|*.csv",
                Multiselect = false
            };
            if (openDialog.ShowDialog() == false)
            {
                return;
            }

            var fileName = openDialog.FileName;

            Stream stream = null;
            ResultParserService parserService = new ResultParserService(GlobalSettings.LeagueContext);
            IEnumerable<Dictionary<string, string>> lines = null; 

            try
            {
                stream = File.Open(fileName, FileMode.Open, FileAccess.Read);
                lines = parserService.ParseCSV(new StreamReader(stream));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error parsing result File", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            finally
            {
                stream?.Dispose();
            }


            //Update LeagueMember database
            var memberList = (await LeagueContext.GetModelsAsync<LeagueMember>())?.ToList();
            parserService.MemberList = memberList;
            var newMembers = parserService.GetNewMemberList(lines).Where(x => !memberList.Exists(y => y.IRacingId == x.IRacingId));

            //foreach (var member in newMembers)
            //{
                await GlobalSettings.LeagueContext.UpdateModelsAsync(newMembers);
            //}
            //var sessionModel = season.GetSessions().SingleOrDefault(x => x.SessionId == session.SessionId);
            if (session == null)
                return;

            var resultRows = parserService.GetResultRows(lines);
            ResultModel result;
                if (session.SessionResult != null)
            {
                result = await LeagueContext.GetModelAsync<ResultModel>(session.SessionResult.ResultId.GetValueOrDefault());
                result.RawResults = new ObservableCollection<ResultRowModel>(resultRows);
                await GlobalSettings.LeagueContext.UpdateModelAsync(result);
            }
            else
            {
                //result = await GlobalSettings.LeagueContext.CreateResultAsync(sessionModel);
                result = new ResultModel(session);
                session.SessionResult = result;
                //result = await GlobalSettings.LeagueContext.UpdateModelAsync(result);
                //session = await GlobalSettings.LeagueContext.UpdateModelAsync(session);
                result.RawResults = new ObservableCollection<ResultRowModel>(resultRows);

                //await GlobalSettings.LeagueContext.UpdateModelAsync(result);
                await GlobalSettings.LeagueContext.UpdateModelAsync(session);
            }
            //CurrentResult = await LeagueContext.GetModelAsync<ResultModel>(season.Results.OrderBy(x => x.Session.Date).LastOrDefault().ResultId);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(Model.LocationId))
            {
                OnPropertyChanged(nameof(Track));
                OnPropertyChanged(nameof(TrackId));
                OnPropertyChanged(nameof(TrackIndex));
                OnPropertyChanged(nameof(TrackConfigs));
                OnPropertyChanged(nameof(Config));
                OnPropertyChanged(nameof(ConfigId));
                OnPropertyChanged(nameof(ConfigIndex));
                OnPropertyChanged(nameof(Location));
            }

            if (propertyName == nameof(Duration))
            {
                OnPropertyChanged(nameof(SessionEnd));
            }

            if (propertyName == nameof(Date))
            {
                OnPropertyChanged(null);
            }
            if (propertyName == nameof(Duration))
            {
                OnPropertyChanged(nameof(DurationComponents));
            }
            if (propertyName == nameof(PracticeLength))
            {
                OnPropertyChanged(nameof(PracticeLenghtComponents));
                OnPropertyChanged(nameof(PracticeEnd));
                OnPropertyChanged(nameof(QualyStart));
                OnPropertyChanged(nameof(QualyEnd));
                OnPropertyChanged(nameof(RaceStart));
                OnPropertyChanged(nameof(RaceEnd));
            }
            if (propertyName == nameof(QualyLength))
            {
                OnPropertyChanged(nameof(QualyLengthComponents));
                OnPropertyChanged(nameof(QualyEnd));
                OnPropertyChanged(nameof(RaceStart));
                OnPropertyChanged(nameof(RaceEnd));
            }
            if (propertyName == nameof(RaceLength))
            {
                OnPropertyChanged(nameof(RaceLengthComponents));
                OnPropertyChanged(nameof(RaceEnd));
            }
        }
    }
}