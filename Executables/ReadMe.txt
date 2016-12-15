To Execute from windows explorer:

see: (Configuring the AAI Log Converter.exe.config).

So long as your configuration file defines a valid path for the DefaultSourceDirectory the application will be able to execute simply by running the AAI Log Converter.exe.




To Execute from command line:
Open the command line.

 - without arguments:
	see: (Configuring the AAI Log Converter.exe.config).
	navigate to the directory containing the AAI Log Converter.exe or specify the full file path to the AAI Log Converter.exe
	"AAI Log Converter.exe"

 - with arguments:
The AAI Log Converter may take in any number of file/directory paths as parameters each path passed will be examined for .txt files for conversion.
The DefaultSourceDirectory parameter in the config file will be ignored when the application is called with arguments.
All other parameters (OutputDirectory, ServiceName) will be included in the execution setup.

	navigate to the directory containing the AAI Log Converter.exe or specify the full file path to the AAI Log Converter.exe
	"AAI Log Converter.exe" "Source directory or File path"


Configuring the AAI Log Converter.exe.config

Open the AAI Log Converter.exe.config and specify the value for the DefaultSourceDirectory, OutputDirectory, ServiceName

The DefaultSourceDirectory can not be null, must be a valid directory or file path, and must point to or contain the file or files you would like to convert.

If the OutputDirectory value is left ("") blank/empty, then The Converted logs will be placed in the folder "Converted AAI Logs" in the directory the executable is located.

Unless you are looking to convert only files with a particular name, leave the ServiceName value ("")blank/empty.

Please do not modify or remove anything other than the DefaultSourceDirectory, OutputDirectory, ServiceName values.



For additional help executing applications from the command line please see:
http://www.howtogeek.com/209694/running-an-.exe-file-via-command-prompt/
http://superuser.com/questions/876933/running-exe-in-command-prompt
http://stackoverflow.com/questions/12010103/launch-a-program-from-command-line-without-opening-a-new-window
http://ss64.com/nt/start.html
