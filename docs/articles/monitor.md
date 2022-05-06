# Active Login Monitor

When using the `BankIdApplicationInsightsEventListener` (`builder.AddApplicationInsightsEventListener()`) structured events from the Active Login flow will be published to Application Insights (Azure Monitor). Below are samples on how to query these using [KQL](https://docs.microsoft.com/en-us/sharepoint/dev/general-development/keyword-query-language-kql-syntax-reference).

## Dashboard
In our provisioning samples we include a way to deploy an Azure Dashboard displaying some of the most important metrics and queries from below. See [ActiveLogin-Monitor.json](https://github.com/ActiveLogin/ActiveLogin.Authentication/blob/main/samples/AzureProvisioningSample/ActiveLogin-Monitor.json).
<!--(/samples/AzureProvisioningSample/ActiveLogin-Monitor.json)-->

![Active Login Monitor](https://alresourcesprod.blob.core.windows.net/docsassets/active-login-monitor-screenshot_1.png)

# KQL Samples

## All info
```kql
customEvents
| where name startswith "ActiveLogin_BankId_"
| project
    timestamp,
    client_City,
    client_CountryOrRegion,
    Event_Name = name,
    Event_TypeId = tostring(customDimensions.AL_Event_TypeId),
    Event_Severity = tostring(customDimensions.AL_Event_Severity),
    Error_ErrorReason = tostring(customDimensions.AL_Error_ErrorReason),
    BankId_BankId_Options_LaunchType = tostring(customDimensions.AL_BankId_Options_LaunchType),
    BankId_Options_UseQrCode = tostring(customDimensions.AL_BankId_Options_UseQrCode),
    BankId_ErrorCode = tostring(customDimensions.AL_BankId_ErrorCode),
    BankId_ErrorDetails = tostring(customDimensions.AL_BankId_ErrorDetails),
    BankId_OrderRef = tostring(customDimensions.AL_BankId_OrderRef),
    BankId_CollectHintCode = tostring(customDimensions.AL_BankId_CollectHintCode),
    BankId_User_CertNotBefore = tostring(customDimensions.AL_BankId_User_CertNotBefore),
    BankId_User_CertNotAfter = tostring(customDimensions.AL_BankId_User_CertNotAfter),
    BankId_User_DeviceIpAddress = tostring(customDimensions.AL_BankId_User_DeviceIpAddress),
    User_Device_Browser = tostring(customDimensions.AL_User_Device_Browser),
    User_Device_Os = tostring(customDimensions.AL_User_Device_Os),
    User_Device_Type = tostring(customDimensions.AL_User_Device_Type),
    User_Device_OsVersion = tostring(customDimensions.AL_User_Device_OsVersion),
    User_Name = tostring(customDimensions.AL_User_Name),
    User_GivenName = tostring(customDimensions.AL_User_GivenName),
    User_Surname = tostring(customDimensions.AL_User_Surname),
    User_SwedishPersonalIdentityNumber = tostring(customDimensions.AL_User_SwedishPersonalIdentityNumber),
    User_DateOfBirthHint = tostring(customDimensions.AL_User_DateOfBirthHint),
    User_AgeHint = tostring(customDimensions.AL_User_AgeHint),
    User_GenderHint = tostring(customDimensions.AL_User_GenderHint),
    ProductName = tostring(customDimensions.AL_ProductName),
    ProductVersion = tostring(customDimensions.AL_ProductVersion),
    BankId_ApiEnvironment = tostring(customDimensions.AL_BankId_ApiEnvironment),
    BankId_ApiVersion = tostring(customDimensions.AL_BankId_ApiVersion)
| order by timestamp desc
| render table
```

## Metadata

### Active Login Version

```kql
customEvents
| where name == "ActiveLogin_BankId_AspNetChallengeSuccess"
| project ActiveLogin_ProductVersion = tostring(customDimensions.AL_ProductVersion)
| summarize count() by ActiveLogin_ProductVersion
| render piechart
```

### Launch Type (SameDevice / OtherDevice)

```kql
customEvents
| where name == "ActiveLogin_BankId_AspNetChallengeSuccess"
| project LaunchType = tostring(customDimensions.AL_BankId_Options_LaunchType)
| summarize count() by LaunchType
| render piechart
```

### Device Type (SameDevice / OtherDevice)

```kql
customEvents
| where name == "ActiveLogin_BankId_AspNetChallengeSuccess"
| project DeviceType = tostring(customDimensions.AL_User_Device_Type)
| summarize count() by DeviceType
| render piechart
```

### Launch Type and Device Type

```kql
customEvents
| where name == "ActiveLogin_BankId_AspNetChallengeSuccess"
| project
    DeviceType = tostring(customDimensions.AL_User_Device_Type),
    LaunchType = tostring(customDimensions.AL_BankId_Options_LaunchType)
| project DeviceTypeAndLaunchType = strcat(DeviceType, ' - ', LaunchType)
| summarize count() by DeviceTypeAndLaunchType
| render piechart
```

### Device Type and Device OS

```kql
customEvents
| where name == "ActiveLogin_BankId_AspNetChallengeSuccess"
| project
    DeviceType = tostring(customDimensions.AL_User_Device_Type),
    DeviceOs = tostring(customDimensions.AL_User_Device_Os)
| project DeviceAndDeviceOs = strcat(DeviceType, ' - ', DeviceOs)
| summarize count() by DeviceAndDeviceOs
| render piechart
```

### Device OS and Device Browser

```kql
customEvents
| where name == "ActiveLogin_BankId_AspNetChallengeSuccess"
| project
    DeviceOs = tostring(customDimensions.AL_User_Device_Os),
    DeviceBrowser = tostring(customDimensions.AL_User_Device_Browser)
| project DeviceOsAndDeviceBrowser = strcat(DeviceOs, ' - ', DeviceBrowser)
| summarize count() by DeviceOsAndDeviceBrowser
| render piechart
```

### Average age

```kql
customEvents
| where name == "ActiveLogin_BankId_CollectCompleted"
| project
    UserAgeHint = toint(customMeasurements.AL_User_AgeHint)
| summarize AverageUserAge = avg(UserAgeHint)
```

## Success

### Succesful logins chart

```kql
customEvents
| where name == "ActiveLogin_BankId_AspNetAuthenticateSuccess"
| project
    timestamp
| summarize Logins = count() by bin(timestamp, 1d)
| render columnchart
```

### Succesful logins by week

```kql
customEvents
| where name == "ActiveLogin_BankId_AspNetAuthenticateSuccess"
| project
    timestamp,
    Year = datetime_part("Year", timestamp),
    Week = week_of_year(timestamp)
| extend
    YearAndWeek = strcat(Year, ' ' , Week)
| order by Year, Week
| summarize Logins = count() by YearAndWeek
| render table
```

### Succesful logins by month

```kql
let MonthNames = dynamic(["", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"]);
customEvents
| where name == "ActiveLogin_BankId_AspNetAuthenticateSuccess"
| project
    timestamp,
    Year = datetime_part("Year", timestamp),
    Month = datetime_part("Month", timestamp)
| extend
    YearAndMonth = strcat(Year, ' ' , tostring(MonthNames[Month]))
| order by Year, Month
| summarize Logins = count() by YearAndMonth
| render table
```

### Success details

```kql
customEvents
| where name startswith "ActiveLogin_BankId_"
| project
    timestamp,
    Event_ShortName = substring(name, 19),
    Event_TypeId = tostring(customDimensions.AL_Event_TypeId),
    Event_Severity = tostring(customDimensions.AL_Event_Severity),
    BankId_Options_LaunchType = tostring(customDimensions.AL_BankId_Options_LaunchType),
    BankId_Options_UseQrCode = tostring(customDimensions.AL_BankId_Options_UseQrCode),
    BankId_OrderRef = tostring(customDimensions.AL_BankId_OrderRef),
    BankId_CollectHintCode = tostring(customDimensions.AL_BankId_CollectHintCode),
    BankId_User_CertNotBefore = tostring(customDimensions.AL_BankId_User_CertNotBefore),
    BankId_User_CertNotAfter = tostring(customDimensions.AL_BankId_User_CertNotAfter),
    BankId_User_DeviceIpAddress = tostring(customDimensions.AL_BankId_User_DeviceIpAddress),
    User_Device_Browser = tostring(customDimensions.AL_User_Device_Browser),
    User_Device_Os = tostring(customDimensions.AL_User_Device_Os),
    User_Device_Type = tostring(customDimensions.AL_User_Device_Type),
    User_Device_OsVersion = tostring(customDimensions.AL_User_Device_OsVersion),
    User_Name = tostring(customDimensions.AL_User_Name),
    User_GivenName = tostring(customDimensions.AL_User_GivenName),
    User_Surname = tostring(customDimensions.AL_User_Surname),
    User_SwedishPersonalIdentityNumber = tostring(customDimensions.AL_User_SwedishPersonalIdentityNumber),
    User_DateOfBirthHint = tostring(customDimensions.AL_User_DateOfBirthHint),
    User_AgeHint = tostring(customDimensions.AL_User_AgeHint),
    User_GenderHint = tostring(customDimensions.AL_User_GenderHint),
    ProductName = tostring(customDimensions.AL_ProductName),
    ProductVersion = tostring(customDimensions.AL_ProductVersion),
    BankId_ApiEnvironment = tostring(customDimensions.AL_BankId_ApiEnvironment),
    BankId_ApiVersion = tostring(customDimensions.AL_BankId_ApiVersion)
| where Event_Severity == "Success"
| order by timestamp desc
| render table
```

## Errors

### Error chart - By event and error code

```kql
customEvents
| where name startswith "ActiveLogin_BankId_"
| project
    timestamp,
    Event_ShortName = substring(name, 19),
    BankId_ErrorCode = tostring(customDimensions.AL_BankId_ErrorCode),
    EventSeverity = tostring(customDimensions.AL_Event_Severity)
| where EventSeverity == "Failure" or EventSeverity == "Error"
| extend
    EventAndErrorCode = strcat(Event_ShortName, ' - ', BankId_ErrorCode)
| summarize count() by bin(timestamp, 1d), EventAndErrorCode
| render columnchart
```

### Error chart - By error code

```kql
customEvents
| where name startswith "ActiveLogin_BankId_"
| project
    timestamp,
    ErrorCode = tostring(customDimensions.AL_BankId_ErrorCode),
    EventSeverity = tostring(customDimensions.AL_Event_Severity)
| where EventSeverity == "Failure" or EventSeverity == "Error"
| summarize count() by bin(timestamp, 1d), ErrorCode
| render columnchart
```

### Error details

```kql
customEvents
| where name startswith "ActiveLogin_BankId_"
| project
    timestamp,
    Event_ShortName = substring(name, 19),
    Event_TypeId = tostring(customDimensions.AL_Event_TypeId),
    Event_Severity = tostring(customDimensions.AL_Event_Severity),
    Error_ErrorReason = tostring(customDimensions.AL_Error_ErrorReason),
    BankId_Options_LaunchType = tostring(customDimensions.AL_BankId_Options_LaunchType),
    BankId_Options_UseQrCode = tostring(customDimensions.AL_BankId_Options_UseQrCode),
    BankId_ErrorCode = tostring(customDimensions.AL_BankId_ErrorCode),
    BankId_ErrorDetails = tostring(customDimensions.AL_BankId_ErrorDetails),
    BankId_OrderRef = tostring(customDimensions.AL_BankId_OrderRef),
    BankId_CollectHintCode = tostring(customDimensions.AL_BankId_CollectHintCode),
    BankId_User_CertNotBefore = tostring(customDimensions.AL_BankId_User_CertNotBefore),
    BankId_User_CertNotAfter = tostring(customDimensions.AL_BankId_User_CertNotAfter),
    BankId_User_DeviceIpAddress = tostring(customDimensions.AL_BankId_User_DeviceIpAddress),
    User_Device_Browser = tostring(customDimensions.AL_User_Device_Browser),
    User_Device_Os = tostring(customDimensions.AL_User_Device_Os),
    User_Device_Type = tostring(customDimensions.AL_User_Device_Type),
    User_Device_OsVersion = tostring(customDimensions.AL_User_Device_OsVersion),
    User_Name = tostring(customDimensions.AL_User_Name),
    User_GivenName = tostring(customDimensions.AL_User_GivenName),
    User_Surname = tostring(customDimensions.AL_User_Surname),
    User_SwedishPersonalIdentityNumber = tostring(customDimensions.AL_User_SwedishPersonalIdentityNumber),
    User_DateOfBirthHint = tostring(customDimensions.AL_User_DateOfBirthHint),
    User_AgeHint = tostring(customDimensions.AL_User_AgeHint),
    User_GenderHint = tostring(customDimensions.AL_User_GenderHint),
    ProductName = tostring(customDimensions.AL_ProductName),
    ProductVersion = tostring(customDimensions.AL_ProductVersion),
    BankId_ApiEnvironment = tostring(customDimensions.AL_BankId_ApiEnvironment),
    BankId_ApiVersion = tostring(customDimensions.AL_BankId_ApiVersion)
| where Event_Severity == "Failure" or Event_Severity == "Error"
| order by timestamp desc
| render table
```

### Errors by error code

```kql
customEvents
| where name startswith "ActiveLogin_BankId_"
| project
    timestamp,
    ErrorCode = tostring(customDimensions.AL_BankId_ErrorCode),
    EventSeverity = tostring(customDimensions.AL_Event_Severity)
| where EventSeverity == "Failure" or EventSeverity == "Error"
| summarize count() by ErrorCode
| render piechart
```

### Errors by type

```kql
customEvents
| where name startswith "ActiveLogin_BankId_"
| project
    Event_ShortName = substring(name, 19),
    Event_Severity = tostring(customDimensions.AL_Event_Severity)
| where Event_Severity == "Failure" or Event_Severity == "Error"
| summarize count() by Event_ShortName
| render piechart
```

## Events

### Events by severity

```kql
customEvents
| where name startswith "ActiveLogin_BankId_"
| project
    timestamp,
    Severity = tostring(customDimensions.AL_Event_Severity)
| summarize count() by bin(timestamp, 1d), Severity
| render columnchart
```
