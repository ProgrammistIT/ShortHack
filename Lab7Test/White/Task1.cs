//using System.Reflection;
//using System.Text.Json;

//namespace Lab7Test.White
//{
//    [TestClass]
//    public sealed class Task1
//    {
//        record InputRow(string Surname, string Club, double FirstJump, double SecondJump);
//        record OutputRow(string Surname, string Club, double JumpSum);

//        private InputRow[] _input;
//        private OutputRow[] _output;
//        private Lab7.White.Task1.Participant[] _student;

//        [TestInitialize]
//        public void LoadData()
//        {
//            var folder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
//            folder = Path.Combine(folder, "Lab7Test", "White"); 
//            var input = JsonSerializer.Deserialize<JsonElement>(
//                File.ReadAllText(Path.Combine(folder, "input.json")))!;
//            var output = JsonSerializer.Deserialize<JsonElement>(
//                File.ReadAllText(Path.Combine(folder, "output.json")))!;

//            _input = input.GetProperty("Task1").Deserialize<InputRow[]>();
//            _output = output.GetProperty("Task1").Deserialize<OutputRow[]>();
//            _student = new Lab7.White.Task1.Participant[_input.Length];
//        }

//        [TestMethod]
//        public void Test_00_OOP()
//        {
//            var type = typeof(Lab7.White.Task1.Participant);
//            Assert.IsTrue(type.IsValueType, "Participant должен быть структурой");
//            Assert.AreEqual(type.GetFields().Count(f => f.IsPublic), 0);
//            Assert.IsTrue(type.GetProperty("Surname")?.CanRead ?? false, "Нет свойства Surname");
//            Assert.IsTrue(type.GetProperty("Club")?.CanRead ?? false, "Нет свойства Club");
//            Assert.IsTrue(type.GetProperty("FirstJump")?.CanRead ?? false, "Нет свойства FirstJump");
//            Assert.IsTrue(type.GetProperty("SecondJump")?.CanRead ?? false, "Нет свойства SecondJump");
//            Assert.IsTrue(type.GetProperty("JumpSum")?.CanRead ?? false, "Нет свойства JumpSum");
//            Assert.IsFalse(type.GetProperty("Surname")?.CanWrite ?? false, "Свойство Surname должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("Club")?.CanWrite ?? false, "Свойство Club должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("FirstJump")?.CanWrite ?? false, "Свойство FirstJump должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("SecondJump")?.CanWrite ?? false, "Свойство SecondJump должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("JumpSum")?.CanWrite ?? false, "Свойство JumpSum должно быть только для чтения");
//            Assert.IsNotNull(type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(string), typeof(string) }, null),
//                "Нет публичного конструктора Participant(string surname, string club)");
//            Assert.IsNotNull(type.GetMethod("Jump", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(double) }, null), 
//                "Нет публичного метода Jump(double result)");
//            Assert.IsNotNull(type.GetMethod("Sort", BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(Lab7.White.Task1.Participant[]) }, null),
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
//                Assert.AreEqual(_input[i].Surname, _student[i].Surname);
//                Assert.AreEqual(_input[i].Club, _student[i].Club);
//                Assert.AreEqual(0, _student[i].FirstJump, 0.0001);
//                Assert.AreEqual(0, _student[i].SecondJump, 0.0001);
//                Assert.AreEqual(0, _student[i].JumpSum, 0.0001);
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
//            Lab7.White.Task1.Participant.Sort(_student);

//            Assert.AreEqual(_output.Length, _student.Length);
//            for (int i = 0; i < _student.Length; i++)
//            {
//                Assert.AreEqual(_output[i].Surname, _student[i].Surname);
//                Assert.AreEqual(_output[i].Club, _student[i].Club);
//                Assert.AreEqual(_output[i].JumpSum, _student[i].JumpSum, 0.01);
//            }
//        }

//        private void Init()
//        {
//            for (int i = 0; i < _input.Length; i++)
//            {
//                _student[i] = new Lab7.White.Task1.Participant(_input[i].Surname, _input[i].Club);
//            }
//        }

//        private void Jump()
//        {
//            for (int i = 0; i < _input.Length; i++)
//            {
//                _student[i].Jump(_input[i].FirstJump);
//                _student[i].Jump(_input[i].SecondJump);
//                _student[i].Jump(1);
//            }
//        }

//        private void CheckStruct(bool jumpsExpected)
//        {
//            for (int i = 0; i < _input.Length; i++)
//            {
//                Assert.AreEqual(_input[i].Surname, _student[i].Surname);
//                Assert.AreEqual(_input[i].Club, _student[i].Club);

//                if (jumpsExpected)
//                {
//                    Assert.AreEqual(_input[i].FirstJump, _student[i].FirstJump, 0.0001);
//                    Assert.AreEqual(_input[i].SecondJump, _student[i].SecondJump, 0.0001);
//                    Assert.AreEqual(_input[i].FirstJump + _input[i].SecondJump, _student[i].JumpSum, 0.0001);
//                }
//                else
//                {
//                    Assert.AreEqual(0, _student[i].FirstJump, 0.0001);
//                    Assert.AreEqual(0, _student[i].SecondJump, 0.0001);
//                    Assert.AreEqual(0, _student[i].JumpSum, 0.0001);
//                }
//            }
//        }
//    }
//}
