﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRLeagueManager.Data;
using iRLeagueManager.Models;
using iRLeagueManager.Models.Results;
using iRLeagueManager.LeagueDBServiceRef;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var LeagueContext = new LeagueContext();
            var dbClient = new DbLeagueServiceClient();

            //var season = LeagueContext.GetModelAsync<SeasonModel>(1).Result;
            //var scoring = new ScoringModel()
            //{
            //    Name = "TestScoring",
            //    AverageRaceNr = -1,
            //    BasePoints = new System.Collections.ObjectModel.ObservableCollection<KeyValuePair<int, int>>(
            //        (new Dictionary<int, int>() { { 1, 20 }, { 2, 19 }, { 3, 18 }, { 4, 17 }, { 5, 16 }, { 6, 15 }, { 7, 14 }, { 8, 13 }, { 9, 12 }, { 10, 11 },
            //            { 11, 10 }, { 12, 9 }, { 13, 8 }, { 14, 7 }, { 15, 6 }, { 16, 5 }, { 17, 4 }, { 18, 3 }, { 19, 2 }, { 20, 1 } }).ToArray()),
            //    BonusPoints = new System.Collections.ObjectModel.ObservableCollection<KeyValuePair<string, int>>(
            //        (new Dictionary<string, int>() { { "p1", 5 }, { "p2", 3 }, { "p3", 1 } }).ToArray()),
            //    DropWeeks = 3,
            //    Season = season
            //};
            //season.Scorings.Add(scoring);
            //season.MainScoring = scoring;
            //LeagueContext.UpdateModelAsync(season).Wait();

            var request = new GetItemsRequest()
            {
                userName = "Test",
                password = "123456",
                databaseName = "TestDatabase",
                requestResponse = true,
                requestItemType = typeof(SessionDataDTO).Name,
                requestItemIds = new long[][] { new long[] { 1 } }
            };
            var response = dbClient.GetFromDatabaseAsync(request).Result;

            Console.Read();
        }
    }
}
