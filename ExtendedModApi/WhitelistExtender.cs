using Sandbox;
using Sandbox.Game.SessionComponents;
using Sandbox.Game.World;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VRage.FileSystem;
using VRage.Plugins;
using VRage.Scripting;

namespace ExtendedModApi
{
	public class WhitelistExtender : IPlugin, IDisposable
	{
		public void Init(object gameInstance)
		{
			using (var whitelist = MyScriptCompiler.Static.Whitelist.OpenBatch())
			{
				whitelist.AllowTypes(MyWhitelistTarget.ModApi, typeof(MySessionComponentResearch));
			}
		}

		public void Dispose() { }
		public void Update() { }
	}
}
