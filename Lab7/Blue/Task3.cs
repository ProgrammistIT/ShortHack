namespace Lab7.Blue
{
    public class Task3
    {
        public struct Participant
        {
            private string _name;
            private string _surname;
            private int[] _penaltyTimes;
            
            public string Name => _name;
            public string Surname => _surname;
            public int[] PenaltyTimes
            {
                get
                {
                    int[] copy = new int[_penaltyTimes.Length];
                    Array.Copy(_penaltyTimes, 0, copy, 0, copy.Length);
                    return copy;
                }
            }
            public int TotalTime => _penaltyTimes.Sum();

            public Participant(string name, string surname)
            {
                _name = name;
                _surname = surname;
                _penaltyTimes = [];
            }

            public bool IsExpelled
            {
                get {
                    bool isExpelled = true;
                    if (_penaltyTimes == null || _penaltyTimes.Length == 0)
                        return !isExpelled;

                    foreach (var time in _penaltyTimes)
                    {
                        if (time >= 10)
                            return isExpelled;
                    }

                    return !isExpelled;
                }
            }

            public void PlayMatch(int time)
            {
                if (time < 0)
                    return;
                
                Array.Resize(ref _penaltyTimes, _penaltyTimes.Length + 1);
                _penaltyTimes[^1] = time;
            }

            public static void Sort(Participant[] array)
            {
                for (int i = 0; i < array.Length - 1; i++)
                {
                    for (int j = 0; j < array.Length - 1 - i; j++)
                    {
                        if (array[j].TotalTime > array[j + 1].TotalTime)
                            (array[j], array[j + 1]) = (array[j + 1], array[j]);
                    }
                }
            }

            public void Print()
            {
                return;
            }
        }
    }
}
