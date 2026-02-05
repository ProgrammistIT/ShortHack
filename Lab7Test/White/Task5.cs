//using System.Globalization;
//using System.Reflection;
//using System.Text.Json;

//namespace Lab7Test.White
//{
//    [TestClass]
//    public sealed class Task5
//    {
//        record InputMatch(int Goals, int Misses);
//        record InputTeam(string Name, InputMatch[] Matches);
//        record OutputMatch(int Goals, int Misses);
//        record OutputTeam(string Name, double TotalScore, double TotalDifference, OutputMatch[] Matches);

//        private InputTeam[] _inputTeams;
//        private OutputTeam[] _outputTeams;
//        private Lab7.White.Task5.Match[] _studentMatches;
//        private Lab7.White.Task5.Team[] _studentTeams;

//        [TestInitialize]
//        public void LoadData()
//        {
//            var folder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
//            folder = Path.Combine(folder, "Lab7Test", "White");

//            var input = JsonSerializer.Deserialize<JsonElement>(
//                File.ReadAllText(Path.Combine(folder, "input.json")))!;
//            var output = JsonSerializer.Deserialize<JsonElement>(
//                File.ReadAllText(Path.Combine(folder, "output.json")))!;

//            _inputTeams = input.GetProperty("Task5").Deserialize<InputTeam[]>();
//            _outputTeams = output.GetProperty("Task5").Deserialize<OutputTeam[]>();

//            _studentTeams = new Lab7.White.Task5.Team[_inputTeams.Length];
//            _studentMatches = new Lab7.White.Task5.Match[_inputTeams.Sum(t => t.Matches.Length)];
//        }

//        [TestMethod]
//        public void Test_00_OOP()
//        {
//            var type = typeof(Lab7.White.Task5.Match);
//            Assert.AreEqual(type.GetFields().Count(f => f.IsPublic), 0);
//            Assert.IsTrue(type.IsValueType, "Match должен быть структурой");
//            Assert.IsTrue(type.GetProperty("Goals")?.CanRead ?? false, "Нет свойства Goals");
//            Assert.IsTrue(type.GetProperty("Misses")?.CanRead ?? false, "Нет свойства Misses");
//            Assert.IsTrue(type.GetProperty("Difference")?.CanRead ?? false, "Нет свойства Difference");
//            Assert.IsTrue(type.GetProperty("Score")?.CanRead ?? false, "Нет свойства Score");
//            Assert.IsFalse(type.GetProperty("Goals")?.CanWrite ?? false, "Свойство Goals должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("Misses")?.CanWrite ?? false, "Свойство Misses должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("Difference")?.CanWrite ?? false, "Свойство Difference должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("Score")?.CanWrite ?? false, "Свойство Score должно быть только для чтения"); 
//            Assert.IsNotNull(type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(int), typeof(int) }, null), 
//                "Нет публичного конструктора Match(int scored, int missed)");
//            Assert.IsNotNull(type.GetMethod("Print", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null), 
//                "Нет публичного метода Print()");
//            Assert.AreEqual(type.GetProperties().Count(f => f.PropertyType.IsPublic), 4);
//            Assert.AreEqual(type.GetConstructors().Count(f => f.IsPublic), 1);
//            Assert.AreEqual(type.GetMethods().Count(f => f.IsPublic), 9);

//            type = typeof(Lab7.White.Task5.Team);
//            Assert.AreEqual(type.GetFields().Count(f => f.IsPublic), 0);
//            Assert.IsTrue(type.IsValueType, "Team должен быть структурой");
//            Assert.IsTrue(type.GetProperty("Name")?.CanRead ?? false, "Нет свойства Name");
//            Assert.IsTrue(type.GetProperty("Matches")?.CanRead ?? false, "Нет свойства Matches");
//            Assert.IsTrue(type.GetProperty("TotalScore")?.CanRead ?? false, "Нет свойства TotalScore");
//            Assert.IsTrue(type.GetProperty("TotalDifference")?.CanRead ?? false, "Нет свойства TotalDifference");
//            Assert.IsFalse(type.GetProperty("Name")?.CanWrite ?? false, "Свойство Name должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("Matches")?.CanWrite ?? false, "Свойство Matches должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("TotalScore")?.CanWrite ?? false, "Свойство TotalScore должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("TotalDifference")?.CanWrite ?? false, "Свойство TotalDifference должно быть только для чтения");
//            Assert.IsNotNull(type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(string) }, null), 
//                "Нет публичного конструктора Team(string name)");
//            Assert.IsNotNull(type.GetMethod("PlayMatch", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(int), typeof(int) }, null), 
//                "Нет публичного метода PlayMatch(int scored, int misses)");
//            Assert.IsNotNull(type.GetMethod("SortTeams", BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(Lab7.White.Task5.Team[]) }, null), 
//                "Нет публичного статического метода SortTeams(Team[] array)");
//            Assert.IsNotNull(type.GetMethod("Print", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null), 
//                "Нет публичного метода Print()");
//            Assert.AreEqual(type.GetProperties().Count(f => f.PropertyType.IsPublic), 4);
//            Assert.AreEqual(type.GetConstructors().Count(f => f.IsPublic), 1);
//            Assert.AreEqual(type.GetMethods().Count(f => f.IsPublic), 11);
//        }

//        [TestMethod]
//        public void Test_01_CreateS()
//        {
//            InitTeams();
//        }

//        [TestMethod]
//        public void Test_02_InitS()
//        {
//            InitTeams();
//            CheckTeams();
//        }

//        [TestMethod]
//        public void Test_03_CreateG()
//        {
//            InitTeams();
//            CheckTeams();
//        }

//        [TestMethod]
//        public void Test_04_InitG()
//        {
//            InitTeams();
//            CheckTeams();
//        }

//        [TestMethod]
//        public void Test_05_PlayMatches()
//        {
//            InitTeams();
//            PlayMatches();
//            CheckTeams(true);
//        }

//        [TestMethod]
//        public void Test_06_Sort()
//        {
//            InitTeams();
//            PlayMatches();
//            Lab7.White.Task5.Team.SortTeams(_studentTeams);
//            CheckTeams(true, true);
//        }

//        [TestMethod]
//        public void Test_07_ArrayLinq()
//        {
//            InitTeams();
//            PlayMatches();
//            ArrayLinq();
//            CheckTeams(true);
//        }

//        private void InitTeams()
//        {
//            for (int i = 0; i < _inputTeams.Length; i++)
//            {
//                _studentTeams[i] = new Lab7.White.Task5.Team(_inputTeams[i].Name);
//            }
//        }

//        private void PlayMatches()
//        {
//            for (int i = 0; i < _inputTeams.Length; i++)
//            {
//                foreach (var match in _inputTeams[i].Matches)
//                    _studentTeams[i].PlayMatch(match.Goals, match.Misses);
//            }
//        }

//        private void ArrayLinq()
//        {
//            for (int i = 0; i < _studentTeams.Length; i++)
//            {
//                if (_studentTeams[i].Matches != null)
//                    for (int j = 0; j < _studentTeams[i].Matches.Length / 2; j++)
//                    {
//                        _inputTeams[i].Matches[j] = _inputTeams[i].Matches[j + 1];
//                        _studentTeams[i].Matches[j] = _studentTeams[i].Matches[j + 1];
//                    }
//            }
//        }

//        private void CheckTeams(bool played = false, bool sorted = false)
//        {
//            for (int i = 0; i < _studentTeams.Length; i++)
//            {
//                if (sorted)
//                {
//                    Assert.AreEqual(_outputTeams[i].Name, _studentTeams[i].Name);
//                    Assert.AreEqual(_outputTeams[i].TotalScore, _studentTeams[i].TotalScore, 0.0001);
//                    Assert.AreEqual(_outputTeams[i].TotalDifference, _studentTeams[i].TotalDifference, 0.0001);
//                }
//                else
//                {
//                    Assert.AreEqual(_inputTeams[i].Name, _studentTeams[i].Name);

//                    if (played)
//                    {
//                        Assert.AreEqual(_inputTeams[i].Matches.Length, _studentTeams[i].Matches.Length);
//                        for (int j = 0; j < _studentTeams[i].Matches.Length; j++)
//                        {
//                            Assert.AreEqual(_inputTeams[i].Matches[j].Goals, _studentTeams[i].Matches[j].Goals);
//                            Assert.AreEqual(_inputTeams[i].Matches[j].Misses, _studentTeams[i].Matches[j].Misses);
//                        }
//                    }
//                }
//            }
//        }
//    }
//}
