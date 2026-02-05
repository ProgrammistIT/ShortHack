using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab7Test
{
    internal class InputData
    {
        private string[] _namesMale = new string[]
        {
            "Иван", "Степан", "Игорь", "Николай", "Александр",
            "Дмитрий", "Григорий", "Лев", "Мирослав", "Савелий",
            "Ярослав", "Михаил", "Марк", "Сергей", "Артем",
            "Артемий", "Никита", "Юрий", "Максим", "Виктор"
        };
        private string[] _namesFemale = new string[]
        {
            "Мария", "Анастасия", "Светлана", "Оксана", "Ольга",
            "Юлия", "Дарья", "Екатерина", "Полина", "Евгения",
            "Марина", "Александра", "Ирина", "Алиса", "Влада",
            "Татьяна", "Инна", "Инесса", "Кристина", "Алевтина"
        };
        private string[] _surnamesFemale = new string[]
        {
            "Иванова", "Петрова", "Козлова", "Сидорова", "Павлова",
            "Жаркова", "Луговая", "Полевая", "Распутина", "Свиридова",
            "Тихонова", "Зайцева", "Лисицына", "Пономарева", "Кристиан",
            "Батова", "Ушакова", "Степанова", "Смирнова", "Чехова"
        };
        private string[] _surnamesMale = new string[]
        {
            "Иванов", "Петров", "Козлов", "Сидоров", "Павлов",
            "Жарков", "Луговой", "Полевой", "Распутин", "Свиридов",
            "Тихонов", "Зайцев", "Лисицын", "Пономарев", "Кристиан",
            "Батов", "Ушаков", "Степанов", "Смирнов", "Чехов"
        };
        private string[] _groups = new string[]
        {
            "ББИ", "БИВТ", "БПМ", "БНТМ", "БЭК", "БНМТ", "БГД"
        };
        private string[] _clubs = new string[]
        {
            "Динамо", "ЦСКА", "Спартак", "Локомотив", "Звери",
            "Метеор", "Василек", "Быки", "СКА", "Рубин",
            "Зенит", "Анжи", "Юность", "Химик", "Металлург",
            "Нефтехим", "Барс", "Ак-барс", "Энергия", "Югра",
            "Урал", "Байкал", "Гранит", "СПБ", "Львы",
            "Быки", "Барс", "Магнит", "Русь", "Юникс"
        };
        private string[] _animals = new string[]
        {
            "Коала", "Панда", "Макака", "Тануки", "Серау", "Кошка", "Сима энага", null
        };
        private string[] _characterTrait = new string[]
        {
            "Амбициозность", "Внимательность", "Дружелюбность", "Скромность", "Проницательность", "Целеустремленность", "Уважительность", null
        };
        private string[] _concept = new string[]
        {
            "Сакура", "Кимоно", "Суши", "Аниме", "Манга", "Фудзияма", "Самурай", null
        };
        Random _random = new Random();

        private (string, string) InitNameSurname()
        {
            string name = _namesMale[_random.Next(0, _namesMale.Length)];
            string surname = _surnamesMale[_random.Next(0, _surnamesMale.Length)];
            if (_random.Next(0, 10) >= 5)
            {
                name = _namesFemale[_random.Next(0, _namesFemale.Length)];
                surname = _surnamesFemale[_random.Next(0, _surnamesFemale.Length)];
            }
            return (name, surname);
        }
        public (string, string, double, double) Init_White_1()
        {
            string surname = _surnamesFemale[_random.Next(0, _surnamesFemale.Length)];
            string club = _clubs[_random.Next(0, _clubs.Length)];
            double res1 = _random.NextDouble() * 10;
            double res2 = _random.NextDouble() * 10;
            return (surname, club, res1, res2);
        }
        public (string, string, string, double) Init_Green_1()
        {
            string surname = _surnamesFemale[_random.Next(0, _surnamesFemale.Length)];
            string group = _clubs[_random.Next(0, _clubs.Length)];
            string trainer = _surnamesMale[_random.Next(0, _surnamesMale.Length)];
            double res = _random.NextDouble() * 150;
            return (surname, group, trainer, res);
        }
        public (string, string) Init_White_2()
        {
            return (InitNameSurname());
        }
        public (string, string, double, double) Init_White_3()
        {
            var name = InitNameSurname();
            double res1 = _random.NextDouble() * 3 + 1;
            double res2 = _random.NextDouble() * 3 + 1;
            return (name.Item1, name.Item2, res1, res2);
        }
        public (string, string, int[]) Init_White_4(int amount)
        {
            var name = InitNameSurname();
            int[] marks = new int[amount];
            for (int i = 0; i < amount; i++)
            {
                marks[i] = _random.Next(0, 6);
                if (marks[i] == 1)
                    marks[i] = _random.Next(3, 6);
            }
            return (name.Item1, name.Item2, marks);
        }
        public (string, string, int[]) Init_Green_2(int amount)
        {
            var name = InitNameSurname();
            int[] marks = new int[amount];
            for (int i = 0; i < amount; i++)
            {
                marks[i] = _random.Next(2, 6);
                if (marks[i] == 2)
                    marks[i] += _random.Next(0, 3);
            }
            return (name.Item1, name.Item2, marks);
        }
        public (string, string, int[]) Init_Green_3(int amount)
        {
            var name = InitNameSurname();
            int[] marks = new int[amount];
            for (int i = 0; i < amount; i++)
            {
                marks[i] = _random.Next(2, 6);
                if (marks[i] == 2)
                    marks[i] += _random.Next(0, 3);
            }
            return (name.Item1, name.Item2, marks);
        }
        public (string, string, double[]) Init_Green_4(int amount)
        {
            var name = InitNameSurname();
            double[] jumps = new double[amount];
            for (int i = 0; i < amount; i++)
            {
                jumps[i] = _random.NextDouble() * 10;
            }
            return (name.Item1, name.Item2, jumps);
        }
        public (string, string, double[], int[]) Init_Purple_1()
        {
            var name = InitNameSurname();
            double[] coefs = new double[4];
            for (int i = 0; i < coefs.Length; i++)
            {
                coefs[i] = Math.Round(_random.NextDouble() + 2.5, 2);
            }
            int[] marks = new int[7];
            for (int i = 0; i < marks.Length; i++)
            {
                marks[i] = _random.Next(1, 7);
            }
            return (name.Item1, name.Item2, coefs, marks);
        }
        public (string, string, int, int[]) Init_Purple_2()
        {
            var name = InitNameSurname();
            int distance = _random.Next(100, 200);
            int[] marks = new int[5];
            for (int i = 0; i < 5; i++)
            {
                marks[i] = _random.Next(1, 21);
            }
            return (name.Item1, name.Item2, distance, marks);
        }
        public (string, string, int[], int[]) Init_Green_5(int amount)
        {
            var name = InitNameSurname();
            int[] marks = new int[amount];
            for (int i = 0; i < amount; i++)
            {
                marks[i] = _random.Next(0, 21);
            }
            int[] marks2 = new int[amount];
            for (int i = 0; i < amount; i++)
            {
                marks2[i] = _random.Next(0, 21);
            }
            return (name.Item1, name.Item2, marks, marks2);
        }
        public (string, string, double[]) Init_White_5(int amount)
        {
            var name = InitNameSurname();
            double[] games = new double[amount];
            for (int i = 0; i < amount; i++)
            {
                games[i] = _random.Next(0, 3) / 2.0;
            }
            return (name.Item1, name.Item2, games);
        }
        public (string, string, int[]) Init_Blue_1(int amount)
        {
            var name = InitNameSurname();
            int[] times = new int[] { 0, 2, 5, 10 };
            int[] penalties = new int[amount];
            for (int i = 0; i < amount; i++)
            {
                penalties[i] = times[_random.Next(0, 4)];
            }
            return (name.Item1, name.Item2, penalties);
        }
        public (string, string, double[]) Init_Purple_3(int amount)
        {
            var name = InitNameSurname();
            double[] marks = new double[amount];
            for (int i = 0; i < amount; i++)
            {
                marks[i] = _random.NextDouble() * 6;
            }
            return (name.Item1, name.Item2, marks);
        }
        public (string, string, string, int[]) Init_Blue_2(int amount)
        {
            string group = _groups[_random.Next(0, _groups.Length)];
            var name = InitNameSurname();
            int[] marks = new int[amount];
            for (int i = 0; i < amount; i++)
            {
                marks[i] = _random.Next(2, 6);
                if (marks[i] == 2)
                    marks[i] += _random.Next(0, 3);
            }
            return (group, name.Item1, name.Item2, marks);
        }
        public (string, string, int[]) Init_Blue_3(int amount)
        {
            string group = _groups[_random.Next(0, _groups.Length)];
            string team = _clubs[_random.Next(0, _clubs.Length)];
            int[] scores = new int[] { 0, 1, 3 };
            int[] matches = new int[amount];
            for (int i = 0; i < amount; i++)
            {
                matches[i] = scores[_random.Next(0, 3)];
                if (matches[i] == 2) matches[i]++;
            }
            return (group, team, matches);
        }
        public (string, string, string, int) Init_Blue_4(int place)
        {
            var name = InitNameSurname();
            string team = _clubs[_random.Next(0, _clubs.Length)];
            switch (place)
            {
                case 1: place = 17; break;
                case 2: place = 12; break;
                case 4: place = 11; break;
                case 5: place = 13; break;
                case 17: place = 1; break;
                case 12: place = 2; break;
                case 11: place = 4; break;
                case 13: place = 5; break;
            }
            return (team, name.Item1, name.Item2, place);
        }
        public (string, string, double) Init_Purple_4()
        {
            var name = InitNameSurname();
            double time = _random.NextDouble() * 500;
            return (name.Item1, name.Item2, time);
        }
        public (string, int, int) Init_Blue_5()
        {
            string team = _clubs[_random.Next(0, _clubs.Length)];
            int goals = _random.Next(0, 6);
            int misses = _random.Next(0, 5);
            return (team, goals, misses);
        }
        public (string, string, string) Init_Purple_5()
        {
            string a = _animals[_random.Next(0, 8)];
            string ct = _characterTrait[_random.Next(0, 8)];
            string c = _concept[_random.Next(0, 8)];
            return (a, ct, c);
        }
    }
}
