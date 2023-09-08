using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VirusSpread.BussnessLogic
{
    public class VirusSpread
    {
        public static ConcurrentBag<Model.PeopleModel.People> peoples;

        public static void Init()
        {
            peoples = new ConcurrentBag<Model.PeopleModel.People>();
            Random random = new Random(DateTime.Now.Millisecond * DateTime.Now.Second);
            for (int i = 0; i < 100; i++)
            {
                Model.PeopleModel.People people = new Model.PeopleModel.People();
                people.ID = i;
                people.X = random.NextDouble() * 400;
                people.Y = random.NextDouble() * 400;
                people.Disease = false;
                peoples.Add(people);
            }
            for (int i = 100; i < 103; i++)
            {
                Model.PeopleModel.People people = new Model.PeopleModel.People();
                people.ID = i;
                people.X = random.NextDouble() * 400;
                people.Y = random.NextDouble() * 400;
                people.Disease = true;
                peoples.Add(people);
            }
        }
        public static void Run(int spreadCount,int moveDistance)
        {
            int count = 0;
            Random random = new Random(DateTime.Now.Millisecond * DateTime.Now.Second);
            while (true)
            {
                if (count > spreadCount)
                {
                    break;
                }
                foreach (var item in peoples)
                {
                    int parity = random.Next(0,200);
                    double tempX, tempY;
                    if (parity % 2 == 0)
                    {
                        tempX = item.X + random.NextDouble() * moveDistance;
                        tempY = item.Y + random.NextDouble() * moveDistance;
                    }
                    else
                    {
                        tempX = item.X - random.NextDouble() * moveDistance;
                        tempY = item.Y - random.NextDouble() * moveDistance;
                    }
                    
                    if (tempX>0 && tempX < 400 && tempY>0 && tempY < 400)
                    {
                        item.X = tempX;
                        item.Y = tempY;
                    }
                    
                }
                Thread.Sleep(1000);
                count++;
            }
        }
        public static void Spread(int spreadCount,int infectious)
        {
            int count = 0;
            while (true)
            {
                if (count > spreadCount)
                {
                    break;
                }
                var healthy = peoples.Where(item => item.Disease == false).ToList();
                var infected = peoples.Where(item => item.Disease == true).ToList();


                var pairsJoin = (from healthyItem in healthy
                                 from infectedItem in infected
                                 select new { healthyItem, infectedItem }).ToList();

                foreach (var item in pairsJoin)
                {
                    CalcDistance(item.healthyItem, item.infectedItem, out double distance);
                    if (distance < infectious)
                    {
                        var infectedLabel = peoples.Where(find => find.ID == item.healthyItem.ID).FirstOrDefault();
                        infectedLabel.Disease = true;
                    }
                }
                Thread.Sleep(1000);
                count++;
            }
        }
        public static void Display(out string statistics)
        {
            statistics = peoples.Where(item => item.Disease == true).Count().ToString();
        }
        public static void CalcDistance(Model.PeopleModel.People A, Model.PeopleModel.People B, out double distance)
        {
            double X = Math.Abs(A.X - B.X);
            double Y = Math.Abs(A.Y - B.Y);
            distance = Math.Sqrt(X * X - Y * Y);
        }
    }
}
