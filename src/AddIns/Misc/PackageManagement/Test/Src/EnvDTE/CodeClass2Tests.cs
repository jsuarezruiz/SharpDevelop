﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeClass2Tests : CodeModelTestBase
	{
		CodeClass2 codeClass;
		
		void CreateClass(string code)
		{
			AddCodeFile("class.cs", code);
			ITypeDefinition typeDefinition = GetFirstTypeDefinition();
			CreateClass(typeDefinition);
		}
		
		ITypeDefinition GetFirstTypeDefinition()
		{
			return assemblyModel.TopLevelTypeDefinitions.First().Resolve();
		}
		
		void CreateClass(ITypeDefinition typeDefinition)
		{
			codeClass = new CodeClass2(codeModelContext, typeDefinition);
		}
		
		[Test]
		public void Language_CSharpProject_ReturnsCSharpModelLanguage()
		{
			CreateClass("class MyClass {}");
			
			string language = codeClass.Language;
			
			Assert.AreEqual(global::EnvDTE.CodeModelLanguageConstants.vsCMLanguageCSharp, language);
		}
		
		[Test]
		public void Language_VisualBasicProject_ReturnsVisualBasicModelLanguage()
		{
			projectLanguage = "VB";
			CreateClass("class MyClass {}");
			
			string language = codeClass.Language;
			
			Assert.AreEqual(global::EnvDTE.CodeModelLanguageConstants.vsCMLanguageVB, language);
		}
		
		[Test]
		public void Access_PublicClass_ReturnsPublic()
		{
			CreateClass("public class MyClass {}");
			
			global::EnvDTE.vsCMAccess access = codeClass.Access;
			
			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPublic, access);
		}
		
		[Test]
		public void Access_PrivateClass_ReturnsPrivate()
		{
			CreateClass("class MyClass {}");
			
			global::EnvDTE.vsCMAccess access = codeClass.Access;
			
			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPrivate, access);
		}
		
		[Test]
		public void ImplementedInterfaces_ClassImplementsGenericICollectionOfString_ReturnsCodeInterfaceForICollection()
		{
			CreateClass(
				"using System;\r\n" +
				"using System.Collections.Generic;\r\n" +
				"class MyClass : ICollection<string> {}");
			
			global::EnvDTE.CodeElements codeElements = codeClass.ImplementedInterfaces;
			
			CodeInterface codeInterface = codeElements.FirstCodeInterfaceOrDefault();
			Assert.AreEqual(1, codeElements.Count);
			Assert.AreEqual("System.Collections.Generic.ICollection<System.String>", codeInterface.FullName);
		}
		
		[Test]
		public void ImplementedInterfaces_ClassHasBaseTypeButNoInterfaces_ReturnsNoItems()
		{
			CreateClass(
				"namespace MyNamespace {\r\n" +
				"    public class MyClass : MyBaseClass {}\r\n" +
				"    public class MyBaseClass {}\r\n" +
				"}");
			
			global::EnvDTE.CodeElements codeElements = codeClass.ImplementedInterfaces;
			
			Assert.AreEqual(0, codeElements.Count);
		}
		
		[Test]
		public void BaseTypes_ClassBaseTypeIsSystemObject_ReturnsSystemObject()
		{
			CreateClass("public class MyClass {}");
			
			global::EnvDTE.CodeElements codeElements = codeClass.Bases;
			
			CodeClass2 baseClass = codeElements.FirstCodeClass2OrDefault();
			Assert.AreEqual(1, codeElements.Count);
			Assert.AreEqual("System.Object", baseClass.FullName);
			Assert.AreEqual("Object", baseClass.Name);
		}
		
		[Test]
		public void BaseTypes_ClassIsSystemObject_ReturnsNoCodeElements()
		{
			CreateClass("public class MyClass {}");
			ITypeDefinition myClassType = GetFirstTypeDefinition();
			ITypeDefinition systemObject = myClassType.DirectBaseTypes.First().GetDefinition();
			CreateClass(systemObject);
		
			global::EnvDTE.CodeElements codeElements = codeClass.Bases;
			
			Assert.AreEqual(0, codeElements.Count);
		}
		
		[Test]
		public void Members_ClassHasOneMethod_ReturnsOneMethod()
		{
			CreateClass(
				"public class MyClass {\r\n" +
				"    public void MyMethod() {}\r\n" +
				"}");
			
			global::EnvDTE.CodeElements codeElements = codeClass.Members;
			
			CodeFunction2 codeFunction = codeElements.FirstCodeFunction2OrDefault();
			Assert.AreEqual(1, codeElements.Count);
			Assert.AreEqual("MyMethod", codeFunction.Name);
		}
		
		[Test]
		public void Members_ClassHasOneProperty_ReturnsOneProperty()
		{
			CreateClass(
				"public class MyClass {\r\n" +
				"    public int MyProperty { get; set; }\r\n" +
				"}");
			
			global::EnvDTE.CodeElements codeElements = codeClass.Members;
			
			CodeProperty2 codeProperty = codeElements.FirstCodeProperty2OrDefault();
			Assert.AreEqual(1, codeElements.Count);
			Assert.AreEqual("MyProperty", codeProperty.Name);
		}
		
		[Test]
		public void Members_ClassHasOneField_ReturnsOneField()
		{
			CreateClass(
				"public class MyClass {\r\n" +
				"    public int MyField;\r\n" +
				"}");
			
			global::EnvDTE.CodeElements codeElements = codeClass.Members;
			
			CodeVariable codeVariable = codeElements.FirstCodeVariableOrDefault();
			Assert.AreEqual(1, codeElements.Count);
			Assert.AreEqual("MyField", codeVariable.Name);
		}
		
		[Test]
		public void Kind_PublicClass_ReturnsClass()
		{
			CreateClass("public class MyClass {}");
			
			global::EnvDTE.vsCMElement kind = codeClass.Kind;
			
			Assert.AreEqual(global::EnvDTE.vsCMElement.vsCMElementClass, kind);
		}
		
		[Test]
		public void Namespace_PublicClass_ReturnsClassNamespace()
		{
			CreateClass(
				"namespace MyNamespace.Test {\r\n" +
				"    public class MyClass {}\r\n" +
				"}");
			
			global::EnvDTE.CodeNamespace codeNamespace = codeClass.Namespace;
			
			Assert.AreEqual("MyNamespace.Test", codeNamespace.FullName);
		}
		
		[Test]
		public void PartialClasses_ClassIsNotPartial_ReturnsClass()
		{
			CreateClass("public class MyClass {}");
			
			global::EnvDTE.CodeElements partialClasses = codeClass.PartialClasses;
			
			CodeClass firstClass = partialClasses.FirstCodeClass2OrDefault();
			Assert.AreEqual(1, partialClasses.Count);
			Assert.AreEqual(codeClass, firstClass);
		}
		
		[Test]
		public void Members_GetFirstPropertyTwice_PropertiesAreConsideredEqualWhenAddedToList()
		{
			CreateClass(
				"public class MyClass {\r\n" +
				"    public int MyProperty { get; set; }\r\n" +
				"}");
			CodeProperty2 property = codeClass.Members.FirstCodeProperty2OrDefault();
			var properties = new List<CodeProperty2>();
			properties.Add(property);
			
			CodeProperty2 property2 = codeClass.Members.FirstCodeProperty2OrDefault();
			
			bool contains = properties.Contains(property2);
			Assert.IsTrue(contains);
		}
		
		[Test]
		public void IsAbstract_ClassIsAbstract_ReturnsTrue()
		{
			CreateClass("public abstract class MyClass {}");
			
			bool isAbstract = codeClass.IsAbstract;
			
			Assert.IsTrue(isAbstract);
		}
		
		[Test]
		public void IsAbstract_ClassIsNotAbstract_ReturnsFalse()
		{
			CreateClass("public class MyClass {}");
			
			bool isAbstract = codeClass.IsAbstract;
			
			Assert.IsFalse(isAbstract);
		}
//		
//		[Test]
//		public void ClassKind_ClassIsPartial_ReturnsPartialClassKind()
//		{
//			CreateProjectContent();
//			CreatePublicClass("MyClass");
//			ClassIsPartial();
//			
//			global::EnvDTE.vsCMClassKind kind = codeClass.ClassKind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMClassKind.vsCMClassKindPartialClass, kind);
//		}
		
		[Test]
		public void ClassKind_ClassIsNotPartial_ReturnsMainClassKind()
		{
			CreateClass("public class MyClass {}");
			
			global::EnvDTE.vsCMClassKind kind = codeClass.ClassKind;
			
			Assert.AreEqual(global::EnvDTE.vsCMClassKind.vsCMClassKindMainClass, kind);
		}
		
		[Test]
		public void IsGeneric_ClassIsGeneric_ReturnsTrue()
		{
			CreateClass("public class MyClass<T> {}");
			
			bool generic = codeClass.IsGeneric;
			
			Assert.IsTrue(generic);
		}
		
		[Test]
		public void IsGeneric_ClassIsNotGeneric_ReturnsFalse()
		{
			CreateClass("public class MyClass {}");
			
			bool generic = codeClass.IsGeneric;
			
			Assert.IsFalse(generic);
		}
//		
//		[Test]
//		public void ClassKind_ChangeClassToBePartial_UsesClassKindUpdaterToModifyClass()
//		{
//			CreateProjectContent();
//			CreatePublicClass("MyClass");
//			
//			codeClass.ClassKind = global::EnvDTE.vsCMClassKind.vsCMClassKindPartialClass;
//			
//			classKindUpdater.AssertWasCalled(updater => updater.MakeClassPartial());
//		}
		
		[Test]
		public void ClassKind_ChangeClassToBeMainClass_ThrowsNotImplementedException()
		{
			CreateClass("public class MyClass {}");
			
			Assert.Throws<NotImplementedException>(() => codeClass.ClassKind = global::EnvDTE.vsCMClassKind.vsCMClassKindMainClass);
		}
		
		[Test]
		public void ImplementedInterfaces_ClassImplementsIDisposable_ReturnsCodeInterfaceForIDisposable()
		{
			CreateClass(
				"using System;\r\n" +
				"class MyClass : IDisposable {}");
			
			global::EnvDTE.CodeElements codeElements = codeClass.ImplementedInterfaces;
			
			CodeInterface codeInterface = codeElements.FirstCodeInterfaceOrDefault();
			Assert.AreEqual(1, codeElements.Count);
			Assert.AreEqual("System.IDisposable", codeInterface.FullName);
		}
		
		[Test]
		public void FullName_GenericClass_ReturnsTypeArguments()
		{
			CreateClass(
				"namespace Test {\r\n" +
				"    class MyClass<T> {}\r\n" +
				"}");
			
			string name = codeClass.FullName;
			
			Assert.AreEqual("Test.MyClass<T>", name);
		}
		
		[Test]
		public void Attributes_ClassHasOneAttribute_ReturnsOneAttribute()
		{
			CreateClass("[System.ObsoleteAttribute] class MyClass {}");
		
			CodeAttribute2 attribute = codeClass.Attributes.FirstCodeAttribute2OrDefault();
			
			Assert.AreEqual(1, codeClass.Attributes.Count);
			Assert.AreEqual("Obsolete", attribute.Name);
		}
		
		[Test]
		public void Attributes_GetItemByNameWhenClassHasOneAttribute_ReturnsOneAttribute()
		{
			CreateClass("[System.ObsoleteAttribute] class MyClass {}");
		
			var attribute = codeClass.Attributes.Item("Obsolete") as CodeAttribute2;
			
			Assert.AreEqual("Obsolete", attribute.Name);
		}
	}
}
