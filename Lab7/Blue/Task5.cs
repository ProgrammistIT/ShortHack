namespace Lab7.Blue
{
    public class Task5
    {
        public struct Sportsman
        {
            private string _name;
            private string _surname;
            private int _place;
            private bool _isSetted;
            
            private const int LastPlace = 18;
            
            public string Name => _name;
            public string Surname => _surname;
            public int Place => _place;

            public Sportsman(string name, string surname)
            {
                _name = name;
                _surname = surname;
                _place = 0;
                _isSetted = false;
            }

            public void SetPlace(int place)
            {
                if (!_isSetted && place is > 0 and <= LastPlace)
                {
                    _place = place;
                    _isSetted = true;
                }
            }

            public void Print()
            {
                return;
            }
        }

        public struct Team
        {
            private string _name;
            private Sportsman[] _sportsmen;
            
            private const int CountOfSportsmenInTeam = 6;
            
            public string Name => _name;
            public Sportsman[] Sportsmen
            {
                get
                {
                    Sportsman[] copy = new Sportsman[_sportsmen.Length];
                    Array.Copy(_sportsmen, copy, _sportsmen.Length);
                    return copy;
                }
            }
            public int TotalScore
            {
                get
                {
                    int sum = 0;
                    foreach (var sportsman in _sportsmen)
                    {
                        if (sportsman.Place != 0)
                            sum += GetPoints(sportsman);
                    }

                    return sum;
                }
            }
            public int TopPlace
            {
                get
                {
                    int topPlace = Int32.MaxValue;

                    foreach (var sportsman in _sportsmen)
                    {
                        if (sportsman.Place != 0 && sportsman.Place < topPlace)
                            topPlace = sportsman.Place;
                    }
                    
                    return topPlace;
                }
            }
            private int GetPoints(Sportsman sportsman)
            {
                switch (sportsman.Place)
                {
                    case 1: return 5;
                    case 2: return 4;
                    case 3: return 3;
                    case 4: return 2;
                    case 5: return 1;
                    default: return 0;
                }
            }
            public Team(string name)
            {
                _name = name;
                _sportsmen = [];
            }

            public void Add(Sportsman sportsman)
            {
                if (_sportsmen.Length < CountOfSportsmenInTeam)
                {
                    Array.Resize(ref _sportsmen, _sportsmen.Length + 1);
                    _sportsmen[^1] = sportsman;
                }
            }
            public void Add(Sportsman[] sportsmen)
            {
                foreach (var sportsman in sportsmen)
                {
                    if (_sportsmen.Length < CountOfSportsmenInTeam)
                    {
                        Array.Resize(ref _sportsmen, _sportsmen.Length + 1);
                        _sportsmen[^1] = sportsman;
                    }
                    else
                        return;
                }
            }
            public static void Sort(Team[] teams)
            {
                for (int i = 0; i < teams.Length - 1; i++)
                {
                    for (int j = 0; j < teams.Length - 1 - i; j++)
                    {
                        if (teams[j].TotalScore < teams[j + 1].TotalScore)
                            (teams[j], teams[j + 1]) = (teams[j + 1], teams[j]);
                        
                        // в случае равенства проверить на наличие спортсмена, занявшего первое место
                        if (teams[j].TotalScore == teams[j + 1].TotalScore && teams[j + 1].HasFirstPlace())
                            (teams[j], teams[j + 1]) = (teams[j + 1], teams[j]);
                    }
                }
            }

            private bool HasFirstPlace()
            {
                bool hasFirstPlace = true;
                foreach (var sportsman in _sportsmen)
                {
                    if (sportsman.Place == 1)
                        return hasFirstPlace;
                }
                return !hasFirstPlace;
            }
            
            public void Print()
            {
                return;
            }  
        }
    }
}
    
    
