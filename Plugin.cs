using Jacobi.Vst.Core;
using Jacobi.Vst.Plugin.Framework;
using Jacobi.Vst.Plugin.Framework.Plugin;
using Microsoft.Extensions.DependencyInjection;
using Sinora.Dsp;

namespace Sinora
{
	/// <summary>
	/// The Plugin root object.
	/// </summary>
	internal sealed class Plugin : VstPluginWithServices
	{
		/// <summary>
		/// TODO: assign a unique plugin id.
		/// </summary>
		private static readonly int UniquePluginId = new FourCharacterCode("1234").ToInt32();
		private const string PluginName = "Sinora";
		private const string ProductName = "Rebmiami";
		private const string VendorName = "Rebmiami";
		/// <summary>
		/// TODO: assign a plugin version.
		/// </summary>
		private const int PluginVersion = 0000;
		private const VstPluginCategory PluginCategory = VstPluginCategory.Synth;
		/// <summary>
		/// TODO: what can your plugin do?
		/// </summary>
		private const VstPluginCapabilities PluginCapabilities = VstPluginCapabilities.ReceiveTimeInfo;
		/// <summary>
		/// The number of samples your plugin lags behind.
		/// </summary>
		private const int InitialDelayInSamples = 0;

		/// <summary>
		/// Initializes the one an only instance of the Plugin root object.
		/// </summary>
		public Plugin()
			: base(PluginName, UniquePluginId,
				new VstProductInfo(ProductName, VendorName, PluginVersion),
				PluginCategory, InitialDelayInSamples, PluginCapabilities)
		{ }

		/// <summary>
		/// Called once to get all the plugin components.
		/// Add components for the IVstXxxx interfaces you want to support.
		/// </summary>
		/// <param name="services">Is never null.</param>
		protected override void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<PluginParameters>()
				.AddSingletonAll<PluginPrograms>()
				.AddSingletonAll<AudioProcessor>()
				.AddSingletonAll<PluginEditor>();
		}
	}
}
