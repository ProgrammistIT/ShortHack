//using System.Reflection;
//using System.Text.Json;

//namespace Lab7Test.Green
//{
//    [TestClass]
//    public sealed class Task3
//    {
//        record InputRow(string Name, string Surname, int[] Marks);
//        record OutputRow(string Name, string Surname, double AverageMark, bool IsExpelled);

//        private InputRow[] _input;
//        private OutputRow[] _output;
//        private Lab7.Green.Task3.Student[] _student;

//        [TestInitialize]
//        public void LoadData()
//        {
//            var folder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
//            folder = Path.Combine(folder, "Lab7Test", "Green");

//            var input = JsonSerializer.Deserialize<JsonElement>(
//                File.ReadAllText(Path.Combine(folder, "input.json")))!;
//            var output = JsonSerializer.Deserialize<JsonElement>(
//                File.ReadAllText(Path.Combine(folder, "output.json")))!;

//            _input = input.GetProperty("Task3").Deserialize<InputRow[]>();
//            _output = output.GetProperty("Task3").Deserialize<OutputRow[]>();
//            _student = new Lab7.Green.Task3.Student[_input.Length];
//        }

//        [TestMethod]
//        public void Test_00_OOP()
//        {
//            var type = typeof(Lab7.Green.Task3.Student);
//			Assert.AreEqual(type.GetFields().Count(f => f.IsPublic), 0);
//			Assert.IsTrue(type.IsValueType, "Student должен быть структурой");
//            Assert.IsTrue(type.GetProperty("Name")?.CanRead ?? false, "Нет свойства Name");
//            Assert.IsTrue(type.GetProperty("Surname")?.CanRead ?? false, "Нет свойства Surname");
//            Assert.IsTrue(type.GetProperty("AverageMark")?.CanRead ?? false, "Нет свойства AverageMark");
//            Assert.IsTrue(type.GetProperty("IsExpelled")?.CanRead ?? false, "Нет свойства IsExpelled");
//            Assert.IsFalse(type.GetProperty("Name")?.CanWrite ?? false, "Свойство Name должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("Surname")?.CanWrite ?? false, "Свойство Surname должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("AverageMark")?.CanWrite ?? false, "Свойство AverageMark должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("IsExpelled")?.CanWrite ?? false, "Свойство IsExpelled должно быть только для чтения");
//			Assert.IsNotNull(type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(string), typeof(string) }, null), "Нет публичного конструктора Student(string name, string surname)");
//			Assert.IsNotNull(type.GetMethod("Exam", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(int) }, null), "Нет публичного метода Exam(int mark)");
//			Assert.IsNotNull(type.GetMethod("SortByAverageMark", BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(Lab7.Green.Task3.Student[]) }, null), "Нет публичного статического метода SortByAverageMark(Student[] array)");
//			Assert.IsNotNull(type.GetMethod("Print", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null), "Нет публичного метода Print()");
//			Assert.AreEqual(type.GetProperties().Count(f => f.PropertyType.IsPublic), 5);
//			Assert.AreEqual(type.GetConstructors().Count(f => f.IsPublic), 1);
//			Assert.AreEqual(type.GetMethods().Count(f => f.IsPublic), 12);
//		}

//        [TestMethod]
//        public void Test_01_Create()
//        {
//            Init();
//        }

//        [TestMethod]
//        public void Test_02_Init()
//        {
//            Init();
//            CheckStruct(marksExpected: false);
//        }

//        [TestMethod]
//        public void Test_03_Exam()
//        {
//            Init();
//            Exam();
//            CheckStruct(marksExpected: true);
//        }

//        [TestMethod]
//        public void Test_04_Sort()
//        {
//            Init();
//            Exam();
//            Lab7.Green.Task3.Student.SortByAverageMark(_student);
//            CheckStruct(marksExpected: true, sorted:true);
//        }

//        [TestMethod]
//        public void Test_05_ArrayLinq()
//        {
//            Init();
//            Exam();
//            ArrayLinq();
//            CheckStruct(marksExpected: true);
//        }

//        private void Init()
//        {
//            for (int i = 0; i < _input.Length; i++)
//            {
//                _student[i] = new Lab7.Green.Task3.Student(_input[i].Name, _input[i].Surname);
//            }
//        }

//        private void Exam()
//        {
//            for (int i = 0; i < _input.Length; i++)
//            {
//                foreach (var mark in _input[i].Marks)
//                {
//                    _student[i].Exam(mark);
//                }
//            }
//        }

//        private void ArrayLinq()
//        {
//            for (int i = 0; i < _student.Length; i++)
//            {
//                for (int j = 0; j < _student[i].Marks.Length; j++)
//                {
//                    _student[i].Marks[j] = -1;
//                }
//            }
//        }
//        private void CheckStruct(bool marksExpected, bool sorted = false)
//        {
//            for (int i = 0; i < _student.Length; i++)
//            {
//                if (sorted)
//                {

//                    Assert.AreEqual(_output.Length, _student.Length);
//                    Assert.AreEqual(_output[i].Name, _student[i].Name);
//                    Assert.AreEqual(_output[i].Surname, _student[i].Surname);
//                    Assert.AreEqual(_output[i].AverageMark, _student[i].AverageMark, 0.0001);
//                    Assert.AreEqual(_output[i].IsExpelled, _student[i].IsExpelled);
//                }
//                else
//                {
//                    Assert.AreEqual(_input[i].Name, _student[i].Name);
//                    Assert.AreEqual(_input[i].Surname, _student[i].Surname);

//                    if (marksExpected)
//                    {
//                        Assert.AreEqual(_input[i].Marks.Where(m => m > 0).Average(), _student[i].AverageMark, 0.0001);
//                        Assert.AreEqual(_input[i].Marks.Any(m => m == 2), _student[i].IsExpelled);
//                    }
//                    else
//                    {
//                        Assert.AreEqual(0, _student[i].AverageMark, 0.0001);
//                        Assert.IsFalse(_student[i].IsExpelled);
//                    }
//                }
//            }
//        }
//    }
//}
