﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.Profiler.Controller.Data;
using ICSharpCode.Profiler.Controller.Queries;

namespace ICSharpCode.Profiler.AddIn.Commands
{
	/// <summary>
	/// Description of FindCallsOfSelected
	/// </summary>
	public class FindCallsOfSelected : ProfilerMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			var list = GetSelectedItems();
			
			if (list.Any()) {
				var items = from item in list select item.Node;	
				
				List<string> parts = new List<string>();
								
				foreach (CallTreeNode node in items) {
					NodePath p = node.GetPath().First();
					if (p != null) {
						parts.Add("c.NameMapping.Id == " + node.NameMapping.Id);
					}
				}
				
				string header = StringParser.Parse("${res:AddIns.Profiler.Commands.FindCallsOfSelected.TabTitle}");
				
				Parent.CreateTab(header, "from c in Calls where " + string.Join(" || ", parts.ToArray()) + " select c");
			}	
		}
	}
}
