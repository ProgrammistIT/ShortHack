//using System.Reflection;
//using System.Text.Json;

//namespace Lab7Test.Green
//{
//    [TestClass]
//    public sealed class Task5
//    {
//        record InputStudent(string Name, string Surname, int[] Marks);
//        record InputGroup(string Name, InputStudent[] Students);
//        record OutputGroup(string Name, double AverageMark);

//        private InputGroup[] _inputGroups;
//        private OutputGroup[] _outputGroups;

//        private Lab7.Green.Task5.Student[] _studentS;
//        private Lab7.Green.Task5.Group[] _studentG;

//        [TestInitialize]
//        public void LoadData()
//        {
//            var folder = Directory.GetParent(Directory.GetCurrentDirectory())
//                                  .Parent.Parent.Parent.FullName;
//            folder = Path.Combine(folder, "Lab7Test", "Green");

//            var input = JsonSerializer.Deserialize<JsonElement>(
//                File.ReadAllText(Path.Combine(folder, "input.json")))!;
//            var output = JsonSerializer.Deserialize<JsonElement>(
//                File.ReadAllText(Path.Combine(folder, "output.json")))!;

//            _inputGroups = input.GetProperty("Task5").Deserialize<InputGroup[]>();
//            _outputGroups = output.GetProperty("Task5").Deserialize<OutputGroup[]>();

//            _studentS = _inputGroups.SelectMany(g => g.Students)
//                                     .Select(s => new Lab7.Green.Task5.Student(s.Name, s.Surname))
//                                     .ToArray();
//        }

//        [TestMethod]
//        public void Test_00_OOP()
//        {
//            var type = typeof(Lab7.Green.Task5.Student);
//			Assert.AreEqual(type.GetFields().Count(f => f.IsPublic), 0);
//			Assert.IsTrue(type.IsValueType, "Student должен быть структурой");
//            Assert.IsTrue(type.GetProperty("Name")?.CanRead ?? false, "Нет свойства Name");
//            Assert.IsTrue(type.GetProperty("Surname")?.CanRead ?? false, "Нет свойства Surname");
//            Assert.IsTrue(type.GetProperty("AverageMark")?.CanRead ?? false, "Нет свойства AverageMark");
//            Assert.IsTrue(type.GetProperty("Marks")?.CanRead ?? false, "Нет свойства Marks");
//            Assert.IsFalse(type.GetProperty("Name")?.CanWrite ?? false, "Свойство Name должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("Surname")?.CanWrite ?? false, "Свойство Surname должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("AverageMark")?.CanWrite ?? false, "Свойство AverageMark должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("Marks")?.CanWrite ?? false, "Свойство Marks должно быть только для чтения");
//			Assert.IsNotNull(type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(string), typeof(string) }, null), "Нет публичного конструктора Student(string name, string surname)");
//			Assert.IsNotNull(type.GetMethod("Exam", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(int) }, null), "Нет публичного метода Exam(int mark)");
//			Assert.IsNotNull(type.GetMethod("Print", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null), "Нет публичного метода Print()");
//			Assert.AreEqual(type.GetProperties().Count(f => f.PropertyType.IsPublic), 4);
//			Assert.AreEqual(type.GetConstructors().Count(f => f.IsPublic), 1);
//			Assert.AreEqual(type.GetMethods().Count(f => f.IsPublic), 10);

//			type = typeof(Lab7.Green.Task5.Group);
//			Assert.AreEqual(type.GetFields().Count(f => f.IsPublic), 0);
//			Assert.IsTrue(typeof(Lab7.Green.Task5.Group).IsValueType, "Group должен быть структурой");
//            Assert.IsTrue(type.GetProperty("Name")?.CanRead ?? false, "Нет свойства Name");
//            Assert.IsTrue(type.GetProperty("Students")?.CanRead ?? false, "Нет свойства Students");
//            Assert.IsTrue(type.GetProperty("AverageMark")?.CanRead ?? false, "Нет свойства AverageMark");
//            Assert.IsFalse(type.GetProperty("Name")?.CanWrite ?? false, "Свойство Name должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("Students")?.CanWrite ?? false, "Свойство Students должно быть только для чтения");
//            Assert.IsFalse(type.GetProperty("AverageMark")?.CanWrite ?? false, "Свойство AverageMark должно быть только для чтения");
//			Assert.IsNotNull(type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(string) }, null), "Нет публичного конструктора Group(string name)");
//			Assert.IsNotNull(type.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(Lab7.Green.Task5.Student) }, null), "Нет публичного метода Add(Student elem)");
//			Assert.IsNotNull(type.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(Lab7.Green.Task5.Student[]) }, null), "Нет публичного метода Add(Student[] array)");
//			Assert.IsNotNull(type.GetMethod("SortByAverageMark", BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(Lab7.Green.Task5.Group[]) }, null), "Нет публичного статического метода SortByAverageMark(Group[] array)");
//			Assert.IsNotNull(type.GetMethod("Print", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null), "Нет публичного метода Print()");
//			Assert.AreEqual(type.GetProperties().Count(f => f.PropertyType.IsPublic), 3);
//			Assert.AreEqual(type.GetConstructors().Count(f => f.IsPublic), 1);
//			Assert.AreEqual(type.GetMethods().Count(f => f.IsPublic), 11);
//		}

//        [TestMethod]
//        public void Test_01_CreateS()
//        {
//            Assert.AreEqual(_studentS.Length, _inputGroups.Sum(g => g.Students.Length));
//        }

//        [TestMethod]
//        public void Test_02_InitS()
//        {
//            CheckStudents(marksExpected: false);
//        }

//        [TestMethod]
//        public void Test_03_Exams()
//        {
//            RunExams();
//            CheckStudents(marksExpected: true);
//        }

//        [TestMethod]
//        public void Test_04_ArrayLinq()
//        {
//            RunExams();
//            ArrayLinq();
//            CheckStudents(marksExpected: true);
//        }

//        [TestMethod]
//        public void Test_05_CreateG()
//        {
//            InitGroups();
//            CheckGroups(filled: false, sorted: false);
//        }

//        [TestMethod]
//        public void Test_06_FillG()
//        {
//            InitGroups();
//            FillGroups();
//            CheckGroups(filled: true, sorted: false);
//        }

//        [TestMethod]
//        public void Test_07_FillManyGroups()
//        {
//            InitGroups();
//            FillManyGroups();
//            CheckGroups(filled: true, sorted: false);
//        }

//        [TestMethod]
//        public void Test_08_ArrayLinqG()
//        {
//            InitGroups();
//            FillGroups();
//            ArrayLinqG();
//            CheckGroups(filled: true, sorted: false);
//        }
//        [TestMethod]
//        public void Test_09_Sort()
//        {
//            RunExams();
//            InitGroups();
//            FillGroups();
//            Lab7.Green.Task5.Group.SortByAverageMark(_studentG);
//            CheckGroups(filled: true, sorted: true);
//        }

//        private void RunExams()
//        {
//            int idx = 0;
//            foreach (var g in _inputGroups)
//            {
//                foreach (var s in g.Students)
//                {
//                    foreach (var m in s.Marks)
//                        _studentS[idx].Exam(m);
//                    idx++;
//                }
//            }
//        }
//        private void ArrayLinq()
//        {
//            for (int i = 0; i < _studentS.Length; i++)
//            {
//                var marksCount = _studentS[i].Marks.Length;
//                for (int j = 0; j < marksCount; j++)
//                {
//                    _studentS[i].Marks[0] = -1;
//                }
//            }
//        }

//        private void ArrayLinqG()
//        {
//            foreach (var g in _inputGroups)
//                for (int i = 0; i < g.Students.Length - 1; i++)
//                    g.Students[i] = g.Students[i + 1];
//        }
//        private void InitGroups()
//        {
//            _studentG = _inputGroups.Select(g => new Lab7.Green.Task5.Group(g.Name)).ToArray();
//        }

//        private void FillGroups()
//        {
//            for (int i = 0; i < _studentG.Length; i++)
//            {
//                var students = _inputGroups[i].Students;
//                int startIdx = _inputGroups.Take(i).Sum(g => g.Students.Length);
//                for (int j = 0; j < students.Length; j++)
//                {
//                    int idx = startIdx + j;
//                    _studentG[i].Add(_studentS[idx]);
//                }
//            }
//        }

//        private void FillManyGroups()
//        {
//            for (int i = 0; i < _studentG.Length; i++)
//            {
//                var startIdx = _inputGroups.Take(i).Sum(g => g.Students.Length);
//                var count = _inputGroups[i].Students.Length;
//                var studentsToAdd = _studentS.Skip(startIdx).Take(count).ToArray();
//                _studentG[i].Add(studentsToAdd);
//            }
//        }

//        private void CheckStudents(bool marksExpected)
//        {
//            int idx = 0;
//            foreach (var g in _inputGroups)
//            {
//                foreach (var s in g.Students)
//                {
//                    var student = _studentS[idx];
//                    Assert.AreEqual(s.Name, student.Name);
//                    Assert.AreEqual(s.Surname, student.Surname);
//                    if (marksExpected)
//                        Assert.AreEqual(s.Marks.Average(), student.AverageMark, 0.0001);
//                    else
//                        Assert.AreEqual(0, student.AverageMark, 0.0001);
//                    idx++;
//                }
//            }
//        }

//        private void CheckGroups(bool filled = false, bool sorted = false)
//        {
//            for (int i = 0; i < _studentG.Length; i++)
//            {
//                var group = _studentG[i];
//                if (!sorted)
//                {
//                    Assert.AreEqual(_inputGroups[i].Name, group.Name);

//                    int startIdx = _inputGroups.Take(i).Sum(g => g.Students.Length);
//                    var expectedStudents = _studentS.Skip(startIdx).Take(_inputGroups[i].Students.Length).ToArray();

//                    if (filled)
//                    {
//                        Assert.AreEqual(expectedStudents.Length, group.Students.Length);
//                        for (int j = 0; j < expectedStudents.Length; j++)
//                        {
//                            Assert.AreEqual(expectedStudents[j].Name, group.Students[j].Name);
//                            Assert.AreEqual(expectedStudents[j].Surname, group.Students[j].Surname);
//                            Assert.AreEqual(expectedStudents[j].AverageMark, group.Students[j].AverageMark, 0.0001);
//                        }
//                    }
//                }
//                else
//                {
//                    Assert.AreEqual(_outputGroups[i].Name, group.Name);
//                    Assert.AreEqual(_outputGroups[i].AverageMark, group.AverageMark, 0.0001);
//                }
//            }
//        }
//    }
//}
