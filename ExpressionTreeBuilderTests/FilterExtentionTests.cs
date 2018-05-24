using ExpressionTreeBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionTreeBuilderTests
{
    [TestClass]
    public class FilterExtentionTests
    {
        List<ReportingTimeActiveModel> dataList;
        IQueryable<ReportingTimeActiveModel> queryable;
        FilterModel FilterModel;
        //FilterRepository baseRepository;
        FilterParamModel filterId;
        FilterParamModel filterName;
        FilterParamModel filterTime;
        FilterParamModel filterDate;
        FilterParamModel filterKlas;
        List<string> groupNames;
        public FilterExtentionTests()
        {
            //baseRepository = new FilterRepository();
            FilterModel = new FilterModel();
            groupNames = new List<string>()
            {
                "klas1",
                "klas2",
                "leerling"
            };
            dataList = new List<ReportingTimeActiveModel>()
            {
                new ReportingTimeActiveModel()
                {
                    Id = 1,
                    UserId =1,
                    Date = new DateTime(2018,04,24,9,0,0),
                    MinutesActive= 210,
                    UserProfile = new UserProfile(){
                        UserName = "Kasper",
                        GroupNames = new List<string>()
                        {
                            groupNames.ElementAt(0),//klas1
                            groupNames.ElementAt(2),//leerling
                        }
                    }
                },
                new ReportingTimeActiveModel()
                {
                    Id = 2,
                    UserId =1,
                    Date = new DateTime(2018,04,24,13,0,0),
                    MinutesActive= 300,
                    UserProfile = new UserProfile(){
                        UserName = "Kasper",
                        GroupNames = new List<string>()
                        {
                            groupNames.ElementAt(0),//klas1
                            groupNames.ElementAt(2),//leerling
                        }
                    }
                },
                new ReportingTimeActiveModel()
                {
                    Id = 3,
                    UserId =2,
                    Date = new DateTime(2018,04,24,9,22,0),
                    MinutesActive= 198,
                    UserProfile = new UserProfile(){
                        UserName = "Remo",
                        GroupNames = new List<string>()
                        {
                            groupNames.ElementAt(1),//klas2
                            groupNames.ElementAt(2),//leerling
                        }
                    }
                },
                new ReportingTimeActiveModel()
                {
                    Id = 3,
                    UserId =2,
                    Date = new DateTime(2018,04,24,13,0,0),
                    MinutesActive= 322,
                    UserProfile = new UserProfile(){
                        UserName = "Remo",
                        GroupNames = new List<string>()
                        {
                            groupNames.ElementAt(1),//klas2
                            groupNames.ElementAt(2),//leerling
                        }
                    }
                }
            };
        }

        [TestInitialize]
        public void InitTest()
        {
            queryable = dataList.AsQueryable();//reset the list each time
            filterId = new FilterParamModel()
            {
                CompareMethod = PropertyValueCompareEnum.Equal,
                Property = "Id",
                Value = 1
            };
            filterName = new FilterParamModel()
            {
                CompareMethod = PropertyValueCompareEnum.Equal,
                Property = "UserProfile.UserName",
                Value = "Kasper"
            };
            filterTime = new FilterParamModel()
            {
                CompareMethod = PropertyValueCompareEnum.GreaterThanOrEqual,
                Property = "MinutesActive",
                Value = 210
            };
            filterDate = new FilterParamModel()
            {
                CompareMethod = PropertyValueCompareEnum.GreaterThanOrEqual,
                Property = "Date",
                Value = new DateTime(2018, 04, 24, 9, 0, 0)
            };
            filterKlas = new FilterParamModel()
            {
                CompareMethod = PropertyValueCompareEnum.ListContains,
                Property = "UserProfile.GroupNames",
                Value = "klas2"
            };
        }
        [TestMethod]
        public void GetFilteredByUserName()
        {
            var filters = new FilterModel()
            {
                Filters = new List<FilterParamModel>()
            };
            filters.Filters.Add(new FilterParamModel()
            {
                CompareMethod = PropertyValueCompareEnum.Equal,
                Property = "UserProfile.UserName",
                Value = "Kasper"
            });
            IQueryable<ReportingTimeActiveModel> filtered = queryable.Filter(filters);
        }
        [TestMethod]
        public void GetFilteredByGroupName()
        {
            var filters = new FilterModel()
            {
                Filters = new List<FilterParamModel>()
            };
            filters.Filters.Add(new FilterParamModel()
            {
                CompareMethod = PropertyValueCompareEnum.ListContains,
                Property = "UserProfile.GroupNames",
                Value = "Groep1A"
            });
            IQueryable<ReportingTimeActiveModel> filtered = queryable.Filter(filters);
        }
        [TestMethod]
        public void FilterWithFilterModelNull()
        {
            IQueryable<ReportingTimeActiveModel> filtered = queryable.Filter(null).OrderBy("MinutesActive");
            Assert.AreEqual(filtered.Count(), 4);
        }
        [TestMethod]
        public void FilterWithParamsListNull()
        {
            IQueryable<ReportingTimeActiveModel> filtered = queryable.Filter(FilterModel).OrderBy("MinutesActive");
            Assert.AreEqual(filtered.Count(), 4);
        }
        [TestMethod]
        public void FilterWithNoParamsListCount0()
        {
            FilterModel.Filters = new List<FilterParamModel>();
            IQueryable<ReportingTimeActiveModel> filtered = queryable.Filter<ReportingTimeActiveModel>(FilterModel);
            Assert.AreEqual(filtered.Count(), 4);
        }
        [TestMethod]
        public void FilterOneParamEquals()
        {
            FilterModel.Filters = new List<FilterParamModel>()
            {
                filterName
            };
            //var res = queryable.Where("UserProfile.UserName == @0","Kasper");
            IQueryable<ReportingTimeActiveModel> filtered = queryable.Filter<ReportingTimeActiveModel>(FilterModel);
            Assert.AreEqual(filtered.Count(), 2);
        }
        [TestMethod]
        public void FilterOneParamOfNestedPropInListEquals()
        {
            FilterModel.Filters = new List<FilterParamModel>()
            {
                filterKlas
            };
            //var res = queryable.Where("UserProfile.UserName == @0","Kasper");
            IQueryable<ReportingTimeActiveModel> filtered = queryable.Filter<ReportingTimeActiveModel>(FilterModel);

            Assert.AreEqual(filtered.Count(), 2);
        }
        [TestMethod]
        public void FilterOneParamNotEqual()
        {
            filterName.CompareMethod = PropertyValueCompareEnum.NotEqual;
            FilterModel.Filters = new List<FilterParamModel>()
            {
                filterName
            };
            //{(x.UserName != "Kasper")}
            IQueryable<ReportingTimeActiveModel> filtered = queryable.Filter<ReportingTimeActiveModel>(FilterModel);
            Assert.AreEqual(filtered.Count(), 2);
        }
        [TestMethod]
        public void FilterOneParamGreaterThan()
        {
            filterId.CompareMethod = PropertyValueCompareEnum.GreaterThan;
            FilterModel.Filters = new List<FilterParamModel>()
            {
                filterId
            };
            //{(x.Id > 1)}
            IQueryable<ReportingTimeActiveModel> filtered = queryable.Filter(FilterModel);
            Assert.AreEqual(filtered.Count(), 3);
        }
        [TestMethod]
        public void FilterOneParamGreaterThanOrEqual()
        {
            filterId.CompareMethod = PropertyValueCompareEnum.GreaterThanOrEqual;
            FilterModel.Filters = new List<FilterParamModel>()
            {
                filterId
            };
            //{(x.Id >= 1)}
            IQueryable<ReportingTimeActiveModel> filtered = queryable.Filter(FilterModel);
            Assert.AreEqual(filtered.Count(), 4);
        }
        [TestMethod]
        public void FilterOneParamLessThan()
        {
            filterId.CompareMethod = PropertyValueCompareEnum.LessThan;
            filterId.Value = 2;
            FilterModel.Filters = new List<FilterParamModel>()
            {
                filterId
            };
            //{(x.Id < 2)}
            IQueryable<ReportingTimeActiveModel> filtered = queryable.Filter(FilterModel);
            Assert.AreEqual(filtered.Count(), 1);
        }
        [TestMethod]
        public void FilterOneParamLessThenOrEqual()
        {
            filterId.CompareMethod = PropertyValueCompareEnum.LessThanOrEqual;
            filterId.Value = 2;
            FilterModel.Filters = new List<FilterParamModel>()
            {
                filterId
            };
            //{(x.Id <= 2)}
            IQueryable<ReportingTimeActiveModel> filtered = queryable.Filter(FilterModel);
            Assert.AreEqual(filtered.Count(), 2);
        }
        [TestMethod]
        public void FilterTwoParamsCombinedWithAnd()
        {
            FilterModel.Filters = new List<FilterParamModel>()
            {
                filterId,
                filterName
            };
            FilterModel.CompareMethod = PropertiesCompareEnum.AND;
            //{((x.Id == 1) And (x.UserName == "Kasper"))}
            IQueryable<ReportingTimeActiveModel> filtered = queryable.Filter(FilterModel);
            Assert.AreEqual(filtered.Count(), 1);
        }

        [TestMethod]
        public void FilterTwoParamsCombinedWithOr()
        {
            filterName.Value = "Remo";
            FilterModel.Filters = new List<FilterParamModel>()
            {
                filterId,
                filterName
            };
            FilterModel.CompareMethod = PropertiesCompareEnum.OR;
            //{((x.Id == 1) Or (x.UserName == "Remo"))}
            IQueryable<ReportingTimeActiveModel> filtered = queryable.Filter<ReportingTimeActiveModel>(FilterModel);
            Assert.AreEqual(filtered.Count(), 3);
        }
        [TestMethod]
        public void FilterThreeParamsCombinedWithAnd()
        {
            filterName.Value = "Remo";
            FilterModel.Filters = new List<FilterParamModel>()
            {
                filterId,
                filterName,
                filterDate
            };
            FilterModel.CompareMethod = PropertiesCompareEnum.AND;
            //{(((x.Id == 1) And (x.UserName == "Remo")) And (x.Date >= 24/04/2018 09:00:00))}
            IQueryable<ReportingTimeActiveModel> filtered = queryable.Filter<ReportingTimeActiveModel>(FilterModel);
            Assert.AreEqual(filtered.Count(), 0);
        }
        [TestMethod]
        public void FilterThreeParamsCombinedWithOr()
        {
            filterName.Value = "Remo";
            FilterModel.Filters = new List<FilterParamModel>()
            {
                filterId,
                filterName,
                filterDate
            };
            FilterModel.CompareMethod = PropertiesCompareEnum.OR;
            //{(((x.Id == 1) Or (x.UserName == "Remo")) Or (x.Date >= 24/04/2018 09:00:00))}
            IQueryable<ReportingTimeActiveModel> filtered = queryable.Filter<ReportingTimeActiveModel>(FilterModel);
            Assert.AreEqual(filtered.Count(), 4);
        }
        [TestMethod]
        public void FilterTwoParamsCombinedWithAndWithAChildOr()
        {
            FilterModel.Filters = new List<FilterParamModel>()
            {
                filterId,
                filterName
            };
            FilterModel.CompareMethod = PropertiesCompareEnum.AND;
            filterDate.CompareMethod = PropertyValueCompareEnum.GreaterThan;
            FilterModel.ChildFilters = new FilterModel()
            {
                Filters = new List<FilterParamModel>()
                {
                    filterDate
                }
            };
            FilterModel.ChildCompareMethod = PropertiesCompareEnum.OR;
            //{(((x.Id == 1) And (x.UserName == "Kasper")) Or (x.Date > 24/04/2018 09:00:00))}
            IQueryable<ReportingTimeActiveModel> filtered = queryable.Filter<ReportingTimeActiveModel>(FilterModel);
            Assert.AreEqual(filtered.Count(), 4);
        }
        [TestMethod]
        public void FilterTwoParamsCombinedWithOrWithAChildAnd()
        {
            FilterModel.Filters = new List<FilterParamModel>()
            {
                filterId,
                filterName
            };
            FilterModel.CompareMethod = PropertiesCompareEnum.OR;
            filterDate.CompareMethod = PropertyValueCompareEnum.GreaterThan;
            FilterModel.ChildFilters = new FilterModel()
            {
                Filters = new List<FilterParamModel>()
                {
                    filterDate
                }
            };
            FilterModel.ChildCompareMethod = PropertiesCompareEnum.AND;
            //{(((x.Id == 1) Or (x.UserName == "Kasper")) And (x.Date > 24/04/2018 09:00:00))}
            IQueryable<ReportingTimeActiveModel> filtered = queryable.Filter<ReportingTimeActiveModel>(FilterModel);
            Assert.AreEqual(filtered.Count(), 1);
        }
        [TestMethod]
        public void FilterParamBetweenDates()
        {
            ReportingTimeActiveModel mId1 = new ReportingTimeActiveModel()
            {
                Date = DateTime.Now.AddHours(1),
                Id = 2,
                MinutesActive = 210,
                DateLogoff = DateTime.Now.AddHours(4),
                UserProfile = new UserProfile() { UserName = "Kasper" }
            };
            ReportingTimeActiveModel mId2 = new ReportingTimeActiveModel()
            {
                Date = DateTime.Now.AddHours(-4),
                Id = 2,
                MinutesActive = 210,
                DateLogoff = DateTime.Now,
                UserProfile = new UserProfile() { UserName = "Kasper" }
            };
            List<ReportingTimeActiveModel> list = new List<ReportingTimeActiveModel>()
            {
                mId1,
                mId2
            };

            DateTime logonTime = DateTime.Now.AddHours(-2);
            DateTime logoffTime = DateTime.Now.AddMinutes(30);
            FilterModel filterBetweenDates = new FilterModel()
            {
                Filters = new List<FilterParamModel>()
                {
                    new FilterParamModel()
                    {
                        CompareMethod = PropertyValueCompareEnum.GreaterThanOrEqual,
                        Property = "Date",
                        //PropertyType = typeof(DateTime),
                        Value = logonTime
                    },
                    new FilterParamModel()
                    {
                        CompareMethod = PropertyValueCompareEnum.LessThanOrEqual,
                        Property = "Date",
                        //PropertyType = typeof(DateTime),
                        Value = logoffTime
                    }
                },
                CompareMethod = PropertiesCompareEnum.AND,
                ChildFilters = new FilterModel()
                {
                    Filters = new List<FilterParamModel>()
                    {
                        new FilterParamModel()
                        {
                            CompareMethod = PropertyValueCompareEnum.GreaterThanOrEqual,
                            Property = "DateLogoff",
                            //PropertyType = typeof(DateTime),
                            Value = logonTime
                        },
                        new FilterParamModel()
                        {
                            CompareMethod = PropertyValueCompareEnum.LessThanOrEqual,
                            Property = "DateLogoff",
                            //PropertyType = typeof(DateTime),
                            Value = logoffTime
                        }
                    }
                },
                ChildCompareMethod = PropertiesCompareEnum.OR
            };
            IQueryable<ReportingTimeActiveModel> filtered = list.AsQueryable().Filter<ReportingTimeActiveModel>(filterBetweenDates);
            Assert.AreEqual(filtered.Count(), 1);
        }
    }
}
