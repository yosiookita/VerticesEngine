using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if __ANDROID__ 
using Android.App;

[assembly: AssemblyTitle("VerticesEngine.DROID")]
//// General Information about an assembly is controlled through the following 
//// set of attributes. Change these attribute values to modify the information
//// associated with an assembly.
//[assembly: AssemblyTitle("VerticesEngine.DROID")]
//[assembly: AssemblyDescription("")]
//[assembly: AssemblyConfiguration("")]
//[assembly: AssemblyCompany("")]
//[assembly: AssemblyProduct("VerticesEngine.DROID")]
//[assembly: AssemblyCopyright("Copyright ©  2016")]
//[assembly: AssemblyTrademark("")]
//[assembly: AssemblyCulture("")]
//[assembly: ComVisible(false)]

//// Version information for an assembly consists of the following four values:
////
////      Major Version
////      Minor Version 
////      Build Number
////      Revision
////
//// You can specify all the values or you can default the Build and Revision Numbers 
//// by using the '*' as shown below:
//// [assembly: AssemblyVersion("1.0.*")]
//[assembly: AssemblyVersion("0.8.*")]
//[assembly: AssemblyFileVersion("1.0.0.0")]



#else

[assembly: AssemblyTitle("VerticesEngine")]


// On Windows, the following GUID is for the ID of the typelib if this
// project is exposed to COM. On other platforms, it unique identifies the
// title storage container when deploying this assembly to the device.
[assembly: Guid("4fbf1f9c-2d6a-4c82-9311-1265c6731a8c")]
#endif
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyProduct("VerticesEngine")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyCompany("Virtex Edge Design")]
[assembly: AssemblyCopyright("Copyright ©  2019")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type. Only Windows
// assemblies support COM.
[assembly: ComVisible(false)]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
[assembly: AssemblyVersion("1.6.0.*")]