using ExceptionReporting.SystemInfo;
using NUnit.Framework;

namespace ExceptionReporting.Tests
{
	public class SysInfoResult_Tests
	{
		private SysInfoResult _sysInfoResult;
		private SysInfoResult _child;

		[SetUp]
		public void SetUp()
		{
			_sysInfoResult = new SysInfoResult("OS");
			_child = new SysInfoResult("OS_Child");
			_sysInfoResult.ChildResults.Add(_child);
			_child.Nodes.AddRange(new[] { "CountryCode = 1", "CodeSet = 7", "OSLanguage = Portugese" });
		}

		[Test]
		public void Can_Filter()
		{
			var filterResults = _sysInfoResult.Filter(new[] {"CountryCode", "CodeSet"});

			Assert.That(filterResults.ChildResults[0].Nodes.Count, Is.EqualTo(2));
			CollectionAssert.Contains(filterResults.ChildResults[0].Nodes, "CountryCode = 1");
			CollectionAssert.Contains(filterResults.ChildResults[0].Nodes, "CodeSet = 7");
		}

		[Test]
		public void Can_Filter_Just_Key()
		{
			_child.AddNode("Code = CodeSet");
			var filterResults = _sysInfoResult.Filter(new[] { "CodeSet"});

			Assert.That(filterResults.ChildResults[0].Nodes.Count, Is.EqualTo(1));
			CollectionAssert.Contains(filterResults.ChildResults[0].Nodes, "CodeSet = 7");
		}

		[Test, Ignore("to fix this we'd need to get a propery key/value pair going")]
		public void Can_Filter_Just_Key_If_Ends_In_SameKey()
		{
			_child.AddNode("ServicePackVersion = 2");
			_child.AddNode("Version = 6.002");
			var filterResults = _sysInfoResult.Filter(new[] { "Version" });

			Assert.That(filterResults.ChildResults[0].Nodes.Count, Is.EqualTo(1));
			CollectionAssert.Contains(filterResults.ChildResults[0].Nodes, "Version = 6.002");
		}
	}
}