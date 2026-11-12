using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;

namespace MelissaData {
	/// <summary>Verifies, standardizes, and enhances U.S. and Canadian addresses</summary>
	/// <remarks>Provides DPV processing, CASS certification, LACSLink, SuiteLink, and RBDI capabilities</remarks>
	public class mdAddr : IDisposable {
		private IntPtr i;

		/// <summary>Programming, installation, and execution error codes returned by Initialize and InitializeDataFiles</summary>
		public enum ProgramStatus {
			/// <summary>No error - initialization was successful</summary>
			ErrorNone = 0,
			/// <summary>Could not open the mdAddr.dat file</summary>
			ErrorOther = 1,
			/// <summary>Out of memory</summary>
			ErrorOutOfMemory = 2,
			/// <summary>A required data file could not be found</summary>
			ErrorRequiredFileNotFound = 3,
			/// <summary>Conflicts found between two or more data files</summary>
			ErrorFoundOldFile = 4,
			/// <summary>The database has expired</summary>
			ErrorDatabaseExpired = 5,
			/// <summary>The license has expired</summary>
			ErrorLicenseExpired = 6
		}
		/// <summary>Controls whether diacritic characters are returned in the standardized output, set via SetDiacritics</summary>
		public enum DiacriticsMode {
			/// <summary>If the input value contains diacritic characters, diacritics will be returned in the output. This is the default setting</summary>
			Auto = 0,
			/// <summary>Diacritics will be returned in the output for all addresses, regardless of input</summary>
			On = 1,
			/// <summary>Diacritics will not be returned in the output for any addresses, regardless of input</summary>
			Off = 2
		}
		/// <summary>Controls how street directionals and suffixes are abbreviated in standardized output, set via SetStandardizationType</summary>
		public enum StandardizeMode {
			/// <summary>The street address is returned with directionals and suffixes shortened according to postal standards. This is the default setting</summary>
			ShortFormat = 0,
			/// <summary>The street address is returned with directionals and suffixes spelled out rather than abbreviated</summary>
			LongFormat = 1,
			/// <summary>The street address is returned with the abbreviation or non-abbreviation of the directionals and suffixes preserved. If any of these components need to be corrected and replaced, the replacement will preserve the original standardization</summary>
			AutoFormat = 2
		}
		/// <summary>Controls whether suite information is parsed into its own field or combined with the address line, set via SetSuiteParseMode</summary>
		public enum SuiteParseMode {
			/// <summary>The suite will be parsed out into the suite field after VerifyAddress(). This is the default setting</summary>
			ParseSuite = 0,
			/// <summary>The suite will be appended to the end of the Address1 line after VerifyAddress()</summary>
			CombineSuite = 1
		}
		/// <summary>Controls whether nickname or former street name inputs are converted to their USPS preferred name, set via SetAliasMode</summary>
		public enum AliasPreserveMode {
			/// <summary>Nickname or former name street inputs will be converted to their USPS preferred street name during the verification process. This is the default setting</summary>
			ConvertAlias = 0,
			/// <summary>Nickname or former name street inputs will be preserved during the verification process</summary>
			PreserveAlias = 1
		}
		/// <summary>Controls the verbosity of the description returned by GetResultCodeDescription</summary>
		public enum ResultCdDescOpt {
			/// <summary>Return long (detailed) result code descriptions. This is the default setting</summary>
			ResultCodeDescriptionLong = 0,
			/// <summary>Return short (brief) result code descriptions</summary>
			ResultCodeDescriptionShort = 1
		}

		[SuppressUnmanagedCodeSecurity]
		private class mdAddrUnmanaged {
			[DllImport("mdAddr", EntryPoint = "mdAddrCreate", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrCreate();
			[DllImport("mdAddr", EntryPoint = "mdAddrDestroy", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrDestroy(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrInitialize", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrInitialize(IntPtr i, IntPtr DataPath, IntPtr NationalPath, IntPtr RegionalPath);
			[DllImport("mdAddr", EntryPoint = "mdAddrInitializeDataFiles", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrInitializeDataFiles(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetInitializeErrorString", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetInitializeErrorString(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetLicenseString", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrSetLicenseString(IntPtr i, IntPtr License);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetBuildNumber", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetBuildNumber(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetDatabaseDate", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetDatabaseDate(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetExpirationDate", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetExpirationDate(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetLicenseExpirationDate", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetLicenseExpirationDate(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetCanadianDatabaseDate", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetCanadianDatabaseDate(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetCanadianExpirationDate", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetCanadianExpirationDate(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetPathToUSFiles", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetPathToUSFiles(IntPtr i, IntPtr DataFilePath);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetPathToCanadaFiles", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetPathToCanadaFiles(IntPtr i, IntPtr DataFilePath);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetPathToDPVDataFiles", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetPathToDPVDataFiles(IntPtr i, IntPtr DataFilePath);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetPathToLACSLinkDataFiles", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetPathToLACSLinkDataFiles(IntPtr i, IntPtr DataFilePath);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetPathToSuiteLinkDataFiles", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetPathToSuiteLinkDataFiles(IntPtr i, IntPtr DataFilePath);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetPathToSuiteFinderDataFiles", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetPathToSuiteFinderDataFiles(IntPtr i, IntPtr DataFilePath);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetPathToRBDIFiles", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetPathToRBDIFiles(IntPtr i, IntPtr DataFilePath);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetRBDIDatabaseDate", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetRBDIDatabaseDate(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetPathToAddrKeyDataFiles", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetPathToAddrKeyDataFiles(IntPtr i, IntPtr DataFilePath);
			[DllImport("mdAddr", EntryPoint = "mdAddrClearProperties", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrClearProperties(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrResetDPV", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrResetDPV(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetCASSEnable", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetCASSEnable(IntPtr i, Int32 Enable);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetUseUSPSPreferredCityNames", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetUseUSPSPreferredCityNames(IntPtr i, Int32 Enable);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetDiacritics", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetDiacritics(IntPtr i, Int32 mode);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetStatusCode", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetStatusCode(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetErrorCode", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetErrorCode(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetErrorString", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetErrorString(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetResults", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetResults(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetResultCodeDescription", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetResultCodeDescription(IntPtr i, IntPtr resultCode, Int32 opt);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetPS3553_B1_ProcessorName", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetPS3553_B1_ProcessorName(IntPtr i, IntPtr ProcessorName);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetPS3553_B4_ListName", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetPS3553_B4_ListName(IntPtr i, IntPtr ListName);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetPS3553_D3_Name", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetPS3553_D3_Name(IntPtr i, IntPtr Name);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetPS3553_D3_Company", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetPS3553_D3_Company(IntPtr i, IntPtr Company);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetPS3553_D3_Address", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetPS3553_D3_Address(IntPtr i, IntPtr Address);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetPS3553_D3_City", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetPS3553_D3_City(IntPtr i, IntPtr City);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetPS3553_D3_State", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetPS3553_D3_State(IntPtr i, IntPtr State);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetPS3553_D3_ZIP", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetPS3553_D3_ZIP(IntPtr i, IntPtr ZIP);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetFormPS3553", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetFormPS3553(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrSaveFormPS3553", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrSaveFormPS3553(IntPtr i, IntPtr PathAndFileName);
			[DllImport("mdAddr", EntryPoint = "mdAddrResetFormPS3553", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrResetFormPS3553(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrResetFormPS3553Counter", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrResetFormPS3553Counter(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetStandardizationType", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetStandardizationType(IntPtr i, Int32 mode);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetSuiteParseMode", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetSuiteParseMode(IntPtr i, Int32 mode);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetAliasMode", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetAliasMode(IntPtr i, Int32 mode);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetFormSOA", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetFormSOA(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrSaveFormSOA", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSaveFormSOA(IntPtr i, IntPtr PathAndFileName);
			[DllImport("mdAddr", EntryPoint = "mdAddrResetFormSOA", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrResetFormSOA(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetSOACustomerInfo", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetSOACustomerInfo(IntPtr i, IntPtr customerName, IntPtr customerAddress);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetSOACPCNumber", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetSOACPCNumber(IntPtr i, IntPtr CPCNumber);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetSOACustomerInfo", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetSOACustomerInfo(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetSOACPCNumber", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetSOACPCNumber(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetSOATotalRecords", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetSOATotalRecords(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetSOAAAPercentage", CallingConvention = CallingConvention.Cdecl)]
			public static extern float mdAddrGetSOAAAPercentage(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetSOAAAExpiryDate", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetSOAAAExpiryDate(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetSOASoftwareInfo", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetSOASoftwareInfo(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetSOAErrorString", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetSOAErrorString(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetCompany", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetCompany(IntPtr i, IntPtr Company);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetLastName", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetLastName(IntPtr i, IntPtr LastName);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetAddress", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetAddress(IntPtr i, IntPtr Address);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetAddress2", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetAddress2(IntPtr i, IntPtr Address2);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetLastLine", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetLastLine(IntPtr i, IntPtr LastLine);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetSuite", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetSuite(IntPtr i, IntPtr Suite);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetCity", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetCity(IntPtr i, IntPtr City);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetState", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetState(IntPtr i, IntPtr State);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetZip", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetZip(IntPtr i, IntPtr Zip);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetPlus4", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetPlus4(IntPtr i, IntPtr Plus4);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetUrbanization", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetUrbanization(IntPtr i, IntPtr Urbanization);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetParsedAddressRange", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetParsedAddressRange(IntPtr i, IntPtr AddressRange);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetParsedPreDirection", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetParsedPreDirection(IntPtr i, IntPtr PreDirection);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetParsedStreetName", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetParsedStreetName(IntPtr i, IntPtr StreetName);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetParsedSuffix", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetParsedSuffix(IntPtr i, IntPtr Suffix);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetParsedPostDirection", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetParsedPostDirection(IntPtr i, IntPtr PostDirection);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetParsedSuiteName", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetParsedSuiteName(IntPtr i, IntPtr SuiteName);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetParsedSuiteRange", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetParsedSuiteRange(IntPtr i, IntPtr SuiteRange);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetParsedRouteService", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetParsedRouteService(IntPtr i, IntPtr RouteService);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetParsedLockBox", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetParsedLockBox(IntPtr i, IntPtr LockBox);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetParsedDeliveryInstallation", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetParsedDeliveryInstallation(IntPtr i, IntPtr DeliveryInstallation);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetCountryCode", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetCountryCode(IntPtr i, IntPtr CountryCode);
			[DllImport("mdAddr", EntryPoint = "mdAddrVerifyAddress", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrVerifyAddress(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetCompany", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetCompany(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetLastName", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetLastName(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetAddress", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetAddress(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetAddress2", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetAddress2(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetSuite", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetSuite(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetCity", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetCity(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetCityAbbreviation", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetCityAbbreviation(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetState", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetState(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetZip", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetZip(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPlus4", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetPlus4(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetCarrierRoute", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetCarrierRoute(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetDeliveryPointCode", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetDeliveryPointCode(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetDeliveryPointCheckDigit", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetDeliveryPointCheckDigit(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetCountyFips", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetCountyFips(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetCountyName", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetCountyName(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetAddressTypeCode", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetAddressTypeCode(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetAddressTypeString", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetAddressTypeString(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetUrbanization", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetUrbanization(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetCongressionalDistrict", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetCongressionalDistrict(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetLACS", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetLACS(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetLACSLinkIndicator", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetLACSLinkIndicator(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPrivateMailbox", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetPrivateMailbox(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetTimeZoneCode", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetTimeZoneCode(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetTimeZone", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetTimeZone(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetMsa", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetMsa(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPmsa", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetPmsa(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetDefaultFlagIndicator", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetDefaultFlagIndicator(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetSuiteStatus", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetSuiteStatus(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetEWSFlag", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetEWSFlag(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetCMRA", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetCMRA(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetDsfVacant", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetDsfVacant(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetCountryCode", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetCountryCode(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetZipType", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetZipType(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetFalseTable", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetFalseTable(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetDPVFootnotes", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetDPVFootnotes(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetLACSLinkReturnCode", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetLACSLinkReturnCode(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetSuiteLinkReturnCode", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetSuiteLinkReturnCode(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetRBDI", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetRBDI(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetELotNumber", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetELotNumber(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetELotOrder", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetELotOrder(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetAddressKey", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetAddressKey(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetMelissaAddressKey", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetMelissaAddressKey(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetMelissaAddressKeyBase", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetMelissaAddressKeyBase(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetOutputParameter", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetOutputParameter(IntPtr i, IntPtr ParameterName);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetInputParameter", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrSetInputParameter(IntPtr i, IntPtr ParameterName, IntPtr Value);
			[DllImport("mdAddr", EntryPoint = "mdAddrFindSuggestion", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrFindSuggestion(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrFindSuggestionNext", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrFindSuggestionNext(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetDsfNoStats", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetDsfNoStats(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetDsfDNA", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetDsfDNA(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_B6_TotalRecords", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_B6_TotalRecords(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_C1a_ZIP4Coded", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_C1a_ZIP4Coded(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_C1c_DPBCAssigned", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_C1c_DPBCAssigned(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_C1d_FiveDigitCoded", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_C1d_FiveDigitCoded(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_C1e_CRRTCoded", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_C1e_CRRTCoded(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_C1f_eLOTAssigned", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_C1f_eLOTAssigned(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_E_HighRiseDefault", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_E_HighRiseDefault(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_E_HighRiseExact", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_E_HighRiseExact(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_E_RuralRouteDefault", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_E_RuralRouteDefault(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_E_RuralRouteExact", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_E_RuralRouteExact(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetZip4HRDefault", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetZip4HRDefault(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetZip4HRExact", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetZip4HRExact(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetZip4RRDefault", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetZip4RRDefault(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetZip4RRExact", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetZip4RRExact(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_E_LACSCount", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_E_LACSCount(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_E_EWSCount", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_E_EWSCount(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_E_DPVCount", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_E_DPVCount(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_X_POBoxCount", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_X_POBoxCount(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_X_HCExactCount", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_X_HCExactCount(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_X_FirmCount", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_X_FirmCount(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_X_GenDeliveryCount", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_X_GenDeliveryCount(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_X_MilitaryZipCount", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_X_MilitaryZipCount(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_X_NonDeliveryCount", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_X_NonDeliveryCount(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_X_StreetCount", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_X_StreetCount(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_X_HCDefaultCount", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_X_HCDefaultCount(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_X_OtherCount", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_X_OtherCount(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_X_LacsLinkCodeACount", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_X_LacsLinkCodeACount(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_X_LacsLinkCode00Count", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_X_LacsLinkCode00Count(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_X_LacsLinkCode14Count", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_X_LacsLinkCode14Count(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_X_LacsLinkCode92Count", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_X_LacsLinkCode92Count(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_X_LacsLinkCode09Count", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_X_LacsLinkCode09Count(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_X_SuiteLinkCodeACount", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_X_SuiteLinkCodeACount(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetPS3553_X_SuiteLinkCode00Count", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdAddrGetPS3553_X_SuiteLinkCode00Count(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetParsedAddressRange", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetParsedAddressRange(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetParsedPreDirection", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetParsedPreDirection(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetParsedStreetName", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetParsedStreetName(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetParsedSuffix", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetParsedSuffix(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetParsedPostDirection", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetParsedPostDirection(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetParsedSuiteName", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetParsedSuiteName(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetParsedSuiteRange", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetParsedSuiteRange(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetParsedPrivateMailboxName", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetParsedPrivateMailboxName(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetParsedPrivateMailboxNumber", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetParsedPrivateMailboxNumber(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetParsedGarbage", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetParsedGarbage(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetParsedRouteService", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetParsedRouteService(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetParsedLockBox", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetParsedLockBox(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetParsedDeliveryInstallation", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetParsedDeliveryInstallation(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdAddrSetReserved", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdAddrSetReserved(IntPtr i, IntPtr Key, IntPtr Value);
			[DllImport("mdAddr", EntryPoint = "mdAddrGetReserved", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdAddrGetReserved(IntPtr i, IntPtr Key);
		}

		/// <summary>Constructs a new AddressCheck instance</summary>
		public mdAddr() {
			i = mdAddrUnmanaged.mdAddrCreate();
		}

		/// <summary>Destroys an existing AddressCheck instance</summary>
		~mdAddr() {
			Dispose();
		}

		public virtual void Dispose() {
			lock (this) {
				mdAddrUnmanaged.mdAddrDestroy(i);
				GC.SuppressFinalize(this);
			}
		}

		/// <summary>Deprecated; use InitializeDataFiles instead. Initializes the data files using explicit file paths</summary>
		/// <param name="DataPath">Path to the mdAddr.dat file</param>
		/// <param name="NationalPath">Path to the mdAddr.nat file</param>
		/// <param name="RegionalPath">Path to a regional data file</param>
		/// <returns>ProgramStatus enum - ErrorNone (0) on success; use GetInitializeErrorString to retrieve the nature of any other error</returns>
	[Obsolete("Use InitializeDataFiles instead.")]
		public ProgramStatus Initialize(string DataPath, string NationalPath, string RegionalPath) {
			EncodedString u_DataPath = new EncodedString(DataPath);
			EncodedString u_NationalPath = new EncodedString(NationalPath);
			EncodedString u_RegionalPath = new EncodedString(RegionalPath);
			return (ProgramStatus)mdAddrUnmanaged.mdAddrInitialize(i, u_DataPath.GetPtr(), u_NationalPath.GetPtr(), u_RegionalPath.GetPtr());
		}

		/// <summary>Opens the needed data files and prepares Address Object for use. Requires SetPathToUSFiles and/or SetPathToCanadaFiles to have been called first</summary>
		/// <returns>ProgramStatus enum - NoError (0) on success; use GetInitializeErrorString to retrieve the nature of any other error</returns>
		public ProgramStatus InitializeDataFiles() {
			return (ProgramStatus)mdAddrUnmanaged.mdAddrInitializeDataFiles(i);
		}

		/// <summary>Returns a description of the error from the last InitializeDataFiles call</summary>
		/// <returns>Error description string</returns>
		public string GetInitializeErrorString() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetInitializeErrorString(i));
		}

		/// <summary>Sets the license string required to enable AddressCheck's complete functionality</summary>
		/// <param name="License">The software license key</param>
		/// <returns>True (1) if the specified license is correct</returns>
		public bool SetLicenseString(string License) {
			EncodedString u_License = new EncodedString(License);
			return (mdAddrUnmanaged.mdAddrSetLicenseString(i, u_License.GetPtr()) != 0);
		}

		/// <summary>Returns the current development release build number of Address Object</summary>
		/// <returns>The current release build number of Address Object</returns>
		public string GetBuildNumber() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetBuildNumber(i));
		}

		/// <summary>Returns the date of the US address data files</summary>
		/// <returns>The US address database date in the format yyyy-mm-dd</returns>
		public string GetDatabaseDate() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetDatabaseDate(i));
		}

		/// <summary>Returns the date when the US address data files will expire</summary>
		/// <returns>The US address expiration date in the format yyyy-mm-dd</returns>
		public string GetExpirationDate() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetExpirationDate(i));
		}

		/// <summary>Returns the expiration date of the current license string</summary>
		/// <returns>The license expiration date in the format yyyy-mm-dd</returns>
		public string GetLicenseExpirationDate() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetLicenseExpirationDate(i));
		}

		/// <summary>Returns the date of the Canadian address data files</summary>
		/// <returns>The Canadian address database date in the format yyyy-mm-dd</returns>
		public string GetCanadianDatabaseDate() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetCanadianDatabaseDate(i));
		}

		/// <summary>Returns the date when the Canadian address data files will expire</summary>
		/// <returns>The Canadian address expiration date in the format yyyy-mm-dd</returns>
		public string GetCanadianExpirationDate() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetCanadianExpirationDate(i));
		}

		/// <summary>Sets the path to the US data files required for US address verification</summary>
		/// <param name="DataFilePath">Path to directory containing mdAddr.dat, mdAddr.nat, mdAddr.str, and mdAddr.lic</param>
		public void SetPathToUSFiles(string DataFilePath) {
			EncodedString u_DataFilePath = new EncodedString(DataFilePath);
			mdAddrUnmanaged.mdAddrSetPathToUSFiles(i, u_DataFilePath.GetPtr());
		}

		/// <summary>Sets the path to the Canadian data files required for Canadian address verification</summary>
		/// <param name="DataFilePath">Path to directory containing mdCanada3.db</param>
		public void SetPathToCanadaFiles(string DataFilePath) {
			EncodedString u_DataFilePath = new EncodedString(DataFilePath);
			mdAddrUnmanaged.mdAddrSetPathToCanadaFiles(i, u_DataFilePath.GetPtr());
		}

		/// <summary>Sets the path to the DPV data files required for CASS processing</summary>
		/// <param name="DataFilePath">Path to DPV data files (dph.hsa, dph.hsx, dph.hsv, dph.hsf, dph.hsc, dph.dte, lcd)</param>
		public void SetPathToDPVDataFiles(string DataFilePath) {
			EncodedString u_DataFilePath = new EncodedString(DataFilePath);
			mdAddrUnmanaged.mdAddrSetPathToDPVDataFiles(i, u_DataFilePath.GetPtr());
		}

		/// <summary>Sets the path to the LACSLink data files required for CASS processing</summary>
		/// <param name="DataFilePath">Path to directory containing mdLACS.dat</param>
		public void SetPathToLACSLinkDataFiles(string DataFilePath) {
			EncodedString u_DataFilePath = new EncodedString(DataFilePath);
			mdAddrUnmanaged.mdAddrSetPathToLACSLinkDataFiles(i, u_DataFilePath.GetPtr());
		}

		/// <summary>Sets the path to the SuiteLink data files required for CASS processing</summary>
		/// <param name="DataFilePath">Path to directory containing mdSteLink.dat</param>
		public void SetPathToSuiteLinkDataFiles(string DataFilePath) {
			EncodedString u_DataFilePath = new EncodedString(DataFilePath);
			mdAddrUnmanaged.mdAddrSetPathToSuiteLinkDataFiles(i, u_DataFilePath.GetPtr());
		}

		/// <summary>Sets the path to the AddressPlus suite finder data files</summary>
		/// <param name="DataFilePath">Path to directory containing mdSuiteFinder.db</param>
		public void SetPathToSuiteFinderDataFiles(string DataFilePath) {
			EncodedString u_DataFilePath = new EncodedString(DataFilePath);
			mdAddrUnmanaged.mdAddrSetPathToSuiteFinderDataFiles(i, u_DataFilePath.GetPtr());
		}

		/// <summary>Sets the path to the RBDI (Residential Business Delivery Indicator) data files</summary>
		/// <param name="DataFilePath">Path to directory containing mdRBDI.dat</param>
		public void SetPathToRBDIFiles(string DataFilePath) {
			EncodedString u_DataFilePath = new EncodedString(DataFilePath);
			mdAddrUnmanaged.mdAddrSetPathToRBDIFiles(i, u_DataFilePath.GetPtr());
		}

		/// <summary>Returns the date of the RBDI data files</summary>
		/// <returns>RBDI database date string</returns>
		public string GetRBDIDatabaseDate() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetRBDIDatabaseDate(i));
		}

		/// <summary>Sets the path to the Melissa Address Key (MAK) data files</summary>
		/// <param name="DataFilePath">Path to the MAK data files</param>
		public void SetPathToAddrKeyDataFiles(string DataFilePath) {
			EncodedString u_DataFilePath = new EncodedString(DataFilePath);
			mdAddrUnmanaged.mdAddrSetPathToAddrKeyDataFiles(i, u_DataFilePath.GetPtr());
		}

		/// <summary>Clears all stored address input and output values before processing the next address</summary>
		public void ClearProperties() {
			mdAddrUnmanaged.mdAddrClearProperties(i);
		}

		/// <summary>Resets internal DPV counters</summary>
		public void ResetDPV() {
			mdAddrUnmanaged.mdAddrResetDPV(i);
		}

		/// <summary>Enables or disables full CASS Certified address verification including DPV processing</summary>
		/// <param name="Enable">Pass 1 to enable CASS processing; 0 to disable (default is disabled)</param>
		public void SetCASSEnable(int Enable) {
			mdAddrUnmanaged.mdAddrSetCASSEnable(i, Enable);
		}

		/// <summary>Forces VerifyAddress to always return the USPS preferred city name regardless of vanity names</summary>
		/// <param name="Enable">Pass 1 to always return the USPS preferred city name</param>
		public void SetUseUSPSPreferredCityNames(int Enable) {
			mdAddrUnmanaged.mdAddrSetUseUSPSPreferredCityNames(i, Enable);
		}

		/// <summary>Sets how diacritic characters are handled for Quebec addresses</summary>
		/// <param name="mode">DiacriticsMode enum - Auto (0), On (1), or Off (2)</param>
		public void SetDiacritics(DiacriticsMode mode) {
			mdAddrUnmanaged.mdAddrSetDiacritics(i, (int)mode);
		}

		/// <summary>Returns the status code from the last VerifyAddress call</summary>
		/// <returns>Status code string</returns>
	[Obsolete("Use GetResults instead.")]
		public string GetStatusCode() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetStatusCode(i));
		}

		/// <summary>Returns the error code from the last VerifyAddress call</summary>
		/// <returns>Error code string</returns>
	[Obsolete("Use GetResults instead.")]
		public string GetErrorCode() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetErrorCode(i));
		}

		/// <summary>Returns a description of the error from the last VerifyAddress call</summary>
		/// <returns>Error description string</returns>
	[Obsolete("Use GetResults instead.")]
		public string GetErrorString() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetErrorString(i));
		}

		/// <summary>Returns a comma-delimited list of result codes describing the outcome of the last VerifyAddress call</summary>
		/// <returns>Comma-delimited result code string (e.g. "AS01" for a fully verified address)</returns>
		public string GetResults() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetResults(i));
		}

		/// <summary>Returns a human-readable description for a given result code</summary>
		/// <param name="resultCode">The four-character result code to look up</param>
		/// <param name="opt">ResultCdDescOpt enum specifying description format (default is long)</param>
		/// <returns>Description of the result code</returns>
		public string GetResultCodeDescription(string resultCode, ResultCdDescOpt opt) {
			EncodedString u_resultCode = new EncodedString(resultCode);
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetResultCodeDescription(i, u_resultCode.GetPtr(), (int)opt));
		}

		/// <summary>Returns a human-readable description for a given result code</summary>
		/// <param name="resultCode">The four-character result code to look up</param>
		/// <returns>Description of the result code</returns>
		public string GetResultCodeDescription(string resultCode) {
			EncodedString u_resultCode = new EncodedString(resultCode);
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetResultCodeDescription(i, u_resultCode.GetPtr(), (int)ResultCdDescOpt.ResultCodeDescriptionLong));
		}

		/// <summary>Sets the name of the company that processed the address list for CASS Form 3553 item B1</summary>
		/// <param name="ProcessorName">Company name that coded the address list</param>
		public void SetPS3553_B1_ProcessorName(string ProcessorName) {
			EncodedString u_ProcessorName = new EncodedString(ProcessorName);
			mdAddrUnmanaged.mdAddrSetPS3553_B1_ProcessorName(i, u_ProcessorName.GetPtr());
		}

		/// <summary>Sets the name or ID of the address list for CASS Form 3553 item B4</summary>
		/// <param name="ListName">List name or ID number (prefix IDs with "ID#")</param>
		public void SetPS3553_B4_ListName(string ListName) {
			EncodedString u_ListName = new EncodedString(ListName);
			mdAddrUnmanaged.mdAddrSetPS3553_B4_ListName(i, u_ListName.GetPtr());
		}

		/// <summary>Sets the list owner's name for CASS Form 3553 item D3</summary>
		/// <param name="Name">Name of the list owner</param>
		public void SetPS3553_D3_Name(string Name) {
			EncodedString u_Name = new EncodedString(Name);
			mdAddrUnmanaged.mdAddrSetPS3553_D3_Name(i, u_Name.GetPtr());
		}

		/// <summary>Sets the list owner's company name for CASS Form 3553 item D3</summary>
		/// <param name="Company">Company name of the list owner</param>
		public void SetPS3553_D3_Company(string Company) {
			EncodedString u_Company = new EncodedString(Company);
			mdAddrUnmanaged.mdAddrSetPS3553_D3_Company(i, u_Company.GetPtr());
		}

		/// <summary>Sets the list owner's street address for CASS Form 3553 item D3</summary>
		/// <param name="Address">Street address of the list owner</param>
		public void SetPS3553_D3_Address(string Address) {
			EncodedString u_Address = new EncodedString(Address);
			mdAddrUnmanaged.mdAddrSetPS3553_D3_Address(i, u_Address.GetPtr());
		}

		/// <summary>Sets the list owner's city for CASS Form 3553 item D3</summary>
		/// <param name="City">City of the list owner</param>
		public void SetPS3553_D3_City(string City) {
			EncodedString u_City = new EncodedString(City);
			mdAddrUnmanaged.mdAddrSetPS3553_D3_City(i, u_City.GetPtr());
		}

		/// <summary>Sets the list owner's state for CASS Form 3553 item D3</summary>
		/// <param name="State">State of the list owner</param>
		public void SetPS3553_D3_State(string State) {
			EncodedString u_State = new EncodedString(State);
			mdAddrUnmanaged.mdAddrSetPS3553_D3_State(i, u_State.GetPtr());
		}

		/// <summary>Sets the list owner's ZIP code for CASS Form 3553 item D3</summary>
		/// <param name="ZIP">ZIP code of the list owner</param>
		public void SetPS3553_D3_ZIP(string ZIP) {
			EncodedString u_ZIP = new EncodedString(ZIP);
			mdAddrUnmanaged.mdAddrSetPS3553_D3_ZIP(i, u_ZIP.GetPtr());
		}

		/// <summary>Returns the CASS Form 3553 as an HTML string for display or printing</summary>
		/// <returns>HTML string of the CASS Form 3553</returns>
		public string GetFormPS3553() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetFormPS3553(i));
		}

		/// <summary>Saves the CASS Form 3553 as an HTML or PDF file</summary>
		/// <param name="PathAndFileName">Full path and filename including .html or .pdf extension</param>
		/// <returns>True (1) if saved successfully</returns>
		public bool SaveFormPS3553(string PathAndFileName) {
			EncodedString u_PathAndFileName = new EncodedString(PathAndFileName);
			return (mdAddrUnmanaged.mdAddrSaveFormPS3553(i, u_PathAndFileName.GetPtr()) != 0);
		}

		/// <summary>Resets all internal counters to zero, useful when producing several CASS forms in sequence for multiple jobs. Removing Address Object from memory also clears the counters</summary>
		public void ResetFormPS3553() {
			mdAddrUnmanaged.mdAddrResetFormPS3553(i);
		}

		/// <summary>Resets only the CASS Form 3553 record counter</summary>
		public void ResetFormPS3553Counter() {
			mdAddrUnmanaged.mdAddrResetFormPS3553Counter(i);
		}

		/// <summary>Sets how address suffixes and directionals are abbreviated in standardized output</summary>
		/// <param name="mode">StandardizeMode enum - ShortFormat (0), LongFormat (1), or AutoFormat (2)</param>
		public void SetStandardizationType(StandardizeMode mode) {
			mdAddrUnmanaged.mdAddrSetStandardizationType(i, (int)mode);
		}

		/// <summary>Controls whether suite information is parsed into its own field or appended to the address line</summary>
		/// <param name="mode">SuiteParseMode enum - ParseSuite (default) or CombineSuite</param>
		public void SetSuiteParseMode(SuiteParseMode mode) {
			mdAddrUnmanaged.mdAddrSetSuiteParseMode(i, (int)mode);
		}

		/// <summary>Controls whether street alias or nickname names are converted to the USPS preferred name or preserved</summary>
		/// <param name="mode">AliasPreserveMode enum - ConvertAlias (default) or PreserveAlias</param>
		public void SetAliasMode(AliasPreserveMode mode) {
			mdAddrUnmanaged.mdAddrSetAliasMode(i, (int)mode);
		}

		/// <summary>Returns the Canada Post Summary of Addresses as an HTML string</summary>
		/// <returns>HTML string of the Summary of Addresses</returns>
		public string GetFormSOA() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetFormSOA(i));
		}

		/// <summary>Saves the Canada Post Summary of Addresses to an HTML file</summary>
		/// <param name="PathAndFileName">Full path and filename including .html extension</param>
		public void SaveFormSOA(string PathAndFileName) {
			EncodedString u_PathAndFileName = new EncodedString(PathAndFileName);
			mdAddrUnmanaged.mdAddrSaveFormSOA(i, u_PathAndFileName.GetPtr());
		}

		/// <summary>Resets the SOA running totals for Canadian address records</summary>
		public void ResetFormSOA() {
			mdAddrUnmanaged.mdAddrResetFormSOA(i);
		}

		/// <summary>Sets the company name and mailing address for the Canada Post Summary of Addresses</summary>
		/// <param name="customerName">Company name as it appears on the Canada Post contract</param>
		/// <param name="customerAddress">Mailing address as it appears on the Canada Post contract</param>
		public void SetSOACustomerInfo(string customerName, string customerAddress) {
			EncodedString u_customerName = new EncodedString(customerName);
			EncodedString u_customerAddress = new EncodedString(customerAddress);
			mdAddrUnmanaged.mdAddrSetSOACustomerInfo(i, u_customerName.GetPtr(), u_customerAddress.GetPtr());
		}

		/// <summary>Sets the Canada Post Contract (CPC) number for the Summary of Addresses</summary>
		/// <param name="CPCNumber">Eight-digit Canada Post contract number</param>
		public void SetSOACPCNumber(string CPCNumber) {
			EncodedString u_CPCNumber = new EncodedString(CPCNumber);
			mdAddrUnmanaged.mdAddrSetSOACPCNumber(i, u_CPCNumber.GetPtr());
		}

		/// <summary>Returns the company name and mailing address set via SetSOACustomerInfo for the Summary of Addresses</summary>
		/// <returns>Customer name and address string</returns>
		public string GetSOACustomerInfo() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetSOACustomerInfo(i));
		}

		/// <summary>Returns the Canada Post Contract (CPC) number set via SetSOACPCNumber for the Summary of Addresses</summary>
		/// <returns>CPC number string</returns>
		public string GetSOACPCNumber() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetSOACPCNumber(i));
		}

		/// <summary>Returns the total number of Canadian address records processed since initialization or last reset</summary>
		/// <returns>Total record count</returns>
		public int GetSOATotalRecords() {
			return mdAddrUnmanaged.mdAddrGetSOATotalRecords(i);
		}

		/// <summary>Returns the address accuracy percentage for Canadian records processed</summary>
		/// <returns>Address accuracy percentage (must be 95% or higher for postage discounts)</returns>
		public float GetSOAAAPercentage() {
			return mdAddrUnmanaged.mdAddrGetSOAAAPercentage(i);
		}

		/// <summary>Returns the expiry date of the current Summary of Addresses</summary>
		/// <returns>SOA expiry date string</returns>
		public string GetSOAAAExpiryDate() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetSOAAAExpiryDate(i));
		}

		/// <summary>Returns the software name and build number shown on the Summary of Addresses</summary>
		/// <returns>Software name and version string</returns>
		public string GetSOASoftwareInfo() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetSOASoftwareInfo(i));
		}

		/// <summary>Returns an error description generated by any SOA function call</summary>
		/// <returns>SOA error description string</returns>
		public string GetSOAErrorString() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetSOAErrorString(i));
		}

		/// <summary>Sets the company name for the address to be verified</summary>
		/// <param name="Company">Optional company name; improves Plus4 accuracy and enables SuiteLink for firm addresses</param>
		public void SetCompany(string Company) {
			EncodedString u_Company = new EncodedString(Company);
			mdAddrUnmanaged.mdAddrSetCompany(i, u_Company.GetPtr());
		}

		/// <summary>Sets the last name associated with a residential address (required for AddressPlus)</summary>
		/// <param name="LastName">Last name associated with the address</param>
		public void SetLastName(string LastName) {
			EncodedString u_LastName = new EncodedString(LastName);
			mdAddrUnmanaged.mdAddrSetLastName(i, u_LastName.GetPtr());
		}

		/// <summary>Sets the primary delivery address line to be verified</summary>
		/// <param name="Address">Street address including number, name, directionals, and suffix</param>
		public void SetAddress(string Address) {
			EncodedString u_Address = new EncodedString(Address);
			mdAddrUnmanaged.mdAddrSetAddress(i, u_Address.GetPtr());
		}

		/// <summary>Sets a secondary address line such as a PO Box or suite on a separate line</summary>
		/// <param name="Address2">Secondary address line</param>
		public void SetAddress2(string Address2) {
			EncodedString u_Address2 = new EncodedString(Address2);
			mdAddrUnmanaged.mdAddrSetAddress2(i, u_Address2.GetPtr());
		}

		/// <summary>Sets the city, state, and ZIP code as a single combined string</summary>
		/// <param name="LastLine">Combined city, state, and ZIP string (e.g. "Rancho Santa Margarita, CA 92688")</param>
		public void SetLastLine(string LastLine) {
			EncodedString u_LastLine = new EncodedString(LastLine);
			mdAddrUnmanaged.mdAddrSetLastLine(i, u_LastLine.GetPtr());
		}

		/// <summary>Sets the suite name and number for the address to be verified</summary>
		/// <param name="Suite">Suite designation (e.g. "Apt 3" or "Ste 100")</param>
		public void SetSuite(string Suite) {
			EncodedString u_Suite = new EncodedString(Suite);
			mdAddrUnmanaged.mdAddrSetSuite(i, u_Suite.GetPtr());
		}

		/// <summary>Sets the city or municipality name for the address to be verified</summary>
		/// <param name="City">City or municipality name</param>
		public void SetCity(string City) {
			EncodedString u_City = new EncodedString(City);
			mdAddrUnmanaged.mdAddrSetCity(i, u_City.GetPtr());
		}

		/// <summary>Sets the two-letter state or province abbreviation for the address to be verified</summary>
		/// <param name="State">Two-letter state or province abbreviation</param>
		public void SetState(string State) {
			EncodedString u_State = new EncodedString(State);
			mdAddrUnmanaged.mdAddrSetState(i, u_State.GetPtr());
		}

		/// <summary>Sets the five-digit US ZIP code or six-character Canadian postal code for the address to be verified</summary>
		/// <param name="Zip">US ZIP code or Canadian postal code</param>
		public void SetZip(string Zip) {
			EncodedString u_Zip = new EncodedString(Zip);
			mdAddrUnmanaged.mdAddrSetZip(i, u_Zip.GetPtr());
		}

		/// <summary>Sets the four-digit ZIP+4 add-on code for the address to be verified</summary>
		/// <param name="Plus4">Four-digit ZIP+4 extension</param>
		public void SetPlus4(string Plus4) {
			EncodedString u_Plus4 = new EncodedString(Plus4);
			mdAddrUnmanaged.mdAddrSetPlus4(i, u_Plus4.GetPtr());
		}

		/// <summary>Sets the urbanization name for Puerto Rican addresses</summary>
		/// <param name="Urbanization">Puerto Rico urbanization (neighborhood) name</param>
		public void SetUrbanization(string Urbanization) {
			EncodedString u_Urbanization = new EncodedString(Urbanization);
			mdAddrUnmanaged.mdAddrSetUrbanization(i, u_Urbanization.GetPtr());
		}

		/// <summary>Sets the pre-parsed delivery number portion of the street address</summary>
		/// <param name="AddressRange">Numeric address range (max 10 characters)</param>
		public void SetParsedAddressRange(string AddressRange) {
			EncodedString u_AddressRange = new EncodedString(AddressRange);
			mdAddrUnmanaged.mdAddrSetParsedAddressRange(i, u_AddressRange.GetPtr());
		}

		/// <summary>Sets the pre-parsed directional that precedes the street name</summary>
		/// <param name="PreDirection">Pre-directional (e.g. "N", "NE", "SW")</param>
		public void SetParsedPreDirection(string PreDirection) {
			EncodedString u_PreDirection = new EncodedString(PreDirection);
			mdAddrUnmanaged.mdAddrSetParsedPreDirection(i, u_PreDirection.GetPtr());
		}

		/// <summary>Sets the pre-parsed street name portion of the address</summary>
		/// <param name="StreetName">Street name (max 28 characters)</param>
		public void SetParsedStreetName(string StreetName) {
			EncodedString u_StreetName = new EncodedString(StreetName);
			mdAddrUnmanaged.mdAddrSetParsedStreetName(i, u_StreetName.GetPtr());
		}

		/// <summary>Sets the pre-parsed street suffix portion of the address</summary>
		/// <param name="Suffix">Street suffix (e.g. "ST", "AVE", "BLVD")</param>
		public void SetParsedSuffix(string Suffix) {
			EncodedString u_Suffix = new EncodedString(Suffix);
			mdAddrUnmanaged.mdAddrSetParsedSuffix(i, u_Suffix.GetPtr());
		}

		/// <summary>Sets the pre-parsed directional that follows the street name</summary>
		/// <param name="PostDirection">Post-directional (e.g. "N", "NE", "SW")</param>
		public void SetParsedPostDirection(string PostDirection) {
			EncodedString u_PostDirection = new EncodedString(PostDirection);
			mdAddrUnmanaged.mdAddrSetParsedPostDirection(i, u_PostDirection.GetPtr());
		}

		/// <summary>Sets the pre-parsed secondary unit designator (e.g. "APT", "STE", "UNIT")</summary>
		/// <param name="SuiteName">Secondary unit designator</param>
		public void SetParsedSuiteName(string SuiteName) {
			EncodedString u_SuiteName = new EncodedString(SuiteName);
			mdAddrUnmanaged.mdAddrSetParsedSuiteName(i, u_SuiteName.GetPtr());
		}

		/// <summary>Sets the pre-parsed secondary unit number</summary>
		/// <param name="SuiteRange">Secondary unit number</param>
		public void SetParsedSuiteRange(string SuiteRange) {
			EncodedString u_SuiteRange = new EncodedString(SuiteRange);
			mdAddrUnmanaged.mdAddrSetParsedSuiteRange(i, u_SuiteRange.GetPtr());
		}

		/// <summary>Sets the pre-parsed route service for Canadian addresses (equivalent to US rural route)</summary>
		/// <param name="RouteService">Canadian route service information</param>
		public void SetParsedRouteService(string RouteService) {
			EncodedString u_RouteService = new EncodedString(RouteService);
			mdAddrUnmanaged.mdAddrSetParsedRouteService(i, u_RouteService.GetPtr());
		}

		/// <summary>Sets the pre-parsed lock box for Canadian addresses (equivalent to US PO Box)</summary>
		/// <param name="LockBox">Canadian lock box information</param>
		public void SetParsedLockBox(string LockBox) {
			EncodedString u_LockBox = new EncodedString(LockBox);
			mdAddrUnmanaged.mdAddrSetParsedLockBox(i, u_LockBox.GetPtr());
		}

		/// <summary>Sets the pre-parsed delivery installation for Canadian addresses</summary>
		/// <param name="DeliveryInstallation">Canadian delivery installation (post office facility)</param>
		public void SetParsedDeliveryInstallation(string DeliveryInstallation) {
			EncodedString u_DeliveryInstallation = new EncodedString(DeliveryInstallation);
			mdAddrUnmanaged.mdAddrSetParsedDeliveryInstallation(i, u_DeliveryInstallation.GetPtr());
		}

		/// <summary>Sets the two-character country code for the address to be verified</summary>
		/// <param name="CountryCode">Two-character country code ("US" or "CA")</param>
		public void SetCountryCode(string CountryCode) {
			EncodedString u_CountryCode = new EncodedString(CountryCode);
			mdAddrUnmanaged.mdAddrSetCountryCode(i, u_CountryCode.GetPtr());
		}

		/// <summary>Verifies, standardizes, and enhances the address set via the Set functions</summary>
		/// <returns>True (1) if the address was successfully coded; check GetResults for match level</returns>
		public bool VerifyAddress() {
			return (mdAddrUnmanaged.mdAddrVerifyAddress(i) != 0);
		}

		/// <summary>Returns the company name associated with the verified address</summary>
		/// <returns>Company name string (unchanged from input)</returns>
		public string GetCompany() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetCompany(i));
		}

		/// <summary>Returns the last name associated with the verified address</summary>
		/// <returns>Last name string (unchanged from input)</returns>
		public string GetLastName() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetLastName(i));
		}

		/// <summary>Returns the standardized primary delivery address line</summary>
		/// <returns>Standardized street address string</returns>
		public string GetAddress() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetAddress(i));
		}

		/// <summary>Returns the second address line associated with the verified address</summary>
		/// <returns>Second address line string</returns>
		public string GetAddress2() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetAddress2(i));
		}

		/// <summary>Returns the standardized suite name and number</summary>
		/// <returns>Suite string (e.g. "APT 3")</returns>
		public string GetSuite() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetSuite(i));
		}

		/// <summary>Returns the verified city or municipality name</summary>
		/// <returns>City name string</returns>
		public string GetCity() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetCity(i));
		}

		/// <summary>Returns the official USPS 13-character city name abbreviation</summary>
		/// <returns>City abbreviation (same as city name if 13 characters or fewer)</returns>
		public string GetCityAbbreviation() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetCityAbbreviation(i));
		}

		/// <summary>Returns the two-letter state or province abbreviation for the verified address</summary>
		/// <returns>Two-letter state or province code</returns>
		public string GetState() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetState(i));
		}

		/// <summary>Returns the verified five-digit US ZIP code or six-character Canadian postal code</summary>
		/// <returns>ZIP or postal code string</returns>
		public string GetZip() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetZip(i));
		}

		/// <summary>Returns the four-digit ZIP+4 add-on code for the verified address</summary>
		/// <returns>Four-digit Plus4 string</returns>
		public string GetPlus4() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetPlus4(i));
		}

		/// <summary>Returns the four-character carrier route code for the verified address</summary>
		/// <returns>Carrier route code (e.g. "C027" or "R001")</returns>
		public string GetCarrierRoute() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetCarrierRoute(i));
		}

		/// <summary>Returns the two-digit delivery point code used in POSTNET barcodes (positions 10-11)</summary>
		/// <returns>Two-digit delivery point code string</returns>
		public string GetDeliveryPointCode() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetDeliveryPointCode(i));
		}

		/// <summary>Returns the one-digit check digit for POSTNET barcodes (position 12)</summary>
		/// <returns>One-digit check digit string</returns>
		public string GetDeliveryPointCheckDigit() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetDeliveryPointCheckDigit(i));
		}

		/// <summary>Returns the five-digit county FIPS code for the verified address</summary>
		/// <returns>Five-digit FIPS code (e.g. "06037" for Los Angeles CA)</returns>
		public string GetCountyFips() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetCountyFips(i));
		}

		/// <summary>Returns the county name for the verified address</summary>
		/// <returns>County name string (up to 25 characters)</returns>
		public string GetCountyName() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetCountyName(i));
		}

		/// <summary>Returns a single-character code indicating the type of the verified address</summary>
		/// <returns>S=Street, P=PO Box, H=High Rise, R=Rural Route, F=Firm, G=General Delivery, A=Alias</returns>
		public string GetAddressTypeCode() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetAddressTypeCode(i));
		}

		/// <summary>Returns a description of the address type code returned by GetAddressTypeCode</summary>
		/// <returns>Address type description string</returns>
		public string GetAddressTypeString() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetAddressTypeString(i));
		}

		/// <summary>Returns the urbanization name for Puerto Rican addresses</summary>
		/// <returns>Urbanization name string</returns>
		public string GetUrbanization() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetUrbanization(i));
		}

		/// <summary>Returns the congressional district for the verified address</summary>
		/// <returns>State abbreviation plus two-digit district number (e.g. "CA01")</returns>
		public string GetCongressionalDistrict() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetCongressionalDistrict(i));
		}

		/// <summary>Returns whether the address has undergone a rural-to-city-style address conversion</summary>
		/// <returns>"L" if the address was converted, empty if not</returns>
		public string GetLACS() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetLACS(i));
		}

		/// <summary>Returns whether the submitted address was corrected using LACSLink data</summary>
		/// <returns>"Y" if updated, "S" if updated but suite unmatched, "N" if not updated</returns>
		public string GetLACSLinkIndicator() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetLACSLinkIndicator(i));
		}

		/// <summary>Returns the private mailbox number for CMRA addresses</summary>
		/// <returns>Private mailbox number string</returns>
		public string GetPrivateMailbox() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetPrivateMailbox(i));
		}

		/// <summary>Returns the numeric time zone code for the verified address</summary>
		/// <returns>Time zone code (e.g. "5" for Eastern, "8" for Pacific)</returns>
		public string GetTimeZoneCode() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetTimeZoneCode(i));
		}

		/// <summary>Returns the time zone name for the verified address</summary>
		/// <returns>Time zone name string (e.g. "Eastern Time", "Pacific Time")</returns>
		public string GetTimeZone() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetTimeZone(i));
		}

		/// <summary>Returns the Metropolitan Statistical Area (MSA) code for the verified address</summary>
		/// <returns>MSA code string</returns>
		public string GetMsa() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetMsa(i));
		}

		/// <summary>Returns the Primary Metropolitan Statistical Area (PMSA) code for the verified address</summary>
		/// <returns>PMSA code string</returns>
		public string GetPmsa() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetPmsa(i));
		}

		/// <summary>Returns whether a default Plus4 code was assigned during address verification</summary>
		/// <returns>Default flag indicator string</returns>
		public string GetDefaultFlagIndicator() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetDefaultFlagIndicator(i));
		}

		/// <summary>Returns the SuiteLink suite status indicator for the verified address</summary>
		/// <returns>Suite status string</returns>
		public string GetSuiteStatus() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetSuiteStatus(i));
		}

		/// <summary>Returns whether the address was found in the Early Warning System (EWS) database. Only returns a value if EnableEarlyWarningSystem was set to true and ews.txt is present in the data files folder</summary>
		/// <returns>"Y" if found in EWS (GetResults also returns "AE06"), empty otherwise</returns>
		public string GetEWSFlag() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetEWSFlag(i));
		}

		/// <summary>Returns whether the address is a private mailbox at a Commercial Mail Receiving Agency (e.g. UPS Store). Does not return data when GetResults returns "AS03" (non-USPS address)</summary>
		/// <returns>"N" if not a CMRA, "Y" if a CMRA, empty if not populated (e.g. certain DPV footnote codes)</returns>
		public string GetCMRA() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetCMRA(i));
		}

		/// <summary>Returns whether the delivery point is currently vacant</summary>
		/// <returns>"Y" if vacant (unoccupied over 90 days), "N" if not vacant</returns>
		public string GetDsfVacant() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetDsfVacant(i));
		}

		/// <summary>Returns the two-character country code for the verified address</summary>
		/// <returns>"US" for United States, "CA" for Canada</returns>
		public string GetCountryCode() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetCountryCode(i));
		}

		/// <summary>Returns the ZIP code type for the verified address</summary>
		/// <returns>"P" for PO Box only, "U" for unique/institutional, "M" for military, empty for standard</returns>
		public string GetZipType() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetZipType(i));
		}

		/// <summary>Returns whether the address appears in the DPV false positive table</summary>
		/// <returns>False positive table indicator string</returns>
		public string GetFalseTable() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetFalseTable(i));
		}

		/// <summary>Returns the USPS standard DPV footnote codes for the verified address</summary>
		/// <returns>String of up to 6 DPV footnote codes</returns>
		public string GetDPVFootnotes() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetDPVFootnotes(i));
		}

		/// <summary>Returns the LACSLink match result code</summary>
		/// <returns>"A" for match, "00" for no match, "14" for unconvertible, "92" for secondary dropped</returns>
		public string GetLACSLinkReturnCode() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetLACSLinkReturnCode(i));
		}

		/// <summary>Returns the SuiteLink match result code indicating whether suite information was appended</summary>
		/// <returns>"A" if suite matched and appended, "00" if no match</returns>
		public string GetSuiteLinkReturnCode() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetSuiteLinkReturnCode(i));
		}

		/// <summary>Returns the Residential Business Delivery Indicator for the verified address</summary>
		/// <returns>"R" for residential, "B" for business, "U" for unknown</returns>
		public string GetRBDI() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetRBDI(i));
		}

		/// <summary>Returns the enhanced Line-of-Travel (eLOT) sequence number for the verified address</summary>
		/// <returns>eLOT number string, or "-1" if eLOT data is unavailable</returns>
		public string GetELotNumber() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetELotNumber(i));
		}

		/// <summary>Returns the eLOT delivery direction indicator</summary>
		/// <returns>"A" for ascending delivery order, "D" for descending, "X" if unavailable</returns>
		public string GetELotOrder() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetELotOrder(i));
		}

		/// <summary>Returns the unique 11-digit USPS address key for the verified address</summary>
		/// <returns>11-digit address key string</returns>
		public string GetAddressKey() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetAddressKey(i));
		}

		/// <summary>Returns the Melissa Address Key (MAK), a persistent 10-digit unique identifier for the address</summary>
		/// <returns>10-digit MAK string</returns>
		public string GetMelissaAddressKey() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetMelissaAddressKey(i));
		}

		/// <summary>Returns the Melissa Address Key Base (BaseMAK), linking suite addresses to their parent building</summary>
		/// <returns>10-digit BaseMAK string</returns>
		public string GetMelissaAddressKeyBase() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetMelissaAddressKeyBase(i));
		}

		/// <summary>Returns the value of a named output parameter after a call to VerifyAddress</summary>
		/// <param name="ParameterName">Parameter name defined by header macros: "CMRA", "PBSA", "Vacant", "ZipIncludedIn3553", "DropIndicator", "ThrowBack", "DeliveryDays", "NoStatIndicator", "NoStatReasonCode", "DPVConfirmationCode", "EnhancedDPVCode", "NoSecureLocation", "DoorNotAccessible", "FalseTable", "RDI", "CongressionalDistrict"</param>
		/// <returns>Parameter value string</returns>
		public string GetOutputParameter(string ParameterName) {
			EncodedString u_ParameterName = new EncodedString(ParameterName);
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetOutputParameter(i, u_ParameterName.GetPtr()));
		}

		/// <summary>Sets a named input parameter before calling VerifyAddress</summary>
		/// <param name="ParameterName">Parameter name defined by header macros: "EnableRDI", "CASSTest", "TotalDPS", "UsePostalCongressionalDistricts"</param>
		/// <param name="Value">Parameter value string</param>
		/// <returns>0 on success</returns>
		public int SetInputParameter(string ParameterName, string Value) {
			EncodedString u_ParameterName = new EncodedString(ParameterName);
			EncodedString u_Value = new EncodedString(Value);
			return mdAddrUnmanaged.mdAddrSetInputParameter(i, u_ParameterName.GetPtr(), u_Value.GetPtr());
		}

		/// <summary>Returns the first suggested address when VerifyAddress could not verify the submitted address</summary>
		/// <returns>True (1) if a suggestion was found</returns>
		public bool FindSuggestion() {
			return (mdAddrUnmanaged.mdAddrFindSuggestion(i) != 0);
		}

		/// <summary>Returns the next suggested address after a successful call to FindSuggestion</summary>
		/// <returns>True (1) if another suggestion was found, false (0) if no more suggestions</returns>
		public bool FindSuggestionNext() {
			return (mdAddrUnmanaged.mdAddrFindSuggestionNext(i) != 0);
		}

		/// <summary>Returns the DSF (Delivery Sequence File) no-stats indicator for the verified address</summary>
		/// <returns>DSF no-stats indicator string</returns>
		public string GetDsfNoStats() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetDsfNoStats(i));
		}

		/// <summary>Returns the DSF Do Not Address indicator for the verified address</summary>
		/// <returns>DSF DNA indicator string</returns>
		public string GetDsfDNA() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetDsfDNA(i));
		}

		/// <summary>Returns the total number of address records verified since the current instance was created or the last ResetFormPS3553 call (CASS Form 3553 item B6)</summary>
		/// <returns>Total verified record count (integer)</returns>
		public int GetPS3553_B6_TotalRecords() {
			return mdAddrUnmanaged.mdAddrGetPS3553_B6_TotalRecords(i);
		}

		/// <summary>Returns the count of records matched to the ZIP+4 level since the current instance was created or the last ResetFormPS3553 call (CASS Form 3553 item C1a)</summary>
		/// <returns>ZIP+4 coded record count (integer)</returns>
		public int GetPS3553_C1a_ZIP4Coded() {
			return mdAddrUnmanaged.mdAddrGetPS3553_C1a_ZIP4Coded(i);
		}

		/// <summary>Returns the count of records assigned a Delivery Point Bar Code since the current instance was created or the last ResetFormPS3553 call (CASS Form 3553 item C1c)</summary>
		/// <returns>DPBC assigned record count (integer)</returns>
		public int GetPS3553_C1c_DPBCAssigned() {
			return mdAddrUnmanaged.mdAddrGetPS3553_C1c_DPBCAssigned(i);
		}

		/// <summary>Returns the count of records verified to the five-digit carrier route level since the current instance was created or the last ResetFormPS3553 call (CASS Form 3553 item C1d)</summary>
		/// <returns>Five-digit coded record count (integer)</returns>
		public int GetPS3553_C1d_FiveDigitCoded() {
			return mdAddrUnmanaged.mdAddrGetPS3553_C1d_FiveDigitCoded(i);
		}

		/// <summary>Returns the count of records verified to the carrier route level since the current instance was created or the last ResetFormPS3553 call (CASS Form 3553 item C1e)</summary>
		/// <returns>Carrier route coded record count (integer)</returns>
		public int GetPS3553_C1e_CRRTCoded() {
			return mdAddrUnmanaged.mdAddrGetPS3553_C1e_CRRTCoded(i);
		}

		/// <summary>Returns the count of records assigned an enhanced Line of Travel (eLOT) sequence number since the current instance was created or the last ResetFormPS3553 call (CASS Form 3553 item C1f)</summary>
		/// <returns>eLOT assigned record count (integer)</returns>
		public int GetPS3553_C1f_eLOTAssigned() {
			return mdAddrUnmanaged.mdAddrGetPS3553_C1f_eLOTAssigned(i);
		}

		/// <summary>Returns the count of high-rise records default matched (matched at the street address level but not to a valid secondary address) since the current instance was created or the last ResetFormPS3553 call (CASS Form 3553 section E)</summary>
		/// <returns>High-rise default match count (integer)</returns>
		public int GetPS3553_E_HighRiseDefault() {
			return mdAddrUnmanaged.mdAddrGetPS3553_E_HighRiseDefault(i);
		}

		/// <summary>Returns the count of high-rise records exactly matched to both a primary and secondary address since the current instance was created or the last ResetFormPS3553 call (CASS Form 3553 section E)</summary>
		/// <returns>High-rise exact match count (integer)</returns>
		public int GetPS3553_E_HighRiseExact() {
			return mdAddrUnmanaged.mdAddrGetPS3553_E_HighRiseExact(i);
		}

		/// <summary>Returns the count of rural route records default matched (verified at the route number level but not to a valid primary range) since the current instance was created or the last ResetFormPS3553 call (CASS Form 3553 section E)</summary>
		/// <returns>Rural route default match count (integer)</returns>
		public int GetPS3553_E_RuralRouteDefault() {
			return mdAddrUnmanaged.mdAddrGetPS3553_E_RuralRouteDefault(i);
		}

		/// <summary>Returns the count of rural route records verified at the route number level and matched to a valid primary range, since the current instance was created or the last ResetFormPS3553 call (CASS Form 3553 section E)</summary>
		/// <returns>Rural route exact match count (integer)</returns>
		public int GetPS3553_E_RuralRouteExact() {
			return mdAddrUnmanaged.mdAddrGetPS3553_E_RuralRouteExact(i);
		}

		/// <summary>Returns the count of high-rise records default matched at the ZIP+4 level, since the current instance was created or the last ResetFormPS3553 call</summary>
		/// <returns>ZIP+4 high-rise default match count (integer)</returns>
		public int GetZip4HRDefault() {
			return mdAddrUnmanaged.mdAddrGetZip4HRDefault(i);
		}

		/// <summary>Returns the count of high-rise records exactly matched at the ZIP+4 level, since the current instance was created or the last ResetFormPS3553 call</summary>
		/// <returns>ZIP+4 high-rise exact match count (integer)</returns>
		public int GetZip4HRExact() {
			return mdAddrUnmanaged.mdAddrGetZip4HRExact(i);
		}

		/// <summary>Returns the count of rural route records default matched at the ZIP+4 level, since the current instance was created or the last ResetFormPS3553 call</summary>
		/// <returns>ZIP+4 rural route default match count (integer)</returns>
		public int GetZip4RRDefault() {
			return mdAddrUnmanaged.mdAddrGetZip4RRDefault(i);
		}

		/// <summary>Returns the count of rural route records exactly matched at the ZIP+4 level, since the current instance was created or the last ResetFormPS3553 call</summary>
		/// <returns>ZIP+4 rural route exact match count (integer)</returns>
		public int GetZip4RRExact() {
			return mdAddrUnmanaged.mdAddrGetZip4RRExact(i);
		}

		/// <summary>Returns the count of records flagged as having undergone a rural-to-city-style address conversion since the current instance was created or the last ResetFormPS3553 call (CASS Form 3553 section E)</summary>
		/// <returns>LACS record count (integer)</returns>
		public int GetPS3553_E_LACSCount() {
			return mdAddrUnmanaged.mdAddrGetPS3553_E_LACSCount(i);
		}

		/// <summary>Returns the count of records that did not appear in the current USPS ZIP+4 file but were valid addresses, since the current instance was created or the last ResetFormPS3553 call (CASS Form 3553 section E)</summary>
		/// <returns>EWS record count (integer)</returns>
		public int GetPS3553_E_EWSCount() {
			return mdAddrUnmanaged.mdAddrGetPS3553_E_EWSCount(i);
		}

		/// <summary>Returns the count of records confirmed via DPV since the current instance was created or the last ResetFormPS3553 call (CASS Form 3553 section E)</summary>
		/// <returns>DPV confirmed record count (integer)</returns>
		public int GetPS3553_E_DPVCount() {
			return mdAddrUnmanaged.mdAddrGetPS3553_E_DPVCount(i);
		}

		/// <summary>Returns the count of PO Box address records verified since the current instance was created or the last ResetFormPS3553 call</summary>
		/// <returns>PO Box record count (integer)</returns>
		public int GetPS3553_X_POBoxCount() {
			return mdAddrUnmanaged.mdAddrGetPS3553_X_POBoxCount(i);
		}

		/// <summary>Returns the count of Highway Contract route records exactly matched since the current instance was created or the last ResetFormPS3553 call</summary>
		/// <returns>Highway Contract exact match count (integer)</returns>
		public int GetPS3553_X_HCExactCount() {
			return mdAddrUnmanaged.mdAddrGetPS3553_X_HCExactCount(i);
		}

		/// <summary>Returns the count of firm/company address records verified since the current instance was created or the last ResetFormPS3553 call</summary>
		/// <returns>Firm record count (integer)</returns>
		public int GetPS3553_X_FirmCount() {
			return mdAddrUnmanaged.mdAddrGetPS3553_X_FirmCount(i);
		}

		/// <summary>Returns the count of General Delivery address records verified since the current instance was created or the last ResetFormPS3553 call</summary>
		/// <returns>General Delivery record count (integer)</returns>
		public int GetPS3553_X_GenDeliveryCount() {
			return mdAddrUnmanaged.mdAddrGetPS3553_X_GenDeliveryCount(i);
		}

		/// <summary>Returns the count of military ZIP code address records verified since the current instance was created or the last ResetFormPS3553 call</summary>
		/// <returns>Military ZIP record count (integer)</returns>
		public int GetPS3553_X_MilitaryZipCount() {
			return mdAddrUnmanaged.mdAddrGetPS3553_X_MilitaryZipCount(i);
		}

		/// <summary>Returns the count of non-deliverable address records since the current instance was created or the last ResetFormPS3553 call</summary>
		/// <returns>Non-delivery record count (integer)</returns>
		public int GetPS3553_X_NonDeliveryCount() {
			return mdAddrUnmanaged.mdAddrGetPS3553_X_NonDeliveryCount(i);
		}

		/// <summary>Returns the count of street-level address records verified since the current instance was created or the last ResetFormPS3553 call</summary>
		/// <returns>Street record count (integer)</returns>
		public int GetPS3553_X_StreetCount() {
			return mdAddrUnmanaged.mdAddrGetPS3553_X_StreetCount(i);
		}

		/// <summary>Returns the count of Highway Contract route records default matched since the current instance was created or the last ResetFormPS3553 call</summary>
		/// <returns>Highway Contract default match count (integer)</returns>
		public int GetPS3553_X_HCDefaultCount() {
			return mdAddrUnmanaged.mdAddrGetPS3553_X_HCDefaultCount(i);
		}

		/// <summary>Returns the count of address records not falling into any standard category, since the current instance was created or the last ResetFormPS3553 call</summary>
		/// <returns>Other record count (integer)</returns>
		public int GetPS3553_X_OtherCount() {
			return mdAddrUnmanaged.mdAddrGetPS3553_X_OtherCount(i);
		}

		/// <summary>Returns the count of records with LACSLink Code A (successful match and conversion), since the current instance was created or the last ResetFormPS3553 call</summary>
		/// <returns>LACSLink Code A count (integer)</returns>
		public int GetPS3553_X_LacsLinkCodeACount() {
			return mdAddrUnmanaged.mdAddrGetPS3553_X_LacsLinkCodeACount(i);
		}

		/// <summary>Returns the count of records with LACSLink Code 00 (no match found), since the current instance was created or the last ResetFormPS3553 call</summary>
		/// <returns>LACSLink Code 00 count (integer)</returns>
		public int GetPS3553_X_LacsLinkCode00Count() {
			return mdAddrUnmanaged.mdAddrGetPS3553_X_LacsLinkCode00Count(i);
		}

		/// <summary>Returns the count of records with LACSLink Code 14 (new address would not convert), since the current instance was created or the last ResetFormPS3553 call</summary>
		/// <returns>LACSLink Code 14 count (integer)</returns>
		public int GetPS3553_X_LacsLinkCode14Count() {
			return mdAddrUnmanaged.mdAddrGetPS3553_X_LacsLinkCode14Count(i);
		}

		/// <summary>Returns the count of records with LACSLink Code 92 (secondary number dropped during conversion), since the current instance was created or the last ResetFormPS3553 call</summary>
		/// <returns>LACSLink Code 92 count (integer)</returns>
		public int GetPS3553_X_LacsLinkCode92Count() {
			return mdAddrUnmanaged.mdAddrGetPS3553_X_LacsLinkCode92Count(i);
		}

		/// <summary>Returns the count of records with LACSLink Code 09, since the current instance was created or the last ResetFormPS3553 call</summary>
		/// <returns>LACSLink Code 09 count (integer)</returns>
		public int GetPS3553_X_LacsLinkCode09Count() {
			return mdAddrUnmanaged.mdAddrGetPS3553_X_LacsLinkCode09Count(i);
		}

		/// <summary>Returns the count of address records with suite information appended via SuiteLink (Code A), since the current instance was created or the last ResetFormPS3553 call</summary>
		/// <returns>SuiteLink Code A record count (integer)</returns>
		public int GetPS3553_X_SuiteLinkCodeACount() {
			return mdAddrUnmanaged.mdAddrGetPS3553_X_SuiteLinkCodeACount(i);
		}

		/// <summary>Returns the count of records with SuiteLink Code 00 (no suite match), since the current instance was created or the last ResetFormPS3553 call</summary>
		/// <returns>SuiteLink Code 00 count (integer)</returns>
		public int GetPS3553_X_SuiteLinkCode00Count() {
			return mdAddrUnmanaged.mdAddrGetPS3553_X_SuiteLinkCode00Count(i);
		}

		/// <summary>Returns the numeric delivery number from the verified street address</summary>
		/// <returns>Address range string (max 10 characters)</returns>
		public string GetParsedAddressRange() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetParsedAddressRange(i));
		}

		/// <summary>Returns the directional preceding the street name from the verified address</summary>
		/// <returns>Pre-directional string (e.g. "N", "SW")</returns>
		public string GetParsedPreDirection() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetParsedPreDirection(i));
		}

		/// <summary>Returns the street name from the verified address</summary>
		/// <returns>Street name string (max 28 characters)</returns>
		public string GetParsedStreetName() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetParsedStreetName(i));
		}

		/// <summary>Returns the street suffix from the verified address</summary>
		/// <returns>Street suffix string (e.g. "ST", "AVE", "BLVD")</returns>
		public string GetParsedSuffix() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetParsedSuffix(i));
		}

		/// <summary>Returns the directional following the street name from the verified address</summary>
		/// <returns>Post-directional string (e.g. "N", "SW")</returns>
		public string GetParsedPostDirection() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetParsedPostDirection(i));
		}

		/// <summary>Returns the secondary unit designator from the verified address</summary>
		/// <returns>Suite designator string (e.g. "APT", "STE", "UNIT")</returns>
		public string GetParsedSuiteName() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetParsedSuiteName(i));
		}

		/// <summary>Returns the secondary unit number from the verified address</summary>
		/// <returns>Suite number string</returns>
		public string GetParsedSuiteRange() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetParsedSuiteRange(i));
		}

		/// <summary>Returns the private mailbox designator from the verified address</summary>
		/// <returns>Private mailbox name string ("PMB" or "#")</returns>
		public string GetParsedPrivateMailboxName() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetParsedPrivateMailboxName(i));
		}

		/// <summary>Returns the private mailbox number from the verified address</summary>
		/// <returns>Private mailbox number string (max 10 characters)</returns>
		public string GetParsedPrivateMailboxNumber() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetParsedPrivateMailboxNumber(i));
		}

		/// <summary>Returns any unrecognized characters left over after parsing the verified address</summary>
		/// <returns>Garbage string (max 50 characters)</returns>
		public string GetParsedGarbage() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetParsedGarbage(i));
		}

		/// <summary>Returns the route service information from the verified Canadian address</summary>
		/// <returns>Route service string</returns>
		public string GetParsedRouteService() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetParsedRouteService(i));
		}

		/// <summary>Returns the lock box information from the verified Canadian address</summary>
		/// <returns>Lock box string</returns>
		public string GetParsedLockBox() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetParsedLockBox(i));
		}

		/// <summary>Returns the delivery installation information from the verified Canadian address</summary>
		/// <returns>Delivery installation string</returns>
		public string GetParsedDeliveryInstallation() {
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetParsedDeliveryInstallation(i));
		}

		/// <summary>Reserved for Melissa Data internal use</summary>
		/// <param name="Key">Reserved parameter name</param>
		/// <param name="Value">Reserved parameter value</param>
		public void SetReserved(string Key, string Value) {
			EncodedString u_Key = new EncodedString(Key);
			EncodedString u_Value = new EncodedString(Value);
			mdAddrUnmanaged.mdAddrSetReserved(i, u_Key.GetPtr(), u_Value.GetPtr());
		}

		/// <summary>Reserved for Melissa Data internal use</summary>
		/// <param name="Key">Reserved parameter name</param>
		/// <returns>Reserved parameter value</returns>
		public string GetReserved(string Key) {
			EncodedString u_Key = new EncodedString(Key);
			return EncodedString.GetEncodedString(mdAddrUnmanaged.mdAddrGetReserved(i, u_Key.GetPtr()));
		}

		private class EncodedString : IDisposable {
			private IntPtr encodedString = IntPtr.Zero;
			private static Encoding encoding = Encoding.GetEncoding("ISO-8859-1");

			public EncodedString(string str) {
				if (str == null)
					str = "";
				byte[] buffer = encoding.GetBytes(str);
				Array.Resize(ref buffer, buffer.Length + 1);
				buffer[buffer.Length - 1] = 0;
				encodedString = Marshal.AllocHGlobal(buffer.Length);
				Marshal.Copy(buffer, 0, encodedString, buffer.Length);
			}

			~EncodedString() {
				Dispose();
			}

			public virtual void Dispose() {
				lock (this) {
					Marshal.FreeHGlobal(encodedString);
					GC.SuppressFinalize(this);
				}
			}

			public IntPtr GetPtr() {
				return encodedString;
			}

			public static string GetEncodedString(IntPtr ptr) {
				if (ptr == IntPtr.Zero)
					return "";
				int len = 0;
				while (Marshal.ReadByte(ptr, len) != 0)
					len++;
				if (len == 0)
					return "";
				byte[] buffer = new byte[len];
				Marshal.Copy(ptr, buffer, 0, len);
				return encoding.GetString(buffer);
			}
		}
	}

	/// <summary>Parses a street address string into its component parts without requiring data file initialization</summary>
	/// <remarks>Lighter-weight alternative to AddressCheck when full verification is not needed</remarks>
	public class mdParse : IDisposable {
		private IntPtr i;

		/// <summary>Programming, installation, and execution error codes returned by Initialize</summary>
		public enum ProgramStatus {
			/// <summary>No error - initialization was successful</summary>
			ErrorNone = 0,
			/// <summary>Could not open the required data file</summary>
			ErrorOther = 1,
			/// <summary>Out of memory</summary>
			ErrorOutOfMemory = 2,
			/// <summary>A required data file could not be found</summary>
			ErrorRequiredFileNotFound = 3,
			/// <summary>Conflicts found between two or more data files</summary>
			ErrorFoundOldFile = 4,
			/// <summary>The database has expired</summary>
			ErrorDatabaseExpired = 5,
			/// <summary>The license has expired</summary>
			ErrorLicenseExpired = 6
		}

		[SuppressUnmanagedCodeSecurity]
		private class mdParseUnmanaged {
			[DllImport("mdAddr", EntryPoint = "mdParseCreate", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdParseCreate();
			[DllImport("mdAddr", EntryPoint = "mdParseDestroy", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdParseDestroy(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdParseInitialize", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdParseInitialize(IntPtr i, IntPtr DataPath);
			[DllImport("mdAddr", EntryPoint = "mdParseGetBuildNumber", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdParseGetBuildNumber(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdParseParse", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdParseParse(IntPtr i, IntPtr AddressToParse);
			[DllImport("mdAddr", EntryPoint = "mdParseParseCanadian", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdParseParseCanadian(IntPtr i, IntPtr AddressToParse);
			[DllImport("mdAddr", EntryPoint = "mdParseParseNext", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdParseParseNext(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdParseLastLineParse", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdParseLastLineParse(IntPtr i, IntPtr LastLine);
			[DllImport("mdAddr", EntryPoint = "mdParseGetZip", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdParseGetZip(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdParseGetPlus4", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdParseGetPlus4(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdParseGetCity", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdParseGetCity(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdParseGetState", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdParseGetState(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdParseGetStreetName", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdParseGetStreetName(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdParseGetRange", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdParseGetRange(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdParseGetPreDirection", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdParseGetPreDirection(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdParseGetPostDirection", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdParseGetPostDirection(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdParseGetSuffix", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdParseGetSuffix(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdParseGetSuiteName", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdParseGetSuiteName(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdParseGetSuiteNumber", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdParseGetSuiteNumber(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdParseGetPrivateMailboxNumber", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdParseGetPrivateMailboxNumber(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdParseGetPrivateMailboxName", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdParseGetPrivateMailboxName(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdParseGetGarbage", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdParseGetGarbage(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdParseGetRouteService", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdParseGetRouteService(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdParseGetLockBox", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdParseGetLockBox(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdParseGetDeliveryInstallation", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdParseGetDeliveryInstallation(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdParseParseRule", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdParseParseRule(IntPtr i);
		}

		/// <summary>Constructs a new Parse instance</summary>
		public mdParse() {
			i = mdParseUnmanaged.mdParseCreate();
		}

		/// <summary>Destroys an existing Parse instance</summary>
		~mdParse() {
			Dispose();
		}

		public virtual void Dispose() {
			lock (this) {
				mdParseUnmanaged.mdParseDestroy(i);
				GC.SuppressFinalize(this);
			}
		}

		/// <summary>Initializes the Parse interface</summary>
		/// <param name="DataPath">Path to data files (optional for basic parsing)</param>
		/// <returns>ProgramStatus enum - ErrorNone (0) on success</returns>
		public ProgramStatus Initialize(string DataPath) {
			EncodedString u_DataPath = new EncodedString(DataPath);
			return (ProgramStatus)mdParseUnmanaged.mdParseInitialize(i, u_DataPath.GetPtr());
		}

		/// <summary>Returns the current build number of Address Object</summary>
		/// <returns>Build number string</returns>
		public string GetBuildNumber() {
			return EncodedString.GetEncodedString(mdParseUnmanaged.mdParseGetBuildNumber(i));
		}

		/// <summary>Parses a US street address string into its individual components</summary>
		/// <param name="AddressToParse">Full street address string to parse (e.g. "123 N Main St E Apt 3")</param>
		public void Parse(string AddressToParse) {
			EncodedString u_AddressToParse = new EncodedString(AddressToParse);
			mdParseUnmanaged.mdParseParse(i, u_AddressToParse.GetPtr());
		}

		/// <summary>Parses a Canadian street address string into its individual components</summary>
		/// <param name="AddressToParse">Full Canadian street address string to parse</param>
		public void ParseCanadian(string AddressToParse) {
			EncodedString u_AddressToParse = new EncodedString(AddressToParse);
			mdParseUnmanaged.mdParseParseCanadian(i, u_AddressToParse.GetPtr());
		}

		/// <summary>Returns an alternate parsing of the address previously submitted to Parse or ParseCanadian</summary>
		/// <returns>True (1) if another parse variant was found, false (0) if no more variants</returns>
		public bool ParseNext() {
			return (mdParseUnmanaged.mdParseParseNext(i) != 0);
		}

		/// <summary>Parses a combined city, state, and ZIP code string into separate components</summary>
		/// <param name="LastLine">Combined last line string (e.g. "Rancho Santa Margarita, CA 92688-2112")</param>
		public void LastLineParse(string LastLine) {
			EncodedString u_LastLine = new EncodedString(LastLine);
			mdParseUnmanaged.mdParseLastLineParse(i, u_LastLine.GetPtr());
		}

		/// <summary>Returns the ZIP code from the parsed address</summary>
		/// <returns>Five-digit ZIP code or Canadian postal code string</returns>
		public string GetZip() {
			return EncodedString.GetEncodedString(mdParseUnmanaged.mdParseGetZip(i));
		}

		/// <summary>Returns the four-digit ZIP+4 extension from the parsed last line</summary>
		/// <returns>Plus4 string</returns>
		public string GetPlus4() {
			return EncodedString.GetEncodedString(mdParseUnmanaged.mdParseGetPlus4(i));
		}

		/// <summary>Returns the city name from the parsed last line</summary>
		/// <returns>City name string</returns>
		public string GetCity() {
			return EncodedString.GetEncodedString(mdParseUnmanaged.mdParseGetCity(i));
		}

		/// <summary>Returns the two-letter state or province abbreviation from the parsed last line</summary>
		/// <returns>State abbreviation string</returns>
		public string GetState() {
			return EncodedString.GetEncodedString(mdParseUnmanaged.mdParseGetState(i));
		}

		/// <summary>Returns the street name from the parsed address</summary>
		/// <returns>Street name string (max 28 characters)</returns>
		public string GetStreetName() {
			return EncodedString.GetEncodedString(mdParseUnmanaged.mdParseGetStreetName(i));
		}

		/// <summary>Returns the delivery number from the parsed address</summary>
		/// <returns>Address range string (max 10 characters)</returns>
		public string GetRange() {
			return EncodedString.GetEncodedString(mdParseUnmanaged.mdParseGetRange(i));
		}

		/// <summary>Returns the directional preceding the street name from the parsed address</summary>
		/// <returns>Pre-directional string (e.g. "N", "NE", "SW")</returns>
		public string GetPreDirection() {
			return EncodedString.GetEncodedString(mdParseUnmanaged.mdParseGetPreDirection(i));
		}

		/// <summary>Returns the directional following the street name from the parsed address</summary>
		/// <returns>Post-directional string (e.g. "N", "NE", "SW")</returns>
		public string GetPostDirection() {
			return EncodedString.GetEncodedString(mdParseUnmanaged.mdParseGetPostDirection(i));
		}

		/// <summary>Returns the street suffix from the parsed address</summary>
		/// <returns>Street suffix string (e.g. "ST", "AVE", "BLVD")</returns>
		public string GetSuffix() {
			return EncodedString.GetEncodedString(mdParseUnmanaged.mdParseGetSuffix(i));
		}

		/// <summary>Returns the secondary unit designator from the parsed address</summary>
		/// <returns>Suite designator string (e.g. "APT", "STE", "UNIT")</returns>
		public string GetSuiteName() {
			return EncodedString.GetEncodedString(mdParseUnmanaged.mdParseGetSuiteName(i));
		}

		/// <summary>Returns the secondary unit number from the parsed address</summary>
		/// <returns>Suite number string (max 6 characters)</returns>
		public string GetSuiteNumber() {
			return EncodedString.GetEncodedString(mdParseUnmanaged.mdParseGetSuiteNumber(i));
		}

		/// <summary>Returns the private mailbox number from the parsed address</summary>
		/// <returns>PMB number string</returns>
		public string GetPrivateMailboxNumber() {
			return EncodedString.GetEncodedString(mdParseUnmanaged.mdParseGetPrivateMailboxNumber(i));
		}

		/// <summary>Returns the private mailbox designator from the parsed address</summary>
		/// <returns>PMB designator string ("PMB" or "#")</returns>
		public string GetPrivateMailboxName() {
			return EncodedString.GetEncodedString(mdParseUnmanaged.mdParseGetPrivateMailboxName(i));
		}

		/// <summary>Returns any unrecognized characters remaining after parsing</summary>
		/// <returns>Garbage string (max 50 characters)</returns>
		public string GetGarbage() {
			return EncodedString.GetEncodedString(mdParseUnmanaged.mdParseGetGarbage(i));
		}

		/// <summary>Returns the route service information from the parsed Canadian address</summary>
		/// <returns>Route service string</returns>
		public string GetRouteService() {
			return EncodedString.GetEncodedString(mdParseUnmanaged.mdParseGetRouteService(i));
		}

		/// <summary>Returns the lock box information from the parsed Canadian address</summary>
		/// <returns>Lock box string</returns>
		public string GetLockBox() {
			return EncodedString.GetEncodedString(mdParseUnmanaged.mdParseGetLockBox(i));
		}

		/// <summary>Returns the delivery installation information from the parsed Canadian address</summary>
		/// <returns>Delivery installation string</returns>
		public string GetDeliveryInstallation() {
			return EncodedString.GetEncodedString(mdParseUnmanaged.mdParseGetDeliveryInstallation(i));
		}

		/// <summary>Returns the rule number used for the last address parse</summary>
		/// <returns>Integer parse rule number</returns>
		public int ParseRule() {
			return mdParseUnmanaged.mdParseParseRule(i);
		}

		private class EncodedString : IDisposable {
			private IntPtr encodedString = IntPtr.Zero;
			private static Encoding encoding = Encoding.GetEncoding("ISO-8859-1");

			public EncodedString(string str) {
				if (str == null)
					str = "";
				byte[] buffer = encoding.GetBytes(str);
				Array.Resize(ref buffer, buffer.Length + 1);
				buffer[buffer.Length - 1] = 0;
				encodedString = Marshal.AllocHGlobal(buffer.Length);
				Marshal.Copy(buffer, 0, encodedString, buffer.Length);
			}

			~EncodedString() {
				Dispose();
			}

			public virtual void Dispose() {
				lock (this) {
					Marshal.FreeHGlobal(encodedString);
					GC.SuppressFinalize(this);
				}
			}

			public IntPtr GetPtr() {
				return encodedString;
			}

			public static string GetEncodedString(IntPtr ptr) {
				if (ptr == IntPtr.Zero)
					return "";
				int len = 0;
				while (Marshal.ReadByte(ptr, len) != 0)
					len++;
				if (len == 0)
					return "";
				byte[] buffer = new byte[len];
				Marshal.Copy(ptr, buffer, 0, len);
				return encoding.GetString(buffer);
			}
		}
	}

	/// <summary>Looks up valid address ranges for a given street name and ZIP code</summary>
	/// <remarks>Use FindStreet and FindStreetNext to iterate over matching street records</remarks>
	public class mdStreet : IDisposable {
		private IntPtr i;

		/// <summary>Programming, installation, and execution error codes returned by Initialize</summary>
		public enum ProgramStatus {
			/// <summary>No error - initialization was successful</summary>
			ErrorNone = 0,
			/// <summary>Could not open the mdAddr.dat file</summary>
			ErrorOther = 1,
			/// <summary>Out of memory</summary>
			ErrorOutOfMemory = 2,
			/// <summary>A required data file could not be found</summary>
			ErrorRequiredFileNotFound = 3,
			/// <summary>Conflicts found between two or more data files</summary>
			ErrorFoundOldFile = 4,
			/// <summary>The database has expired</summary>
			ErrorDatabaseExpired = 5,
			/// <summary>The license has expired</summary>
			ErrorLicenseExpired = 6
		}
		/// <summary>Controls how suite information is presented in GetAutoCompletion suggestions</summary>
		public enum AutoCompletionMode {
			/// <summary>Show each individual suite</summary>
			AutoCompleteSingleSuite = 0,
			/// <summary>Show the suite ranges</summary>
			AutoCompleteRangedSuite = 1,
			/// <summary>Use a '#' to denote where a suite should go</summary>
			AutoCompletePlaceHolderSuite = 2,
			/// <summary>Do not show suites</summary>
			AutoCompleteNoSuite = 3
		}

		[SuppressUnmanagedCodeSecurity]
		private class mdStreetUnmanaged {
			[DllImport("mdAddr", EntryPoint = "mdStreetCreate", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetCreate();
			[DllImport("mdAddr", EntryPoint = "mdStreetDestroy", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdStreetDestroy(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetInitialize", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdStreetInitialize(IntPtr i, IntPtr DataPath, IntPtr NationalPath, IntPtr RegionalPath);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetInitializeErrorString", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetInitializeErrorString(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetDatabaseDate", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetDatabaseDate(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetBuildNumber", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetBuildNumber(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetSetLicenseString", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdStreetSetLicenseString(IntPtr i, IntPtr License);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetLicenseExpirationDate", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetLicenseExpirationDate(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetFindStreet", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdStreetFindStreet(IntPtr i, IntPtr StreetName, IntPtr ZIP, Int32 CodeOnly);
			[DllImport("mdAddr", EntryPoint = "mdStreetFindStreetNext", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdStreetFindStreetNext(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetIsAddressInRange", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdStreetIsAddressInRange(IntPtr i, IntPtr InputRange, IntPtr LoRange, IntPtr HiRange);
			[DllImport("mdAddr", EntryPoint = "mdStreetIsAddressInRange2", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdStreetIsAddressInRange2(IntPtr i, IntPtr InputRange, IntPtr LoRange, IntPtr HiRange, IntPtr OddEvenFlag);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetAutoCompletion", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetAutoCompletion(IntPtr i, IntPtr Address, IntPtr Zip, Int32 mode, Int32 DPVOnly);
			[DllImport("mdAddr", EntryPoint = "mdStreetResetAutoCompletion", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdStreetResetAutoCompletion(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetBaseAlternateIndicator", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetBaseAlternateIndicator(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetLACSIndicator", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetLACSIndicator(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetUrbanizationCode", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetUrbanizationCode(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetUrbanizationName", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetUrbanizationName(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetLastLineNumber", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetLastLineNumber(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetAddressType", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetAddressType(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetCongressionalDistrict", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetCongressionalDistrict(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetCountyFips", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetCountyFips(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetCompany", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetCompany(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetCarrierRoute", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetCarrierRoute(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetZip", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetZip(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetDeliveryInstallation", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetDeliveryInstallation(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetPlus4High", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetPlus4High(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetPlus4Low", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetPlus4Low(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetSuiteRangeOddEven", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetSuiteRangeOddEven(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetSuiteRangeHigh", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetSuiteRangeHigh(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetSuiteRangeLow", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetSuiteRangeLow(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetSuiteName", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetSuiteName(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetPostDirection", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetPostDirection(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetSuffix", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetSuffix(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetStreetName", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetStreetName(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetPreDirection", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetPreDirection(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetPrimaryRangeOddEven", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetPrimaryRangeOddEven(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetPrimaryRangeHigh", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetPrimaryRangeHigh(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdStreetGetPrimaryRangeLow", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdStreetGetPrimaryRangeLow(IntPtr i);
		}

		/// <summary>Constructs a new StreetData instance</summary>
		public mdStreet() {
			i = mdStreetUnmanaged.mdStreetCreate();
		}

		/// <summary>Destroys an existing StreetData instance</summary>
		~mdStreet() {
			Dispose();
		}

		public virtual void Dispose() {
			lock (this) {
				mdStreetUnmanaged.mdStreetDestroy(i);
				GC.SuppressFinalize(this);
			}
		}

		/// <summary>Opens the required data files and prepares the StreetData interface for use</summary>
		/// <param name="DataPath">Path to the mdAddr.dat file</param>
		/// <param name="NationalPath">Path to the mdAddr.nat file</param>
		/// <param name="RegionalPath">Deprecated; pass an empty string if required by the language</param>
		/// <returns>ProgramStatus enum - ErrorNone (0) on success; use GetInitializeErrorString to retrieve the nature of any other error</returns>
		public ProgramStatus Initialize(string DataPath, string NationalPath, string RegionalPath) {
			EncodedString u_DataPath = new EncodedString(DataPath);
			EncodedString u_NationalPath = new EncodedString(NationalPath);
			EncodedString u_RegionalPath = new EncodedString(RegionalPath);
			return (ProgramStatus)mdStreetUnmanaged.mdStreetInitialize(i, u_DataPath.GetPtr(), u_NationalPath.GetPtr(), u_RegionalPath.GetPtr());
		}

		/// <summary>Returns a description of the error from the last Initialize call</summary>
		/// <returns>Error description string</returns>
		public string GetInitializeErrorString() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetInitializeErrorString(i));
		}

		/// <summary>Returns the date of the US address data files</summary>
		/// <returns>Database date string in yyyy-mm-dd format</returns>
		public string GetDatabaseDate() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetDatabaseDate(i));
		}

		/// <summary>Returns the current build number of Address Object</summary>
		/// <returns>Build number string</returns>
		public string GetBuildNumber() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetBuildNumber(i));
		}

		/// <summary>Sets the license string required to enable full StreetData functionality</summary>
		/// <param name="License">The software license key</param>
		/// <returns>True (1) if the specified license is correct</returns>
		public bool SetLicenseString(string License) {
			EncodedString u_License = new EncodedString(License);
			return (mdStreetUnmanaged.mdStreetSetLicenseString(i, u_License.GetPtr()) != 0);
		}

		/// <summary>Returns the expiration date of the current license string</summary>
		/// <returns>License expiration date string</returns>
		public string GetLicenseExpirationDate() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetLicenseExpirationDate(i));
		}

		/// <summary>Retrieves the first address range matching the given street name pattern and ZIP code</summary>
		/// <param name="StreetName">Street name to search; wildcards allowed at end (e.g. "MA*")</param>
		/// <param name="ZIP">Five-digit ZIP code to search within</param>
		/// <param name="CodeOnly">If true, restricts results to the exact ZIP code; if false, includes the surrounding city</param>
		/// <returns>True (1) if a matching record was found</returns>
		public bool FindStreet(string StreetName, string ZIP, bool CodeOnly) {
			EncodedString u_StreetName = new EncodedString(StreetName);
			EncodedString u_ZIP = new EncodedString(ZIP);
			return (mdStreetUnmanaged.mdStreetFindStreet(i, u_StreetName.GetPtr(), u_ZIP.GetPtr(), (CodeOnly ? 1 : 0)) != 0);
		}

		/// <summary>Retrieves the next matching address range after a successful FindStreet call</summary>
		/// <returns>True (1) if another record was found, false (0) if no more records</returns>
		public bool FindStreetNext() {
			return (mdStreetUnmanaged.mdStreetFindStreetNext(i) != 0);
		}

		/// <summary>Determines if an address number falls within a street address range</summary>
		/// <param name="InputRange">Address number to test</param>
		/// <param name="LoRange">Low end of the valid range</param>
		/// <param name="HiRange">High end of the valid range</param>
		/// <returns>True (1) if the address falls within the range</returns>
	[Obsolete("Use IsAddressInRange2 instead.")]
		public bool IsAddressInRange(string InputRange, string LoRange, string HiRange) {
			EncodedString u_InputRange = new EncodedString(InputRange);
			EncodedString u_LoRange = new EncodedString(LoRange);
			EncodedString u_HiRange = new EncodedString(HiRange);
			return (mdStreetUnmanaged.mdStreetIsAddressInRange(i, u_InputRange.GetPtr(), u_LoRange.GetPtr(), u_HiRange.GetPtr()) != 0);
		}

		/// <summary>Determines if an address number falls within a street address range and matches the odd/even indicator. Supersedes the older IsAddressInRange function, which did not account for odd/even ranges</summary>
		/// <param name="InputRange">Address number to test (from input address)</param>
		/// <param name="LoRange">Low end of the valid range (from GetPrimaryRangeLow)</param>
		/// <param name="HiRange">High end of the valid range (from GetPrimaryRangeHigh)</param>
		/// <param name="OddEvenFlag">Odd/even indicator from GetPrimaryRangeOddEven</param>
		/// <returns>True (1) if the address is within range and matches the odd/even indicator</returns>
		public bool IsAddressInRange2(string InputRange, string LoRange, string HiRange, string OddEvenFlag) {
			EncodedString u_InputRange = new EncodedString(InputRange);
			EncodedString u_LoRange = new EncodedString(LoRange);
			EncodedString u_HiRange = new EncodedString(HiRange);
			EncodedString u_OddEvenFlag = new EncodedString(OddEvenFlag);
			return (mdStreetUnmanaged.mdStreetIsAddressInRange2(i, u_InputRange.GetPtr(), u_LoRange.GetPtr(), u_HiRange.GetPtr(), u_OddEvenFlag.GetPtr()) != 0);
		}

		/// <summary>Returns one possible street address suggestion for auto-complete based on partial address and ZIP code</summary>
		/// <param name="Address">Full or partial street address input</param>
		/// <param name="Zip">Five-digit US ZIP code</param>
		/// <param name="mode">AutoCompletionMode enum controlling suite display (SingleSuite, RangedSuite, PlaceHolderSuite, NoSuite)</param>
		/// <param name="DPVOnly">If true, returns only DPV-confirmed addresses</param>
		/// <returns>Suggested address string, or empty string when no more suggestions remain</returns>
		public string GetAutoCompletion(string Address, string Zip, AutoCompletionMode mode, bool DPVOnly) {
			EncodedString u_Address = new EncodedString(Address);
			EncodedString u_Zip = new EncodedString(Zip);
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetAutoCompletion(i, u_Address.GetPtr(), u_Zip.GetPtr(), (int)mode, (DPVOnly ? 1 : 0)));
		}

		/// <summary>Resets the auto-completion state to prepare for a new address lookup</summary>
		public void ResetAutoCompletion() {
			mdStreetUnmanaged.mdStreetResetAutoCompletion(i);
		}

		/// <summary>Returns whether the matched street record is a base or alternate record</summary>
		/// <returns>"B" for base (preferred) record, "A" for alternate record</returns>
		public string GetBaseAlternateIndicator() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetBaseAlternateIndicator(i));
		}

		/// <summary>Returns whether the street address has undergone a rural-to-city-style conversion</summary>
		/// <returns>"L" if converted, empty if not</returns>
		public string GetLACSIndicator() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetLACSIndicator(i));
		}

		/// <summary>Returns the urbanization code for Puerto Rican street records</summary>
		/// <returns>Urbanization code string (max 6 characters)</returns>
		public string GetUrbanizationCode() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetUrbanizationCode(i));
		}

		/// <summary>Returns the urbanization name for Puerto Rican street records</summary>
		/// <returns>Urbanization name string</returns>
		public string GetUrbanizationName() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetUrbanizationName(i));
		}

		/// <summary>Returns the last line number used for advanced address matching tie-breaking</summary>
		/// <returns>Last line number string (max 6 characters)</returns>
		public string GetLastLineNumber() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetLastLineNumber(i));
		}

		/// <summary>Returns the address type code for the matched street record</summary>
		/// <returns>S=Street, P=PO Box, H=High Rise, R=Rural Route, F=Firm, G=General Delivery</returns>
		public string GetAddressType() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetAddressType(i));
		}

		/// <summary>Returns the congressional district for the matched street record</summary>
		/// <returns>State abbreviation plus two-digit district number (e.g. "CA01")</returns>
		public string GetCongressionalDistrict() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetCongressionalDistrict(i));
		}

		/// <summary>Returns the five-digit county FIPS code for the matched street record</summary>
		/// <returns>Five-digit FIPS code (e.g. "06037" for Los Angeles CA)</returns>
		public string GetCountyFips() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetCountyFips(i));
		}

		/// <summary>Returns the company or building name associated with the matched street record</summary>
		/// <returns>Company name string (max 40 characters)</returns>
		public string GetCompany() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetCompany(i));
		}

		/// <summary>Returns the carrier route code for the matched street record</summary>
		/// <returns>Four-character carrier route code (e.g. "C027")</returns>
		public string GetCarrierRoute() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetCarrierRoute(i));
		}

		/// <summary>Returns the five-digit ZIP code for the matched street record</summary>
		/// <returns>Five-digit ZIP code string</returns>
		public string GetZip() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetZip(i));
		}

		/// <summary>Returns the delivery installation for matched Canadian street records</summary>
		/// <returns>Delivery installation string</returns>
		public string GetDeliveryInstallation() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetDeliveryInstallation(i));
		}

		/// <summary>Returns the highest Plus4 code in the range for the matched street record</summary>
		/// <returns>Four-digit Plus4 high value string</returns>
		public string GetPlus4High() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetPlus4High(i));
		}

		/// <summary>Returns the lowest Plus4 code in the range for the matched street record</summary>
		/// <returns>Four-digit Plus4 low value string</returns>
		public string GetPlus4Low() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetPlus4Low(i));
		}

		/// <summary>Returns the odd/even indicator for the suite range of the matched street record</summary>
		/// <returns>"O" for odd only, "E" for even only, "B" for both</returns>
		public string GetSuiteRangeOddEven() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetSuiteRangeOddEven(i));
		}

		/// <summary>Returns the highest suite number in the range for the matched street record</summary>
		/// <returns>Suite range high value string (max 8 characters)</returns>
		public string GetSuiteRangeHigh() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetSuiteRangeHigh(i));
		}

		/// <summary>Returns the lowest suite number in the range for the matched street record</summary>
		/// <returns>Suite range low value string (max 8 characters)</returns>
		public string GetSuiteRangeLow() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetSuiteRangeLow(i));
		}

		/// <summary>Returns the suite designator for the matched street record</summary>
		/// <returns>Suite designator string (e.g. "APT", "STE", "UNIT")</returns>
		public string GetSuiteName() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetSuiteName(i));
		}

		/// <summary>Returns the directional following the street name for the matched street record</summary>
		/// <returns>Post-directional string (e.g. "N", "NE", "SW")</returns>
		public string GetPostDirection() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetPostDirection(i));
		}

		/// <summary>Returns the street suffix for the matched street record</summary>
		/// <returns>Street suffix string (e.g. "ST", "AVE", "BLVD")</returns>
		public string GetSuffix() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetSuffix(i));
		}

		/// <summary>Returns the street name for the matched street record</summary>
		/// <returns>Street name string (max 28 characters)</returns>
		public string GetStreetName() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetStreetName(i));
		}

		/// <summary>Returns the directional preceding the street name for the matched street record</summary>
		/// <returns>Pre-directional string (e.g. "N", "NE", "SW")</returns>
		public string GetPreDirection() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetPreDirection(i));
		}

		/// <summary>Returns the odd/even indicator for the primary address range of the matched street record</summary>
		/// <returns>"O" for odd only, "E" for even only, "B" for both</returns>
		public string GetPrimaryRangeOddEven() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetPrimaryRangeOddEven(i));
		}

		/// <summary>Returns the highest street number in the primary address range of the matched record</summary>
		/// <returns>Primary range high value string (10 digits, zero-padded)</returns>
		public string GetPrimaryRangeHigh() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetPrimaryRangeHigh(i));
		}

		/// <summary>Returns the lowest street number in the primary address range of the matched record</summary>
		/// <returns>Primary range low value string (10 digits, zero-padded)</returns>
		public string GetPrimaryRangeLow() {
			return EncodedString.GetEncodedString(mdStreetUnmanaged.mdStreetGetPrimaryRangeLow(i));
		}

		private class EncodedString : IDisposable {
			private IntPtr encodedString = IntPtr.Zero;
			private static Encoding encoding = Encoding.GetEncoding("ISO-8859-1");

			public EncodedString(string str) {
				if (str == null)
					str = "";
				byte[] buffer = encoding.GetBytes(str);
				Array.Resize(ref buffer, buffer.Length + 1);
				buffer[buffer.Length - 1] = 0;
				encodedString = Marshal.AllocHGlobal(buffer.Length);
				Marshal.Copy(buffer, 0, encodedString, buffer.Length);
			}

			~EncodedString() {
				Dispose();
			}

			public virtual void Dispose() {
				lock (this) {
					Marshal.FreeHGlobal(encodedString);
					GC.SuppressFinalize(this);
				}
			}

			public IntPtr GetPtr() {
				return encodedString;
			}

			public static string GetEncodedString(IntPtr ptr) {
				if (ptr == IntPtr.Zero)
					return "";
				int len = 0;
				while (Marshal.ReadByte(ptr, len) != 0)
					len++;
				if (len == 0)
					return "";
				byte[] buffer = new byte[len];
				Marshal.Copy(ptr, buffer, 0, len);
				return encoding.GetString(buffer);
			}
		}
	}

	/// <summary>Looks up geographic data, city names, and postal information by ZIP code or city name</summary>
	/// <remarks>Use FindZip, FindZipInCity, and FindCityInState to retrieve ZIP code and city records</remarks>
	public class mdZip : IDisposable {
		private IntPtr i;

		/// <summary>Programming, installation, and execution error codes returned by Initialize</summary>
		public enum ProgramStatus {
			/// <summary>No error - initialization was successful</summary>
			ErrorNone = 0,
			/// <summary>Could not open the mdAddr.dat file</summary>
			ErrorOther = 1,
			/// <summary>Out of memory</summary>
			ErrorOutOfMemory = 2,
			/// <summary>A required data file could not be found</summary>
			ErrorRequiredFileNotFound = 3,
			/// <summary>Conflicts found between two or more data files</summary>
			ErrorFoundOldFile = 4,
			/// <summary>The database has expired</summary>
			ErrorDatabaseExpired = 5,
			/// <summary>The license has expired</summary>
			ErrorLicenseExpired = 6
		}

		[SuppressUnmanagedCodeSecurity]
		private class mdZipUnmanaged {
			[DllImport("mdAddr", EntryPoint = "mdZipCreate", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdZipCreate();
			[DllImport("mdAddr", EntryPoint = "mdZipDestroy", CallingConvention = CallingConvention.Cdecl)]
			public static extern void mdZipDestroy(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdZipInitialize", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdZipInitialize(IntPtr i, IntPtr DataPath, IntPtr NationalPath, IntPtr RegionalPath);
			[DllImport("mdAddr", EntryPoint = "mdZipGetInitializeErrorString", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdZipGetInitializeErrorString(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdZipGetDatabaseDate", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdZipGetDatabaseDate(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdZipGetBuildNumber", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdZipGetBuildNumber(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdZipSetLicenseString", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdZipSetLicenseString(IntPtr i, IntPtr License);
			[DllImport("mdAddr", EntryPoint = "mdZipGetLicenseExpirationDate", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdZipGetLicenseExpirationDate(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdZipFindZip", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdZipFindZip(IntPtr i, IntPtr Zip, Int32 GetLastLineRecord);
			[DllImport("mdAddr", EntryPoint = "mdZipFindZipNext", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdZipFindZipNext(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdZipFindZipInCity", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdZipFindZipInCity(IntPtr i, IntPtr City, IntPtr State);
			[DllImport("mdAddr", EntryPoint = "mdZipFindZipInCityNext", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdZipFindZipInCityNext(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdZipFindCityInState", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdZipFindCityInState(IntPtr i, IntPtr City, IntPtr State);
			[DllImport("mdAddr", EntryPoint = "mdZipFindCityInStateNext", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdZipFindCityInStateNext(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdZipComputeDistance", CallingConvention = CallingConvention.Cdecl)]
			public static extern double mdZipComputeDistance(IntPtr i, double lat1, double long1, double lat2, double long2);
			[DllImport("mdAddr", EntryPoint = "mdZipComputeBearing", CallingConvention = CallingConvention.Cdecl)]
			public static extern double mdZipComputeBearing(IntPtr i, double lat1, double long1, double lat2, double long2);
			[DllImport("mdAddr", EntryPoint = "mdZipGetCountyNameFromFips", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdZipGetCountyNameFromFips(IntPtr i, IntPtr FipsCode);
			[DllImport("mdAddr", EntryPoint = "mdZipGetSCFArea", CallingConvention = CallingConvention.Cdecl)]
			public static extern Int32 mdZipGetSCFArea(IntPtr i, IntPtr Zip, IntPtr LatN, IntPtr LatS, IntPtr LonE, IntPtr LonW);
			[DllImport("mdAddr", EntryPoint = "mdZipGetZip", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdZipGetZip(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdZipGetCity", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdZipGetCity(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdZipGetCityAbbreviation", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdZipGetCityAbbreviation(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdZipGetState", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdZipGetState(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdZipGetZipType", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdZipGetZipType(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdZipGetCountyName", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdZipGetCountyName(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdZipGetCountyFips", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdZipGetCountyFips(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdZipGetAreaCode", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdZipGetAreaCode(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdZipGetLongitude", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdZipGetLongitude(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdZipGetLatitude", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdZipGetLatitude(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdZipGetTimeZone", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdZipGetTimeZone(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdZipGetTimeZoneCode", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdZipGetTimeZoneCode(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdZipGetMsa", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdZipGetMsa(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdZipGetPmsa", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdZipGetPmsa(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdZipGetFacilityCode", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdZipGetFacilityCode(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdZipGetLastLineIndicator", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdZipGetLastLineIndicator(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdZipGetLastLineNumber", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdZipGetLastLineNumber(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdZipGetPreferredLastLineNumber", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdZipGetPreferredLastLineNumber(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdZipGetAutomation", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdZipGetAutomation(IntPtr i);
			[DllImport("mdAddr", EntryPoint = "mdZipGetFinanceNumber", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr mdZipGetFinanceNumber(IntPtr i);
		}

		/// <summary>Constructs a new ZipData instance</summary>
		public mdZip() {
			i = mdZipUnmanaged.mdZipCreate();
		}

		/// <summary>Destroys an existing ZipData instance</summary>
		~mdZip() {
			Dispose();
		}

		public virtual void Dispose() {
			lock (this) {
				mdZipUnmanaged.mdZipDestroy(i);
				GC.SuppressFinalize(this);
			}
		}

		/// <summary>Opens the required data files and prepares the ZipData interface for use</summary>
		/// <param name="DataPath">Path to the mdAddr.dat file</param>
		/// <param name="NationalPath">Path to the mdAddr.nat file</param>
		/// <param name="RegionalPath">Deprecated; pass an empty string if required by the language</param>
		/// <returns>ProgramStatus enum - ErrorNone (0) on success; use GetInitializeErrorString to retrieve the nature of any other error</returns>
		public ProgramStatus Initialize(string DataPath, string NationalPath, string RegionalPath) {
			EncodedString u_DataPath = new EncodedString(DataPath);
			EncodedString u_NationalPath = new EncodedString(NationalPath);
			EncodedString u_RegionalPath = new EncodedString(RegionalPath);
			return (ProgramStatus)mdZipUnmanaged.mdZipInitialize(i, u_DataPath.GetPtr(), u_NationalPath.GetPtr(), u_RegionalPath.GetPtr());
		}

		/// <summary>Returns a description of the error from the last Initialize call</summary>
		/// <returns>Error description string</returns>
		public string GetInitializeErrorString() {
			return EncodedString.GetEncodedString(mdZipUnmanaged.mdZipGetInitializeErrorString(i));
		}

		/// <summary>Returns the date of the US address data files</summary>
		/// <returns>Database date string in yyyy-mm-dd format</returns>
		public string GetDatabaseDate() {
			return EncodedString.GetEncodedString(mdZipUnmanaged.mdZipGetDatabaseDate(i));
		}

		/// <summary>Returns the current build number of Address Object</summary>
		/// <returns>Build number string</returns>
		public string GetBuildNumber() {
			return EncodedString.GetEncodedString(mdZipUnmanaged.mdZipGetBuildNumber(i));
		}

		/// <summary>Sets the license string required to enable full ZipData functionality</summary>
		/// <param name="License">The software license key</param>
		/// <returns>True (1) if the specified license is correct</returns>
		public bool SetLicenseString(string License) {
			EncodedString u_License = new EncodedString(License);
			return (mdZipUnmanaged.mdZipSetLicenseString(i, u_License.GetPtr()) != 0);
		}

		/// <summary>Returns the expiration date of the current license string</summary>
		/// <returns>License expiration date string</returns>
		public string GetLicenseExpirationDate() {
			return EncodedString.GetEncodedString(mdZipUnmanaged.mdZipGetLicenseExpirationDate(i));
		}

		/// <summary>Retrieves the first city record for the specified ZIP code</summary>
		/// <param name="Zip">Five-digit ZIP code to look up</param>
		/// <param name="GetLastLineRecord">If true, returns only the official preferred city name record; disables FindZipNext</param>
		/// <returns>True (1) if a record was found</returns>
		public bool FindZip(string Zip, bool GetLastLineRecord) {
			EncodedString u_Zip = new EncodedString(Zip);
			return (mdZipUnmanaged.mdZipFindZip(i, u_Zip.GetPtr(), (GetLastLineRecord ? 1 : 0)) != 0);
		}

		/// <summary>Retrieves the next city record for the ZIP code passed to FindZip</summary>
		/// <returns>True (1) if another record was found, false (0) if no more records</returns>
		public bool FindZipNext() {
			return (mdZipUnmanaged.mdZipFindZipNext(i) != 0);
		}

		/// <summary>Retrieves the first ZIP code record for the specified city and state</summary>
		/// <param name="City">City name to look up (max 28 characters; wildcards allowed with *)</param>
		/// <param name="State">Two-character state abbreviation</param>
		/// <returns>True (1) if a record was found</returns>
		public bool FindZipInCity(string City, string State) {
			EncodedString u_City = new EncodedString(City);
			EncodedString u_State = new EncodedString(State);
			return (mdZipUnmanaged.mdZipFindZipInCity(i, u_City.GetPtr(), u_State.GetPtr()) != 0);
		}

		/// <summary>Retrieves the next ZIP code record for the city and state passed to FindZipInCity</summary>
		/// <returns>True (1) if another record was found, false (0) if no more records</returns>
		public bool FindZipInCityNext() {
			return (mdZipUnmanaged.mdZipFindZipInCityNext(i) != 0);
		}

		/// <summary>Retrieves the first city record matching the given city name and state</summary>
		/// <param name="City">City name to search (max 28 characters; wildcards allowed with *)</param>
		/// <param name="State">Two-character state abbreviation</param>
		/// <returns>True (1) if a match was found</returns>
		public bool FindCityInState(string City, string State) {
			EncodedString u_City = new EncodedString(City);
			EncodedString u_State = new EncodedString(State);
			return (mdZipUnmanaged.mdZipFindCityInState(i, u_City.GetPtr(), u_State.GetPtr()) != 0);
		}

		/// <summary>Retrieves the next matching city record after a successful FindCityInState call</summary>
		/// <returns>True (1) if another match was found, false (0) if no more records</returns>
		public bool FindCityInStateNext() {
			return (mdZipUnmanaged.mdZipFindCityInStateNext(i) != 0);
		}

		/// <summary>Computes the distance in miles between two geographic coordinates</summary>
		/// <param name="lat1">Latitude of point 1 in degrees</param>
		/// <param name="long1">Longitude of point 1 in degrees (negative for US)</param>
		/// <param name="lat2">Latitude of point 2 in degrees</param>
		/// <param name="long2">Longitude of point 2 in degrees (negative for US)</param>
		/// <returns>Distance in miles, or 9999 if coordinates are invalid</returns>
		public double ComputeDistance(double lat1, double long1, double lat2, double long2) {
			return mdZipUnmanaged.mdZipComputeDistance(i, lat1, long1, lat2, long2);
		}

		/// <summary>Computes the compass bearing in degrees from point 1 to point 2</summary>
		/// <param name="lat1">Latitude of point 1 in degrees</param>
		/// <param name="long1">Longitude of point 1 in degrees (negative for US)</param>
		/// <param name="lat2">Latitude of point 2 in degrees</param>
		/// <param name="long2">Longitude of point 2 in degrees (negative for US)</param>
		/// <returns>Bearing in degrees (0=North, 90=East, 180=South, 270=West), or 999 if coordinates are invalid</returns>
		public double ComputeBearing(double lat1, double long1, double lat2, double long2) {
			return mdZipUnmanaged.mdZipComputeBearing(i, lat1, long1, lat2, long2);
		}

		/// <summary>Returns the county name for a given county FIPS code</summary>
		/// <param name="FipsCode">Five-digit county FIPS code</param>
		/// <returns>County name string</returns>
		public string GetCountyNameFromFips(string FipsCode) {
			EncodedString u_FipsCode = new EncodedString(FipsCode);
			return EncodedString.GetEncodedString(mdZipUnmanaged.mdZipGetCountyNameFromFips(i, u_FipsCode.GetPtr()));
		}

		/// <summary>Returns the geographic bounding box (north, south, east, west) for a Sectional Center Facility area</summary>
		/// <param name="Zip">ZIP code to look up the SCF area for</param>
		/// <param name="LatN">Pointer to receive the northern latitude boundary</param>
		/// <param name="LatS">Pointer to receive the southern latitude boundary</param>
		/// <param name="LonE">Pointer to receive the eastern longitude boundary</param>
		/// <param name="LonW">Pointer to receive the western longitude boundary</param>
		/// <returns>Number of ZIP codes in the SCF area</returns>
		public int GetSCFArea(string Zip, IntPtr LatN, IntPtr LatS, IntPtr LonE, IntPtr LonW) {
			EncodedString u_Zip = new EncodedString(Zip);
			return mdZipUnmanaged.mdZipGetSCFArea(i, u_Zip.GetPtr(), LatN, LatS, LonE, LonW);
		}

		/// <summary>Returns the five-digit ZIP code from the current ZipData record</summary>
		/// <returns>Five-digit ZIP code string</returns>
		public string GetZip() {
			return EncodedString.GetEncodedString(mdZipUnmanaged.mdZipGetZip(i));
		}

		/// <summary>Returns the city name from the current ZipData record</summary>
		/// <returns>City name string (max 28 characters)</returns>
		public string GetCity() {
			return EncodedString.GetEncodedString(mdZipUnmanaged.mdZipGetCity(i));
		}

		/// <summary>Returns the official USPS 13-character city name abbreviation from the current record</summary>
		/// <returns>City abbreviation (full city name if 13 characters or fewer)</returns>
		public string GetCityAbbreviation() {
			return EncodedString.GetEncodedString(mdZipUnmanaged.mdZipGetCityAbbreviation(i));
		}

		/// <summary>Returns the two-letter state abbreviation from the current ZipData record</summary>
		/// <returns>Two-letter state abbreviation string</returns>
		public string GetState() {
			return EncodedString.GetEncodedString(mdZipUnmanaged.mdZipGetState(i));
		}

		/// <summary>Returns the ZIP code type for the current record</summary>
		/// <returns>"P" for PO Box only, "U" for unique/institutional, "M" for military, empty for standard</returns>
		public string GetZipType() {
			return EncodedString.GetEncodedString(mdZipUnmanaged.mdZipGetZipType(i));
		}

		/// <summary>Returns the county name for the current ZipData record</summary>
		/// <returns>County name string (max 25 characters)</returns>
		public string GetCountyName() {
			return EncodedString.GetEncodedString(mdZipUnmanaged.mdZipGetCountyName(i));
		}

		/// <summary>Returns the five-digit county FIPS code for the current ZipData record</summary>
		/// <returns>Five-digit FIPS code string</returns>
		public string GetCountyFips() {
			return EncodedString.GetEncodedString(mdZipUnmanaged.mdZipGetCountyFips(i));
		}

		/// <summary>Returns the dominant telephone area code for the ZIP code</summary>
		/// <returns>Three-digit area code string</returns>
		public string GetAreaCode() {
			return EncodedString.GetEncodedString(mdZipUnmanaged.mdZipGetAreaCode(i));
		}

		/// <summary>Returns the longitude of the geographic center of the ZIP code</summary>
		/// <returns>Longitude string accurate to 6 decimal places (negative for US)</returns>
		public string GetLongitude() {
			return EncodedString.GetEncodedString(mdZipUnmanaged.mdZipGetLongitude(i));
		}

		/// <summary>Returns the latitude of the geographic center of the ZIP code</summary>
		/// <returns>Latitude string accurate to 4 decimal places</returns>
		public string GetLatitude() {
			return EncodedString.GetEncodedString(mdZipUnmanaged.mdZipGetLatitude(i));
		}

		/// <summary>Returns the time zone name for the ZIP code</summary>
		/// <returns>Time zone name string (e.g. "Eastern Time", "Pacific Time")</returns>
		public string GetTimeZone() {
			return EncodedString.GetEncodedString(mdZipUnmanaged.mdZipGetTimeZone(i));
		}

		/// <summary>Returns the numeric time zone code for the ZIP code</summary>
		/// <returns>Time zone code (e.g. "5" for Eastern, "8" for Pacific)</returns>
		public string GetTimeZoneCode() {
			return EncodedString.GetEncodedString(mdZipUnmanaged.mdZipGetTimeZoneCode(i));
		}

		/// <summary>Returns the Metropolitan Statistical Area (MSA) code for the ZIP code</summary>
		/// <returns>MSA code string</returns>
		public string GetMsa() {
			return EncodedString.GetEncodedString(mdZipUnmanaged.mdZipGetMsa(i));
		}

		/// <summary>Returns the Primary Metropolitan Statistical Area (PMSA) code for the ZIP code</summary>
		/// <returns>PMSA code string</returns>
		public string GetPmsa() {
			return EncodedString.GetEncodedString(mdZipUnmanaged.mdZipGetPmsa(i));
		}

		/// <summary>Returns the USPS postal facility type code for the ZIP code</summary>
		/// <returns>A=Airport Mail Facility, B=Branch, C=Community Post Office, D=Area Distribution Center, E=Sectional Center Facility, F=Delivery Distribution Center, G=General Mail Facility, K=Bulk Mail Facility, M=Money Order Unit, N=Community or Place name, P=Post Office, S=Station, U=Urbanization (Puerto Rico), X=Vanity name (should not be used). Not returned for Canadian postal codes</returns>
		public string GetFacilityCode() {
			return EncodedString.GetEncodedString(mdZipUnmanaged.mdZipGetFacilityCode(i));
		}

		/// <summary>Returns whether the current city name is the official USPS last-line name for the ZIP code. Not returned for Canadian postal codes</summary>
		/// <returns>"L" if this is the official last-line city (only one record per ZIP Code is coded with "L"), empty otherwise</returns>
		public string GetLastLineIndicator() {
			return EncodedString.GetEncodedString(mdZipUnmanaged.mdZipGetLastLineIndicator(i));
		}

		/// <summary>Returns the last line number used for advanced address matching tie-breaking. Not returned for Canadian postal codes</summary>
		/// <returns>Six-character last line number string</returns>
		public string GetLastLineNumber() {
			return EncodedString.GetEncodedString(mdZipUnmanaged.mdZipGetLastLineNumber(i));
		}

		/// <summary>Returns the preferred last line number for the current city record</summary>
		/// <returns>Six-character preferred last line number string</returns>
		public string GetPreferredLastLineNumber() {
			return EncodedString.GetEncodedString(mdZipUnmanaged.mdZipGetPreferredLastLineNumber(i));
		}

		/// <summary>Returns the carrier route rate mail indicator for letter-size mail in the ZIP code. Not returned for Canadian postal codes</summary>
		/// <returns>"A" carrier route rates apply and merging is permitted, "B" carrier route rates apply and merging is not permitted, "C" carrier route rates do not apply and merging is permitted, "D" carrier route rates do not apply and merging is not permitted</returns>
		public string GetAutomation() {
			return EncodedString.GetEncodedString(mdZipUnmanaged.mdZipGetAutomation(i));
		}

		/// <summary>Returns the USPS finance number for the postal facility associated with the ZIP code</summary>
		/// <returns>Finance number string</returns>
		public string GetFinanceNumber() {
			return EncodedString.GetEncodedString(mdZipUnmanaged.mdZipGetFinanceNumber(i));
		}

		private class EncodedString : IDisposable {
			private IntPtr encodedString = IntPtr.Zero;
			private static Encoding encoding = Encoding.GetEncoding("ISO-8859-1");

			public EncodedString(string str) {
				if (str == null)
					str = "";
				byte[] buffer = encoding.GetBytes(str);
				Array.Resize(ref buffer, buffer.Length + 1);
				buffer[buffer.Length - 1] = 0;
				encodedString = Marshal.AllocHGlobal(buffer.Length);
				Marshal.Copy(buffer, 0, encodedString, buffer.Length);
			}

			~EncodedString() {
				Dispose();
			}

			public virtual void Dispose() {
				lock (this) {
					Marshal.FreeHGlobal(encodedString);
					GC.SuppressFinalize(this);
				}
			}

			public IntPtr GetPtr() {
				return encodedString;
			}

			public static string GetEncodedString(IntPtr ptr) {
				if (ptr == IntPtr.Zero)
					return "";
				int len = 0;
				while (Marshal.ReadByte(ptr, len) != 0)
					len++;
				if (len == 0)
					return "";
				byte[] buffer = new byte[len];
				Marshal.Copy(ptr, buffer, 0, len);
				return encoding.GetString(buffer);
			}
		}
	}
}
