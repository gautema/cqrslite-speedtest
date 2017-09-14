using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CQRSlite.Caching;
using CQRSlite.Commands;
using CQRSlite.Domain;
using CQRSlite.Events;
using CQRSlite.Routing;

namespace CQRSlite_speedtest
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().DoStuff().Wait();
        }

        public async Task DoStuff()
        {
            var watch = new Stopwatch();
            watch.Start();
            var router = new Router();
            var eventstore = new EventStore(router);
            IRepository rep = new Repository(eventstore);
            var cache = new MemoryCache();
            //rep = new CacheRepository(rep, eventstore, cache);
            var session = new Session(rep);

            for (var index = 0; index < 15_000; index++)
            {
                await session.Add(new Employee(Guid.NewGuid()));
            }
            await session.Commit();

            for (var i = 0; i < 15_000; i++)
            {
                await session.Add(new Employee(Guid.NewGuid()));
                await session.Commit();
            }

            for (var i = 0; i < 15_000; i++)
            {
                var rep2 = new Repository(eventstore);
                var session2 = new Session(rep2);
                await session2.Add(new Employee(Guid.NewGuid()));
                await session2.Commit();
            }

            Parallel.For(0, 300, async i =>
            {

                var id2 = Guid.NewGuid();
                var employee = new Employee(id2);
                var session2 = new Session(rep);

                await session2.Add(employee);
                await session2.Commit();

                for (int j = 0; j < 100; j++)
                {
                    var e = await session2.Get<Employee>(id2);
                    for (int k = 0; k < 10; k++)
                    {
                        e.GiveRaise(10);
                    }
                    await session2.Commit();
                }
            });



            var id = Guid.NewGuid();
            await session.Add(new Employee(id));
            await session.Commit();
            for (var i = 0; i < 5_000; i++)
            {
                var employee = await session.Get<Employee>(id);
                employee.GiveRaise(10);
                await session.Commit();
            }

            var registrar = new RouteRegistrar(new ServiceLocator(rep, router));
            registrar.Register(typeof(EmployeeGiveRaise));

            var num = 3000;
            var tasks = new Task[num];
            for (int i = 0; i < num; i++)
            {
                tasks[i] = router.Send(new EmployeeGiveRaise(id, 10));
            }
            await Task.WhenAll(tasks);


            Parallel.For(0, 3000, async i =>
            {
                var id2 = Guid.NewGuid();
                var employee = new Employee(id2);
                var session2 = new Session(rep);

                await session2.Add(employee);
                await session2.Commit();

                for (int j = 0; j < 100; j++)
                {
                    await router.Send(new EmployeeGiveRaise(id2, 10));
                }
            });

            watch.Stop();
            Console.WriteLine($"{eventstore.Count:##,###} events saved");
            Console.WriteLine($"{eventstore.Read:##,###} events read from eventstore");
            Console.WriteLine($"{Employee.AppliedEvents:##,###} events applied");
            Console.WriteLine($"{watch.ElapsedMilliseconds:##,###} ms");
            Console.WriteLine($"{Employee.AppliedEvents/watch.ElapsedMilliseconds:##,###} events handled per ms");
        }
    }

    internal class ServiceLocator : IServiceLocator
    {
        private readonly IRepository _repository;
        private readonly Router _bus;

        public ServiceLocator(IRepository repository, Router bus)
        {
            _repository = repository;
            _bus = bus;
        }

        public T GetService<T>()
        {
            return (T) GetService(typeof(T));
        }

        public object GetService(Type type)
        {
            if(type == typeof(IHandlerRegistrar))
                return _bus;
            return new EmployeeHandler(new Session(_repository));
        }
    }

    public class EmployeeHandler : IEventHandler<EmployeeRaiseGiven>, ICommandHandler<EmployeeGiveRaise>
    {
        private readonly ISession _session;

        public EmployeeHandler(ISession session)
        {
            _session = session;
        }

        public Task Handle(EmployeeRaiseGiven message)
        {
            return Task.CompletedTask;
        }

        public async Task Handle(EmployeeGiveRaise message)
        {
            var employee = await _session.Get<Employee>(message.Id);
            employee.GiveRaise(message.Raise);
            await _session.Commit();
        }
    }

    public class EmployeeGiveRaise : ICommand
    {
        public EmployeeGiveRaise(Guid id, int raise)
        {
            Id = id;
            Raise = raise;
        }
        public int ExpectedVersion { get; set; }
        public Guid Id { get; set; }
        public int Raise { get; set; }
    }
}
