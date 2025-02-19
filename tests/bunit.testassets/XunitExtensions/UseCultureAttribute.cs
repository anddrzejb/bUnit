using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Xunit.Sdk;

namespace Xunit
{
	/// <summary>
	/// Apply this attribute to your test method to replace the
	/// <see cref="Thread.CurrentThread" /> <see cref="CultureInfo.CurrentCulture" /> and
	/// <see cref="CultureInfo.CurrentUICulture" /> with another culture.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	[SuppressMessage("Design", "CA1019:Define accessors for attribute arguments", Justification = "<Pending>")]
	public sealed class UseCultureAttribute : BeforeAfterTestAttribute
	{
		readonly Lazy<CultureInfo> culture;
		readonly Lazy<CultureInfo> uiCulture;

		CultureInfo originalCulture;
		CultureInfo originalUICulture;

		/// <summary>
		/// Replaces the culture and UI culture of the current thread with
		/// <paramref name="culture" />
		/// </summary>
		/// <param name="culture">The name of the culture.</param>
		/// <remarks>
		/// <para>
		/// This constructor overload uses <paramref name="culture" /> for both
		/// <see cref="Culture" /> and <see cref="UICulture" />.
		/// </para>
		/// </remarks>
		public UseCultureAttribute(string culture)
			: this(culture, culture) { }


		/// <summary>
		/// Replaces the culture and UI culture of the current thread with
		/// <paramref name="culture" /> and <paramref name="uiCulture" />
		/// </summary>
		/// <param name="culture">The name of the culture.</param>
		/// <param name="uiCulture">The name of the UI culture.</param>
		public UseCultureAttribute(string culture, string uiCulture)
		{
			this.culture = new Lazy<CultureInfo>(() => new CultureInfo(culture, false));
			this.uiCulture = new Lazy<CultureInfo>(() => new CultureInfo(uiCulture, false));
		}

		/// <summary>
		/// Gets the culture.
		/// </summary>
		public CultureInfo Culture { get { return culture.Value; } }

		/// <summary>
		/// Gets the UI culture.
		/// </summary>
		public CultureInfo UICulture { get { return uiCulture.Value; } }

		/// <summary>
		/// Stores the current <see cref="Thread.CurrentPrincipal" />
		/// <see cref="CultureInfo.CurrentCulture" /> and <see cref="CultureInfo.CurrentUICulture" />
		/// and replaces them with the new cultures defined in the constructor.
		/// </summary>
		/// <param name="methodUnderTest">The method under test</param>
		public override void Before(MethodInfo methodUnderTest)
		{
			originalCulture = Thread.CurrentThread.CurrentCulture;
			originalUICulture = Thread.CurrentThread.CurrentUICulture;

			Thread.CurrentThread.CurrentCulture = Culture;
			Thread.CurrentThread.CurrentUICulture = UICulture;

			CultureInfo.CurrentCulture.ClearCachedData();
			CultureInfo.CurrentUICulture.ClearCachedData();
		}

		/// <summary>
		/// Restores the original <see cref="CultureInfo.CurrentCulture" /> and
		/// <see cref="CultureInfo.CurrentUICulture" /> to <see cref="Thread.CurrentPrincipal" />
		/// </summary>
		/// <param name="methodUnderTest">The method under test</param>
		public override void After(MethodInfo methodUnderTest)
		{
			Thread.CurrentThread.CurrentCulture = originalCulture;
			Thread.CurrentThread.CurrentUICulture = originalUICulture;

			CultureInfo.CurrentCulture.ClearCachedData();
			CultureInfo.CurrentUICulture.ClearCachedData();
		}
	}
}
