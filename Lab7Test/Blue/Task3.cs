using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Lab7Test.Blue
{
   [TestClass]
   public sealed class Task3
   {
       record InputRow(string Name, string Surname, int[] PenaltyTimes);
       record OutputRow(string Name, string Surname, int TotalTime, bool IsExpelled);

       private InputRow[] _input;
       private OutputRow[] _output;
       private Lab7.Blue.Task3.Participant[] _student;

       [TestInitialize]
       public void LoadData()
       {
           var folder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
           folder = Path.Combine(folder, "Lab7Test", "Blue");

           var input = JsonSerializer.Deserialize<JsonElement>(
               File.ReadAllText(Path.Combine(folder, "input.json")))!;
           var output = JsonSerializer.Deserialize<JsonElement>(
               File.ReadAllText(Path.Combine(folder, "output.json")))!;

           _input = input.GetProperty("Task3").Deserialize<InputRow[]>()!;
           _output = output.GetProperty("Task3").Deserialize<OutputRow[]>()!;
           _student = new Lab7.Blue.Task3.Participant[_input.Length];
       }

       [TestMethod]
       public void Test_00_OOP()
       {
           var type = typeof(Lab7.Blue.Task3.Participant);
           Assert.IsTrue(type.IsValueType, "Participant должен быть структурой");
			Assert.AreEqual(type.GetFields().Count(f => f.IsPublic), 0);
			Assert.IsTrue(type.GetProperty("Name")?.CanRead ?? false, "Нет свойства Name");
           Assert.IsTrue(type.GetProperty("Surname")?.CanRead ?? false, "Нет свойства Surname");
           Assert.IsTrue(type.GetProperty("PenaltyTimes")?.CanRead ?? false, "Нет свойства PenaltyTimes");
           Assert.IsTrue(type.GetProperty("TotalTime")?.CanRead ?? false, "Нет свойства TotalTime");
           Assert.IsTrue(type.GetProperty("IsExpelled")?.CanRead ?? false, "Нет свойства IsExpelled");
           Assert.IsFalse(type.GetProperty("Name")?.CanWrite ?? false, "Свойство Name должно быть только для чтения");
           Assert.IsFalse(type.GetProperty("Surname")?.CanWrite ?? false, "Свойство Surname должно быть только для чтения");
           Assert.IsFalse(type.GetProperty("PenaltyTimes")?.CanWrite ?? false, "Свойство PenaltyTimes должно быть только для чтения");
           Assert.IsFalse(type.GetProperty("TotalTime")?.CanWrite ?? false, "Свойство TotalTime должно быть только для чтения");
           Assert.IsFalse(type.GetProperty("IsExpelled")?.CanWrite ?? false, "Свойство IsExpelled должно быть только для чтения");
			Assert.IsNotNull(type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(string), typeof(string) }, null), "Нет публичного конструктора Participant(string name, string surname)");
			Assert.IsNotNull(type.GetMethod("PlayMatch", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(int) }, null), "Нет публичного метода PlayMatch(int time)");
			Assert.IsNotNull(type.GetMethod("Sort", BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(Lab7.Blue.Task3.Participant[]) }, null), "Нет публичного статического метода Sort(Participant[] array)");
			Assert.IsNotNull(type.GetMethod("Print", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null), "Нет публичного метода Print()");
			Assert.AreEqual(type.GetProperties().Count(f => f.PropertyType.IsPublic), 5);
			Assert.AreEqual(type.GetConstructors().Count(f => f.IsPublic), 1);
			Assert.AreEqual(type.GetMethods().Count(f => f.IsPublic), 12);
		}

       [TestMethod]
       public void Test_01_Create()
       {
           Init();
           CheckStruct(matchesExpected: false);
       }

       [TestMethod]
       public void Test_02_Init()
       {
           Init();
           CheckStruct(matchesExpected: false);
       }

       [TestMethod]
       public void Test_03_PlayMatches()
       {
           Init();
           PlayMatches();
           CheckStruct(matchesExpected: true);
       }

       [TestMethod]
       public void Test_04_Sort()
       {
           Init();
           PlayMatches();

           Lab7.Blue.Task3.Participant.Sort(_student);

           Assert.AreEqual(_output.Length, _student.Length);
           for (int i = 0; i < _student.Length; i++)
           {
               Assert.AreEqual(_output[i].Name, _student[i].Name);
               Assert.AreEqual(_output[i].Surname, _student[i].Surname);
               Assert.AreEqual(_output[i].TotalTime, _student[i].TotalTime);
               Assert.AreEqual(_output[i].IsExpelled, _student[i].IsExpelled);
           }
       }

       [TestMethod]
       public void Test_05_ArrayLinq()
       {
           Init();
           PlayMatches();
           ArrayLinq();
           CheckStruct(matchesExpected: true);
       }

       private void Init()
       {
           for (int i = 0; i < _input.Length; i++)
           {
               _student[i] = new Lab7.Blue.Task3.Participant(_input[i].Name, _input[i].Surname);
           }
       }

       private void PlayMatches()
       {
           for (int i = 0; i < _input.Length; i++)
           {
               foreach (var time in _input[i].PenaltyTimes)
               {
                   _student[i].PlayMatch(time);
               }
               _student[i].PlayMatch(-1);
           }
       }

       private void ArrayLinq()
       {
           for (int i = 0; i < _student.Length; i++)
           {
               var times = _student[i].PenaltyTimes;
               if (times == null) continue;
               for (int j = 0; j < times.Length; j++)
                   times[j] = -1;
           }
       }

       private void CheckStruct(bool matchesExpected)
       {
           Assert.AreEqual(_input.Length, _student.Length);

           for (int i = 0; i < _input.Length; i++)
           {
               Assert.AreEqual(_input[i].Name, _student[i].Name);
               Assert.AreEqual(_input[i].Surname, _student[i].Surname);

               if (matchesExpected)
               {
                   Assert.IsNotNull(_student[i].PenaltyTimes, "PenaltyTimes должны быть инициализированы после PlayMatch");
                   Assert.AreEqual(_input[i].PenaltyTimes.Length, _student[i].PenaltyTimes.Length);

                   int sum = 0;
                   bool expelled = false;
                   for (int j = 0; j < _input[i].PenaltyTimes.Length; j++)
                   {
                       Assert.AreEqual(_input[i].PenaltyTimes[j], _student[i].PenaltyTimes[j]);
                       sum += _input[i].PenaltyTimes[j];
                       if (_input[i].PenaltyTimes[j] == 10) expelled = true;
                   }

                   Assert.AreEqual(sum, _student[i].TotalTime);
                   Assert.AreEqual(expelled, _student[i].IsExpelled);
               }
               else
               {
                   if (_student[i].PenaltyTimes == null)
                   {
                       // ok
                   }
                   else
                   {
                       for (int j = 0; j < _student[i].PenaltyTimes.Length; j++)
                           Assert.AreEqual(0, _student[i].PenaltyTimes[j]);
                   }

                   Assert.AreEqual(0, _student[i].TotalTime);
                   Assert.AreEqual(false, _student[i].IsExpelled);
               }
           }
       }
   }
}

