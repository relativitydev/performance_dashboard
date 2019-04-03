using kCura.PDD.Web.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace kCura.PDD.Web.Test
{
	using kCura.PDB.Core.Models.HealthChecks;
	using NUnit.Framework;

    [TestFixture]
    public class PeakUserTests
    {
        [Test]
        public void RunApplicationHealthMapper_PeakUsersForDateRange()
        {
            Boolean pass_or_fail = true;
            IList<HealthBase> healthData = new List<HealthBase>();
            Dictionary<int, List<int>> keys = LoadDateTestRangeData(healthData);
            var applicationHealthDatas = ApplicationHealthMapper.ToApplicationHealth(healthData);

            foreach (var dta in applicationHealthDatas)
            {
                var healthItems = healthData.Where(x => ((ApplicationHealth)x).CaseArtifactId == dta.ArtifactID);
                var derivedAnswer = Math.Ceiling(healthItems.GroupBy(x => x.MeasureDate.Date).Average(x => x.Max(y => Math.Max(((ApplicationHealth)y).UserCount, 0))));
                if (dta.Users != derivedAnswer)
                {
                    pass_or_fail = false;
                }
            }

            Assert.IsTrue(pass_or_fail);
        }

        [Test]
        public void RunApplicationHealthMapper_PeakUsersForSingleDay()
        {
            Boolean pass_or_fail = true;
            IList<HealthBase> healthData = new List<HealthBase>();
            Dictionary<int, List<int>> keys = LoadDateTestRangeData(healthData, 1);
            var applicationHealthDatas = ApplicationHealthMapper.ToApplicationHealth(healthData);

            foreach (var dta in applicationHealthDatas)
            {

                List<int> usrCounts = keys[Convert.ToInt32(dta.ArtifactID)];
                double derivedAnswer = 0;
                if (usrCounts.Where(a => a > 0).Any())
                {
                    derivedAnswer = usrCounts.Max();
                }
                if (dta.Users != derivedAnswer)
                {
                    pass_or_fail = false;
                }
            }

            Assert.IsTrue(pass_or_fail);
        }

        [Test]
        public void RunApplicationHealthMapper_PeakUsersForNegativeOrZeroedOutUserCounts_ForSingleDay()
        {
            Boolean pass_or_fail = true;
            IList<HealthBase> healthData = new List<HealthBase>();
            Dictionary<int, List<int>> keys = LoadDateTestRangeData(healthData, 1, true);
            var applicationHealthDatas = ApplicationHealthMapper.ToApplicationHealth(healthData);

            foreach (var dta in applicationHealthDatas)
            {

                List<int> usrCounts = keys[Convert.ToInt32(dta.ArtifactID)];

                double derivedAnswer = 0;
                if (usrCounts.Where(a => a > 0).Any())
                {
                    derivedAnswer = usrCounts.Max();
                }
                if (dta.Users != derivedAnswer)
                {
                    pass_or_fail = false;
                }
            }

            Assert.IsTrue(pass_or_fail);
        }

        [Test]
        public void RunApplicationHealthMapper_PeakUsersForNegativeOrZeroedOutUserCounts_ForMultipleDays()
        {
            Boolean pass_or_fail = true;
            IList<HealthBase> healthData = new List<HealthBase>();
            Dictionary<int, List<int>> keys = LoadDateTestRangeData(healthData, 5, true);
            var applicationHealthDatas = ApplicationHealthMapper.ToApplicationHealth(healthData);

            foreach (var dta in applicationHealthDatas)
            {

                List<int> usrCounts = keys[Convert.ToInt32(dta.ArtifactID)];
                double derivedAnswer = 0;
                if (usrCounts.Where(a => a > 1).Any())
                {
                    derivedAnswer = Math.Ceiling(usrCounts.Where(a => a > 1).Average());
                }
                if (dta.Users != derivedAnswer)
                {
                    pass_or_fail = false;
                }
            }

            Assert.IsTrue(pass_or_fail);
        }


        [Test]
        public void RunApplicationHealthMapper_PeakUsersForNegativeOrZeroedOutUserCounts_For200Days()
        {
            Boolean pass_or_fail = true;
            IList<HealthBase> healthData = new List<HealthBase>();
            Dictionary<int, List<int>> keys = LoadDateTestRangeData(healthData, 200, true);
            var applicationHealthDatas = ApplicationHealthMapper.ToApplicationHealth(healthData);

            foreach (var dta in applicationHealthDatas)
            {

                List<int> usrCounts = keys[Convert.ToInt32(dta.ArtifactID)];
                double derivedAnswer = 0;
                if (usrCounts.Where(a => a > 1).Any())
                {
                    derivedAnswer = Math.Ceiling(usrCounts.Where(a => a > 1).Average());
                }
                if (dta.Users != derivedAnswer)
                {
                    pass_or_fail = false;
                }
            }

            Assert.IsTrue(pass_or_fail);
        }


        [Test]
        public void RunApplicationHealthMapper_StraightZeros_ForSingleDay()
        {
            Boolean pass_or_fail = true;
            IList<HealthBase> healthData = new List<HealthBase>();
            Dictionary<int, List<int>> keys = LoadDateTestRangeData(healthData, 1, true, true);
            var applicationHealthDatas = ApplicationHealthMapper.ToApplicationHealth(healthData);

            foreach (var dta in applicationHealthDatas)
            {

                List<int> usrCounts = keys[Convert.ToInt32(dta.ArtifactID)];
                double derivedAnswer = 0;
                if (usrCounts.Where(a => a > 0).Any())
                {
                    derivedAnswer = usrCounts.Max();
                }
                if (dta.Users != derivedAnswer)
                {
                    pass_or_fail = false;
                }
            }

            Assert.IsTrue(pass_or_fail);
        }

        [Test]
        public void RunApplicationHealthMapper_StraightZeros_ForMultipleDays()
        {
            Boolean pass_or_fail = true;
            IList<HealthBase> healthData = new List<HealthBase>();
            Dictionary<int, List<int>> keys = LoadDateTestRangeData(healthData, 5, true, true);
            var applicationHealthDatas = ApplicationHealthMapper.ToApplicationHealth(healthData);

            foreach (var dta in applicationHealthDatas)
            {

                List<int> usrCounts = keys[Convert.ToInt32(dta.ArtifactID)];
                double derivedAnswer = 0;
                if (usrCounts.Where(a => a > 1).Any())
                {
                    derivedAnswer = Math.Ceiling(usrCounts.Where(a => a > 1).Average());
                }
                if (dta.Users != derivedAnswer)
                {
                    pass_or_fail = false;
                }
            }

            Assert.IsTrue(pass_or_fail);
        }

        [Test]
        public void RunApplicationHealthMapper_StraightOnes_ForSingleDay()
        {
            Boolean pass_or_fail = true;
            IList<HealthBase> healthData = new List<HealthBase>();
            Dictionary<int, List<int>> keys = LoadDateTestRangeData(healthData, 1, true, false, true);
            var applicationHealthDatas = ApplicationHealthMapper.ToApplicationHealth(healthData);

            foreach (var dta in applicationHealthDatas)
            {

                List<int> usrCounts = keys[Convert.ToInt32(dta.ArtifactID)];
                double derivedAnswer = 0;
                if (usrCounts.Where(a => a > 0).Any())
                {
                    derivedAnswer = usrCounts.Max();
                }
                if (dta.Users != derivedAnswer)
                {
                    pass_or_fail = false;
                }
            }

            Assert.IsTrue(pass_or_fail);
        }

        [Test]
        public void RunApplicationHealthMapper_StraightOnes_ForMultipleDays()
        {
            Boolean pass_or_fail = true;
            IList<HealthBase> healthData = new List<HealthBase>();
            Dictionary<int, List<int>> keys = LoadDateTestRangeData(healthData, 5, true, false, true);
            var applicationHealthDatas = ApplicationHealthMapper.ToApplicationHealth(healthData);

            foreach (var dta in applicationHealthDatas)
            {
                if (dta.Users != 1)
                {
                    pass_or_fail = false;
                }
            }

            Assert.IsTrue(pass_or_fail);
        }


        private static Dictionary<int, List<int>> LoadDateTestRangeData(IList<HealthBase> healthData, int daysToRun = 5, Boolean zeroOutOrNegativeUserCounts = false, Boolean completeZeros = false, Boolean completeOnes = false)
        {
            //load up the artifact list
            List<int> artifactlst = new List<int>();
            artifactlst.Add(1014823);
            artifactlst.Add(1015024);
            artifactlst.Add(1015542);
            artifactlst.Add(1015555);
            artifactlst.Add(1015599);
            artifactlst.Add(1015603);
            artifactlst.Add(1015617);
            artifactlst.Add(1016503);
            artifactlst.Add(1016506);


            Random rnd = new Random();
            Dictionary<int, List<int>> _answerKey = new Dictionary<int, List<int>>();
            int incrIDS = 0;
            foreach (var art in artifactlst)
            {
                List<int> answers = new List<int>();

                for (int days = 0; days < daysToRun; days++)
                {

                    for (int occur = 0; occur < 15; occur++)
                    {
                        var sDate = DateTime.Now.AddDays(-(days));
                        var rndNmbr = rnd.Next(-500, (zeroOutOrNegativeUserCounts ? 1 : 500));
                        if (completeZeros) rndNmbr = 0;
                        if (completeOnes) rndNmbr = 1;

                        answers.Add(rndNmbr);
                        //generate
                        healthData.Add(new ApplicationHealth()
                        {
                            Id = incrIDS++,
                            CaseArtifactId = art,
                            ErrorCount = 0,
                            LRQCount = 0,
                            UserCount = rndNmbr,
                            WorkspaceName = String.Empty,
                            DatabaseLocation = String.Empty,
                            MeasureDate = sDate
                        });

                    }
                }

                _answerKey.Add(art, answers);
            }
            return _answerKey;
        }
    }
}