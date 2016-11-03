# EncompassReports

###Installation

- Save `App.config.sample` as `App.config` and configure with your appropiate settings
- Add references to Encompass drivers according to the SDK Manual

 1. `EllieMae.Encompass.AsmResolver.dll`
 2. `EllieMae.Encompass.Runtime.dll`
 3. `EncompassObjects.dll`

 *EncompassObjects.dll should have "Copy Local" property set to false

View SDK Manual for more details

[EncompassSDK download](http://download.elliemae.com/encompass/updates/16.3.0/encompass163sdk.exe)
*license and active account needed to access sdk

###Compatability
- OS: Windows
- Software: EncompassSDK and license key
- .NET 4
- Recommended Editor: Microsoft Visual Studio

###Usage
`EncompassReports.exe [-r] [-to] [-cc]`
 - -r [integer] report number to run
 - -to [string] to email
 - -cc [string] cc email
 - -bcc [string] bcc email //not yet implemented

###Debug mode
When executing from IDE or without any arguments, program will run in "debug mode"
This means it will not attempt to email the specified report. Instead, it will save the report as `report.html` in the current directory.

###SMTP server
Currently, code is defaulted to use a smtp server at `localhost` to send email.
