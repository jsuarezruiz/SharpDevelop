﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Parser
{
	/// <summary>
	/// Creates ParserDescriptor objects for the parsing service.
	/// </summary>
	/// <attribute name="supportedfilenamepattern">
	/// filename pattern (regex) for which the parser is used. (e.g. "\.(c|cpp|h|hpp)$")
	/// </attribute>
	/// <attribute name="class">
	/// Name of the IParser class.
	/// </attribute>
	/// <usage>Only in /SharpDevelop/Parser</usage>
	/// <returns>
	/// An ParserDescriptor object that wraps the IParser object.
	/// </returns>
	sealed class ParserDoozer : IDoozer
	{
		/// <summary>
		/// Gets if the doozer handles codon conditions on its own.
		/// If this property return false, the item is excluded when the condition is not met.
		/// </summary>
		public bool HandleConditions {
			get {
				return false;
			}
		}
		
		/// <summary>
		/// Creates an item with the specified sub items. And the current
		/// Condition status for this item.
		/// </summary>
		public object BuildItem(BuildItemArgs args)
		{
			return new ParserDescriptor(args.Codon);
		}
	}
}
