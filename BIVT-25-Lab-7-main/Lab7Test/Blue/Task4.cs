using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Lab7Test.Blue
{
    [TestClass]
    public sealed class Task4
    {
        record InputTeam(string Name, int[] Scores);
        record InputGroup(string Name, InputTeam[] Teams);
        record OutputGroup(string Name, int TotalScore);

        private InputGroup[] _inputGroups;
        private OutputGroup[] _output;

        private Lab7.Blue.Task4.Team[] _teams;
        private Lab7.Blue.Task4.Group[] _groups;

        [TestInitialize]
        public void LoadData()
        {
            var folder = Directory.GetParent(Directory.GetCurrentDirectory())
                                  .Parent.Parent.Parent.FullName;
            folder = Path.Combine(folder, "Lab7Test", "Blue");

            var input = JsonSerializer.Deserialize<JsonElement>(
                File.ReadAllText(Path.Combine(folder, "input.json")))!;
            var output = JsonSerializer.Deserialize<JsonElement>(
                File.ReadAllText(Path.Combine(folder, "output.json")))!;

            _inputGroups = input.GetProperty("Task4").Deserialize<InputGroup[]>()!;
            _output = output.GetProperty("Task4").Deserialize<OutputGroup[]>()!;

            _teams = _inputGroups.SelectMany(g => g.Teams)
                                 .Select(t => new Lab7.Blue.Task4.Team(t.Name))
                                 .ToArray();

            _groups = _inputGroups.Select(g => new Lab7.Blue.Task4.Group(g.Name))
                                  .ToArray();
        }

        [TestMethod]
        public void Test_00_OOP()
        {
            var type = typeof(Lab7.Blue.Task4.Team);
            Assert.IsTrue(type.IsValueType, "Team должен быть структурой");
			Assert.AreEqual(type.GetFields().Count(f => f.IsPublic), 0);
			Assert.IsTrue(type.GetProperty("Name")?.CanRead ?? false, "Нет свойства Name");
            Assert.IsTrue(type.GetProperty("Scores")?.CanRead ?? false, "Нет свойства Scores");
            Assert.IsTrue(type.GetProperty("TotalScore")?.CanRead ?? false, "Нет свойства TotalScore");
            Assert.IsFalse(type.GetProperty("Name")?.CanWrite ?? false, "Name должно быть только для чтения");
            Assert.IsFalse(type.GetProperty("Scores")?.CanWrite ?? false, "Scores должно быть только для чтения");
            Assert.IsFalse(type.GetProperty("TotalScore")?.CanWrite ?? false, "TotalScore должно быть только для чтения");
			Assert.IsNotNull(type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(string) }, null), "Нет публичного конструктора Team(string name)");
			Assert.IsNotNull(type.GetMethod("PlayMatch", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(int) }, null), "Нет публичного метода PlayMatch(int time)");
			Assert.IsNotNull(type.GetMethod("Print", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null), "Нет публичного метода Print()");
			Assert.AreEqual(type.GetProperties().Count(f => f.PropertyType.IsPublic), 3);
			Assert.AreEqual(type.GetConstructors().Count(f => f.IsPublic), 1);
			Assert.AreEqual(type.GetMethods().Count(f => f.IsPublic), 9);

			type = typeof(Lab7.Blue.Task4.Group);
            Assert.IsTrue(type.IsValueType, "Group должен быть структурой");
			Assert.AreEqual(type.GetFields().Count(f => f.IsPublic), 0);
			Assert.IsTrue(type.GetProperty("Name")?.CanRead ?? false, "Нет свойства Name");
            Assert.IsTrue(type.GetProperty("Teams")?.CanRead ?? false, "Нет свойства Teams");
            Assert.IsFalse(type.GetProperty("Name")?.CanWrite ?? false, "Name должно быть только для чтения");
            Assert.IsFalse(type.GetProperty("Teams")?.CanWrite ?? false, "Teams должно быть только для чтения");
			Assert.IsNotNull(type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(string) }, null), "Нет публичного конструктора Group(string name)");
			Assert.IsNotNull(type.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(Lab7.Blue.Task4.Team) }, null), "Нет публичного метода Add(Team team)");
			Assert.IsNotNull(type.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(Lab7.Blue.Task4.Team[]) }, null), "Нет публичного метода Add(Team[] team)");
			Assert.IsNotNull(type.GetMethod("Sort", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null), "Нет публичного метода Sort()");
			Assert.IsNotNull(type.GetMethod("Merge", BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(Lab7.Blue.Task4.Group), typeof(Lab7.Blue.Task4.Group), typeof(int) }, null), "Нет публичного статического метода Merge(Group group1, Group group2, int size)");
			Assert.IsNotNull(type.GetMethod("Print", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null), "Нет публичного метода Print()");
			Assert.AreEqual(type.GetProperties().Count(f => f.PropertyType.IsPublic), 2);
			Assert.AreEqual(type.GetConstructors().Count(f => f.IsPublic), 1);
			Assert.AreEqual(type.GetMethods().Count(f => f.IsPublic), 11);
		}

        [TestMethod]
        public void Test_01_CreateTeams()
        {
            Assert.AreEqual(_teams.Length, _inputGroups.Sum(g => g.Teams.Length));
        }

        [TestMethod]
        public void Test_02_InitTeams()
        {
            CheckTeams(scoresExpected: false);
        }

        [TestMethod]
        public void Test_03_PlayMatches()
        {
            PlayMatches();
            CheckTeams(scoresExpected: true);
        }

        [TestMethod]
        public void Test_04_CreateGroups()
        {
            CheckGroups(filled: false);
        }

        [TestMethod]
        public void Test_05_FillGroups()
        {
            PlayMatches();
            FillGroups();
            CheckGroups(filled: true);
        }

        [TestMethod]
        public void Test_06_FillGroups()
        {
            PlayMatches();
            FillManyGroups();
            CheckGroups(filled: true);
        }

		[TestMethod]
		public void Test_07_FillGroups()
		{
			PlayMatches();
			FillGroups();
			FillManyGroups();
			FillGroups();
			CheckGroups(filled: true);
		}

		[TestMethod]
        public void Test_08_SortGroups()
        {
            PlayMatches();
            FillGroups();
            foreach (var g in _groups) g.Sort();
            CheckGroups(filled: true, sorted: true);
        }

        [TestMethod]
        public void Test_09_MergeFinalists()
        {
            PlayMatches();
            FillGroups();
            foreach (var g in _groups) g.Sort();

            var merged = Lab7.Blue.Task4.Group.Merge(_groups[0], _groups[1], 12);

            Assert.AreEqual("Финалисты", merged.Name);
            Assert.AreEqual(_output.Length, merged.Teams.Length);
            for (int i = 0; i < _output.Length; i++)
            {
                Assert.AreEqual(_output[i].Name, merged.Teams[i].Name);
                Assert.AreEqual(_output[i].TotalScore, merged.Teams[i].TotalScore);
            }
        }

        private void PlayMatches()
        {
            int idx = 0;
            foreach (var g in _inputGroups)
            {
                foreach (var t in g.Teams)
                {
                    foreach (var s in t.Scores)
                        _teams[idx].PlayMatch(s);
                    idx++;
                }
            }
        }

        private void FillGroups()
        {
            int idx = 0;
            for (int gi = 0; gi < _groups.Length; gi++)
            {
                var teamsToAdd = _teams.Skip(idx).Take(_inputGroups[gi].Teams.Length).ToArray();
                foreach (var team in teamsToAdd)
                    _groups[gi].Add(team);
                idx += _inputGroups[gi].Teams.Length;
            }
        }
        private void FillManyGroups()
        {
            int idx = 0;
            for (int gi = 0; gi < _groups.Length; gi++)
            {
                var teamsToAdd = _teams.Skip(idx).Take(_inputGroups[gi].Teams.Length).ToArray();
                _groups[gi].Add(teamsToAdd);
                idx += _inputGroups[gi].Teams.Length;
            }
		}

		private void CheckTeams(bool scoresExpected)
        {
            int idx = 0;
            foreach (var g in _inputGroups)
            {
                foreach (var t in g.Teams)
                {
                    var team = _teams[idx];
                    Assert.AreEqual(t.Name, team.Name);
                    if (scoresExpected)
                        Assert.AreEqual(t.Scores.Sum(), team.TotalScore);
                    else
                        Assert.AreEqual(0, team.TotalScore);
                    idx++;
                }
            }
        }

        private void CheckGroups(bool filled, bool sorted = false)
        {
            for (int i = 0; i < _groups.Length; i++)
            {
                var group = _groups[i];
                if (!filled)
                {
                    Assert.AreEqual(_inputGroups[i].Name, group.Name);
                    Assert.IsTrue(group.Teams.Length == 12);
                }
                else if (!sorted)
                {
                    Assert.AreEqual(_inputGroups[i].Name, group.Name);
                    Assert.AreEqual(_inputGroups[i].Teams.Length, group.Teams.Length);
                }
                else
                {
                    var scores = group.Teams.Select(t => t.TotalScore).ToArray();
                    for (int j = 1; j < scores.Length; j++)
                        Assert.IsTrue(scores[j - 1] >= scores[j],
                            $"Группа {group.Name} не отсортирована по TotalScores");
                }
            }
        }
    }
}
