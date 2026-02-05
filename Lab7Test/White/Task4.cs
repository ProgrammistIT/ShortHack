//using System.Reflection;
//using System.Text.Json;

//namespace Lab7Test.White
//{
//    [TestClass]
//    public sealed class Task4
//    {
//        record InputRow(string Name, string Surname, double[] Scores);
//        record OutputRow(string Name, string Surname, double TotalScore);

//        private InputRow[] _input;
//        private OutputRow[] _output;
//        private Lab7.White.Task4.Participant[] _student;

//        [TestInitialize]
//        public void LoadData()
//        {
//            var folder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
//            folder = Path.Combine(folder, "Lab7Test", "White");

//            var input = JsonSerializer.Deserialize<JsonElement>(
//                File.ReadAllText(Path.Combine(folder, "input.json")))!;
//            var output = JsonSerializer.Deserialize<JsonElement>(
//                File.ReadAllText(Path.Combine(folder, "output.json")))!;

//            _input = input.GetProperty("Task4").Deserialize<InputRow[]>();
//            _output = output.GetProperty("Task4").Deserialize<OutputRow[]>();
//            _student = new Lab7.White.Task4.Participant[_input.Length];
//        }

//        [TestMethod]
//        public void Test_00_OOP()
//        {
//            var type = typeof(Lab7.White.Task4.Participant);
//            Assert.AreEqual(type.GetFields().Count(f => f.IsPublic), 0);
//            Assert.IsTrue(type.IsValueType, "Participant должен быть структурой");
//            Assert.IsTrue(type.GetProperty("Name")?.CanRead ?? false, "Нет свойства Name");
//            Assert.IsTrue(type.GetProperty("Surname")?.CanRead ?? false, "Нет свойства Surname");
//            Assert.IsTrue(type.GetProperty("TotalScore")?.CanRead ?? false, "Нет свойства TotalScore");
//            Assert.IsTrue(type.GetProperty("Scores")?.CanRead ?? false, "Нет свойства Scores");
//            Assert.IsFalse(type.GetProperty("Name")?.CanWrite ?? false, "Свойство Name должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("Surname")?.CanWrite ?? false, "Свойство Surname должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("TotalScore")?.CanWrite ?? false, "Свойство TotalScore должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("Scores")?.CanWrite ?? false, "Свойство Scores должно быть только для чтения");
//            Assert.IsNotNull(type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(string), typeof(string) }, null),
//                "Нет публичного конструктора Participant(string name, string surname)");
//            Assert.IsNotNull(type.GetMethod("PlayMatch", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(double) }, null),
//                "Нет публичного метода PlayMatch(double result)");
//            Assert.IsNotNull(type.GetMethod("Sort", BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(Lab7.White.Task4.Participant[]) }, null),
//                "Нет публичного статического метода Sort(Participant[] array)");
//            Assert.IsNotNull(type.GetMethod("Print", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null),
//                "Нет публичного метода Print()");
//            Assert.AreEqual(type.GetProperties().Count(f => f.PropertyType.IsPublic), 4);
//            Assert.AreEqual(type.GetConstructors().Count(f => f.IsPublic), 1);
//            Assert.AreEqual(type.GetMethods().Count(f => f.IsPublic), 11);
//        }

//        [TestMethod]
//        public void Test_01_Create()
//        {
//            Init();
//            CheckStruct(justNames: true);
//        }

//        [TestMethod]
//        public void Test_02_Init()
//        {
//            Init();
//            CheckStruct(justNames: true);
//        }

//        [TestMethod]
//        public void Test_03_PlayMatches()
//        {
//            Init();
//            PlayMatches();
//            CheckStruct(justNames: false);
//        }

//        [TestMethod]
//        public void Test_04_Sort()
//        {
//            Init();
//            PlayMatches();
//            Lab7.White.Task4.Participant.Sort(_student);
//            CheckStruct(justNames: false, sorted:true);
//        }

//        [TestMethod]
//        public void Test_05_ArrayLinq()
//        {
//            Init();
//            PlayMatches();
//            ArrayLinq();
//            CheckStruct(justNames: false);
//        }

//        private void Init()
//        {
//            for (int i = 0; i < _input.Length; i++)
//            {
//                _student[i] = new Lab7.White.Task4.Participant(_input[i].Name, _input[i].Surname);
//            }
//        }

//        private void PlayMatches()
//        {
//            for (int i = 0; i < _input.Length; i++)
//            {
//                foreach (var score in _input[i].Scores)
//                    _student[i].PlayMatch(score);
//            }
//        }

//        private void ArrayLinq()
//        {
//            for (int i = 0; i < _student.Length; i++)
//            {
//                if (_student[i].Scores != null)
//                    for (int j = 0; j < _student[i].Scores.Length; j++)
//                        _student[i].Scores[j] = -1;
//            }
//        }

//        private void CheckStruct(bool justNames, bool sorted = false)
//        {
//            for (int i = 0; i < _student.Length; i++)
//            {
//                if (sorted)
//                {
//                    Assert.AreEqual(_output[i].Name, _student[i].Name);
//                    Assert.AreEqual(_output[i].Surname, _student[i].Surname);
//                    Assert.AreEqual(_output[i].TotalScore, _student[i].TotalScore, 0.0001);
//                }
//                else
//                {
//                    Assert.AreEqual(_input[i].Name, _student[i].Name);
//                    Assert.AreEqual(_input[i].Surname, _student[i].Surname);

//                    if (!justNames)
//                    {

//                        Assert.AreEqual(_input[i].Scores.Sum(), _student[i].TotalScore, 0.0001);
//                        if (_student[i].Scores != null)
//                        {
//                            Assert.AreEqual(_input[i].Scores.Length, _student[i].Scores.Length);
//                            for (int j = 0; j < _student[i].Scores.Length; j++)
//                                Assert.AreEqual(_input[i].Scores[j], _student[i].Scores[j], 0.0001);
//                        }
//                    }
//                }
//            }
//        }
//    }
//}
