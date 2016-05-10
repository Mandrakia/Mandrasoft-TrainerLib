#include <metahost.h>
#include <iostream>
#include <sstream>
#include <string>
#include <vector>
using std::wstring;
using std::vector;

#pragma comment(lib, "mscoree.lib")

#import "mscorlib.tlb" raw_interfaces_only \
    high_property_prefixes("_get","_put","_putref") \
    rename("ReportEvent", "InteropServices_ReportEvent")
std::vector<wstring> splitManyW(const wstring &original, const wstring &delimiters)
{
	std::wstringstream stream(original);
	std::wstring line;
	vector <wstring> wordVector;

	while (std::getline(stream, line))
	{
		std::size_t prev = 0, pos;
		while ((pos = line.find_first_of(delimiters, prev)) != std::wstring::npos)
		{
			if (pos > prev)
				wordVector.emplace_back(line.substr(prev, pos - prev));

			prev = pos + 1;
		}

		if (prev < line.length())
			wordVector.emplace_back(line.substr(prev, std::wstring::npos));
	}

	return wordVector;
}
ICLRRuntimeHost* GetRuntimeHost(LPCWSTR dotNetVersion)
{
	ICLRMetaHost* metaHost = NULL;
	ICLRRuntimeInfo* info = NULL;
	ICLRRuntimeHost* runtimeHost = NULL;

	// Get the CLRMetaHost that tells us about .NET on this machine
	if (S_OK == CLRCreateInstance(CLSID_CLRMetaHost, IID_ICLRMetaHost, (LPVOID*)&metaHost))
	{
		// Get the runtime information for the particular version of .NET
		if (S_OK == metaHost->GetRuntime(dotNetVersion, IID_ICLRRuntimeInfo, (LPVOID*)&info))
		{
			// Get the actual host
			if (S_OK == info->GetInterface(CLSID_CLRRuntimeHost, IID_ICLRRuntimeHost, (LPVOID*)&runtimeHost))
			{
				// Start it. This is okay to call even if the CLR is already running
				runtimeHost->Start();
			}
		}
	}
	if (NULL != info)
		info->Release();
	if (NULL != metaHost)
		metaHost->Release();

	return runtimeHost;
}
int ExecuteClrCode(ICLRRuntimeHost* host, LPCWSTR assemblyPath, LPCWSTR typeName,
	LPCWSTR function, LPCWSTR param)
{
	if (NULL == host)
		return -1;

	DWORD result = -1;
	if (S_OK != host->ExecuteInDefaultAppDomain(assemblyPath, typeName, function, param, &result))
		return -1;

	return result;
}
__declspec(dllexport) int bootLoad(LPCWSTR args)
{
	wstring separators = L"|";
	vector<wstring> results = splitManyW(args, separators);
	LPCWSTR assemblyPath = results[0].c_str();
	LPCWSTR typeName = results[1].c_str();
	LPCWSTR function = results[2].c_str();
	LPCWSTR param = results[3].c_str();
	ICLRRuntimeHost *pClrRuntimeHost = GetRuntimeHost(L"v4.0.30319");
	ExecuteClrCode(pClrRuntimeHost, assemblyPath, typeName, function, param);
	pClrRuntimeHost->Release();
	return 1;
}

