//using System.Reflection;
//using System.Text.Json;

//namespace Lab7Test.Green
//{
//    [TestClass]
//    public sealed class Task1
//    {
//        record InputRow(string Surname, string Group, string Trainer, double Result);
//        record OutputRow(string Surname, string Group, string Trainer, double Result, bool HasPassed);

//        private InputRow[] _input;
//        private OutputRow[] _output;
//        private Lab7.Green.Task1.Participant[] _student;

//        [TestInitialize]
//        public void LoadData()
//        {
//            var field = typeof(Lab7.Green.Task1.Participant)
//                .GetField("_passed", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
//            field.SetValue(null, 0);

//            var folder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
//            folder = Path.Combine(folder, "Lab7Test", "Green");

//            var input = JsonSerializer.Deserialize<JsonElement>(
//                File.ReadAllText(Path.Combine(folder, "input.json")))!;
//            var output = JsonSerializer.Deserialize<JsonElement>(
//                File.ReadAllText(Path.Combine(folder, "output.json")))!;

//            _input = input.GetProperty("Task1").Deserialize<InputRow[]>()!;
//            _output = output.GetProperty("Task1").Deserialize<OutputRow[]>();
//            _student = new Lab7.Green.Task1.Participant[_input.Length];
//        }

//        [TestMethod]
//        public void Test_00_OOP()
//        {
//            var type = typeof(Lab7.Green.Task1.Participant);
//            Assert.IsTrue(type.IsValueType, "Participant должен быть структурой");
//            Assert.AreEqual(type.GetFields().Count(f => f.IsPublic), 0);
//            Assert.IsTrue(type.GetProperty("Surname")?.CanRead ?? false, "Нет свойства Surname");
//            Assert.IsTrue(type.GetProperty("Group")?.CanRead ?? false, "Нет свойства Group");
//            Assert.IsTrue(type.GetProperty("Trainer")?.CanRead ?? false, "Нет свойства Trainer");
//            Assert.IsTrue(type.GetProperty("Result")?.CanRead ?? false, "Нет свойства Result");
//            Assert.IsTrue(type.GetProperty("HasPassed")?.CanRead ?? false, "Нет свойства HasPassed");
//            Assert.IsFalse(type.GetProperty("Surname")?.CanWrite ?? false, "Свойство Surname должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("Group")?.CanWrite ?? false, "Свойство Group должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("Trainer")?.CanWrite ?? false, "Свойство Trainer должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("Result")?.CanWrite ?? false, "Свойство Result должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("HasPassed")?.CanWrite ?? false, "Свойство HasPassed должно быть только для чтения");
//            Assert.IsNotNull(type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(string), typeof(string), typeof(string) }, null), "Нет публичного конструктора Participant(string surname, string group, string trainer)");
//            Assert.IsNotNull(type.GetMethod("Run", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(double) }, null), "Нет публичного метода Run(double result)");
//            Assert.IsNotNull(type.GetMethod("Print", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null), "Нет публичного метода Print()");
//            Assert.AreEqual(type.GetProperties().Count(f => f.PropertyType.IsPublic), 6);
//            Assert.AreEqual(type.GetConstructors().Count(f => f.IsPublic), 1);
//            Assert.AreEqual(type.GetMethods().Count(f => f.IsPublic), 12);
//        }

//        [TestMethod]
//        public void Test_01_Create()
//        {
//            Init();
//        }

//        [TestMethod]
//        public void Test_02_Init()
//        {
//            Init();
//            CheckStruct(runExpected: false);
//        }

//        [TestMethod]
//        public void Test_02_Run()
//        {
//            Init();
//            Run();
//            CheckStruct(runExpected: true);
//        }

//        [TestMethod]
//        public void Test_03_Count()
//        {
//            Init();
//            Run();
//            double expectedPassed = _output.Count(o => o.HasPassed);
//            Assert.AreEqual(expectedPassed, Lab7.Green.Task1.Participant.PassedTheStandard);
//        }

//        private void Init()
//        {
//            for (int i = 0; i < _input.Length; i++)
//            {
//                _student[i] = new Lab7.Green.Task1.Participant(
//                    _input[i].Surname,
//                    _input[i].Group,
//                    _input[i].Trainer
//                );
//            }
//        }

//        private void Run()
//        {
//            for (int i = 0; i < _input.Length; i++)
//            {
//                _student[i].Run(_input[i].Result);
//                _student[i].Run(-1);
//            }
//        }

//        private void CheckStruct(bool runExpected)
//        {
//            for (int i = 0; i < _input.Length; i++)
//            {
//                Assert.AreEqual(_output[i].Surname, _student[i].Surname);
//                Assert.AreEqual(_output[i].Group, _student[i].Group);
//                Assert.AreEqual(_output[i].Trainer, _student[i].Trainer);

//                if (runExpected)
//                {
//                    Assert.AreEqual(_output[i].Result, _student[i].Result, 0.0001);
//                    Assert.AreEqual(_output[i].HasPassed, _student[i].HasPassed);
//                }
//                else
//                {
//                    Assert.AreEqual(0, _student[i].Result, 0.0001);
//                    Assert.IsFalse(_student[i].HasPassed);
//                }
//            }
//        }
//    }
//}
