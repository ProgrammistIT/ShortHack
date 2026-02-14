namespace Lab7.Blue
{
    public class Task2
    {
        public struct Participant
        {
            private string _name;
            private string _surname;
            private int[,] _marks;
            
            private bool _FirstJump;
            private bool _SecondJump;
            
            public string Name => _name;
            public string Surname => _surname;
            public int[,] Marks
            {
                get
                {
                    int[,] copy = new int[_marks.GetLength(0), _marks.GetLength(1)];
                    Array.Copy(_marks, 0, copy, 0, _marks.Length);
                    return copy;
                }
            }
            public int TotalScore
            {
                get
                {
                    int total = 0;
                    foreach (int value in _marks)
                        total += value;

                    return total;
                }
            }

            public Participant(string name, string surname)
            {
                _name = name;
                _surname = surname;
                _marks = new int[2, 5];
                _FirstJump = false;
                _SecondJump = false;
            }

            public void Jump(int[] result)
            {
                if (result.Length != 5 || result == null || (_FirstJump && _SecondJump))
                    return;
                if (!_FirstJump)
                {
                    for (int j = 0; j < _marks.GetLength(1); j++)
                        _marks[0, j] = result[j];
                    _FirstJump = true;
                }

                else if (!_SecondJump)
                {
                    for (int j = 0; j < _marks.GetLength(1); j++)
                        _marks[1, j] = result[j];
                    _SecondJump = true;
                }
            }

            public static void Sort(Participant[] array)
            {
                for (int i = 0; i < array.Length - 1; i++)
                {
                    for (int j = 0; j < array.Length - 1 - i; j++)
                    {
                        if (array[j].TotalScore < array[j + 1].TotalScore)
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
