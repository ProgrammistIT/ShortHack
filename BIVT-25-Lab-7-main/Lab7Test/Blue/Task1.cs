using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Lab7Test.Blue
{
    [TestClass]
    public sealed class Task1
    {
        record InputRow(string Name, string Surname);
        record OutputRow(string Name, string Surname, int Votes);

        private InputRow[] _input;
        private OutputRow[] _output;
        private Lab7.Blue.Task1.Response[] _student;

        [TestInitialize]
        public void LoadData()
        {
            var folder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
            folder = Path.Combine(folder, "Lab7Test", "Blue");

            var input = JsonSerializer.Deserialize<JsonElement>(
                File.ReadAllText(Path.Combine(folder, "input.json")))!;
            var output = JsonSerializer.Deserialize<JsonElement>(
                File.ReadAllText(Path.Combine(folder, "output.json")))!;

            _input = input.GetProperty("Task1").Deserialize<InputRow[]>()!;
            _output = output.GetProperty("Task1").Deserialize<OutputRow[]>()!;
            _student = new Lab7.Blue.Task1.Response[_input.Length];
        }

        [TestMethod]
        public void Test_00_OOP()
        {
            var type = typeof(Lab7.Blue.Task1.Response);
            Assert.IsTrue(type.IsValueType, "Response должен быть структурой");
			Assert.AreEqual(type.GetFields().Count(f => f.IsPublic), 0);
			Assert.IsTrue(type.GetProperty("Name")?.CanRead ?? false, "Нет свойства Name");
            Assert.IsTrue(type.GetProperty("Surname")?.CanRead ?? false, "Нет свойства Surname");
            Assert.IsTrue(type.GetProperty("Votes")?.CanRead ?? false, "Нет свойства Votes");
            Assert.IsFalse(type.GetProperty("Name")?.CanWrite ?? false, "Свойство Name должно быть только для чтения");
            Assert.IsFalse(type.GetProperty("Surname")?.CanWrite ?? false, "Свойство Surname должно быть только для чтения");
            Assert.IsFalse(type.GetProperty("Votes")?.CanWrite ?? false, "Свойство Votes должно быть только для чтения");
			Assert.IsNotNull(type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(string), typeof(string) }, null), "Нет публичного конструктора Response(string name, string surname)");
			Assert.IsNotNull(type.GetMethod("CountVotes", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(Lab7.Blue.Task1.Response[]) }, null), "Нет публичного метода CountVotes(Response[] responses)");
			Assert.IsNotNull(type.GetMethod("Print", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null), "Нет публичного метода Print()");
			Assert.AreEqual(type.GetProperties().Count(f => f.PropertyType.IsPublic), 3);
			Assert.AreEqual(type.GetConstructors().Count(f => f.IsPublic), 1);
			Assert.AreEqual(type.GetMethods().Count(f => f.IsPublic), 9);
		}

        [TestMethod]
        public void Test_01_Create()
        {
            Init();
            CheckStruct(initial: true);
        }

        [TestMethod]
        public void Test_02_Count()
        {
            Init();
            var votes = new int[_student.Length];
            for (int i = 0; i < _student.Length; i++)
            {
				votes[i] = _student[i].CountVotes(_student);
            }

            for (int i = 0; i < _student.Length; i++)
            {
                Assert.AreEqual(_output[i].Votes, votes[i],
                    $"Количество голосов для {_student[i].Name} {_student[i].Surname} не совпадает");
            }
		}

		[TestMethod]
		public void Test_02_Count_Half()
		{
			Init();
			var votes = new int[_student.Length];
			for (int i = 0; i < _student.Length-1; i++)
			{
				votes[i] = _student[i].CountVotes(_student);
			}

			for (int i = 0; i < _student.Length-1; i++)
			{
				Assert.AreEqual(_output[i].Votes, votes[i],
					$"Количество голосов для {_student[i].Name} {_student[i].Surname} не совпадает");
			}
			for (int i = 0; i < _student.Length; i++)
			{
				Assert.AreEqual(_output[i].Votes, _student[i].Votes,
					$"Количество голосов для {_student[i].Name} {_student[i].Surname} не совпадает");
			}
		}

		private void Init()
        {
            for (int i = 0; i < _input.Length; i++)
            {
                _student[i] = new Lab7.Blue.Task1.Response(_input[i].Name, _input[i].Surname);
            }
        }

        private void CheckStruct(bool initial)
        {
            Assert.AreEqual(_input.Length, _student.Length);

            for (int i = 0; i < _student.Length; i++)
            {
                Assert.AreEqual(_input[i].Name, _student[i].Name);
                Assert.AreEqual(_input[i].Surname, _student[i].Surname);

                if (initial)
                    Assert.AreEqual(0, _student[i].Votes,
                        $"У студента {_student[i].Name} {_student[i].Surname} не должно быть голосов до CountVotes");
            }
        }
    }
}
