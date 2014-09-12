##Elephanet - A .NET api to postgresqls json.##

###With an api thats easy to use###

This is VERY not ready for production.  I have NOT used it in production, so beware.

Heavily influenced by the RavenDb .NET client, this libary provides a simple api to allow easy use of postgres as a document store, taking advantage of Postgresql 9.4 and its new json indexing, allowing for fast querying of native json objects.

At the moment, not much has been implemented.

```
//create you poco
public class Car
{
 	public Guid Id {get;set;}
	public string Make {get;set;}
	public string Model {get;set;}
	public string ImageUrl {get;set;}
	public string NumberPlate {get;set;}
}

	//now shove it in the database
	DocumentStore store = new DocumentStore("Server=127.0.0.1;Port=5432;User Id=store_user;password=my super secret password;database=store;");
	var car = new Car {
		Id = Guid.NewGuid(),
		Make = "Audi",
		Model = "A8",
		ImageUrl = "http://some_image_url",
		NumberPlate = "ABC029"
	};

	using (var session = store.OpenSession())
	{
		session.Store<Car>(car);
		session.SaveChanges();
	}

	//get the same car back out of the db
	using (var session = store.OpenSession())
	{
		var car = session.Load<Car>(car.Id);
	}
```

###Still to come###

* Querying beyond a simple key fetch
* MORE TESTS!
* More docs
