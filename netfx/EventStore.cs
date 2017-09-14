using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;

namespace CQRSlite_speedtest
{
    public class EventStore : IEventStore
    {
        private readonly IEventPublisher _publisher;

        public EventStore(IEventPublisher publisher)
        {
            _publisher = publisher;
        }

        public long Saved;
        public long Read;
        private static List<IEvent> _emptyList = new List<IEvent>();
        private static object _lock = new object();
        Dictionary<Guid, List<IEvent>> _dict = new Dictionary<Guid, List<IEvent>>();
        public Task Save(IEnumerable<IEvent> events, CancellationToken token = default(CancellationToken))
        {
            var enumerable = events.ToArray();
            Interlocked.Add(ref Saved, enumerable.Length);
            List<IEvent> list;
            var id = enumerable[0].Id;

            lock (_lock)
            {
                var exist = _dict.TryGetValue(id, out list);
                if (!exist)
                {
                    list = new List<IEvent>();
                    _dict.Add(id, list);
                    
                }
            }
            foreach (var e in enumerable)
            {

                list.Add(e);
                _publisher.Publish(e, token);
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<IEvent>> Get(Guid aggregateId, int fromVersion, CancellationToken token)
        {
            if (_dict.ContainsKey(aggregateId))
            {
                var all = _dict[aggregateId];
                if (all.Count <= fromVersion + 1)
                    return Task.FromResult<IEnumerable<IEvent>>(_emptyList);
                var events = all.GetRange(fromVersion + 1, all.Count);
                Interlocked.Add(ref Read, events.Count);
                return Task.FromResult((IEnumerable<IEvent>)events);

            }
            return Task.FromResult<IEnumerable<IEvent>>(_emptyList);
        }

        public int Count => _dict.Sum(x => x.Value.Count);
    }
}