﻿using System;
using Elephanet.Serialization;
using Shouldly;
using Xunit;

namespace Elephanet.Tests
{
    public class JsonConverterTests
    {
        private readonly JilJsonConverter _converter;
        private Guid _id;
        private readonly string _description;
        private readonly AnotherEntity _entityToSerialize;

        public JsonConverterTests()
        {
            _converter = new JilJsonConverter();
            _id = Guid.NewGuid();
            _description = "xxx";
            _entityToSerialize = new AnotherEntity {Id = _id,Description = _description};
        }

        [Fact]
        public void JsonConverter_GivenAnInheritedObject_ShouldSerialize()
        {
            string json = _converter.Serialize(_entityToSerialize);
            json.ShouldContain(_id.ToString());
            json.ShouldContain(_description);
        }

        [Fact]
        public void JsonConverter_GivenAnInheritedObject_ShouldDeserialize()
        {
            string json = _converter.Serialize(_entityToSerialize);
            var entity = _converter.Deserialize<AnotherEntity>(json);
            entity.Id.ShouldBe(_id);
            entity.Description.ShouldBe(_description);
        }

    }

    public class BaseEntity
    {
        public Guid Id { get; set; }
    }

    public class AnotherEntity : BaseEntity
    {
       public string Description { get; set; } 
    } 
}
