using System.Collections.Immutable;

namespace Lab7.Blue
{
    public class Task4
    {
        public struct Team
        {
            private string _name;
            private int[] _scores;

            public int TotalScore => _scores.Sum();
            public string Name => _name;

            public int[] Scores
            {
                get
                {
                    int[] copy = new int[_scores.Length];
                    Array.Copy(_scores, copy, _scores.Length);
                    return copy;
                }
            }

            public Team(string name)
            {
                _name = name;
                _scores = [];
            }

            // добавление очков в команду
            public void PlayMatch(int result)
            {
                Array.Resize(ref _scores, _scores.Length + 1);
                _scores[^1] = result;
            }

            public void Print()
            {
                return;
            }
        }

        public struct Group
        {
            private string _name;
            private Team[] _teams;
            private int _count;
            
            public string Name => _name;

            public Team[] Teams
            {
                get
                {
                    Team[] copy = new Team[_teams.Length];
                    Array.Copy(_teams, copy, _teams.Length);
                    return copy;
                }
            }
            
            public Group(string name)
            {
                _name = name;
                _teams = new Team[12];
                _count = 0;
            }

            // добавление одной команды
            public void Add(Team team)
            {
                if (_count >= 12)
                    return;
                else
                {
                    _teams[_count] = team;
                    _count++;
                }
            }

            // добавление нескольких команд
            public void Add(Team[] teams)
            {
                    foreach (var team in teams)
                    {
                        if (_count < 12)
                        {
                            _teams[_count] = team;
                            _count++;
                        }
                        else
                        {
                            return;
                        }
                    }
            }
            
            // сортировка по убыванию количества очков
            public void Sort()
            {
                for (int i = 0; i < _teams.Length - 1; i++)
                {
                    for (int j = 0; j < _teams.Length - 1 - i; j++)
                    {
                        if (_teams[j].TotalScore < _teams[j + 1].TotalScore)
                            (_teams[j], _teams[j + 1]) = (_teams[j + 1], _teams[j]);
                    }
                }
            }
            
            public static Group Merge(Group group1, Group group2, int size)
            {
                Group result = new Group("Финалисты");
                
                group1.Sort();
                group2.Sort();
                
                int countTeam = size / 2;

                for (int i = 0; i < countTeam; i++)
                {
                    result.Add(group1._teams[i]);
                    result.Add(group2._teams[i]);
                }
                result.Sort();
                return result;
            }
            
            public void Print()
            {
                return;
            }
        }
    }
}
