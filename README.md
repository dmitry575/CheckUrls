# Checker urls

Simple programm: get urls from file and send `GET` request to url and write the status code of response to output file

## INSTALLATION
```
dotnet publish -c Release -o ./publish
```

If  you want build application to one exe file on windows:

```
dotnet publish -c Release -r win-x64 -o ./publish /p:PublishSingleFile=true /p:PublishTrimmed=true
```

Application parammetrs:

```
  -f, --file     Set input file with urls need check status
  -o, --output   Set out file with result
  -v, --verbose  Set to write all message to console
  --help         Display this help screen.
  ```
