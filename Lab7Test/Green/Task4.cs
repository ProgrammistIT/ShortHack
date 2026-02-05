//using System.Reflection;
//using System.Text.Json;

//namespace Lab7Test.Green
//{
//    [TestClass]
//    public sealed class Task4
//    {
//        record InputRow(string Name, string Surname, double[] Jumps);
//        record OutputRow(string Name, string Surname, double BestJump);

//        private InputRow[] _input;
//        private OutputRow[] _output;
//        private Lab7.Green.Task4.Participant[] _student;

//        [TestInitialize]
//        public void LoadData()
//        {
//            var folder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
//            folder = Path.Combine(folder, "Lab7Test", "Green");

//            var input = JsonSerializer.Deserialize<JsonElement>(
//                File.ReadAllText(Path.Combine(folder, "input.json")))!;
//            var output = JsonSerializer.Deserialize<JsonElement>(
//                File.ReadAllText(Path.Combine(folder, "output.json")))!;

//            _input = input.GetProperty("Task4").Deserialize<InputRow[]>();
//            _output = output.GetProperty("Task4").Deserialize<OutputRow[]>();
//            _student = new Lab7.Green.Task4.Participant[_input.Length];
//        }

//        [TestMethod]
//        public void Test_00_OOP()
//        {
//            var type = typeof(Lab7.Green.Task4.Participant);
//            Assert.IsTrue(type.IsValueType, "Participant должен быть структурой");
//			Assert.AreEqual(type.GetFields().Count(f => f.IsPublic), 0);
//			Assert.IsTrue(type.GetProperty("Name")?.CanRead ?? false, "Нет свойства Name");
//            Assert.IsTrue(type.GetProperty("Surname")?.CanRead ?? false, "Нет свойства Surname");
//            Assert.IsTrue(type.GetProperty("Jumps")?.CanRead ?? false, "Нет свойства Jumps");
//            Assert.IsTrue(type.GetProperty("BestJump")?.CanRead ?? false, "Нет свойства BestJump");
//            Assert.IsFalse(type.GetProperty("Name")?.CanWrite ?? false, "Свойство Name должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("Surname")?.CanWrite ?? false, "Свойство Surname должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("Jumps")?.CanWrite ?? false, "Свойство Jumps должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("BestJump")?.CanWrite ?? false, "Свойство BestJump должно быть только для чтения");
//			Assert.IsNotNull(type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(string), typeof(string) }, null), "Нет публичного конструктора Participant(string name, string surname)");
//			Assert.IsNotNull(type.GetMethod("Jump", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(double) }, null), "Нет публичного метода Jump(double result)");
//			Assert.IsNotNull(type.GetMethod("Sort", BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(Lab7.Green.Task4.Participant[]) }, null), "Нет публичного статического метода Sort(Participant[] array)");
//			Assert.IsNotNull(type.GetMethod("Print", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null), "Нет публичного метода Print()");
//			Assert.AreEqual(type.GetProperties().Count(f => f.PropertyType.IsPublic), 4);
//			Assert.AreEqual(type.GetConstructors().Count(f => f.IsPublic), 1);
//			Assert.AreEqual(type.GetMethods().Count(f => f.IsPublic), 11);
//		}

//        [TestMethod]
//        public void Test_01_Create()
//        {
//            Init();
//            CheckStruct(jumpsExpected: false);
//        }

//        [TestMethod]
//        public void Test_02_Jumps()
//        {
//            Init();
//            RunJumps();
//            CheckStruct(jumpsExpected: true);
//        }

//        [TestMethod]
//        public void Test_03_ArrayLinq()
//        {
//            Init();
//            RunJumps();
//            ArrayLinq();
//            CheckStruct(jumpsExpected: true);
//        }

//        [TestMethod]
//        public void Test_04_Sort()
//        {
//            Init();
//            RunJumps();
//            Lab7.Green.Task4.Participant.Sort(_student);
//            CheckStruct(jumpsExpected: true, sorted: true);
//        }

//        private void Init()
//        {
//            for (int i = 0; i < _input.Length; i++)
//            {
//                _student[i] = new Lab7.Green.Task4.Participant(_input[i].Name, _input[i].Surname);
//            }
//        }

//        private void RunJumps()
//        {
//            for (int i = 0; i < _input.Length; i++)
//            {
//                foreach (var jump in _input[i].Jumps)
//                {
//                    _student[i].Jump(jump);
//                }
//            }
//        }

//        private void ArrayLinq()
//        {
//            for (int i = 0; i < _student.Length; i++)
//            {
//                for (int j = 0; j < _student[i].Jumps.Length; j++)
//                {
//                    _student[i].Jumps[j] = -1;
//                }
//            }
//        }
//        private void CheckStruct(bool jumpsExpected, bool sorted = false)
//        {
//            for (int i = 0; i < _student.Length; i++)
//            {
//                if (sorted)
//                {
//                    Assert.AreEqual(_output.Length, _student.Length);
//                    Assert.AreEqual(_output[i].Name, _student[i].Name);
//                    Assert.AreEqual(_output[i].Surname, _student[i].Surname);
//                    Assert.AreEqual(_output[i].BestJump, _student[i].BestJump, 0.0001);
//                }
//                else
//                {
//                    Assert.AreEqual(_input[i].Name, _student[i].Name);
//                    Assert.AreEqual(_input[i].Surname, _student[i].Surname);

//                    if (jumpsExpected)
//                    {
//                        Assert.AreEqual(_input[i].Jumps.Length, _student[i].Jumps.Length);
//                        Assert.AreEqual(_input[i].Jumps.Max(), _student[i].BestJump, 0.0001);
//                        for (int j = 0; j < _student[i].Jumps.Length; j++)
//                            Assert.AreEqual(_input[i].Jumps[j], _student[i].Jumps[j], 0.0001);
//                    }
//                    else
//                    {
//                        Assert.AreEqual(new int[3].Length, _student[i].Jumps.Length); for (int j = 0; j < _student[i].Jumps.Length; j++)
//                            Assert.AreEqual(0, _student[i].Jumps[j], 0.0001);
//                        Assert.AreEqual(0, _student[i].BestJump, 0.0001);
//                    }
//                }
//            }
//        }

//    }
//}
