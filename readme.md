[![Build Status](https://travis-ci.org/YoloDev/elephanet.svg?branch=master)](https://travis-ci.org/YoloDev/elephanet) 
##Elephanet - A .NET document database built on PostgreSQL.##

###With an api thats easy to use###

A document db api backed by Postgresql.

Heavily influenced by the RavenDb .NET client, this libary provides a simple api to allow easy use of postgres as a document store, taking advantage of Postgresql 9.4 and its new jsonb indexing, allowing for fast querying of native json objects.

###Quick Start

For Windows

1.  Install Postgresql > 9.4 (available at http://www.postgresql.org/download/windows/). When you do so, pay particular attention to your postgres user password (you will need this in the next step)
2.  Clone (or fork) this repository
3.  Alter create_store.sql file and replace "my super secret password" with your own password
4.  Run create_store.bat from within a cmd prompt.

For Ubuntu (and likely other Debian based distros)

1.  Install Postgresql via apt-get.  Make sure it is greater than version 9.4
2.  Install Mono (http://www.mono-project.com/docs/getting-started/install/linux/)
3.  Clone (or fork) this repository
4.  Alter create_store.sql file and replace "my super secret password" with your own password
5.  run `psql -f create_store.sql -U postgres`

###Got Questions?

- https://jabbr.net/#/rooms/elephanet (chat) 
- https://groups.google.com/d/forum/elephanet (forum)



###Example Code

```
using System;

public class Car
{
 	public Guid Id {get;set;}
	public string Make {get;set;}
	public string Model {get;set;}
	public string ImageUrl {get;set;}
	public string NumberPlate {get;set;}
}
```

```
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
		var car = session.GetById<Car>(myAudi.Id);
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

	//update existing object
	using (var session = store.OpenSession())
	{
		var audi = session.GetById<Car>(myOldAudi.Id);
		audi.Image_Url = "http://some_new_url";

		session.Store<Car>(audi);
		session.SaveChanges();
	}

	//query by make
	using (var session = store.OpenSession())
	{
		var audis = session.Query<Car>().Where(c => c.Make == "Audi").ToList();
	}

	//delete
	using (var session = store.OpenSession())
	{
		session.Delete<Car>(myOldAudi.Id);
	}

	//delete all of a particular type
	using (var session = store.OpenSession())
	{
		session.DeleteAll<Car>();
	}
	
```

###Currently implemented###

* You can ```session.Store<T>(T entity)```
* You can ```session.SaveChanges();```
* You can ```session.GetById<T>(Guid id)```
* You can ```session.GetByIds<T>(IEnumerable<Guid> ids)```
* You can ```session.Delete<T>(Guid id)```
* You can ```session.GetAll<T>();```
* You can ```session.DeleteAll<T>();```
* You can ```session.Query<T>(x => x.SomeAttribute == "some value").ToList();```
* You can ```session.Query<T>(x => x.SomeAttribute == "some value").Take(10).Skip(5);```
* You can ```session.Query<T>(x => x.SomeAttribute == "some value").OrderBy(c => c.SomeOtherAttibute);```
* You can ```session.Query<T>(x => x.SomeAttribute == "some value").OrderByDescending(c => c.SomeOtherAttibute);```

###Things of note:

* You can implement your own custom json serialization (Jil is internalised by default)
* Store<T>() is a unit of work stored in memory.  SaveChanges() flushes the in memory values to the database
