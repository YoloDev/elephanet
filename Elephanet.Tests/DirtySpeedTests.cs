using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ploeh.AutoFixture;
using System.Diagnostics;
using Xunit;

namespace Elephanet.Tests
{
    public class DirtySpeedTests : IDisposable
    {
        DocumentStore _store;
        const int writeCount = 50000;
        Stopwatch _watch;

        public DirtySpeedTests()
        {
            _store = new DocumentStore("Server=127.0.0.1;Port=5432;User id=store_user;password=my super secret password;database=store;");
            _watch = new Stopwatch();
        }


        public List<Car> GenerateCars(int numberOfCars)
        {
            var dummyCars = new Fixture().Build<Car>()
            .With(x => x.Make, "Subaru")
            .CreateMany(numberOfCars);
            return dummyCars.ToList();
        }

        [Fact]
        public void InsertSpeedTest()
        {
            var cars = GenerateCars(writeCount);
            _watch.Reset();
            _watch.Start();
            using (var session = _store.OpenSession())
            {
                foreach (Car car in cars)
                {
                    session.Store(car);
                }

                session.SaveChanges();
            }
            _watch.Stop();
            var rate = writeCount / _watch.Elapsed.TotalSeconds;
            Console.WriteLine(string.Format("{0} writes p/s",rate));
            Console.WriteLine(string.Format("{0} writes in {1} seconds",writeCount, _watch.Elapsed.TotalSeconds));
        }

        [Fact]
        public void LinqWhereQuerySpeedTest()
        {
            int seedNumber = 20000; //that should be enough to see if we are hitting the index nicely
            var cars = GenerateCars(seedNumber);

            //replace with a few other makes in there
            cars[1000].Make = "Ford";
            cars[1201].Make = "Ford";
            cars[2005].Make = "Ford";

            //save to the database
            using (var session = _store.OpenSession())
            {
                foreach (Car car in cars)
                {
                    session.Store(car);
                }

                session.SaveChanges();
            }

            using (var session = _store.OpenSession())
            {
                _watch.Start();
                var query = session.Query<Car>().Where(c => c.Make == "Ford");
                //force the query
                var fords  = query.ToList();
                _watch.Stop();
            }

            Console.WriteLine(string.Format("{0} ms for reading 3 records from {1} total (initial query)", _watch.Elapsed.TotalMilliseconds, seedNumber));

        }

        public void Dispose()
        {
            using (var session = _store.OpenSession())
            {
                session.DeleteAll<Car>();
            }
        }
    }
}
