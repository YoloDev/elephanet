##Elephanet - A .NET api to PostgreSQL's json.##

###With an api thats easy to use###

This is VERY not ready for production.  I have NOT used it in production, so beware.

Heavily influenced by the RavenDb .NET client, this libary provides a simple api to allow easy use of postgres as a document store, taking advantage of Postgresql 9.4 and its new json indexing, allowing for fast querying of native json objects.

At the moment, not much has been implemented.

```

public class Car
{
 	public Guid Id {get;set;}
	public string Make {get;set;}
	public string Model {get;set;}
	public string ImageUrl {get;set;}
	public string NumberPlate {get;set;}
}

	//create the datastore
	DocumentStore store = new DocumentStore("Server=127.0.0.1;Port=5432;User Id=store_user;password=my super secret password;database=store;");
	
	
	//create the object
	var myAudi = new Car {
		Id = Guid.NewGuid(),
		Make = "Audi",
		Model = "A8",
		ImageUrl = "http://some_image_url",
		NumberPlate = "ABC029"
	};

	//save the object to the document store
	using (var session = store.OpenSession())
	{
		session.Store<Car>(myAudi);
		session.SaveChanges();
	}

	//get the same car back out of the document store
	using (var session = store.OpenSession())
	{
		var car = session.Load<Car>(myAudi.Id);
	}


	//create a couple of other cars	
	var myFord = new Car {
		Id = Guid.NewGuid(),
		Make = "Ford",
		Model = "Mustang",
		ImageUrl = "http://some_image_url",
		NumberPlate = "XYZ999"
	};

	var myOldAudi {
		Id = Guid.NewGuid(),
		Make = "Audi",
		Model = "A5",
		ImageUrl = "http://some_image_url",
		NumberPlate = "ABC002"
	};

	//store these other cars
	using (var session = store.OpenSession())
	{
		session.Store<Car>(myOldAudi);
		session.Store<Car>(myFord);
		session.SaveChanges();
	}

	//query by make
	using (var session = store.OpenSession())
	{
		var audis = session.Query(new Sql(":make", new[]{"Audi"}));
	}

	
```

###Currently implemented###

* You can load new documents to a unit of work cache (session.Store<T>());
* You can save the unit of work cache to the database (session.SaveChanges());
* You can retrieve individual documents from the database (session.Load<T>(your_id));
* You can retrieve documents from the database via a basic query (session.Query<T>(new Sql(":property", new[] {"value"}));)
* You can implement your own custom json serialization (uses NewtonSoft.Json by default), and it is easily overridable

###Still to come###

* Updates
* Deletes
* linq provider
* MORE TESTS!
* More docs
