using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Lab7Test.Blue
{
   [TestClass]
   public sealed class Task2
   {
       record InputRow(string Name, string Surname, int[][] Marks);
       record OutputRow(string Name, string Surname, int TotalScore);

       private InputRow[] _input;
       private OutputRow[] _output;
       private Lab7.Blue.Task2.Participant[] _student;

       [TestInitialize]
       public void LoadData()
       {
           var folder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
           folder = Path.Combine(folder, "Lab7Test", "Blue");

           var input = JsonSerializer.Deserialize<JsonElement>(
               File.ReadAllText(Path.Combine(folder, "input.json")))!;
           var output = JsonSerializer.Deserialize<JsonElement>(
               File.ReadAllText(Path.Combine(folder, "output.json")))!;

           _input = input.GetProperty("Task2").Deserialize<InputRow[]>()!;
           _output = output.GetProperty("Task2").Deserialize<OutputRow[]>()!;
           _student = new Lab7.Blue.Task2.Participant[_input.Length];
       }

       [TestMethod]
       public void Test_00_OOP()
       {
           var type = typeof(Lab7.Blue.Task2.Participant);
           Assert.IsTrue(type.IsValueType, "Participant должен быть структурой");
			Assert.AreEqual(type.GetFields().Count(f => f.IsPublic), 0);
			Assert.IsTrue(type.GetProperty("Name")?.CanRead ?? false, "Нет свойства Name");
           Assert.IsTrue(type.GetProperty("Surname")?.CanRead ?? false, "Нет свойства Surname");
           Assert.IsTrue(type.GetProperty("Marks")?.CanRead ?? false, "Нет свойства Marks");
           Assert.IsTrue(type.GetProperty("TotalScore")?.CanRead ?? false, "Нет свойства TotalScore");
           Assert.IsFalse(type.GetProperty("Name")?.CanWrite ?? false, "Свойство Name должно быть только для чтения");
           Assert.IsFalse(type.GetProperty("Surname")?.CanWrite ?? false, "Свойство Surname должно быть только для чтения");
           Assert.IsFalse(type.GetProperty("Marks")?.CanWrite ?? false, "Свойство Marks должно быть только для чтения");
           Assert.IsFalse(type.GetProperty("TotalScore")?.CanWrite ?? false, "Свойство TotalScore должно быть только для чтения");
			Assert.IsNotNull(type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(string), typeof(string) }, null), "Нет публичного конструктора Participant(string name, string surname)");
			Assert.IsNotNull(type.GetMethod("Jump", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(int[]) }, null), "Нет публичного метода Jump(int[] result)");
			Assert.IsNotNull(type.GetMethod("Sort", BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(Lab7.Blue.Task2.Participant[]) }, null), "Нет публичного статического метода Sort(Participant[] array)");
			Assert.IsNotNull(type.GetMethod("Print", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null), "Нет публичного метода Print()");
			Assert.AreEqual(type.GetProperties().Count(f => f.PropertyType.IsPublic), 4);
			Assert.AreEqual(type.GetConstructors().Count(f => f.IsPublic), 1);
			Assert.AreEqual(type.GetMethods().Count(f => f.IsPublic), 11);
		}

       [TestMethod]
       public void Test_01_Create()
       {
           Init();
           CheckStruct(jumpsExpected: false);
       }

       [TestMethod]
       public void Test_02_Init()
       {
           Init();
           CheckStruct(jumpsExpected: false);
       }

       [TestMethod]
       public void Test_03_Jumps()
       {
           Init();
           Jump();
           CheckStruct(jumpsExpected: true);
       }

       [TestMethod]
       public void Test_04_Sort()
       {
           Init();
           Jump();

           Lab7.Blue.Task2.Participant.Sort(_student);

           Assert.AreEqual(_output.Length, _student.Length);
           for (int i = 0; i < _student.Length; i++)
           {
               Assert.AreEqual(_output[i].Name, _student[i].Name);
               Assert.AreEqual(_output[i].Surname, _student[i].Surname);
               Assert.AreEqual(_output[i].TotalScore, _student[i].TotalScore);
           }
       }

       [TestMethod]
       public void Test_05_ArrayLinq()
       {
           Init();
           Jump();
           ArrayLinq();
           CheckStruct(jumpsExpected: true);
       }

       private void Init()
       {
           for (int i = 0; i < _input.Length; i++)
           {
               _student[i] = new Lab7.Blue.Task2.Participant(_input[i].Name, _input[i].Surname);
           }
       }

       private void Jump()
       {
           for (int i = 0; i < _input.Length; i++)
           {
               foreach (var jump in _input[i].Marks)
               {
                   _student[i].Jump(jump);
               }
               _student[i].Jump(new int[] { -1, -1, -1, -1, -1 });
           }
       }

       private void ArrayLinq()
       {
           for (int i = 0; i < _student.Length; i++)
           {
               var marks = _student[i].Marks;
               if (marks == null) continue;
               int rows = marks.GetLength(0);
               int cols = marks.GetLength(1);
               for (int r = 0; r < rows; r++)
                   for (int c = 0; c < cols; c++)
                       marks[r, c] = -1;
           }
       }

       private void CheckStruct(bool jumpsExpected)
       {
           Assert.AreEqual(_input.Length, _student.Length);

           for (int i = 0; i < _input.Length; i++)
           {
               Assert.AreEqual(_input[i].Name, _student[i].Name);
               Assert.AreEqual(_input[i].Surname, _student[i].Surname);

               var studentMarks = _student[i].Marks;

               if (jumpsExpected)
               {
                   Assert.IsNotNull(studentMarks, "Marks должны быть инициализированы после Jump");

                   int expectedRows = _input[i].Marks.Length;
                   int expectedCols = _input[i].Marks.Length > 0 ? _input[i].Marks[0].Length : 0;

                   Assert.AreEqual(expectedRows, studentMarks.GetLength(0));
                   Assert.AreEqual(expectedCols, studentMarks.GetLength(1));

                   int sum = 0;
                   for (int r = 0; r < expectedRows; r++)
                   {
                       for (int c = 0; c < expectedCols; c++)
                       {
                           Assert.AreEqual(_input[i].Marks[r][c], studentMarks[r, c]);
                           sum += _input[i].Marks[r][c];
                       }
                   }

                   Assert.AreEqual(sum, _student[i].TotalScore);
               }
               else
               {
                   if (studentMarks == null)
                   {
                       // ок
                   }
                   else
                   {
                       int rows = studentMarks.GetLength(0);
                       int cols = studentMarks.GetLength(1);
                       for (int r = 0; r < rows; r++)
                           for (int c = 0; c < cols; c++)
                               Assert.AreEqual(0, studentMarks[r, c]);
                   }

                   Assert.AreEqual(0, _student[i].TotalScore);
               }
           }
       }
   }
}

