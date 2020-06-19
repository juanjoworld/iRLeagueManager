﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Collection;
using AutoMapper.EquivalencyExpression;
using iRLeagueManager.Data;
using iRLeagueManager.LeagueDBServiceRef;
using iRLeagueManager.UserDBServiceRef;
using iRLeagueManager.Models;
using iRLeagueManager.Models.Sessions;
using iRLeagueManager.Models.Members;
using iRLeagueManager.Models.Results;
using iRLeagueManager.Models.Reviews;
using iRLeagueManager.Interfaces;
using iRLeagueManager.Enums;
using iRLeagueManager.Timing;
using iRLeagueManager.Converters;

namespace iRLeagueManager
{
    internal class ModelMapperProfile : Profile
    {
        public List<LeagueMember> MemberList => LeagueContext.MemberList.ToList();
        public LeagueContext LeagueContext { get; internal set; }
        public ModelManager ModelManager => LeagueContext.ModelManager;
        //private SeasonModel CurrentSeason { get; set; }
        private static IEnumerable<ScheduleModel> CurrentSchedules { get; set; }
        private static IEnumerable<SessionModel> CurrentSessions { get; set; }

        public ModelMapperProfile()
        {
            // Mapping Season data
            CreateMap<SeasonDataDTO, SeasonModel>()
                .EqualityComparison((src, dest) => src.SeasonId == dest.SeasonId)
                .ConstructUsing(source => ModelManager.PutOrGetModel(new SeasonModel(source.SeasonId)))
                .AfterMap((src, dest) =>
                {
                    CurrentSchedules = null;
                    dest.InitReset();
                })
                .ReverseMap();
            CreateMap<SeasonInfoDTO, SeasonModel>()
                .EqualityComparison((src, dest) => src.SeasonId == dest.SeasonId)
                .ConstructUsing(source => ModelManager.PutOrGetModel(new SeasonModel(source.SeasonId)))
                .AfterMap((src, dest) =>
                {
                    dest.InitReset();
                })
                .ReverseMap();

            // Mapping League member data
            CreateMap<LeagueMemberDataDTO, LeagueMember>()
                .ConstructUsing(source => (source != null) ? ModelManager.PutOrGetModel(new LeagueMember(source.MemberId.GetValueOrDefault())) : null)
                .ReverseMap();
            CreateMap<LeagueMemberInfoDTO, LeagueMember>()
                .ConvertUsing(source => (source != null) ? ModelManager.PutOrGetModel(new LeagueMember(source.MemberId.GetValueOrDefault())) : null);
            //.ConstructUsing(source => new LeagueMember(source.MemberId));
            CreateMap<LeagueMember, LeagueMemberInfoDTO>();

            // Mapping incident data
            CreateMap<IncidentReviewDataDTO, IncidentReviewModel>()
                .ConstructUsing(source => ModelManager.PutOrGetModel(new IncidentReviewModel(source.ReviewId)))
                .AfterMap((src, dest) =>
                {
                    dest.InitReset();
                })
                .ReverseMap();
            CreateMap<IncidentReviewInfoDTO, IncidentReviewModel>()
                .ConstructUsing(source => ModelManager.PutOrGetModel(new IncidentReviewModel(source.ReviewId)))
                .AfterMap((src, dest) =>
                {
                    dest.InitReset();
                })
                .ReverseMap();

            // Mapping comment data
            CreateMap<ReviewCommentDataDTO, ReviewCommentModel>()
                .ConstructUsing(source => ModelManager.PutOrGetModel(new ReviewCommentModel(source.CommentId.GetValueOrDefault())))
                .AfterMap((src, dest) =>
                {
                    dest.InitReset();
                })
                .ReverseMap();
            CreateMap<CommentDataDTO, CommentBase>()
                .ConstructUsing(source => ModelManager.PutOrGetModel(new CommentBase(source.CommentId.GetValueOrDefault())))
                .AfterMap((src, dest) =>
                {
                    dest.InitReset();
                })
                .ReverseMap();
            CreateMap<CommentInfoDTO, CommentBase>()
                .ConstructUsing(source => ModelManager.PutOrGetModel(new CommentBase(source.CommentId.GetValueOrDefault())))
                .AfterMap((src, dest) =>
                {
                    dest.InitReset();
                })
                .ReverseMap();

            // Mapping schedule data
            CreateMap<ScheduleDataDTO, ScheduleModel>()
                .BeforeMap((src, dest) =>
                {
                    if (CurrentSchedules == null)
                    {
                        CurrentSchedules = new ScheduleModel[0];
                    }
                    //CurrentSessions = dest?.Sessions;
                })
                //.ForMember(dest => dest.Season, opt => opt.Ignore())
                .EqualityComparison((src, dest) => src.ScheduleId == dest.ScheduleId)
                .ConstructUsing(source => ModelManager.PutOrGetModel(new ScheduleModel(source.ScheduleId)))
                .AfterMap((src, dest) =>
                {
                    dest.InitReset();
                    CurrentSessions = null;
                    SortObservableCollection(dest.Sessions, x => x.Date);
                    int i = 1;
                    foreach (var race in dest.Sessions.Where(x => x.SessionType == SessionType.Race).Cast<RaceSessionModel>())
                    {
                        race.RaceId = i;
                        i++;
                    }
                })
                .ReverseMap();
            //CreateMap<ScheduleInfoDTO, ScheduleModel>()
            //    .EqualityComparison((src, dest) => src.ScheduleId == dest.ScheduleId)
            //    .ConstructUsing(source => new ScheduleModel(source.ScheduleId))
            //    .AfterMap((src, dest) =>
            //    {
            //        dest.InitReset();
            //    })
            //    .ReverseMap()
            //    .As<ScheduleDataDTO>();
            CreateMap<ScheduleInfoDTO, ScheduleInfo>()
                .EqualityComparison((src, dest) => src.ScheduleId == dest.ScheduleId)
                .ReverseMap()
                .IncludeAllDerived();

            // Mapping session data
            CreateMap<SessionDataDTO, SessionModel>()
                .BeforeMap((src, dest) =>
                {
                    if (CurrentSessions == null)
                    {
                        CurrentSessions = new SessionModel[0];
                    }
                })
                //.ForMember(dest => dest.Season, opt => opt.Ignore())
                //.ForMember(dest => dest.Schedule, opt => opt.Ignore())
                //.ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.LocationId))
                .EqualityComparison((src, dest) => src.SessionId == dest.SessionId)
                //.ConstructUsing(source => (source.SessionType == SessionType.Race) ? new RaceSessionModel(source.SessionId) : new SessionModel(source.SessionId, source.SessionType))
                .ConstructUsing(source => ModelManager.PutOrGetModel(new SessionModel(source.SessionId, source.SessionType)))
                .AfterMap((src, dest) =>
                {
                    dest.InitReset();
                })
                .Include<RaceSessionDataDTO, RaceSessionModel>();
            CreateMap<RaceSessionDataDTO, RaceSessionModel>()
                .BeforeMap((src, dest) =>
                {
                    if (CurrentSessions == null)
                    {
                        CurrentSessions = new SessionModel[0];
                    }
                })
                //.ForMember(dest => dest.Season, opt => opt.Ignore())
                //.ForMember(dest => dest.Schedule, opt => opt.Ignore())
                //.ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.LocationId))
                .EqualityComparison((src, dest) => src.SessionId == dest.SessionId)
                .ConstructUsing(source => ModelManager.PutOrGetModel(new RaceSessionModel(source.SessionId, source.RaceId)))
                .AfterMap((src, dest) =>
                {
                    dest.InitReset();
                });
            CreateMap<SessionModel, SessionDataDTO>()
                //.ForMember(dest => dest.LocationId, opt => opt.MapFrom(source => source.Location))
                .Include<RaceSessionModel, RaceSessionDataDTO>();
            CreateMap<RaceSessionModel, RaceSessionDataDTO>();
                //.ForMember(dest => dest.LocationId, opt => opt.MapFrom(source => source.Location));
            CreateMap<SessionInfoDTO, SessionInfo>()
                //.BeforeMap((src, dest) =>
                //{
                //    if (CurrentSessions == null)
                //    {
                //        CurrentSessions = new SessionInfo[0];
                //    }
                //})
                .ConstructUsing(source => new SessionInfo(source.SessionId, source.SessionType))
                .ReverseMap();

            // Mapping result data
            CreateMap<ResultDataDTO, ResultModel>()
                .ConstructUsing(source => ModelManager.PutOrGetModel(new ResultModel(source.ResultId.GetValueOrDefault())))
                //.ForMember(dest => dest.Season, opt => opt.Ignore())
                .EqualityComparison((src, dest) => src.ResultId == dest.ResultId)
                .ReverseMap();
            //.ForMember(dest => dest.RawResults, opt => opt.Ignore())
            //.ForMember(dest => dest.FinalResults, opt => opt.Ignore())
            //.ForMember(dest => dest.Reviews, opt => opt.Ignore());
            CreateMap<ResultInfoDTO, ResultInfo>()
                .ConstructUsing(source => new ResultInfo(source.ResultId.GetValueOrDefault()))
                .ReverseMap()
                .Include<ResultModel, ResultDataDTO>();

            CreateMap<ResultRowDataDTO, ResultRowModel>()
                .ConstructUsing(source => ModelManager.PutOrGetModel(new ResultRowModel(source.ResultRowId)))
                .EqualityComparison((src, dest) => src.ResultRowId == dest.ResultRowId)
                .ReverseMap();

            CreateMap<ScoringDataDTO, ScoringModel>()
                .ConstructUsing(source => ModelManager.PutOrGetModel(new ScoringModel(source.ScoringId)))
                .EqualityComparison((src, dest) => src.ScoringId == dest.ScoringId)
                .ForMember(dest => dest.BasePoints, opt => opt.MapFrom((src, dest, result) =>
                {
                    if (src.BasePoints == null || src.BasePoints == "" || src.BasePoints == " ")
                        return new ObservableCollection<ScoringModel.BasePointsValue>();
                    string[] pointString = src.BasePoints.Split(' ');
                    ObservableCollection<ScoringModel.BasePointsValue> pairs = new ObservableCollection<ScoringModel.BasePointsValue>();
                    for (int i = 0; i < pointString.Count(); i++)
                    {
                        pairs.Add(new ScoringModel.BasePointsValue(i + 1, int.Parse(pointString[i])));
                    }
                    return pairs;
                }))
                .ForMember(dest => dest.BonusPoints, opt => opt.MapFrom((src, dest, result) =>
                {
                    if (src.BonusPoints == null || src.BonusPoints == "" || src.BonusPoints == " ")
                        return new ObservableCollection<ScoringModel.BonusPointsValue>();
                    string[] pointString = src.BonusPoints.Split(' ');
                    ObservableCollection<ScoringModel.BonusPointsValue> pairs = new ObservableCollection<ScoringModel.BonusPointsValue>();
                    for (int i = 0; i < pointString.Count(); i++)
                    {
                        var stringPair = pointString[i].Split(':');
                        pairs.Add(new ScoringModel.BonusPointsValue(stringPair.First(), int.Parse(stringPair.Last())));
                    }
                    return pairs;
                }))
                //.ForMember(dest => dest.BonusPoints, opt => opt.ConvertUsing<BonusPointsConverter, string>())
                .ForMember(dest => dest.IncPenaltyPoints, opt => opt.Ignore())
                .ForMember(dest => dest.MultiScoringResults, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.BasePoints, opt => opt.MapFrom(src => (src.BasePoints.Count > 0) ? src.BasePoints.Select(x => x.Value.ToString()).Aggregate((x, y) => x + " " + y) : ""))
                .ForMember(dest => dest.BonusPoints, opt => opt.MapFrom(src => (src.BonusPoints.Count > 0) ? src.BonusPoints.Select(x => x.Key + ":" + x.Value.ToString()).Aggregate((x, y) => x + " " + y) : ""))
                .ForMember(dest => dest.IncPenaltyPoints, opt => opt.Ignore());
            CreateMap<ScoringInfoDTO, ScoringModel>()
                .ConstructUsing(source => ModelManager.PutOrGetModel(new ScoringModel(source.ScoringId)))
                .EqualityComparison((src, dest) => src.ScoringId == dest.ScoringId)
                .ForAllMembers(opt => opt.Ignore());
            CreateMap<ScoringModel, ScoringInfoDTO>();
            CreateMap<ScoringInfoDTO, ScoringInfo>()
                .ConstructUsing(source => new ScoringInfo(source.ScoringId))
                .EqualityComparison((src, dest) => src.ScoringId == dest.ScoringId);
            CreateMap<ScoredResultRowDataDTO, ScoredResultRowModel>()
                .ConstructUsing(source => ModelManager.PutOrGetModel(new ScoredResultRowModel() { ScoredResultRowId = source.ScoredResultRowId}))
                .EqualityComparison((src, dest) => src.ScoredResultRowId == dest.ScoredResultRowId);

            CreateMap<ScoredResultDataDTO, ScoredResultModel>()
                .ConstructUsing(source => ModelManager.PutOrGetModel(new ScoredResultModel() { Scoring = new ScoringInfo(source.Scoring.ScoringId), ResultId = source.ResultId}))
                .EqualityComparison((src, dest) => src.Session.SessionId == dest.Session.SessionId && src.Scoring.ScoringId == dest.Scoring.ScoringId)
                //.ForMember(dest => dest.FinalResults, opt => opt.MapFrom(src => src.ScoredResults))
                ;
            CreateMap<StandingsDataDTO, StandingsModel>()
                .ConstructUsing(source => ModelManager.PutOrGetModel(new StandingsModel() { Scoring = new ScoringInfo(source.Scoring.ScoringId) }))
                .EqualityComparison((src, dest) => src.Scoring.ScoringId == dest.Scoring.ScoringId);
            CreateMap<StandingsRowDataDTO, StandingsRowModel>()
                .ConstructUsing(source => ModelManager.PutOrGetModel(new StandingsRowModel() { Scoring = new ScoringInfo(source.Scoring.ScoringId), Member = new LeagueMember(source.Member.MemberId) }))
                .EqualityComparison((src, dest) => src.Scoring.ScoringId == dest.Scoring.ScoringId && src.Member.MemberId == dest.Member.MemberId);

            CreateMap<AddPenaltyDTO, AddPenaltyModel>()
                .ConstructUsing(source => ModelManager.PutOrGetModel(new AddPenaltyModel(source.ScoredResultRowId)))
                .EqualityComparison((src, dest) => src.ScoredResultRowId == dest.ScoredResultRowId)
                .ReverseMap();

            CreateMap<UserDTO, UserModel>()
                .ConstructUsing(src => new UserModel(src.UserId));
                //.ForMember(dest => dest.Admin, opt => opt.MapFrom(src => src));
            CreateMap<UserModel, UserDTO>();
            //.ForMember(dest => dest.AdminId, opt => opt.MapFrom(src => (src.Admin != null) ? (int?)src.Admin.AdminId : null))
            //.ForMember(dest => dest.IsOwner, opt => opt.MapFrom(src => (src.Admin != null) ? (bool?)src.Admin.IsOwner : null))
            //.ForMember(dest => dest.LeagueName, opt => opt.MapFrom(src => (src.Admin != null) ? src.Admin.LeagueName : null))
            //.ForMember(dest => dest.Rights, opt => opt.MapFrom(src => (src.Admin != null) ? src.Admin.Rights : 0));

            //CreateMap<UserDTO, AdminModel>()
            //    .ConstructUsing(src => (LeagueContext.CurrentUser.Admin.AdminId == src.AdminId) ? LeagueContext.CurrentUser.Admin : new AdminModel((src.AdminId == null) ? 0 : (int)src.AdminId));
            CreateMap<TimeSpan, LapTime>()
                .ConvertUsing<LapTimeConverter>();
            CreateMap<TimeSpan, LapInterval>()
                .ConvertUsing<LapIntervalConverter>();

            CreateMap<LapTime, TimeSpan>()
                .ConvertUsing<LapTimeConverter>();
            CreateMap<LapInterval, TimeSpan>()
                .ConvertUsing<LapIntervalConverter>();
        }

        private static void SetCurrentSchedules (IEnumerable<ScheduleModel> schedules)
        {
            CurrentSchedules = schedules;
        }

        private static TDest ConditionalConstructor<TSource, TDest>(TSource source, IEnumerable<TDest> sourceCollection, Func<TDest, bool> comparer, Func<TDest> constructor)
        {
            TDest dest = sourceCollection.SingleOrDefault(x => comparer.Invoke(x));
            if (dest != null)
                return dest;
            return constructor.Invoke();
        }

        private void SortObservableCollection<T, TKey>(ObservableCollection<T> collection, Func<T, TKey> key)
        {
            if (collection == null)
                return;

            var sortedList = collection.OrderBy(key);

            for (int i = 0; i < sortedList.Count(); i++)
            {
                var item = sortedList.ElementAt(i);
                if (!collection.ElementAt(i).Equals(item))
                {
                    var index = collection.IndexOf(item);
                    collection.Move(index, i);
                }
            }
        }
    }

    public static class MappingExpressions
    {
        public static IMappingExpression<TSource, TDestination> MapOnlyIfChanged<TSource, TDestination>(this IMappingExpression<TSource, TDestination> map)
        {
            map.ForAllMembers(source =>
            {
                source.Condition((sourceObject, destObject, sourceProperty, destProperty) =>
                {
                    if (sourceProperty == null)
                        return !(destProperty == null);
                    return !sourceProperty.Equals(destProperty);
                });
            });
            return map;
        }

        //public static IMappingExpression<TSource, TDest> GetModelsFromCollection<TSource, TDest>(this IMappingExpression<TSource, TDest> map, IEnumerable<TDest> sourceCollection, System.Linq.Expressions.Expression<Func<TSource, TDest, bool>> comparer, System.Linq.Expressions.Expression<Func<TDest>> constructor)
        //{
        //    map.ConstructUsing((source) =>
        //    {
        //        var dest = sourceCollection.SingleOrDefault(x => comparer.Com)
        //        return dest;
        //    });
        //    return map;
        //}

        private static TDest ConditionalConstructor<TSource, TDest>(TSource source, IEnumerable<TDest> sourceCollection, Func<TSource, TDest, bool> comparer, Func<TDest> constructor)
        {
            TDest dest = sourceCollection.SingleOrDefault(x => comparer.Invoke(source, x));
            if (dest != null)
                return dest;
            return constructor.Invoke();
        }

        //public static IMappingExpression<TSource, TDest> UpdateModelValues<TSource, TDest>(this IMappingExpression<TSource, TDest> map)
        //{
        //    map.ForAllMembers(cfg =>
        //    {
        //        cfg.MapFrom((src, dest, srcMember, context) => 
        //        {
        //            var mapper = context.Mapper;

        //            if (dest is SeasonModel season && src is SeasonInfoDTO seasonData)
        //            {
        //                if (seasonData.SeasonId == season.SeasonId)
        //                {
        //                    mapper.Map(src, dest);
        //                    return dest;
        //                }
        //                else
        //                    return mapper.Map<TDest>(src);
        //            }
        //            else if (dest is ScheduleModel schedule && src is ScheduleInfoDTO scheduleData)
        //            {
        //                if (scheduleData.ScheduleId == schedule.ScheduleId)
        //                {
        //                    mapper.Map(src, dest);
        //                    return dest;
        //                }
        //                else
        //                    return mapper.Map<TDest>(src);
        //            }
        //            else if (dest is SessionModel session && src is SessionInfoDTO sessionData)
        //            {
        //                if (sessionData.SessionId == session.SessionId)
        //                {
        //                    mapper.Map(src, dest);
        //                    return dest;
        //                }
        //                else
        //                    return mapper.Map<TDest>(src);
        //            }
        //            else
        //            {
        //                return mapper.Map<TDest>(src);
        //            }
        //        });
        //    });
        //    return map;
        //}
    }

    //public class SessionTypeConverter : IValueConverter<LeagueDBServiceRef.SessionType, Enums.SessionType>, IValueConverter<Enums.SessionType, LeagueDBServiceRef.SessionType>
    //{
    //    Enums.SessionType IValueConverter<LeagueDBServiceRef.SessionType, Enums.SessionType>.Convert(LeagueDBServiceRef.SessionType sourceMember, ResolutionContext context)
    //    {
    //        return (Enums.SessionType)sourceMember;
    //    }

    //    LeagueDBServiceRef.SessionType IValueConverter<Enums.SessionType, LeagueDBServiceRef.SessionType>.Convert(Enums.SessionType sourceMember, ResolutionContext context)
    //    {
    //        return (LeagueDBServiceRef.SessionType)sourceMember;
    //    }
    //}

    public class LapTimeConverter : ITypeConverter<TimeSpan, LapTime>, ITypeConverter<LapTime, TimeSpan>
    {
        LapTime ITypeConverter<TimeSpan, LapTime>.Convert(TimeSpan source, LapTime destination, ResolutionContext context)
        {
            return new LapTime(source);
        }

        TimeSpan ITypeConverter<LapTime, TimeSpan>.Convert(LapTime source, TimeSpan destination, ResolutionContext context)
        {
            return source.Time;
        }
    }

    public class LapIntervalConverter : ITypeConverter<TimeSpan, LapInterval>, ITypeConverter<LapInterval, TimeSpan>
    {
        LapInterval ITypeConverter<TimeSpan, LapInterval>.Convert(TimeSpan source, LapInterval destination, ResolutionContext context)
        {
            return new LapInterval(source);
        }

        TimeSpan ITypeConverter<LapInterval, TimeSpan>.Convert(LapInterval source, TimeSpan destination, ResolutionContext context)
        {
            return source.Time;
        }
    }
}
