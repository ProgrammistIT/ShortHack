//using System.Reflection;
//using System.Text.Json;

//namespace Lab7Test.White
//{
//    [TestClass]
//    public sealed class Task3
//    {
//        record InputRow(string Name, string Surname, int[] Marks);
//        record OutputRow(string Name, string Surname, double AverageMark, int Skipped);

//        private InputRow[] _input;
//        private OutputRow[] _output;
//        private Lab7.White.Task3.Student[] _student;

//        [TestInitialize]
//        public void LoadData()
//        {
//            var folder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
//            folder = Path.Combine(folder, "Lab7Test", "White");
//            var input = JsonSerializer.Deserialize<JsonElement>(
//                File.ReadAllText(Path.Combine(folder, "input.json")))!;
//            var output = JsonSerializer.Deserialize<JsonElement>(
//                File.ReadAllText(Path.Combine(folder, "output.json")))!;

//            _input = input.GetProperty("Task3").Deserialize<InputRow[]>();
//            _output = output.GetProperty("Task3").Deserialize<OutputRow[]>();
//            _student = new Lab7.White.Task3.Student[_input.Length];
//        }

//        [TestMethod]
//        public void Test_00_OOP()
//        {
//            var type = typeof(Lab7.White.Task3.Student);
//            Assert.IsTrue(type.IsValueType, "Student должен быть структурой");
//            Assert.AreEqual(type.GetFields().Count(f => f.IsPublic), 0);
//            Assert.IsTrue(type.GetProperty("Name")?.CanRead ?? false, "Нет свойства Name");
//            Assert.IsTrue(type.GetProperty("Surname")?.CanRead ?? false, "Нет свойства Surname");
//            Assert.IsTrue(type.GetProperty("AverageMark")?.CanRead ?? false, "Нет свойства AverageMark");
//            Assert.IsTrue(type.GetProperty("Skipped")?.CanRead ?? false, "Нет свойства Skipped");
//            Assert.IsFalse(type.GetProperty("Name")?.CanWrite ?? false, "Свойство Name должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("Surname")?.CanWrite ?? false, "Свойство Surname должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("AverageMark")?.CanWrite ?? false, "Свойство AverageMark должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("Skipped")?.CanWrite ?? false, "Свойство Skipped должно быть только для чтения");
//            Assert.IsNotNull(type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(string), typeof(string) }, null),
//                "Нет публичного конструктора Student(string name, string surname)");
//            Assert.IsNotNull(type.GetMethod("Lesson", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(int) }, null),
//                "Нет публичного метода Lesson(int mark)");
//            Assert.IsNotNull(type.GetMethod("SortBySkipped", BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(Lab7.White.Task3.Student[]) }, null),
//                "Нет публичного статического метода SortBySkipped(Student[] array)");
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
//            for (int i = 0; i < _student.Length; i++)
//            {
//                Assert.AreEqual(_input[i].Name, _student[i].Name);
//                Assert.AreEqual(_input[i].Surname, _student[i].Surname);
//                Assert.AreEqual(0, _student[i].AverageMark, 0.0001);
//                Assert.AreEqual(0, _student[i].Skipped);
//            }
//        }

//        [TestMethod]
//        public void Test_02_Init()
//        {
//            Init();
//            CheckStruct();
//        }

//        [TestMethod]
//        public void Test_03_Lessons()
//        {
//            Init();
//            Lessons();
//            CheckStruct();
//        }

//        [TestMethod]
//        public void Test_04_Sort()
//        {
//            Init();
//            Lessons();
//            Lab7.White.Task3.Student.SortBySkipped(_student);

//            Assert.AreEqual(_output.Length, _student.Length);
//            for (int i = 0; i < _student.Length; i++)
//            {
//                Assert.AreEqual(_output[i].Name, _student[i].Name);
//                Assert.AreEqual(_output[i].Surname, _student[i].Surname);
//                Assert.AreEqual(_output[i].AverageMark, _student[i].AverageMark, 0.01);
//                Assert.AreEqual(_output[i].Skipped, _student[i].Skipped);
//            }
//        }

//        private void Init()
//        {
//            for (int i = 0; i < _input.Length; i++)
//            {
//                _student[i] = new Lab7.White.Task3.Student(_input[i].Name, _input[i].Surname);
//            }
//        }

//        private void Lessons()
//        {
//            for (int i = 0; i < _input.Length; i++)
//            {
//                foreach (var mark in _input[i].Marks)
//                {
//                    _student[i].Lesson(mark);
//                }
//            }
//        }

//        private void CheckStruct()
//        {
//            for (int i = 0; i < _input.Length; i++)
//            {
//                Assert.AreEqual(_input[i].Name, _student[i].Name);
//                Assert.AreEqual(_input[i].Surname, _student[i].Surname);
//            }
//        }
//    }
//}
