using Elephanet.Tests;
using Ploeh.AutoFixture;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Diagnostics;

namespace Elephanet.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            const int records = 10000;
            var watch = new Stopwatch();
            Console.WriteLine("Creating {0} records", records);
            DocumentStore store = new DocumentStore("Server=127.0.0.1;Port=5432;User id=store_user;password=my super secret password;database=store;");
            var cars = GenerateCars(records);
            Console.WriteLine("Generated {0} records", records);
            Console.WriteLine("Saving {0} records to database", records);

            watch.Start();
            using (var session = store.OpenSession())
            {
                foreach(var car in cars)
                {
                    session.Store(car);
                }
                session.SaveChanges();
            }
            watch.Stop();
            var rate = records / watch.Elapsed.TotalSeconds;

            Console.WriteLine("Saved {0} records to datatbase in {1}s @ {2} p/s rate", records, watch.Elapsed.TotalSeconds, rate);
            Console.WriteLine("Press any key to do query test....");
            Console.ReadLine();

            List<Car> fords;
            using (var session = store.OpenSession())
            {
                watch.Reset();
                watch.Start();
                fords = session.Query<Car>().Where(c => c.Make == "Ford").ToList();
                watch.Stop();
            }
            foreach (var ford in fords)
            {
                Console.WriteLine("Id: {0}, Make: {1}", ford.Id, ford.Make);
            }
            Console.WriteLine("Query took {0}ms", watch.Elapsed.TotalMilliseconds);

            List<Car> holdens;
            using (var session = store.OpenSession())
            {
                watch.Reset();
                watch.Start();
                holdens = session.Query<Car>().Where(c => c.Make == "Holden").ToList();
                watch.Stop();
            }
            foreach (var holden in holdens)
            {
                Console.WriteLine("Id: {0}, Make: {1}", holden.Id, holden.Make);
            }
            Console.WriteLine("Query took {0}ms", watch.Elapsed.TotalMilliseconds);

            //cleanup
            using (var session = store.OpenSession())
            {
                session.DeleteAll<Car>();
            }
            Console.ReadLine();
        }

        public static List<Car> GenerateCars(int numberOfCars)
        {
            var dummyCars = new Fixture().Build<Car>()
            .With(x => x.Make, "Subaru")
            .CreateMany(numberOfCars).ToList();
            //spice it up
            dummyCars[100].Make = "Ford";
            dummyCars[1000].Make = "Ford";
            dummyCars[2948].Make = "Ford";
            dummyCars[256].Make = "Holden";
            dummyCars[269].Make = "Holden";
            dummyCars[452].Make = "Holden";

            return dummyCars.ToList();
        }
    }
}
