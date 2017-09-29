
// MIT License
// 
// Copyright (c) 2009-2017 Luca Piccioni
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// 
// This file is automatically generated

#pragma warning disable 649, 1572, 1573

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

using Khronos;

namespace OpenGL
{
	public partial class Gl
	{
		/// <summary>
		/// [GL] glGenQueryResourceTagNV: Binding for glGenQueryResourceTagNV.
		/// </summary>
		/// <param name="tagIds">
		/// A <see cref="T:Int32[]"/>.
		/// </param>
		[RequiredByFeature("GL_NV_query_resource_tag")]
		public static void GenQueryResourceTagNV(Int32[] tagIds)
		{
			unsafe {
				fixed (Int32* p_tagIds = tagIds)
				{
					Debug.Assert(Delegates.pglGenQueryResourceTagNV != null, "pglGenQueryResourceTagNV not implemented");
					Delegates.pglGenQueryResourceTagNV((Int32)tagIds.Length, p_tagIds);
					LogCommand("glGenQueryResourceTagNV", null, tagIds.Length, tagIds					);
				}
			}
			DebugCheckErrors(null);
		}

		/// <summary>
		/// [GL] glGenQueryResourceTagNV: Binding for glGenQueryResourceTagNV.
		/// </summary>
		[RequiredByFeature("GL_NV_query_resource_tag")]
		public static Int32 GenQueryResourceTagNV()
		{
			Int32[] retValue = new Int32[1];
			GenQueryResourceTagNV(retValue);
			return (retValue[0]);
		}

		/// <summary>
		/// [GL] glDeleteQueryResourceTagNV: Binding for glDeleteQueryResourceTagNV.
		/// </summary>
		/// <param name="tagIds">
		/// A <see cref="T:Int32[]"/>.
		/// </param>
		[RequiredByFeature("GL_NV_query_resource_tag")]
		public static void DeleteQueryResourceTagNV(Int32[] tagIds)
		{
			unsafe {
				fixed (Int32* p_tagIds = tagIds)
				{
					Debug.Assert(Delegates.pglDeleteQueryResourceTagNV != null, "pglDeleteQueryResourceTagNV not implemented");
					Delegates.pglDeleteQueryResourceTagNV((Int32)tagIds.Length, p_tagIds);
					LogCommand("glDeleteQueryResourceTagNV", null, tagIds.Length, tagIds					);
				}
			}
			DebugCheckErrors(null);
		}

		/// <summary>
		/// [GL] glQueryResourceTagNV: Binding for glQueryResourceTagNV.
		/// </summary>
		/// <param name="tagId">
		/// A <see cref="T:Int32"/>.
		/// </param>
		/// <param name="tagString">
		/// A <see cref="T:String"/>.
		/// </param>
		[RequiredByFeature("GL_NV_query_resource_tag")]
		public static void QueryResourceTagNV(Int32 tagId, String tagString)
		{
			Debug.Assert(Delegates.pglQueryResourceTagNV != null, "pglQueryResourceTagNV not implemented");
			Delegates.pglQueryResourceTagNV(tagId, tagString);
			LogCommand("glQueryResourceTagNV", null, tagId, tagString			);
			DebugCheckErrors(null);
		}

		internal unsafe static partial class Delegates
		{
			[RequiredByFeature("GL_NV_query_resource_tag")]
			[SuppressUnmanagedCodeSecurity()]
			internal unsafe delegate void glGenQueryResourceTagNV(Int32 n, Int32* tagIds);

			[RequiredByFeature("GL_NV_query_resource_tag")]
			[ThreadStatic]
			internal static glGenQueryResourceTagNV pglGenQueryResourceTagNV;

			[RequiredByFeature("GL_NV_query_resource_tag")]
			[SuppressUnmanagedCodeSecurity()]
			internal unsafe delegate void glDeleteQueryResourceTagNV(Int32 n, Int32* tagIds);

			[RequiredByFeature("GL_NV_query_resource_tag")]
			[ThreadStatic]
			internal static glDeleteQueryResourceTagNV pglDeleteQueryResourceTagNV;

			[RequiredByFeature("GL_NV_query_resource_tag")]
			[SuppressUnmanagedCodeSecurity()]
			internal delegate void glQueryResourceTagNV(Int32 tagId, String tagString);

			[RequiredByFeature("GL_NV_query_resource_tag")]
			[ThreadStatic]
			internal static glQueryResourceTagNV pglQueryResourceTagNV;

		}
	}

}