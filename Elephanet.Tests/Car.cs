using System;

namespace Elephanet.Tests
{
    public class Car
    {
        public Guid Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string ImageUrl { get; set; }
        public string NumberPlate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
