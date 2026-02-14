using System.Reflection;
using System.Text.Json;

namespace Lab7Test.Blue
{
   [TestClass]
   public sealed class Task5
   {
       record InputSportsman(string Name, string Surname, int Place);
       record InputTeam(string Name, InputSportsman[] Sportsmen);
       record OutputTeam(string Name, int TotalScore, int TopPlace);

       private InputTeam[] _inputTeams;
       private OutputTeam[] _outputTeams;

       private Lab7.Blue.Task5.Sportsman[] _studentS;
       private Lab7.Blue.Task5.Team[] _studentG;

       [TestInitialize]
       public void LoadData()
       {
           var folder = Directory.GetParent(Directory.GetCurrentDirectory())
                                 .Parent.Parent.Parent.FullName;
           folder = Path.Combine(folder, "Lab7Test", "Blue");

           var input = JsonSerializer.Deserialize<JsonElement>(
               File.ReadAllText(Path.Combine(folder, "input.json")))!;
           var output = JsonSerializer.Deserialize<JsonElement>(
               File.ReadAllText(Path.Combine(folder, "output.json")))!;

           _inputTeams = input.GetProperty("Task5").Deserialize<InputTeam[]>();
           _outputTeams = output.GetProperty("Task5").Deserialize<OutputTeam[]>();

           _studentS = _inputTeams.SelectMany(t => t.Sportsmen)
                                  .Select(s => new Lab7.Blue.Task5.Sportsman(s.Name, s.Surname))
                                  .ToArray();
       }

       [TestMethod]
       public void Test_00_OOP()
       {
           var type = typeof(Lab7.Blue.Task5.Sportsman);
           Assert.IsTrue(type.IsValueType, "Sportsman должен быть структурой");
			Assert.AreEqual(type.GetFields().Count(f => f.IsPublic), 0);
			Assert.IsTrue(type.GetProperty("Name")?.CanRead ?? false, "Нет свойства Name");
           Assert.IsTrue(type.GetProperty("Surname")?.CanRead ?? false, "Нет свойства Surname");
           Assert.IsTrue(type.GetProperty("Place")?.CanRead ?? false, "Нет свойства Place");
           Assert.IsFalse(type.GetProperty("Name")?.CanWrite ?? false, "Свойство Name должно быть только для чтения");
           Assert.IsFalse(type.GetProperty("Surname")?.CanWrite ?? false, "Свойство Surname должно быть только для чтения");
           Assert.IsFalse(type.GetProperty("Place")?.CanWrite ?? false, "Свойство Place должно быть только для чтения");
			Assert.IsNotNull(type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(string), typeof(string) }, null), "Нет публичного конструктора Sportsman(string name, string surname)");
			Assert.IsNotNull(type.GetMethod("SetPlace", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(int) }, null), "Нет публичного метода SetPlace(int place)");
			Assert.IsNotNull(type.GetMethod("Print", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null), "Нет публичного метода Print()");
			Assert.AreEqual(type.GetProperties().Count(f => f.PropertyType.IsPublic), 3);
			Assert.AreEqual(type.GetConstructors().Count(f => f.IsPublic), 1);
			Assert.AreEqual(type.GetMethods().Count(f => f.IsPublic), 9);

			type = typeof(Lab7.Blue.Task5.Team);
           Assert.IsTrue(type.IsValueType, "Team должен быть структурой");
			Assert.AreEqual(type.GetFields().Count(f => f.IsPublic), 0);
			Assert.IsTrue(type.GetProperty("Name")?.CanRead ?? false, "Нет свойства Name");
           Assert.IsTrue(type.GetProperty("Sportsmen")?.CanRead ?? false, "Нет свойства Sportsmen");
           Assert.IsTrue(type.GetProperty("TotalScore")?.CanRead ?? false, "Нет свойства TotalScore");
           Assert.IsTrue(type.GetProperty("TopPlace")?.CanRead ?? false, "Нет свойства TopPlace");
			Assert.IsNotNull(type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(string) }, null), "Нет публичного конструктора Team(string name)");
			Assert.IsNotNull(type.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(Lab7.Blue.Task5.Sportsman) }, null), "Нет публичного метода Add(Sportsman sportsman)");
			Assert.IsNotNull(type.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(Lab7.Blue.Task5.Sportsman[]) }, null), "Нет публичного метода Add(Sportsman[] sportsmen)");
			Assert.IsNotNull(type.GetMethod("Sort", BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(Lab7.Blue.Task5.Team[]) }, null), "Нет публичного статического метода Sort(Team[] array)");
			Assert.IsNotNull(type.GetMethod("Print", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null), "Нет публичного метода Print()");
			Assert.AreEqual(type.GetProperties().Count(f => f.PropertyType.IsPublic), 4);
			Assert.AreEqual(type.GetConstructors().Count(f => f.IsPublic), 1);
			Assert.AreEqual(type.GetMethods().Count(f => f.IsPublic), 12);
		}

       [TestMethod]
       public void Test_01_CreateS()
       {
           Assert.AreEqual(_studentS.Length, _inputTeams.Sum(t => t.Sportsmen.Length));
       }

       [TestMethod]
       public void Test_02_InitS()
       {
           CheckSportsmen(placeExpected: false);
       }

       [TestMethod]
       public void Test_03_SetPlaces()
       {
           RunPlaces();
           CheckSportsmen(placeExpected: true);
       }

       [TestMethod]
       public void Test_04_ArrayLinq()
       {
           RunPlaces();
           ArrayLinq();
           CheckSportsmen(placeExpected: true);
       }

       [TestMethod]
       public void Test_05_CreateG()
       {
           InitTeams();
           CheckTeams(filled: false);
       }

       [TestMethod]
       public void Test_06_FillG()
       {
           InitTeams();
           FillTeams();
           CheckTeams(filled: true);
       }

       [TestMethod]
       public void Test_07_FillManyTeams()
       {
           InitTeams();
           FillManyTeams();
           CheckTeams(filled: true);
       }

       [TestMethod]
       public void Test_08_ArrayLinqG()
       {
           InitTeams();
           FillTeams();
           ArrayLinqTeams();
           CheckTeams(filled: true);
       }

       [TestMethod]
       public void Test_09_Sort()
       {
           RunPlaces();
           InitTeams();
           FillTeams();
           Lab7.Blue.Task5.Team.Sort(_studentG);
           CheckTeamsSorted();
       }

       private void RunPlaces()
       {
           int idx = 0;
           foreach (var t in _inputTeams)
           {
               foreach (var s in t.Sportsmen)
               {
                   _studentS[idx].SetPlace(s.Place);
                   idx++;
               }
           }
       }

       private void ArrayLinq()
       {
           foreach (var s in _studentS)
           {
               s.SetPlace(-1);
           }
       }

       private void InitTeams()
       {
           _studentG = _inputTeams.Select(t => new Lab7.Blue.Task5.Team(t.Name)).ToArray();
       }

       private void FillTeams()
       {
           int idx = 0;
           for (int i = 0; i < _studentG.Length; i++)
           {
               var sportsmen = _inputTeams[i].Sportsmen;
               foreach (var s in sportsmen)
               {
                   _studentG[i].Add(_studentS[idx]);
                   idx++;
               }
           }
       }

       private void FillManyTeams()
       {
           int idx = 0;
           for (int i = 0; i < _studentG.Length; i++)
           {
               var sportsmenToAdd = _studentS.Skip(idx).Take(_inputTeams[i].Sportsmen.Length).ToArray();
               _studentG[i].Add(sportsmenToAdd);
               idx += sportsmenToAdd.Length;
           }
       }
       private void ArrayLinqTeams()
       {
           for (int ti = 0; ti < _studentG.Length; ti++)
           {
               var t = _studentG[ti];
               var sp = t.Sportsmen.ToArray();
               for (int i = 0; i < sp.Length - 1; i++)
                   sp[i] = sp[i + 1];
               for (int i = 0; i < sp.Length; i++)
                   t.Add(sp[i]);
               _studentG[ti] = t;
           }
       }


       private void CheckSportsmen(bool placeExpected)
       {
           int idx = 0;
           foreach (var t in _inputTeams)
           {
               foreach (var s in t.Sportsmen)
               {
                   var sp = _studentS[idx];
                   Assert.AreEqual(s.Name, sp.Name);
                   Assert.AreEqual(s.Surname, sp.Surname);
                   if (placeExpected)
                       Assert.AreEqual(s.Place, sp.Place);
                   else
                       Assert.AreEqual(0, sp.Place);
                   idx++;
               }
           }
       }

       private void CheckTeams(bool filled)
       {
           for (int i = 0; i < _studentG.Length; i++)
           {
               var t = _studentG[i];
               Assert.AreEqual(_inputTeams[i].Name, t.Name);
               if (filled)
               {
                   Assert.AreEqual(_inputTeams[i].Sportsmen.Length, t.Sportsmen.Length);
                   for (int j = 0; j < t.Sportsmen.Length; j++)
                   {
                       var spExpected = _studentS.Skip(_inputTeams.Take(i).Sum(x => x.Sportsmen.Length)).Skip(j).First();
                       Assert.AreEqual(spExpected.Name, t.Sportsmen[j].Name);
                       Assert.AreEqual(spExpected.Surname, t.Sportsmen[j].Surname);
                       Assert.AreEqual(spExpected.Place, t.Sportsmen[j].Place);
                   }
               }
           }
       }

       private void CheckTeamsSorted()
       {
           for (int i = 0; i < _studentG.Length; i++)
           {
               Assert.AreEqual(_outputTeams[i].Name, _studentG[i].Name);
               Assert.AreEqual(_outputTeams[i].TotalScore, _studentG[i].TotalScore);
               Assert.AreEqual(_outputTeams[i].TopPlace, _studentG[i].TopPlace);
           }
       }
   }
}

