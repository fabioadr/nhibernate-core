﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;
using System.Linq;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.CompositeId
{
	using System.Threading.Tasks;
	/// <summary>
	/// Summary description for ClassWithCompositeIdFixture.
	/// </summary>
	[TestFixture]
	public class ClassWithCompositeIdFixtureAsync : TestCase
	{
		private DateTime firstDateTime = new DateTime(2003, 8, 16);
		private DateTime secondDateTime = new DateTime(2003, 8, 17);
		private Id id;
		private Id secondId;

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override string[] Mappings
		{
			get { return new string[] {"CompositeId.ClassWithCompositeId.hbm.xml"}; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return !(dialect is Dialect.FirebirdDialect); // Firebird has no CommandTimeout, and locks up during the tear-down of this fixture
		}

		protected override void OnSetUp()
		{
			id = new Id("stringKey", 3, firstDateTime);
			secondId = new Id("stringKey2", 5, secondDateTime);
		}

		protected override void OnTearDown()
		{
			using (ISession s = Sfi.OpenSession())
			{
				s.Delete("from ClassWithCompositeId");
				s.Flush();
			}
		}


		/// <summary>
		/// Test the basic CRUD operations for a class with a Composite Identifier
		/// </summary>
		/// <remarks>
		/// The following items are tested in this Test Script
		/// <list type="">
		///		<item>
		///			<term>Save</term>
		///		</item>
		///		<item>
		///			<term>Load</term>
		///		</item>
		///		<item>
		///			<term>Criteria</term>
		///		</item>
		///		<item>
		///			<term>Update</term>
		///		</item>
		///		<item>
		///			<term>Delete</term>
		///		</item>
		///		<item>
		///			<term>Criteria - No Results</term>
		///		</item>
		/// </list>
		/// </remarks>
		[Test]
		public async Task TestSimpleCRUDAsync()
		{
			ClassWithCompositeId theClass;
			ClassWithCompositeId theSecondClass;

			// insert the new objects
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
			    theClass = new ClassWithCompositeId(id);
			    theClass.OneProperty = 5;

			    theSecondClass = new ClassWithCompositeId(secondId);
			    theSecondClass.OneProperty = 10;

			    await (s.SaveAsync(theClass));
			    await (s.SaveAsync(theSecondClass));

			    await (t.CommitAsync());
			}

			// verify they were inserted and test the SELECT
			ClassWithCompositeId theClass2;
			ClassWithCompositeId theSecondClass2;
			using (ISession s2 = OpenSession())
			using (ITransaction t2 = s2.BeginTransaction())
			{
				theClass2 = (ClassWithCompositeId) await (s2.LoadAsync(typeof(ClassWithCompositeId), id));
				Assert.AreEqual(id, theClass2.Id);

				IList results2 = await (s2.CreateCriteria(typeof(ClassWithCompositeId))
				                   .Add(Expression.Eq("Id", secondId))
				                   .ListAsync());

				Assert.AreEqual(1, results2.Count);
				theSecondClass2 = (ClassWithCompositeId) results2[0];

				ClassWithCompositeId theClass2Copy = (ClassWithCompositeId) await (s2.LoadAsync(typeof(ClassWithCompositeId), id));

				// verify the same results through Criteria & Load were achieved
				Assert.AreSame(theClass2, theClass2Copy);

				// compare them to the objects created in the first session
				Assert.AreEqual(theClass.Id, theClass2.Id);
				Assert.AreEqual(theClass.OneProperty, theClass2.OneProperty);

				Assert.AreEqual(theSecondClass.Id, theSecondClass2.Id);
				Assert.AreEqual(theSecondClass.OneProperty, theSecondClass2.OneProperty);

				// test the update functionallity
				theClass2.OneProperty = 6;
				theSecondClass2.OneProperty = 11;

				await (s2.UpdateAsync(theClass2));
				await (s2.UpdateAsync(theSecondClass2));

				await (t2.CommitAsync());
			}

			// lets verify the update went through
			using (ISession s3 = OpenSession())
			using (ITransaction t3 = s3.BeginTransaction())
			{
				ClassWithCompositeId theClass3 = (ClassWithCompositeId) await (s3.LoadAsync(typeof(ClassWithCompositeId), id));
				ClassWithCompositeId theSecondClass3 = (ClassWithCompositeId) await (s3.LoadAsync(typeof(ClassWithCompositeId), secondId));

				// check the update properties
				Assert.AreEqual(theClass3.OneProperty, theClass2.OneProperty);
				Assert.AreEqual(theSecondClass3.OneProperty, theSecondClass2.OneProperty);

				// test the delete method
				await (s3.DeleteAsync(theClass3));
				await (s3.DeleteAsync(theSecondClass3));

				await (t3.CommitAsync());
			}

			// lets verify the delete went through
			using (ISession s4 = OpenSession())
			{
				try
				{
					ClassWithCompositeId theClass4 = (ClassWithCompositeId) await (s4.LoadAsync(typeof(ClassWithCompositeId), id));
				}
				catch (ObjectNotFoundException)
				{
					// I expect this to be thrown because the object no longer exists...
				}

				IList results = await (s4.CreateCriteria(typeof(ClassWithCompositeId))
				                  .Add(Expression.Eq("Id", secondId))
				                  .ListAsync());

				Assert.AreEqual(0, results.Count);
			}
		}

		[Test]
		public async Task CriteriaAsync()
		{
			Id id = new Id("stringKey", 3, firstDateTime);
			ClassWithCompositeId cId = new ClassWithCompositeId(id);
			cId.OneProperty = 5;

			// add the new instance to the session so I have something to get results 
			// back for
			using (ISession s = OpenSession())
			{
				await (s.SaveAsync(cId));
				await (s.FlushAsync());
			}

			using (ISession s = OpenSession())
			{
				ICriteria c = s.CreateCriteria(typeof(ClassWithCompositeId));
				c.Add(Expression.Eq("Id", id));

				// right now just want to see if the Criteria is valid
				IList results = await (c.ListAsync());

				Assert.AreEqual(1, results.Count);
			}
		}

		[Test]
		public async Task HqlAsync()
		{
			// insert the new objects
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				ClassWithCompositeId theClass = new ClassWithCompositeId(id);
				theClass.OneProperty = 5;

				ClassWithCompositeId theSecondClass = new ClassWithCompositeId(secondId);
				theSecondClass.OneProperty = 10;

				await (s.SaveAsync(theClass));
				await (s.SaveAsync(theSecondClass));

				await (t.CommitAsync());
			}

			using (ISession s2 = OpenSession())
			{
				IQuery hql = s2.CreateQuery("from ClassWithCompositeId as cwid where cwid.Id.KeyString = :keyString");

				hql.SetString("keyString", id.KeyString);

				IList results = await (hql.ListAsync());

				Assert.AreEqual(1, results.Count);
			}
		}

		[Test]
		public async Task HqlInClauseAsync()
		{
			var id1 = id;
			var id2 = secondId;
			var id3 = new Id(id.KeyString, id.GetKeyShort(), id2.KeyDateTime);

			// insert the new objects
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				await (s.SaveAsync(new ClassWithCompositeId(id1) {OneProperty = 5}));
				await (s.SaveAsync(new ClassWithCompositeId(id2) {OneProperty = 10}));
				await (s.SaveAsync(new ClassWithCompositeId(id3)));

				await (t.CommitAsync());
			}

			using (var s = OpenSession())
			{
				var results1 = await (s.CreateQuery("from ClassWithCompositeId x where x.Id in (:id1, :id2)")
								.SetParameter("id1", id1)
								.SetParameter("id2", id2)
								.ListAsync<ClassWithCompositeId>());
				var results2 = await (s.CreateQuery("from ClassWithCompositeId x where  x.Id in (:id1)")
								.SetParameter("id1", id1)
								.ListAsync<ClassWithCompositeId>());
				var results3 = await (s.CreateQuery("from ClassWithCompositeId x where  x.Id not in (:id1)")
								.SetParameter("id1", id1)
								.ListAsync<ClassWithCompositeId>());
				var results4 = await (s.CreateQuery("from ClassWithCompositeId x where x.Id not in (:id1, :id2)")
								.SetParameter("id1", id1)
								.SetParameter("id2", id2)
								.ListAsync<ClassWithCompositeId>());

				Assert.Multiple(
					() =>
					{
						Assert.That(results1.Count, Is.EqualTo(2), "in multiple ids");
						Assert.That(results1.Select(x => x.Id), Is.EquivalentTo(new[] {id1, id2}), "in multiple ids");
						Assert.That(results2.Count, Is.EqualTo(1), "in single id");
						Assert.That(results2.Single().Id, Is.EqualTo(id1), "in single id");
						Assert.That(results3.Count, Is.EqualTo(2), "not in single id");
						Assert.That(results3.Select(x => x.Id), Is.EquivalentTo(new[] {id2, id3}), "not in single id");
						Assert.That(results4.Count, Is.EqualTo(1), "not in multiple ids");
						Assert.That(results4.Single().Id, Is.EqualTo(id3), "not in multiple ids");
					});
			}
		}

		[Test]
		public async Task QueryOverInClauseSubqueryAsync()
		{
			if (!TestDialect.SupportsRowValueConstructorSyntax)
			{
					Assert.Ignore();
			}

			// insert the new objects
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				await (s.SaveAsync(new ClassWithCompositeId(id) {OneProperty = 5}));
				await (s.SaveAsync(new ClassWithCompositeId(secondId) {OneProperty = 10}));
				await (s.SaveAsync(new ClassWithCompositeId(new Id(id.KeyString, id.GetKeyShort(), secondId.KeyDateTime))));

				await (t.CommitAsync());
			}

			using (var s = OpenSession())
			{
				var results = await (s.QueryOver<ClassWithCompositeId>().WithSubquery.WhereProperty(p => p.Id).In(QueryOver.Of<ClassWithCompositeId>().Where(p => p.Id.KeyString == id.KeyString).Select(p => p.Id)).ListAsync());
				Assert.That(results.Count, Is.EqualTo(2));
			}
		}

		[Test]
		public async Task HqlInClauseSubqueryAsync()
		{
			if (!TestDialect.SupportsRowValueConstructorSyntax)
				Assert.Ignore();

			// insert the new objects
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				await (s.SaveAsync(new ClassWithCompositeId(id) {OneProperty = 5}));
				await (s.SaveAsync(new ClassWithCompositeId(secondId) {OneProperty = 10}));
				await (s.SaveAsync(new ClassWithCompositeId(new Id(id.KeyString, id.GetKeyShort(), secondId.KeyDateTime))));

				await (t.CommitAsync());
			}

			using (var s = OpenSession())
			{
				var results = await (s.CreateQuery("from ClassWithCompositeId x where  x.Id in (select s.Id from ClassWithCompositeId s where s.Id.KeyString = :keyString)")
								.SetParameter("keyString", id.KeyString).ListAsync());
				Assert.That(results.Count, Is.EqualTo(2));
			}
		}

		//GH-1376
		[Test]
		public async Task HqlInClauseSubquery_ForEntityAsync()
		{
			if (!TestDialect.SupportsRowValueConstructorSyntax)
				Assert.Ignore();

			// insert the new objects
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				await (s.SaveAsync(new ClassWithCompositeId(id) {OneProperty = 5}));
				await (s.SaveAsync(new ClassWithCompositeId(secondId) {OneProperty = 10}));
				await (s.SaveAsync(new ClassWithCompositeId(new Id(id.KeyString, id.GetKeyShort(), secondId.KeyDateTime))));

				await (t.CommitAsync());
			}

			using (var s = OpenSession())
			{
				var results = await (s.CreateQuery("from ClassWithCompositeId x where x in (select s from ClassWithCompositeId s where s.Id.KeyString = :keyString)")
								.SetParameter("keyString", id.KeyString).ListAsync());
				Assert.That(results.Count, Is.EqualTo(2));
			}
		}
	}
}
