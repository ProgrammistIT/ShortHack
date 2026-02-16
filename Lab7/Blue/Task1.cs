namespace Lab7.Blue
{
    public class Task1
    {
        public struct Response
        {
            private int _votes;
            private string _name;
            private string _surname;
            
            public int Votes => _votes;
            public string Surname => _surname;
            public string Name => _name;
            
            private int VotesSetter
            {
                set { _votes = value; }
            }

            public Response(string name, string surname)
            {
                _name = name;
                _surname = surname;
                _votes = 0;
            }
            
            public int CountVotes(Response[] responses)
            {
                if (responses.Length == 0 || responses == null) return 0;
                int count = 0;

                foreach (var response in responses)
                {
                    if (response.Name == _name && response.Surname == _surname)
                        count++;
                }
                
                for (int i = 0; i < responses.Length; i++)
                {
                    if (responses[i].Name == _name && responses[i].Surname == _surname)
                        responses[i].VotesSetter = count;
                }
                
                return count;
            }
            
            public void Print()
            {
                return;
            }
        }
    }
}
