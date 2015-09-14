using System;

namespace Elephanet.Tests.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public string PropertyOne { get; set; }
        public string PropertyTwo { get; set; }
        public string PropertyThree { get; set; }
        public string PropertyFour { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}