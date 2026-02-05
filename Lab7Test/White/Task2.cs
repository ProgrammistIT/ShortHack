//using System.Reflection;
//using System.Text.Json;

//namespace Lab7Test.White
//{
//    [TestClass]
//    public sealed class Task2
//    {
//        record InputRow(string Name, string Surname, double FirstJump, double SecondJump);
//        record OutputRow(string Name, string Surname, double BestJump);

//        private InputRow[] _input;
//        private OutputRow[] _output;
//        private Lab7.White.Task2.Participant[] _student;

//        [TestInitialize]
//        public void LoadData()
//        {
//            var folder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
//            folder = Path.Combine(folder, "Lab7Test", "White");
//            var input = JsonSerializer.Deserialize<JsonElement>(
//                File.ReadAllText(Path.Combine(folder, "input.json")))!;
//            var output = JsonSerializer.Deserialize<JsonElement>(
//                File.ReadAllText(Path.Combine(folder, "output.json")))!;

//            _input = input.GetProperty("Task2").Deserialize<InputRow[]>();
//            _output = output.GetProperty("Task2").Deserialize<OutputRow[]>();
//            _student = new Lab7.White.Task2.Participant[_input.Length];
//        }

//        [TestMethod]
//        public void Test_00_OOP()
//        {
//            var type = typeof(Lab7.White.Task2.Participant);
//            Assert.IsTrue(type.IsValueType, "Participant должен быть структурой");
//            Assert.AreEqual(type.GetFields().Count(f => f.IsPublic), 0);
//            Assert.IsTrue(type.GetProperty("Name")?.CanRead ?? false, "Нет свойства Name");
//            Assert.IsTrue(type.GetProperty("Surname")?.CanRead ?? false, "Нет свойства Surname");
//            Assert.IsTrue(type.GetProperty("FirstJump")?.CanRead ?? false, "Нет свойства FirstJump");
//            Assert.IsTrue(type.GetProperty("SecondJump")?.CanRead ?? false, "Нет свойства SecondJump");
//            Assert.IsTrue(type.GetProperty("BestJump")?.CanRead ?? false, "Нет свойства BestJump");
//            Assert.IsFalse(type.GetProperty("Name")?.CanWrite ?? false, "Свойство Name должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("Surname")?.CanWrite ?? false, "Свойство Surname должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("FirstJump")?.CanWrite ?? false, "Свойство FirstJump должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("SecondJump")?.CanWrite ?? false, "Свойство SecondJump должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("BestJump")?.CanWrite ?? false, "Свойство BestJump должно быть только для чтения");
//            Assert.IsNotNull(type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(string), typeof(string) }, null),
//                "Нет публичного конструктора Participant(string surname, string club)");
//            Assert.IsNotNull(type.GetMethod("Jump", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(double) }, null),
//                "Нет публичного метода Jump(double result)");
//            Assert.IsNotNull(type.GetMethod("Sort", BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(Lab7.White.Task2.Participant[]) }, null),
//                "Нет публичного статического метода Sort(Participant[] array)");
//            Assert.IsNotNull(type.GetMethod("Print", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null),
//                "Нет публичного метода Print()");
//            Assert.AreEqual(type.GetProperties().Count(f => f.PropertyType.IsPublic), 5);
//            Assert.AreEqual(type.GetConstructors().Count(f => f.IsPublic), 1);
//            Assert.AreEqual(type.GetMethods().Count(f => f.IsPublic), 12);
//        }

//        [TestMethod]
//        public void Test_01_Create()
//        {
//            Init();
//            for (int i = 0; i < _student.Length; i++)
//            {
//                Assert.AreEqual(_input[i].Name, _student[i].Name);
//                Assert.AreEqual(_input[i].Surname, _student[i].Surname);
//                Assert.AreEqual(0, _student[i].FirstJump, 0.0001);
//                Assert.AreEqual(0, _student[i].SecondJump, 0.0001);
//                Assert.AreEqual(0, _student[i].BestJump, 0.0001);
//            }
//        }

//        [TestMethod]
//        public void Test_02_Init()
//        {
//            Init();
//            CheckStruct(jumpsExpected: false);
//        }

//        [TestMethod]
//        public void Test_03_Jumps()
//        {
//            Init();
//            Jump();
//            CheckStruct(jumpsExpected: true);
//        }

//        [TestMethod]
//        public void Test_04_Sort()
//        {
//            Init();
//            Jump();
//            Lab7.White.Task2.Participant.Sort(_student);

//            Assert.AreEqual(_output.Length, _student.Length);
//            for (int i = 0; i < _student.Length; i++)
//            {
//                Assert.AreEqual(_output[i].Name, _student[i].Name);
//                Assert.AreEqual(_output[i].Surname, _student[i].Surname);
//                Assert.AreEqual(_output[i].BestJump, _student[i].BestJump, 0.01);
//            }
//        }

//        private void Init()
//        {
//            for (int i = 0; i < _input.Length; i++)
//            {
//                _student[i] = new Lab7.White.Task2.Participant(_input[i].Name, _input[i].Surname);
//            }
//        }

//        private void Jump()
//        {
//            for (int i = 0; i < _input.Length; i++)
//            {
//                _student[i].Jump(_input[i].FirstJump);
//                _student[i].Jump(_input[i].SecondJump);
//                _student[i].Jump(-1);
//            }
//        }

//        private void CheckStruct(bool jumpsExpected)
//        {
//            for (int i = 0; i < _input.Length; i++)
//            {
//                Assert.AreEqual(_input[i].Name, _student[i].Name);
//                Assert.AreEqual(_input[i].Surname, _student[i].Surname);

//                if (jumpsExpected)
//                {
//                    Assert.AreEqual(_input[i].FirstJump, _student[i].FirstJump, 0.0001);
//                    Assert.AreEqual(_input[i].SecondJump, _student[i].SecondJump, 0.0001);
//                    Assert.AreEqual(Math.Max(_input[i].FirstJump, _input[i].SecondJump), _student[i].BestJump, 0.0001);
//                }
//                else
//                {
//                    Assert.AreEqual(0, _student[i].FirstJump, 0.0001);
//                    Assert.AreEqual(0, _student[i].SecondJump, 0.0001);
//                    Assert.AreEqual(0, _student[i].BestJump, 0.0001);
//                }
//            }
//        }
//    }
//}
