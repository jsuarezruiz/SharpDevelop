﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AddInManager2;
using ICSharpCode.AddInManager2.Model;
using ICSharpCode.AddInManager2.Tests.Fakes;
using ICSharpCode.SharpDevelop;
using NuGet;
using NUnit.Framework;

namespace ICSharpCode.AddInManager2.Tests
{
	[TestFixture]
	[Category("PackageRepositoriesTests")]
	public class PackageRepositoriesTests
	{
		[Test, Description("Configured repositories must be correctly loaded from settings.")]
		public void ReadRepositoriesFromConfiguration()
		{
			FakeAddInManagerSettings settings = new FakeAddInManagerSettings();
			settings.PackageRepositories = new string[]
			{
				@"Repository1=C:\Repositories\Repository1",
				@"Repository2=C:\Repositories\Repository2"
			};
			
			AddInManagerEvents events = new AddInManagerEvents();
			PackageRepositories packageRepositories = new PackageRepositories(events, settings);
			
			var packageSources = packageRepositories.RegisteredPackageSources;
			Assert.That(packageSources, Is.Not.Null);
			Assert.That(packageSources.Count(), Is.EqualTo(3));
			
			// Default repository also must be present in first place
			Assert.That(packageSources.ElementAt(0).Name, Is.EqualTo(PackageRepositories.DefaultRepositoryName));
			Assert.That(packageSources.ElementAt(0).Source, Is.EqualTo(PackageRepositories.DefaultRepositorySource));
			
			// Then others are following
			Assert.That(packageSources.ElementAt(1).Name, Is.EqualTo("Repository1"));
			Assert.That(packageSources.ElementAt(1).Source, Is.EqualTo(@"C:\Repositories\Repository1"));
			Assert.That(packageSources.ElementAt(2).Name, Is.EqualTo("Repository2"));
			Assert.That(packageSources.ElementAt(2).Source, Is.EqualTo(@"C:\Repositories\Repository2"));
		}
		
		[Test, Description("Configured repositories including default repository must be correctly loaded from settings.")]
		public void ReadRepositoriesWithDefaultFromConfiguration()
		{
			FakeAddInManagerSettings settings = new FakeAddInManagerSettings();
			settings.PackageRepositories = new string[]
			{
				@"Repository1=C:\Repositories\Repository1",
				@"Repository2=C:\Repositories\Repository2",
				PackageRepositories.DefaultRepositoryName + "=" + PackageRepositories.DefaultRepositorySource
			};
			
			AddInManagerEvents events = new AddInManagerEvents();
			PackageRepositories packageRepositories = new PackageRepositories(events, settings);
			
			var packageSources = packageRepositories.RegisteredPackageSources;
			Assert.That(packageSources, Is.Not.Null);
			Assert.That(packageSources.Count(), Is.EqualTo(3));
			
			Assert.That(packageSources.ElementAt(0).Name, Is.EqualTo("Repository1"));
			Assert.That(packageSources.ElementAt(0).Source, Is.EqualTo(@"C:\Repositories\Repository1"));
			Assert.That(packageSources.ElementAt(1).Name, Is.EqualTo("Repository2"));
			Assert.That(packageSources.ElementAt(1).Source, Is.EqualTo(@"C:\Repositories\Repository2"));
			Assert.That(packageSources.ElementAt(2).Name, Is.EqualTo(PackageRepositories.DefaultRepositoryName));
			Assert.That(packageSources.ElementAt(2).Source, Is.EqualTo(PackageRepositories.DefaultRepositorySource));
		}
		
		[Test, Description("Invalid repository configuration must be handled without exceptions.")]
		public void ReadInvalidRepositoriesFromConfiguration()
		{
			FakeAddInManagerSettings settings = new FakeAddInManagerSettings();
			settings.PackageRepositories = new string[]
			{
				@"Repository1|C:\Repositories\Repository1",
				"=",
				""
			};
			
			AddInManagerEvents events = new AddInManagerEvents();
			PackageRepositories packageRepositories = new PackageRepositories(events, settings);
			
			var packageSources = packageRepositories.RegisteredPackageSources;
			Assert.That(packageSources, Is.Not.Null);
			
			// Must be 1, because DefaultRepository is always there...
			Assert.That(packageSources.Count(), Is.EqualTo(1));
		}
		
		[Test, Description("Configured repositories must be correctly saved to settings.")]
		public void SaveRepositoriesToConfiguration()
		{
			FakeAddInManagerSettings settings = new FakeAddInManagerSettings();
			settings.PackageRepositories = new string[]
			{
				@"Repository1=C:\Repositories\Repository1",
				@"Repository2=C:\Repositories\Repository2"
			};
			
			AddInManagerEvents events = new AddInManagerEvents();
			PackageRepositories packageRepositories = new PackageRepositories(events, settings);
			
			List<PackageSource> packageSources = new List<PackageSource>();
			packageSources.Add(new PackageSource(@"C:\Repositories\Repository3", "Repository3"));
			packageSources.Add(new PackageSource(@"C:\Repositories\Repository4", "Repository4"));
			packageRepositories.RegisteredPackageSources = packageSources;
			
			var packageRepositoriesSetting = settings.PackageRepositories;
			Assert.That(packageRepositoriesSetting, Is.Not.Null);
			Assert.That(packageRepositoriesSetting.Count(), Is.EqualTo(3));
			
			// Default repository also must be there in first place
			Assert.That(packageRepositoriesSetting.ElementAt(0),
			            Is.EqualTo(PackageRepositories.DefaultRepositoryName + "=" + PackageRepositories.DefaultRepositorySource));
			
			// Then others are following
			Assert.That(packageRepositoriesSetting.ElementAt(1), Is.EqualTo(@"Repository3=C:\Repositories\Repository3"));
			Assert.That(packageRepositoriesSetting.ElementAt(2), Is.EqualTo(@"Repository4=C:\Repositories\Repository4"));
		}
		
		[Test, Description("Configured repositories including default repository must be correctly saved to settings.")]
		public void SaveRepositoriesWithDefaultToConfiguration()
		{
			FakeAddInManagerSettings settings = new FakeAddInManagerSettings();
			settings.PackageRepositories = new string[]
			{
				@"Repository1=C:\Repositories\Repository1",
				@"Repository2=C:\Repositories\Repository2",
				PackageRepositories.DefaultRepositoryName + "=" + PackageRepositories.DefaultRepositorySource
			};
			
			AddInManagerEvents events = new AddInManagerEvents();
			PackageRepositories packageRepositories = new PackageRepositories(events, settings);
			
			List<PackageSource> packageSources = new List<PackageSource>();
			packageSources.Add(new PackageSource(@"C:\Repositories\Repository3", "Repository3"));
			packageSources.Add(new PackageSource(@"C:\Repositories\Repository4", "Repository4"));
			packageSources.Add(new PackageSource(PackageRepositories.DefaultRepositorySource, PackageRepositories.DefaultRepositoryName));
			packageRepositories.RegisteredPackageSources = packageSources;
			
			var packageRepositoriesSetting = settings.PackageRepositories;
			Assert.That(packageRepositoriesSetting, Is.Not.Null);
			Assert.That(packageRepositoriesSetting.Count(), Is.EqualTo(3));
			
			Assert.That(packageRepositoriesSetting.ElementAt(0), Is.EqualTo(@"Repository3=C:\Repositories\Repository3"));
			Assert.That(packageRepositoriesSetting.ElementAt(1), Is.EqualTo(@"Repository4=C:\Repositories\Repository4"));
			Assert.That(packageRepositoriesSetting.ElementAt(2),
			            Is.EqualTo(PackageRepositories.DefaultRepositoryName + "=" + PackageRepositories.DefaultRepositorySource));
		}
		
		[Test, Description("Configured invalid package sources list must be handled without exceptions.")]
		public void SaveInvalidRepositoryListToConfiguration()
		{
			FakeAddInManagerSettings settings = new FakeAddInManagerSettings();
			settings.PackageRepositories = new string[]
			{
				@"Repository1=C:\Repositories\Repository1",
				@"Repository2=C:\Repositories\Repository2"
			};
			
			AddInManagerEvents events = new AddInManagerEvents();
			PackageRepositories packageRepositories = new PackageRepositories(events, settings);

			// Setting invalid package source collection (null)			
			packageRepositories.RegisteredPackageSources = null;
			
			var packageRepositoriesSetting = settings.PackageRepositories;
			Assert.That(packageRepositoriesSetting, Is.Not.Null);
			// Must be 1, because DefaultRepository is always there...
			Assert.That(packageRepositoriesSetting.Count(), Is.EqualTo(1));
		}
	}
}