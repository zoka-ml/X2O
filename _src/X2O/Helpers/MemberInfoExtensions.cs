using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Zoka.X2O.Helpers
{
	/// <summary>Extensions to the MemberInfo class</summary>
	public static class MemberInfoExtensions
	{
		/// <summary>Will return the type of the member, in cases it is Field or Property</summary>
		/// <exception cref="NotSupportedException">In case the member is not Field nor Property</exception>
		public static Type GetTypeOfMember(this MemberInfo _member_info)
		{
			if (_member_info.MemberType == MemberTypes.Field)
				return (_member_info as FieldInfo).FieldType;
			if (_member_info.MemberType == MemberTypes.Property)
				return (_member_info as PropertyInfo).PropertyType;

			throw new NotSupportedException("Getting type of member which is not Field nor Property is not allowed");
		}

		/// <summary>Will set the value of the member in case it is Field or Property</summary>
		/// <exception cref="NotSupportedException">In case the member is not Field nor Property</exception>
		public static void SetMemberValue(this MemberInfo _member_info, object _parent_object, object _value)
		{
			if (_member_info.MemberType == MemberTypes.Field)
				(_member_info as FieldInfo).SetValue(_parent_object, _value);
			else if (_member_info.MemberType == MemberTypes.Property)
				(_member_info as PropertyInfo).SetValue(_parent_object, _value);
			else throw new NotSupportedException("Setting value of member which is not Field nor Property is not allowed");
		}

	}
}
