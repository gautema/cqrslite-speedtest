using System;
using System.Threading;
using CQRSlite.Domain;
using CQRSlite.Events;

namespace CQRSlite_speedtest
{
    public class Employee : AggregateRoot
    {
        private int _events;
        public static long AppliedEvents;
        private Employee()
        {
        }
        public Employee(Guid id)
        {
            Id = id;
            ApplyChange(new EmployeeCreated(id, 100));
        }

        public void Apply(EmployeeCreated e)
        {
            Interlocked.Increment(ref AppliedEvents);
            Salary = e.Salary;
        }


        public void GiveRaise(int i)
        {
            ApplyChange(new EmployeeRaiseGiven(i));
        }

        private void Apply(EmployeeRaiseGiven e)
        {
            Interlocked.Increment(ref AppliedEvents);
            Salary += e.Raise;
        }

        public int Salary { get; set; }

        public override int GetHashCode()
        {
            return 1;
        }
    }

    public class EmployeeRaiseGiven : IEvent
    {
        public EmployeeRaiseGiven(int i)
        {
            Raise = i;
        }

        public int Raise { get; set; }

        public Guid Id { get; set; }
        public int Version { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public override int GetHashCode()
        {
            return 2;
        }
    }

    public class EmployeeCreated : IEvent
    {
        public EmployeeCreated(Guid id, int salary)
        {
            Id = id;
            Salary = salary;
        }

        public Guid Id { get; set; }
        public int Salary { get; }
        public int Version { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public override int GetHashCode()
        {
            return 3;
        }
    }
}